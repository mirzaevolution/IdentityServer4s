using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MGR.IDP.Entities
{
    public static class ConfigurationDbContextExtensions
    {
        public static void EnsureSeedDataForContext(this ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in AuthConfiguration.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in AuthConfiguration.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in AuthConfiguration.GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}
