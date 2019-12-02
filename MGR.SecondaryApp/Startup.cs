using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MGR.SecondaryApp.Helpers;

namespace MGR.SecondaryApp
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = new PathString("/AccessDenied");
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration["AuthServerBaseAddress"];
                    options.ClientId = Configuration["ClientId"];
                    options.ClientSecret = Configuration["ClientSecret"];
                    options.GetClaimsFromUserInfoEndpoint = false;
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    options.SaveTokens = true;
                    options.Scope.Add("profile");
                    options.Scope.Add("department");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("mgr_secondaryapi");
                    options.ClaimActions.MapJsonKey("name", "name");
                    options.ClaimActions.MapJsonKey("role", "role");
                    options.ClaimActions.MapJsonKey("user_department", "user_department");
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = tokenInfo =>
                        {
                            var claims = tokenInfo.Principal.Claims.Where(c => c.Type == "role").Select(c => new Claim(ClaimTypes.Role, c.Value));
                            if (claims != null && claims.Any())
                            {
                                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
                                tokenInfo.Principal.AddIdentity(claimsIdentity);
                            }
                            return Task.CompletedTask;
                        }
                    };
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });

            services.AddHttpContextAccessor();
            services.AddTransient<OAuthHelper>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
