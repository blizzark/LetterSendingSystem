using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LetterSendingSystem.Entities;
using System.Net.Http.Json;
using LetterSendingSystem.Helper;

namespace LetterSendingSystem.Connect
{
    internal static class LetterRepository
    {
        public static async Task<List<Letter>?> GetListUserLetters(int userId)
        {
            using var response = await Request.Client.GetAsync($"{Request.hostName}{Routes.LETTERS}{userId}").ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {

                List<Letter>? letters = await response.Content.ReadFromJsonAsync<List<Letter>>();
                return letters;
            }
            else
            {
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
            }
        }

        public static async Task<List<Letter>?> GetListUserHistory(int userId)
        {

            using var response = await Request.Client.GetAsync($"{Request.hostName}{Routes.HISTORY}{userId}").ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                // считываем ответ
                List<Letter>? letters = await response.Content.ReadFromJsonAsync<List<Letter>>();
                return letters;
            }
            else
            {
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
            }
        }

        public static async Task PostLetter(Letter letter)
        {
            using var response = await Request.Client.PostAsJsonAsync($"{Request.hostName}{Routes.SEND_LETTER}", letter).ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
        }
    }
}
