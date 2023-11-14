using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetterSendingSystem.Entities;

namespace LetterSendingSystem.JsonItems
{
    public class UserAndToken
    {
        public string AccessToken { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
