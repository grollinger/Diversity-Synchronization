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
        

        private static Model.AsyncOperationInstance simulateWork(Model.AsyncOperationFinishedHandler finishedCallback)
        {
            var p = new Model.AsyncOperationInstance(true,finishedCallback);
                p.IsProgressIndeterminate = false;
                p.StatusDescription = "MainWindow_Title";
                p.StatusOutput = "Output1234";
                
            p.IsProgressIndeterminate = false;

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (sender, args) =>
            {
                p.Progress += 20;
                if (p.Progress >= 100 || p.IsCancelRequested)
                {
                    (sender as DispatcherTimer).Stop();
                    p.success();
                }
            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            return p;
        }

        public Model.AsyncOperationInstance loadDefinitions()
        {
            return simulateWork(DefinitionsLoaded);
        }

        public event Model.AsyncOperationFinishedHandler DefinitionsLoaded;

        public Model.AsyncOperationInstance loadTaxonLists(IEnumerable<Model.TaxonList> taxa)
        {
            return simulateWork(TaxaLoaded);
        }

        public event Model.AsyncOperationFinishedHandler TaxaLoaded;

        public Model.AsyncOperationInstance loadProperties()
        {
            return simulateWork(PropertiesLoaded);
        }

        public event Model.AsyncOperationFinishedHandler PropertiesLoaded;
    }
}
