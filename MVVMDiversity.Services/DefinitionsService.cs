using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using MVVMDiversity.Model;

namespace MVVMDiversity.Services
{
    public partial class DefinitionsService : IDefinitionsService
    {
        [Dependency]
        public IConnectionProvider Connections { get; set; }

        [Dependency]
        public IUserOptionsService Settings { get; set; }

        [Dependency]
        public IUserProfileService Profiles { get; set; }  

        DefinitionLoader _defLoader;
        PropertyLoader _propLoader;
        TaxonLoader _taxLoader;

        public DefinitionsService()
        {
            _defLoader = new DefinitionLoader(this);
            _propLoader = new PropertyLoader(this);
            _taxLoader = new TaxonLoader(this);
        }   


        public BackgroundOperation loadDefinitions(Action finishedCallback)
        {
            var progress = BackgroundOperation.newUninterruptable();
            new Action<BackgroundOperation>(_defLoader.loadCollectionDefinitions).BeginInvoke(progress,(res) =>
                {
                    if( finishedCallback != null)
                        finishedCallback();
                }, null);
            return progress;
        }

        public BackgroundOperation loadTaxonLists(IEnumerable<Model.TaxonList> taxa, Action finishedCallback)
        {
            var progress = BackgroundOperation.newUninterruptable();
            new Action(()=>
            {
                _taxLoader.startTaxonDownload(taxa,progress);
            }).BeginInvoke(
            (res) => 
            {
                if (finishedCallback != null)
                    finishedCallback();
            },null);

            return progress;
        }

        public BackgroundOperation loadProperties(Action finishedCallback)
        {
            var progress = BackgroundOperation.newUninterruptable();
            new Action<BackgroundOperation>(_propLoader.updateProperties).BeginInvoke(progress, (res) =>
            {
                if (finishedCallback != null)
                    finishedCallback();
            }, null);
            return progress;
        }
    }
}
