using ClientWebApp.Models;
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
        private HttpClient httpClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:5001/");
            httpClient.Timeout = new TimeSpan(0, 0, 10);

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
