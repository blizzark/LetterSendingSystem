using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebServerMail.Options
{
    public class AuthOptions
    {
        public const string ISSUER = "localhost"; 
        public const string AUDIENCE = "LetterSendingSystem"; 
        const string KEY = "7hHLsZBS5AsHqsDKBgwj7g";   
        public const int LIFETIME = 10000;
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
