using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MGR.WebApp.Helpers
{
    public class IdentityHelpers
    {
        private readonly IHttpContextAccessor _context;
        public IdentityHelpers(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor;
        }
        public async Task<TokenResponse> RenewToken()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44314/");
                var discoveryResponse = await client.GetDiscoveryDocumentAsync();
                string refreshToken = await _context.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
                var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = discoveryResponse.TokenEndpoint,
                    ClientId = "mgr.coreapi",
                    ClientSecret = "secret",
                    GrantType = OpenIdConnectParameterNames.RefreshToken,
                    RefreshToken = refreshToken
                });
                List<AuthenticationToken> authenticationTokens = new List<AuthenticationToken>();
                DateTime expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);

                authenticationTokens.Add(new AuthenticationToken() { Name = OpenIdConnectParameterNames.IdToken, Value = await _context.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken) });
                authenticationTokens.Add(new AuthenticationToken() { Name = OpenIdConnectParameterNames.AccessToken, Value = tokenResponse.AccessToken });
                authenticationTokens.Add(new AuthenticationToken() { Name = OpenIdConnectParameterNames.RefreshToken, Value = tokenResponse.RefreshToken });
                authenticationTokens.Add(new AuthenticationToken() { Name = "expires_at", Value = expiresAt.ToString("o", System.Globalization.CultureInfo.InvariantCulture) });

                AuthenticateResult authenticateResult = await _context.HttpContext.AuthenticateAsync();
                if (authenticateResult.Succeeded)
                {
                    authenticateResult.Properties.StoreTokens(authenticationTokens);

                    await _context.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        authenticateResult.Principal,
                        authenticateResult.Properties);
                }
                return tokenResponse;
            }
        }
    }
}
