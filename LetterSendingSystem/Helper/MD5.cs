using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace LetterSendingSystem.Helper
{
    /// <summary>
    /// Class for creating a password hash
    /// </summary>
    public class MD5
    {

        /// <summary>
        /// Creates a hash by string
        /// </summary>
        /// <param name="input">Usually the password</param>
        /// <returns>hash</returns>
        public static string GetHash(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }
    }
}
