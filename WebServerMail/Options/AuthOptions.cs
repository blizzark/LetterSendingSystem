using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebServerMail.Options
{
    public class AuthOptions
    {
        public const string ISSUER = "localhost"; // издатель токена
        public const string AUDIENCE = "LetterSendingSystem"; // потребитель токена
        const string KEY = "7hHLsZBS5AsHqsDKBgwj7g";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
