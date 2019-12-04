using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace MGR.IDP.Entities
{
    public static class MirzaCoreDbContextExtensions
    {
        public static void SeedData(this MirzaCoreDbContext dbContext)
        {
            if (!dbContext.Users.Any())
            {
                List<User> users = new List<User>
                {
                    new User
                    {
                        UserName = "mirzaevolution",
                        Password = "future".Sha256(),
                        Claims = new List<UserClaim>
                        {
                            new UserClaim("name","Mirza Ghulam Rasyid"),
                            new UserClaim("role","Admin"),
                            new UserClaim("role","User"),
                            new UserClaim("user_department","Software Engineering")
                        }
                    },
                    new User
                    {
                        UserName = "amanda",
                        Password = "future".Sha256(),
                        Claims = new List<UserClaim>
                        {
                            new UserClaim("name","Amanda"),
                            new UserClaim("role","User"),
                            new UserClaim("user_department","Software Engineering")
                        }
                    }
                };
                dbContext.Users.AddRange(users);
                dbContext.SaveChanges();
            }
        }
    }
}
