using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
//using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;

namespace BirthdayTimer.Util
{
    public class BirthdayUtil
    {
        public static List<string> MonthList = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        public static BirthDayInfo SelectedBirthdayInfo;

        public static void StoreInfo(BirthDayInfo newMember)
        {
            try
            {
                string key = newMember.Name;
                string json = JsonConvert.SerializeObject(newMember);

                if (!IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    IsolatedStorageSettings.ApplicationSettings.Add(key, json);
                else
                    IsolatedStorageSettings.ApplicationSettings[key] = json;

                IsolatedStorageSettings.ApplicationSettings.Save();
                if (newMember.Note != "Facebook contact")
                    StoreImageInIsolatedFile(key, newMember.PersonImage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void StoreImageInIsolatedFile(string imageName, BitmapImage bitmapImage)
        {
            try
            {
                if (bitmapImage.UriSource != null)
                {
                    var wb = new WriteableBitmap(bitmapImage);
                    var temp = new MemoryStream();
                    wb.SaveJpeg(temp, wb.PixelWidth, wb.PixelHeight, 0, 100);

                    using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!myIsolatedStorage.FileExists(imageName))
                        {
                            myIsolatedStorage.CreateFile(imageName);
                        }

                        using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(imageName + ".jpg", FileMode.Create, myIsolatedStorage))
                        {
                            fileStream.Write(((MemoryStream)temp).ToArray(), 0, ((MemoryStream)temp).ToArray().Length);
                            fileStream.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void UpdateInfo(BirthDayInfo newMember)
        {
            try
            {
                string key = newMember.Name;
                if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                {
                    string json = JsonConvert.SerializeObject(newMember);
                    IsolatedStorageSettings.ApplicationSettings[key] = json;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
            catch (Exception)
            {

            }
        }

        public static void DeleteInfo(BirthDayInfo deleteMember)
        {
            string key = deleteMember.Name;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
                IsolatedStorageSettings.ApplicationSettings.Save();

            }
        }

        public static void DeleteInfo(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
                IsolatedStorageSettings.ApplicationSettings.Save();

            }
        }

        public static BirthDayReminderViewModel selectedObjectInfo;
        
        public static void StoreBirthDayCollection(BirthDayReminderViewModel birthDayReminderViewModel)
        {
            if (birthDayReminderViewModel != null)
                selectedObjectInfo = birthDayReminderViewModel;
        }

        public static object GetBirthDayCollection()
        {
            return selectedObjectInfo;
        }

        public static void GetTodayBirthdayCollection()
        {
            List<BirthDayInfo> todayBirthList = new List<BirthDayInfo>();
            Dictionary<string, object> birthDayCollection = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kvp in IsolatedStorageSettings.ApplicationSettings)
            {
                BirthDayInfo birthdayInfo = (BirthDayInfo)JsonConvert.DeserializeObject((string)kvp.Value, typeof(BirthDayInfo));
                if (birthdayInfo.Date.Equals(DateTime.Today) && birthdayInfo.GreetingsDone == false)
                    todayBirthList.Add(birthdayInfo);
            }

            string message = TostNotification(todayBirthList);

        }

        private static string TostNotification(List<BirthDayInfo> todayBirthList)
        {
            ShellToast tost = new ShellToast();
            if (todayBirthList.Count > 0 && todayBirthList.Count == 1)
            {
                tost.Content = todayBirthList[0].Name + " has a birthday today.";
            }
            else if (todayBirthList.Count > 1)
            {

                tost.Content = todayBirthList[0].Name + "and" + (todayBirthList.Count - 1) + " others have a birthday today.";
            }

            tost.NavigationUri = new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute);
            tost.Title = "Upcoming Birthday";

            if (todayBirthList.Count > 0)
            {
                tost.Show();
            }

            return tost.Content;
        }


       
    }
}
