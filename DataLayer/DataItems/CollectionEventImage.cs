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
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
using UBT.AI4.Bio.DivMobi.DataLayer.SyncAttributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class CollectionEventImage : ISerializableObject
    {
        #region Instance Data

        [ID]
        [Column]
        private int?    _CollectionEventID;
        [ID]
        [Column]
        private string  _URI;
        //[Column]
        //private string  _ResourceURI;
        [Column]
        [DirectSync]
        private string  _ImageType;
        //[Column]
        //private string  _Notes;
        [Column]
        private DateTime _LogUpdatedWhen;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        //[ManyToOne]
        //[MappedBy("_collectionEventImages")]
        //private CollectionEvent _collectionEvent;

        //[ManyToOne]
        //[MappedBy("_collectionEventImages")]
        //private CollEventImageType_Enum _collectionEventImageType;
        #endregion



        #region Default constructor

        public CollectionEventImage()
        {
            this._ImageType = "photography";
        }

        #endregion



        #region Properties

        public int? CollectionEventID { get { return _CollectionEventID; } set { _CollectionEventID = value; } }
        public string URI { get { return _URI; } set { _URI = value; } }
        //public string ResourceURI { get { return _ResourceURI; } set { _ResourceURI = value; } }
        public string ImageType { get { return _ImageType; } set { _ImageType = value; } }
        //public string Notes { get { return _Notes; } set { _Notes = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        //public CollectionEvent CollectionEvent 
        //{
        //    get { return _collectionEvent; }
        //    set { _collectionEvent = value; } 
        //}
        //public CollEventImageType_Enum CollEventImageType_Enum
        //{
        //    get { return _collectionEventImageType; }
        //    set { _collectionEventImageType = value; }
        //}
        #endregion



        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CollectionEventImage: ");
            if (this._ImageType != null)
            {
                sb.Append(this._ImageType).Append(",");
            }
            if (this._CollectionEventID != null)
            {
                sb.Append(this._CollectionEventID).Append(",");
            }
            if (this._URI != null)
                sb.Append(this._URI);
            return sb.ToString();
        }

        #endregion
    }
}
