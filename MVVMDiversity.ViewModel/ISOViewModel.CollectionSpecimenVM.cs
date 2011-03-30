using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class CollectionSpecimenVM : ISOViewModel
        {
            CollectionSpecimen _obj;
            public CollectionSpecimenVM(CollectionSpecimen spec)
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
