using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MGR.SecondaryApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using IdentityModel.Client;
using MGR.SecondaryApp.Helpers;

namespace MGR.SecondaryApp.Controllers
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
        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
      
        [Authorize]
        public async Task<IActionResult> Hello()
        {
            using (HttpClient client = new HttpClient())
            {
                string authAddress = _configuration["AuthServerBaseAddress"];
                string baseAddress = _configuration["ApiServerBaseAddress"];
                string clientId = _configuration["ClientId"];
                string clientSecret = _configuration["ClientSecret"];
                string token = await HttpContext.GetTokenAsync("access_token");
                string refreshToken = await HttpContext.GetTokenAsync("refresh_token");

                string expireAt = await HttpContext.GetTokenAsync("expires_at");
                DateTime expiresAtDateTime;
                bool expiresAtParseResult = DateTime.TryParse(expireAt, out expiresAtDateTime);
                if (string.IsNullOrEmpty(expireAt) ||
                   !expiresAtParseResult ||
                   expiresAtDateTime.AddMinutes(-2).ToUniversalTime() < DateTime.UtcNow)
                {
                    var tokenResponse = await _oauthHelper.RenewToken(authAddress, clientId, clientSecret);
                    token = tokenResponse.AccessToken;
                }
                client.BaseAddress = new Uri(baseAddress);
                client.SetBearerToken(token);
                var response = await client.GetAsync("api/hello");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized || 
                        response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return Redirect("/AccessDenied");
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        [Route("/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [Route("/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            using(HttpClient client = new HttpClient())
            {
                string authBaseAddress = _configuration["AuthServerBaseAddress"];
                string clientId = _configuration["ClientId"];
                string clientSecret = _configuration["ClientSecret"];
                string accessToken = await HttpContext.GetTokenAsync("access_token");
                string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                client.BaseAddress = new Uri(authBaseAddress);
                //revoking access token
                DiscoveryDocumentResponse documentResponse = await client.GetDiscoveryDocumentAsync();
                TokenRevocationResponse accessTokenRevokeResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = documentResponse.RevocationEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Token = accessToken,
                    TokenTypeHint = "access_token"
                });

                //revoking refresh token
                TokenRevocationResponse refreshTokenResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = documentResponse.RevocationEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Token = refreshToken,
                    TokenTypeHint = "refresh_token"
                });

            }
            return RedirectToAction("Index", "Home");
            //return SignOut
            //    (
            //        CookieAuthenticationDefaults.AuthenticationScheme,
            //        OpenIdConnectDefaults.AuthenticationScheme
            //    );
        }
    }
}
