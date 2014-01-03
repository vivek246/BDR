using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace BirthdayTimer.Util
{
    public class BirthDayInfo : INotifyPropertyChanged
    {
        #region Properties
        private BitmapImage personImage_;
        /// <summary>
        /// Birthday Person Image
        /// </summary>
        [JsonIgnore]
        public BitmapImage PersonImage
        {
            get
            {
                if (personImage_ != null)
                    return personImage_;
                else if (imageUri != null)
                {
                    personImage_ = new BitmapImage();
                    personImage_.UriSource = imageUri;
                    return personImage_;
                }
                else
                {
                    ReadImageFromIsolatedFile(Name);
                    return personImage_;
                }

            }
            set
            {
                personImage_ = value;
                NotifyPropertyChanged("PersonImage");
            }
        }

        public Uri imageUri
        { get; set; }
        private string name_;
        /// <summary>
        /// Birthday Person Name
        /// </summary>
        public string Name
        {
            get
            {
                return name_;
            }
            set
            {
                if (value != name_)
                {
                    name_ = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string DisplayName
        {
            get { return "Name : " + Name; }
        }

        public string DisplayMonth
        {
            get
            {
                return BirthdayUtil.MonthList[this.Date.Month - 1];
            }
        }
        private DateTime date_ = DateTime.Today;
        /// <summary>
        /// Birthday Person Date
        /// </summary>
        public DateTime Date
        {
            get
            {
                return date_.Date;
            }
            set
            {
                if (value != date_)
                {
                    date_ = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        public string Year
        {
            get
            {
                return (DateTime.Now.Year - Date.Year).ToString() + " year old";
            }
        }
        public string DisplayDate
        {
            get { return "Date : " + Date.Month + "/" + Date.Day + "/" + Date.Year; }
        }

        private string number_;
        /// <summary>
        /// Birthday Related Note
        /// </summary>
        public string Number
        {
            get
            {
                return number_;
            }
            set
            {
                if (value != number_)
                {
                    number_ = value;
                    NotifyPropertyChanged("Number");
                }
            }
        }

        public string DisplayNumber
        {
            get { return "Number : " + Number; }
        }
        private string note_;
        /// <summary>
        /// Birthday Related Note
        /// </summary>
        public string Note
        {
            get
            {
                return note_;
            }
            set
            {
                if (value != note_)
                {
                    note_ = value;
                    NotifyPropertyChanged("Note");
                }
            }
        }

        public string DisplayNote
        {
            get { return "Note : " + Note; }
        }

        public bool GreetingsDone = false;

        public BitmapImage StatusImage
        {
            get
            {
                if (GreetingsDone == true)
                {
                    return new BitmapImage(new Uri("Assets\\Button-Ok-icon.png", UriKind.RelativeOrAbsolute));
                }
                return null;
            }
        }
        #endregion

        internal BirthDayInfo(string name, DateTime date, string number, string note)
        {
            //this.PersonImage = img;
            this.Name = name;
            this.Date = date;
            this.Number = number;
            this.Note = note;

        }

        public BirthDayInfo()
        {
        }

        private BitmapImage ReadImageFromIsolatedFile(string imageName)
        {
            try
            {
                if (!string.IsNullOrEmpty(imageName))
                {
                    personImage_ = new BitmapImage();
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(imageName + ".jpg", FileMode.Open, FileAccess.Read))
                        {
                            personImage_.SetSource(fileStream);
                        }
                    }

                }
                else
                    personImage_ = new BitmapImage(new Uri("\\Assets\\micky.jpg", UriKind.RelativeOrAbsolute));

                return personImage_;
            }
            catch (Exception ex)
            {
                personImage_ = new BitmapImage(new Uri("\\Assets\\micky.jpg", UriKind.RelativeOrAbsolute));
                return personImage_;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
