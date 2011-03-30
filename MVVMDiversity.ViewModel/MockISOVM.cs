using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.ViewModel;

namespace MVVMDiversity.ViewModel
{
    public class MockISOVM : ISOViewModel
    {
        public MockISOVM()
        {
            Name = "Mockingbird";
            IsExpanded = true;
        }

        internal override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject Parent
        {
            get { return null; }
        }

        internal override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject ISO
        {
            get { return null; }
        }

        internal override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
        {
            get { return null; }
        }
    }
}
