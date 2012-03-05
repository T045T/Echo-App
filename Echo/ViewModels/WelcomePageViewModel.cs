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
        SettingsModel sm;
        private TransitionFrame root;


        #region CommandProperties
        private ICommand _Continue;
        public ICommand Continue
        {
            get { return _Continue; }
        }

        private ICommand _ImportFromPhonebook;
        public ICommand ImportFromPhonebook
        {
            get { return _ImportFromPhonebook; }
        }
        private ICommand _DontImport;
        public ICommand DontImport
        {
            get { return _DontImport; }
        }

        private ICommand _Finish;
        public ICommand Finish
        {
            get { return _Finish; }
        }
        #endregion

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

        public WelcomePageViewModel()
        {
            sm = new SettingsModel();
            root = Application.Current.RootVisual as TransitionFrame;
            _Continue = new DelegateCommand(this.ContinueImpl, (o => Page1Visibility == Visibility.Visible));
            _ImportFromPhonebook = new DelegateCommand(this.ImportFromPhonebookImpl);
            _DontImport = new DelegateCommand(this.DontImportImpl);
            _Finish = new DelegateCommand(this.FinishImpl);
        }

        #region CommandImplementations
        public void ContinueImpl(object sender)
        {
            Page2Visibility = Visibility.Visible;
        }

        public void ImportFromPhonebookImpl(object sender)
        {
            if (sm.AddOrUpdateValue(sm.ImportSettingKeyName, true))
            {
                sm.Save();
            }
            throw new NotImplementedException();
            Page3Visibility = Visibility.Visible;
        }

        public void DontImportImpl(object sender)
        {
            if (sm.AddOrUpdateValue(sm.ImportSettingKeyName, false)) {
                sm.Save();
            }
            Page3Visibility = Visibility.Visible;
        }

        public void FinishImpl(object sender) {
            root.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        #endregion
    }
}
