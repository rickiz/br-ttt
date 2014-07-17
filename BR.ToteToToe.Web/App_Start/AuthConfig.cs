using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using BR.ToteToToe.Web.Models;

namespace BR.ToteToToe.Web
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            var fbData = new Dictionary<string, object>();
            fbData.Add("Icon", Links.Images.signin.facebook_sign_in_png);

            OAuthWebSecurity.RegisterFacebookClient(
                appId: "267986850024373",
                appSecret: "846b5689ed1fbb3a2e9443be881de92c",
                displayName: "Facebook",
                extraData: fbData);

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
