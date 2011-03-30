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
            
            public CollectionSpecimenVM(CollectionSpecimen spec)
                :base(spec)
            {
                
            }

            private CollectionSpecimen SPEC { get { return ISO as CollectionSpecimen; } }


            public override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject Parent
            {
                get 
                {
                    if (SPEC != null)
                        return SPEC.CollectionEvent;
                    else
                        return null;
                }
            }           

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
            {
                get 
                {
                    if (SPEC != null)
                        foreach (var iu in SPEC.IdentificationUnits)
                            yield return iu;
                }
            }

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Properties
            {
                get
                {
                    if (SPEC != null)
                        yield return SPEC.CollectionAgent.First();
                }
            }

            protected override string getName()
            {
                if (ISO != null)
                    return ISO.ToString();
                else
                    return "No CollectionSpecimen";
            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.Specimen;
            }
        }
    }
}
