using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using Echo.Helpers;
using Echo.Model;
using Echo.ViewModels;

namespace Echo.Logic
{
    public class EchoClientLogic
    {
        private BackgroundWorker myWorker;
        private UDCListModel udc;
        private INavigationService navService;
        private IWindowManager winMan;
        private int CalleeID;

        public EchoClientLogic(INavigationService navService, IWindowManager winMan, UDCListModel udc)
        {
            myWorker = new BackgroundWorker();
            this.navService = navService;
            this.winMan = winMan;
            this.udc = udc;
            this.CalleeID = 0;
            myWorker.WorkerReportsProgress = true;
            myWorker.ProgressChanged += new ProgressChangedEventHandler(myWorker_ProgressChanged);
            myWorker.DoWork += new DoWorkEventHandler(myWorker_DoWork);
        }

        void myWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var backgroundCallerID = e.UserState as int?;
            if (backgroundCallerID != null)
            {
                CalleeID = (int)backgroundCallerID;
                Coroutine.BeginExecute(IncomingCall());
            }
        }

        public void startWork()
        {
            myWorker.RunWorkerAsync();
        }

        void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(8000);
            var foo = from UserModel u in udc.DataContext.UserTable select u;
            int tmpID = 0;
            if (foo.Any())
                tmpID = foo.First().ID;
            myWorker.ReportProgress(0, tmpID);
            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    MessageBox.Show("Pling!");
            //});
            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    IncomingCall();
            //});
        }
        private IEnumerator<IResult> IncomingCall()
        {
            var callDialog = new ShowDialog<IncomingCallDialogViewModel>().ConfigureWith(x => x.CallerID = CalleeID);
            yield return callDialog;
            if (callDialog.Dialog.Answered)
                navService.UriFor<ActiveCallPageViewModel>()
                    .WithParam(x => x.calleeID, CalleeID)
                    .WithParam(x => x.isIncoming, true)
                    .Navigate();
        }
    }
}




