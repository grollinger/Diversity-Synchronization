//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Tobias Schneider, Lehrstuhl Angewndte Informatik IV, Universität Bayreuth
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

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public enum ExpandLevel
    {
        EventSeries=0,
        Event=1,
        Specimen=2,
        IdentificationUnit=3
    }

    public class DisplayLevel
    {
        private ExpandLevel lvl=ExpandLevel.Specimen;

        public DisplayLevel(string input)
        {
            switch (input)
            {
                case "EventSeries":
                    lvl = ExpandLevel.EventSeries;
                    break;
                case "Event":
                    lvl = ExpandLevel.Event;
                    break;
                case "Specimen":
                    lvl = ExpandLevel.Specimen;
                    break;
                case "IdentificationUnit":
                    lvl = ExpandLevel.IdentificationUnit;
                    break;
                default:
                    lvl = ExpandLevel.Specimen;
                    break;
            }
        }

        public int? Index
        {
            get
            {
                if (this.lvl != null)
                    return (int)lvl;
                else
                    return null;
            }
        }

        public string DisplayLevelString
        {
            get
            {
                switch (lvl)
                {
                    case ExpandLevel.EventSeries:
                        return "EventSeries";
                    case ExpandLevel.Event:
                        return "Event";
                    case ExpandLevel.Specimen:
                        return "Specimen";
                    case ExpandLevel.IdentificationUnit:
                        return "IdentificationUnit";
                    default:
                        return "Unknown";
                }
            }
        }

        public override string ToString()
        {
            if (this.DisplayLevelString != null)
            {
                String text = "Expandlevel: " + this.DisplayLevelString;
                return text;
            }
            else return "Expandlevel: unknown";
        }
    }


}