using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MGR.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using IdentityModel.Client;
using MGR.WebApp.Helpers;

namespace MGR.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IdentityHelpers _identityHelpers;
        public HomeController(IdentityHelpers identityHelpers)
        {
            _identityHelpers = identityHelpers;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            var user = User.Identity.Name;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [Authorize]
        public async Task<IActionResult> Claims()
        {
            List<Tuple<string, string>> claims = new List<Tuple<string, string>>();
            string idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            string accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            string refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            claims.Add(new Tuple<string, string>(OpenIdConnectParameterNames.IdToken, idToken));
            claims.Add(new Tuple<string, string>(OpenIdConnectParameterNames.AccessToken, accessToken));
            claims.Add(new Tuple<string, string>(OpenIdConnectParameterNames.RefreshToken, refreshToken));
            claims.AddRange(User.Claims.Select(x => new Tuple<string, string>(x.Type, x.Value)));



            return View(claims);
        }


        [Authorize]
        [HttpGet("/getcoreapidata")]
        public async Task<IActionResult> GetCoreApiData()
        {
            string access_token = await HttpContext.GetTokenAsync("access_token");
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(access_token);
                HttpResponseMessage response = await client.GetAsync("https://localhost:44331/api/hello");
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    return Ok(message);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var tokenResponse = await _identityHelpers.RenewToken();

                    client.SetBearerToken(tokenResponse.AccessToken);
                    response = await client.GetAsync("https://localhost:44331/api/hello");
                    if (response.IsSuccessStatusCode)
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        return Ok(message);
                    }
                    return StatusCode(401);
                }
                else
                {
                    return BadRequest();
                }
            }
        }


        [Authorize, Route("/Logout")]
        public IActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

        }

    }
}
