using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Caliburn.Micro;
using Echo.Logic;
using Echo.Model;
using Echo.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Echo
{
    public class MainPageViewModel : Conductor<object>.Collection.AllActive
    {
        private readonly INavigationService navService;
        private IPhoneService phoneService;

        private ApplicationBar appBar;
        private ContactsViewModel _cvm;
        public ContactsViewModel cvm
        {
            get { return _cvm; }
            set
            {
                _cvm = value;
                NotifyOfPropertyChange("cvm");
            }
        }

        private RecentsViewModel _rvm;
        public RecentsViewModel rvm
        {
            get { return _rvm; }
            set
            {
                _rvm = value;
                NotifyOfPropertyChange("rvm");
            }
        }

        private TrainerFrontViewModel _tvm;
        public TrainerFrontViewModel tvm
        {
            get { return _tvm; }
            set
            {
                _tvm = value;
                NotifyOfPropertyChange("tvm");
            }
        }

        private static bool firstConstructor = true;
        private Connection con;

        public bool ClearBackStack { get; set; }
        public bool Reload { get; set; }

        public MainPageViewModel(INavigationService navService, 
            WelcomePageViewModel wvm,
            IPhoneService phoneService, 
            ContactsViewModel cvm, 
            RecentsViewModel rvm, 
            TrainerFrontViewModel tvm, 
            SettingsModel sm,
            Connection con)
        {
            this.navService = navService;
            this.phoneService = phoneService;

            this.con = con;
            this.cvm = cvm;
            this.rvm = rvm;
            this.tvm = tvm;
            Items.Add(cvm);
            Items.Add(rvm);
            Items.Add(tvm);

            if (sm.getValueOrDefault<bool>(sm.ShowWelcomeScreenSettingKeyName, sm.ShowWelcomeScreenDefault))
            {
                //sm.AddOrUpdateValue(sm.ShowWelcomeScreenSettingKeyName, false);
                navService.UriFor<WelcomePageViewModel>().Navigate();
            }
        }


        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var viewPage = view as MainPage;
            appBar = viewPage.ApplicationBar as ApplicationBar;
            exchangeAppBarButtons(cvm.AppBarButtons);
            appBar.IsVisible = true;
            viewPage.Panorama.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(Items_SelectionChanged);
        }


        protected override void OnActivate()
        {
            if (navService.CanGoBack)
                navService.RemoveBackEntry();
            base.OnActivate();
            if (appBar != null && appBar.Buttons.Count == 0 && appBar.MenuItems.Count == 0)
                exchangeAppBarButtons(cvm.AppBarButtons);
        }

        public void Settings()
        {
            navService.UriFor<SettingsPageViewModel>().Navigate();
        }

        public void About()
        {
            navService.Navigate(new Uri("/Views/ActiveCallPage.xaml", UriKind.Relative));
        }

        private void exchangeAppBarButtons(List<ApplicationBarIconButton> inList)
        {
            appBar.Buttons.Clear();
            if (inList != null)
            {
                foreach (ApplicationBarIconButton b in inList)
                {
                    appBar.Buttons.Add(b);
                }
            }
            appBar.Mode = ApplicationBarMode.Default;
        }

        public void Contacts_gotFocus(object sender, EventArgs e)
        {
            exchangeAppBarButtons(cvm.AppBarButtons);
            appBar.Mode = ApplicationBarMode.Default;
        }

        public void Recents_gotFocus(object sender, EventArgs e)
        {
            exchangeAppBarButtons(rvm.AppBarButtons);
            appBar.Mode = ApplicationBarMode.Default;
        }

        public void Trainer_gotFocus(object sender, EventArgs e)
        {

        }

        public void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as Panorama).SelectedIndex)
            {
                case 0:
                    exchangeAppBarButtons(cvm.AppBarButtons);
                    appBar.Mode = ApplicationBarMode.Default;
                    break;
                case 1:
                    exchangeAppBarButtons(rvm.AppBarButtons);
                    appBar.Mode = ApplicationBarMode.Default;
                    break;
                default:
                    exchangeAppBarButtons(null);
                    appBar.Mode = ApplicationBarMode.Minimized;
                    break;
            }
        }

        public void GotoAbout()
        {

        }
    }
}