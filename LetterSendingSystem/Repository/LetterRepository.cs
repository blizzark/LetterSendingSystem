﻿using LetterSendingSystem.Entities;
using LetterSendingSystem.Helper;
using LetterSendingSystem.Socket;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LetterSendingSystem.Connect
{
    internal static class LetterRepository
    {
        public static async Task<List<Letter>?> GetListUserLetters(int userId)
        {
            using var response = await Request.Get($"{Request.hostName}{Routes.LETTERS}{userId}").ConfigureAwait(false);

            if (response is null)
                return null;

            return await response.Content.ReadFromJsonAsync<List<Letter>>();

        }

        public static async Task<List<Letter>?> GetListUserHistory(int userId)
        {

            using var response = await Request.Get($"{Request.hostName}{Routes.HISTORY}{userId}").ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if ( response is null)
                return null;


            return await response.Content.ReadFromJsonAsync<List<Letter>>();

        }

        public static async Task SendLetter(Letter letter)
        {
            using var response = await Request.Post($"{Request.hostName}{Routes.SEND_LETTER}", letter).ConfigureAwait(false);
            LetterSendViewModel letterSend = new LetterSendViewModel(letter);
            await letterSend.SendMessage();
        }
    }
}
