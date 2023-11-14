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
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });
#endregion


string connection = builderASP.Configuration.GetConnectionString("DefaultConnection")!;
builderASP.Services.AddDbContext<MailDbContext>(options => options.UseSqlServer(connection));

builderASP.Services.AddControllersWithViews();

var app = builderASP.Build();


app.UseAuthentication();
app.UseAuthorization();


app.MapDefaultControllerRoute();



app.Run();




