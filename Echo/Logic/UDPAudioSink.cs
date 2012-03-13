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
using System.Net.Sockets;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Caliburn.Micro;
using g711audio;

namespace Echo.Logic
{
    public class UDPAudioSink : Screen
    {
        private SocketAsyncEventArgs socketEventArg;

        private bool _Sending;
        private bool ALAW;

        private string Server;
        private int? _Port;
        public int? Port
        {
            get { return _Port; }
            set
            {
                if (value != _Port)
                {
                    _Port = value;
                }
            }
        }

        private DispatcherTimer dt;

        Socket mySocket;
        Microphone microphone;
        byte[] buffer;
        byte[] downSampleBuffer;
        MemoryStream stream;


        public UDPAudioSink(bool ALaw)
        {
            _Sending = false;
            ALAW = ALaw;

            dt = new DispatcherTimer();
            microphone = Microphone.Default;
            stream = new MemoryStream();

            microphone.BufferReady += new EventHandler<EventArgs>(microphone_BufferReady);
        }
        public void StartTimer()
        {
            // Timer to simulate the XNA Game Studio game loop (Microphone is from XNA Game Studio)

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

        public void ConnectSocket(string AddressOrDns, int Port)
        {
            StartTimer();
            this.Server = AddressOrDns;
            this.Port = Port;
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            int port = Port;
            socketEventArg = new SocketAsyncEventArgs();
            IPAddress Server;
            if (IPAddress.TryParse(AddressOrDns, out Server))
            {
                socketEventArg.RemoteEndPoint = new IPEndPoint(Server, Port);
            }
            else
            {
                socketEventArg.RemoteEndPoint = new DnsEndPoint(AddressOrDns, Port);
            }

            socketEventArg.Completed += OnConnectCompleted;
            mySocket.ConnectAsync(socketEventArg);
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            socketEventArg.Completed -= OnConnectCompleted;
            if (e.SocketError == SocketError.Success)
            {
                _Sending = true;

            }
            else
            {
                // Error
            }

        }

        public bool StartSending(string AddressOrDns)
        {
            if (Port != null)
            {
                ConnectSocket(AddressOrDns, (int)Port);
                microphone.BufferDuration = TimeSpan.FromMilliseconds(150);
                buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];
                downSampleBuffer = new byte[buffer.Length / 2];
                microphone.Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public void StopSending()
        {
            _Sending = false;
            Port = null;
            NotifyOfPropertyChange("CanStartSending");
            NotifyOfPropertyChange("CanStopSending");
            microphone.Stop();
            dt.Stop();
        }

        void microphone_BufferReady(object sender, EventArgs e)
        {
            if (!_Sending) return;
            //System.Diagnostics.Debug.WriteLine(DateTime.Now.Millisecond);

            microphone.GetData(buffer);
            //stream.Write(buffer, 0, buffer.Length);
            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{

            #region Butterworth Lowpass (4 poles)
            /*
             * Filter code adapted from http://www-users.cs.york.ac.uk/~fisher/mkfilter/trad.html
             * Using parameters: 
             * Butterworth Lowpass
             * Filter Order: 4
             * Sample Rate: 16000
             * Corner Frequency 1: 4000
             * Corner Frequency 2: none
             * Additional Zero: none
             * Matched z-transform: no
             * Lower Limit: none
             */
            //float[] xv = new float[5];
            //float[] yv = new float[5];
            //for (int i = 0; i < 5; i++)
            //{
            //    xv[i] = 0;
            //    yv[i] = 0;
            //}
            //int lowpassed;
            //double gain = 1.064046542e+01;
            for (int i = 0; i < buffer.Length; i += 4)
            {
                //xv[0] = xv[1]; xv[1] = xv[2]; xv[2] = xv[3]; xv[3] = xv[4];
                //xv[4] = (float)((((int)buffer[i] << 8) & (int)buffer[i + 1]) / gain);
                //yv[0] = yv[1]; yv[1] = yv[2]; yv[2] = yv[3]; yv[3] = yv[4];
                //yv[4] = (xv[0] + xv[4]) + 4 * (xv[1] + xv[3]) + 6 * xv[2]
                //          + ((float)-0.0176648009 * yv[0]) +
                //          +((float)-0.4860288221 * yv[2]);
                //lowpassed = (int)yv[4];
                //buffer[i + 1] = (byte)(lowpassed >> 8);
                //buffer[i] = (byte)(lowpassed & 0xff);
                if (i % 4 == 0)
                {
                    //int average = ((((int)buffer[i] << 8) & (int)buffer[i + 1]) + (((int)buffer[i - 2] << 8) & (int)buffer[i - 1])) / 2;
                    downSampleBuffer[i / 2] = buffer[i];
                    downSampleBuffer[i / 2 + 1] = buffer[i + 1];
                }
            }
            #endregion
            if (ALAW)
            {
                SendBytes(ALawEncoder.ALawEncode(downSampleBuffer));
            }
            else
            {
                SendBytes(MuLawEncoder.MuLawEncode(downSampleBuffer));
            }
            //});
        }

        public void SendBytes(byte[] buffer)
        {
            if (Port != null && mySocket != null && mySocket.Connected)
            {
                socketEventArg.SetBuffer(buffer, 0, buffer.Length);
                //socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(socketEventArg_Completed);
                mySocket.SendAsync(socketEventArg);
            }
            else
            {
                StopSending();
            }
        }
    }
}
