using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using g711audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

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

        private string _IP_Address;
        public string Server
        {
            get { return _IP_Address; }
            set
            {
                if (value != _IP_Address)
                {
                    _IP_Address = value;
                    NotifyOfPropertyChange("IP_Address");
                }
            }
        }

        private string _Port;
        public string Port
        {
            get { return _Port; }
            set
            {
                if (value != _Port)
                {
                    _Port = value;
                    NotifyOfPropertyChange("Port");
                }
            }
        }
        #endregion
        private bool _Sending;

        Socket mySocket;
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

        public void ConnectSocket()
        {
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress address = IPAddress.Parse(Server);
            int port = int.Parse(Port);
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs()
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse(Server), int.Parse(Port))
            };
            socketEventArg.Completed += OnConnectCompleted;
            mySocket.ConnectAsync(socketEventArg);
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _Sending = true;
                NotifyOfPropertyChange("CanStartSending");
                NotifyOfPropertyChange("CanStopSending");
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Connection Error: " + e.SocketError.ToString());
                }); 
            }

        }

        public void SendBytes(byte[] buffer) {
            if (mySocket != null) {
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs()
                {
                    RemoteEndPoint = new IPEndPoint(IPAddress.Parse(Server), int.Parse(Port))
                };
                socketEventArg.SetBuffer(buffer, 0, buffer.Length);
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(socketEventArg_Completed);
                mySocket.SendAsync(socketEventArg);
            }
        }

        void socketEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                int foo = 0;
            }
        }



        void microphone_BufferReady(object sender, EventArgs e)
        {
            if (!_Sending) return;
            microphone.GetData(buffer);
            stream.Write(buffer, 0, buffer.Length);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (ALAW)
                {
                    SendBytes(ALawEncoder.ALawEncode(buffer));
                }
                else if (ULAW)
                {
                    SendBytes(MuLawEncoder.MuLawEncode(buffer));
                }
            });
        }

        public void updateView(byte[] buffer)
        {
            if (ALAW)
            {
                byte[] tmpBytes = ALawEncoder.ALawEncode(buffer);
            }
            else if (ULAW)
            {
                byte[] tmpBytes = MuLawEncoder.MuLawEncode(buffer);
            }
            else
            {
                return;
            }
        }

        public void StartSending()
        {
            ConnectSocket();
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

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Server = "192.168.178.36";
            Port = "1135";
        }
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
