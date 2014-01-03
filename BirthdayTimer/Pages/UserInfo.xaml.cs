using System;
using System.Windows;
using System.Windows.Navigation;
using BirthdayTimer.Util;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;


namespace BirthdayTimer
{
    public partial class UserInfo : PhoneApplicationPage
    {
        BirthDayInfo CurrentBirthdayInfo = new BirthDayInfo();
        public UserInfo()
        {
            InitializeComponent();
            DataContext = new UserInfoViewModel(BirthdayUtil.SelectedBirthdayInfo);
            CurrentBirthdayInfo = BirthdayUtil.SelectedBirthdayInfo;
            tempImage.Visibility = Visibility.Visible;
            personImage.Visibility = Visibility.Collapsed;
            personImage.Loaded += personImage_Loaded;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode.Equals(NavigationMode.Back))
            {
                DataContext = null;
                DataContext = new UserInfoViewModel(BirthdayUtil.SelectedBirthdayInfo);
                CurrentBirthdayInfo = BirthdayUtil.SelectedBirthdayInfo;
                tempImage.Visibility = Visibility.Visible;
                personImage.Visibility = Visibility.Collapsed;
            }
        }

        void personImage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (((FrameworkElement)sender).Height > 200)
            //{
            ((FrameworkElement)sender).Height = 200;
            ((FrameworkElement)sender).Width = 200;
            //}
            personImage.Visibility = Visibility.Visible;
            tempImage.Visibility = Visibility.Collapsed;
        }

        private void CallBirthDayPerson(object sender, RoutedEventArgs e)
        {
            var task = new PhoneCallTask();
            task.PhoneNumber = CurrentBirthdayInfo.Number;
            task.DisplayName = CurrentBirthdayInfo.Name;
            task.Show();
            CurrentBirthdayInfo.GreetingsDone = true;
            BirthdayUtil.UpdateInfo(CurrentBirthdayInfo);

        }

        private void SMSBirthDayPerson(object sender, RoutedEventArgs e)
        {
            SmsComposeTask task = new SmsComposeTask();
            task.To = CurrentBirthdayInfo.Number;
            task.Body = "Happy Birthday " + CurrentBirthdayInfo.Name;
            task.Show();
            CurrentBirthdayInfo.GreetingsDone = true;
            BirthdayUtil.UpdateInfo(CurrentBirthdayInfo);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/EditUserInfo.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}