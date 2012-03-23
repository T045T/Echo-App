using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Echo.Model;

namespace Echo.ViewModels
{
    public class CallLogPageViewModel :Screen
    {
        public string CallLogID { get; set; }
        public bool ComingFromContact { get; set; }

        #region Properties
        private CallLogModel _CallLog;
        public CallLogModel CallLog
        {
            get { return _CallLog; }
            set
            {
                _CallLog = value;
                NotifyOfPropertyChange("CallLog");
            }
        }
        private UserModel _User;
        public UserModel User
        {
            get { return _User; }
            set
            {
                _User = value;
                NotifyOfPropertyChange("User");
            }
        }
        private ObservableCollection<CallLogEntry> _OldEntries;
        public ObservableCollection<CallLogEntry> OldEntries
        {
            get { return _OldEntries; }
            set
            {
                _OldEntries = value;
                NotifyOfPropertyChange("OldEntries");
            }
        }
        #endregion Properties

        private INavigationService navService;
        private UDCListModel udc;

        public CallLogPageViewModel(INavigationService navService, UDCListModel udc)
        {
            this.navService = navService;
            this.udc = udc;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var log = from CallLogModel clm in udc.DataContext.CallLogTable 
                      where clm.CallLogID.Equals(CallLogID) 
                      select clm;
            if (log.Any()) {
                CallLog = log.First();
            } else {
                return;
            }
            User = CallLog.User;

            // Select all the call logs from the 7 days before the requested Log
            var olderLogs = from CallLogModel clm in User.CallLogs 
                            where (clm.StartTime < CallLog.StartTime) 
                                && (CallLog.StartTime - clm.StartTime <= TimeSpan.FromDays(7))
                            orderby clm.StartTime
                            select clm;
            var tmpCollection = new List<CallLogEntry>();
            foreach (CallLogModel clm in olderLogs) {
                tmpCollection.AddRange(clm.Entries);
            }
            tmpCollection.OrderBy(x => x.Timestamp);
            OldEntries = new ObservableCollection<CallLogEntry>(tmpCollection);
        }

        public void GotoContactPage()
        {
            if (ComingFromContact)
            {
                navService.GoBack();
            }
            else
            {
                navService.UriFor<ContactDetailsPageViewModel>()
                    .WithParam(x => x.TargetUserID, User.ID)
                    .Navigate();
            }
        }
    }
}
