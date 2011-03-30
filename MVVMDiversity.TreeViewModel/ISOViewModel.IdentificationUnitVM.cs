using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel

    {
        private class IdentificationUnitVM : ISOViewModel
        {
            
            public IdentificationUnitVM(IdentificationUnit iu)
                : base(iu)
            {     
            }

            private IdentificationUnit IU { get { return ISO as IdentificationUnit; } }      

            private static ISOIcon getIcon(IdentificationUnit iu)
            {
                ISOIcon ico = ISOIcon.Unknown;               

                if (iu.TaxonomicGroup != null)
                {
                    
                }
                return ico;
            }



            public override ISerializableObject Parent
            {
                get 
                {
                    if(IU != null)
                        if(IU.RelatedUnit != null) 
                            return IU.RelatedUnit;
                        else
                            return IU.CollectionSpecimen;
                    return null;
                }
            }            

            public override IEnumerable<ISerializableObject> Children
            {
                get 
                {
                    if (IU != null)
                    {
                        foreach (var iu in IU.ChildUnits)
                            yield return iu;

                        foreach (var item in IU.IdentificationUnitAnalysis)
                            yield return item;
                    }
                }
            }

            public override IEnumerable<ISerializableObject> Properties
            {
                get 
                {
                    if (IU != null)
                    {
                        foreach (var item in IU.IdentificationUnitGeoAnalysis)
                            yield return item;
                    }
                }
            }

            protected override string getName()
            {
                if (IU != null)
                {
                    return string.Format("{0}{1}, {2}",
                        (!string.IsNullOrEmpty(IU.UnitIdentifier))? "("+IU.UnitIdentifier+") " : "",
                        IU.UnitDescription ?? "",
                        IU.LastIdentificationCache ?? "");
                }
                return "No Identification Unit";        
            }

            protected override ISOIcon getIcon()
            {
                ISOIcon ico = ISOIcon.Unknown;
                if (IU != null)
                {
                    try
                    {
                        ico = (ISOIcon)Enum.Parse(typeof(ISOIcon), IU.TaxonomicGroup ?? "", true);
                    }
                    catch (Exception)
                    {
                    }
                }
                return ico;
            }
        }
    }
}
