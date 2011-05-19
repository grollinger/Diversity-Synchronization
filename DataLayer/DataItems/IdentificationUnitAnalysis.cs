using System;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
using UBT.AI4.Bio.DivMobi.DataLayer.SyncAttributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class IdentificationUnitAnalysis : ISerializableObject
    {
        #region Instance data
        [ID]
        [Column]
        private int _CollectionSpecimenID;
        [ID]
        [Column]
        private int _IdentificationUnitID;
        [ID]
        [Column]
        //[DirectSync]
        private int _AnalysisID;
        [ID] //Eindeutigkeit garantieren!
        [Column]
        private string  _AnalysisNumber;
        [Column]
        private string _AnalysisResult;
        //[Column]
        //private string _ExternalAnalysisURI;
        [Column]
        private string _ResponsibleName;
        [Column]
        private string _ResponsibleAgentURI;
        //[Column]
        //private string  _Notes;
        [Column]
        private string _AnalysisDate;

        [Column]
        private DateTime _LogUpdatedWhen;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [ManyToOne]
        [JoinColums(DefColumn = "_AnalysisID", JoinColumn = "_AnalysisID")]
        private Analysis _Analysis;

        [ManyToOne]
        [MappedBy("_IdentificationUnitAnalysis")]
        private IdentificationUnit _IdentificationUnit;


        #endregion

        #region Default constructor

        public IdentificationUnitAnalysis()
        {
            this._AnalysisDate = DateTime.Today.ToLongDateString();
        }

        #endregion

        #region Properties

        public int CollectionSpecimenID { get { return _CollectionSpecimenID; } set { _CollectionSpecimenID = value; } }
        public int IdentificationUnitID { get { return _IdentificationUnitID; } set { _IdentificationUnitID = value; } }
        public int AnalysisID { get { return _AnalysisID; } set { _AnalysisID = value; } }
        public string AnalysisNumber { get { return _AnalysisNumber; } set { _AnalysisNumber = value; } }
        public string AnalysisResult { get { return _AnalysisResult; } set { _AnalysisResult = value; } }
        //public string ExternalAnalysisURI { get { return _ExternalAnalysisURI; } set { _ExternalAnalysisURI = value; } }
        public string ResponsibleName { get { return _ResponsibleName; } set { _ResponsibleName = value; } }
        public string ResponsibleAgentURI { get { return _ResponsibleAgentURI; } set { _ResponsibleAgentURI = value; } }
        //public string Notes { get { return _Notes; } set { _Notes = value; } }
        public string AnalysisDate { get { return _AnalysisDate; } set { _AnalysisDate = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        public Analysis Analysis//im Prinzip eliminierbar
        {
            get { return _Analysis; }
            set { _Analysis = value; }
        }
        public IdentificationUnit IdentificationUnit
        {
            get { return _IdentificationUnit; }
            set { _IdentificationUnit = value; }
        }

        #endregion

        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("IdentificationUnitAnalysis: ");
            if (this._Analysis != null && this._Analysis.DisplayText!=null)
            {
                sb.Append(this._Analysis.DisplayText).Append(",");
            }
            if (this._AnalysisResult != null)
            {
                sb.Append(this._AnalysisResult).Append(",");
            }
            if (this._AnalysisDate != null)
            {
                sb.Append(this._AnalysisDate).Append(",");
            }
            if (this._IdentificationUnitID!= null)
                sb.Append(this._IdentificationUnitID);
            return sb.ToString();
        }

        #endregion
    }
}
