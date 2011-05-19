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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class IdentSave: ISerializableObject
    {
        [ID]
        [Column]
        private String _LastIdentificationCache;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        private DateTime _LogUpdatedWhen;

        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public String LastIdentificationCache
        {
            get { return _LastIdentificationCache; }
            set { _LastIdentificationCache = value; }
        }

        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }
    }
}