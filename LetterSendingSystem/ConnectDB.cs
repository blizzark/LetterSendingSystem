using LetterSendingSystem.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;

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
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                // считываем ответ
                var userAndTocken = await response.Content.ReadFromJsonAsync<JSON_UserAndTocken>();

                User user = userAndTocken!.User;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAndTocken.access_token);
                return user;
            }
            else
            {
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
            }
        }

        public static async Task<User?> GetInformationOfUser(int UserId)
        {
            using var response = await httpClient.GetAsync($"{hostName}/api/users/{UserId}").ConfigureAwait(false);
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

        public static async Task<List<Letter>?> GetListUserLetters(int userId)
        {
            using var response = await httpClient.GetAsync($"{hostName}/api/letters/{userId}").ConfigureAwait(false);
            
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
            
            using var response = await httpClient.GetAsync($"{hostName}/api/history/{userId}").ConfigureAwait(false);
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
            using var response = await httpClient.PostAsJsonAsync($"{hostName}/api/letter", letter).ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
           if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
        }

        public static async Task<User?> CreateUser(User user)
        {
            using var response = await httpClient.PostAsJsonAsync($"{hostName}/api/create/user", user).ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");

            return await response.Content.ReadFromJsonAsync<User>();
        }
    }
}
