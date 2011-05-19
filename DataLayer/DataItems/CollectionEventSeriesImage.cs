using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class CollectionEventSeriesImage : ISerializableObject
    {
        #region Instance Data
        [ID]
        [Column]
        private int _SeriesID;
        [ID]
        [Column]
        private string _URI;
        [Column]
        private string _ResourceURI;
        [Column]
        private string _ImageType;
        [Column]
        private string _Notes;
        [Column]
        private string _DataWithholdingReason;
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [ManyToOne]
        [MappedBy("_CollectionEventSeriesImages")]
        private CollectionEventSeries _CollectionEventSeries;

        [ManyToOne]
        [MappedBy("_CollectionEventSeriesImages")]
        private CollEventSeriesImageType_Enum _CollEventSeriesImageType;

        #endregion

        
        #region Default constructor

        public CollectionEventSeriesImage()
        {

        }

        #endregion

        #region Properties

        public int SeriesID { get { return _SeriesID; } set { _SeriesID = value; } }
        public string URI { get { return _URI; } set { _URI = value; } }
        public string ResourceURI { get { return _ResourceURI; } set { _ResourceURI = value; } }
        public string ImageType { get { return _ImageType; } set { _ImageType = value; } }
        public string Notes { get { return _Notes; } set { _Notes = value; } }
        public string DataWithholdingReason { get { return _DataWithholdingReason; } set { _DataWithholdingReason = value; } }

        public CollectionEventSeries CollectionEventSeries
        {
            get { return _CollectionEventSeries; }
            set { _CollectionEventSeries = value; }
        }
        public CollEventSeriesImageType_Enum CollEventSeriesImageType_Enum
        {
            get { return _CollEventSeriesImageType; }
            set { _CollEventSeriesImageType = value; }
        }
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
