



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
        // �������� ������������ �� id
        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (users.Count == 0) return Results.NotFound(new { message = "������������ �� �������" });
        

        // ���� ������������ ������, ���������� ���
        return Results.Json(users);
    }

   
});

app.MapGet("/api/history/{UserId}", (int UserId) =>
{
    using (MailDbContext db = new MailDbContext())
    {

        // �������� ������������ �� id
        List<Letter> letters = db.Letters.Where(x => x.Sender == UserId).ToList();

        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (letters == null) return Results.NotFound(new { message = "������ �� �������" });

        // ���� ������������ ������, ���������� ���
        return Results.Json(letters);
    }


});


app.MapGet("/api/letters/{UserId}", (int UserId) =>
{
    using (MailDbContext db = new MailDbContext())
    {

        // �������� ������������ �� id
        List<Letter> letters = db.Letters.Where(x => x.Recipient == UserId).ToList();

        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (letters == null) return Results.NotFound(new { message = "������ �� �������" });

        // ���� ������������ ������, ���������� ���
        return Results.Json(letters);
    }


});

app.MapGet("/api/users/{id}", (int id) =>
{
    using (MailDbContext db = new MailDbContext())
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
    using (MailDbContext db = new MailDbContext())
    {

        User? user = db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
        // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
        if (user == null) return Results.NotFound(new { message = "������������ �� ������" });

        // ���� ������������ ������, ���������� ���
        return Results.Json(user);
    }


});

app.MapPost("/api/letter", (Letter letter) =>
{
    using (MailDbContext db = new MailDbContext())
    {
        db.Letters.Add(letter);
        db.SaveChanges();
    }
   
});


app.MapPost("/api/create/user", (User user) =>
{
    using (MailDbContext db = new MailDbContext())
    {
        User? existenceCheckUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
        if (existenceCheckUser != null) return Results.BadRequest(new { message = "������������ � ����� ������ ��� ���������������!" });

        db.Users.Add(user);
        db.SaveChanges();
        return Results.Json(user);
    }

});

app.Run();


