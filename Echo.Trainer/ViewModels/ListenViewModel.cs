using Caliburn.Micro;

namespace Echo.Trainer.ViewModels
{
    public class ListenViewModel
    {
        private INavigationService navService;

        private int sentenceNo;
        public int SentenceNo { get { return sentenceNo; } }
        private int wordNo;
        public int WordNo { get { return wordNo; } }
        private int phonemeNo;
        public int PhonemeNo { get { return phonemeNo; } }

        public ListenViewModel(INavigationService navService)
        {
            this.navService = navService;
        }

        /// <summary>
        /// Navigates to the content selection page for the pronunciation trainer
        /// </summary>
        /// <param name="whereTo">selects which pivot item to navigate to - 0: Sentences, 1: Words, 2: Phonemes</param>
        public void ContentSelection(int whereTo)
        {
            if (whereTo >= 0 && whereTo < 3)
                navService.UriFor<ListeningContentSelectionPageViewModel>().WithParam(x => x.StartItem, whereTo);
        }

        public void ContinuePrevious()
        {

        }
    }
}
