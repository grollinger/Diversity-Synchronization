using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class EventSeriesVM : ISOViewModel
        {

            public EventSeriesVM(CollectionEventSeries ces)
                : base(ces) { }

            

            private CollectionEventSeries CES { get { return ISO as CollectionEventSeries; } }
            
            public override ISerializableObject Parent
            {
                get 
                {
                    return null;
                }
            }           

            public override IEnumerable<ISerializableObject> Children
            {
                get
                {
                    if (CES != null)
                    {
                        foreach (var ev in CES.CollectionEvents)
                            yield return ev;
                    }
                }
            }           

            public override IEnumerable<ISerializableObject> Properties
            {
                get 
                {
                    return null;
                }
            }

            protected override string getName()
            {
                if (CES != null)
                {
                    return string.Format("{0}, {1} {2}",
                        CES.SeriesCode,
                        CES.Description,
                        (CES.DateStart != null) ? CES.DateStart.Value.ToString("dd.MM.yyyy HH:mm") : "[No Date]");
                }
                else
                    return "No Event Series";

            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.EventSeries;
            }
        }
    }
}
