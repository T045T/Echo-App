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
using NetworkTestApp_Moritz.Logic;

namespace NetworkTestApp_Moritz
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Konstruktor
        private Connection testcon;
        public MainPage()
        {
            InitializeComponent();
            Crypt.init();
            testcon = new Connection();
        }

        private void PageTitle_Tap(object sender, GestureEventArgs e)
        {
            this.testcon.logout();
        }
    }
}