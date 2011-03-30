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
            
            public CollectionEventVM(CollectionEvent ev)
                :base(ev)
            {
                
            }

            public CollectionEvent EV { get { return ISO as CollectionEvent; } }

            private static string getName(CollectionEvent spec)
            {
                throw new NotImplementedException();
            }

            public override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject Parent
            {
                get
                {
                    if (EV != null)
                        return EV.CollectionEventSeries;
                    else
                        return null;
                }
            }          

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
            {
                get 
                {
                    if (EV != null)
                        foreach (var spec in EV.CollectionSpecimen)
                            yield return spec;
                }
            }

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Properties
            {
                get
                {
                    if (EV != null)
                    {
                        foreach (var prop in EV.CollectionEventProperties)
                            yield return prop;
                        foreach (var loc in EV.CollectionEventLocalisation)
                            yield return loc;
                    }
                }
            }

            protected override string getName()
            {
                if (ISO != null)
                    return ISO.ToString();
                else
                    return "No CollectionEvent";
            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.Event;
            }
        }
    }
}
