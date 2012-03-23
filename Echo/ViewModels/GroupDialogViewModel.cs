using Caliburn.Micro;

namespace Echo.ViewModels
{
    public class GroupDialogViewModel : Screen
    {
        public string Result { get; set; }

        private string _GroupName;
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                if (value != _GroupName)
                {
                    _GroupName = value;
                    NotifyOfPropertyChange("GroupName");
                }
            }
        }


        public void Confirm()
        {
            Result = GroupName;
            TryClose();
        }

        public void Cancel()
        {
            Result = null;
            TryClose();
        }
    }
}
