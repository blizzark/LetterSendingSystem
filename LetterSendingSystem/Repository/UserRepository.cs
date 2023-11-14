using LetterSendingSystem.Entities;
using LetterSendingSystem.Helper;
using LetterSendingSystem.JsonItems;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LetterSendingSystem.Connect
{
    internal static class UserRepository
    {
        public static async Task<User?> Auth(RestClient client)
        {
            using var response = await Request.Post($"{Request.hostName}{Routes.AUTH}", client).ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404

            if (response is null)
                return null;

            var userAndTocken = await response.Content.ReadFromJsonAsync<UserAndToken>();

            User user = userAndTocken!.User;
            Request.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAndTocken.AccessToken);
            return user;

        }

        public static async Task<User?> GetUser(int UserId)
        {
            using var response = await Request.Get($"{Request.hostName}{Routes.USERS}{UserId}").ConfigureAwait(false);

            if (response is null)
                return null;
            return await response.Content.ReadFromJsonAsync<User>();

        }

        public static async Task<List<User>?> GetListUser(string searchText)
        {
            using var response = await Request.Get($"{Request.hostName}{Routes.SEARCH}{searchText}").ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response is null)
                return null;

            return await response.Content.ReadFromJsonAsync<List<User>>();

        }

        public static async Task<User?> CreateUser(User user)
        {
            using var response = await Request.Post($"{Request.hostName}{Routes.CREATE_USER}", user).ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response is null)
                return null;

            User? registerUser = await response.Content.ReadFromJsonAsync<User>();

            if(registerUser is null)
                return null;

            return Auth(new RestClient() { Login = registerUser.Email, Password = registerUser.Password }).Result;
        }

    }
}
