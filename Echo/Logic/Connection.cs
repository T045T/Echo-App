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
using System.Windows.Threading;

namespace Echo.Logic
{
    public delegate void DataReceivedEventHandler(object sender, string e);
    public delegate void AcquiredPortEventHandler(object sender, int e);
    public delegate void CallStartedHandler(object sender, EventArgs e);

    public class Connection : Screen
    {
        private INavigationService navService;
        private SettingsModel setModel;
        private UDCListModel udc;

        private string sessionToken;
        private Socket socket;
        public Socket Socket { get { return socket; } }
        private IPEndPoint endpoint; //Change to DNSEndpoint
        private DnsEndPoint dnsEndpoint;
        private SocketAsyncEventArgs sendArgs;
        private SocketAsyncEventArgs receiveArgs;

        private IPAddress ip;
        private string dns;
        private int voicePort;

        private int LastSendOperation;
        private int LastReceiveOperation;

        private DispatcherTimer keepaliveWorker;

        public bool Connected;
        //{
        //    get { return this.Socket.Connected; }
        //}

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

        private bool _Analyzing;
        public bool Analyzing
        {
            get { return _Analyzing; }
            set
            {
                if (value != _Analyzing)
                {
                    _Analyzing = value;
                    NotifyOfPropertyChange("Analyzing");
                }
            }
        }
        public event DataReceivedEventHandler DataReceived;
        public event AcquiredPortEventHandler AcquiredPort;
        public event CallStartedHandler CallStarted;

        protected virtual void OnDataReceived(string s)
        {
            if (DataReceived != null)
            {
                DataReceived(this, s);
            }
        }

        protected virtual void OnAcquiredPort(int i)
        {
            if (AcquiredPort != null)
            {
                AcquiredPort(this, i);
            }
        }

        protected virtual void OnCallStarted()
        {
            if (CallStarted != null)
            {
                CallStarted(this, new EventArgs());
            }
        }

        public Connection(INavigationService navService, SettingsModel setModel, UDCListModel udc, IPhoneService phoneService)
        {
            phoneService.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(phoneService_Deactivated);
            phoneService.Activated += new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(phoneService_Activated);
            Crypt.init();
            this.LastSendOperation = -1;
            this.LastReceiveOperation = -1;
            this.navService = navService;
            this.setModel = setModel;
            this.udc = udc;

            this.keepaliveWorker = new DispatcherTimer();
            keepaliveWorker.Interval = TimeSpan.FromSeconds(20);
            keepaliveWorker.Tick += new EventHandler(keepaliveWorker_Tick);

            Analyzing = false;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //this.sendArgs = new SocketAsyncEventArgs();
            //this.receiveArgs = new SocketAsyncEventArgs();
            //sendArgs.UserToken = true; // UserToken zeigt an, ob das Objekt gerade verfügbar ist.
            //receiveArgs.UserToken = true;
            //sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Send_Completed);
            //receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);

            Connect();
        }

        void phoneService_Activated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            Connect();
        }

        void phoneService_Deactivated(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            logout();
        }

        void keepaliveWorker_Tick(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => KeepAlive());
        }

        public void Connect()
        {
            LastSendOperation = -1;
            LastReceiveOperation = -1;

            SocketAsyncEventArgs initialConnectionArgs = new SocketAsyncEventArgs();

            this.sendArgs = new SocketAsyncEventArgs();
            this.receiveArgs = new SocketAsyncEventArgs();
            sendArgs.UserToken = true; // UserToken zeigt an, ob das Objekt gerade verfügbar ist.
            receiveArgs.UserToken = true;
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Send_Completed);
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);

            string AddressOrDns = setModel.getValueOrDefault<string>(setModel.EchoServerSettingKeyName, setModel.EchoServerDefault);
            int port = setModel.getValueOrDefault<int>(setModel.EchoPortSettingKeyName, setModel.EchoPortDefault);
            if (this.socket.Connected)
            {
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.NoDelay = true;
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
                initialConnectionArgs.RemoteEndPoint = endpoint;
            }
            else
            {
                this.dnsEndpoint = new DnsEndPoint(dns, port);
                sendArgs.RemoteEndPoint = dnsEndpoint;
                receiveArgs.RemoteEndPoint = dnsEndpoint;
                initialConnectionArgs.RemoteEndPoint = dnsEndpoint;
            }

            initialConnectionArgs.Completed += new EventHandler<SocketAsyncEventArgs>(initialConnection_Completed);

            this.socket.ConnectAsync(initialConnectionArgs);
            //this.socket.ConnectAsync(receiveArgs);
        }

        void initialConnection_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                e.Completed -= new EventHandler<SocketAsyncEventArgs>(initialConnection_Completed);
                this.login(sendArgs);
                listen(receiveArgs);
            }
        }

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Connect:
                        this.login(e);
                        listen(e);
                        break;
                    case SocketAsyncOperation.Receive:
                        //should not happen
                        break;
                    case SocketAsyncOperation.Send:
                        switch (LastSendOperation)
                        {
                            case ClientHeader.RECONNECT:
                                sendUserData(e);
                                break;
                            case ClientHeader.LOGOUT:
                                Connected = false;
                                Socket.Shutdown(SocketShutdown.Both);
                                Socket.Close();
                                Ringing = false;
                                Analyzing = false;
                                break;
                        }
                        LastSendOperation = -1;
                        break;
                    //default:
                    //throw new Exception("Invalid operation completed");
                }
            }
            else
            {
                Connected = false;
                if (Socket.Connected)
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }
            }
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                switch (e.LastOperation)
                {
                    //case SocketAsyncOperation.Connect:
                    //    this.listen(e);
                    //    break;
                    case SocketAsyncOperation.Receive:
                        if (e.Buffer.Length == 1 && LastReceiveOperation != ServerHeader.ERROR) //interpret header
                        {
                            int header = e.Buffer[0];
                            switch (header)
                            {

                                case ServerHeader.KEY:
                                    LastReceiveOperation = ServerHeader.KEY;
                                    receiveData(297, e);
                                    break;
                                case ServerHeader.TOKEN:
                                    LastReceiveOperation = ServerHeader.TOKEN;
                                    receiveData(344, e);
                                    break;
                                case ServerHeader.INCOMINGCALL:
                                    LastReceiveOperation = ServerHeader.INCOMINGCALL;
                                    receiveData(344, e);
                                    break;
                                case ServerHeader.REGISTERSUCCESS:
                                    LastReceiveOperation = ServerHeader.REGISTERSUCCESS;
                                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                                    {
                                        registerSuccessful();
                                    });
                                    listen(e);
                                    break;
                                case ServerHeader.ERROR:
                                    LastReceiveOperation = ServerHeader.ERROR;
                                    listen(e);
                                    break;
                                case ServerHeader.REMOTEHANGUP:
                                    LastReceiveOperation = ServerHeader.REMOTEHANGUP;
                                    remoteHangup();
                                    break;
                                case ServerHeader.RINGING:
                                    LastReceiveOperation = ServerHeader.RINGING;
                                    this.ringing();
                                    break;
                                case ServerHeader.CALLEEPICKUP:
                                    LastReceiveOperation = ServerHeader.CALLEEPICKUP;
                                    calleePickup();
                                    break;
                                case ServerHeader.ANALYSING:
                                    LastReceiveOperation = ServerHeader.ANALYSING;
                                    analyzing();
                                    listen(e);
                                    break;
                                case ServerHeader.VOICEPORT:
                                    LastReceiveOperation = ServerHeader.VOICEPORT;
                                    receiveData(4, e);
                                    break;
                                case ServerHeader.TEXT:
                                    LastReceiveOperation = ServerHeader.TEXT;
                                    receiveData(4, e);
                                    break;
                                default:
                                    LastReceiveOperation = -1;
                                    listen(e);
                                    break;
                            }
                        }
                        else //interpret data
                        {
                            switch (LastReceiveOperation)
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
                                    listen(e);
                                    break;
                                case ServerHeader.ERROR:
                                    error(e.Buffer, e);
                                    listen(e);
                                    break;
                                case ServerHeader.VOICEPORT:
                                    voicePortReceived(e.Buffer);
                                    listen(e);
                                    break;
                                case ServerHeader.TEXT:
                                    textLengthReceived(e.Buffer, e);
                                    LastReceiveOperation = ServerHeader.MORETEXT;
                                    break;
                                case ServerHeader.MORETEXT:
                                    moreTextReceived(e.Buffer);
                                    listen(e);
                                    LastReceiveOperation = -1;
                                    break;
                            }
                            if (LastReceiveOperation != ServerHeader.MORETEXT)
                            {
                                LastReceiveOperation = -1;
                            }
                        }
                        break;
                    case SocketAsyncOperation.Send:
                        this.listen(e);
                        break;
                    default:
                        //throw new Exception("Invalid operation completed");
                        break;
                }
            }
            else
            {
                Connected = false;
                if (Socket.Connected)
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }
            }
        }

        private void login(SocketAsyncEventArgs e)
        {
            byte header = ClientHeader.LOGIN;
            String xmlKey = Crypt.getPublicKeyInXML();
            byte[] data = Encoding.UTF8.GetBytes(xmlKey);

            Connected = true;
            sendData(header, data, e);
        }

        private void sendData(byte header, byte[] data, SocketAsyncEventArgs e)
        {
            if (this.Connected)
            {
                byte[] buffer = new byte[data.Length + 1];
                buffer[0] = header;

                for (int i = 1; i <= data.Length; i++)
                {
                    buffer[i] = data[i - 1];
                }
                e.SetBuffer(buffer, 0, buffer.Length);
                if (!this.socket.SendAsync(e))
                {
                    Send_Completed(this, e);
                }
            }
        }

        private void sendHeader(int headerValue)
        {
            if (this.Connected)
            {
                byte[] header = new byte[1];
                header[0] = (byte)headerValue;
                this.sendArgs.SetBuffer(header, 0, header.Length);
                
                if (!this.socket.SendAsync(this.sendArgs))
                {
                    Send_Completed(this, sendArgs);
                }
            }
        }

        private void listen(SocketAsyncEventArgs e)
        {
            receiveData(1, e);
        }

        private void receiveData(int length, SocketAsyncEventArgs e)
        {
            if (Connected)
            {
                byte[] buffer = new byte[length];
                e.SetBuffer(buffer, 0, buffer.Length);
                if (!this.socket.ReceiveAsync(e))
                {
                    Receive_Completed(this, e);
                }
            }
        }

        private void keyReceived(byte[] data, SocketAsyncEventArgs e)
        {
            String xmlKey = Encoding.UTF8.GetString(data, 0, data.Length);
            Crypt.setServerPublicKey(xmlKey);
            sendUserData(e);
        }

        private void voicePortReceived(byte[] data)
        {
            String port = Encoding.UTF8.GetString(data, 0, data.Length);
            if (!int.TryParse(port, out voicePort))
            {
                voicePort = -1;
            }
            else
            {
                OnAcquiredPort(voicePort);
            }
        }

        private void textLengthReceived(byte[] data, SocketAsyncEventArgs e)
        {
            String textLength = Encoding.UTF8.GetString(data, 0, data.Length);
            int length = int.Parse(textLength);
            receiveData(length, e);
        }

        private void moreTextReceived(byte[] data)
        {
            Analyzing = false;
            String text = Encoding.UTF8.GetString(data, 0, data.Length);
            OnDataReceived(text);
            //do something
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
            if (userMatch.Any())
            {
                caller = userMatch.First();
            }
            else
            {
                caller = new UserModel(fromID, "Unknown", "Unknown", "");
                udc.addUser(caller);
                udc.SubmitChanges();
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
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
            keepaliveWorker.Start();
        }

        private void error(byte[] data, SocketAsyncEventArgs e)
        {
            int error = data[0];
            switch (error)
            {
                case ErrorCodes.CORRUPTEDDATA:
                    break;
                case ErrorCodes.LOGINFAILED:
                    //reconnect();
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
            OnCallStarted();
        }

        private void analyzing()
        {
            Analyzing = true;
        }

        public void logout()
        {
            keepaliveWorker.Stop();
            LastSendOperation = ClientHeader.LOGOUT;
            this.sendHeader(ClientHeader.LOGOUT);
        }

        public void KeepAlive()
        {
            LastSendOperation = ClientHeader.KEEPALIVE;
            this.sendHeader(ClientHeader.KEEPALIVE);
        }

        public void pickupCall()
        {
            LastSendOperation = ClientHeader.PICKUPCALL;
            this.sendHeader(ClientHeader.PICKUPCALL);
        }

        public void rejectCall()
        {
            LastSendOperation = ClientHeader.REJECTCALL;
            this.sendHeader(ClientHeader.REJECTCALL);
        }

        public void hangup()
        {
            Ringing = false;
            LastSendOperation = ClientHeader.HANGUP;
            this.sendHeader(ClientHeader.HANGUP);
        }

        public void call(String uri) // "sip:userName@dom.ain | Nummer
        {
            String datastring = uri + ";" + sessionToken;
            byte[] data = Encoding.UTF8.GetBytes(datastring);
            byte[] netData = encryptNetData(data);
            LastSendOperation = ClientHeader.INITIATECALL;
            sendData(ClientHeader.INITIATECALL, netData, sendArgs);
        }

        public void reconnect()
        {
            LastSendOperation = ClientHeader.RECONNECT;
            this.sendHeader(ClientHeader.RECONNECT);
        }
    }
}


