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
using System.Collections.Generic;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
using UBT.AI4.Bio.DivMobi.DataLayer.SyncAttributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    
    
    public class CollectionEvent : ISerializableObject
    {
        #region Instance data

        [ID(Autoinc=true)]
        [Column]
        private int _CollectionEventID;
        [Column]
        private int? _Version;
        [Column]
        private int? _SeriesID;
        [Column]
        private string _CollectorsEventNumber;
        [Column]
        private DateTime _CollectionDate;
        [Column]
        private byte? _CollectionDay;
        [Column]
        private byte? _CollectionMonth;
        [Column]
        private short? _CollectionYear;

        [Column]
        private string _CollectionDateSupplement;

        [Column]
        [CheckForeingKeyConstraint("CollEventDateCategory_Enum", ExternColumn = "Code", Target = "server")]
        private string _CollectionDateCategory;
        [Column]
        private string _CollectionTime;
        [Column]
        private string _CollectionTimeSpan;
        [Column]
        private string _LocalityDescription;
        [Column]
        private string _HabitatDescription;

        [Column]
        private string _ReferenceTitle;
        [Column]
        private string _ReferenceURI;

        [Column]
        private string _CollectingMethod;

        [Column]
        private string _Notes;
        [Column]
        private string _CountryCache;

        [Column]
        private DateTime _LogUpdatedWhen;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [Resolve]
        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        [JoinColums(DefColumn = "_CollectionEventID", JoinColumn = "_CollectionEventID")]
        private IDirectAccessIterator<CollectionSpecimen> _collectionSpecimen;

        //[OneToMany(DeleteType = DeleteTypes.CASCADE)]
        //[JoinColums(DefColumn = "_CollectionEventID", JoinColumn = "_CollectionEventID")]
        //private IDirectAccessIterator<CollectionEventImage> _collectionEventImages;

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        [JoinColums(DefColumn = "_CollectionEventID", JoinColumn = "_CollectionEventID")]
        private IDirectAccessIterator<CollectionEventLocalisation> _collectionEventLocalisations;

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        [JoinColums(DefColumn = "_CollectionEventID", JoinColumn = "_CollectionEventID")]
        private IDirectAccessIterator<CollectionEventProperty> _collectionEventProperties;

        [ManyToOne]
        [MappedBy("_CollectionEvents")]
        private CollectionEventSeries _CollectionEventSeries;

       

        #endregion


        #region Default constructor

        public CollectionEvent()
        {
            this.Version = 1;
            DateTime now = DateTime.Now;
            this.CollectionDate = now.Date;
            this._CollectionDay = (byte?)CollectionDate.Day;
            this._CollectionMonth = (byte?)CollectionDate.Month;
            this._CollectionYear = (short?)CollectionDate.Year;
            //this.CollectionTime = now.ToLongTimeString();Soll nicht verwendet werden laut Markus
            this._CollectionDateCategory = "actual";

            //this.SeriesID = 0;
            //this.IsAvailable = false;
        }

        #endregion


        #region Properties

        public int CollectionEventID { get { return _CollectionEventID; } set { _CollectionEventID = value; } }
        public int? Version { get { return _Version; } set { _Version = value; } }
        public int? SeriesID { get { return _SeriesID; } set { _SeriesID = value; } }
        public string CollectorsEventNumber { get { return _CollectorsEventNumber; } set { _CollectorsEventNumber = value; } }
        public DateTime CollectionDate { get { return _CollectionDate; } set { _CollectionDate = value; } }
        public byte? CollectionDay { get { return (byte?)_CollectionDay; } set { _CollectionDay = value; } }
        public byte? CollectionMonth { get { return (byte?)_CollectionMonth; } set { _CollectionMonth = value; } }
        public short? CollectionYear { get { return (short?)_CollectionYear; } set { _CollectionYear = value; } }

        public string CollectionDateSupplement { get { return _CollectionDateSupplement; } set { _CollectionDateSupplement = value; } }
        
        public string CollectionDateCategory { get { return _CollectionDateCategory; } set { _CollectionDateCategory = value; } }
        public string CollectionTime { get { return _CollectionTime; } set { _CollectionTime = value; } }
        public string CollectionTimeSpan { get { return _CollectionTimeSpan; } set { _CollectionTimeSpan = value; } }
        public string LocalityDescription { get { return _LocalityDescription; } set { _LocalityDescription = value; } }
        public string HabitatDescription { get { return _HabitatDescription; } set { _HabitatDescription = value; } }

        public string ReferenceTitle { get { return _ReferenceTitle; } set { _ReferenceTitle = value; } }
        public string ReferenceURI { get { return _ReferenceURI; } set { _ReferenceURI = value; } }
             
        public string CollectingMethod { get { return _CollectingMethod; } set { _CollectingMethod = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        public string Notes { get { return _Notes; } set { _Notes = value; } }
        public string CountryCache { get { return _CountryCache; } set { _CountryCache = value; } }
        //public string DataWithholdingReason { get { return _DataWithholdingReason; } set { _DataWithholdingReason = value; } }
        //public bool? IsAvailable { get { return _IsAvailable; } set { _IsAvailable = value; } }
        //public int? ExpeditionID { get { return _ExpeditionID; } set { _ExpeditionID = value; } }

        public IDirectAccessIterator<CollectionSpecimen> CollectionSpecimen 
        {
            get { return _collectionSpecimen; } 
        }

        //public IDirectAccessIterator<CollectionEventImage> CollectionEventImage
        //{
        //    get { return _collectionEventImages; }
        //}
        public IDirectAccessIterator<CollectionEventLocalisation> CollectionEventLocalisation
        {
            get { return _collectionEventLocalisations; }
        }
        public IDirectAccessIterator<CollectionEventProperty> CollectionEventProperties
        {
            get { return _collectionEventProperties; }
        }
       

        public CollectionEventSeries CollectionEventSeries
        {
            get { return _CollectionEventSeries; }
        }
        #endregion

        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CollectionEvent: ");
            if (this._CollectorsEventNumber!= null)
            {
                sb.Append(this._CollectorsEventNumber).Append(",");
            }
            if (this._CollectionDate != null)
                sb.Append(this._CollectionDate.ToShortDateString());
            return sb.ToString();
        }

        #endregion
    }
}
 