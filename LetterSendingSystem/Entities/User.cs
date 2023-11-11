using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LetterSendingSystem.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string SecondName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public override string ToString() => $"{Email} ({FirstName} {SecondName})";
    }
}
