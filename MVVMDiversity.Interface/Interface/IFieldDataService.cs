using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.Interface
{
    public interface IFieldDataService 
    {
        IEnumerable<SearchSpecification> SearchTypes { get; }

        AsyncOperationInstance uploadData(string userNr, int projectID);
        event AsyncOperationFinishedHandler UploadFinished;

        AsyncOperationInstance downloadData(IList<ISerializableObject> selection);
        event AsyncOperationFinishedHandler DownloadFinished;

        AsyncOperationInstance<IList<ISerializableObject>> startSearch(SearchSpecification search, int currentProjectID);
        event AsyncOperationFinishedHandler<IList<ISerializableObject>> SearchFinished;
    }
}
