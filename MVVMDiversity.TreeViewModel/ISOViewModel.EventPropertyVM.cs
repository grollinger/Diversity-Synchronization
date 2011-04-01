using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using MVVMDiversity.Enums;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class EventPropertyVM : ISOViewModel
        {

            public EventPropertyVM (CollectionEventProperty p)
                : base(p)
	        {

	        }

            private CollectionEventProperty PROP { get { return ISO as CollectionEventProperty; } }


            public override ISerializableObject Parent
            {
                get { return null; }
            }

            public override IEnumerable<ISerializableObject> Properties
            {
                get { return null; }
            }

            public override IEnumerable<ISerializableObject> Children
            {
                get { return null; }
            }

            protected override string getName()
            {
                if (PROP != null)
                {
                    if (!string.IsNullOrEmpty(PROP.DisplayText))
                        return PROP.DisplayText;
                    else
                        return string.Format("EventProperty ({0})", PROP.PropertyID);
                }
                else
                    return "No Property";
            }

            protected override Enums.ISOIcon getIcon()
            {
                return ISOIcon.SiteProperty;
            }
        }
    }
}
