using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LetterSendingSystem.Helper
{
    internal static class Request
    {
        public static HttpClient Client = new HttpClient();
        public static string hostName = string.Empty;
    }
}
