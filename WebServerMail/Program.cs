using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebServerMail;
using WebServerMail.Options;

#region builderEntity
var builderEntity = new ConfigurationBuilder();
// установка пути к текущему каталогу
builderEntity.SetBasePath(Directory.GetCurrentDirectory());
// получаем конфигурацию из файла appsettings.json
builderEntity.AddJsonFile("appsettings.json");
// создаем конфигурацию
var config = builderEntity.Build();
// получаем строку подключени€
string connectionString = config.GetConnectionString("DefaultConnection")!;

var optionsBuilder = new DbContextOptionsBuilder<MailDbContext>();
var options = optionsBuilder.UseSqlServer(connectionString).Options;
#endregion
#region builderASP
var builderASP = WebApplication.CreateBuilder();

builderASP.Services.AddAuthorization();
builderASP.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироватьс€ издатель при валидации токена
            ValidateIssuer = true,
            // строка, представл€юща€ издател€
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироватьс€ потребитель токена
            ValidateAudience = true,
            // установка потребител€ токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироватьс€ врем€ существовани€
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидаци€ ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });
#endregion

var app = builderASP.Build();

app.UseAuthentication();
app.UseAuthorization();



app.MapGet("/", () => "Hello World!");

app.MapGet("/api/search/{searchText}", [Authorize] (string searchText) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {
        List<User> users = db.Users.Where(x => x.FirstName.Contains(searchText) || x.SecondName.Contains(searchText) || x.Email.Contains(searchText)).ToList();
        //List<User> users = db.Users.Where(x => x.FirstName.StartsWith(searchText) || x.SecondName.StartsWith(searchText) || x.Email.StartsWith(searchText)).ToList();
        if (users.Count == 0) return Results.NotFound(new { message = "ѕользователи не найдены" });


        // если пользователь найден, отправл€ем его
        return Results.Json(users);
    }


});

app.MapGet("/api/history/{UserId}", [Authorize] (int UserId) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {
        var letters = from letter in db.Letters
                      join user in db.Users on letter.Sender equals user.Id
                      where user.Id == UserId
                      orderby letter.Date descending
                      select new
                      {
                          EmailSender = db.Users.FirstOrDefault(x => x.Id == letter.Recipient)!.Email,
                          Titel = letter.Titel,
                          Text = letter.Text,
                          Date = letter.Date
                      };

        if (letters == null) return Results.NotFound(new { message = "ѕисьма не найдены" });

        return Results.Json(letters.ToList());
    }
});


app.MapGet("/api/letters/{UserId}", [Authorize] (int UserId) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {
        var letters = from letter in db.Letters
                      join user in db.Users on letter.Recipient equals user.Id
                      where user.Id == UserId
                      orderby letter.Date descending
                      select new
                      {
                          EmailSender = db.Users.FirstOrDefault(x => x.Id == letter.Sender)!.Email,
                          Titel = letter.Titel,
                          Text = letter.Text,
                          Date = letter.Date
                      };

        if (letters == null) return Results.NotFound(new { message = "ѕисьма не найдены" });

        return Results.Json(letters.ToList());
    }


});

app.MapGet("/api/users/{id}", [Authorize] (int id) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {

        // получаем пользовател€ по id
        User? user = db.Users.Find(id);
        // если не найден, отправл€ем статусный код и сообщение об ошибке
        if (user == null) return Results.NotFound(new { message = "ѕользователь не найден" });

        // если пользователь найден, отправл€ем его
        return Results.Json(user);
    }


});

app.MapGet("/api/users/{login}/{password}", (string login, string password) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {

        User? userCheck = db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
        // если не найден, отправл€ем статусный код и сообщение об ошибке
        if (userCheck is null) return Results.Unauthorized();

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, userCheck.Email) };
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        // формируем ответ
        var response = new
        {
            access_token = encodedJwt,
            user = userCheck
        };

        return Results.Json(response);
    }


});

app.MapPost("/api/letter", [Authorize] (Letter letter) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {
        db.Letters.Add(letter);
        db.SaveChanges();
    }

});


app.MapPost("/api/create/user", [Authorize] (User user) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {
        User? existenceCheckUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
        if (existenceCheckUser != null) return Results.BadRequest(new { message = "ѕользователь с такой почтой уже зарегистрирован!" });

        db.Users.Add(user);
        db.SaveChanges();
        return Results.Json(user);
    }

});

app.Run();




