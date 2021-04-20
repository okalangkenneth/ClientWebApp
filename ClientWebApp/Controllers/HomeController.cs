using ClientWebApp.ClientTypes;
using ClientWebApp.Extensions;
using ClientWebApp.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNamespace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClientWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly EventClient2 eventClient;
        private HttpClient httpClient;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, EventClient2 eventClient)
        {
            _logger = logger;

            //Version 1
            this.httpClientFactory = httpClientFactory;
            this.eventClient = eventClient;
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:5001/");
            httpClient.Timeout = new TimeSpan(0, 0, 50);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        //Get
        public async Task<IActionResult> GetWithShortCut()
        {
            var response = await httpClient.GetAsync("api/events/FromClient");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                var events = JsonConvert.DeserializeObject<IEnumerable<EventDayDto>>(content);
            }
            //else
            //{
            //    var xmlSerialiser = new XmlSerializer(typeof(EventDayDto));
            //    var events = (IEnumerable<EventDayDto>)xmlSerialiser.Deserialize(new StringReader(content));
            //}



            return View();
        }

        //Get with HttpRequest message
        public async Task<IActionResult> HttpRequestMessage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/events");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<IEnumerable<EventDayDto>>(content);

            return View();
        }

        //Post
        public async Task<IActionResult> Post()
        {

            var eventDto = new EventDayDto()
            {
                Name = "FromClient",
                EventDate = new DateTime(2020, 12, 18),
                LocationAddress = "Storgatan 12",
                LocationCityTown = "Stockholm",
                LocationStateProvince = "Stockholm",
                LocationPostalCode = "12345",
                LocationCountry = "Sweden",
                Length = 1,
            };

            //ShortCut
            // var request = await httpClient.PostAsync("api/events", new StringContent(JsonConvert.SerializeObject(eventDto), Encoding.UTF8, "application/json"));

            var jsonData = JsonConvert.SerializeObject(eventDto);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/events");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(jsonData); //, Encoding.Default, "application/json");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdEvent = JsonConvert.DeserializeObject<EventDayDto>(content);

            return View();
        }

        public async Task<IActionResult> Put()
        //public async Task<IActionResult> Index()
        {

            var eventDto = new EventDayDto()
            {
                Name = "FromClient",
                EventDate = new DateTime(2020, 12, 18),
                LocationAddress = "Storgatan 12",
                LocationCityTown = "Stockholm",
                LocationStateProvince = "Stockholm",
                LocationPostalCode = "12345",
                LocationCountry = "Norge",
                Length = 1,
            };

            var jsonData = JsonConvert.SerializeObject(eventDto);

            var request = new HttpRequestMessage(HttpMethod.Put, "api/events/FromClient");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(jsonData); //, Encoding.Default, "application/json");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedEvent = JsonConvert.DeserializeObject<EventDayDto>(content);

            return View();
        }


        public async Task<IActionResult> PatchInsert()
        // public async Task<IActionResult> Index()
        {

            var patchDokument = new JsonPatchDocument<EventDayDto>();

            var lecture = new LectureDto
            {
                Title = "Patch",
                Level = 2,
                SpeakerId = 1
            };


            patchDokument.Add(e => e.Lectures, new List<LectureDto>() { lecture });

            var jsonData = JsonConvert.SerializeObject(patchDokument);

            var request = new HttpRequestMessage(HttpMethod.Patch, "api/events/FromClient");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(jsonData); //, Encoding.Default, "application/json");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedEvent = JsonConvert.DeserializeObject<EventDayDto>(content);

            return View();
        }


        public async Task<IActionResult> Patch()
        //public async Task<IActionResult> Index()
        {

            var patchDokument = new JsonPatchDocument<EventDayDto>();

            // patchDokument.Remove(e => e.Name);
            patchDokument.Remove(e => e.LocationPostalCode);
            patchDokument.Replace(e => e.LocationCountry, "Danmark");
            patchDokument.Add(e => e.Length, 5);

            var jsonData = JsonConvert.SerializeObject(patchDokument);

            var request = new HttpRequestMessage(HttpMethod.Patch, "api/events/FromClient");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(jsonData); //, Encoding.Default, "application/json");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedEvent = JsonConvert.DeserializeObject<EventDayDto>(content);

            return View();
        }

        //Get with Streams
        public async Task<IActionResult> GetWithStreams()
        // public async Task<IActionResult> Index()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/events");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await httpClient.SendAsync(request))
            {

                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                using (var streamReader = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var events = serializer.Deserialize<IEnumerable<EventDayDto>>(jsonTextReader);
                    }
                }
            }

            return View();
        }

        public async Task<IActionResult> GetWithStreamsAndCompletionMode()
        // public async Task<IActionResult> Index()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/events");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {

                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var events = stream.ReadAndDeserializeFromJson<IEnumerable<EventDayDto>>();
            }

            return View();
        }

         public async Task<IActionResult> PostWithStreamAndResponseWithStream()
       // public async Task<IActionResult> Index()
        {

            var eventDto = new EventDayDto()
            {
                Name = "Kalle1",
                EventDate = new DateTime(2020, 12, 18),
                LocationAddress = "Storgatan 12",
                LocationCityTown = "Stockholm",
                LocationStateProvince = "Stockholm",
                LocationPostalCode = "12345",
                LocationCountry = "Sweden",
                Length = 1,
            };

            var memoryStream = new MemoryStream();

            //using(var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(), 1024, true))
            //{
            //    using(var jsonTextWriter = new JsonTextWriter(streamWriter))
            //    {
            //        var jsonSerializer = new JsonSerializer();
            //        jsonSerializer.Serialize(jsonTextWriter, eventDto);
            //        await jsonTextWriter.FlushAsync();
            //    }
            //}

            await memoryStream.WriteAndSerializeToJsonAsync(eventDto);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/events"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var streamContent = new StreamContent(memoryStream))
                {
                    request.Content = streamContent;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        var stream = await response.Content.ReadAsStreamAsync();
                        var cretedEvent = stream.ReadAndDeserializeFromJson<EventDayDto>();
                    }
                }
            }

            return View();
        }

        //Version 1
        public async Task<IActionResult> GetWithHttpClientFactory()
       // public async Task<IActionResult> Index()
        {

            var client = httpClientFactory.CreateClient();
            
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/api/events");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await client.SendAsync(request))
            {

                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                using (var streamReader = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var events = serializer.Deserialize<IEnumerable<EventDayDto>>(jsonTextReader);
                    }
                }
            }

            return View();
        }  
        
        
        //Version 2
        public async Task<IActionResult> GetWithNamedHttpClientFactory()
       // public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient("EventClient");


            var request = new HttpRequestMessage(HttpMethod.Get, "api/events");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                      response.EnsureSuccessStatusCode();
                      var events = stream.ReadAndDeserializeFromJson<IEnumerable<EventDayDto>>();
                }
            }
            return View();
        } 
        
        //Version 3
       // public async Task<IActionResult> GetWithTypedHttpClientFactory()
       //// public async Task<IActionResult> Index()
       // {
       //     var request = new HttpRequestMessage(HttpMethod.Get, "api/events");
       //     request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

       //     using (var response = await eventClient.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
       //     {
       //         using (var stream = await response.Content.ReadAsStreamAsync())
       //         {
       //               response.EnsureSuccessStatusCode();
       //               var events = stream.ReadAndDeserializeFromJson<IEnumerable<EventDayDto>>();
       //         }
       //     }
       //     return View();
       // } 
        
       
        public async Task<IActionResult> Index()
        {
            var events = await eventClient.GetEvent("Kalle1");
            var events1 =await eventClient.GetAllEvents();
            var events2 =await eventClient.Get<IEnumerable<EventDayDto>>("api/events");
            var events3 =await eventClient.Get<EventDayDto>("api/events/kalle1");

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
