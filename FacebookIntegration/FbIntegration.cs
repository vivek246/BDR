using System;
using System.Threading.Tasks;
using System.Windows;
using BirthdayTimer.Util;
using Facebook;
using Facebook.Client;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace FacebookIntegration
{
    public class FbIntegration
    {

        internal static string AccessToken = string.Empty;
        internal static string FacebookId = string.Empty;
        public static bool isAuthenticated = false;
        public static FacebookSessionClient FacebookSessionClient = new FacebookSessionClient(Constants.FacebookAppId);
        private FacebookSession session;

        public async Task FacebookAuthentication()
        {
            if (!isAuthenticated)
            {
                isAuthenticated = true;
                await Authenticate();
            }
            else
            {
                await GetFriendsInformation();
            }
        }

        public async Task Authenticate()
        {
            string message = String.Empty;
            try
            {
                session = await FacebookSessionClient.LoginAsync();
                AccessToken = session.AccessToken;
                FacebookId = session.FacebookId;
                if (AccessToken != null)
                    await GetFriendsInformation();


            }
            catch (InvalidOperationException e)
            {
                message = "Login failed!" + e.Message;
                MessageBox.Show(message);
            }
        }

        public async Task GetFriendsInformation()
        {


        }

        public void GetFriendsInformation(string token)
        {
            FacebookClient fb = new FacebookClient(token);
            fb.GetCompleted += fb_GetCompleted;
            fb.GetTaskAsync(Constants.SerciceRequest);
        }

        void fb_GetCompleted(object sender, FacebookApiEventArgs e)
        {
            try
            {
                if (e.Error != null)
                    return;


                if (e.GetResultData() == null) return;
                var result = JsonConvert.DeserializeObject(e.GetResultData().ToString(), typeof(FbFriendsList)) as FbFriendsList;
                if (result.friends == null || result.friends.data == null) return;
                foreach (var info in result.friends.data)
                {
                    if (info.birthday == null) continue;
                    var newInfo = new BirthDayInfo();
                    newInfo.Name = info.name;
                    newInfo.Note = "Facebook contact";
                    string[] splitedDate = info.birthday.Split('/');
                    if (splitedDate.Length == 2)
                        newInfo.Date = new DateTime(DateTime.Now.Year, Convert.ToInt16(splitedDate[0]), Convert.ToInt16(splitedDate[1]));
                    else
                        newInfo.Date = new DateTime(Convert.ToInt16(splitedDate[2]), Convert.ToInt16(splitedDate[0]), Convert.ToInt16(splitedDate[1]));
                    newInfo.imageUri = new Uri(info.picture.data.url); ;
                    BirthdayUtil.StoreInfo(newInfo);
                }


                // Back to MainPage
                var rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (rootFrame != null)
                    rootFrame.GoBack();

            }
            catch
            {
            }
        }
    }

    class Constants
    {
        public static readonly string FacebookAppId = "301750813296712";
        public static readonly string SerciceRequest = "100000560403131?fields=friends.fields(name,birthday,email,picture)";
    }



}
