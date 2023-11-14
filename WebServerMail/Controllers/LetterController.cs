using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebServerMail.Common;

namespace WebServerMail.Controllers
{
    [Route("api/")]
    [Authorize]
    public class LetterController : Controller
    {
        private readonly MailDbContext db;
        private IHubContext<LetterHub> letterHub;
        public LetterController(MailDbContext context, IHubContext<LetterHub> letterHub)
        {
            db = context;
            this.letterHub = letterHub;
        }
        [HttpGet("get-list-user-letters/{userId}")]
        public IResult GetListUserLetters(int userId)
        {

            var letters = from letter in db.Letters
                          join user in db.Users on letter.Recipient equals user.Id
                          where user.Id == userId
                          orderby letter.Date descending
                          select new
                          {
                              EmailSender = db.Users.FirstOrDefault(x => x.Id == letter.Sender)!.Email,
                              Titel = letter.Titel,
                              Text = letter.Text,
                              Date = letter.Date
                          };

            if (letters == null) return Results.NotFound(new { message = "Письма не найдены" });

            return Results.Json(letters.ToList());
        }
        [HttpGet("get-list-user-history/{userId}")]
        public IResult GetListUserHistory(int userId)
        {

            var letters = from letter in db.Letters
                          join user in db.Users on letter.Sender equals user.Id
                          where user.Id == userId
                          orderby letter.Date descending
                          select new
                          {
                              EmailSender = db.Users.FirstOrDefault(x => x.Id == letter.Recipient)!.Email,
                              Titel = letter.Titel,
                              Text = letter.Text,
                              Date = letter.Date
                          };

            if (letters == null) return Results.NotFound(new { message = "Письма не найдены" });

            return Results.Json(letters.ToList());
        }
        [HttpPost("send-letter/")]
        public void SendLetter([FromBody] Letter letter)
        {

            db.Letters.Add(letter);
            db.SaveChanges();

            letterHub.Clients.User(letter.Recipient.ToString()).SendAsync(letter.Recipient.ToString(), "");
        }
    }
}
