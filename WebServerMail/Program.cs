



using System;
using WebServerMail;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/users/{id}", (int id) =>
{
    using (MailDbContext db = new MailDbContext())
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
    using (MailDbContext db = new MailDbContext())
    {

        User? user = db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
        // если не найден, отправл€ем статусный код и сообщение об ошибке
        if (user == null) return Results.NotFound(new { message = "ѕользователь не найден" });

        // если пользователь найден, отправл€ем его
        return Results.Json(user);
    }


});

app.MapPost("/api/letters", (Letter letter) =>
{
    using (MailDbContext db = new MailDbContext())
    {
        db.Letters.Add(letter);
        db.SaveChanges();
    }
   
});

app.Run();


