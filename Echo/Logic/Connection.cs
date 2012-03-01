using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Caliburn.Micro;
using Echo.Model;
using System.Windows;
using System.Collections.Generic;
using Echo.Helpers;
using Echo.ViewModels;
using System.Linq;
using System.ComponentModel;
using System.Threading;

namespace Echo.Logic
{
    public delegate void DataReceivedEventHandler(object sender, EventArgs e);

    public class Connection : Screen
    {
        private INavigationService navService;
        private SettingsModel setModel;
        private UDCListModel udc;

        private string sessionToken; 
        private Socket socket;
        private IPEndPoint endpoint; //Change to DNSEndpoint
        private DnsEndPoint dnsEndpoint;
        private SocketAsyncEventArgs sendArgs;
        private SocketAsyncEventArgs receiveArgs;

        private IPAddress ip;
        private string dns;
        private int port;

        private BackgroundWorker keepaliveWorker;

        private bool _Ringing;
        public bool Ringing
        {
            get { return _Ringing; }
            set
            {
                if (value != _Ringing)
                {
                    _Ringing = value;
                    NotifyOfPropertyChange("Ringing");
                }
            }
        }

        public event DataReceivedEventHandler DataReceived;

        protected virtual void OnDataReceived(EventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        public Connection(INavigationService navService, SettingsModel setModel, UDCListModel udc)
        {
            this.navService = navService;
            this.setModel = setModel;
            this.udc = udc;

            this.keepaliveWorker = new BackgroundWorker();
            keepaliveWorker.WorkerReportsProgress = false;
            keepaliveWorker.DoWork += new DoWorkEventHandler(keepaliveWorker_DoWork);

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.sendArgs = new SocketAsyncEventArgs();
            this.receiveArgs = new SocketAsyncEventArgs();
            sendArgs.UserToken = new ReceiveInfo(this.socket);
            receiveArgs.UserToken = new ReceiveInfo(this.socket);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Send_Completed);
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);

            Connect();
        }

        void keepaliveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => KeepAlive());
            // keepalives every 20 secs
            Thread.Sleep(20000);
        }

        public void Connect()
        {
            string AddressOrDns = setModel.getValueOrDefault<string>(setModel.EchoServerSettingKeyName, setModel.EchoServerDefault);
            int port = setModel.getValueOrDefault<int>(setModel.EchoPortSettingKeyName, setModel.EchoPortDefault);
            if (this.socket.Connected)
            {
                this.socket.Close();
            }

            ip = null;
            try
            {
                ip = IPAddress.Parse(AddressOrDns);
            }
            catch (Exception e)
            {
                dns = AddressOrDns;
            }
            if (ip != null)
            {
                this.endpoint = new IPEndPoint(ip, port);
                sendArgs.RemoteEndPoint = endpoint;
                receiveArgs.RemoteEndPoint = endpoint;
            }
            else
            {
                this.dnsEndpoint = new DnsEndPoint(dns, port);
                sendArgs.RemoteEndPoint = dnsEndpoint;
                receiveArgs.RemoteEndPoint = dnsEndpoint;
            }

            this.socket.ConnectAsync(sendArgs);
            this.socket.ConnectAsync(receiveArgs);
        }


        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            ReceiveInfo info = sendArgs.UserToken as ReceiveInfo;
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    this.login(e);
                    break;
                case SocketAsyncOperation.Receive:
                    //should not happen
                    break;
                case SocketAsyncOperation.Send:
                    switch (info.LastOperation)
                    {
                        case ClientHeader.RECONNECT:
                            sendUserData(e);
                            break;
                        
                    }
                    info.LastOperation = -1;
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
                    if (e.Buffer.Length == 1 && info.LastOperation != ServerHeader.ERROR) //interpret header
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
                                registerSuccessful();
                                listen(e);
                                break;
                            case ServerHeader.ERROR:
                                info.LastOperation = ServerHeader.ERROR;
                                receiveData(1, e);
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
                            case ServerHeader.ANALYSING:
                                info.LastOperation = ServerHeader.ANALYSING;
                                this.analyzing();
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
                                break;
                            case ServerHeader.TOKEN:
                                tokenReceived(e.Buffer, e);
                                listen(e);
                                break;
                            case ServerHeader.INCOMINGCALL:
                                incomingCall(e.Buffer, e);
                                break;
                            case ServerHeader.ERROR:
                                error(e.Buffer, e);
                                break;                            
                        }
                        info.LastOperation = -1;
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
            string username = setModel.getValueOrDefault<string>(setModel.UsernameSettingKeyName, setModel.UsernameDefault);
            string password = setModel.getValueOrDefault<string>(setModel.PasswordSettingKeyName, setModel.PasswordDefault);
            string sipServer = setModel.getValueOrDefault<string>(setModel.ServerSettingKeyName, setModel.ServerDefault);
            string sipPort = setModel.getValueOrDefault<int>(setModel.PortSettingKeyName, setModel.PortDefault).ToString();

            String userdata = username + ";" + sipServer + ";" + password + ";" + sipPort; //"1467440;sipgate.de;DDAFTG;5060";
            
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
           
            sessionToken = decryptAndVerify(data);
          
        }

        private void incomingCall(byte[] data, SocketAsyncEventArgs e)
        {
            UserModel caller;
            string fromID = decryptAndVerify(data);
            var userMatch = from UserModel u in udc.DataContext.UserTable where u.UserID.Equals(fromID) select u;
            if (userMatch.Any()) {
                caller = userMatch.First();
            } else {
                caller = new UserModel(fromID, "Unknown", "", "");
                udc.addUser(caller);
                udc.SubmitChanges();
            }

            Deployment.Current.Dispatcher.BeginInvoke(() => {
                Coroutine.BeginExecute(IncomingCall(caller.ID));
            });
                //do something
            
        }

        private ShowDialog<IncomingCallDialogViewModel> callDialog;
        private IEnumerator<IResult> IncomingCall(int CalleeID)
        {
            callDialog = new ShowDialog<IncomingCallDialogViewModel>().ConfigureWith(x => x.CallerID = CalleeID);
            yield return callDialog;
            if (callDialog.Dialog.Answered)
            {
                // don't forget to call pickupCall in ActiveCallPageViewModel!!!
                navService.UriFor<ActiveCallPageViewModel>()
                    .WithParam(x => x.calleeID, CalleeID)
                    .WithParam(x => x.isIncoming, true)
                    .Navigate();
            }
            else
            {
                rejectCall();
            }
        }

        private void registerSuccessful()
        {
        }

        private void error(byte[] data, SocketAsyncEventArgs e)
        {
            int error = data[0];
            switch (error)
            {
                case ErrorCodes.CORRUPTEDDATA:
                    break;
                case ErrorCodes.LOGINFAILED:
                    reconnect();
                    break;
                case ErrorCodes.CALLFAILED:
                    break;
                case ErrorCodes.SERVERERROR:
                    break;
            }
        }

        private void remoteHangup()
        {
            Ringing = false;
            if (callDialog != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    callDialog.Dialog.TryClose();
                });
            }
        }

        private void ringing()
        {
            Ringing = true;
        }

        private void calleePickup()
        {
            Ringing = false;
        }

        private void analyzing()
        {
        }

        public void logout()
        {
            byte[] header = new byte[1];
            header[0] = ClientHeader.LOGOUT;
            ReceiveInfo info = sendArgs.UserToken as ReceiveInfo;
            info.LastOperation = ClientHeader.LOGOUT;
            this.sendArgs.SetBuffer(header, 0, header.Length);
            this.socket.SendAsync(this.sendArgs);
        }

        public void KeepAlive()
        {
            byte[] header = new byte[1];
            header[0] = ClientHeader.KEEPALIVE;
            ReceiveInfo info = sendArgs.UserToken as ReceiveInfo;
            info.LastOperation = ClientHeader.KEEPALIVE;
            this.sendArgs.SetBuffer(header, 0, header.Length);
            this.socket.SendAsync(sendArgs);
        }

        public void pickupCall()
        {
            byte[] header = new byte[1];
            header[0] = ClientHeader.PICKUPCALL;
            ReceiveInfo info = sendArgs.UserToken as ReceiveInfo;
            info.LastOperation = ClientHeader.PICKUPCALL;
            this.sendArgs.SetBuffer(header, 0, header.Length);
            this.socket.SendAsync(sendArgs);
        }

        public void rejectCall()
        {
            byte[] header = new byte[1];
            header[0] = ClientHeader.REJECTCALL;
            ReceiveInfo info = sendArgs.UserToken as ReceiveInfo;
            info.LastOperation = ClientHeader.REJECTCALL;
            this.sendArgs.SetBuffer(header, 0, header.Length);
            this.socket.SendAsync(sendArgs);
        }

        public void hangup()
        {
            Ringing = false;
            byte[] header = new byte[1];
            header[0] = ClientHeader.HANGUP;
            ReceiveInfo info = sendArgs.UserToken as ReceiveInfo;
            info.LastOperation = ClientHeader.HANGUP;
            this.sendArgs.SetBuffer(header, 0, header.Length);
            this.socket.SendAsync(sendArgs);
        }

        public void call(String uri) // "sip:userName@dom.ain | Nummer
        {
            byte[] data = Encoding.UTF8.GetBytes(uri);
            byte[] netData = encryptNetData(data);
            ReceiveInfo info = sendArgs.UserToken as ReceiveInfo;
            info.LastOperation = ClientHeader.INITIATECALL;
            sendData(ClientHeader.INITIATECALL, netData, sendArgs);
        }

        public void reconnect()
        {
            byte[] header = new byte[1];
            header[0] = ClientHeader.RECONNECT;
            ReceiveInfo info = sendArgs.UserToken as ReceiveInfo;
            info.LastOperation = ClientHeader.RECONNECT;
            this.sendArgs.SetBuffer(header, 0, header.Length);
            this.socket.SendAsync(sendArgs);
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


