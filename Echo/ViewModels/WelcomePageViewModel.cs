using System.Windows;
using Caliburn.Micro;
using Echo.Model;

namespace Echo.ViewModels
{
    public class WelcomePageViewModel : Screen
    {
        private SettingsModel sm;
        private INavigationService navService;

        #region VisibilityProperties
        private Visibility _Page1Visibility;
        public Visibility Page1Visibility
        {
            get { return _Page1Visibility; }
            set
            {
                if (value == Visibility.Visible)
                {
                    Page2Visibility = Visibility.Collapsed;
                    Page3Visibility = Visibility.Collapsed;
                    _Page1Visibility = Visibility.Visible;
                }
                else
                {
                    _Page1Visibility = value;
                }
                NotifyOfPropertyChange("Page1Visibility");
            }
        }

        private Visibility _Page2Visibility;
        public Visibility Page2Visibility
        {
            get { return _Page2Visibility; }
            set
            {
                if (value == Visibility.Visible)
                {
                    Page1Visibility = Visibility.Collapsed;
                    Page3Visibility = Visibility.Collapsed;
                    _Page2Visibility = Visibility.Visible;
                }
                else
                {
                    _Page2Visibility = value;
                }
                NotifyOfPropertyChange("Page2Visibility");
            }
        }
        private Visibility _Page3Visibility;
        public Visibility Page3Visibility
        {
            get { return _Page3Visibility; }
            set
            {
                if (value == Visibility.Visible)
                {
                    Page2Visibility = Visibility.Collapsed;
                    Page1Visibility = Visibility.Collapsed;
                    _Page3Visibility = Visibility.Visible;
                }
                else
                {
                    _Page3Visibility = value;
                }
                NotifyOfPropertyChange("Page3Visibility");
            }
        }

        #endregion VisibilityProperties

        public WelcomePageViewModel(SettingsModel sm, INavigationService navService)
        {
            this.sm = sm;
            this.navService = navService;
            UserName = "";
            Password = "";
        }

        #region Text Field Properties
        private string _UserName;
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                if (value != _UserName)
                {
                    _UserName = value;
                    NotifyOfPropertyChange("UserName");
                    NotifyOfPropertyChange("CanContinue");
                }
            }
        }
        private string _Password;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if (value != _Password)
                {
                    _Password = value;
                    NotifyOfPropertyChange("Password");
                    NotifyOfPropertyChange("CanContinue");
                }
            }
        }
        #endregion

        #region Button Methods
        public void Continue()
        {
            Page2Visibility = Visibility.Visible;
        }
        public bool CanContinue { get { return UserName.Length > 0 && Password.Length > 0; } }

        public void NoAccount()
        {
            // TODO implement account helper
        }
        public bool CanNoAccount { get { return false; } }

        public void ImportContacts()
        {
            // TODO actually import contacts
            if (sm.AddOrUpdateValue(sm.ImportSettingKeyName, true))
            {
                sm.Save();
            }
        }
        public bool CanImportContacts { get { return false; } }

        public void DontImportContacts()
        {
            Page3Visibility = Visibility.Visible;
        }
        public bool CanDontImportContacts { get { return true; } }

        public void Finish()
        {
            navService.GoBack();
        }
        public bool CanFinish { get { return true; } }
        #endregion
    }
}
