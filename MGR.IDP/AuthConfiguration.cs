using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
namespace MGR.IDP
{
    public class AuthConfiguration
    {
        //identity resource = scope = group ==> must be registered on the client
        //information inside group = claim ==> must be included on the user
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(), //untuk menghasilkan id_token
                new IdentityResources.Profile()
                {
                    UserClaims =
                    {
                        "role"
                    }
                },
                new IdentityResource
                {
                    Name = "department",
                    DisplayName = "Department Information",
                    Description = "Show department related information",
                    UserClaims =
                    {
                        "user_department"
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysIncludeUserClaimsInIdToken = false, //nanti panggil user info endpoint
                    AllowOfflineAccess = true, //refresh_token
                    AllowedGrantTypes =
                    {
                        GrantType.Hybrid,
                        OidcConstants.GrantTypes.RefreshToken
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "department"
                    },
                    ClientId = "mgr.mainapp",
                    ClientSecrets =
                    {
                        new Secret("mainapp_secret".Sha256())
                    },
                    RequireConsent = false,
                    RedirectUris =
                    {
                        "https://localhost:44315/signin-oidc",
                        "https://localhost:44308/signin-oidc"

                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44315/signout-callback-oidc",
                        "https://localhost:44308"

                    }
                },
                new Client
                {
                    ClientId = "mgr.webform",
                    ClientSecrets =
                    {
                        new Secret("webform_secret".Sha256())
                    },
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true, //refresh_token
                    //RequireClientSecret = false,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        "https://localhost:44308/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44308"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "department"
                    },
                    AllowedGrantTypes = 
                    {
                        GrantType.Implicit,
                        OidcConstants.GrantTypes.RefreshToken
                    }
                }
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "12345",
                    Username = "mirzaevolution",
                    Password = "future",
                    Claims =
                    {
                        new Claim("name","Mirza Ghulam Rasyid"),
                        new Claim("role","Admin"),
                        new Claim("role","User"),
                        new Claim("user_department","Software Engineering")
                    }
                },
                new TestUser
                {
                    SubjectId = "54321",
                    Username = "amanda",
                    Password = "future",
                    Claims =
                    {
                        new Claim("name","Amanda"),
                        new Claim("role","User"),
                        new Claim("user_department","Software Engineering")
                    }
                }
            };
        }
    }
}
