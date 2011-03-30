using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using MVVMDiversity.ViewModel;

namespace MVVMDiversity.Interface
{
    public interface IISOViewModel
    {
        Guid Rowguid { get; }
        string Name { get; }
        ISOIcon Icon { get; }

        ISerializableObject Parent { get; }
        ISerializableObject ISO { get; }
        IEnumerable<ISerializableObject> Properties { get; }
        IEnumerable<ISerializableObject> Children { get; }
        
    }
}
