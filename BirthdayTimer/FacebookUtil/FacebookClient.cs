using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace FacebookUtils
{
    public class FacebookClient
    {
        private static FacebookClient instance;
        private string accessToken;

        private static readonly IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        private String appId = "301750813296712";
        private String clientSecret = "54ec0653f53a3d7b42b19481de6a9563";
        private String scope = "user_about_me,read_stream";

        public FacebookClient()
        {
            try
            {
                accessToken = (string)appSettings["accessToken"];
            }
            catch (KeyNotFoundException e)
            {
                accessToken = "";
            }
        }

        public static FacebookClient Instance
        {
            get
            {
                if (instance == null)
                    instance = new FacebookClient();
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public string AccessToken
        {
            get
            {
                return accessToken;
            }
            set
            {
                accessToken = value;
                if (accessToken.Equals(""))
                    appSettings.Remove("accessToken");
                else
                    appSettings.Add("accessToken", accessToken);
            }
        }

        public virtual String GetLoginUrl()
        {
            return "https://m.facebook.com/dialog/oauth?client_id=" + appId + "&redirect_uri=https://www.facebook.com/connect/login_success.html&scope=" + scope + "&display=touch";
        }

        public virtual String GetAccessTokenRequestUrl(string code)
        {
            return "https://graph.facebook.com/oauth/access_token?client_id=" + appId + "&redirect_uri=https://www.facebook.com/connect/login_success.html&client_secret=" + clientSecret + "&code=" + code;
        }

        public virtual String GetAccessTokenExchangeUrl(string accessToken)
        {
            return "https://graph.facebook.com/oauth/access_token?client_id=" + appId + "&client_secret=" + clientSecret + "&grant_type=fb_exchange_token&fb_exchange_token=" + accessToken;
        }

        public void PostMessageOnWall(string message, UploadStringCompletedEventHandler handler)
        {
            WebClient client = new WebClient();
            client.UploadStringCompleted += handler;
            client.UploadStringAsync(new Uri("https://graph.facebook.com/me/feed"), "POST", "message=" + HttpUtility.UrlEncode(message) + "&access_token=" + FacebookClient.Instance.AccessToken);
        }

        public void ExchangeAccessToken(UploadStringCompletedEventHandler handler)
        {
            WebClient client = new WebClient();
            client.UploadStringCompleted += handler;
            client.UploadStringAsync(new Uri(GetAccessTokenExchangeUrl(FacebookClient.Instance.AccessToken)), "POST", "");
        }


        internal string GetFriendInformation(string p)
        {
            throw new NotImplementedException();
        }
    }
}
