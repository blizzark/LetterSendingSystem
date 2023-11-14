using LetterSendingSystem.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using LetterSendingSystem.Helper;

namespace LetterSendingSystem.Socket
{
    internal class LetterSendViewModel
    {
        HubConnection hubConnection;

        public Letter letter;

        public LetterSendViewModel(Letter letter)
        {
            this.letter = letter;
            // создание подключения
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Request.hostName}{Routes.HUB}")
                .Build();
        }

        // Отправка сообщения
        public async Task SendMessage()
        {
            await hubConnection.StartAsync();
            try
            {
                await hubConnection.InvokeAsync("Send", letter);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await hubConnection.StopAsync();
            }
        }
    }
}
