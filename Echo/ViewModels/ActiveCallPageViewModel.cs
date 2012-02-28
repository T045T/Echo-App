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
using Caliburn.Micro;
using Echo.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace Echo.ViewModels
{
    public class ActiveCallPageViewModel : Screen
    {

        private UDCListModel udc;
        private INavigationService navService;

        #region Properties
        public int calleeID { get; set; }
        public bool isIncoming { get; set; }

        private bool _CallInProgress;
        public bool CallInProgress
        {
            get { return _CallInProgress; }
            set
            {
                if (value != _CallInProgress)
                {
                    _CallInProgress = value;
                    NotifyOfPropertyChange("CallInProgress");
                }
            }
        }

        private UserModel _Callee;
        public UserModel Callee
        {
            get { return _Callee; }
            set
            {
                _Callee = value;
                NotifyOfPropertyChange("Callee");
            }
        }

        private DateTime _CallStart;
        public DateTime CallStart
        {
            get { return _CallStart; }
            set
            {
                _CallStart = value;
                NotifyOfPropertyChange("CallStart");
            }
        }

        public string TimeElapsed
        {
            get
            {
                var timeSpan = (DateTime.Now - CallStart);
                return String.Format("{0:00{:};;}{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
        }

        private CallLogModel _CurrentCallLog;
        public CallLogModel CurrentCallLog
        {
            get { return _CurrentCallLog; }
            set
            {
                _CurrentCallLog = value;
                NotifyOfPropertyChange("CurrentCallLog");
            }
        }

        private CallLogModel _PreviousCallLog;
        public CallLogModel PreviousCallLog
        {
            get { return _PreviousCallLog; }
            set
            {
                _PreviousCallLog = value;
                NotifyOfPropertyChange("PreviousCallLog");
            }
        }

        private bool _ServerAnalyzing;
        public bool ServerAnalyzing
        {
            get { return _ServerAnalyzing; }
            set
            {
                if (value != _ServerAnalyzing)
                {
                    _ServerAnalyzing = value;
                    NotifyOfPropertyChange("ServerAnalyzing");
                }
            }
        }
        #endregion

        public ActiveCallPageViewModel(INavigationService navService, UDCListModel udc)
        {
            this.udc = udc;
            this.navService = navService;
            CallInProgress = false;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Callee = udc.GetUser(calleeID);
            if (Callee == null) return;
            var previousLogs = from CallLogModel clm in udc.DataContext.CallLogTable where clm.CalleeID == this.calleeID orderby clm.StartTime descending select clm;
            List<CallLogModel> tmp = new List<CallLogModel>(previousLogs);
            if (previousLogs.Any())
            {
                PreviousCallLog = previousLogs.First();
            }
            CurrentCallLog = new CallLogModel(Callee.ID);
            CallStart = CurrentCallLog.StartTime;

            CurrentCallLog.addEntry("Helvetica salvia keytar, tattooed lo-fi eiusmod freegan DIY bespoke sed pop-up mlkshk small batch four loko brunch.");
            Callee.CallLogs.Add(CurrentCallLog);
            udc.SubmitChanges();
            if (isIncoming)
            {
                // Answer call
            }
            else
            {
                // start call
            }

            CallInProgress = true;
        }

        public bool CanEndCall()
        {
            return CallInProgress;
        }

        public void EndCall()
        {
            // Actually end the call
            CallInProgress = false;
            navService.GoBack();
        }

        public override void CanClose(Action<bool> callback)
        {
            udc.SubmitChanges();
            udc.LoadListsFromDatabase();
            CallInProgress = false;
            callback(true);
        }
    }
}
