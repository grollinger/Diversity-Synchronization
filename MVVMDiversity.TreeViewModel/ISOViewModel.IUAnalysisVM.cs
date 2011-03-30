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
        private class IUAnalysisVM : ISOViewModel
        {
            public IUAnalysisVM(IdentificationUnitAnalysis an)
                :base (an)
            {                    
            }

            private IdentificationUnitAnalysis IUA { get { return ISO as IdentificationUnitAnalysis; } }

            public override ISerializableObject Parent
            {
                get 
                {
                    if (IUA != null)
                        return IUA.IdentificationUnit;
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
                if (IUA != null)
                    return string.Format("{0}{1}: {2}",
                        IUA.AnalysisResult ?? "",
                        (IUA.Analysis != null) ? IUA.Analysis.MeasurementUnit ?? "" : "",
                        (IUA.Analysis != null) ? IUA.Analysis.DisplayText ?? "" : "");
                return "No IU Analysis";
            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.Analysis;
            }

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
            {
                get { return null; }
            }
        }
    }
}
