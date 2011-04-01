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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using MVVMDiversity.Enums;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class EventPropertyVM : ISOViewModel
        {

            public EventPropertyVM (CollectionEventProperty p)
                : base(p)
	        {

	        }

            private CollectionEventProperty PROP { get { return ISO as CollectionEventProperty; } }


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
                if (PROP != null)
                {
                    if (!string.IsNullOrEmpty(PROP.DisplayText))
                        return PROP.DisplayText;
                    else
                        return string.Format("EventProperty ({0})", PROP.PropertyID);
                }
                else
                    return "No Property";
            }

            protected override Enums.ISOIcon getIcon()
            {
                return ISOIcon.SiteProperty;
            }
        }
    }
}
