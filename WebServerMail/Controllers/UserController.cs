using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebServerMail.Entities;
using WebServerMail.Options;

namespace WebServerMail.Controllers
{
    [Route("api/")]
    public class UserController : Controller
    {
        private readonly MailDbContext db;



        public UserController(MailDbContext context)
        {
            db = context;
        }
        [HttpPost("auth/")]
        public IActionResult Auth([FromBody] RestClient client)
        {
            User? user = db.Users.FirstOrDefault(u => u.Email == client.Login && u.Password == client.Password);
            if (user is null)
                return Unauthorized(new { errorText = "Invalid login or password." });

            var identity = GetIdentity(user);

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity!.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                accessToken = encodedJwt,
                user = user
            };

            return Json(response);
        }

        private ClaimsIdentity? GetIdentity(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        [Authorize]
        [HttpGet("get-user/{id}")]
        public IResult GetUser(int id)
        {

              
                User? user = db.Users.Find(id);
             
                if (user == null) return Results.NotFound(new { message = "No users found" });
            
                return Results.Json(user);  
        }

        [Authorize]
        [HttpGet("get-list-user/{searchText}")]
        public IResult GetListUser(string searchText)
        {

            List<User> users = db.Users.Where(x => x.FirstName.Contains(searchText) || x.SecondName.Contains(searchText) || x.Email.Contains(searchText)).Take(5).ToList();
            if (users.Count == 0) return Results.NotFound(new { message = "No users found" });


            return Results.Json(users);
        }

        [HttpPost("create-user/")]
        public IResult CreateUser([FromBody] User user)
        {

            User? existenceCheckUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existenceCheckUser != null) return Results.BadRequest(new { message = "User already exists" });

            db.Users.Add(user);
            db.SaveChanges();
            return Results.Json(user);
        }
    }
}
