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

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class CollectionEventVM : ISOViewModel
        {
            
            public CollectionEventVM(CollectionEvent ev)
                :base(ev)
            {
                
            }

            public CollectionEvent EV { get { return ISO as CollectionEvent; } }

            private static string getName(CollectionEvent spec)
            {
                throw new NotImplementedException();
            }

            public override UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject Parent
            {
                get
                {
                    if (EV != null)
                        return EV.CollectionEventSeries;
                    else
                        return null;
                }
            }          

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Children
            {
                get 
                {
                    if (EV != null)
                        foreach (var spec in EV.CollectionSpecimen)
                            yield return spec;
                }
            }

            public override IEnumerable<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> Properties
            {
                get
                {
                    if (EV != null)
                    {
                        foreach (var prop in EV.CollectionEventProperties)
                            yield return prop;
                        foreach (var loc in EV.CollectionEventLocalisation)
                            yield return loc;
                    }
                }
            }

            protected override string getName()
            {
                if (ISO != null)
                    return ISO.ToString();
                else
                    return "No CollectionEvent";
            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.Event;
            }
        }
    }
}
