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

namespace TestApp.echo
{
    public class Connection
    {
        private Socket socket;
        IPEndPoint endpoint; //Change to DNSEndpoint
        SocketAsyncEventArgs sendArgs;
        SocketAsyncEventArgs receiveArgs;

        public Connection()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("192.168.178.25");
            this.endpoint = new IPEndPoint(ip, 9042);
            sendArgs.UserToken = this;
            receiveArgs.UserToken = this;
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Send_Completed);
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
            sendArgs.RemoteEndPoint = this.endpoint;
            receiveArgs.RemoteEndPoint = this.endpoint;
            this.socket.ConnectAsync(sendArgs);
            this.socket.ConnectAsync(receiveArgs);
        }

        public Socket Socket
        {
            get { return this.Socket; }
        }

        public SocketAsyncEventArgs SendArgs
        {
            get { return this.sendArgs; }
            set { this.sendArgs = value; }
        }

        public SocketAsyncEventArgs ReceiveArgs
        {
            get { return this.receiveArgs; }
            set { this.receiveArgs = value; }
        }

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            
        }
    }
}
