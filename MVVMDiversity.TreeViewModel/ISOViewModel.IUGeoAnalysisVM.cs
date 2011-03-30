using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class IUGeoAnalysisVM : ISOViewModel
        {
            public IUGeoAnalysisVM(IdentificationUnitGeoAnalysis an)
                : base(an)
            {
            }

            private IdentificationUnitGeoAnalysis IUGA { get { return ISO as IdentificationUnitGeoAnalysis; } }

            public override ISerializableObject Parent
            {
                get
                {
                    if (IUGA != null)
                        return IUGA.IdentificationUnit;
                    else
                        return null;
                }
            }

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Properties
            {
                get { return null; }
            }

            protected override string getName()
            {
                if (IUGA != null)
                {                    
                    if (IUGA.GeoLatitude != null && IUGA.GeoLongitude != null)
                    {
                        return string.Format("{0}{1} {2}{3}",
                            dec2degree(IUGA.GeoLatitude ?? 0d),
                            (IUGA.GeoLatitude >= 0) ? "N" : "S",
                            dec2degree(IUGA.GeoLongitude ?? 0d),
                            (IUGA.GeoLongitude >= 0) ? "E" : "W");               
                    }
                    else
                    {
                       return IUGA.Geography;
                    }
                }
                return "NO IU GeoAnalysis";
            }

            public static string dec2degree(double dec)
            {
                int deg = (int)dec;
                dec -= deg;
                dec *= 60;
                int min = (int)dec;
                dec -= min;
                dec *= 60;
                int sec = (int)dec;

                return string.Format("{0}° {1}' {2}''",
                    deg,
                    min,
                    sec);                         
            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.GeoAnalysis;
            }

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
            {
                get { return null; }
            }
        }
    }
}
