using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WebServerMail.Common
{
    [Authorize]
    public class LetterHub : Hub
    {
        public async Task Send(Letter letter)
        {
            // Отправка сообщения клиенту по его userId
            await Clients.User(letter.Recipient.ToString()).SendAsync("letter", letter);
        }
    }

    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
