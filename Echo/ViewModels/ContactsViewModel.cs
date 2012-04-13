using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Echo.Helpers;
using Echo.Model;
using Echo.Views;
using Microsoft.Phone.Shell;

namespace Echo.ViewModels
{
    public class ContactsViewModel : Screen
    {

        public List<ApplicationBarIconButton> AppBarButtons { get; private set; }

        private UDCListModel udc;
        private INavigationService navService;
        private SettingsModel sm;
        private BackgroundWorker bgWorker;
        private bool loadedOnce = false;


        #region Properties and backing variables
        public bool NameOrderChanged
        {
            get
            {
                bool tmp = sm.getValueOrDefault<bool>(sm.NameOrderSettingKeyName, sm.NameOrderDefault);
                return sm.NameOrder != tmp;
            }
        }
        public Visibility NameOrder
        {
            get { return sm.NameOrder ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility NotNameOrder
        {
            get { return !sm.NameOrder ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ObservableCollection<TitleGroup<UserModel>> UsersByName
        {
            get
            {
                if (sm.NameOrder)
                {
                    return udc.UsersByFirstName;
                }
                else
                {
                    return udc.UsersByLastName;
                }
            }
        }

        public ObservableCollection<GroupModel> GroupList
        {
            get { return udc.GroupList; }
        }

        private bool _DoneLoading;
        public bool DoneLoading
        {
            get { return _DoneLoading; }
            set
            {
                if (value != _DoneLoading)
                {
                    _DoneLoading = value;
                    NotifyOfPropertyChange("DoneLoading");
                }
            }
        }

        public SettingsModel SM
        {
            get { return sm; }
        }
        #endregion

        public ContactsViewModel(INavigationService navService, SettingsModel sm, UDCListModel udc)
        {
            this.navService = navService;
            this.DisplayName = "contacts";
            CreateApplicationBarButtons();

            this.sm = sm;
            NotifyOfPropertyChange("NameOrder");
            this.udc = udc;
            NotifyOfPropertyChange("UsersByName");
        }


        protected override void OnActivate()
        {
            base.OnActivate();
            NotifyOfPropertyChange("UsersByName");
        }

        private ContactsView myView;

        public void refreshBinding()
        {
            if (myView != null)
            {
                myView.UserLongList.DataContext = null;
                myView.UserLongList.DataContext = this;
            }
        }


        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            myView = (view as ContactsView);
            refreshBinding();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
        }

        public bool LoadedOnce { get { return loadedOnce; } }

        private void CreateApplicationBarButtons()
        {
            AppBarButtons = new List<ApplicationBarIconButton>();

            var favs = new ApplicationBarIconButton();
            favs.IconUri = new Uri("icons/appbar.favs.rest.png", UriKind.Relative);
            favs.IsEnabled = true;
            favs.Text = "favorites";
            favs.Click += new EventHandler((sender, e) => navService.Navigate(new Uri("/Views/TrainerPage.xaml", UriKind.Relative)));
            AppBarButtons.Add(favs);

            var add = new ApplicationBarIconButton();
            add.IconUri = new Uri("/icons/appbar.add.rest.png", UriKind.Relative);
            add.IsEnabled = true;
            add.Text = "add";
            add.Click += new EventHandler((sender, e) =>
                navService.UriFor<ContactEditPageViewModel>().
                    WithParam(x => x.CreateUser, true).
                    WithParam(x => x.TargetUserID, 0).
                    Navigate());
            AppBarButtons.Add(add);

            var status = new ApplicationBarIconButton();
            status.IconUri = new Uri("/icons/appbar.edit.rest.png", UriKind.Relative);
            status.IsEnabled = true;
            status.Text = "status";
            status.Click += new EventHandler((sender, e) => 
                navService.UriFor<NetworkTestPageViewModel>()
                .Navigate());
            AppBarButtons.Add(status);

            var search = new ApplicationBarIconButton();
            search.IconUri = new Uri("/icons/appbar.feature.search.rest.png", UriKind.Relative);
            search.IsEnabled = true;
            search.Text = "search";
            search.Click += new EventHandler(search_Click);
            AppBarButtons.Add(search);
        }

        public void ContactTapped(UserModel um)
        {
            navService.UriFor<ContactDetailsPageViewModel>().
                    WithParam(x => x.TargetUserID, um.ID).
                    Navigate();
        }

        public void GroupTapped(GroupModel gm)
        {
            navService.UriFor<GroupPageViewModel>().
                WithParam(x => x.GroupName, gm.GroupName).
                Navigate();
        }

        public void EditContact(UserModel um)
        {
            navService.UriFor<ContactEditPageViewModel>().
                WithParam(x => x.CreateUser, false).
                WithParam(x => x.TargetUserID, um.ID).
                Navigate();
        }

        public void DeleteContact(UserModel um)
        {
            if (MessageBox.Show("Are you sure you want to delete " + um.FirstLast + "?", "Delete", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
            {
                udc.removeUser(um);
                refreshBinding();
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            udc.removeUser("qq");
        }
    }
}
