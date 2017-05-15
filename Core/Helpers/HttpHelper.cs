using System;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;

namespace MyCC.Core.Helpers
{
    public static class HttpHelper
    {
        private const int ClientDefaultBufferSize = 256000;

        public static HttpClient CreateClient()
        {
            return new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = ClientDefaultBufferSize };
        }

        public static Task<HttpResponseMessage> GetAsync(Uri uri) => CreateClient().GetAsync(uri);
    }
}