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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class AnalysisResult : ISerializableObject
    {
        #region Instance Data

        [ID]
        [Column]
        private int _AnalysisID;
        [ID]
        [Column]
        private String _AnalysisResult;
        [Column]
        private String _Description;
        [Column]
        private String _DisplayText;
        [Column]
        private int _DisplayOrder;
        [Column]
        private string _Notes;

        [Column]
        private DateTime _LogUpdatedWhen;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        //[ManyToOne]
        //[MappedBy("_AnalysisResults")]
        //private Analysis _analysis;

        #endregion

        #region Default constructor

        public AnalysisResult()
        {
        }

        #endregion

        #region Properties

        public int AnalysisID { get { return _AnalysisID; } set { _AnalysisID = value; } }
        public String Analysisresult{ get { return _AnalysisResult; } set { _AnalysisResult = value; } }
        public String Description { get { return _Description; } set { _Description = value; } }
        public String DisplayText { get { return _DisplayText; } set { _DisplayText = value; } }
        public int DisplayOrder { get { return _DisplayOrder; } set { _DisplayOrder = value; } }
        public String Notes { get { return _Notes; } set { _Notes = value; } }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        //public Analysis Analysis { get { return _analysis; } set { _analysis = value; } }
        #endregion

        #region ToString override

        public override string ToString()
        {
            if (this.DisplayText != null)
            {
                String text ="AnalysisResult: "+ this.DisplayText;
                return text;
            }
            else return AttributeWorker.ToStringHelper(this, 30);
        }

        #endregion
    }
}
