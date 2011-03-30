using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using System.ComponentModel;
using MVVMDiversity.Interface;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.ViewModel
{
    public class ISOViewModelContainer : Dictionary<Guid,IISOViewModel>, IISOViewModelStore
    {
        bool IISOViewModelStore.ContainsKey(Guid key)
        {            
            return this.ContainsKey(key);
        }

        public IISOViewModel retrieveVM(Guid key)
        {            
            IISOViewModel res = null;
            TryGetValue(key, out res);
            return res;
        }

        public IISOViewModel addOrRetrieveVMForISO(ISerializableObject iso)
        {
            IISOViewModel res = null;
            if (!TryGetValue(iso.Rowguid, out res))
            {
                res = ISOViewModel.fromISO(iso);
                lock(this)
                    Add(iso.Rowguid, res);
            }
            return res;
        }

        
    }
}
