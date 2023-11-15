using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LetterSendingSystem.Entities
{
    /// <summary>
    /// User model in the database
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = MessageConst.FIRST_NAME_NOT_SPECIFIED)]
        [StringLength(20, MinimumLength = 2, ErrorMessage = MessageConst.ILLEGAL_FIRST_NAME_LENGTH + "{2}-{1}")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = MessageConst.SECOND_NAME_NOT_SPECIFIED)]
        [StringLength(20, MinimumLength = 2, ErrorMessage = MessageConst.ILLEGAL_SECOND_NAME_LENGTH + "{2}-{1}")]
        public string SecondName { get; set; } = null!;

        [Required(ErrorMessage = MessageConst.EMAIL_NOT_SPECIFIED)]
        [StringLength(50, MinimumLength = 4, ErrorMessage = MessageConst.ILLEGAL_EMAIL_LENGTH + "{2}-{1}")]
        //[EmailAddress(ErrorMessage = "Почта указана в неверном формате")] - работает плохо
        [RegularExpression(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", ErrorMessage = MessageConst.EMAIL_IS_IN_INCORRECT_FORMAT)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = MessageConst.PASSWORD_NOT_SPECIFIED)]
        [StringLength(50, MinimumLength = 3, ErrorMessage = MessageConst.ILLEGAL_PASSWORD_LENGTH + "{2}-{1}")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = MessageConst.PASSWORD_CONFIRMATION_NOT_SPECIFIED)]
        [Compare("Password", ErrorMessage = MessageConst.PASSWORD_MISMATCH)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = MessageConst.INVALID_PASSWORD_CONFIRMATION_LENGTH + "{2}-{1}")]
        public string ConfirmPassword { get; set; } = null!;

        public override string ToString() => $"{Email} ({FirstName} {SecondName})";
    }
}
