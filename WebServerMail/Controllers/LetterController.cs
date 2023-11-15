using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebServerMail.Controllers
{
    [Route("api/")]
    [Authorize]
    public class LetterController : Controller
    {
        private readonly MailDbContext db;
        public LetterController(MailDbContext context)
        {
            db = context;
        }
        [HttpGet("get-list-user-letters/{userId}/{page}")]
        public IResult GetListUserLetters(int userId, int page)
        {
            const int SKIP_TAKE_ELEMENTS = 20;


            var letters = (from letter in db.Letters
                          join user in db.Users on letter.Recipient equals user.Id
                          where user.Id == userId
                          orderby letter.Date descending
                          select new
                          {
                              EmailSender = db.Users.FirstOrDefault(x => x.Id == letter.Sender)!.Email,
                              Titel = letter.Titel,
                              Text = letter.Text,
                              Date = letter.Date
                          }).Skip(page * SKIP_TAKE_ELEMENTS).Take(SKIP_TAKE_ELEMENTS);

            if (letters == null) return Results.NotFound(new { message = "No letters found" });

            return Results.Json(letters.ToList());
        }
        [HttpGet("get-list-user-history/{userId}/{page}")]
        public IResult GetListUserHistory(int userId, int page)
        {
            const int SKIP_TAKE_ELEMENTS = 20;

            var letters = (from letter in db.Letters
                          join user in db.Users on letter.Sender equals user.Id
                          where user.Id == userId
                          orderby letter.Date descending
                          select new
                          {
                              EmailSender = db.Users.FirstOrDefault(x => x.Id == letter.Recipient)!.Email,
                              Titel = letter.Titel,
                              Text = letter.Text,
                              Date = letter.Date
                          }).Skip(page * SKIP_TAKE_ELEMENTS).Take(SKIP_TAKE_ELEMENTS);

            if (letters == null) return Results.NotFound(new { message = "No letters found" });

            return Results.Json(letters.ToList());
        }
        [HttpPost("send-letter/")]
        public void SendLetter([FromBody] Letter letter)
        {
            db.Letters.Add(letter);
            db.SaveChanges();
        }
    }
}
