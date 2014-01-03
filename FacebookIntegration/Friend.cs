using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookIntegration
{
    public class FbFriendsList
    {
        public string id { get; set; }
        public FriendsInfo friends { get; set; }
    }

    public class FriendsInfo
    {
        public List<Friend> data { get; set; }
    }

    public class Friend
    {
        public string name { get; set; }

        public string birthday { get; set; }

        public string id { get; set; }

        public PicturesInfo picture { get; set; }
        //public PicturesInfo picture { get; set; }
    }

    public class PicturesInfo
    {
        //public List<PictureInfo> data { get; set; }
        public PictureInfo data { get; set; }
    }

    public class PictureInfo
    {
        public string url { get; set; }
        public bool is_silhouette { get; set; }
    }


    public class FacebookData
    {
        private static ObservableCollection<Friend> friends = new ObservableCollection<Friend>();

        public static ObservableCollection<Friend> Friends
        {
            get
            {
                return friends;
            }
        }
    }

}
