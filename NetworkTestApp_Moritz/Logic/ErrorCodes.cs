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

namespace NetworkTestApp_Moritz.Logic
{
    public abstract class ErrorCodes
    {
        public const int CORRUPTEDDATA = 5;
	    public const int LOGINFAILED = 10;
	    public const int CALLFAILED = 15;
	    public const int SERVERERROR = 20;
    }
}
