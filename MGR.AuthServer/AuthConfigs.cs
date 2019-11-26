using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace MGR.AuthServer
{
    public class AuthConfigs
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResource
                {
                    Name = "job",
                    DisplayName = "Job Informations",
                    Description = "Job related informations",
                    UserClaims =
                    {
                        "job_title",
                        "job_department"
                    }
                }
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                //Web Application
                new Client
                {
                    AllowedGrantTypes = { GrantType.Hybrid, "refresh_token" },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    AllowRememberConsent = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Phone,
                        "job"
                    },
                    ClientId = "mgr.webapp",
                    ClientSecrets =
                    {
                        new Secret("webapp_secret".Sha256()),
                        new Secret("webapp2_secret".Sha256())
                    },
                    RedirectUris =
                    {
                        "https://localhost:44311/signin-oidc",
                        "https://localhost:44365/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44311/signout-callback-oidc",
                        "https://localhost:44365/signout-callback-oidc"
                    },
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    UpdateAccessTokenClaimsOnRefresh = true

                }
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "4950b171-6ecf-4670-9d76-d958a271e7d4",
                    Username = "mirzaevolution",
                    Password = "future",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Email,"ghulamcyber@hotmail.com"),   // ===> Email Scope
                        new Claim(JwtClaimTypes.Name, "Mirza Ghulam Rasyid"),       // ===> Profile Scope
                        new Claim(JwtClaimTypes.PhoneNumber,"085806377218"),        // ===> Phone Scope
                        new Claim("job_title","Pentester"),                         // ===> Job Scope
                        new Claim("job_department", "Cyber Security")               // ===> Job Scope
                    }
                }
            };
        }
    }
}
