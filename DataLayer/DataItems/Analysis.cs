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
using System.Collections.Generic;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class Analysis : ISerializableObject  
    {
        #region Instance Data
        [ID]
        [Column]
        private int    _AnalysisID;
        [Column]
        private int?    _AnalysisParentID;
        [Column]
        private string  _DisplayText;
        [Column]
        private string  _Description;
        [Column]
        private string  _MeasurementUnit;
        [Column]
        private string  _Notes;
        [Column]
        private string  _AnalysisURI;
        [Column]
        private bool _OnlyHierarchy;

        [Column]
        private DateTime _LogUpdatedWhen;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [OneToMany]
        [JoinColums(DefColumn = "_AnalysisID", JoinColumn = "_AnalysisID")]
        private IDirectAccessIterator<AnalysisTaxonomicGroup> _analysisTaxonomicGroups;

        //[OneToMany]
        //[JoinColums(DefColumn = "_AnalysisID", JoinColumn = "_AnalysisID")]
        //private IDirectAccessIterator<IdentificationUnitAnalysis> _IdentificationUnitAnalysis;

        //[OneToMany]
        //[JoinColums(DefColumn = "_AnalysisID", JoinColumn = "_AnalysisParentID")]
        //private IDirectAccessIterator<Analysis> _Analysis;

        //[OneToMany]
        //[JoinColums(DefColumn = "_AnalysisID", JoinColumn = "_AnalysisID")]
        //private IDirectAccessIterator<AnalysisResult> _AnalysisResults;

        //[ManyToOne]
        //[MappedBy("_Analysis")]
        //private Analysis _analysisParent;

        #endregion

        #region Default constructor

        public Analysis()
        {
            this.AnalysisParentID = null;
        }

        #endregion

        #region Properties

        public int AnalysisID { get { return _AnalysisID; } set { _AnalysisID = value; } }
        public int? AnalysisParentID { get { return _AnalysisParentID; } set { _AnalysisParentID = value; } }
        public string DisplayText { get { return _DisplayText; } set { _DisplayText = value; } }
        public string Description { get { return _Description; } set { _Description = value; } }
        public string MeasurementUnit { get { return _MeasurementUnit; } set { _MeasurementUnit = value; } }
        public string Notes { get { return _Notes; } set { _Notes = value; } }
        public string AnalysisURI { get { return _AnalysisURI; } set { _AnalysisURI = value; } }
        public bool OnlyHierarchy { get { return _OnlyHierarchy; } set { _OnlyHierarchy = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        //public IDirectAccessIterator<AnalysisTaxonomicGroup> AnalysisTaxonomicGroup
        //{
        //    get { return _analysisTaxonomicGroups; }
        //}
        //public IDirectAccessIterator<Analysis> AnalysisChildren
        //{
        //    get { return _Analysis; }
        //}
        //public IDirectAccessIterator<AnalysisResult> AnalysisResults
        //{
        //    get { return _AnalysisResults; }
        //}
        //public IDirectAccessIterator<IdentificationUnitAnalysis> IdentificationUnitAnalysis
        //{
        //    get { return _IdentificationUnitAnalysis; }

        //}
        //public Analysis AnalysisParent 
        //{
        //    get { return _analysisParent; }
        //    set { _analysisParent = value; } 
        //}

        #endregion

        #region ToString override

        public override string ToString()
        {
            if (this.DisplayText != null)
            {
                String text ="Analysis: "+ this.DisplayText;
                return text;
            }
            else return AttributeWorker.ToStringHelper(this, 30);
        }

        #endregion
    }
}
