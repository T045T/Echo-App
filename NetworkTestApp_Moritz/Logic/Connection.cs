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
using NetworkTestApp_Moritz.Logic;
using System.Text;

namespace NetworkTestApp_Moritz.Logic
{
    public class Connection
    {
        private Socket socket;
        private IPEndPoint endpoint; //Change to DNSEndpoint
        private SocketAsyncEventArgs sendArgs;
        private SocketAsyncEventArgs receiveArgs;

        public Connection()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("192.168.178.25");
            this.endpoint = new IPEndPoint(ip, 9042);
            this.sendArgs = new SocketAsyncEventArgs();
            this.receiveArgs = new SocketAsyncEventArgs();
            sendArgs.UserToken = new ReceiveInfo(this.socket);
            receiveArgs.UserToken = new ReceiveInfo(this.socket);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Send_Completed);
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
            sendArgs.RemoteEndPoint = this.endpoint;
            receiveArgs.RemoteEndPoint = this.endpoint;
            this.socket.ConnectAsync(sendArgs);
            this.socket.ConnectAsync(receiveArgs);
        }

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    this.login(e);
                    break;
                case SocketAsyncOperation.Receive:
                    //should not happen
                    break;
                case SocketAsyncOperation.Send:
                    //data sent
                    break;
                //default:
                    //throw new Exception("Invalid operation completed");
            } 
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            ReceiveInfo info = e.UserToken as ReceiveInfo;
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    this.listen(e);
                    break;
                case SocketAsyncOperation.Receive:
                    if (e.Buffer.Length == 1) //interpret header
                    {
                        int header = e.Buffer[0];
                        switch (header)
                        {
                            case ServerHeader.KEY:
                                info.LastOperation = ServerHeader.KEY;
                                receiveData(297, e);
                                break;
                            case ServerHeader.TOKEN:
                                info.LastOperation = ServerHeader.TOKEN;
                                receiveData(344, e);
                                break;
                            case ServerHeader.INCOMINGCALL:
                                info.LastOperation = ServerHeader.INCOMINGCALL;
                                receiveData(344, e);
                                break;
                            case ServerHeader.REGISTERSUCCESS:
                                info.LastOperation = ServerHeader.REGISTERSUCCESS;
                                //melden
                                listen(e);
                                break;
                            case ServerHeader.ERROR:
                                info.LastOperation = ServerHeader.ERROR;
                                
                                break;
                            case ServerHeader.REMOTEHANGUP:
                                info.LastOperation = ServerHeader.REMOTEHANGUP;
                                this.remoteHangup();
                                break;
                            case ServerHeader.RINGING:
                                info.LastOperation = ServerHeader.RINGING;
                                this.ringing();
                                break;
                            case ServerHeader.CALLEEPICKUP:
                                info.LastOperation = ServerHeader.CALLEEPICKUP;
                                this.calleePickup();
                                break;
                            default:
                                info.LastOperation = -1;
                                this.listen(e);
                                break;
                        }
                        break;
                    }
                    else //interpret data
                    {             
                        switch (info.LastOperation)
                        {
                            case ServerHeader.KEY:
                                keyReceived(e.Buffer, e);
                                info.LastOperation = -1;
                                break;
                            case ServerHeader.TOKEN:
                                tokenReceived(e.Buffer, e);
                                info.LastOperation = -1;
                                listen(e);
                                break;
                            case ServerHeader.INCOMINGCALL:
                                incomingCall(e.Buffer, e);
                                info.LastOperation = -1;
                                break;
                            case ServerHeader.REGISTERSUCCESS:

                                info.LastOperation = -1;
                                break;
                            case ServerHeader.ERROR:

                                info.LastOperation = -1;
                                break;                            
                        }
                    }
                    break;
                case SocketAsyncOperation.Send:
                    this.listen(e);
                    break;
                //default:
                    //throw new Exception("Invalid operation completed");
            } 
        }

        private void listen(SocketAsyncEventArgs e)
        {
            byte[] header = new byte[1];
            e.SetBuffer(header, 0, header.Length);
            ReceiveInfo info = e.UserToken as ReceiveInfo;
            info.Sock.ReceiveAsync(e);
        }

        private void login(SocketAsyncEventArgs e)
        {
            byte header = ClientHeader.LOGIN;
            String xmlKey = Crypt.getPublicKeyInXML();
            byte[] data = Encoding.UTF8.GetBytes(xmlKey);

            sendData(header, data, e);
        }

        private static void sendData(byte header, byte[] data, SocketAsyncEventArgs e)
        {
            byte[] buffer = new byte[data.Length + 1];
            buffer[0] = header;
            
            for (int i = 1; i <= data.Length; i++)
            {
                buffer[i] = data[i - 1];
            }

            ReceiveInfo info = e.UserToken as ReceiveInfo;
            e.SetBuffer(buffer, 0, buffer.Length);
            info.Sock.SendAsync(e);
        }

        private static void receiveData(int length, SocketAsyncEventArgs e)
        {
            ReceiveInfo info = e.UserToken as ReceiveInfo;
            byte[] buffer = new byte[length];
            e.SetBuffer(buffer, 0, buffer.Length);
            info.Sock.ReceiveAsync(e);
        }

        private void keyReceived(byte[] data, SocketAsyncEventArgs e) 
        {
            String xmlKey = Encoding.UTF8.GetString(data, 0, data.Length);
            Crypt.setServerPublicKey(xmlKey);
            sendUserData(e);
        }

        private void sendUserData(SocketAsyncEventArgs e)
        {
            String userdata = "1467440;sipgate.de;DDAFTG;5060";
            byte[] data = Encoding.UTF8.GetBytes(userdata);
            byte[] netData = encryptNetData(data);
            sendData(ClientHeader.USERDATA, netData, e);
        }

        private static byte[] encryptNetData(byte[] data)
        {
            byte[] signature = Crypt.sign(data);
            data = Crypt.encrypt(data);
            data = Encoding.UTF8.GetBytes(System.Convert.ToBase64String(data));
            signature = Encoding.UTF8.GetBytes(System.Convert.ToBase64String(signature));
            return mergeArrays(data, signature);
        }

        private static byte[] mergeArrays(byte[] first, byte[] second)
        {
            byte[] array = new byte[first.Length + second.Length];
            System.Array.Copy(first, array, first.Length);
            System.Array.Copy(second, 0, array, first.Length, second.Length);
            return array;
        }

        private static String decryptAndVerify(byte[] data)
        {
            String message = String.Empty;

            UTF8Encoding utf8 = new UTF8Encoding();

            String datatext = utf8.GetString(data, 0, data.Length / 2);
            String sigtext = utf8.GetString(data, data.Length / 2, data.Length / 2);

            if (!(String.IsNullOrWhiteSpace(datatext) || String.IsNullOrWhiteSpace(sigtext)))
            {
                byte[] tmpdata = System.Convert.FromBase64String(datatext);
                byte[] signature = System.Convert.FromBase64String(sigtext);


                tmpdata = Crypt.decrypt(tmpdata);
                if (Crypt.verify(tmpdata, signature))
                {
                    message = utf8.GetString(tmpdata, 0, tmpdata.Length);

                }
                else
                {
                    //corrupted Data
                }
            }
            else
            {
                //String null
            }
            return message;
        }

        private void tokenReceived(byte[] data, SocketAsyncEventArgs e)
        {
           
            String sessionToken = decryptAndVerify(data);
          
        }

        private void incomingCall(byte[] data, SocketAsyncEventArgs e)
        {
            
            String from = decryptAndVerify(data);
                //do something
            
        }

        private void registerSuccessful(byte[] data, SocketAsyncEventArgs e)
        {
        }

        private void error(byte[] data, SocketAsyncEventArgs e)
        {
        }

        private void remoteHangup()
        {
        }

        private void ringing()
        {
        }

        private void calleePickup()
        {
        }

        public void logout()
        {
            byte[] header = new byte[1];
            header[0] = ClientHeader.LOGOUT;
            this.sendArgs.SetBuffer(header, 0, header.Length);
            this.socket.SendAsync(this.sendArgs);
        }

        public void KeepAlive()
        {
        }

        public void pickupCall()
        {
        }

        public void recectCall()
        {
        }

        public void hangup()
        {
        }

        public void call(String uri)
        {
            byte[] data = Encoding.UTF8.GetBytes(uri);
            byte[] netData = encryptNetData(data);
            sendData(ClientHeader.INITIATECALL, netData, sendArgs);
        }

        public void reconnect()
        {
        }
    }

    public class ReceiveInfo
    {
        private Socket sock;
        private int lastOperation;
        public ReceiveInfo(Socket sock)
        {
            this.sock = sock;
            this.lastOperation = -1;
        }
        public Socket Sock
        {
            get { return this.sock; }
        }
        public int LastOperation
        {
            get { return this.lastOperation; }
            set { this.lastOperation = value; }
        }
    }
}


