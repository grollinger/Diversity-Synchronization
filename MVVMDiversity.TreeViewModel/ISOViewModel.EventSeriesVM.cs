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
using MVVMDiversity.Enums;

namespace MVVMDiversity.ViewModel
{
    public partial class ISOViewModel
    {
        private class EventSeriesVM : ISOViewModel
        {

            public EventSeriesVM(CollectionEventSeries ces)
                : base(ces) { }

            

            private CollectionEventSeries CES { get { return ISO as CollectionEventSeries; } }                 

            public override IEnumerable<ISerializableObject> Children
            {
                get
                {
                    if (CES != null)
                    {
                        foreach (var ev in CES.CollectionEvents)
                            yield return ev;
                    }
                }
            }          

            protected override string getName()
            {
                if (CES != null)
                {
                    return string.Format("{0}, {1} {2}",
                        CES.SeriesCode,
                        CES.Description,
                        (CES.DateStart != null) ? CES.DateStart.Value.ToString("dd.MM.yyyy HH:mm") : "[No Date]");
                }
                else
                    return "No Event Series";

            }

            protected override ISOIcon getIcon()
            {
                return ISOIcon.EventSeries;
            }
        }
    }
}
