using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BirthdayTimer.Util;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace BirthdayTimer
{
    public partial class EditUserInfo : PhoneApplicationPage
    {

        string id = string.Empty;
        public EditUserInfo()
        {
            InitializeComponent();
            DataContext = new UserInfoViewModel(BirthdayUtil.SelectedBirthdayInfo);
            id = BirthdayUtil.SelectedBirthdayInfo.Name;
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.Completed += task_Completed;
            task.Show();
        }

        void task_Completed(object sender, PhotoResult e)
        {
            if (e.ChosenPhoto != null)
            {
                BitmapImage imageSource = new BitmapImage();
                imageSource.SetSource(e.ChosenPhoto);
                personImage.Source = imageSource;
                personImage.Title = string.Empty;
            }
            else
                personImage.Title = "Add photo";
        }


        private void SubmitInfo(object sender, RoutedEventArgs e)
        {
            BirthdayUtil.DeleteInfo(id);
            BirthdayUtil.StoreInfo(BirthdayUtil.SelectedBirthdayInfo);
            NavigationService.GoBack();
        }

        private void nameValue_GotFocus(object sender, RoutedEventArgs e)
        {
            //increaseHeight.Height = 360;
        }

        private void nameValue_LostFocus(object sender, RoutedEventArgs e)
        {
            //increaseHeight.Height = 0;
        }
    }
}