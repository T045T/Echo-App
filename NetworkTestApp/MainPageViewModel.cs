using Caliburn.Micro;

using System.Collections.ObjectModel;
namespace Echo {
    public class MainPageViewModel : Screen {
        public bool ULAW { get; set; }
        public bool ALAW { get; set; }

        private ObservableCollection<byte> _InputBytes;
        public ObservableCollection<byte> InputBytes
        {
            get { return _InputBytes; }
            set
            {
                _InputBytes = value;
                NotifyOfPropertyChange("InputBytes");
            }
        }
        private ObservableCollection<byte> _G711Bytes;
        public ObservableCollection<byte> G711Bytes
        {
            get { return _G711Bytes; }
            set
            {
                _G711Bytes = value;
                NotifyOfPropertyChange("G711Bytes");
            }
        }

        private bool _Sending;

        public MainPageViewModel()
        {
            _Sending = false;
        }

        public void StartSending()
        {
            _Sending = true;
        }
        public bool canStartSending() { return !_Sending; }
        public void StopSending()
        {
            _Sending = false;
        }
        public bool canStopSending() { return _Sending; }
    }
}


