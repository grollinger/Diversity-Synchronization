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
        private class EventVM : ISOViewModel
        {
            
            public EventVM(CollectionEvent ev)
                :base(ev)
            {
                
            }

            public CollectionEvent EV { get { return ISO as CollectionEvent; } }            

            public override ISerializableObject Parent
            {
                get
                {
                    if (EV != null)
                        return EV.CollectionEventSeries;
                    else
                        return null;
                }
            }          

            public override IEnumerable<ISerializableObject> Children
            {
                get 
                {
                    if (EV != null)
                        foreach (var spec in EV.CollectionSpecimen)
                            yield return spec;
                }
            }

            public override IEnumerable<ISerializableObject> Properties
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
                {
                    return string.Format("{0}{1} [{2}]",
                        !string.IsNullOrEmpty(EV.LocalityDescription) ? EV.LocalityDescription + ", " : "",
                        (EV.CollectionDate != null) ? EV.CollectionDate.ToShortDateString() : string.Empty,
                        (EV.CollectorsEventNumber!=null) ? EV.CollectorsEventNumber : string.Empty);
                }
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
