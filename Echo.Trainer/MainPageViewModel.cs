using Echo.Trainer.ViewModels;
using Caliburn.Micro;

namespace Echo.Trainer {
    public class MainPageViewModel : Conductor<object>.Collection.AllActive
    {
        private ListenViewModel lvm;
        public ListenViewModel Listen { get { return lvm; } }
        private SpeakViewModel svm;
        public SpeakViewModel Speak { get { return svm; } }
        private DownloadPanoramaItemViewModel dvm;
        public DownloadPanoramaItemViewModel DownloadContent { get { return dvm; } }

        public MainPageViewModel(ListenViewModel lvm, SpeakViewModel svm, DownloadPanoramaItemViewModel dvm)
        {
            this.lvm = lvm;
            this.svm = svm;
            this.dvm = dvm;
            Items.Add(svm);
            Items.Add(lvm);
            Items.Add(dvm);
        }
    }
}


