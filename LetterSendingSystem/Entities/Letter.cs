using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

        public override string ToString() => $"{ConnectDB.GetInformationOfUser(Sender)?.Result?.Email, -25} ({Titel}) {"",-25} {Date}";
    }
}
