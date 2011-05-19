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
    public class IdentificationUnit : ISerializableObject
    {
        #region Instance Data

        [ID]
        [Column]
        private int _CollectionSpecimenID;
        [ID(Autoinc = true)]
        [Column]
        private int _IdentificationUnitID;
        [Column]
        private string _LastIdentificationCache;
        [Column]
        private string _FamilyCache;
        [Column]
        private string _OrderCache;
        [Column]
        private string _TaxonomicGroup;
        [Column]
        private bool? _OnlyObserved;
        [Column]
        private int? _RelatedUnitID;
        [Column]
        private string _RelationType;
        [Column]
        private string _ColonisedSubstratePart;
        [Column]
        private string _LifeStage;
        [Column]
        private string _Gender;
        [Column]
        private short? _NumberOfUnits;
        //[Column]
        //private string _ExsiccataNumber;
        //[Column]
        //private short? _ExsiccataIdentification;
        [Column]
        private string _UnitIdentifier;
        [Column]
        private string _UnitDescription;
        [Column]
        private string _Circumstances;
        [Column]
        private short? _DisplayOrder;
        [Column]
        private string _Notes;
        [Column]
        private DateTime _LogUpdatedWhen;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        #region Navigation

        [ManyToOne]
        [MappedBy("_IdentificationUnits")]
        private CollectionSpecimen _CollectionSpecimen;

        //[ManyToOne]
        //[JoinColums(DefColumn="_Code",  JoinColumn="_TaxonomicGroup")]
        //private CollTaxonomicGroup_Enum _CollTaxonomicGroup_Enum;

        [ManyToOne]
        [MappedBy("_ChildUnits")]
        private IdentificationUnit _RelatedUnit;

        //[ManyToOne]
        //[MappedBy("_IdentificationUnits")]
        //private CollUnitRelationType_Enum _CollUnitRelationType_Enum;

        //[ManyToOne]
        //[MappedBy("_IdentificationUnits")]
        //private CollCircumstances_Enum _CollCircumstances_Enum;

        //[OneToMany]
        //[JoinColums(DefColumn = "_CollectionSpecimenID", JoinColumn = "_CollectionSpecimenID")]
        //[JoinColums(DefColumn = "_IdentificationUnitID", JoinColumn = "_IdentificationUnitID")]
        //private IDirectAccessIterator<CollectionSpecimenImage> _CollectionSpecimenImages;//

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        //[JoinColums(DefColumn = "_CollectionSpecimenID", JoinColumn = "_CollectionSpecimenID")]
        [JoinColums(DefColumn = "_IdentificationUnitID", JoinColumn = "_IdentificationUnitID")]
        private IDirectAccessIterator<Identification> _Identifications;

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        //[JoinColums(DefColumn = "_CollectionSpecimenID", JoinColumn = "_CollectionSpecimenID")]
        [JoinColums(DefColumn = "_IdentificationUnitID", JoinColumn = "_IdentificationUnitID")]
        private IDirectAccessIterator<IdentificationUnitAnalysis> _IdentificationUnitAnalysis;

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        //[JoinColums(DefColumn = "_CollectionSpecimenID", JoinColumn = "_CollectionSpecimenID")]
        [JoinColums(DefColumn = "_IdentificationUnitID", JoinColumn = "_IdentificationUnitID")]
        private IDirectAccessIterator<IdentificationUnitGeoAnalysis> _IdentificationUnitGeoAnalysis;

        [OneToMany(DeleteType = DeleteTypes.CASCADE)]
        [JoinColums(DefColumn = "_IdentificationUnitID", JoinColumn = "_RelatedUnitID")]
        private IDirectAccessIterator<IdentificationUnit> _ChildUnits;

        #endregion

        #endregion

        #region Default constructor

        public IdentificationUnit()
        {
            this.LastIdentificationCache    = String.Empty;
            this.OnlyObserved               = true;
            this.TaxonomicGroup             = @"unknown";
            // Default value: 
            this.DisplayOrder = 0;
        }

        #endregion

        #region Properties

        public int CollectionSpecimenID { get { return _CollectionSpecimenID; } set { _CollectionSpecimenID = value; } }
        public int IdentificationUnitID { get { return _IdentificationUnitID; } set { _IdentificationUnitID = value; } }
        public string LastIdentificationCache { get { return _LastIdentificationCache; } set { _LastIdentificationCache = value; } }
        public string FamilyCache { get { return _FamilyCache; } set { _FamilyCache = value; } }
        public string OrderCache { get { return _OrderCache; } set { _OrderCache = value; } }
        public string TaxonomicGroup { get { return _TaxonomicGroup; } set { _TaxonomicGroup = value; } }
        public bool? OnlyObserved { get { return _OnlyObserved; } set { _OnlyObserved = value; } }
        public int? RelatedUnitID { get { return _RelatedUnitID; } set { _RelatedUnitID = value; } }
        public string RelationType { get { return _RelationType; } set { _RelationType = value; } }
        public string ColonisedSubstratePart { get { return _ColonisedSubstratePart; } set { _ColonisedSubstratePart = value; } }
        public string LifeStage { get { return _LifeStage; } set { _LifeStage = value; } }
        public string Gender { get { return _Gender; } set { _Gender	 = value; } }
        public short? NumberOfUnits { get { return _NumberOfUnits; } set { _NumberOfUnits = value; } }
        //public string ExsiccataNumber { get { return _ExsiccataNumber; } set { _ExsiccataNumber	 = value; } }
        //public short? ExsiccataIdentification { get { return _ExsiccataIdentification	; } set { _ExsiccataIdentification = value; } }
        public string UnitIdentifier { get { return _UnitIdentifier; } set { _UnitIdentifier = value; } }
        public string UnitDescription { get { return _UnitDescription; } set { _UnitDescription = value; } }
        public string Circumstances { get { return _Circumstances; } set { _Circumstances = value; } }
        public short? DisplayOrder { get { return _DisplayOrder; } set { _DisplayOrder	= value; } }
        public string Notes { get { return _Notes; } set { _Notes = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        public CollectionSpecimen CollectionSpecimen 
        {
            get { return _CollectionSpecimen; } 
            set { _CollectionSpecimen = value; }
        }
        //public CollTaxonomicGroup_Enum CollTaxonomicGroup_Enum
        //{
        //    get { return _CollTaxonomicGroup_Enum; }
        //}
        public IdentificationUnit RelatedUnit
        {
            get { return _RelatedUnit; }
        }
        //public CollUnitRelationType_Enum CollUnitRelationType_Enum
        //{
        //    get { return _CollUnitRelationType_Enum; }
        //}
        //public CollCircumstances_Enum CollCircumstances_Enum
        //{
        //    get { return _CollCircumstances_Enum; }
        //}
        //public IDirectAccessIterator<CollectionSpecimenImage> CollectionSpecimenImage
        //{
        //    get { return _CollectionSpecimenImages; }
        //}
        public IDirectAccessIterator<Identification> Identifications
        {
            get { return _Identifications; }
        }
        public IDirectAccessIterator<IdentificationUnitAnalysis> IdentificationUnitAnalysis
        {
            get { return _IdentificationUnitAnalysis; }
        }
        public IDirectAccessIterator<IdentificationUnitGeoAnalysis> IdentificationUnitGeoAnalysis
        {
            get { return _IdentificationUnitGeoAnalysis; }
        }
        public IDirectAccessIterator<IdentificationUnit> ChildUnits
        {
            get { return _ChildUnits;  }
        }
        #endregion

        #region ToString override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("IdentificationUnit: ");
            if (this.UnitIdentifier != null)
            {
                sb.Append("(").Append(this.UnitIdentifier).Append(") ");
            }
            if (this.LastIdentificationCache != null)
            {
                sb.Append(this.LastIdentificationCache).Append(",");
            }
            if (this.UnitDescription != null)
            {
                sb.Append(this.UnitDescription).Append(",");
            }
            if (this._IdentificationUnitID != null)
            {
                sb.Append(this._IdentificationUnitID).Append(",");
            }
            if (this.TaxonomicGroup != null)
                sb.Append(this.TaxonomicGroup);
            return sb.ToString();
        }

        #endregion
    }
}
