using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class Collection : ISerializableObject
    {
        #region Instance Data

        [ID]
        [Column]
        private int _CollectionID;

        [Column]
        private int? _CollectionParentID;

        [Column]
        private string _CollectionName;

        [Column]
        private string _CollectionAcronym;

        [Column]
        private string _AdministrativeContactName;

        [Column]
        private string _AdministrativeContactAgentUri;

        [Column]
        private string _Description;

        [Column]
        private string _Location;

        [Column]
        private string _CollectionOwner;

        [Column]
        private short _DisplayOrder;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [OneToMany]
        [JoinColums(DefColumn = "_CollectionID", JoinColumn = "_CollectionID")]
        private IDirectAccessIterator<CollectionSpecimenPart> _collectionSpecimenParts;

        [OneToMany]
        [JoinColums(DefColumn = "_CollectionID", JoinColumn = "_CollectionParentID")]
        private IDirectAccessIterator<Collection> _collections;

        #endregion

        #region Default Constructor

        public Collection()
        {
        }

        #endregion

        #region Properties

        public int CollectionID{get{return _CollectionID;}set{_CollectionID=value;}}
        public int? CollectionParentID{get{return _CollectionParentID;}set{_CollectionParentID=value;}}
        public string CollectionName{get{return _CollectionName;} set{_CollectionName=value;}}
        public string CollectionAcronym{get{return _CollectionAcronym;}set{_CollectionAcronym=value;}}
        public string AdministrativeContactName{get{return _AdministrativeContactName;}set{_AdministrativeContactName=value;}}
        public string AdministrativeContactAgentUri{get{return _AdministrativeContactAgentUri;} set{_AdministrativeContactAgentUri=value;}}
        public string Description{get{return _Description;}set{_Description=value;}}
        public string Location { get { return _Location; } set { _Location = value; } }
        public string CollectionOwner { get { return _CollectionOwner; } set { _CollectionOwner = value; } }
        public short DisplayOrder { get { return _DisplayOrder; } set { _DisplayOrder = value; } }

        public IDirectAccessIterator<CollectionSpecimenPart> CollectionSpecimenParts { get { return _collectionSpecimenParts; } set { _collectionSpecimenParts = value; } }
        public IDirectAccessIterator<Collection> Collevtions { get { return _collections; } set { _collections = value; } }
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
