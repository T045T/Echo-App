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

namespace Echo.Logic
{
    public abstract class ClientHeader
    {
        public const int LOGIN = 5;
	    public const int USERDATA = 10;
	    public const int LOGOUT = 15;
	    public const int KEEPALIVE = 20;
	    public const int PICKUPCALL = 25;
	    public const int REJECTCALL = 30;
	    public const int DTMF = 35;
	    public const int HANGUP = 40;
	    public const int INITIATECALL = 45;
	    public const int RECONNECT = 50;
    }
}
