using AuthService.Db;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace AuthService
{
    public class TokenGenerator
    {
        public int Expires { get; } = 300;
        public int RefreshExpires { get; } = 600;

        readonly private IOptions<AuthOption> authOption;
        readonly private IRepository<UserCredentials> repository;

        public TokenGenerator(IOptions<AuthOption> authOption, IRepository<UserCredentials> repository)
        {
            this.authOption = authOption;
            this.repository = repository;
        }

        public async Task<(string access, string refresh)> GenerateTokensAsync(string userId)
        {
            var user = await repository.GetByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not exist");
            if (user.Id == null)
            {
                throw new ArgumentException("UserId not exist");
            }

            return (GenerateAccessToken(user.Id, user.Email), GenerateRefreshToken(user.Id, user.Version));
        }

        private string GenerateAccessToken(string userId, string email)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.NameIdentifier, userId) };
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: authOption.Value.ISSUER,
                    audience: authOption.Value.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromSeconds(Expires)),
                    signingCredentials: new SigningCredentials(AuthOption.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private string GenerateRefreshToken(string userId, int currentVersion)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Version, currentVersion.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: authOption.Value.ISSUER,
                audience: authOption.Value.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromSeconds(RefreshExpires)),
                signingCredentials: new SigningCredentials(AuthOption.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<(string access, string refresh)> RefreshTokenAsync(string expiredAccessToken, string refreshToken)
        {
            var userId = GetUserIdFromToken(expiredAccessToken);
            if (userId == null) throw new BadHttpRequestException("", 400);

            var refreshClaims = DecodeRefreshToken(refreshToken);

            if (RefreshExpires != refreshClaims.expires) throw new UnauthorizedAccessException();
            if (userId != refreshClaims.userId) throw new UnauthorizedAccessException();
            var user = await repository.GetByIdAsync(userId) ?? throw new UnauthorizedAccessException();
            if (user.Id == null)
            {
                throw new ArgumentException("UserId not exist");
            }
            
            if (refreshClaims.version != user.Version)
                throw new UnauthorizedAccessException();

            return (GenerateAccessToken(user.Id, user.Email), GenerateRefreshToken(user.Id, user.Version));
        }

        private static string? GetUserIdFromToken(string token) => new JwtSecurityTokenHandler().ReadJwtToken(token).
            Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        private static (string userId, int version, int expires) DecodeRefreshToken(string token)
        {
            try
            {
                var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;

                var nameIdentifierClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                var versionClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Version);
                var expClaim = claims.FirstOrDefault(c => c.Type == "exp");

                if (nameIdentifierClaim == null)
                    throw new BadHttpRequestException("Missing 'nameidentifier' claim in token", StatusCodes.Status400BadRequest);
        
                if (versionClaim == null)
                    throw new BadHttpRequestException("Missing 'version' claim in token", StatusCodes.Status400BadRequest);

                if (expClaim == null)
                    throw new BadHttpRequestException("Missing 'exp' claim in token", StatusCodes.Status400BadRequest);

                if (!int.TryParse(versionClaim.Value, out var version)) throw new BadHttpRequestException("Invalid 'version' claim format (must be integer)", StatusCodes.Status400BadRequest);

                if (!int.TryParse(expClaim.Value, out var expires))
                    throw new BadHttpRequestException("Invalid 'exp' claim format (must be integer)", StatusCodes.Status400BadRequest);

                return (nameIdentifierClaim.Value, version, expires);
            }
            catch (Exception)
            {
                throw new BadHttpRequestException("", 400);
            }
        }
    }
}
