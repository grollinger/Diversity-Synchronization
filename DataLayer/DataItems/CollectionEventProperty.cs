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
    public class CollectionEventProperty : ISerializableObject
    {
        #region Instance data

        [ID]
        [Column]
        private int? _CollectionEventID;
        [ID]
        [Column]
        private int? _PropertyID;
        [Column]
        private string _DisplayText;
        [Column]
        private string _PropertyURI;
        [Column]
        private string _PropertyHierarchyCache;
        [Column]
        private string _PropertyValue;
        [Column]
        private string _ResponsibleName;
        [Column]
        private string _ResponsibleAgentURI;
        //[Column]
        //private string _Notes;
        [Column]
        private float? _AverageValueCache;
        [Column]
        private DateTime _LogUpdatedWhen;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        //[ManyToOne]
        //[MappedBy("_collectionEventProperties")]
        //private CollectionEvent _collectionEvent;

        //[ManyToOne]
        //[JoinColums(DefColumn = "_PropertyID", JoinColumn="_PropertyID")]
        //private Property _property;

        #endregion

         #region Default constructor

        public CollectionEventProperty()
        {
        }

        #endregion

        #region Properties

        public int? CollectionEventID { get { return _CollectionEventID; } set { _CollectionEventID = value; } }

        public int? PropertyID { get { return _PropertyID; } set { _PropertyID = value; } }

        public string DisplayText { get { return _DisplayText; } set { _DisplayText = value; } }

        public string PropertyURI { get { return _PropertyURI; } set { _PropertyURI = value; } }

        public string PropertyHierarchyCache { get { return _PropertyHierarchyCache; } set { _PropertyHierarchyCache = value; } }

        public string PropertyValue { get { return _PropertyValue; } set { _PropertyValue = value; } }

        public string ResponsibleName { get { return _ResponsibleName; } set { _ResponsibleName = value; } }

        public string ResponsibleAgentURI { get { return _ResponsibleAgentURI; } set { _ResponsibleAgentURI = value; } }

        //public string Notes { get { return _Notes; } set { _Notes = value; } }

        public float? AverageValueCache { get { return _AverageValueCache; } set { _AverageValueCache = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        //public CollectionEvent Event
        //{
        //    get { return _collectionEvent; }
        //}
        //public Property Property
        //{
        //    get { return _property; }
        //}
        #endregion

        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CollectionEventPorperty: ");
            if (this._PropertyID != null)
            {
                sb.Append(this._PropertyID).Append(",");
            }
            if (this._CollectionEventID != null)
            {
                sb.Append(this._CollectionEventID).Append(",");
            }
            if (this._DisplayText != null)
                sb.Append(this._DisplayText);
            return sb.ToString();
        }

        #endregion
    }
}
