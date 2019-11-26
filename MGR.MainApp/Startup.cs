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
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityModel;
using IdentityModel.Client;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MGR.MainApp
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
            
            //untuk menghapus default claim map dari microsoft
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                //ini utk membuat cookie authentication
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                //ini utk challenge menggunakan openidconnect
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    //url idp
                    options.Authority = "https://localhost:44385/";
                    options.ClientId = "mgr.mainapp";
                    options.ClientSecret = "mainapp_secret";
                    //utk dapatkan related claim info
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    //agar id_token tidak hilang saat refresh page
                    options.SaveTokens = true;
                    //setelah autentikasi, akan mendapatkan id_token (Web) dan access_token + refresh_token (API)
                    options.ResponseType = OidcConstants.ResponseTypes.CodeIdToken;
                    //untuk daftarkan list of scopes/identity resources
                    options.Scope.Add("offline_access");
                    options.Scope.Add("profile");
                    options.Scope.Add("department");

                    options.ClaimActions.MapJsonKey("name", "name");
                    options.ClaimActions.MapJsonKey("role", "role");
                    options.ClaimActions.MapJsonKey("user_department", "user_department");
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminPolicy", policyOptions =>
                {
                    policyOptions.RequireAuthenticatedUser();
                    //policyOptions.RequireRole("Admin");
                    policyOptions.RequireClaim("role", "Admin", "User");
                });
            });
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
