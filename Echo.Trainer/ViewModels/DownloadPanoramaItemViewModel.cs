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
using Caliburn.Micro;

namespace Echo.Trainer.ViewModels
{
    public class DownloadPanoramaItemViewModel
    {
        private INavigationService navService;

        public DownloadPanoramaItemViewModel(INavigationService navService)
        {
            this.navService = navService;
        }

        public void NavigateToDetailPage(string detailPagePath)
        {
            MessageBox.Show(String.Format("Sorry, can't navigate to {0} yet", detailPagePath));
        }

        public void LoadMoreContent()
        {
            MessageBox.Show("Sorry, no more content available at this time");
        }
    }
}
