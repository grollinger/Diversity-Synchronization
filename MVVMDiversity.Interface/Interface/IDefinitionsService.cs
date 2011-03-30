using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MVVMDiversity.Model;

namespace MVVMDiversity.Interface
{
    public interface IDefinitionsService
    {   
        BackgroundOperation loadDefinitions(Action finishedCallback);
        BackgroundOperation loadTaxonLists(IEnumerable<TaxonList> taxa, Action finishedCallback);
        BackgroundOperation loadProperties(Action finishedCallback);
    }
}
