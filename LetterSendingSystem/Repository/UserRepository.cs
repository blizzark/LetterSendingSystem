﻿using LetterSendingSystem.Entities;
using LetterSendingSystem.Helper;
using LetterSendingSystem.JsonItems;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LetterSendingSystem.Connect
{
    /// <summary>
    /// Repository for working with users
    /// </summary>
    internal static class UserRepository
    {
        /// <summary>
        /// Authorization and receiving a token
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<User?> Auth(RestClient client)
        {
            using HttpResponseMessage? response = await Request.Post($"{Request.hostName}{Routes.AUTH}", client).ConfigureAwait(false);


            if (response is null)
                return null;

            UserAndToken? userAndToken = await response.Content.ReadFromJsonAsync<UserAndToken>();

            User user = userAndToken!.User;
            Request.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAndToken.AccessToken);
            return user;

        }
        /// <summary>
        /// Returns the user by his ID
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static async Task<User?> GetUser(int UserId)
        {
            using HttpResponseMessage? response = await Request.Get($"{Request.hostName}{Routes.USERS}{UserId}").ConfigureAwait(false);

            if (response is null)
                return null;
            return await response.Content.ReadFromJsonAsync<User>();

        }

        /// <summary>
        /// Returns a list of users who have an occurrence string in their first/last name/mail
        /// </summary>
        /// <param name="searchText">occurrence string</param>
        /// <returns></returns>
        public static async Task<List<User>?> GetListUser(string searchText)
        {
            using HttpResponseMessage? response = await Request.Get($"{Request.hostName}{Routes.SEARCH}{searchText}").ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if (response is null)
                return null;

            return await response.Content.ReadFromJsonAsync<List<User>>();

        }

        /// <summary>
        /// Creates and immediately authorizes a new user on the server
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<User?> CreateUser(User user)
        {
            using HttpResponseMessage? response = await Request.Post($"{Request.hostName}{Routes.CREATE_USER}", user).ConfigureAwait(false);
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
