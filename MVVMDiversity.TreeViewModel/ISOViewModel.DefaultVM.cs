using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace MVVMDiversity.ViewModel
{
	public partial class ISOViewModel
	{
        private class DefaultVM : ISOViewModel
        {            
            public DefaultVM(ISerializableObject iso)
                : base(iso)
            {
               
            }


            public override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject Parent
            {
                get 
                { 
                    //Really bad idea to 'return this' here
                    return null; 
                }
            }            

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
            {
                get { return null; }
            }

            public override IEnumerable<ISerializableObject> Properties
            {
                get { return null; }
            }

            protected override string getName()
            {
                return (ISO != null) ? ISO.ToString() : "No ISO";
            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.Unknown;
            }
        }
	}
}
