using Caliburn.Micro;
namespace Echo {
    public class OldMainPageViewModel : Conductor<object>.Collection.OneActive {
        readonly INavigationService navService;

        public OldMainPageViewModel(INavigationService navService)
        {
            this.navService = navService;
        }
    }
}
