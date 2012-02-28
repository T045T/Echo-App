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
using Echo.Model;
using Caliburn.Micro;
using System.Linq;

namespace Echo.ViewModels
{
    public class ContactDetailsPageViewModel : Screen, INavigationTarget
    {
        private UDCListModel udc;
        private INavigationService navService;

        #region Properties
        public bool ClearBackStack { get; set; }
        public bool Reload { get; set; }

        public int TargetUserID { get; set; }

        private UserModel _User;
        public UserModel User
        {
            get
            {
                return _User;
            }
            set
            {
                if (value != _User)
                {
                    _User = value;
                    NotifyOfPropertyChange("User");
                }
            }
        }

        private CallLogModel _LastCallLog;
        public CallLogModel LastCallLog
        {
            get { return _LastCallLog; }
            set
            {
                _LastCallLog = value;
                NotifyOfPropertyChange("LastCallLog");
            }
        }

        private CallLogEntry _LastCallLogEntry;
        public CallLogEntry LastCallLogEntry
        {
            get
            {
                return _LastCallLogEntry;
            }
            set
            {
                if (value != _LastCallLogEntry)
                {
                    _LastCallLogEntry = value;
                    NotifyOfPropertyChange("LastCallLogEntry");
                }
            }
        }
        #endregion Properties

        public ContactDetailsPageViewModel(INavigationService navService, UDCListModel udc)
        {
            this.udc = udc;
            this.navService = navService;
        }


        protected override void OnActivate()
        {
            base.OnActivate();
            if (ClearBackStack)
            {
                navService.RemoveBackEntry();
                navService.RemoveBackEntry();
            }
            User = udc.GetUser(TargetUserID);
            if (User == null)
                return;
            else
                NotifyOfPropertyChange("User");
            var logList = User.CallLogs.OrderByDescending((log) => log.StartTime);
            if (logList.Any() && logList.First().Entries.Any()) {
                LastCallLog = logList.First();
                LastCallLogEntry = logList.First().Entries.First();
            }
            else
            {
                LastCallLogEntry = new CallLogEntry("You have no recent calls with " + User.FirstLast + ".", DateTime.Now, "[none]");
                LastCallLog = new CallLogModel(User.ID, DateTime.Now);

                LastCallLog.addEntry("Echo park synth fixie, accusamus anim gentrify occaecat photo booth.");
                User.CallLogs.Add(LastCallLog);
                //udc.CallLogTable.InsertOnSubmit(log);
                udc.SubmitChanges();
                //log.addEntry("And now we're testing the wrapping abilities... god, I hope this works...");
                //User.CallLogs.Add(log);
                //udc.SubmitChanges();
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
        }

        public void StartCall()
        {
            navService.UriFor<ActiveCallPageViewModel>()
                .WithParam(x => x.isIncoming, false)
                .WithParam(x => x.calleeID, this.TargetUserID)
                .Navigate();
        }

        public void EditUser()
        {
            navService.UriFor<ContactEditPageViewModel>()
                .WithParam(x => x.TargetUserID, this.User.ID)
                .WithParam(x => x.CreateUser, false)
                .Navigate();
        }

        public void LogTapped(object dataContext)
        {
            var Log = dataContext as CallLogModel;
            navService.UriFor<CallLogPageViewModel>()
                .WithParam(x => x.CallLogID, Log.CallLogID)
                .WithParam(x => x.ComingFromContact, true)
                .Navigate();
        }

        public override void CanClose(Action<bool> callback)
        {
                udc.SubmitChanges();
            callback(true);
        }

    }
}
