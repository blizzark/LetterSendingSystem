using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using WebServerMail;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

#region builderEntity
var builderEntity = new ConfigurationBuilder();
// ��������� ���� � �������� ��������
builderEntity.SetBasePath(Directory.GetCurrentDirectory());
// �������� ������������ �� ����� appsettings.json
builderEntity.AddJsonFile("appsettings.json");
// ������� ������������
var config = builderEntity.Build();
// �������� ������ �����������
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
            // ���������, ����� �� �������������� �������� ��� ��������� ������
            ValidateIssuer = true,
            // ������, �������������� ��������
            ValidIssuer = AuthOptions.ISSUER,
            // ����� �� �������������� ����������� ������
            ValidateAudience = true,
            // ��������� ����������� ������
            ValidAudience = AuthOptions.AUDIENCE,
            // ����� �� �������������� ����� �������������
            ValidateLifetime = true,
            // ��������� ����� ������������
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // ��������� ����� ������������
            ValidateIssuerSigningKey = true,
        };
    });
#endregion

var app = builderASP.Build();

app.UseAuthentication();
app.UseAuthorization();



app.MapGet("/", [Authorize] () => "Hello World!");

app.MapGet("/api/search/{searchText}", [Authorize] (string searchText) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {
        List<User> users = db.Users.Where(x => x.FirstName.Contains(searchText) || x.SecondName.Contains(searchText) || x.Email.Contains(searchText)).ToList();
        //List<User> users = db.Users.Where(x => x.FirstName.StartsWith(searchText) || x.SecondName.StartsWith(searchText) || x.Email.StartsWith(searchText)).ToList();
        // �������� ������������ �� id
        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (users.Count == 0) return Results.NotFound(new { message = "������������ �� �������" });
        

        // ���� ������������ ������, ���������� ���
        return Results.Json(users);
    }

   
});

app.MapGet("/api/history/{UserId}", [Authorize] (int UserId) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {

        // �������� ������������ �� id
        List<Letter> letters = db.Letters.Where(x => x.Sender == UserId).ToList();

        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (letters == null) return Results.NotFound(new { message = "������ �� �������" });

        // ���� ������������ ������, ���������� ���
        return Results.Json(letters);
    }


});


app.MapGet("/api/letters/{UserId}",[Authorize] (int UserId) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {

   
        List<Letter> letters = db.Letters.Where(x => x.Recipient == UserId).ToList();

        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (letters == null) return Results.NotFound(new { message = "������ �� �������" });

      
        return Results.Json(letters);
    }


});

app.MapGet("/api/users/{id}", [Authorize] (int id) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {

        // �������� ������������ �� id
        User? user = db.Users.Find(id);
        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (user == null) return Results.NotFound(new { message = "������������ �� ������" });

        // ���� ������������ ������, ���������� ���
        return Results.Json(user);
    }


});

app.MapGet("/api/users/{login}/{password}", (string login, string password) =>
{
    using (MailDbContext db = new MailDbContext(options))
    {

        User? userCheck = db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (userCheck is null) return Results.Unauthorized();

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, userCheck.Email) };
        // ������� JWT-�����
        var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        // ��������� �����
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
        if (existenceCheckUser != null) return Results.BadRequest(new { message = "������������ � ����� ������ ��� ���������������!" });

        db.Users.Add(user);
        db.SaveChanges();
        return Results.Json(user);
    }

});

app.Run();


public class AuthOptions
{
    public const string ISSUER = "localhost"; // �������� ������
    public const string AUDIENCE = "LetterSendingSystem"; // ����������� ������
    const string KEY = "7hHLsZBS5AsHqsDKBgwj7g";   // ���� ��� ��������
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}

