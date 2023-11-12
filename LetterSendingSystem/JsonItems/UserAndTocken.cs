using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetterSendingSystem.Entities;

namespace LetterSendingSystem.JsonItems
{
    public class UserAndTocken
    {
        public string access_token { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
