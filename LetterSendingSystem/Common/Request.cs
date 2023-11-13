using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LetterSendingSystem.Helper
{
    internal static class Request
    {
        public static HttpClient Client = new HttpClient();
        public static string hostName = string.Empty;

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

        public async static Task<HttpResponseMessage> Get(string url)
        {
            return await Client.GetAsync(url).ConfigureAwait(false);
        }

        public async static Task<HttpResponseMessage> Post(string url, object obj)
        {
            return await Client.PostAsJsonAsync(url, obj).ConfigureAwait(false);
        }
    }
}
