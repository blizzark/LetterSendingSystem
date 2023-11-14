using LetterSendingSystem.Entities;
using LetterSendingSystem.Helper;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LetterSendingSystem.Connect
{
    /// <summary>
    /// Repository for working with letters
    /// </summary>
    internal static class LetterRepository
    {
        /// <summary>
        /// Returns a selection of letters received by the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page">Sample page</param>
        /// <returns></returns>
        public static async Task<List<Letter>?> GetListUserLetters(int userId, int page)
        {
            using var response = await Request.Get($"{Request.hostName}{Routes.LETTERS}{userId}/{page}").ConfigureAwait(false);

            if (response is null)
                return null;

            return await response.Content.ReadFromJsonAsync<List<Letter>>();

        }
        /// <summary>
        /// Returns a selection of letters sent by the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static async Task<List<Letter>?> GetListUserHistory(int userId, int page)
        {

            using var response = await Request.Get($"{Request.hostName}{Routes.HISTORY}{userId}/{page}").ConfigureAwait(false);
            // если объект на сервере найден, то есть статусный код равен 404
            if ( response is null)
                return null;


            return await response.Content.ReadFromJsonAsync<List<Letter>>();

        }

        /// <summary>
        /// Sends a letter to the server
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static async Task SendLetter(Letter letter)
        {
            using var response = await Request.Post($"{Request.hostName}{Routes.SEND_LETTER}", letter).ConfigureAwait(false);
        }
    }
}
