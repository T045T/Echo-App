using Microsoft.Phone.Controls;
using Microsoft.Silverlight.Testing;

namespace Echo.test
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Konstruktor
        public MainPage()
        {
            InitializeComponent();
            Content = UnitTestSystem.CreateTestPage();
        }
    }
}