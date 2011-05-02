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
        private class EventLocalisationVM : ISOViewModel
        {
            public EventLocalisationVM(CollectionEventLocalisation l)
                : base(l)
            {

            }

            private CollectionEventLocalisation LOC { get { return ISO as CollectionEventLocalisation; } }            

            protected override string getName()
            {
                if (LOC != null)
                {
                    //Geo Localisation
                    if (LOC.LocalisationSystemID == 8)
                    {
                        if (LOC.Location1 != null && LOC.Location2 != null)
                            try
                            {
                                double lat = Double.Parse(LOC.Location2, System.Globalization.NumberStyles.AllowDecimalPoint);
                                double lon = Double.Parse(LOC.Location1, System.Globalization.NumberStyles.AllowDecimalPoint);

                                return formatLocalisation(lat, lon);
                            }
                            catch
                            {
                                
                            }
                    }
                    //Altitude
                    else if (LOC.LocalisationSystemID == 4)
                    {
                        if(LOC.Location1 != null)
                            try
                            {
                                double alt = Double.Parse(LOC.Location1);
                                return formatAltitude(alt);
                            }
                            catch
                            {                      
                            }
                    }
                    return string.Format("({0};{1})",
                        LOC.Location1 ?? "",
                        LOC.Location2 ?? "");
                }
                else
                    return "No Localisation";


            }

            protected override Enums.ISOIcon getIcon()
            {
                return ISOIcon.Location;
            }
        }
	}
}
