using Microsoft.AspNetCore.Mvc;

namespace WebServerMail.Controllers
{
    public class LetterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
