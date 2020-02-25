using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Rewind.One.AuthServer.Entities;
namespace Rewind.One.AuthServer.Extensions
{
    public static class AuthServerContextExtensions
    {
        public static void CreateInitUsers(this AuthServerContext context)
        {
            if (!context.Users.Any())
            {
                AppUser user = new AppUser
                {
                    FullName = "Mirza Ghulam Rasyid",
                    UserName = "ghulamcyber@hotmail.com",
                    Password = "future".Sha256(),
                    Claims = new List<AppUserClaims>
                    {
                        new AppUserClaims(JwtClaimTypes.Name,"Mirza Ghulam Rasyid"),
                        new AppUserClaims(JwtClaimTypes.Email, "ghulamcyber@hotmail.com"),
                        new AppUserClaims(JwtClaimTypes.PhoneNumber, "085806377218")
                        //,new AppUserClaims(Constants.CLAIM_DEV_LANG_NAME,"netcore")
                        //,new AppUserClaims(Constants.CLAIM_DEV_PLATFORM_NAME,"win32")
                    },
                    IsActive = true
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
        public static void SeedConfigurations(this ConfigurationDbContext context)
        {
            try
            {
                if (!context.Clients.Any())
                {
                    foreach (var client in InMemoryAuthConfiguration.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                }
                if (!context.ApiResources.Any())
                {
                    foreach (var apiResource in InMemoryAuthConfiguration.GetApiResources())
                    {
                        context.ApiResources.Add(apiResource.ToEntity());
                    }
                }
                if (!context.IdentityResources.Any())
                {
                    foreach (var identityResource in InMemoryAuthConfiguration.GetIdentityResources())
                    {
                        context.IdentityResources.Add(identityResource.ToEntity());
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
    }

}
