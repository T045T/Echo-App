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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.ObjectModel;
using System.Linq;

namespace Echo.Model
{
    [Table]
    public class GroupModel : LinqModelBase
    {
        [Column(IsVersion = true)]
        private Binary version;

        private string _GroupName;
        [Column(IsPrimaryKey = true)]
        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                NotifyPropertyChanging("GroupName");
                _GroupName = value;
                RaisePropertyChangedEvent("GroupName");
            }
        }

        private EntitySet<GroupMapModel> _UserGroups;
        [Association(ThisKey = "GroupName", OtherKey = "GroupName", Storage = "_UserGroups")]
        public EntitySet<GroupMapModel> UserGroups {
            get { return _UserGroups; }
            private set
            {
                NotifyPropertyChanging("UserGroups");
                _UserGroups.Assign(value);
                RaisePropertyChangedEvent("UserGroups");
            }
        }
        public ObservableCollection<UserModel> Users
        {
            get { return new ObservableCollection<UserModel>(from GroupMapModel gmm in Queryable.AsQueryable(UserGroups) select gmm.User); }
        }

        public bool UserIsMember(int UserID)
        {
            return (from GroupMapModel m in UserGroups where m.UserID.Equals(UserID) select m).Any();
        }


        public GroupModel()
        {
            this._UserGroups = new EntitySet<GroupMapModel>(this.attachUserGroup, this.detachUserGroup);
        }

        public GroupModel(string Name)
        {
            this._UserGroups = new EntitySet<GroupMapModel>(this.attachUserGroup, this.detachUserGroup);
            this.GroupName = Name;
        }

        public void attachUserGroup(GroupMapModel gmm)
        {
            NotifyPropertyChanging("GroupMapModel");
            gmm.Group = this;
            //RaisePropertyChangedEvent("GroupMapModel");
        }
        public void detachUserGroup(GroupMapModel gmm)
        {
            NotifyPropertyChanging("GroupMapModel");
            gmm.Group = null;
            //RaisePropertyChangedEvent("GroupMapModel");
        }

        public void addJunction(GroupMapModel gmm)
        {
            NotifyPropertyChanging("UserGroups");
            this.UserGroups.Add(gmm);
            RaisePropertyChangedEvent("UserGroups");
        }
        public void remJunction(GroupMapModel gmm)
        {
            NotifyPropertyChanging("UserGroups");
            this.UserGroups.Remove(gmm);
            RaisePropertyChangedEvent("UserGroups");
        }

        public void addUser(UserModel um) {
            NotifyPropertyChanging("UserGroups");
            this.UserGroups.Add(new GroupMapModel(this.GroupName, um.ID));
            RaisePropertyChangedEvent("UserGroups");
        }

        public void remUser(UserModel um)
        {
            NotifyPropertyChanging("UserGroups");
            this.UserGroups.Remove(new GroupMapModel(this.GroupName, um.ID));
            RaisePropertyChangedEvent("UserGroups");
        }
    }
}
