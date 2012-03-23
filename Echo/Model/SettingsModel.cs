using System;
using System.IO.IsolatedStorage;
using Caliburn.Micro;

namespace Echo.Model
{
    public class SettingsModel : PropertyChangedBase
    {
        IsolatedStorageSettings settings;

        // storage key names
        const string _UsernameSettingKeyName = "UsernameSetting";
        const string _PasswordSettingKeyName = "PasswordSetting";
        const string _ServerSettingKeyName = "ServerSetting";
        const string _PortSettingKeyName = "PortSetting";
        const string _EchoServerSettingKeyName = "EchoServerSetting";
        const string _EchoPortSettingKeyName = "EchoPortSetting";
        const string _UseSSLSettingKeyName = "UseSSLSetting";
        const string _STTSettingKeyName = "STTSetting";
        const string _BackgroundSettingKeyName = "BGSetting";
        const string _RunUnderLockSettingKeyName = "RunUnderLockSetting";
        const string _ImportSettingKeyName = "ImportSetting";
        const string _NameOrderSettingKeyName = "NameOrderSetting";
        const string _ShowWelcomeScreenSettingKeyName = "ShowWelcomeScreen";

        // keyName Properties
        public string UsernameSettingKeyName { get { return _UsernameSettingKeyName; } }
        public string PasswordSettingKeyName { get { return _PasswordSettingKeyName; } }
        public string ServerSettingKeyName { get { return _ServerSettingKeyName; } }
        public string PortSettingKeyName { get { return _PortSettingKeyName; } }
        public string EchoServerSettingKeyName { get { return _EchoServerSettingKeyName; } }
        public string EchoPortSettingKeyName { get { return _EchoPortSettingKeyName; } }
        public string UseSSLSettingKeyName { get { return _UseSSLSettingKeyName; } }
        public string STTSettingKeyName { get { return _STTSettingKeyName; } }
        public string BackgroundSettingKeyName { get { return _BackgroundSettingKeyName; } }
        public string RunUnderLockSettingKeyName { get { return _RunUnderLockSettingKeyName; } }
        public string ImportSettingKeyName { get { return _ImportSettingKeyName; } }
        public string NameOrderSettingKeyName { get { return _NameOrderSettingKeyName; } }
        public string ShowWelcomeScreenSettingKeyName { get { return _ShowWelcomeScreenSettingKeyName; } }

        // Default values
        const bool _STTSettingDefault = true;
        const bool _BackgroundSettingDefault = true;
        const bool _RunUnderLockDefault = true;
        const bool _UseSSLDefault = false;
        const bool _ImportSettingDefault = false;
        const string _UsernameDefault = "";
        const string _PasswordDefault = "";
        const string _ServerDefault = "1.1.1.1";
        const int _PortDefault = 3246;
        const string _EchoServerDefault = "server.echo-app.org";
        const int _EchoPortDefault = 1123;
        const bool _NameOrderDefault = true; // true -> "First Last", false -> "Last, First"
        const bool _ShowWelcomeScreenDefault = true;


        // defaultValue Properties
        public bool STTSettingDefault { get { return _STTSettingDefault; } }
        public bool BackgroundSettingDefault { get { return _BackgroundSettingDefault; } }
        public bool RunUnderLockDefault { get { return _RunUnderLockDefault; } }
        public bool UseSSLDefault { get { return _UseSSLDefault; } }
        public bool ImportSettingDefault { get { return _ImportSettingDefault; } }
        public string UsernameDefault { get { return _UsernameDefault; } }
        public string PasswordDefault { get { return _PasswordDefault; } }
        public string ServerDefault { get { return _ServerDefault; } }
        public int PortDefault { get { return _PortDefault; } }
        public string EchoServerDefault { get { return _ServerDefault; } }
        public int EchoPortDefault { get { return _PortDefault; } }
        public bool NameOrderDefault { get { return _NameOrderDefault; } }
        public bool ShowWelcomeScreenDefault { get { return _ShowWelcomeScreenDefault; } }

        // value Properties
        /// <summary>
        /// Boolean value: true means "First Last", false means "Last, First"
        /// </summary>
        public bool NameOrder { get { return this.getValueOrDefault<bool>(NameOrderSettingKeyName, NameOrderDefault); } }
        public SettingsModel()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        // Special constructor for testing use - should only be accessed in Echo.test
        internal SettingsModel(bool clearSettings)
        {
            IsolatedStorageSettings.ApplicationSettings.Clear();
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public bool AddOrUpdateValue(string key, Object value)
        {
            bool valueChanged = false;
            if (settings.Contains(key))
            {
                if (settings[key] != value)
                {
                    settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        public T getValueOrDefault<T>(string key, T defaultValue)
        {
            T value;
            if (settings.Contains(key))
            {
                value = (T)settings[key];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        public void Save()
        {
            settings.Save();
        }


        public bool NetworkSettingsAreChanged
        {
            get
            {
                return !getValueOrDefault<string>(ServerSettingKeyName, ServerDefault).Equals(ServerDefault)
                    ||  getValueOrDefault<int>(PortSettingKeyName, PortDefault) != PortDefault
                    || !getValueOrDefault<string>(UsernameSettingKeyName, UsernameDefault).Equals(UsernameDefault)
                    || !getValueOrDefault<string>(PasswordSettingKeyName, PasswordDefault).Equals(PasswordDefault)
                    || !getValueOrDefault<string>(EchoServerSettingKeyName, EchoServerDefault).Equals(EchoServerDefault)
                    ||  getValueOrDefault<int>(EchoPortSettingKeyName, EchoPortDefault) != EchoPortDefault;
            }
        }
    }
}
