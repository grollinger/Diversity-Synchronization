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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
namespace UBT.AI4.Bio.DivMobi.ListSynchronization
{
    public class ListContainer
    {
        internal ISerializableObject iso;
        internal ISerializableObject partner;
        private SyncStates_Enum state;

        public ListContainer(ISerializableObject iso)
        {
            this.iso=iso;
            this.state = SyncStates_Enum.PrematureState;
        }

        public ListContainer(ISerializableObject iso, SyncStates_Enum state):this(iso)
        {
            this.state = state;
        }

        public ListContainer(ISerializableObject iso, ISerializableObject partner, SyncStates_Enum state)
            : this(iso, state)
        {
            this.partner = partner;
        }

        public SyncStates_Enum State
        {
            get { return state; }
            set { state = value; }
        }
        public ISerializableObject Iso
        {
            get { return iso; }
        }
        public ISerializableObject Partner
        {
            get { return partner; }
        }

    }
}
