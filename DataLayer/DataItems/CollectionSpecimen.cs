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

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    
    public class CollectionSpecimen : ISerializableObject
    {
        #region Instance data

        [ID(Autoinc = true)]
        [Column]
        private int         _CollectionSpecimenID;
        [Column]
        private int?        _Version;
        [Column]
        private int?        _CollectionEventID;
        //[Column]
        //private int?        _CollectionID;
        [Column]
        private string _AccessionNumber;
        //[Column]
        //private DateTime?    _AccessionDate;
        //[Column]
        //private string      _AccessionDateSupplement;
        //[Column]
        //private string      _AccessionDateCategory;
        [Column]
        private string _DepositorsName;
        [Column]
        private string _DepositorsAgentURI;
        [Column]
        private string _DepositorsAccessionNumber;
        [Column]
        private string _LabelTitle;
        [Column]
        private string _LabelType;
        //[Column]
        //private string      _LabelTranscriptionState;
        //[Column]
        //private string      _LabelTranscriptionNotes;
        [Column]
        private string _ExsiccataURI;
        [Column]
        private string _ExsiccataAbbreviation;
        [Column]
        private string      _OriginalNotes;
        [Column]
        private string _AdditionalNotes;
        //[Column]
        //private string      _ReferenceTitle;
        //[Column]
        //private string      _ReferenceURI;
        //[Column]
        //private string      _Problems;
        [Column]
        private string      _DataWithholdingReason;

        //[ColumnNew(Mapping = "xx_IsAvailable")]
        //private bool?       _IsAvailable;
        [Column]
        private DateTime _LogUpdatedWhen;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [ManyToOne]
        [MappedBy("_collectionSpecimen")]
        private CollectionEvent _collectionEvent;
        
        [OneToMany(DeleteType = DeleteTypes.CASCADE)] //Workaround:Muss so modelliert werden, da zuerst das Specimen gebildet werden muss (Modellierung DiversityCollection)
        [JoinColums(DefColumn = "_CollectionSpecimenID", JoinColumn = "_CollectionSpecimenID")]
        private IDirectAccessIterator<CollectionProject> _collectionProject;

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        [JoinColums(DefColumn = "_CollectionSpecimenID", JoinColumn = "_CollectionSpecimenID")]
        private IDirectAccessIterator<CollectionAgent> _collectionAgents;

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        [JoinColums(DefColumn = "_CollectionSpecimenID", JoinColumn = "_CollectionSpecimenID")]
        private IDirectAccessIterator<IdentificationUnit> _IdentificationUnits;

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        [JoinColums(DefColumn = "_CollectionSpecimenID", JoinColumn = "_CollectionSpecimenID")]
        private IDirectAccessIterator<CollectionSpecimenImage> _collectionSpecimenImages;

        #endregion

        #region Default constructor

        public CollectionSpecimen() 
        {
            this.Version        = 1;
            //this.AccessionDate  = DateTime.Today; Accessiondate auf Wunsch von Markus entfernt
            //this.AccessionDateCategory = "actual";
            //this.IsAvailable = false; //in der datenbank eigentlich in diesem fall nullable (db fehler?)
        }

        #endregion

        #region Properties

        public int CollectionSpecimenID { get { return _CollectionSpecimenID; } set { this._CollectionSpecimenID = value; } }
        public int? Version { get { return _Version; } set { _Version = value; } }
        public int? CollectionEventID { get { return _CollectionEventID; } set { _CollectionEventID = value; } }
        //public int? CollectionID { get { return _CollectionID; } set { _CollectionID = value; } }
        public string AccessionNumber { get { return _AccessionNumber; } set { _AccessionNumber = value; } }
        //public DateTime? AccessionDate { get { return _AccessionDate; } set { _AccessionDate = value; } }
        //public byte? AccessionDay 
        //{ 
        //    get 
        //    {
        //        if (_AccessionDate.HasValue)
        //        {
        //            return (byte?)_AccessionDate.Value.Day;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    } 
        //    set 
        //    {
        //        throw new InvalidOperationException();
        //    } 
        //}
        //public byte? AccessionMonth 
        //{ 
        //    get 
        //    {
        //        if (_AccessionDate.HasValue)
        //        {
        //            return (byte?)_AccessionDate.Value.Month;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    } 
        //    set
        //    {
        //        throw new InvalidOperationException();
        //    } 
        //}
        //public short? AccessionYear 
        //{ 
        //    get 
        //    {
        //        if (_AccessionDate.HasValue)
        //        {
        //            return (short?)_AccessionDate.Value.Year;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    } 
        //    set 
        //    {
        //        throw new InvalidOperationException();
        //    } 
        //}
        //public string AccessionDateSupplement { get { return _AccessionDateSupplement; } set { _AccessionDateSupplement = value; } }
        //public string AccessionDateCategory { get { return _AccessionDateCategory; } set { _AccessionDateCategory = value; } }
        public string DepositorsName { get { return _DepositorsName; } set { _DepositorsName = value; } }
        public string DepositorsAgentURI { get { return _DepositorsAgentURI; } set { _DepositorsAgentURI = value; } }
        public string DepositorsAccessionNumber { get { return _DepositorsAccessionNumber; } set { _DepositorsAccessionNumber = value; } }
        public string LabelTitle { get { return _LabelTitle; } set { _LabelTitle = value; } }
        public string LabelType { get { return _LabelType; } set { _LabelType = value; } }
        //public string LabelTranscriptionState { get { return _LabelTranscriptionState; } set { _LabelTranscriptionState = value; } }
        //public string LabelTranscriptionNotes { get { return _LabelTranscriptionNotes; } set { _LabelTranscriptionNotes = value; } }
        public string ExsiccataURI { get { return _ExsiccataURI; } set { _ExsiccataURI = value; } }
        public string ExsiccataAbbreviation { get { return _ExsiccataAbbreviation; } set { _ExsiccataAbbreviation = value; } }
        public string OriginalNotes { get { return _OriginalNotes; } set { _OriginalNotes = value; } }
        public string AdditionalNotes { get { return _AdditionalNotes; } set { _AdditionalNotes = value; } }
        //public string ReferenceTitle { get { return _ReferenceTitle; } set { _ReferenceTitle = value; } }
        //public string ReferenceURI { get { return _ReferenceURI; } set { _ReferenceURI = value; } }
        //public string Problems { get { return _Problems; } set { _Problems = value; } }
        public string DataWithholdingReason { get { return _DataWithholdingReason; } set { _DataWithholdingReason = value; } }
        //public bool? IsAvailable { get { return _IsAvailable; } set { _IsAvailable = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        public CollectionEvent CollectionEvent { get { return _collectionEvent; } set { _collectionEvent = value; } }
        //public CollLabelTranscriptionState_Enum CollLabelTransiptionState_Enum { get { return _collLabelTranscriptionState_Enum; } set { _collLabelTranscriptionState_Enum = value; } }
        //public CollLabelType_Enum CollLabelType_Enum { get { return _collLabelType_enum; } set { _collLabelType_enum = value; } }
        public IDirectAccessIterator<CollectionProject> CollectionProject { get { return _collectionProject; } set { _collectionProject = value; } }
        public IDirectAccessIterator<CollectionAgent> CollectionAgent//Bei gelegenheit eliminieren, ist nur in der Synchronisation drin
        {
            get { return _collectionAgents; }
            set { _collectionAgents = value; }
        }
        public IDirectAccessIterator<IdentificationUnit> IdentificationUnits
        {
            get { return _IdentificationUnits; }
            set { _IdentificationUnits = value; }
        }
        public IDirectAccessIterator<CollectionSpecimenImage> CollectionSpecimenImage
        {
            get { return _collectionSpecimenImages; }
            set { _collectionSpecimenImages = value; }
        }

        #endregion

        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CollectionSpecimen: ");
            if (this._CollectionSpecimenID != null)
            {
                sb.Append(this._CollectionSpecimenID).Append(",");
            }
            if (this._AccessionNumber != null)
                sb.Append(this._AccessionNumber);
            return sb.ToString();
        }

        #endregion
    }
}
