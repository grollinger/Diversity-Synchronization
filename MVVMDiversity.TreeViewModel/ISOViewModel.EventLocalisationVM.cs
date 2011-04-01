using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using MVVMDiversity.Enums;

namespace MVVMDiversity.ViewModel
{
	public partial class ISOViewModel
	{
        private class EventLocalisationVM : ISOViewModel
        {
            public EventLocalisationVM(CollectionEventLocalisation l)
                : base(l)
            {

            }

            private CollectionEventLocalisation LOC { get { return ISO as CollectionEventLocalisation; } }

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
                if (LOC != null)
                {
                    //Geo Localisation
                    if (LOC.LocalisationSystemID == 8)
                    {
                        if (LOC.Location1 != null && LOC.Location2 != null)
                            try
                            {
                                double lat = Double.Parse(LOC.Location2, System.Globalization.NumberStyles.AllowDecimalPoint);
                                double lon = Double.Parse(LOC.Location1, System.Globalization.NumberStyles.AllowDecimalPoint);

                                return formatLocalisation(lat, lon);
                            }
                            catch
                            {
                                
                            }
                    }
                    //Altitude
                    else if (LOC.LocalisationSystemID == 4)
                    {
                        if(LOC.Location1 != null)
                            try
                            {
                                double alt = Double.Parse(LOC.Location1);
                                return formatAltitude(alt);
                            }
                            catch
                            {                      
                            }
                    }
                    return string.Format("({0};{1})",
                        LOC.Location1 ?? "",
                        LOC.Location2 ?? "");
                }
                else
                    return "No Localisation";


            }

            protected override Enums.ISOIcon getIcon()
            {
                return ISOIcon.Location;
            }
        }
	}
}
