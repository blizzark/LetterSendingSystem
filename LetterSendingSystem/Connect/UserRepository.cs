using LetterSendingSystem.JsonItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LetterSendingSystem.Entities;
using System.Net.Http.Json;

namespace LetterSendingSystem.Connect
{
    internal static class UserRepository
    {
        public static async Task<User?> Auth(string login, string password)
        {
            using var response = await Request.Client.GetAsync($"{Request.hostName}/api/users/{login}/{password}").ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                // считываем ответ
                var userAndTocken = await response.Content.ReadFromJsonAsync<UserAndTocken>();

                User user = userAndTocken!.User;
                Request.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAndTocken.access_token);
                return user;
            }
            else
            {
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
            }
        }

        public static async Task<User?> GetUser(int UserId)
        {
            using var response = await Request.Client.GetAsync($"{Request.hostName}/api/users/{UserId}").ConfigureAwait(false);
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
            using var response = await Request.Client.GetAsync($"{Request.hostName}/api/search/{searchText}").ConfigureAwait(false);
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

        public static async Task<User?> CreateUser(User user)
        {
            using var response = await Request.Client.PostAsJsonAsync($"{Request.hostName}/api/create/user", user).ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");

            return await response.Content.ReadFromJsonAsync<User>();
        }

    }
}
