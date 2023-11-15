using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterSendingSystem.Entities
{
    /// <summary>
    /// Class for wrapping authorization data
    /// </summary>
    public class RestClient
    {
        [Required(ErrorMessage = MessageConst.EMAIL_NOT_SPECIFIED)]
        [RegularExpression(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", ErrorMessage = MessageConst.MAIL_IN_WRONG_FORMAT)]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = MessageConst.PASSWORD_NOT_SPECIFIED)]
        public string Password { get; set; } = null!;
    }
}
