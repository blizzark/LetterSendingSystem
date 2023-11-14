using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LetterSendingSystem.Helper
{
    /// <summary>
    /// Layer class to simplify access to the server
    /// </summary>
    internal static class Request
    {
        public static HttpClient Client = new HttpClient();
        public static string hostName = string.Empty;

        /// <summary>
        /// Checks the status of the response
        /// </summary>
        /// <param name="response">Response from the server</param>
        /// <returns>Null or response</returns>
        /// <exception cref="System.Exception">Throws an exception if an unexpected error occurs</exception>
        public static HttpResponseMessage? CheckStatus(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;

            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                return response;
            }
            else
            {
                throw new System.Exception($"Ошибка на сервере {response.StatusCode}");
            }
        }

        /// <summary>
        /// Get a request to the server
        /// </summary>
        /// <param name="url">Server link</param>
        /// <returns>Verified answer (null, answer)</returns>
        public async static Task<HttpResponseMessage?> Get(string url)
        {
            return CheckStatus(await Client.GetAsync(url).ConfigureAwait(false));
        }
        /// <summary>
        /// Post a request to the server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns>Verified answer (null, answer)</returns>
        public async static Task<HttpResponseMessage?> Post(string url, object obj)
        {
            return CheckStatus(await Client.PostAsJsonAsync(url, obj).ConfigureAwait(false));
        }
    }
}
