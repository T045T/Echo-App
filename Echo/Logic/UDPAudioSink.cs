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
        private bool _Sending;
        private bool ALAW;

        private string Server;
        private int Port;

        private DispatcherTimer dt;

        Socket mySocket;
        Microphone microphone;
        byte[] buffer;
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
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
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
            if (e.SocketError == SocketError.Success)
            {
                _Sending = true;

            }
            else
            {
                // Error
            }

        }

        public void StartSending(string AddressOrDns, int Port)
        {
            ConnectSocket(AddressOrDns, Port);
            microphone.BufferDuration = TimeSpan.FromMilliseconds(1000);
            buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];
            microphone.Start();
        }
        public void StopSending()
        {
            _Sending = false;
            NotifyOfPropertyChange("CanStartSending");
            NotifyOfPropertyChange("CanStopSending");
            microphone.Stop();
            dt.Stop();
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
                else
                {
                    SendBytes(MuLawEncoder.MuLawEncode(buffer));
                }
            });
        }

        public void SendBytes(byte[] buffer)
        {
            if (mySocket != null && mySocket.Connected)
            {
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs()
                {
                    RemoteEndPoint = new IPEndPoint(IPAddress.Parse(Server), Port)
                };
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
