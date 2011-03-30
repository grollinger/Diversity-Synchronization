using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class EventSeriesVM : ISOViewModel
        {
            CollectionEventSeries _obj;
            public EventSeriesVM(CollectionEventSeries ces)                
            {

            }

            internal override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject Parent
            {
                get 
                {
                    return null;
                }
            }

            internal override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject ISO
            {
                get { throw new NotImplementedException(); }
            }

            internal override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
