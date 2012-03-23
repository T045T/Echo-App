using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Echo.Model
{
    /// <summary>
    /// This class models a call log, which in turn contains a number of call log entries and an associated user.
    /// These relations are modeled via a database.
    /// </summary>
    [Table]
    public class CallLogModel : LinqModelBase
    {
        [Column(IsVersion = true)]
        private Binary _version;

        private EntityRef<UserModel> _user;

        // Association with the UserModel table
        [Association(Storage = "_user", OtherKey = "ID", ThisKey = "CalleeID", IsForeignKey = true)]
        public UserModel User
        {
            get { return _user.Entity; }
            set
            {
                _user.Entity = value;
                if (value != null)
                {
                    CalleeID = value.ID;
                }
            }
        }

        private EntitySet<CallLogEntry> logEntries;

        // Association with the call log entries that belong to this call log
        [Association(Storage = "logEntries", ThisKey = "CallLogID", OtherKey = "CallLogID")]
        public EntitySet<CallLogEntry> Entries
        {
            get { return logEntries; }
            private set
            {
                if (logEntries != value)
                {
                    logEntries.Assign(value);
                    NotifyOfPropertyChange("Entries");
                }
            }
        }

        private string _CallLogID;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false)]
        public string CallLogID
        {
            get { return _CallLogID; }
            private set
            {
                if (_CallLogID != value)
                {
                    NotifyPropertyChanging("CallLogID");
                    _CallLogID = value;
                    NotifyOfPropertyChange("CallLogID");
                }
            }
        }

        private DateTime _StartTime;

        [Column(IsDbGenerated = false, DbType = "DATETIME NOT NULL", CanBeNull = false)]
        public DateTime StartTime
        {
            get { return _StartTime; }
            private set
            {
                if (_StartTime != value)
                {
                    NotifyPropertyChanging("StartTime");
                    _StartTime = value;
                    NotifyOfPropertyChange("StartTime");
                }
            }
        }

        private int _CalleeID;

        [Column]
        public int CalleeID
        {
            get { return _CalleeID; }
            set
            {
                if (_CalleeID != value)
                {
                    NotifyPropertyChanging("CalleeID");
                    _CalleeID = value;
                    NotifyOfPropertyChange("CalleeID");
                }
            }
        }

        [Column]
        public string Snippet { get; set; }
        


        public CallLogModel(int CalleeID, DateTime StartTime, string CallLogID)
        {
            this.CalleeID = CalleeID;
            this.StartTime = StartTime;
            this.logEntries = new EntitySet<CallLogEntry>(this.attachLog, this.detachLog);
            this._user = new EntityRef<UserModel>();
            this.CallLogID = CallLogID;
        }
        public CallLogModel(int CalleeID, DateTime StartTime)
        {
            this.CalleeID = CalleeID;
            this.StartTime = StartTime;
            this.logEntries = new EntitySet<CallLogEntry>(this.attachLog, this.detachLog);
            this._user = new EntityRef<UserModel>();
            this.CallLogID = CalleeID.ToString() + "_" + StartTime.ToLongTimeString();
        }
        public CallLogModel(int CalleeID)
        {
            this.CalleeID = CalleeID;
            this.StartTime = DateTime.Now;
            this.logEntries = new EntitySet<CallLogEntry>(this.attachLog, this.detachLog);
            this._user = new EntityRef<UserModel>();
            this.CallLogID = CalleeID.ToString() + "_" + DateTime.Now.ToLongTimeString();
        }
        public CallLogModel()
        {
            this.logEntries = new EntitySet<CallLogEntry>(this.attachLog, this.detachLog);
            this._user = new EntityRef<UserModel>();
            this.CallLogID = "[DBGenerated]" + DateTime.Now.ToLongTimeString();
        }

        // attachLog and detachLog are invoked whenever something is added to or removed from the CallLogEntry EntitySet.
        // They take care of maintaining the proper links between log entries and log objects.
        public void attachLog(CallLogEntry e)
        {
            NotifyPropertyChanging("CallLogEntry");
            e.CallLogID = this.CallLogID;
            NotifyOfPropertyChange("CallLogEntry");
        }
        public void detachLog(CallLogEntry e)
        {
            NotifyPropertyChanging("CallLogEntry");
            e.CallLog = null;
            NotifyOfPropertyChange("CallLogEntry");
        }

        public void addEntry(CallLogEntry e)
        {
            if (e.Timestamp.CompareTo(this.StartTime) >= 0 && e.CallLogID.Equals(this.CallLogID))
            {
                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{
                    this.Entries.Add(e);
                    if (Snippet == null || Snippet.Length == 0)
                        Snippet = e.Content;
                //});
            }
        }
        public void addEntry(string content, DateTime timestamp)
        {
            // only add the entry if the timestamp is equal to or later than the start timestamp
            if (timestamp.CompareTo(this.StartTime) >= 0)
            {
                addEntry(new CallLogEntry(content, timestamp, this.CallLogID));
            }
        }
        public void addEntry(string content)
        {
            addEntry(new CallLogEntry(content, DateTime.Now, this.CallLogID));
        }
    }
}
