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

namespace NetworkTestApp.Logic
{
    public abstract class ServerHeader
    {
        public const int KEY = 5;
	    public const int TOKEN = 10;
	    public const int INCOMINGCALL = 15;
	    public const int REGISTERSUCCESS = 20;
	    public const int ERROR = 25;
	    public const int REMOTEHANGUP = 30;
	    public const int RINGING = 35;
	    public const int CALLEEPICKUP = 40;
    }
}
