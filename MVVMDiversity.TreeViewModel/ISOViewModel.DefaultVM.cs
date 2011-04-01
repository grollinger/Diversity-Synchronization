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
using MVVMDiversity.Enums;

namespace MVVMDiversity.ViewModel
{
	public partial class ISOViewModel
	{
        private class DefaultVM : ISOViewModel
        {            
            public DefaultVM(ISerializableObject iso)
                : base(iso)
            {
               
            }


            public override ISerializableObject Parent
            {
                get 
                { 
                    //Really bad idea to 'return this' here
                    return null; 
                }
            }            

            public override IEnumerable<ISerializableObject> Children
            {
                get { return null; }
            }

            public override IEnumerable<ISerializableObject> Properties
            {
                get { return null; }
            }

            protected override string getName()
            {
                return (ISO != null) ? ISO.ToString() : "No ISO";
            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.Unknown;
            }
        }
	}
}
