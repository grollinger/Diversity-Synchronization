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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class CollectionProject:ISerializableObject
    {
        #region Instance data
        [ID]
        [Column]
        private int _CollectionSpecimenID;

        [ID]
        [Column]
        private int _ProjectID;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [Column]
        private DateTime _LogUpdatedWhen;

        #endregion

        #region Default Constructor

        public CollectionProject()
        {
        }

        #endregion

        #region Properties

        public int CollectionSpecimenID { get { return _CollectionSpecimenID; } set { _CollectionSpecimenID = value; } }
        public int ProjectID { get { return _ProjectID; } set { _ProjectID = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        #endregion

        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CollectionProject: ");
            if (this._ProjectID != null)
            {
                sb.Append(this._ProjectID).Append(",");
            }
            if (this._CollectionSpecimenID != null)
                sb.Append(this._CollectionSpecimenID);
            return sb.ToString();
        }

        #endregion

    }
}
