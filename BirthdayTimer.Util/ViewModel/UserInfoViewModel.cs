
namespace BirthdayTimer.Util
{
    public  class UserInfoViewModel 
    {
        public UserInfoViewModel(BirthDayInfo birthDayInfo)
        {
            this.SelectedBirthDay = birthDayInfo;
        }
        public BirthDayInfo SelectedBirthDay { get; set; }

       
    }
}
