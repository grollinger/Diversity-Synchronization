using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;

namespace MVVMDiversity.Interface
{
    public interface IDefinitionsService
    {   
        AsyncOperationInstance loadDefinitions();
        event AsyncOperationFinishedHandler DefinitionsLoaded;

        AsyncOperationInstance loadTaxonLists(IEnumerable<TaxonList> taxa);
        event AsyncOperationFinishedHandler TaxaLoaded;

        AsyncOperationInstance loadProperties();
        event AsyncOperationFinishedHandler PropertiesLoaded;
    }
}
