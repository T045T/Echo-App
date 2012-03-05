using System;
using System.Windows;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Echo.Model;
using Caliburn.Micro;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.ComponentModel;
using Echo.Helpers;
using Echo.Views;
using System.Linq;
using System.Data.Linq;
using System.Windows.Navigation;

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
            /*
            if (this.ReloadList || this._NameOrder != (sm.getValueOrDefault<bool>(sm.NameOrderSettingKeyName, sm.NameOrderDefault)))
            {
                this.LoadInBackground(new RunWorkerCompletedEventHandler((sender, args) =>
                {

                }));
            };
            */
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
        #region old user and list implementation
        //private bool bgLock;
        //public void LoadInBackground(RunWorkerCompletedEventHandler eh)
        //{
        //    NameOrder = (sm.getValueOrDefault<bool>(sm.NameOrderSettingKeyName, sm.NameOrderDefault) ? Visibility.Visible : Visibility.Collapsed);
        //    if (!bgLock)
        //    {
        //        bgLock = true;
        //        bgWorker = new BackgroundWorker();
        //        bgWorker.DoWork += new DoWorkEventHandler((sender, args) => LoadListsFromDatabase());
        //        bgWorker.RunWorkerCompleted += eh;
        //        bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((sender, args) =>
        //        {
        //            Deployment.Current.Dispatcher.BeginInvoke(() =>
        //            {
        //                this.bgLock = false;
        //                this.loadedOnce = true;
        //            });
        //        });
        //        bgWorker.RunWorkerAsync();
        //    }
        //    else
        //    {
        //        bgWorker.RunWorkerCompleted += eh;
        //    }
        //}

        //private ObservableCollection<Group<UserModel>> _UsersByName;
        //public ObservableCollection<Group<UserModel>> UsersByName
        //{
        //    get { return _UsersByName; }
        //    set
        //    {
        //        if (value != _UsersByName)
        //        {
        //            _UsersByName = value;
        //            NotifyOfPropertyChange("UsersByName");
        //        }
        //    }
        //}
        //private ObservableCollection<UserModel> _UserList;
        //public ObservableCollection<UserModel> UserList
        //{
        //    get { return _UserList; }
        //    set
        //    {
        //        if (value != _UserList)
        //        {
        //            _UserList = value;
        //            NotifyOfPropertyChange("UserList");
        //        }
        //    }
        //}
        //private ObservableCollection<GroupModel> _GroupList;
        //public ObservableCollection<GroupModel> GroupList
        //{
        //    get { return _GroupList; }
        //    set
        //    {
        //        if (value != _GroupList)
        //        {
        //            _GroupList = value;
        //            NotifyOfPropertyChange("GroupList");
        //        }
        //    }
        //}
        //private ObservableCollection<GroupMapModel> _JunctionTable;
        //public ObservableCollection<GroupMapModel> JunctionTable
        //{
        //    get { return _JunctionTable; }
        //    set
        //    {
        //        _JunctionTable = value;
        //        NotifyOfPropertyChange("JunctionTable");
        //    }
        //}

        //private static readonly string alphabet = "#abcdefghijklmnopqrstuvwxyz";
        //private Func<UserDataContext, IQueryable<TitleGroup<UserModel>>> UsersByFirstName =
        //    CompiledQuery.Compile((UserDataContext dataContext) => (from UserModel u in dataContext.UserTable
        //                                                            group u by u.FirstName.Substring(0, 1).ToLower() into foo
        //                                                            orderby foo.Key
        //                                                            select new TitleGroup<UserModel>(foo.Key, foo)));

        ///*
        //private Func<UserDataContext, IEnumerable<Group<UserModel>>> UsersByFirstNameList =
        //    CompiledQuery.Compile((UserDataContext dataContext) => (from UserModel u in dataContext.UserList
        //                                                    group u by u.FirstName.Substring(0, 1).ToLower() into foo
        //                                                    orderby foo.Key
        //                                                    select new Group<UserModel>(foo.Key, foo)));
        //*/
        //private Func<UserDataContext, IQueryable<TitleGroup<UserModel>>> UsersByLastName =
        //    CompiledQuery.Compile((UserDataContext dataContext) => (from UserModel u in dataContext.UserTable
        //                                                            group u by u.LastName.Substring(0, 1).ToLower() into foo
        //                                                            orderby foo.Key
        //                                                            select new TitleGroup<UserModel>(foo.Key, foo)));

        //private Func<UserDataContext, IOrderedEnumerable<GroupModel>> GroupsFromDb =
        //    CompiledQuery.Compile((UserDataContext dataContext) => (dataContext.GroupTable.ToList()).OrderBy(x => x.GroupName));
        //private void LoadListsFromDatabase()
        //{
        //    List<TitleGroup<UserModel>> UsersFromDb;
        //    if (sm.NameOrder)
        //    {
        //        UsersFromDb = UsersByFirstName(udc.DataContext).ToList();
        //    }
        //    else
        //    {
        //        UsersFromDb = UsersByLastName(udc.DataContext).ToList();
        //    }
        //    bool createdNumberGroup = false;
        //    List<UserModel> NumberGroup = new List<UserModel>();
        //    List<TitleGroup<UserModel>> tmp = new List<TitleGroup<UserModel>>();
        //    foreach (TitleGroup<UserModel> g in UsersFromDb)
        //    {

        //        if (Regex.Match(g.Title, "[^a-zA-Z]").Success)
        //        {
        //            if (!createdNumberGroup)
        //            {
        //                NumberGroup.AddRange(g.Items);
        //                createdNumberGroup = true;
        //            }
        //            else
        //            {
        //                NumberGroup.AddRange(g.Items);
        //            }
        //            tmp.Add(g);
        //        }
        //    }
        //    if (createdNumberGroup)
        //    {
        //        foreach (TitleGroup<UserModel> g in tmp)
        //        {
        //            UsersFromDb.Remove(g);
        //        }
        //        UsersFromDb.Insert(0, new TitleGroup<UserModel>("#", NumberGroup));
        //        UsersFromDb.OrderBy(group => group.Title);
        //    }
        //    var EmptyGroups = new List<TitleGroup<UserModel>>();
        //    foreach (char c in alphabet)
        //    {
        //        EmptyGroups.Add(new TitleGroup<UserModel>(c.ToString(), new List<UserModel>()));
        //    }
        //    UsersByName = new ObservableCollection<TitleGroup<UserModel>>(UsersFromDb.Union(EmptyGroups).OrderBy((x) => x.Title));
        //    GroupList = new ObservableCollection<GroupModel>(GroupsFromDb(udc.DataContext));
        //}

        //private string findInitial(string input)
        //{

        //    string initial = input.Substring(0, 1);
        //    if (Regex.Match(initial, "[a-zA-Z]").Success)
        //    {
        //        return initial;
        //    }
        //    else
        //    {
        //        return "#";
        //    }
        //}

        //private bool AddUser(UserModel newUser)
        //{
        //    udc.UserTable.InsertOnSubmit(newUser);
        //    try
        //    {
        //        udc.SubmitChanges();
        //    }
        //    catch (System.Data.Linq.DuplicateKeyException)
        //    {
        //        return false;
        //    }
        //    UserList.Add(newUser);
        //    return true;
        //}

        //private bool RemoveUser(UserModel remUser)
        //{
        //    udc.UserTable.DeleteOnSubmit(remUser);
        //    try
        //    {
        //        udc.SubmitChanges();
        //    }
        //    catch (System.Data.Common.DbException de)
        //    {
        //        return false;
        //    }
        //    UserList.Remove(remUser);
        //    return true;
        //}
        #endregion

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

    public class Group<T> : IEnumerable<T>
    {
        public Group(string name, IEnumerable<T> items)
        {
            this.Title = name;
            this.Items = new List<T>(items);
        }

        public override bool Equals(object obj)
        {
            Group<T> that = obj as Group<T>;

            return (that != null) && (this.Title.Equals(that.Title));
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }

        public bool HasItems
        {
            get { return Items.Count > 0; }
        }

        public string Title
        {
            get;
            set;
        }

        public IList<T> Items
        {
            get;
            set;
        }

        public Brush GroupBackgroundBrush
        {
            get
            {
                if (HasItems)
                    return (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                else
                    return (SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"];
            }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion
    }
}
