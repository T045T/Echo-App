using System.Windows;

namespace Echo
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Other code…
            }
        }
    }
}