using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Echo.Model
{
    [Table]
    public class CallLogEntry : LinqModelBase
    {
        [Column(IsVersion = true)]
        private Binary _version;

        [Column(IsPrimaryKey = true, CanBeNull = false, DbType = "INT NOT NULL Identity", IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID;

        private string _Content;
        [Column]
        public string Content
        {
            get { return _Content; }
            private set
            {
                NotifyPropertyChanging("Content");
                _Content = value;
                RaisePropertyChangedEvent("Content");
            }
        }

        private DateTime _Timestamp;
        [Column(IsPrimaryKey = true)]
        public DateTime Timestamp
        {
            get { return _Timestamp; }
            private set
            {
                NotifyPropertyChanging("Timestamp");
                _Timestamp = value;
                RaisePropertyChangedEvent("Timestamp");
            }
        }

        private string _CallLogID;
        [Column]
        public string CallLogID
        {
            get { return _CallLogID; }
            set
            {
                NotifyPropertyChanging("CallLogID");
                _CallLogID = value;
                RaisePropertyChangedEvent("CallLogID");
            }
        }

        private double _Confidence;
        [Column]
        public double Confidence
        {
            get { return _Confidence; }
            set
            {
                if (value != _Confidence)
                {
                    NotifyPropertyChanging("Confidence");
                    _Confidence = value;
                    RaisePropertyChangedEvent("Confidence");
                }
            }
        }

        private EntityRef<CallLogModel> _log;

        [Association(Storage = "_log", ThisKey = "CallLogID", OtherKey = "CallLogID", IsForeignKey = true)]
        public CallLogModel CallLog
        {
            get { return _log.Entity; }
            set
            {
                if ((_log.Entity != null && !_log.Entity.Equals(value)) || _log.Entity == null)
                {
                    NotifyPropertyChanging("CallLog");
                    _log.Entity = value;
                    RaisePropertyChangedEvent("CallLog");
                    if (value != null)
                    {
                        CallLogID = value.CallLogID;
                    }
                }
            }
        }

        public bool Dubious
        {
            // 80% confidence is the threshold for a trustworthy entry.
            get { return Confidence < 0.8; }
        }

        public CallLogEntry(string Content, DateTime Timestamp, string CallLogID)
        {
            this.CallLogID = CallLogID;
            this.Content = Content;
            this.Timestamp = Timestamp;
            this.Confidence = 1.0;
        }
        public CallLogEntry(string Content, DateTime Timestamp, string CallLogID, double Confidence)
        {
            this.CallLogID = CallLogID;
            this.Content = Content;
            this.Timestamp = Timestamp;
            this.Confidence = Confidence;
        }
        public CallLogEntry()
        {
            this._log = new EntityRef<CallLogModel>();
        }
    }
}
