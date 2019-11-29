using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MGR.MainApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using MGR.MainApp.Helpers;
using Microsoft.AspNetCore.Authentication;
using IdentityModel.Client;

namespace MGR.MainApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly OAuthHelper _oauthHelper;
        public HomeController(IConfiguration configuration, OAuthHelper oauthHelper)
        {
            _configuration = configuration;
            _oauthHelper = oauthHelper;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "RequireAdminPolicy")]
        //[Authorize]
        public IActionResult Privacy()
        {
            string name = User.FindFirst(c=>c.Type == "name")?.Value;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/logout")]
        public IActionResult Logout()
        {
            return SignOut
                (
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    OpenIdConnectDefaults.AuthenticationScheme
                );
        }
        [Authorize]
        public async Task<IActionResult> Hello()
        {
            using(HttpClient client = new HttpClient())
            {
                string authAddress = _configuration["AuthServerBaseAddress"];
                string baseAddress = _configuration["ApiServerBaseAddress"];
                string clientId = _configuration["ClientId"];
                string clientSecret = _configuration["ClientSecret"];
                string token = await HttpContext.GetTokenAsync("access_token");
                string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                client.BaseAddress = new Uri(baseAddress);
                client.SetBearerToken(token);
                var response = await client.GetAsync("api/hello");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var tokenResponse = await _oauthHelper.RenewToken(authAddress, clientId,clientSecret);
                    response = await client.GetAsync("api/hello");
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return Ok(result);
                    }
                    else
                        return StatusCode(500);
                }
                else
                {
                    return BadRequest();
                }
            }
        }
    }
}
