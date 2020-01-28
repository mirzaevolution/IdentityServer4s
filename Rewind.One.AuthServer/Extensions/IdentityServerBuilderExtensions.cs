using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using Rewind.One.AuthServer.Services;

namespace Rewind.One.AuthServer.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddPersistentUserService(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<IAuthServerService, AuthServerService>();
            builder.AddProfileService<AuthServerProfileDataService>();
            return builder;
        }
    }
}
