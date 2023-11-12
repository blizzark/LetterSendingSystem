using System;
using System.Collections.Generic;


namespace WebServerMail;

public partial class Letter
{
    public int Id { get; set; }

    public string? Titel { get; set; }

    public int Sender { get; set; }

    public int Recipient { get; set; }

    public DateTime Date { get; set; }

    public string? Text { get; set; }

    public virtual User RecipientNavigation { get; set; } = null!;

    public virtual User SenderNavigation { get; set; } = null!;
}
