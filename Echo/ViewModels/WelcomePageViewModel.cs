using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Echo.Helpers;
using Microsoft.Phone.Controls;
using Echo.Model;
using Caliburn.Micro;

namespace Echo.ViewModels
{
    public class WelcomePageViewModel : Screen
    {
        private SettingsModel sm;
        private INavigationService navService;


        //#region CommandProperties
        //private ICommand _Continue;
        //public ICommand Continue
        //{
        //    get { return _Continue; }
        //}

        //private ICommand _ImportFromPhonebook;
        //public ICommand ImportFromPhonebook
        //{
        //    get { return _ImportFromPhonebook; }
        //}
        //private ICommand _DontImport;
        //public ICommand DontImport
        //{
        //    get { return _DontImport; }
        //}

        //private ICommand _Finish;
        //public ICommand Finish
        //{
        //    get { return _Finish; }
        //}
        //#endregion

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
            //_ImportFromPhonebook = new DelegateCommand(this.ImportFromPhonebookImpl);
            //_DontImport = new DelegateCommand(this.DontImportImpl);
            //_Finish = new DelegateCommand(this.FinishImpl);
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
