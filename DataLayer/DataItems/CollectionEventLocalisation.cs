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

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class CollectionEventLocalisation : ISerializableObject
    {
        #region Instance data

        [ID]
        [Column]
        private int? _CollectionEventID;
        [ID]
        [Column]
        private int  _LocalisationSystemID;
        [Column]
        private string _Location1;
        [Column]
        private string _Location2;
        [Column]
        private string _LocationAccuracy;
        [Column]
        private string _LocationNotes;
        [Column] 
        private DateTime _DeterminationDate;
        //[Column]
        //private string _DistanceToLocation;
        //[Column]
        //private string _DirectionToLocation;
        [Column]
        private string _ResponsibleName;
        [Column]
        private string _ResponsibleAgentURI;
        [Column]
        private double? _AverageAltitudeCache;//Double?
        [Column]
        private double? _AverageLatitudeCache;
        [Column]
        private double? _AverageLongitudeCache;
        [Column]
        private DateTime _LogUpdatedWhen;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        //[ManyToOne]
        //[MappedBy("_collectionEventLocalisations")]
        //private CollectionEvent _CollectionEvent;

        //[ManyToOne]
        //[JoinColums(DefColumn = "_LocalisationSystemID", JoinColumn = "_LocalisationSystemID")]
        //private LocalisationSystem _LocalisationSystem;

        #endregion



        #region Default constructor

        public CollectionEventLocalisation()
        {
            this.DeterminationDate = DateTime.Now;
        }

        #endregion

        #region Properties

        public int? CollectionEventID { get { return _CollectionEventID; } set { _CollectionEventID = value; } }
        public int LocalisationSystemID { get { return _LocalisationSystemID; } set { _LocalisationSystemID = value; } }
        public string Location1 { get { return _Location1; } set { _Location1 = value; } }
        public string Location2 { get { return _Location2; } set { _Location2 = value; } }
        public string LocationAccuracy { get { return _LocationAccuracy; } set { _LocationAccuracy = value; } }
        public string LocationNotes { get { return _LocationNotes; } set { _LocationNotes = value; } }
        public DateTime DeterminationDate { get { return _DeterminationDate; } set {
            _DeterminationDate = value;   
        } }
        //public string DistanceToLocation { get { return _DistanceToLocation; } set { _DistanceToLocation = value; } }
        //public string DirectionToLocation { get { return _DirectionToLocation; } set { _DirectionToLocation = value; } }
        public string ResponsibleName { get { return _ResponsibleName; } set { _ResponsibleName = value; } }
        public string ResponsibleAgentURI { get { return _ResponsibleAgentURI; } set { _ResponsibleAgentURI = value; } }
        public double? AverageAltitudeCache { get { return _AverageAltitudeCache; } set { _AverageAltitudeCache = value; } }
        public double? AverageLatitudeCache { get { return _AverageLatitudeCache; } set { _AverageLatitudeCache = value; } }
        public double? AverageLongitudeCache { get { return _AverageLongitudeCache; } set { _AverageLongitudeCache = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        //public CollectionEvent CollectionEvent
        //{
        //    get{return _CollectionEvent;}
        //    set{_CollectionEvent=value;}
        //}
        //public LocalisationSystem LocalisationSystem
        //{
        //    get { return _LocalisationSystem; }
        //    set { _LocalisationSystem = value; }
        //}
        #endregion


        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CollectionEventLocalisation: ");
            if (this._LocalisationSystemID != null)
            {
                sb.Append(this._LocalisationSystemID).Append(",");
            }
            if (this._CollectionEventID != null)
            {
                sb.Append(this._CollectionEventID).Append(",");
            }
            if (this._Location1!= null)
                sb.Append(this._Location1).Append(";");
            if (this._Location2 != null)
                sb.Append(this._Location2);
            return sb.ToString();
        }

        #endregion
    }
}
