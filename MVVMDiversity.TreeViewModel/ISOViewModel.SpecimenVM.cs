//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Georg Rollinger
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using MVVMDiversity.Enums;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class SpecimenVM : ISOViewModel
        {
            
            public SpecimenVM(CollectionSpecimen spec)
                :base(spec)
            {
                
            }

            private CollectionSpecimen SPEC { get { return ISO as CollectionSpecimen; } }


            public override ISerializableObject Parent
            {
                get 
                {
                    if (SPEC != null)
                        return SPEC.CollectionEvent;
                    else
                        return null;
                }
            }           

            public override IEnumerable<ISerializableObject> Children
            {
                get 
                {
                    if (SPEC != null)
                        foreach (var iu in SPEC.IdentificationUnits)
                            yield return iu;
                }
            }

            public override IEnumerable<ISerializableObject> Properties
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
                {
                    if (!string.IsNullOrEmpty(SPEC.AccessionNumber))
                        return SPEC.AccessionNumber;
                    else
                        return string.Format("Specimen [{0}]",SPEC.CollectionSpecimenID);
                }
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
