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
using UBT.AI4.Bio.DivMobi.DataLayer.SyncAttributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class Property : ISerializableObject
    {
        #region Instance Data

        [ID]
        [Column]
        [DirectSync]
        private int _PropertyID;
        [Column]
        private int? _PropertyParentID;
        [Column]
        private string _PropertyName;
        [Column]
        private string _DefaultAccuracyOfProperty;
        [Column]
        private string _DefaultMeasurementUnit;
        [Column]
        private string _ParsingMethodName;
        [Column]
        private string _DisplayText;
        [Column]
        private bool? _DisplayEnabled;
        [Column]
        private short? _DisplayOrder;
        [Column]
        private string _Description;
        
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;
        
        #endregion

        #region Default constructor

        public Property()
        {
            this.PropertyName = @"unknown";
            this.ParsingMethodName = @"unknown";
        }

        #endregion

        #region Properties

        public int PropertyID { get { return _PropertyID; } set { _PropertyID = value; } }

        public int? PropertyParentID { get { return _PropertyParentID; } set { _PropertyParentID = value; } }

        public string PropertyName { get { return _PropertyName; } set { _PropertyName = value; } }

        public string DefaultAccuracyOfProperty { get { return _DefaultAccuracyOfProperty; } set { _DefaultAccuracyOfProperty = value; } }

        public string DefaultMeasurementUnit { get { return _DefaultMeasurementUnit; } set { _DefaultMeasurementUnit = value; } }

        public string ParsingMethodName { get { return _ParsingMethodName; } set { _ParsingMethodName = value; } }

        public string DisplayText { get { return _DisplayText; } set { _DisplayText = value; } }

        public bool? DisplayEnabled { get { return _DisplayEnabled; } set { _DisplayEnabled = value; } }

        public short? DisplayOrder { get { return _DisplayOrder; } set { _DisplayOrder = value; } }

        public string Description { get { return _Description; } set { _Description = value; } }
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Tabelle wird nur vom Repository aufs Mobilgerät geschrieben. Etwaige Änderungen sollen bis zum nächsten Clean ignoriert werden.. Deswegen wird defaultmäßig der 1.Januar 1900 zurückgegeben.
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        #endregion

        #region ToString override

        public override string ToString()
        {
            if (this.DisplayText != null)
            {
                String text = "Property: " + this.DisplayText;
                return text;
            }
            else return AttributeWorker.ToStringHelper(this, 30);
        }

        #endregion
    }
}
