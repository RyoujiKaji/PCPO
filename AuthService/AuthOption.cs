using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService
{
    public class AuthOption
    {
        required public string ISSUER { get; set; }
        required public string AUDIENCE { get; set; }
        public static string KEY { get; set; } = string.Empty;
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
