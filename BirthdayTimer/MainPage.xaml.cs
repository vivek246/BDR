using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BirthdayTimer.Util;
using FacebookIntegration;
using FacebookUtils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;


namespace BirthdayTimer
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.Text = "Loading...";
            ShowHideMethod(true);

            if (isDataPickerOpen == false)
                DataContext = new BirthDayReminderViewModel();

            EmptyBirthdayMessage();
            isDataPickerOpen = false;
            ShowHideMethod(false);
        }

        private void ShowHideMethod(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        private void EmptyBirthdayMessage()
        {
            if ((this.DataContext as BirthDayReminderViewModel).TodayBirthDayList.Count == 0)
                NoBirthday.Visibility = Visibility.Visible;
            else
                NoBirthday.Visibility = Visibility.Collapsed;
        }

        bool isDataPickerOpen = false;

       
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.Uri.ToString().Equals("/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml"))
                isDataPickerOpen = true;
            else
                isDataPickerOpen = false;

        }

        private void SubmitInfo(object sender, RoutedEventArgs e)
        {
            if (nameValue.Text == null)
                MessageBox.Show("Please enter Name");
            else if (phoneNumber.Text == null)
                MessageBox.Show("Please enter Number");
            else
            {
                (this.DataContext as BirthDayReminderViewModel).SubmitedInfo();
                personImage.Title = "Add photo";
                EmptyBirthdayMessage();
            }
        }

        private void SelectedBirthdayInformation(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedVaule = ((BirthDayInfo)(((FrameworkElement)sender).DataContext));
            if (selectedVaule != null)
                DisplayInfo(selectedVaule);
        }

        private void DeleteSelectedBirthdayInfo(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedVaule = ((BirthDayInfo)(((FrameworkElement)sender).DataContext));
            if (selectedVaule != null)
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to delete birthday information?", "Info", MessageBoxButton.OKCancel);
                if (result.Equals(MessageBoxResult.OK))
                {
                    (this.DataContext as BirthDayReminderViewModel).RemoveThisBirthday(selectedVaule);
                    BirthdayUtil.DeleteInfo(selectedVaule);
                }

            }
        }

        private void FetchBirthdayFromContact(object sender, RoutedEventArgs e)
        {
            Contacts phoneBook = new Contacts();
            phoneBook.SearchCompleted += phoneBook_SearchCompleted;
            //Start the asynchronous search.
            phoneBook.SearchAsync(String.Empty, FilterKind.None, "Contacts Test #1");

        }

        void phoneBook_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            IEnumerable<Contact> contacts = (IEnumerable<Contact>)e.Results;

            foreach (Contact contact in contacts)
            {
                IEnumerable<DateTime> items = contact.Birthdays;

                foreach (DateTime dt in items)
                {
                    BirthDayInfo createNew = new BirthDayInfo();
                    createNew.Date = dt;
                    createNew.Name = contact.DisplayName;

                    IEnumerable<ContactPhoneNumber> phoneNumber = contact.PhoneNumbers;
                    foreach (ContactPhoneNumber phone in phoneNumber)
                    {
                        if (createNew.Number == null)
                            createNew.Number = phone.PhoneNumber;
                    }

                    IEnumerable<string> notes = contact.Notes;
                    foreach (String note in notes)
                    {
                        if (createNew.Note == null)
                            createNew.Note = note;
                    }

                    if (contact.GetPicture() != null)
                    {
                        Stream contactPic = contact.GetPicture();
                        createNew.PersonImage.SetSource(contactPic);
                    }
                    BirthdayUtil.StoreInfo(createNew);
                }
            }

            DataContext = new BirthDayReminderViewModel();
            MessageBox.Show("Done");
        }

        private void nameValue_GotFocus(object sender, RoutedEventArgs e)
        {
            increaseHeight.Height = 360;
        }

        private void nameValue_LostFocus(object sender, RoutedEventArgs e)
        {
            increaseHeight.Height = 0;
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedVaule = ((BirthDayInfo)((sender as TextBlock).DataContext));
            if (selectedVaule != null)
            {
                DisplayInfo(selectedVaule);
            }
        }

        private void DisplayInfo(BirthDayInfo selectedVaule)
        {
            BirthdayUtil.SelectedBirthdayInfo = selectedVaule;
            NavigationService.Navigate(new Uri("/Pages/UserInfo.xaml", UriKind.RelativeOrAbsolute));
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

        private void Delete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedVaule = ((BirthDayInfo)(((FrameworkElement)sender).DataContext));
            if (selectedVaule != null)
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to delete birthday information?", "Info", MessageBoxButton.OKCancel);
                if (result.Equals(MessageBoxResult.OK))
                {
                    (this.DataContext as BirthDayReminderViewModel).RemoveThisBirthday(selectedVaule);
                    BirthdayUtil.DeleteInfo(selectedVaule);
                    DataContext = new BirthDayReminderViewModel();
                }

            }
        }

        private void Rate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = "8b9ddb1c-41d5-4031-992f-0bba95860b9c";
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.Show();
        }

        private async void Loginwithfacebook_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ConnectPage.xaml", UriKind.RelativeOrAbsolute));
        }


    }
}