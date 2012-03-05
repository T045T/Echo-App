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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Echo.Model;
using System.Linq;
using Caliburn.Micro;

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

        //protected override void OnActivate()
        //{
        //    base.OnActivate();
        //    NotifyOfPropertyChange("RecentCalls");
        //}

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
