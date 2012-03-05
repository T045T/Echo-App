using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.IO.IsolatedStorage;

namespace Echo.Model
{
    [Table]
    public class UserModel : LinqModelBase
    {
        //private EntitySet<CallLogModel> _callLogs;

        [Column(IsVersion = true)]
        private Binary _version;

        private int _ID;
        [Column(IsPrimaryKey = true, CanBeNull = false, DbType = "INT NOT NULL Identity", IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID
        {
            get { return _ID; }
            set
            {
                if (_ID != value)
                {
                    NotifyPropertyChanging("ID");
                    _ID = value;
                    RaisePropertyChangedEvent("ID");
                }
            }
        }

        private string _UserID;
        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string UserID
        {
            get { return _UserID; }
            set
            {
                if (_UserID != value)
                {
                    NotifyPropertyChanging("UserID");
                    _UserID = value;
                    RaisePropertyChangedEvent("UserID");
                }
            }
        }

        private string _FirstName;

        [Column]
        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                if (_FirstName != value)
                {
                    NotifyPropertyChanging("FirstName");
                    _FirstName = value;
                    RaisePropertyChangedEvent("FirstName");
                    RaisePropertyChangedEvent("FirstLast");
                    RaisePropertyChangedEvent("LastFirst");
                }
            }
        }


        private string _LastName;

        [Column]
        public string LastName
        {
            get { return _LastName; }
            set
            {
                if (_LastName != value)
                {
                    NotifyPropertyChanging("LastName");
                    _LastName = value;
                    RaisePropertyChangedEvent("LastName");
                    RaisePropertyChangedEvent("FirstLast");
                    RaisePropertyChangedEvent("LastFirst");
                }
            }
        }

        public string LastFirst
        {
            get { return LastName + ", " + FirstName; }
        }
        public string FirstLast
        {
            get { return FirstName + " " + LastName; }
        }

        private string _UserImagePath;
        [Column]
        public string UserImagePath
        {
            get { return _UserImagePath ?? ""; }
            set
            {
                if (_UserImagePath != value)
                {
                    NotifyPropertyChanging("UserImagePath");
                    _UserImagePath = value;
                    RaisePropertyChangedEvent("UserImagePath");
                }
            }
        }

        public ImageSource UserImageSource
        {
            get
            {
                if (UserImagePath.Length > 0)
                {
                    BitmapImage img = new BitmapImage();
                    using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = isoStore.OpenFile(UserImagePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            img.SetSource(stream);
                        }
                    }
                    return img;
                }
                else
                {
                    return new BitmapImage();
                }
            }
        }

        public bool HasImage { get { return UserImagePath.Length > 0; } }

        private EntitySet<CallLogModel> _callLogs;
        [Association(Storage = "_callLogs", ThisKey = "ID", OtherKey = "CalleeID")]
        public EntitySet<CallLogModel> CallLogs
        {
            get { return _callLogs; }
            set
            {
                if (_callLogs != value)
                {
                    _callLogs = value;
                    RaisePropertyChangedEvent("CallLogs");
                }
            }
        }

        public UserModel(string UserID, string FirstName, string LastName, string UserImagePath)
        {
            _callLogs = new EntitySet<CallLogModel>(new Action<CallLogModel>(this.attachCallLog), new Action<CallLogModel>(this.detachCallLog));
            this.UserID = UserID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.UserImagePath = UserImagePath;
        }

        public UserModel()
        {
            _callLogs = new EntitySet<CallLogModel>(new Action<CallLogModel>(this.attachCallLog), new Action<CallLogModel>(this.detachCallLog));
        }

        private void attachCallLog(CallLogModel cm)
        {
            NotifyPropertyChanging("CallLogModel");
            cm.User = this;
            RaisePropertyChangedEvent("CallLogModel");
        }

        private void detachCallLog(CallLogModel cm)
        {
            NotifyPropertyChanging("CallLogModel");
            cm.User = null;
            RaisePropertyChangedEvent("CallLogModel");
        }
    }
}
