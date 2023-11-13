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
        public const string LETTERS = "/api/letters/";
        public const string HISTORY = "/api/history/";
        public const string SEND_LETTER = "/api/send/letter";
        public const string USERS = "/api/users/";
        public const string SEARCH = "/api/search/";
        public const string CREATE_USER = "/api/create/user";
    }
}
