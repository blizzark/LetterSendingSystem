using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LetterSendingSystem.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано имя пользователя")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Недопустимая длина имени {2}-{1}")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Не указана фамилия пользователя")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Недопустимая длина фамилии {2}-{1}")]
        public string SecondName { get; set; } = null!;

        [Required(ErrorMessage = "Не указана почта пользователя")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Недопустимая длина почты {2}-{1}")]
        //[EmailAddress(ErrorMessage = "Почта указана в неверном формате")] - работает плохо
        [RegularExpression(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", ErrorMessage = "Почта указана в неверном формате")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Недопустимая длина пароля {2}-{1}")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Не указано подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Недопустимая длина подтверждения пароля {2}-{1}")]
        public string ConfirmPassword { get; set; } = null!;

        public override string ToString() => $"{Email} ({FirstName} {SecondName})";
    }
}
