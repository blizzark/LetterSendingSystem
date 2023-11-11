using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterSendingSystem.Entities
{
    internal class Letter
    {
        public int Id { get; set; }

        public string? Titel { get; set; }

        public int Sender { get; set; }

        public int Recipient { get; set; }

        public DateTime Date { get; set; }

        public string? Text { get; set; }
    }
}
