using System;
using System.Collections.Generic;

namespace WebServerMail;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Letter> LetterRecipientNavigations { get; set; } = new List<Letter>();

    public virtual ICollection<Letter> LetterSenderNavigations { get; set; } = new List<Letter>();
}
