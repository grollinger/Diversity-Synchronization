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
using System.Data.Common;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public enum UpdateStates
    {
        UPDATED, NO_ROWS_AFFECTED, PRIMARYKEY_MODIFIED
    }

    public interface ISerializableObjectPool : IObeserver
    {
        ISerializableObject RetrieveObject(Type objectType, DbDataReader reader, ref bool isLoaded);
        void InsertObject(ISerializableObject iso, DbConnection connection, ISerializerTransaction transaction);
        void InsertObject(ISerializableObject iso, DbConnection connection, ISerializerTransaction transaction,string table);
        UpdateStates UpdateObject(ISerializableObject iso, DbConnection connection, ISerializerTransaction transaction);
        void DeleteObject(ISerializableObject iso, DbConnection connection, ISerializerTransaction transaction);
        bool IsManagedObject(ISerializableObject iso);
        void Register(ISerializableObject iso);
    }
}
