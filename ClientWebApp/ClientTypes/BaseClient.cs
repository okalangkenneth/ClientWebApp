using ClientWebApp.Extensions;
using MyNamespace;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientWebApp.ClientTypes
{
    public class BaseClient
    {
        protected HttpClient client { get; }

        public BaseClient(HttpClient client)
        {
            this.client = client;
            this.client.DefaultRequestHeaders.Clear();
        }

        public BaseClient(HttpClient client, Uri uri) : this(client)
        {
            this.client.BaseAddress = uri;
        }

        public async Task<T> Get<T>(string route, string contentType = "application/json")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            //Do something
                        }
                    }

                    response.EnsureSuccessStatusCode();
                    return stream.ReadAndDeserializeFromJson<T>();
                }
            }

        }
    }
}