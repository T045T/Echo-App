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
            var logList = User.CallLogs.OrderBy((log) => log.StartTime);
            if (logList.Any() && logList.First().Entries.Any()) {
                LastCallLogEntry = logList.First().Entries.First();
            }
            else
            {
                LastCallLogEntry = new CallLogEntry("You have no recent calls with " + User.FirstLast + ".", DateTime.Now, "[none]");
                var log = new CallLogModel(User.ID, DateTime.Now);

                log.addEntry("Echo park synth fixie, accusamus anim gentrify occaecat photo booth.");
                User.CallLogs.Add(log);
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

        public void EditUser()
        {
            navService.UriFor<ContactEditPageViewModel>()
                .WithParam(x => x.TargetUserID, this.User.ID)
                .WithParam(x => x.CreateUser, false)
                .Navigate();
        }

        public override void CanClose(Action<bool> callback)
        {
                udc.SubmitChanges();
            callback(true);
        }

    }
}
