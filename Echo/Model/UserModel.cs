using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                    NotifyOfPropertyChange("ID");
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
                    NotifyOfPropertyChange("UserID");
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
                    NotifyOfPropertyChange("FirstName");
                    NotifyOfPropertyChange("FirstLast");
                    NotifyOfPropertyChange("LastFirst");
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
                    NotifyOfPropertyChange("LastName");
                    NotifyOfPropertyChange("FirstLast");
                    NotifyOfPropertyChange("LastFirst");
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
                    NotifyOfPropertyChange("UserImagePath");
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
                    NotifyOfPropertyChange("CallLogs");
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
            NotifyOfPropertyChange("CallLogModel");
        }

        private void detachCallLog(CallLogModel cm)
        {
            NotifyPropertyChanging("CallLogModel");
            cm.User = null;
            NotifyOfPropertyChange("CallLogModel");
        }
    }
}
