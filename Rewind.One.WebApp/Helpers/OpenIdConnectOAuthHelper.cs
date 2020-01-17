using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Rewind.One.WebApp.Helpers
{
    public class OpenIdConnectOAuthHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public OpenIdConnectOAuthHelper(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

        }
        public async Task<string> DetectAndGetToken()
        {
            string token = string.Empty;
            string expiresAtString = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");

            if (!string.IsNullOrEmpty(expiresAtString))
            {
                if (DateTimeOffset.Parse(expiresAtString) < DateTimeOffset.UtcNow)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string baseAddress = _configuration["AuthServer"];
                        string refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(
                                OpenIdConnectParameterNames.RefreshToken
                            );
                        DiscoveryDocumentResponse discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync(baseAddress);
                        if (discoveryDocumentResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {

                            TokenResponse tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                            {
                                ClientId = _configuration["ClientId"],
                                ClientSecret = _configuration["ClientSecret"],
                                Address = discoveryDocumentResponse.TokenEndpoint,
                                RefreshToken = refreshToken,
                                GrantType = OpenIdConnectGrantTypes.RefreshToken
                            });
                            if (tokenResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                            {
                                token = tokenResponse.AccessToken;
                                DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                                List<AuthenticationToken> authenticationTokens = new List<AuthenticationToken>
                                {
                                    new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken) },
                                    new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = tokenResponse.AccessToken },
                                    new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = tokenResponse.RefreshToken},
                                    new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o") }
                                };
                                var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
                                if (authenticationResult.Succeeded)
                                {
                                    authenticationResult.Properties.StoreTokens(authenticationTokens);
                                    await _httpContextAccessor
                                        .HttpContext
                                        .SignInAsync(
                                            CookieAuthenticationDefaults.AuthenticationScheme,
                                            authenticationResult.Principal,
                                            authenticationResult.Properties);
                                }
                                return token;
                            }

                        }
                    }
                }
                else
                {
                    token = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                }
            }

            return token;
        }
    }
}
