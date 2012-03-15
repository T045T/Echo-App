using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using Caliburn.Micro;
using Echo.ViewModels;
using System.Windows.Controls.Primitives;
using Echo.Logic;
using Echo.Model;

namespace Echo
{
    public class MainPageViewModel : Conductor<object>.Collection.AllActive
    {
        private readonly INavigationService navService;
        private IPhoneService phoneService;
        private IWindowManager winMan;

        //private Popup myPopup;
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
            //this.winMan = windowMan;
            this.navService = navService;
            this.phoneService = phoneService;

            this.con = con;
            //if (firstConstructor)
            //{
            //    myPopup = new Popup() { IsOpen = false, Child = new StartupSplashScreen() };
            //}
            //else
            //{
            //    myPopup = new Popup() { IsOpen = false, Child = new SplashScreen() };
            //}
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
                //winMan.ShowPopup(new WelcomePageViewModel());
            }
            //ActivateItem(cvm);
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
            if (!firstConstructor)
            {
                //myPopup.Child = new SplashScreen();
            }
            else
            {
                firstConstructor = false;
            }
            if (!cvm.LoadedOnce || cvm.NameOrderChanged || Reload)
            {
                //myPopup.IsOpen = true;
                //PropertyChanged += new PropertyChangedEventHandler((sender, args) =>
                //{
                //    if (args.PropertyName.Equals("LoadedLists"))
                //        Deployment.Current.Dispatcher.BeginInvoke(() => {
                //            myPopup.IsOpen = false;
                //            if (appBar != null)
                //                appBar.IsVisible = true;
                //        });
                //});
                //cvm.LoadInBackground(new RunWorkerCompletedEventHandler((sender, args) =>
                //{
                //    Deployment.Current.Dispatcher.BeginInvoke(() =>
                //    {
                //        myPopup.IsOpen = false;
                //        if (appBar != null)
                //            appBar.IsVisible = true;
                //    });
                //}));
            }
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
                    //ActivateItem(cvm);
                    break;
                case 1:
                    exchangeAppBarButtons(rvm.AppBarButtons);
                    appBar.Mode = ApplicationBarMode.Default;
                    //ActivateItem(rvm);
                    break;
                default:
                    exchangeAppBarButtons(null);
                    appBar.Mode = ApplicationBarMode.Minimized;
                    //ActivateItem(tvm);
                    break;
            }
        }

        public void GotoAbout()
        {

        }
    }
}