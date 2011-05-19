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


namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    [Table("MappingDictionary")]
    public class PropertyNames:ISerializableObject
    {
        #region Instance Data

        [Column]
        private int _PropertyID;

        
        [Column]
        private string _PropertyURI;

        [Column]
        private string _DisplayText;

        [Column]
        private string _HierarchyCache;

        [ID]
        [Column]
        private int _TermID;

        [Column]
        private int _BroaderTermID;
        
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [ManyToOne]
        [JoinColums(DefColumn = "_TermID", JoinColumn = "_BroaderTermID")]
        private PropertyNames _parentPropertyName;

        #endregion

        public PropertyNames()
        {
        }

        #region Properties

        public int PropertyID { get { return _PropertyID; } set { _PropertyID = value; } }

        public string PropertyURI { get { return _PropertyURI; } set { _PropertyURI = value; } }

        public string DisplayText { get { return _DisplayText; } set { _DisplayText = value; } }

        public string HierarchyCache { get { return _HierarchyCache; } set { _HierarchyCache = value; } }

        public int TermID { get { return _TermID; } set { _TermID = value; } }

        public int BroaderTermID { get { return _BroaderTermID; } set { _BroaderTermID = value; } }
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Tabelle wird nur vom Repository aufs Mobilgerät geschrieben. Etwaige Änderungen sollen bis zum nächsten Clean ignoriert werden.. Deswegen wird defaultmäßig der 1.Januar 1900 zurückgegeben.
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        public PropertyNames ParentPropertyName
        {
            get { return _parentPropertyName; }
        }
        #endregion

        #region ToString override

        public override string ToString()
        {
            if (this.DisplayText != null)
            {
                String text = "PropertyNames: " + this.DisplayText;
                return text;
            }
            else return AttributeWorker.ToStringHelper(this, 30);
        }

        #endregion

    }
}
