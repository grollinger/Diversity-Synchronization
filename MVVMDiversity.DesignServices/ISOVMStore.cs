using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;

namespace MVVMDiversity.DesignServices
{
    public class ISOVMStore : IISOViewModelStore
    {
        public bool ContainsKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IISOViewModel retrieveVM(Guid key)
        {
            throw new NotImplementedException();
        }

        public IISOViewModel addOrRetrieveVMForISO(UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject iso)
        {
            throw new NotImplementedException();
        }
    }
}
