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
        private class IdentificationUnitVM : ISOViewModel
        {
            IdentificationUnit _obj;
            public IdentificationUnitVM(IdentificationUnit iu)
            {
                _obj = iu;
            }



            internal override ISerializableObject Parent
            {
                get 
                {
                    ISerializableObject res;
                    if(_obj.RelatedUnit != null) 
                        res = _obj.RelatedUnit;
                    else
                        res = _obj.CollectionSpecimen;
                    return res;
                }
            }

            internal override ISerializableObject ISO
            {
                get { throw new NotImplementedException(); }
            }

            internal override IEnumerable<ISerializableObject> Children
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
