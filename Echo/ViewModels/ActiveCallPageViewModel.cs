using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Echo.Logic;
using Echo.Model;
using Echo.Helpers;

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

        private string _CurrentText;
        public string CurrentText
        {
            get { return _CurrentText; }
            set
            {
                _CurrentText = value;
                NotifyOfPropertyChange("CurrentText");
            }
        }
        private string _LastText;
        public string LastText
        {
            get { return _LastText; }
            set
            {
                _LastText = value;
                NotifyOfPropertyChange("LastText");
            }
        }

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
                    NotifyOfPropertyChange("CanEndCall");
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
            con.RemoteHangup += new RemoteHangupEventHandler((obj, e) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    CallInProgress = false;
                    EndCall();
                });
            });
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
                if (PreviousCallLog.Entries.Any())
                {
                    LastText = PreviousCallLog.Entries.First().Content;
                }
            }
            StartCall();
        }

        private void StartCall()
        {
            #region insert_test_content
            //CurrentCallLog = new CallLogModel(Callee.ID);
            //CallStart = CurrentCallLog.StartTime;
            //TimeUpdater.Start();
            //Callee.CallLogs.Add(CurrentCallLog);
            //CurrentCallLog.addEntry("Sorry, I'm going to be late tonight.");
            //CurrentCallLog.addEntry("Can you get some groceries before the store closes?");
            //CurrentCallLog.addEntry("Thank you so much!");
            //CurrentCallLog.addEntry("See you soon.");
            #endregion insert_test_content

            udc.SubmitChanges();
            udc.LoadListsFromDatabase();
            if (isIncoming)
            {
                if (con.VoicePort != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            PrepareCall((int)con.VoicePort);
                            con.pickupCall();
                            StartTransmission();
                        });
                }
                else
                {
                    con.AcquiredPort += new AcquiredPortEventHandler(con_AcquiredPort);
                }
            }
            else
            {
                con.call(Callee.UserID);
                con.CallStarted += new CallStartedHandler(con_CallStarted);
            }
        }

        private void PrepareCall(int Port)
        {
            CurrentCallLog = new CallLogModel(Callee.ID);
            CallStart = CurrentCallLog.StartTime;
            TimeUpdater.Start();

            Callee.CallLogs.Add(CurrentCallLog);
            udc.SubmitChanges();

            CallInProgress = true;


            this.UDPSink.StopSending();
            this.UDPSink.Port = Port;
        }
        public void StartTransmission()
        {
            this.UDPSink.StartSending(setModel.getValueOrDefault<string>(setModel.EchoServerSettingKeyName, setModel.EchoServerDefault));
            CallInProgress = true;
        }

        void con_AcquiredPort(object sender, int e)
        {
            if (isIncoming)
            {
                con.AcquiredPort -= new AcquiredPortEventHandler(con_AcquiredPort);
                Deployment.Current.Dispatcher.BeginInvoke(() => StartCall());
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    PrepareCall((int)con.VoicePort);
                                    con.CallStarted -= new CallStartedHandler(con_CallStarted);
                                    StartTransmission();
                                });
            }
        }

        void con_CallStarted(object sender, EventArgs e)
        {
            if (con.VoicePort != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    PrepareCall((int)con.VoicePort);
                    con.CallStarted -= new CallStartedHandler(con_CallStarted);
                    StartTransmission();
                });
            }
            else
            {
                con.AcquiredPort += new AcquiredPortEventHandler(con_AcquiredPort);
            }
        }

        void con_DataReceived(object sender, string e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (e.Length > 0)
                {
                    CurrentCallLog.addEntry(e);
                    LastText = CurrentText;
                    CurrentText = e;
                }
            });
        }

        public bool CanEndCall
        {
            get { return CallInProgress; }
        }

        private bool testBool;
        public bool TestBool
        {
            get { return testBool; }
            set
            {
                if (value != testBool)
                {
                    testBool = value;
                    NotifyOfPropertyChange("TestBool");
                }
            }
        }

        public void EndCall()
        {
            // Actually end the call
            if (CallInProgress)
            {
                UDPSink.StopSending();
                CallInProgress = false;
                con.hangup();
            }
            TimeUpdater.Stop();
            navService.GoBack();
        }

        public override void CanClose(Action<bool> callback)
        {
            if (!CallInProgress)
            {
                udc.SubmitChanges();
                udc.LoadListsFromDatabase();
                UDPSink.StopSending();
                callback(true);
                //con.hangup();
            }
            else
            {
                callback(false);
            }
        }
    }
}
