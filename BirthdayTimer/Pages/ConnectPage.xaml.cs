using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BirthdayTimer.Util;
using FacebookIntegration;
using FacebookUtils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Tools;

namespace BirthdayTimer.Pages
{
    public partial class ConnectPage : PhoneApplicationPage
    {
        public ConnectPage()
        {
            InitializeComponent();
            ShowHideMethod(true);
            SystemTray.ProgressIndicator.Text = "Featching your facebook data";
            // Clear Cookie to remove current logged in user data
            mWebBrowser.ClearCookiesAsync();

            // Go to Login url
            mWebBrowser.Source = new Uri(FacebookClient.Instance.GetLoginUrl());
        }

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            String uri = e.Uri.ToString();

            if (uri.StartsWith("https://www.facebook.com/connect/login_success.html") || uri.StartsWith("http://www.facebook.com/connect/login_success.html"))
            {
                // Remove junk text added by facebook from url
                if (uri.EndsWith("#_=_"))
                    uri = uri.Substring(0, uri.Length - 4);

                String queryString = e.Uri.Query.ToString();

                // Acquire the code from Query String
                IEnumerable<KeyValuePair<string, string>> pairs = UriToolKits.ParseQueryString(queryString);
                string code = KeyValuePairUtils.GetValue(pairs, "code");

                // Get access_token from code using Asynchronous HTTP Request
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(AccessTokenDownloadCompleted);
                client.DownloadStringAsync(new Uri(FacebookClient.Instance.GetAccessTokenRequestUrl(code)));
            }
        }

        //friends?fields=id,name,birthday"
        void AccessTokenDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string data = e.Result;
            data = "?" + data;

            // Acquire access_token and expires timestamp
            IEnumerable<KeyValuePair<string, string>> pairs = UriToolKits.ParseQueryString(data);
            string accessToken = KeyValuePairUtils.GetValue(pairs, "access_token");
            string expires = KeyValuePairUtils.GetValue(pairs, "expires");

            // Save access_token
            FacebookClient.Instance.AccessToken = accessToken;

            if (!string.IsNullOrEmpty(FacebookClient.Instance.AccessToken))
            {
                // Get access_token from code using Asynchronous HTTP Request
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetFriendsInformation);
                client.DownloadStringAsync(new Uri("https://graph.facebook.com/me?fields=friends.fields(name,birthday,email,picture)&access_token=" + FacebookClient.Instance.AccessToken));
            }
        }

        private void GetFriendsInformation(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null) return;
                var result = JsonConvert.DeserializeObject(e.Result.ToString(), typeof(FbFriendsList)) as FbFriendsList;
                if (result.friends == null || result.friends.data == null) return;
                foreach (var info in result.friends.data)
                {
                    if (info == null || info.birthday == null) continue;
                    var newInfo = new BirthDayInfo();
                    newInfo.Name = info.name;
                    newInfo.Note = "Facebook contact";
                    string[] splitedDate = info.birthday.Split('/');
                    if (splitedDate.Length == 2)
                        newInfo.Date = new DateTime(DateTime.Now.Year, Convert.ToInt16(splitedDate[0]), Convert.ToInt16(splitedDate[1]));
                    else
                        newInfo.Date = new DateTime(Convert.ToInt16(splitedDate[2]), Convert.ToInt16(splitedDate[0]), Convert.ToInt16(splitedDate[1]));
                    if (info.picture != null && info.picture.data.url != null)
                        newInfo.imageUri = new Uri(info.picture.data.url);
                    BirthdayUtil.StoreInfo(newInfo);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void ShowHideMethod(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

    }
}