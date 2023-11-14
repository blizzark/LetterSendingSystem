using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebServerMail.Options;

namespace WebServerMail.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly MailDbContext db;

        public UserController(MailDbContext context)
        {
            db = context;
        }

        [HttpGet("auth/{login}/{password}")]
        public IActionResult Auth(string login, string password)
        {
            var identity = GetIdentity(login, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid login or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                accessToken = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }

        private ClaimsIdentity? GetIdentity(string login, string password)
        {

                User? userCheck = db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
                
                if (userCheck is null) return null;

            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userCheck.Email),
                };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;

        }


        [HttpGet("get-user/{id}")]
        public IResult GetUser(int id)
        {

                // получаем пользователя по id
                User? user = db.Users.Find(id);
                // если не найден, отправляем статусный код и сообщение об ошибке
                if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });
                // если пользователь найден, отправляем его
                return Results.Json(user);  
        }
        [HttpGet("get-list-user/{searchText}")]
        public IResult GetListUser(string searchText)
        {

            List<User> users = db.Users.Where(x => x.FirstName.Contains(searchText) || x.SecondName.Contains(searchText) || x.Email.Contains(searchText)).ToList();
            //List<User> users = db.Users.Where(x => x.FirstName.StartsWith(searchText) || x.SecondName.StartsWith(searchText) || x.Email.StartsWith(searchText)).ToList();
            if (users.Count == 0) return Results.NotFound(new { message = "Пользователи не найдены" });


            // если пользователь найден, отправляем его
            return Results.Json(users);
        }
        [HttpGet("create-user/{user}")]
        public IResult CreateUser(User user)
        {

            User? existenceCheckUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existenceCheckUser != null) return Results.BadRequest(new { message = "Пользователь с такой почтой уже зарегистрирован!" });

            db.Users.Add(user);
            db.SaveChanges();
            return Results.Json(user);
        }
    }
}
