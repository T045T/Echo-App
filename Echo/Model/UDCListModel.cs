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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Echo.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO.IsolatedStorage;

namespace Echo.Model
{
    public class UDCListModel : INotifyPropertyChanged
    {
        private UserDataContext udc;
        public UserDataContext DataContext
        {
            get { return udc; }
            set
            {
                udc = value;
                RaisePropertyChangedEvent("DataContext");
            }
        }

        public UDCListModel()
        {
            this.udc = new UserDataContext();
            LoadListsFromDatabase();
        }

        // Special constructor for testing use - should only be accessed in Echo.test
        internal UDCListModel(string ConnectionString, bool overWrite)
        {
            this.udc = new UserDataContext(ConnectionString, overWrite);
            LoadListsFromDatabase();
        }

        private readonly string alphabet = "#abcdefghijklmnopqrstuvwxyz";
        private ObservableCollection<TitleGroup<UserModel>> _UsersByFirstName;
        private ObservableCollection<TitleGroup<UserModel>> _UsersByLastName;
        private ObservableCollection<GroupModel> _GroupList;
        private ObservableCollection<CallLogModel> _AllLogsList;
        public ObservableCollection<TitleGroup<UserModel>> UsersByFirstName
        {
            get
            {
                return _UsersByFirstName;
            }
            private set
            {
                if (_UsersByFirstName != value)
                {
                    _UsersByFirstName = value;
                    RaisePropertyChangedEvent("UsersByFirstName");
                }
            }
        }
        public ObservableCollection<TitleGroup<UserModel>> UsersByLastName
        {
            get
            {
                return _UsersByLastName;
            }
            private set
            {
                if (_UsersByLastName != value)
                {
                    _UsersByLastName = value;
                    RaisePropertyChangedEvent("UsersByLastName");
                }
            }
        }
        public ObservableCollection<GroupModel> GroupList
        {
            get
            {
                return _GroupList;
            }
            private set
            {
                if (_GroupList != value)
                {
                    _GroupList = value;
                    RaisePropertyChangedEvent("GroupList");
                }
            }
        }


        public ObservableCollection<CallLogModel> AllLogsList
        {
            get
            {
                return _AllLogsList;
            }
            private set
            {
                if (value != _AllLogsList)
                {
                    _AllLogsList = value;
                    RaisePropertyChangedEvent("AllLogsList");
                }
            }
        }

        private bool _LoadedLists;
        public bool LoadedLists
        {
            get
            {
                return _LoadedLists;
            }
            set
            {
                if (value != _LoadedLists)
                {
                    _LoadedLists = value;
                    RaisePropertyChangedEvent("LoadedLists");
                }
            }
        }

        public bool SubmitChanges()
        {
            try
            {
                udc.SubmitChanges();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        #region List Loading

        public void LoadListsFromDatabase()
        {
            udc.SubmitChanges();
            if (udc.UserTable.Any())
            {
                var UsersFromDbByFirst = new List<TitleGroup<UserModel>>(from UserModel u in udc.UserTable
                                                                         group u by u.FirstName.Substring(0, 1).ToLower() into foo
                                                                         orderby foo.Key
                                                                         select new TitleGroup<UserModel>(foo.Key, foo));

                var UsersFromDbByLast = new List<TitleGroup<UserModel>>(from UserModel u in udc.UserTable
                                                                        group u by u.LastName.Substring(0, 1).ToLower() into foo
                                                                        orderby foo.Key
                                                                        select new TitleGroup<UserModel>(foo.Key, foo));

                UsersByFirstName = SortIntoTitleGroups(UsersFromDbByFirst);
                UsersByLastName = SortIntoTitleGroups(UsersFromDbByLast);

                foreach (TitleGroup<UserModel> g in UsersByFirstName)
                    g.OrderBy((u, v) => u.FirstName.CompareTo(v.FirstName));
                foreach (TitleGroup<UserModel> g in UsersByLastName)
                    g.OrderBy((u, v) => u.LastName.CompareTo(v.LastName));
            }
            else
            {
                UsersByFirstName = new ObservableCollection<TitleGroup<UserModel>>();
                foreach (char c in alphabet)
                {
                    UsersByFirstName.Add(new TitleGroup<UserModel>(c.ToString(), new List<UserModel>()));
                }
                UsersByLastName = new ObservableCollection<TitleGroup<UserModel>>();
                foreach (char c in alphabet)
                {
                    UsersByLastName.Add(new TitleGroup<UserModel>(c.ToString(), new List<UserModel>()));
                }
            }
            if (udc.GroupTable.Any())
            {
                GroupList = new ObservableCollection<GroupModel>(udc.GroupTable.ToList().OrderBy(x => x.GroupName));
            }
            else
            {
                GroupList = new ObservableCollection<GroupModel>();
            }
            if (udc.CallLogTable.Any())
            {
                AllLogsList = new ObservableCollection<CallLogModel>(udc.CallLogTable.ToList().OrderByDescending(c => c.StartTime));
            }
            else
            {
                AllLogsList = new ObservableCollection<CallLogModel>();
            }
            LoadedLists = true;

        }

        private ObservableCollection<TitleGroup<UserModel>> SortIntoTitleGroups(List<TitleGroup<UserModel>> UsersFromDb)
        {
            bool createdNumberGroup = false;
            List<UserModel> NumberGroup = new List<UserModel>();
            List<TitleGroup<UserModel>> tmp = new List<TitleGroup<UserModel>>();
            foreach (TitleGroup<UserModel> g in UsersFromDb)
            {

                if (Regex.Match(g.Title, "[^a-zA-Z]").Success)
                {
                    if (!createdNumberGroup)
                    {
                        NumberGroup.AddRange(g.Items);
                        createdNumberGroup = true;
                    }
                    else
                    {
                        NumberGroup.AddRange(g.Items);
                    }
                    tmp.Add(g);
                }
            }
            if (createdNumberGroup)
            {
                foreach (TitleGroup<UserModel> g in tmp)
                {
                    UsersFromDb.Remove(g);
                }
                UsersFromDb.Insert(0, new TitleGroup<UserModel>("#", NumberGroup));
                UsersFromDb.OrderBy(group => group.Title);
            }
            var EmptyGroups = new List<TitleGroup<UserModel>>();
            foreach (char c in alphabet)
            {
                EmptyGroups.Add(new TitleGroup<UserModel>(c.ToString(), new List<UserModel>()));
            }
            return new ObservableCollection<TitleGroup<UserModel>>(UsersFromDb.Union(EmptyGroups).OrderBy((x) => x.Title));
        }

        #endregion List Loading


        public void removeGroup(string GroupName)
        {
            var group = from GroupModel g in udc.GroupTable where g.GroupName.Equals(GroupName) select g;
            if (group.Any())
            {
                var junctions = from GroupMapModel gmm in udc.JunctionTable where gmm.GroupName.Equals(GroupName) select gmm;
                GroupList.Remove(group.First());
                udc.JunctionTable.DeleteAllOnSubmit(junctions);
                udc.SubmitChanges();
                udc.GroupTable.DeleteAllOnSubmit(group);
                udc.SubmitChanges();
            }
        }

        public bool addGroup(string GroupName)
        {
            return addGroup(new GroupModel(GroupName));
        }
        public bool addGroup(GroupModel Group)
        {
            udc.GroupTable.InsertOnSubmit(Group);

            try
            {
                udc.SubmitChanges();
            }
            catch (Exception dbe)
            {
                return false;
            }
            GroupList.Add(Group);
            GroupList.OrderBy(g => g.GroupName);
            return true;
        }

        #region User Methods

        public UserModel GetUser(int? UserID)
        {
            if (UserID != null)
            {
                var user = from u in udc.UserTable where u.ID == UserID select u;
                if (user.Any())
                {
                    return user.First();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public void addToGroup(UserModel u, GroupModel g)
        {
            GroupMapModel junctionModel = new GroupMapModel(g.GroupName, u.ID);
            //JunctionTable.InsertOnSubmit(junctionModel);

            g.addJunction(junctionModel);
            udc.SubmitChanges();
        }
        public void removeFromGroup(UserModel u, GroupModel g)
        {
            var junction = from GroupMapModel gmm in udc.JunctionTable where gmm.UserID.Equals(u.UserID) && gmm.GroupName.Equals(g.GroupName) select gmm;
            GroupMapModel junctionModel;
            if (junction.Any())
            {
                junctionModel = junction.First();
                udc.JunctionTable.DeleteOnSubmit(junctionModel);
                udc.SubmitChanges();
            }
        }

        public void removeUser(string UserID)
        {
            var user = from UserModel m in udc.UserTable where m.UserID.Equals(UserID) select m;
            if (user.Any())
                removeUser(user.First());
        }
        public void removeUser(UserModel User)
        {
            if (udc.UserTable.Contains(User))
            {
                foreach (TitleGroup<UserModel> lettergroup in UsersByFirstName)
                {
                    if (lettergroup.Contains(User))
                    {
                        lettergroup.Remove(User);
                    }
                }
                foreach (TitleGroup<UserModel> lettergroup in UsersByLastName)
                {
                    if (lettergroup.Contains(User))
                    {
                        lettergroup.Remove(User);
                    }
                }
                var junctions = from GroupMapModel gmm in udc.JunctionTable where gmm.UserID.Equals(User.ID) select gmm;
                var logs = from CallLogModel clm in udc.CallLogTable where clm.CalleeID.Equals(User.ID) select clm;
                foreach (CallLogModel e in logs)
                {
                    var entries = from CallLogEntry en in udc.EntryTable where en.CallLogID.Equals(e.CallLogID) select en;
                    udc.EntryTable.DeleteAllOnSubmit(entries);
                    AllLogsList.Remove(e);
                }

                if (User.HasImage)
                {
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (myIsolatedStorage.FileExists(User.UserImagePath))
                        {
                            myIsolatedStorage.DeleteFile(User.UserImagePath);
                        }
                    }
                }
                udc.CallLogTable.DeleteAllOnSubmit(logs);
                udc.SubmitChanges();
                udc.JunctionTable.DeleteAllOnSubmit(junctions);
                udc.SubmitChanges();
                udc.UserTable.DeleteOnSubmit(User);
                udc.SubmitChanges();
            }
        }

        public bool addUser(UserModel User)
        {
            if ((from u in udc.UserTable where u.UserID.Equals(User.UserID) select u).Any())
                return false;

            udc.UserTable.InsertOnSubmit(User);
            try { udc.SubmitChanges(); }
            catch (Exception dbe) { return false; }

            string firstInitial = findInitial(User.FirstName);
            string lastInitial = findInitial(User.LastName);
            foreach (TitleGroup<UserModel> gr in UsersByFirstName)
            {
                if (gr.Title.Equals(firstInitial))
                {
                    gr.Add(User);
                    gr.OrderBy((u, v) => u.FirstName.CompareTo(v.FirstName));
                    RaisePropertyChangedEvent("UsersByFirstName");
                }
            }
            foreach (TitleGroup<UserModel> gr in UsersByLastName)
            {
                if (gr.Title.Equals(lastInitial))
                {
                    gr.Add(User);
                    gr.OrderBy((u, v) => u.LastName.CompareTo(v.LastName));
                    RaisePropertyChangedEvent("UsersByLastName");
                }
            }
            return true;
        }

        public bool addUser(string UserID, string FirstName, string LastName, string UserImagePath)
        {
            return addUser(new UserModel(UserID, FirstName, LastName, UserImagePath));
        }


        public bool changeUserID(UserModel m, string NewUserID)
        {
            if ((from u in udc.UserTable where u.UserID.Equals(NewUserID) select u).Any())
                return false;
            m.UserID = NewUserID;
            try
            {
                udc.SubmitChanges();
            }
            catch (Exception e) { return false; }

            return true;
            //if (!NewUserID.Equals(m.UserID))
            //{
            //    UserModel newUser = new UserModel(NewUserID, m.FirstName, m.LastName, m.UserImagePath);
            //    if (addUser(newUser))
            //    {
            //        var junctions = from GroupMapModel gmm in udc.JunctionTable where gmm.UserID.Equals(m.UserID) select gmm;
            //        var logs = from CallLogModel clm in udc.CallLogTable where clm.CalleeID.Equals(m.UserID) select clm;
            //        List<GroupMapModel> newGmm = new List<GroupMapModel>();
            //        List<CallLogModel> newClm = new List<CallLogModel>();
            //        foreach (GroupMapModel gmm in junctions)
            //        {
            //            //newGmm.Add(new GroupMapModel(gmm.GroupName, newUser.UserID));
            //            gmm.UserID = newUser.ID;
            //        }
            //        udc.SubmitChanges();
            //        foreach (CallLogModel clm in logs)
            //        {
            //            //var tmp = new CallLogModel(newUser.UserID, clm.StartTime);
            //            //newClm.Add(new CallLogModel(newUser.UserID, clm.StartTime));
            //            clm.CalleeID = newUser.ID;
            //        }
            //        udc.SubmitChanges();
            //        this.removeUser(m);
            //        try
            //        {
            //            udc.SubmitChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            bool foo = false;
            //        }
            //        return newUser;
            //    }
            //    else
            //    {
            //        return m;
            //    }
            //}
            //else
            //{
            //    return m;
            //}
        }

        //public bool SubmitUserChanges(UserModel User)
        //{
        //}
        #endregion User Methods

        private string findInitial(string input)
        {
            if (input.Length > 0)
            {
                string initial = input.Substring(0, 1);
                if (Regex.Match(initial, "[a-zA-Z]").Success)
                {
                    return initial;
                }
                else
                {
                    return "#";
                }
            }
            else
            {
                return "#";
            }
        }

        public void deleteCallLogs()
        {
            udc.CallLogTable.DeleteAllOnSubmit(udc.CallLogTable);
            udc.EntryTable.DeleteAllOnSubmit(udc.EntryTable);
            udc.SubmitChanges();
            LoadListsFromDatabase();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        public virtual void RaisePropertyChangedEvent(string propertyName)
        {
            // Exit if no subscribers
            if (PropertyChanged == null) return;
            // Raise event
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }
        #endregion
    }
}
