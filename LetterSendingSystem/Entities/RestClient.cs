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
        [Required(ErrorMessage = "Не указана почта пользователя")]
        [RegularExpression(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", ErrorMessage = "Почта указана в неверном формате")]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "Не указан пароль")]
        public string Password { get; set; } = null!;
    }
}
