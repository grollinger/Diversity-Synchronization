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
using System.Collections.Generic;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{

    public class CollectionAgent : ISerializableObject
    {
        #region Instance data

        [ID]
        [Column]
        private int        _CollectionSpecimenID;
        [ID]
        [Column]
        private string      _CollectorsName;
        [Column]
        private string      _CollectorsAgentURI;
        [Column]
        private DateTime    _CollectorsSequence;
        [Column]
        private string      _CollectorsNumber;

        [Column]
        private string _Notes;
        //[Column]
        //private string      _DataWithholdingReason;
        
        //[Column(Mapping="xx_IsAvailable")]
        //private bool?       _IsAvailable;
        [Column]
        private DateTime _LogUpdatedWhen;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;


        //[ManyToOne]
        //[MappedBy("_collectionAgents")]
        //private CollectionSpecimen _collectionSpecimen;

        #endregion


        #region Default constructor

        public CollectionAgent() 
        {
            this.CollectorsSequence = DateTime.Now;
            //this.IsAvailable = false;
        }

        #endregion



        #region Properties

        public int CollectionSpecimenID { get { return _CollectionSpecimenID; } set { _CollectionSpecimenID = value; } }
        public string CollectorsName { get { return _CollectorsName; } set { _CollectorsName = value; } }
        public string CollectorsAgentURI { get { return _CollectorsAgentURI; } set { _CollectorsAgentURI = value; } }
        public DateTime CollectorsSequence { get { return _CollectorsSequence; } set { _CollectorsSequence = value; } }
        public string CollectorsNumber { get { return _CollectorsNumber; } set { _CollectorsNumber = value; } }
        public string Notes { get { return _Notes; } set { _Notes = value; } }
        //public string DataWithholdingReason { get { return _DataWithholdingReason; } set { _DataWithholdingReason = value; } }
        //public bool? IsAvailable { get { return _IsAvailable; } set { _IsAvailable = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        //public CollectionSpecimen CollectionSpecimen
        //{
        //    get { return _collectionSpecimen; }
        //    set { _collectionSpecimen = value; }
        //}
        #endregion



        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CollectionAgent: ");
            if (this._CollectorsName != null)
            {
                sb.Append(this._CollectorsName).Append(",");
            }
            if (this._CollectionSpecimenID != null)
                sb.Append(this._CollectionSpecimenID);
            return sb.ToString();
        }

        #endregion
    }
}
