using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityModel;
using System;

namespace Rewind.One.AuthServer
{
    public class InMemoryAuthConfiguration
    {

        #region Scope Related Constants
        private const string SCOPE_DEV_ENV_NAME = "dev_environment";
        private const string SCOPE_DEV_ENV_DESC = "Developer Enviroment Scope List";
        #endregion

        #region Claim Related Constants
        private const string CLAIM_DEV_PLATFORM_NAME = "dev_platform";
        private const string CLAIM_DEV_LANG_NAME = "dev_prog_lang";
        #endregion

        #region API Related Constants
        private const string API_CRYPTO_NAME = "crypto_api";
        private const string API_CRYPTO_DESC = "Crypto Api";
        private const string API_CRYPTO_SECRET = "cryptoapisecret";

        #endregion


        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(API_CRYPTO_NAME,API_CRYPTO_DESC)
                {
                    UserClaims =
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email,
                        CLAIM_DEV_LANG_NAME,
                        CLAIM_DEV_PLATFORM_NAME
                    },
                    ApiSecrets =
                    {
                        new Secret(API_CRYPTO_SECRET.Sha256())
                    }
                }
            };
        }
        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResource()
                {
                    Name = SCOPE_DEV_ENV_NAME,
                    Description = SCOPE_DEV_ENV_DESC,
                    DisplayName = SCOPE_DEV_ENV_DESC,
                    UserClaims =
                    {
                        CLAIM_DEV_PLATFORM_NAME,
                        CLAIM_DEV_LANG_NAME
                    }
                }
            };
        }
        public static List<Client> GetClients()
        {
            return new List<Client>
            {
                //api
                new Client
                {
                    ClientId = "rewind_crypto_api",
                    ClientSecrets =
                    {
                        new Secret("apisecret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedCorsOrigins =
                    {
                        "*"
                    },
                    AllowedScopes =
                    {
                        OidcConstants.StandardScopes.OfflineAccess,
                        API_CRYPTO_NAME
                    },
                    AllowOfflineAccess = true
                },

                //web client
                new Client
                {
                    ClientId = "rewind_crypto_webapp",
                    ClientSecrets =
                    {
                        new Secret("webappsecret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes =
                    {
                        OidcConstants.StandardScopes.OpenId,
                        OidcConstants.StandardScopes.Profile,
                        OidcConstants.StandardScopes.Email,
                        OidcConstants.StandardScopes.Phone,
                        API_CRYPTO_NAME,
                        SCOPE_DEV_ENV_NAME
                    },
                    AccessTokenType = AccessTokenType.Reference,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    RedirectUris =
                    {
                        "https://localhost:44388/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44388/signout-callback-oidc"
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
                    SubjectId = "69db740f-5ff2-446c-8dea-66befdcf13db",
                    Username = "mirzaevolution",
                    Password = "future",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name,"Mirza Ghulam Rasyid"),
                        new Claim(JwtClaimTypes.Email, "ghulamcyber@hotmail.com"),
                        new Claim(JwtClaimTypes.PhoneNumber, "085806377218"),
                        new Claim(CLAIM_DEV_LANG_NAME,"netcore"),
                        new Claim(CLAIM_DEV_PLATFORM_NAME,"win32")
                    }
                }
            };
        }
    }
}
