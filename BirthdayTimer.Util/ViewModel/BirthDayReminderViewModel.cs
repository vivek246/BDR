using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace BirthdayTimer.Util
{
    public class BirthDayReminderViewModel : INotifyPropertyChanged
    {
        #region property
        private ObservableCollection<BirthDayInfo> allBirthDayList = new ObservableCollection<BirthDayInfo>();
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BirthDayInfo> AllBirthDayList
        {
            get { return allBirthDayList; }
            set
            {
                allBirthDayList = value;
                NotifyPropertyChanged("AllBirthDayList");
            }

        }

        private ObservableCollection<BirthDayInfo> currentMonthBirthDayList = new ObservableCollection<BirthDayInfo>();
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BirthDayInfo> CurrentMonthBirthDayList
        {
            get { return currentMonthBirthDayList; }
            set
            {
                currentMonthBirthDayList = value;
                NotifyPropertyChanged("CurrentMonthBirthDayList");
            }

        }

        private ObservableCollection<BirthDayInfo> todayBirthDayList = new ObservableCollection<BirthDayInfo>();
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BirthDayInfo> TodayBirthDayList
        {
            get { return todayBirthDayList; }
            set
            {
                todayBirthDayList = value;
                NotifyPropertyChanged("TodayBirthDayList");
            }
        }

        private BirthDayInfo submitBirthDayInfo_;
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public BirthDayInfo SubmitBirthDayInfo
        {
            get { return submitBirthDayInfo_; }
            set
            {
                submitBirthDayInfo_ = value;
                NotifyPropertyChanged("SubmitBirthDayInfo");
            }
        }
        #endregion


        public BirthDayReminderViewModel()
        {
            this.AllBirthDayList = new ObservableCollection<BirthDayInfo>();
            this.TodayBirthDayList = new ObservableCollection<BirthDayInfo>();
            this.CurrentMonthBirthDayList = new ObservableCollection<BirthDayInfo>();
            this.GroupedPeople = new List<AlphaKeyGroup<BirthDayInfo>>();

            this.SubmitBirthDayInfo = new BirthDayInfo();
            this.GetAllBirthDayInformation();

            GroupedPeople = AlphaKeyGroup<BirthDayInfo>.CreateGroups(
                    AllBirthDayList,
                    (BirthDayInfo s) => { return s.DisplayMonth; },
                    true); ;
        }

        public void SubmitedInfo()
        {
            try
            {
                BirthdayUtil.StoreInfo(SubmitBirthDayInfo);
                AddToCurrentList(SubmitBirthDayInfo);
                SubmitBirthDayInfo = new BirthDayInfo();
            }
            catch (Exception)
            {
            }
        }

        private void GetAllBirthDayInformation()
        {
            Dictionary<string, object> birthDayCollection = new Dictionary<string, object>();
            IsolatedStorageSettings value = IsolatedStorageSettings.ApplicationSettings;
            foreach (KeyValuePair<string, object> kvp in value)
            {
                if (kvp.Key.Equals("accessToken")) continue;
                BirthDayInfo infoData = new BirthDayInfo();
                infoData = (BirthDayInfo)JsonConvert.DeserializeObject((string)kvp.Value, typeof(BirthDayInfo));
                AddToCurrentList(infoData);
            }
        }

        private void AddToCurrentList(BirthDayInfo infoData)
        {
            //Get Today Birthday data.
            if (infoData.Date.Day.Equals(DateTime.Now.Day) && infoData.Date.Month.Equals(DateTime.Now.Month))
            {
                this.TodayBirthDayList.Add(infoData);
            }
            else
            {
                infoData.GreetingsDone = false;
            }
            if (infoData.Date.Month.Equals(DateTime.Now.Month))
            {
                this.CurrentMonthBirthDayList.Add(infoData);
            }

            this.AllBirthDayList.Add(infoData);
        }

        public void RemoveThisBirthday(BirthDayInfo selectedVaule)
        {
            this.AllBirthDayList.Remove(selectedVaule);
            this.TodayBirthDayList.Remove(selectedVaule);
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


        private List<AlphaKeyGroup<BirthDayInfo>> groupedPeople_;
        /// <summary>
        /// A collection for Person objects grouped by their first character.
        /// </summary>
        public List<AlphaKeyGroup<BirthDayInfo>> GroupedPeople
        {
            get { return groupedPeople_; }
            set { groupedPeople_ = value; NotifyPropertyChanged("GroupedPeople"); }
        }


    }
}


