using AuthService.Db;
using AuthService.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        readonly private TokenGenerator tokenGenerator;
        readonly private IRepository<UserCredentials> repository;

        public AuthController(IRepository<UserCredentials> repository, TokenGenerator tokenGenerator)
        {
            this.repository = repository;
            this.tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto>> LoginAsync(RequestDto requestDTO)
        {
            try
            {
                var user = await repository.GetByEmailAsync(requestDTO.Email);
                if (user == null) return NotFound();

                if (user.Pass != requestDTO.Pass) return Unauthorized();

                if (user.Id == null)
                {
                    return Problem("Internal server error: user has no ID"); 
                }

                (var access, var refresh) = await tokenGenerator.GenerateTokensAsync(user.Id);

                return Ok(new ResponseDto
                {
                    Token = access,
                    RefreshToken = refresh,
                    Expires = tokenGenerator.Expires
                });
            }
            catch (ArgumentException)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ResponseDto>> RefreshAsync(RefreshRequestDto dto)
        {
            try
            {
                var resp = await tokenGenerator.RefreshTokenAsync(dto.Token, dto.RefreshToken);

                return Ok(new ResponseDto
                {
                    Token = resp.access,
                    RefreshToken = resp.refresh,
                    Expires = tokenGenerator.Expires
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("afrer process");
            }
            catch (BadHttpRequestException)
            {
                return BadRequest();
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto>> CreateAsync(RequestDto dto)
        {
            if (await repository.ExistsAsync(dto.Email)) return BadRequest();

            var user = new UserCredentials
            {
                Email = dto.Email,
                Pass = dto.Pass
            };

            await repository.CreateAsync(user);

            return Created();
        }

        [HttpPatch("changepass")]
        public async Task<ActionResult<ResponseDto>> ChangePassAsync(RequestDto dto) =>
            await repository.ChangePassAsync(dto.Email, dto.Pass) ? NoContent() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) => await repository.DeleteByIdAsync(id) ? NoContent() : NotFound();
    }
}
