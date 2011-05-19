using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{

    public class CollLabelTranscriptionState_Enum :ISerializableObject
    {
       
        #region Instance Data
        [ID]
        [Column]
        private string _Code;

        [Column]
        private string _Description;

        [Column]
        private string _DisplayText;

        [Column]
        private short _DisplayOrder;

        [Column]
        private bool _DisplayEnable;

        [Column]
        private string _InternalNotes;

        [Column]
        private string _ParentCode;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [OneToMany]
        [JoinColums(DefColumn = "_Code", JoinColumn = "_LabelTranscriptionState")]
        private IDirectAccessIterator<CollectionSpecimen> _collectionSpecimen;

        #endregion

        #region Default Constructor

        public CollLabelTranscriptionState_Enum()
        {
        }

        #endregion

        #region Properties

        public string Code { get { return _Code; } set { _Code = value; } }
        public string Description { get { return _Description; } set { _Description = value; } }
        public string DisplayText { get { return _DisplayText; } set { _DisplayText = value; } }
        public short DisplayOrder { get { return _DisplayOrder; } set { _DisplayOrder = value; } }
        public bool DisplayEnable { get { return _DisplayEnable; } set { _DisplayEnable = value; } }
        public string InternalNotes { get { return _InternalNotes; } set { _InternalNotes = value; } }
        public string ParentCode { get { return _ParentCode; } set { _ParentCode = value; } }

        public IDirectAccessIterator<CollectionSpecimen> CollectionSpecimen
        {
            get { return _collectionSpecimen; }
        }
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Tabelle wird nur vom Repository aufs Mobilgerät geschrieben.
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        #endregion

        #region ToString Override

        public override string ToString()
        {
            return AttributeWorker.ToStringHelper(this, 30);
        }

        #endregion
    }
}
