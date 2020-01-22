using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
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
                        new AppUserClaims(JwtClaimTypes.PhoneNumber, "085806377218"),
                        new AppUserClaims(Constants.CLAIM_DEV_LANG_NAME,"netcore"),
                        new AppUserClaims(Constants.CLAIM_DEV_PLATFORM_NAME,"win32")
                    }
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }

}
