using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace MVVMDiversity.Interface
{
    public interface IISOViewModelStore
    {
        bool ContainsKey(Guid key);
        IISOViewModel retrieveVM(Guid key);
        IISOViewModel addOrRetrieveVMForISO(ISerializableObject iso);
    }
}
