using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
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
using Echo.Logic;

namespace Echo.ViewModels
{
    public class SettingsPageViewModel : Screen
    {
        private Connection con;

        private bool Dirty;
        private SettingsModel _SetModel;
        public SettingsModel SetModel
        {
            get { return _SetModel; }
            private set
            {
                if (_SetModel != value)
                {
                    _SetModel = value;
                    NotifyOfPropertyChange("SetModel");
                }
            }

        }

        public override void CanClose(Action<bool> callback)
        {
            if (Dirty)
            {
                SetModel.Save();
                con.reconnect();
            }
            callback(true);
        }

        public SettingsPageViewModel(SettingsModel SetModel, Connection con)
        {
            this.SetModel = SetModel;
            this.con = con;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var tmp = view as Echo.Views.SettingsView;
            //MessageBox.Show(tmp.DataContext.ToString());
        }

        #region general settings
        public bool NameOrderSetting
        {
            get
            {
                return SetModel.getValueOrDefault<bool>(SetModel.NameOrderSettingKeyName, SetModel.NameOrderDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.NameOrderSettingKeyName, value))
                {
                    Dirty = true;
                }
            }
        }

        public bool ImportSetting
        {
            get
            {
                return SetModel.getValueOrDefault<bool>(SetModel.ImportSettingKeyName, SetModel.ImportSettingDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.ImportSettingKeyName, value))
                {
                    Dirty = true;
                }
            }
        }
        public bool STTSetting
        {
            get
            {
                return SetModel.getValueOrDefault<bool>(SetModel.STTSettingKeyName, SetModel.STTSettingDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.STTSettingKeyName, value))
                {
                    Dirty = true;
                }
            }
        }
        public bool BackgroundSetting
        {
            get
            {
                return SetModel.getValueOrDefault<bool>(SetModel.BackgroundSettingKeyName, SetModel.BackgroundSettingDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.BackgroundSettingKeyName, value))
                {
                    Dirty = true;
                }
            }
        }
        public bool RunUnderLockSetting
        {
            get
            {
                return SetModel.getValueOrDefault<bool>(SetModel.RunUnderLockSettingKeyName, SetModel.RunUnderLockDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.RunUnderLockSettingKeyName, value))
                {
                    Dirty = true;
                }
            }
        }
        #endregion

        #region network settings
        public string UsernameSetting
        {
            get
            {
                return SetModel.getValueOrDefault<string>(SetModel.UsernameSettingKeyName, SetModel.UsernameDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.UsernameSettingKeyName, value))
                {
                    Dirty = true;
                    NotifyOfPropertyChange("TestButtonEnabled");
                }
            }
        }
        public string PasswordSetting
        {
            get
            {
                return SetModel.getValueOrDefault<string>(SetModel.PasswordSettingKeyName, SetModel.PasswordDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.PasswordSettingKeyName, value))
                {
                    Dirty = true;
                    NotifyOfPropertyChange("TestButtonEnabled");
                }
            }
        }
        public string ServerSetting
        {
            get
            {
                return SetModel.getValueOrDefault<string>(SetModel.ServerSettingKeyName, SetModel.ServerDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.ServerSettingKeyName, value))
                {
                    Dirty = true;
                    NotifyOfPropertyChange("TestButtonEnabled");
                }
            }
        }
        public int PortSetting
        {
            get
            {
                return SetModel.getValueOrDefault<int>(SetModel.PortSettingKeyName, SetModel.PortDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.PortSettingKeyName, value))
                {
                    Dirty = true;
                    NotifyOfPropertyChange("TestButtonEnabled");
                }
            }
        }

        public string EchoServerSetting
        {
            get
            {
                return SetModel.getValueOrDefault<string>(SetModel.EchoServerSettingKeyName, SetModel.EchoServerDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.EchoServerSettingKeyName, value))
                {
                    Dirty = true;
                    NotifyOfPropertyChange("TestButtonEnabled");
                }
            }
        }
        public int EchoPortSetting
        {
            get
            {
                return SetModel.getValueOrDefault<int>(SetModel.EchoPortSettingKeyName, SetModel.EchoPortDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.EchoPortSettingKeyName, value))
                {
                    Dirty = true;
                    NotifyOfPropertyChange("TestButtonEnabled");
                }
            }
        }

        public bool UseSSLSetting
        {
            get
            {
                return SetModel.getValueOrDefault<bool>(SetModel.UseSSLSettingKeyName, SetModel.UseSSLDefault);
            }
            set
            {
                if (SetModel.AddOrUpdateValue(SetModel.UseSSLSettingKeyName, value))
                {
                    Dirty = true;
                    NotifyOfPropertyChange("TestButtonEnabled");
                }
            }
        }
        #endregion

        public bool TestButtonEnabled
        {
            get { return SetModel.NetworkSettingsAreChanged; }
        }
    }
}
