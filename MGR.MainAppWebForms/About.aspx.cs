using IdentityModel.Client;
using MGR.MainAppWebForms.Helpers;
using MGR.MainAppWebForms.Models;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MGR.MainAppWebForms
{
    public partial class About : Page
    {
        private readonly IOwinContext _owinContext;
        public About()
        {
            if (User.Identity.IsAuthenticated)
            {
                _owinContext = Context.GetOwinContext();
            }
        }
        private TokenResponse RenewToken()
        {
            string idpEndpoint = WebConfigurationManager.AppSettings["AuthServerEndpoint"];
            string clientId = WebConfigurationManager.AppSettings["ClientId"];
            string clientSecret = WebConfigurationManager.AppSettings["ClientSecret"];
            TokenResponse tokenResponse = new OAuthHelper().RenewToken(idpEndpoint: idpEndpoint, clientId: clientId, clientSecret: clientSecret);
            return tokenResponse;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                //var claims = _owinContext.Authentication.User.Claims;
                //var accessToken = claims.FirstOrDefault(c => c.Type == "access_token");
                //accessToken = new System.Security.Claims.Claim("access_token", Guid.NewGuid().ToString());
                //var newClaims = new List<Claim>(claims.Where(c => c.Type != "access_token"));
                //newClaims.Add(accessToken);
                //var claimIdentity = new ClaimsIdentity("Cookies","name","role");
                //claimIdentity.AddClaims(newClaims);
                //_owinContext.Authentication.SignIn(claimIdentity);
                //var name = User.Identity.Name;
                //labelName.InnerText = name;

                using(HttpClient client = new HttpClient())
                {
                    string apiEndpoint = WebConfigurationManager.AppSettings["MainApiServerEndpoint"];
                    string accessToken = _owinContext.Authentication.User.Claims.FirstOrDefault(c => c.Type == "access_token").Value;
                   
                    //test drive renew token
                    //string accessToken = RenewToken().AccessToken;

                    client.BaseAddress = new Uri(apiEndpoint);
                    client.SetBearerToken(accessToken);

                    HttpResponseMessage response = client.GetAsync("api/hello").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        HelloResponse result = JsonConvert.DeserializeObject<HelloResponse>(json);
                        labelName.InnerText = result.Message;
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        var tokenResponse = RenewToken();
                        client.SetBearerToken(tokenResponse.AccessToken);
                        response = client.GetAsync("api/hello").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string json = response.Content.ReadAsStringAsync().Result;
                            HelloResponse result = JsonConvert.DeserializeObject<HelloResponse>(json);
                            labelName.InnerText = result.Message;
                        }
                    }
                }
            }
        }
    }
}