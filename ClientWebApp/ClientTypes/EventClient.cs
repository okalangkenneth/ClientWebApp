using ClientWebApp.Extensions;
using MyNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientWebApp.ClientTypes
{
    public class EventClient
    {
        private HttpClient client { get; }

        public EventClient(HttpClient client)
        {
            this.client = client;
            this.client.BaseAddress = new Uri("https://localhost:5001/");
            this.client.Timeout = new TimeSpan(0, 0, 40);
            this.client.DefaultRequestHeaders.Clear();
        }

        public async Task<IEnumerable<EventDayDto>> GetEvents()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/events");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    response.EnsureSuccessStatusCode();
                    return stream.ReadAndDeserializeFromJson<IEnumerable<EventDayDto>>();
                }
            }

        }
    }

    public class EventClient2 : BaseClient
    {

        public EventClient2(HttpClient client) : base(client, new Uri("https://localhost:5001/"))
        {
            this.client.Timeout = new TimeSpan(0, 0, 40);
        }

        public async Task<IEnumerable<EventDayDto>> GetAllEvents()
        {
            return await base.Get<IEnumerable<EventDayDto>>("api/events");
        }

        public async Task<EventDayDto> GetEvent(string name)
        {
            return await base.Get<EventDayDto>($"api/events/{name}");
        }

    }
}

