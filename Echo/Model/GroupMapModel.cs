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

namespace Echo.Model
{
    [Table]
    public class GroupMapModel : LinqModelBase
    {
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

        private int _UserID;
        [Column(IsPrimaryKey = true)]
        public int UserID
        {
            get { return _UserID; }
            set
            {
                NotifyPropertyChanging("UserID");
                _UserID = value;
                RaisePropertyChangedEvent("UserID");
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;

        private EntityRef<UserModel> _User;
        [Association(ThisKey = "UserID", OtherKey = "ID", Storage = "_User", IsForeignKey = true)]
        public UserModel User
        {
            get { return _User.Entity; }
            set
            {
                NotifyPropertyChanging("User");
                _User.Entity = value;
                RaisePropertyChangedEvent("User");
            }
        }

        private EntityRef<GroupModel> _Group;
        [Association(ThisKey = "GroupName", OtherKey = "GroupName", Storage = "_Group", IsForeignKey = true)]
        public GroupModel Group
        {
            get { return _Group.Entity; }
            set
            {
                NotifyPropertyChanging("Group");
                _Group.Entity = value;
                if (value != null)
                {
                    GroupName = value.GroupName;
                }
                RaisePropertyChangedEvent("Group");
            }
        }

        public GroupMapModel(string GroupName, int UserID)
        {
            this.GroupName = GroupName;
            this.UserID = UserID;
        }

        public GroupMapModel()
        { }
    }
}
