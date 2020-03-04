using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TwitterGiveawayPicker.Models;

namespace TwitterGiveawayPicker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string encodedString;
        private string bearerToken;
        private readonly myConfiguration _myConfiguration;

        public HomeController(IOptions<myConfiguration> myConfiguration)
        {
            _myConfiguration = myConfiguration.Value;
        }

        

        public async Task<IActionResult> Index()
        {
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

        public async Task<IActionResult> Authenticate()
        {
            string APIKey = _myConfiguration.APIKey;
            string APISecret = _myConfiguration.APISecret;
            string temp = APIKey + ":" + APISecret;
            encodedString = Base64Encode(temp);

            using(HttpClient client = new HttpClient())
            {
                string url = @"https://api.twitter.com/oauth2/token?grant_type=client_credentials";
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedString);
                var HttpResponse = await client.PostAsync(url, null);
                AuthResponse resp = JsonConvert.DeserializeObject<AuthResponse>(await HttpResponse.Content.ReadAsStringAsync());

                return PartialView("AuthResult", resp);
            }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
