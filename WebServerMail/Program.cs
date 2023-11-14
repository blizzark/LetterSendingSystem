using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebServerMail;
using WebServerMail.Options;

//#region builderEntity
//var builderEntity = new ConfigurationBuilder();
//// установка пути к текущему каталогу
//builderEntity.SetBasePath(Directory.GetCurrentDirectory());
//// получаем конфигурацию из файла appsettings.json
//builderEntity.AddJsonFile("appsettings.json");
//// создаем конфигурацию
//var config = builderEntity.Build();
//// получаем строку подключения
//string connectionString = config.GetConnectionString("DefaultConnection")!;

//var optionsBuilder = new DbContextOptionsBuilder<MailDbContext>();
//var options = optionsBuilder.UseSqlServer(connectionString).Options;
//#endregion
#region builderASP
var builderASP = WebApplication.CreateBuilder();

builderASP.Services.AddAuthorization();
builderASP.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });
#endregion


// получаем строку подключения из файла конфигурации
string connection = builderASP.Configuration.GetConnectionString("DefaultConnection")!;
// добавляем контекст ApplicationContext в качестве сервиса в приложение
builderASP.Services.AddDbContext<MailDbContext>(options => options.UseSqlServer(connection));

builderASP.Services.AddControllersWithViews();

var app = builderASP.Build();

//app.UseStaticFiles();
//app.UseSession(); //будем использовать сессию


//app.UseRouting(); //подбор нужной конечной точки проходит теперь здесь, как минимум 2 middleware выше выполнятся для любого запроса

app.UseAuthentication();
app.UseAuthorization();


app.MapDefaultControllerRoute();




//app.MapGet("/", () => "Hello World!");

//app.MapGet("/api/search/{searchText}", [Authorize] (string searchText) =>
//{
//    using (MailDbContext db = new MailDbContext(options))
//    {
//        List<User> users = db.Users.Where(x => x.FirstName.Contains(searchText) || x.SecondName.Contains(searchText) || x.Email.Contains(searchText)).ToList();
//        //List<User> users = db.Users.Where(x => x.FirstName.StartsWith(searchText) || x.SecondName.StartsWith(searchText) || x.Email.StartsWith(searchText)).ToList();
//        if (users.Count == 0) return Results.NotFound(new { message = "Пользователи не найдены" });


//        // если пользователь найден, отправляем его
//        return Results.Json(users);
//    }


//});

//app.MapGet("/api/history/{UserId}", [Authorize] (int UserId) =>
//{
//    using (MailDbContext db = new MailDbContext(options))
//    {
//        var letters = from letter in db.Letters
//                      join user in db.Users on letter.Sender equals user.Id
//                      where user.Id == UserId
//                      orderby letter.Date descending
//                      select new
//                      {
//                          EmailSender = db.Users.FirstOrDefault(x => x.Id == letter.Recipient)!.Email,
//                          Titel = letter.Titel,
//                          Text = letter.Text,
//                          Date = letter.Date
//                      };

//        if (letters == null) return Results.NotFound(new { message = "Письма не найдены" });

//        return Results.Json(letters.ToList());
//    }
//});


//app.MapGet("/api/letters/{UserId}", [Authorize] (int UserId) =>
//{
//    using (MailDbContext db = new MailDbContext(options))
//    {
//        var letters = from letter in db.Letters
//                      join user in db.Users on letter.Recipient equals user.Id
//                      where user.Id == UserId
//                      orderby letter.Date descending
//                      select new
//                      {
//                          EmailSender = db.Users.FirstOrDefault(x => x.Id == letter.Sender)!.Email,
//                          Titel = letter.Titel,
//                          Text = letter.Text,
//                          Date = letter.Date
//                      };

//        if (letters == null) return Results.NotFound(new { message = "Письма не найдены" });

//        return Results.Json(letters.ToList());
//    }


//});

//app.MapGet("/api/users/{id}", [Authorize] (int id) =>
//{
//    using (MailDbContext db = new MailDbContext(options))
//    {

//        // получаем пользователя по id
//        User? user = db.Users.Find(id);
//        // если не найден, отправляем статусный код и сообщение об ошибке
//        if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

//        // если пользователь найден, отправляем его
//        return Results.Json(user);
//    }


//});

//app.MapGet("/api/users/{login}/{password}", (string login, string password) =>
//{
//    using (MailDbContext db = new MailDbContext(options))
//    {

//        User? userCheck = db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
//        // если не найден, отправляем статусный код и сообщение об ошибке
//        if (userCheck is null) return Results.Unauthorized();

//        var claims = new List<Claim> { new Claim(ClaimTypes.Name, userCheck.Email) };
//        // создаем JWT-токен
//        var jwt = new JwtSecurityToken(
//                issuer: AuthOptions.ISSUER,
//                audience: AuthOptions.AUDIENCE,
//                claims: claims,
//                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
//                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
//        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

//        // формируем ответ
//        var response = new
//        {
//            access_token = encodedJwt,
//            user = userCheck
//        };

//        return Results.Json(response);
//    }


//});

//app.MapPost("/api/send/letter", [Authorize] (Letter letter) =>
//{
//    using (MailDbContext db = new MailDbContext(options))
//    {
//        db.Letters.Add(letter);
//        db.SaveChanges();
//    }

//});


//app.MapPost("/api/create/user", [Authorize] (User user) =>
//{
//    using (MailDbContext db = new MailDbContext(options))
//    {
//        User? existenceCheckUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
//        if (existenceCheckUser != null) return Results.BadRequest(new { message = "Пользователь с такой почтой уже зарегистрирован!" });

//        db.Users.Add(user);
//        db.SaveChanges();
//        return Results.Json(user);
//    }

//});

app.Run();




