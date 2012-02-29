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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using g711audio;
using System.Collections.ObjectModel;

namespace Echo.ViewModels
{
    public class NetworkTestPageViewModel : Screen
    {
        #region Properties
        public bool ULAW { get; set; }
        public bool ALAW { get; set; }

        private ObservableCollection<ByteHolder> _InputBytes;
        public ObservableCollection<ByteHolder> InputBytes
        {
            get { return _InputBytes; }
            set
            {
                _InputBytes = value;
                NotifyOfPropertyChange("InputBytes");
            }
        }
        private ObservableCollection<ByteHolder> _G711Bytes;
        public ObservableCollection<ByteHolder> G711Bytes
        {
            get { return _G711Bytes; }
            set
            {
                _G711Bytes = value;
                NotifyOfPropertyChange("G711Bytes");
            }
        }
        #endregion
        private bool _Sending;

        Microphone microphone;
        byte[] buffer;
        MemoryStream stream;
        SoundEffect sound;

        public NetworkTestPageViewModel()
        {
            _Sending = false;

            microphone = Microphone.Default;
            stream = new MemoryStream();

            InputBytes = new ObservableCollection<ByteHolder>();
            G711Bytes = new ObservableCollection<ByteHolder>();
            StartTimer();
            

            microphone.BufferReady += new EventHandler<EventArgs>(microphone_BufferReady);
        }

        public void StartTimer()
        {
            // Timer to simulate the XNA Game Studio game loop (Microphone is from XNA Game Studio)
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(50);
            dt.Tick += delegate
            {
                try
                {
                    FrameworkDispatcher.Update();
                }
                catch { }
            };
            dt.Start();
        }

        public void ConnectSocket() {



        void microphone_BufferReady(object sender, EventArgs e)
        {
            microphone.GetData(buffer);
            stream.Write(buffer, 0, buffer.Length);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                updateView(buffer);
            });
        }

        public void updateView(byte[] buffer)
        {
            DateTime tmp = DateTime.Now;
            byte[] tmpBytes = ALawEncoder.ALawEncode(buffer);
            TimeSpan time = DateTime.Now - tmp;
            

            InputBytes.Add(new ByteHolder(buffer));
            //NotifyOfPropertyChange("InputBytes");
            G711Bytes.Add(new ByteHolder(ALawEncoder.ALawEncode(buffer)));
            //NotifyOfPropertyChange("G711Bytes");
        }

        public void StartSending()
        {
            _Sending = true;
            NotifyOfPropertyChange("CanStartSending");
            NotifyOfPropertyChange("CanStopSending");
            microphone.BufferDuration = TimeSpan.FromMilliseconds(1000);
            buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];
            microphone.Start();
        }
        public bool CanStartSending { get { return !_Sending; } }
        public void StopSending(object foo)
        {
            _Sending = false;
            NotifyOfPropertyChange("CanStartSending");
            NotifyOfPropertyChange("CanStopSending");
            microphone.Stop();
        }
        public bool CanStopSending { get { return _Sending; } }
    }

    public class ByteHolder : Screen
    {
        private string _ByteString;
        public string ByteString
        {
            get { return _ByteString; }
            set
            {
                if (value != _ByteString)
                {
                    _ByteString = value;
                    NotifyOfPropertyChange("ByteString");
                }
            }
        }

        public ByteHolder(byte[] buffer)
        {
            ByteString = BitConverter.ToString(buffer);
        }
    }
}
