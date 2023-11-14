using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebServerMail;
using WebServerMail.Common;
using WebServerMail.Options;


#region builderASP
var builder = WebApplication.CreateBuilder();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // если запрос направлен хабу
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                {
                    // получаем токен из строки запроса
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
#endregion


// получаем строку подключения из файла конфигурации
string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<MailDbContext>(options => options.UseSqlServer(connection));

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();      // подключема сервисы SignalR

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();


app.MapDefaultControllerRoute();


app.MapHub<LetterHub>("/hub");


app.Run();




