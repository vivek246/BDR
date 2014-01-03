using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls;

namespace BirthDay
{
    class PersonInfo
    {
        /// <summary>
        /// Birthday Person Image
        /// </summary>
        public Image PersonImage { get; set; }

        /// <summary>
        /// Birthday Person Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Birthday Person Date
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Birthday Related Note
        /// </summary>
        public string Note { get; set; }

        PersonInfo person;

        public PersonInfo(Image img, string name, DateTime? date, string note)
        {
            this.PersonImage = img;
            this.Name = name;
            this.Date = date;
            this.Note = note;

        }

        public static IsolatedStorageFile isoStore
        {
            get { return IsolatedStorageFile.GetUserStoreForApplication(); }
        }

        internal static void StoreInfo(PersonInfo newMember)
        {
            string key = newMember.Name + System.Guid.NewGuid();
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Add(key, newMember);
            settings.Save();
        }
    }
}
