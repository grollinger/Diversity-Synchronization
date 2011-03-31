using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using System.Windows.Threading;

namespace MVVMDiversity.DesignServices
{
    public class Definitions : IDefinitionsService
    {
        public Model.BackgroundOperation loadDefinitions(Action finishedCallback)
        {
            return simulateWork(finishedCallback);
        }

        public Model.BackgroundOperation loadTaxonLists(IEnumerable<Model.TaxonList> taxa, Action finishedCallback)
        {
            return simulateWork(finishedCallback);
        }

        public Model.BackgroundOperation loadProperties(Action finishedCallback)
        {
            return simulateWork(finishedCallback);
        }

        private static Model.BackgroundOperation simulateWork(Action finishedCallback)
        {
            var p = Model.BackgroundOperation.newInterruptable();
                p.IsProgressIndeterminate = false;
                p.ProgressDescriptionID = "MainWindow_Title";
                p.ProgressOutput = "Output1234";
                
            p.IsProgressIndeterminate = false;

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (sender, args) =>
            {
                p.Progress += 20;
                if (p.Progress >= 100 || p.IsCancelRequested)
                {
                    (sender as DispatcherTimer).Stop();
                    finishedCallback();
                }
            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            return p;
        }       
    }
}
