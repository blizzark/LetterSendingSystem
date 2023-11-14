using Microsoft.AspNetCore.Mvc;

namespace WebServerMail.Controllers
{
    [Route("api/[controller]")]
    public class LetterController : Controller
    {
        private readonly MailDbContext db;
        public LetterController(MailDbContext context)
        {
            db = context;
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
        [HttpGet("send-letter/{letter}")]
        public void SendLetter(Letter letter)
        {
            db.Letters.Add(letter);
            db.SaveChanges();
        }
    }
}
