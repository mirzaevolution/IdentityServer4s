using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace MGR.MainAppWebForms.Helpers
{
    public class OAuthHelper
    {
        public TokenResponse RenewToken(string idpEndpoint, string clientId, string clientSecret)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(idpEndpoint);
                IOwinContext owinContext = HttpContext.Current.GetOwinContext();
                
                IEnumerable<Claim> currentClaims = owinContext.Authentication.User.Claims;
                DiscoveryDocumentResponse discoveryResponse = client.GetDiscoveryDocumentAsync().Result;
                
                string refreshToken = currentClaims.FirstOrDefault(c => c.Type == "refresh_token").Value;

                var tokenResponse = client.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = discoveryResponse.TokenEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    GrantType = "refresh_token",
                    RefreshToken = refreshToken
                }).Result;
                List<Claim> newClaims = new List<Claim>(
                        currentClaims.Where(c=> c.Type != "access_token" && c.Type != "refresh_token" && c.Type != "expires_at")
                    );
                
                DateTime expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);

                newClaims.Add(new Claim("access_token", tokenResponse.AccessToken));
                newClaims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
                newClaims.Add(new Claim("expires_at", expiresAt.ToString("o", System.Globalization.CultureInfo.InvariantCulture)));

                var claimIdentity = new ClaimsIdentity("Cookies", "name", "role");
                claimIdentity.AddClaims(newClaims);
                owinContext.Authentication.SignIn(claimIdentity);
                return tokenResponse;
            }
        }
    }
}