using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MGR.WebAppV2.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace MGR.WebAppV2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
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

        [Authorize, Route("/Logout")]
        public IActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

        }
    }
}
