using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebServerMail;
using WebServerMail.Options;

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


app.UseAuthentication();
app.UseAuthorization();


app.MapDefaultControllerRoute();



app.Run();




