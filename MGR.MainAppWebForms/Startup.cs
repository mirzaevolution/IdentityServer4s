using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Linq;
using System.Security.Claims;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(MGR.MainAppWebForms.Startup))]

namespace MGR.MainAppWebForms
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
         
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "Cookies"
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            //{
            //    AuthenticationType = "oidc",
            //    SignInAsAuthenticationType = "Cookies",
            //    Authority = "https://localhost:44385/",
            //    ClientId = "mgr.webform",
            //    ClientSecret = "webform_secret",
            //    RedirectUri = "https://localhost:44308/signin-oidc",
            //    PostLogoutRedirectUri = "https://localhost:44308",
            //    ResponseType = "id_token token",
            //    Scope = "openid profile department offline_access",
            //    UseTokenLifetime = false,
            //    SaveTokens = true,
            //    Notifications = new OpenIdConnectAuthenticationNotifications
            //    {
            //        SecurityTokenValidated = async n =>
            //        {
            //            var claims_to_exclude = new[]
            //            {
            //                "aud", "iss", "nbf", "exp", "nonce", "iat", "at_hash"
            //            };

            //            var claims_to_keep =
            //                n.AuthenticationTicket.Identity.Claims
            //                .Where(x => false == claims_to_exclude.Contains(x.Type)).ToList();
            //            claims_to_keep.Add(new Claim("id_token", n.ProtocolMessage.IdToken));

            //            //if (n.ProtocolMessage.AccessToken != null)
            //            //{
            //            //    claims_to_keep.Add(new Claim("access_token", n.ProtocolMessage.AccessToken));
            //            //    using (HttpClient client = new HttpClient())
            //            //    {
            //            //        client.BaseAddress = new Uri("https://localhost:44385/");
            //            //        var discoveryResponse = await client.GetDiscoveryDocumentAsync();
            //            //        var userInfoResponse = await client.GetUserInfoAsync(new UserInfoRequest
            //            //        {
            //            //            Address = discoveryResponse.UserInfoEndpoint,
            //            //            Token = n.ProtocolMessage.AccessToken
            //            //        });
            //            //        var userInfoClaims = userInfoResponse.Claims
            //            //            .Where(x => x.Type != "sub") // filter sub since we're already getting it from id_token
            //            //            .Select(x => new Claim(x.Type, x.Value));
            //            //        claims_to_keep.AddRange(userInfoClaims);
            //            //    }
            //            //    //var userInfoClient = new UserInfoClient(new Uri("https://localhost:44333/core/connect/userinfo"), n.ProtocolMessage.AccessToken);
            //            //    //var userInfoResponse = await userInfoClient.GetAsync();
            //            //    //var userInfoClaims = userInfoResponse.Claims
            //            //    //    .Where(x => x.Item1 != "sub") // filter sub since we're already getting it from id_token
            //            //    //    .Select(x => new Claim(x.Item1, x.Item2));
            //            //    //claims_to_keep.AddRange(userInfoClaims);
            //            //}

            //            var ci = new ClaimsIdentity(
            //                n.AuthenticationTicket.Identity.AuthenticationType,
            //                "name", "role");
            //            ci.AddClaims(claims_to_keep);

            //            n.AuthenticationTicket = new Microsoft.Owin.Security.AuthenticationTicket(
            //                ci, n.AuthenticationTicket.Properties
            //            );
            //        },
            //        RedirectToIdentityProvider = n =>
            //        {
            //            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
            //            {
            //                var id_token = n.OwinContext.Authentication.User.FindFirst("id_token")?.Value;
            //                n.ProtocolMessage.IdTokenHint = id_token;
            //            }

            //            return Task.FromResult(0);
            //        }
            //    }
            //});
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "oidc",
                SignInAsAuthenticationType = "Cookies",
                Authority = "https://localhost:44385/",
                ClientId = "mgr.mainapp",
                ClientSecret = "mainapp_secret",
                RedirectUri = "https://localhost:44308/signin-oidc",
                PostLogoutRedirectUri = "https://localhost:44308",
                ResponseType = "code id_token token",
                Scope = "openid profile department mgr_mainapi offline_access",
                SaveTokens = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async openIdMessage =>
                    {

                        var claimsToExclude = new[]
                        {
                           "aud", "iss", "nbf", "exp", "nonce", "iat", "at_hash"
                        };

                        var claims =
                            openIdMessage.AuthenticationTicket.Identity.Claims
                            .Where(x => false == claimsToExclude.Contains(x.Type)).ToList();
                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri("https://localhost:44385/");
                            var discoveryResponse = await client.GetDiscoveryDocumentAsync();

                            var responseCodeToken = client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                            {
                                Address = discoveryResponse.TokenEndpoint,
                                ClientId = "mgr.mainapp",
                                ClientSecret = "mainapp_secret",
                                GrantType = "authorization_code",
                                Code = openIdMessage.Code,
                                RedirectUri = openIdMessage.RedirectUri
                            }).Result;

                            if (!string.IsNullOrEmpty(responseCodeToken.IdentityToken) &&
                                !string.IsNullOrEmpty(responseCodeToken.AccessToken) &&
                                !string.IsNullOrEmpty(responseCodeToken.RefreshToken))
                            {
                                claims.Add(new Claim("id_token", responseCodeToken.IdentityToken));
                                claims.Add(new Claim("access_token", responseCodeToken.AccessToken));
                                claims.Add(new Claim("refresh_token", responseCodeToken.RefreshToken));
                            }
                            var claimsIdentity = new ClaimsIdentity(
                                openIdMessage.AuthenticationTicket.Identity.AuthenticationType,
                                "name", "role");
                            claimsIdentity.AddClaims(claims);
                            openIdMessage.AuthenticationTicket = new Microsoft.Owin.Security.AuthenticationTicket(claimsIdentity, openIdMessage.AuthenticationTicket.Properties);
                        }
                    },
                    //SecurityTokenValidated = async openIdMessage =>
                    //{
                        
                    //},
                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var id_token = n.OwinContext.Authentication.User.FindFirst("id_token")?.Value;
                            n.ProtocolMessage.IdTokenHint = id_token;
                        }

                        return Task.FromResult(0);
                    }
                }
            });
            app.UseStageMarker(PipelineStage.Authenticate);
        }
    }
}
