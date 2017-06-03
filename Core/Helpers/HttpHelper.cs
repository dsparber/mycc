using System;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Helpers
{
    public static class HttpHelper
    {
        private const int ClientDefaultBufferSize = 256000;

        public static HttpClient CreateClient()
        {
            return new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = ClientDefaultBufferSize };
        }

        public static Task<HttpResponseMessage> GetResponse(this Uri uri) => CreateClient().GetAsync(uri);

        public static async Task<string> GetContent(this Uri uri)
        {
            var response = await uri.GetResponse();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<JToken> GetJson(this Uri uri)
        {
            var content = await uri.GetContent();
            return JToken.Parse(content);
        }
    }
}