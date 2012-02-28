using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Caliburn.Micro;

using System.Collections.ObjectModel;
using System.Windows.Threading;
using System;
using System.Collections.Generic;

using g711audio;

namespace Echo
{
    public class MainPageViewModel : Screen
    {
        #region Properties
        public bool ULAW { get; set; }
        public bool ALAW { get; set; }

        private List<byte> _InputBytes;
        public List<byte> InputBytes
        {
            get { return _InputBytes; }
            set
            {
                _InputBytes = value;
                NotifyOfPropertyChange("InputBytes");
            }
        }
        private List<byte> _G711Bytes;
        public List<byte> G711Bytes
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

        public MainPageViewModel()
        {
            _Sending = false;

            microphone = Microphone.Default;
            stream = new MemoryStream();

            // Timer to simulate the XNA Game Studio game loop (Microphone is from XNA Game Studio)
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(50);
            dt.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            dt.Start();

            microphone.BufferReady += new EventHandler<EventArgs>(microphone_BufferReady);
        }

        void microphone_BufferReady(object sender, EventArgs e)
        {
            microphone.GetData(buffer);
            //stream.Write(buffer, 0, buffer.Length);
                InputBytes.AddRange(buffer);
                G711Bytes.AddRange(ALawEncoder.ALawEncode(buffer));
            //InputBytes.AddRange(buffer);
        }

        public void StartSending()
        {
            _Sending = true;
            microphone.BufferDuration = TimeSpan.FromMilliseconds(1000);
            buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];
            microphone.Start();
        }
        public bool canStartSending() { return !_Sending; }
        public void StopSending()
        {
            _Sending = false;
            microphone.Stop();
        }
        public bool canStopSending() { return _Sending; }
    }
}


