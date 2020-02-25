using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Rewind.One.AuthServer.Entities;
using Microsoft.EntityFrameworkCore;
using Rewind.One.AuthServer.Extensions;
using Rewind.One.AuthServer.Services;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Rewind.One.AuthServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AuthServerContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            });

            services.AddAuthentication()
                .AddGoogle("Google", options =>
                 {
                     options.ClientId = "1000424342606-inpso6u9jtv60jv8v9r8av04rljqn5uc.apps.googleusercontent.com";
                     options.ClientSecret = "ueFT0tIBkkW0x9hbY09dGYRE";
                     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                 });

            string assemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddInMemoryIdentityResources(InMemoryAuthConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(InMemoryAuthConfiguration.GetApiResources())
                .AddInMemoryClients(InMemoryAuthConfiguration.GetClients())
                //.AddTestUsers(InMemoryAuthConfiguration.GetTestUsers())
                .AddPersistentUserService()
                .AddConfigurationStore<ConfigurationDbContext>(options =>
                {
                    options.ConfigureDbContext = context =>
                    {
                        context.UseSqlServer(Configuration.GetConnectionString("Default"), sql => sql.MigrationsAssembly(assemblyName));
                    };
                })
                .AddOperationalStore<PersistedGrantDbContext>(options =>
                {
                    options.ConfigureDbContext = context =>
                    {
                        context.UseSqlServer(Configuration.GetConnectionString("Default"), sql => sql.MigrationsAssembly(assemblyName));
                    };
                })
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidatorService>()
                //.AddDeveloperSigningCredential();
                .AddSigningCredential(LoadCertificate());
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            AuthServerContext authServerContext,
            ConfigurationDbContext configurationDbContext)
        {
            authServerContext.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                authServerContext.CreateInitUsers();
                configurationDbContext.SeedConfigurations();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        public X509Certificate2 LoadCertificateFromStore()
        {
            string thumbprint = "31D3C2057B473858EA5468EEB090CD488556ACCF";
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);
                if (certCollection.Count == 0)
                {
                    throw new Exception("The specified certificate is not found");
                }
                return certCollection[0];
            }
        }
        public X509Certificate2 LoadCertificate()
        {
            string location = "Certs/MirzaCert.pfx";
            if (File.Exists(location))
            {
                string password = "future";
                X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
                x509Certificate2Collection.Import(location, password, X509KeyStorageFlags.UserKeySet);
                if (x509Certificate2Collection.Count > 0)
                {
                    return x509Certificate2Collection[0];
                }
            }
            throw new FileNotFoundException($"{location} is not found");
        }
    }
}
