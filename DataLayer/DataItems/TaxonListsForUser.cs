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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class TaxonListsForUser:ISerializableObject 
    {
        #region Instance Data

        [ID]
        [Column]
        private string _DataSource;

        [Column]
        private string _DisplayText;
        [Column]
        private string _TaxonomicGroup;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        #endregion

        #region Default constructor

        public TaxonListsForUser() { }

        #endregion

        #region Properties

        public string DataSource { get { return _DataSource; } set { _DataSource = value; } }
        public string DisplayText { get { return _DisplayText; } set { _DisplayText = value; } }
        public string TaxonomicGroup{get{return _TaxonomicGroup;} set{_TaxonomicGroup=value;}}
        
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Einträge der Tabelle werden nicht synchronisiert. Sie werden nur benötigt um die Schnittstelle zu implementieren.
        #endregion

        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("TaxonListsForUser: ");
            if (this._DataSource != null)
            {
                sb.Append(this._DataSource).Append(",");
            }
            if (this._DisplayText != null)
            {
                sb.Append(this._DisplayText).Append(",");
            }
            if(this._TaxonomicGroup!=null)
            {
                sb.Append(this._TaxonomicGroup);
            }
            return sb.ToString();
        }

        #endregion
    }
}

