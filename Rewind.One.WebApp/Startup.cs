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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using IdentityModel;
using static IdentityModel.OidcConstants;
using System.IdentityModel.Tokens.Jwt;
using Rewind.One.WebApp.Helpers;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;

namespace Rewind.One.WebApp
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.AccessDeniedPath = new PathString("/AccessDenied");
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.AccessDeniedPath = new PathString("/AccessDenied");
                options.Authority = Configuration["AuthServer"];
                options.ClientId = Configuration["ClientId"];
                options.ClientSecret = Configuration["ClientSecret"];
                options.SaveTokens = true;
                options.Scope.Add(StandardScopes.OpenId);
                options.Scope.Add(StandardScopes.Profile);
                options.Scope.Add(StandardScopes.Email);
                options.Scope.Add(StandardScopes.Phone);
                options.Scope.Add("offline_access");
                options.Scope.Add("crypto_api");
                options.Scope.Add("dev_environment");
                options.GetClaimsFromUserInfoEndpoint = false;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = ResponseTypes.CodeIdToken;

                //options.ClaimActions.MapJsonKey(JwtClaimTypes.Name, JwtClaimTypes.Name);
                //options.ClaimActions.MapJsonKey(JwtClaimTypes.Email, JwtClaimTypes.Name);
                //options.ClaimActions.MapJsonKey(JwtClaimTypes.PhoneNumber, JwtClaimTypes.PhoneNumber);
                //options.ClaimActions.MapJsonKey("dev_prog_lang", "dev_prog_lang");
                //options.ClaimActions.MapJsonKey("dev_platform", "dev_platform");

                options.ClaimActions.MapAll();
             
            });
            services.AddHttpContextAccessor();
            services.AddTransient<OpenIdConnectOAuthHelper>();
            services.AddHttpClient("AuthorizedHttpClient", (serviceProvider, options) =>
             {
                 var helper = serviceProvider.GetRequiredService<OpenIdConnectOAuthHelper>();
                 string token = helper.DetectAndGetToken().ConfigureAwait(false).GetAwaiter().GetResult();
                 if (!string.IsNullOrEmpty(token))
                 {
                     options.SetBearerToken(token);
                 }
             });
            services.AddHttpClient("GeneralHttpClient");
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
