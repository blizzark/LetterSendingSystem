using LetterSendingSystem.Entities;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LetterSendingSystem
{
    internal static class ConnectDB
    {
        static HttpClient httpClient = new HttpClient();
        static string hostName = "http://localhost:5161";
        public static async Task<User?> GetUser(string login, string password)
        {
            using var response = await httpClient.GetAsync($"{hostName}/api/users/{login}/{password}").ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;

            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                // считываем ответ
                User? person = await response.Content.ReadFromJsonAsync<User>();
                return person;
            }
            else
            {
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
            }
        }

        public static async Task<List<User>?> GetListUser(string searchText)
        {
            using var response = await httpClient.GetAsync($"{hostName}/api/search/{searchText}").ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                // считываем ответ
                List<User>? persons = await response.Content.ReadFromJsonAsync<List<User>>();
                return persons;
            }
            else
            {
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
            }
        }

        public static async Task PostLetter(Letter letter)
        {
            using var response = await httpClient.PostAsJsonAsync($"{hostName}/api/letters", letter).ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
           if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
        }
    }
}
