using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MVVMDiversity.Model;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.Interface
{
    public interface IFieldDataService 
    {
        IEnumerable<SearchSpecification> SearchTypes { get; }

        BackgroundOperation uploadData(string userNr, int projectID, Action finishedCallback);

        BackgroundOperation downloadData(IList<ISerializableObject> selection, Action finishedCallback);

        BackgroundOperation executeSearch(SearchSpecification search, int currentProjectID, Action<IList<ISerializableObject>> finishedCallback);        
    }
}
