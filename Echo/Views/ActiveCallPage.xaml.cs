using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Echo.Helpers;
using Caliburn.Micro;

namespace Echo.Views
{
    public partial class ActiveCallPage : PhoneApplicationPage
    {
        public ActiveCallPage()
        {
            InitializeComponent();
        }

        public void ScrollToBottom()
        {
            //Mediator.ScrollableHeightMultiplier = 1;
            CallLogScrollViewer.UpdateLayout();
            CallLogScrollViewer.ScrollToVerticalOffset(CallLogScrollViewer.ScrollableHeight);
        }
    }
}
