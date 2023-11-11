using LetterSendingSystem.Entities;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LetterSendingSystem
{
    internal static class ConnectDB
    {
        static HttpClient httpClient = new HttpClient();

        public static async Task<User?> GetUser(string login, string password)
        {
            using var response = await httpClient.GetAsync($"http://localhost:5161/api/users/{login}/{password}").ConfigureAwait(false);
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
    }
}
