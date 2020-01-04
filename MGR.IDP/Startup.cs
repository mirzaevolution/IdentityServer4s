using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MGR.IDP.Entities;
using MGR.IDP.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Services;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.DbContexts;

namespace MGR.IDP
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDbContext<MirzaCoreDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.AddScoped<IMirzaCoreRepository, MirzaCoreRepository>();
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                //.AddInMemoryIdentityResources(AuthConfiguration.GetIdentityResources())
                //.AddInMemoryClients(AuthConfiguration.GetClients())
                //.AddInMemoryApiResources(AuthConfiguration.GetApiResources())
                //.AddTestUsers(AuthConfiguration.GetTestUsers())
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbcontextOptions =>
                    {
                        dbcontextOptions.UseSqlServer(
                                Configuration.GetConnectionString("DefaultConnection"),
                                sql => sql.MigrationsAssembly(migrationsAssembly)
                        );
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbcontextOptions =>
                    {
                        dbcontextOptions.UseSqlServer(
                                Configuration.GetConnectionString("DefaultConnection"),
                                sql => sql.MigrationsAssembly(migrationsAssembly)
                        );
                    };
                })
                .AddProfileService<MirzaCoreUserProfileService>()
                .AddDeveloperSigningCredential();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            MirzaCoreDbContext context,
            ConfigurationDbContext configurationDbContext,
            PersistedGrantDbContext persistedGrantDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            try
            {
                configurationDbContext.Database.Migrate();
                configurationDbContext.EnsureSeedDataForContext();
                persistedGrantDbContext.Database.Migrate();
                context.Database.Migrate();
                context.SeedData();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
