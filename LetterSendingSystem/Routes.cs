using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterSendingSystem
{
    public static class Routes
    {
        public const string LETTERS = "/api/get-list-user-letters/";
        public const string HISTORY = "/api/get-list-user-history/";
        public const string SEND_LETTER = "/api/send-letter/";
        public const string USERS = "/api/get-user/";
        public const string AUTH = "/api/auth/";
        public const string SEARCH = "/api/get-list-user/";
        public const string CREATE_USER = "/api/create-user/";
    }
}
