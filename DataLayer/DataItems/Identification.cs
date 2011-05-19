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
    public class Identification : ISerializableObject
    {
        #region Instance Data
        [ID]
        [Column]
        private int? _CollectionSpecimenID;
        [ID]
        [Column]
        private int? _IdentificationUnitID;
        [ID]
        [Column]
        private short? _IdentificationSequence;
        [Column]
        private DateTime _IdentificationDate;
        [Column]
        private byte? _IdentificationDay;
        [Column]
        private byte? _IdentificationMonth;
        [Column]
        private short? _IdentificationYear;
        //[Column]
        //private string _IdentificationDateSupplement;
        [Column]
        private string _IdentificationDateCategory;
        [Column]
        private string _VernacularTerm;
        [Column]
        private string _TaxonomicName;
        [Column]
        private string _NameURI;
        [Column]
        private string _IdentificationCategory;
        [Column]
        private string _IdentificationQualifier;
        //[Column]
        //private string _TypeStatus;
        //[Column]
        //private string _TypeNotes;
        //[Column]
        //private string _ReferenceTitle;
        //[Column]
        //private string _ReferenceURI;
        //[Column]
        //private string _Notes;
        [Column]
        private string _ResponsibleName;
        [Column]
        private string _ResponsibleAgentURI;
        [Column]
        private DateTime _LogUpdatedWhen;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        //[ManyToOne] //Wieso wird diese Constraint benötigt? Ist das nicht OneToMany?
        //[MappedBy("_Identifications")]
        //private IdentificationUnit _IdentificationUnit;

        //[ManyToOne]
        //[MappedBy("_Identifications")]
        //private CollIdentificationCategory_Enum _CollIdentificationCategory_Enum;

        //[ManyToOne]
        //[MappedBy("_Identifications")]
        //private CollIdentificationQualifier_Enum _CollIdentificationQualifier_Enum;

        //[ManyToOne]
        //[MappedBy("_identifications")]
        //private CollTypeStatus_Enum _CollTypeStatus_Enum;

        #endregion
  

        #region Default constructor

        public Identification()
        {
            this._IdentificationDate = DateTime.Today;
            this._IdentificationDay = (byte)DateTime.Today.Day;
            this._IdentificationMonth = (byte)DateTime.Today.Month;
            this._IdentificationYear = (short)DateTime.Today.Year;
            this._IdentificationDateCategory = "actual";
            this._IdentificationCategory = "determination";
            this._IdentificationSequence = 1;
        }

        #endregion



        #region Properties

        public int? CollectionSpecimenID { get { return _CollectionSpecimenID; } set { _CollectionSpecimenID = value; } }
        public int? IdentificationUnitID { get { return _IdentificationUnitID; } set { _IdentificationUnitID = value; } }
        public short? IdentificationSequence { get { return _IdentificationSequence; } set { _IdentificationSequence = value; } }
        public DateTime IdentificationDate { get { return _IdentificationDate; } set { _IdentificationDate = value; } }
        public byte? IdentificationDay { get { return (byte?)_IdentificationDay; } set { _IdentificationDay = value; } }
        public byte? IdentificationMonth { get { return (byte?)_IdentificationMonth; } set { _IdentificationMonth = value; } }
        public short? IdentificationYear { get { return (short?)_IdentificationYear; } set { _IdentificationYear = value; } }
        //public string IdentificationDateSupplement { get { return _IdentificationDateSupplement; } set { _IdentificationDateSupplement = value; } }
        public string IdentificationDateCategory { get { return _IdentificationDateCategory; } set { _IdentificationDateCategory = value; } }
        public string VernacularTerm { get { return _VernacularTerm; } set { _VernacularTerm = value; } }
        public string TaxonomicName { get { return _TaxonomicName; } set { _TaxonomicName = value; } }
        public string NameURI { get { return _NameURI; } set { _NameURI = value; } }
        public string IdentificationCategory { get { return _IdentificationCategory; } set { _IdentificationCategory = value; } }
        public string IdentificationQualifier { get { return _IdentificationQualifier; } set { _IdentificationQualifier = value; } }
        //public string TypeStatus { get { return _TypeStatus; } set { _TypeStatus = value; } }
        //public string TypeNotes { get { return _TypeNotes; } set { _TypeNotes = value; } }
        //public string ReferenceTitle { get { return _ReferenceTitle; } set { _ReferenceTitle = value; } }
        //public string ReferenceURI { get { return _ReferenceURI; } set { _ReferenceURI = value; } }
        //public string Notes { get { return _Notes; } set { _Notes = value; } }
        public string ResponsibleName { get { return _ResponsibleName; } set { _ResponsibleName = value; } }
        public string ResponsibleAgentURI { get { return _ResponsibleAgentURI; } set { _ResponsibleAgentURI = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        //public IdentificationUnit IdentificationUnit
        //{
        //    get { return _IdentificationUnit; }
        //    set { _IdentificationUnit = value; }
        //}
        #endregion

        //public CollIdentificationCategory_Enum CollIdentificationCategory_Enum
        //{
        //    get { return _CollIdentificationCategory_Enum; }
        //    set { _CollIdentificationCategory_Enum = value; }
        //}

        //public CollIdentificationQualifier_Enum CollIdentificationQualifier_Enum
        //{
        //    get { return _CollIdentificationQualifier_Enum; }
        //    set { _CollIdentificationQualifier_Enum = value; }
        //}

        //public CollTypeStatus_Enum CollTypeStatus_Enum
        //{
        //    get { return _CollTypeStatus_Enum; }
        //    set { _CollTypeStatus_Enum = value; }
        //}


        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Identificationt: ");
            if (this._TaxonomicName != null)
            {
                sb.Append(this._TaxonomicName).Append(",");
            }
            if (this._IdentificationDate != null)
                sb.Append(this._IdentificationDate.ToShortDateString());
            if (this._CollectionSpecimenID != null)
            {
                sb.Append(this._CollectionSpecimenID);
            }
            return sb.ToString();
        }

        #endregion
   }
}
