using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using Rewind.One.WebApp.Helpers;
using Rewind.One.WebApp.Models;

namespace Rewind.One.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OpenIdConnectOAuthHelper _authHelper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public HomeController(
            OpenIdConnectOAuthHelper authHelper,
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _authHelper = authHelper;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
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
            try
            {
                string expires_at = await HttpContext.GetTokenAsync("expires_at");
                claims.Add(new Tuple<string, string>("expires_at", expires_at));
            }
            catch { }
            claims.AddRange(User.Claims.Select(x => new Tuple<string, string>(x.Type, x.Value)));
            return View(claims);
        }


        [Authorize, Route("/Logout")]
        public IActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

        }
        [Authorize]
        public async Task<IActionResult> GetApiData()
        {
            ViewBag.Message = "";
            HttpClient client = _httpClientFactory.CreateClient("AuthorizedHttpClient");
            HttpResponseMessage response = await client.GetAsync("https://localhost:44356/api/hello");
            if (response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                HelloApiResponse messageObj = JsonConvert.DeserializeObject<HelloApiResponse>(message);

                try
                {
                    ViewBag.Message = messageObj.Message;

                }
                catch
                {
                    ViewBag.Message = $"{response.StatusCode} - {response.ReasonPhrase}";

                }

            }

            else
            {
                //ViewBag.Message = $"{response.StatusCode} - {response.ReasonPhrase}";
                return Redirect("/AccessDenied");

            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetApiDataWithoutExpirationDetector()
        {
            ViewBag.Message = "";
            HttpClient client = _httpClientFactory.CreateClient("GeneralHttpClient");
            string accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.SetBearerToken(accessToken);
            }
            HttpResponseMessage response = await client.GetAsync("https://localhost:44356/api/hello");
            if (response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                HelloApiResponse messageObj = JsonConvert.DeserializeObject<HelloApiResponse>(message);

                try
                {
                    ViewBag.Message = messageObj.Message;

                }
                catch
                {
                    return Redirect("/AccessDenied");

                }

            }

            else
            {
                return Redirect("/AccessDenied");

            }
            return View();
        }

        [Route("/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> RevokeTokens()
        {
            string accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            string refeshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            if(!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refeshToken))
            {
                string baseAddress = _configuration["AuthServer"];
                string clientId = _configuration["ClientId"];
                string clientSecret = _configuration["ClientSecret"];
                HttpClient client = _httpClientFactory.CreateClient("GeneralHttpClient");
                DiscoveryDocumentResponse discoveryDocumentResponse =
                    await client.GetDiscoveryDocumentAsync(
                            baseAddress
                        );
                if(discoveryDocumentResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    var accessTokenRevokeResult = await client.RevokeTokenAsync(new TokenRevocationRequest
                    {
                        Address = discoveryDocumentResponse.RevocationEndpoint,
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Token = accessToken
                        
                    });
                    //if (!accessTokenRevokeResult.IsError)
                    //{
                    //    var refreshTokenRevokeResult = await client.RevokeTokenAsync(new TokenRevocationRequest
                    //    {
                    //        Address = discoveryDocumentResponse.RevocationEndpoint,
                    //        ClientId = clientId,
                    //        ClientSecret = clientSecret,
                    //        Token = refeshToken

                    //    });

                    //}
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
