using Microsoft.Phone.Controls;

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
