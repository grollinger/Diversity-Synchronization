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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
using MVVMDiversity.Enums;

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
