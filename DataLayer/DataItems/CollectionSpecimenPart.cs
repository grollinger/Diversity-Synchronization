using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class CollectionSpecimenPart:ISerializableObject
    {
        #region Instance Data

        [ID]
        [Column]
        private int _CollectionSpecimenID;

        [Column]
        private int _SpecimenPartID;

        [Column]
        private int? _DerivedFromSpecimenPartID;

        [Column]
        private string _PreparationMethod;

        [Column]
        private DateTime _PreparationDate;

        [Column]
        private string _AccessionNumber;

        [Column]
        private string _PartSublabel;

        [Column]
        private int _CollectionID;

        [Column]
        private string _MaterialCategory;

        [Column]
        private string _StorageLocation;

        [Column]
        private short? _Stock;

        [Column]
        private string _Notes;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [ManyToOne]
        [MappedBy("_collectionSpecimenParts")]
        private CollMaterialCategory_Enum _CollMaterialCategory;

        [ManyToOne]
        [MappedBy("_collectionSpecimenParts")]
        private CollectionSpecimen _CollectionSpecimen;

        [ManyToOne]
        [MappedBy("_collectionSpecimenParts")]
        private Collection _Collection;

        [OneToMany]
        [JoinColums(DefColumn = "_SpecimenPartID", JoinColumn = "_DerivedFromSpecimenPartID")]
        private IDirectAccessIterator<CollectionSpecimenPart> _CollectionSpecimenParts;

        #endregion

        #region Default Constructor

        public CollectionSpecimenPart()
        {
            _SpecimenPartID = 1;
        }

        #endregion

        #region Properties


        public int CollectionSpecimenID { get { return _CollectionSpecimenID; } set { _CollectionSpecimenID = value; } }
        public int SpecimenPartID{get {return _SpecimenPartID;} set{_SpecimenPartID=value;}}
        public int? DerivedFromSpecimenPartID { get { return _DerivedFromSpecimenPartID; } set { _DerivedFromSpecimenPartID = value; } }
        public string PreparationMethod { get { return _PreparationMethod; } set { _PreparationMethod = value; } }
        public DateTime PreparationDate { get { return _PreparationDate; } set { _PreparationDate = value; } }
        public string AccessionNumber { get { return _AccessionNumber; } set { _AccessionNumber = value; } }
        public string PartSublabel { get { return _PartSublabel; } set { _PartSublabel = value; } }
        public int CollectionID { get { return _CollectionID; } set { _CollectionID = value; } }
        public string MaterialCategory { get { return _MaterialCategory; } set { _MaterialCategory = value; } }
        public string StorageLocation { get { return _StorageLocation; } set { _StorageLocation = value; } }
        public short? Stock { get { return _Stock; } set { _Stock = value; } }
        public string Notes { get { return _Notes; } set { _Notes = value; } }

        public CollMaterialCategory_Enum CollMaterialCategory { get { return _CollMaterialCategory; } }
        public CollectionSpecimen CollectionSpecimen { get { return _CollectionSpecimen; } set { _CollectionSpecimen = value; } }
        public Collection Collection { get { return _Collection; } set { _Collection = value; } }
        public IDirectAccessIterator<CollectionSpecimenPart> CollectionSpecimenParts{get{return _CollectionSpecimenParts;}set{_CollectionSpecimenParts=value;}}
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Tabelle wird nur vom Repository aufs Mobilgerät geschrieben.
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        #endregion

        #region ToString override

        public override string ToString()
        {
            return AttributeWorker.ToStringHelper(this, 30);
        }

        #endregion

    }
}
