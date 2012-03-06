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
using Echo.Logic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Echo.ViewModels
{
    public class ActiveCallPageViewModel : Screen
    {

        private UDCListModel udc;
        private Connection con;
        private SettingsModel setModel;
        private INavigationService navService;
        private UDPAudioSink UDPSink;

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

        public Connection Con { get { return con; } }

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

        private DispatcherTimer TimeUpdater;
        public string TimeElapsed
        {
            get
            {
                var timeSpan = (DateTime.Now - CallStart);
                return String.Format("{0:00:;;''}{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
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

        #endregion

        public ActiveCallPageViewModel(INavigationService navService, UDCListModel udc, Connection con, SettingsModel setModel)
        {
            this.setModel = setModel;
            this.udc = udc;
            this.navService = navService;
            this.con = con;
            CallInProgress = false;
            con.DataReceived += new DataReceivedEventHandler(con_DataReceived);
            con.AcquiredPort += new AcquiredPortEventHandler(con_AcquiredPort);
            // TODO Should be a DispatcherTimer =)
            TimeUpdater = new DispatcherTimer();
            TimeUpdater.Interval = TimeSpan.FromSeconds(1);
            TimeUpdater.Tick += new EventHandler(TimeUpdater_Tick);

            this.UDPSink = new UDPAudioSink(true);
        }

        void TimeUpdater_Tick(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NotifyOfPropertyChange("TimeElapsed"));
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
            if (isIncoming)
            {
                // Answer call
                    con.pickupCall();
            }
            else
            {
                con.call(Callee.UserID);
                // start call
            }

            CallInProgress = true;
        }

        private void StartCall()
        {
            CurrentCallLog = new CallLogModel(Callee.ID);
            CallStart = CurrentCallLog.StartTime;
            TimeUpdater.Start();

            Callee.CallLogs.Add(CurrentCallLog);
            udc.SubmitChanges();

            CallInProgress = true;
        }

        void con_AcquiredPort(object sender, int e)
        {
            this.UDPSink.StopSending();
            this.UDPSink.Port = e;
            if (isIncoming)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    con.pickupCall();
                    this.UDPSink.StartSending(setModel.getValueOrDefault<string>(setModel.EchoServerSettingKeyName, setModel.EchoServerDefault));
                    StartCall();
                });
            }
            else
            {
                con.CallStarted += new CallStartedHandler(con_CallStarted);
            }
        }

        void con_CallStarted(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.UDPSink.StartSending(setModel.getValueOrDefault<string>(setModel.EchoServerSettingKeyName, setModel.EchoServerDefault));
                StartCall();
                con.CallStarted -= new CallStartedHandler(con_CallStarted);
            });
        }

        void con_DataReceived(object sender, string e)
        {
            CurrentCallLog.addEntry(e);
        }

        public bool CanEndCall
        {
            get { return CallInProgress; }
        }

        public void EndCall()
        {
            // Actually end the call
            con.hangup();
            UDPSink.StopSending();
            CallInProgress = false;
            TimeUpdater.Stop();
            navService.GoBack();
        }

        public override void CanClose(Action<bool> callback)
        {
            if (!CallInProgress)
            {
                udc.SubmitChanges();
                udc.LoadListsFromDatabase();
                CallInProgress = false;
                UDPSink.StopSending();
                callback(true);
            }
            else
            {
                callback(false);
            }
        }
    }
}
