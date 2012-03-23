using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Echo.Model;
using Microsoft.Phone.Shell;

namespace Echo.ViewModels
{
    public class RecentsViewModel : Screen
    {
        public List<ApplicationBarIconButton> AppBarButtons { get; private set; }
        public UDCListModel UDC { get { return udc; } }

        private INavigationService navService;
        private UDCListModel udc;


        public RecentsViewModel(INavigationService navService, UDCListModel udc)
        {
            this.navService = navService;
            this.udc = udc;
            this.DisplayName = "Recents";
            CreateApplicationBarButtons();
        }

        private void CreateApplicationBarButtons()
        {
            
            AppBarButtons = new List<ApplicationBarIconButton>();

            var clear = new ApplicationBarIconButton();
            clear.IconUri = new Uri("/icons/appbar.delete.rest.png", UriKind.Relative);
            clear.IsEnabled = true;
            clear.Text = "clear";
            clear.Click += new EventHandler(clear_Click);

            AppBarButtons.Add(clear);
        }
        public void GotoCallLogPage(object dataContext)
        {
            var Log = dataContext as CallLogModel;
            navService.UriFor<CallLogPageViewModel>()
                .WithParam(x => x.CallLogID, Log.CallLogID)
                .WithParam(x => x.ComingFromContact, false)
                .Navigate();
        }

        private void clear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete ALL OF YOUR RECENT CALLS?", "", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
            {
                udc.deleteCallLogs();
                NotifyOfPropertyChange("RecentCalls");
            }
            
        }
    }
}
