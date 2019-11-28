using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MGR.MainAppWebForms
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                Session.Clear();

                Request.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType, "oidc");
                Request.GetOwinContext().Authentication.SignOut();
            }
            else
            {
                Response.Redirect("~/");
            }
        }
    }
}