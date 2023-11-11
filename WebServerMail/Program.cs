



using System;
using WebServerMail;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/search/{searchText}", (string searchText) =>
{
    using (MailDbContext db = new MailDbContext())
    {
        List<User> users = db.Users.Where(x => x.FirstName.Contains(searchText) || x.SecondName.Contains(searchText) || x.Email.Contains(searchText)).ToList();
        //List<User> users = db.Users.Where(x => x.FirstName.StartsWith(searchText) || x.SecondName.StartsWith(searchText) || x.Email.StartsWith(searchText)).ToList();
        // получаем пользовател€ по id
        // если не найден, отправл€ем статусный код и сообщение об ошибке
        if (users.Count == 0) return Results.NotFound(new { message = "ѕользователи не найдены" });
        

        // если пользователь найден, отправл€ем его
        return Results.Json(users);
    }

   
});

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


