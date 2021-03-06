﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Helpers
{
    public static class HttpHelper
    {
        private const int ClientDefaultBufferSize = 1024 * 1024 * 1024;

        public static HttpClient CreateClient((string name, string value)? header = null)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = ClientDefaultBufferSize };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Chrome/22.0.1229.94");
            if (header != null)
                client.DefaultRequestHeaders.Add(header.Value.name, header.Value.value);
            return client;
        }

        public static Task<HttpResponseMessage> GetResponse(this Uri uri, (string name, string value)? header = null) => CreateClient(header).GetAsync(uri);

        public static async Task<string> GetContent(this Uri uri, (string name, string value)? header = null)
        {
            var response = await uri.GetResponse(header);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<JToken> GetJson(this Uri uri, (string name, string value)? header = null)
        {
            var content = await uri.GetContent(header);
            return JToken.Parse(content);
        }
    }
}