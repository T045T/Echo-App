﻿using System;
using Caliburn.Micro;
using Echo.Model;

namespace Echo.ViewModels
{
    public class IncomingCallDialogViewModel : Screen
    {
        private UDCListModel udc;
        private IVibrateController vibCon;

        public bool Answered { get; set; }
        public int CallerID { get; set; }

        private UserModel _Caller;
        public UserModel Caller {
            get { return _Caller; }
            set {
                _Caller = value;
                NotifyOfPropertyChange("Caller");
            }
        }

        public IncomingCallDialogViewModel(UDCListModel udc, IVibrateController vibCon)
        {
            Answered = false;
            this.vibCon = vibCon;
            this.udc = udc;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            vibCon.Start(TimeSpan.FromMilliseconds(450));
            Caller = udc.GetUser(CallerID);
        }

        public void Answer()
        {
            Answered = true;
            TryClose();
        }

        public void Decline()
        {
            Answered = false;
            TryClose();
        }
    }
}
