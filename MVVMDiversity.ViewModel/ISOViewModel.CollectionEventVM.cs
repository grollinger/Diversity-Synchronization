using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class CollectionEventVM : ISOViewModel
        {
            CollectionEvent _obj;
            public CollectionEventVM(CollectionEvent spec)
            {
                _obj = spec;
            }

            internal override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject Parent
            {
                get { return _obj.CollectionEvent; }
            }

            internal override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject ISO
            {
                get { return _obj; }
            }

            internal override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
            {
                get { yield return _obj.CollectionAgent.First(); }
            }
        }
    }
}
