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
    public class UserProfile : ISerializableObject
    {
        #region Instance Data

        [ID(Autoinc = true)]
        [Column]
        private int? _UserProfileID;

        [Column]
        private string _LoginName;

        [Column]
        private string _CombinedNameCache;

        [Column]
        private string _AgentURI;

        [Column]
        private bool? _DefaultIUGeographyAnalysis;

        [Column]
        private string _ToolbarIcons;

        [Column]
        private int? _ProjectID;
       
        [Column]
        private string _ProjectName;

        [Column]
        private int? _LastAnalysisID;

        [Column]
        private String _DisplayLevel;

        [Column]
        private string _LanguageContext;

        [Column]
        private string _Context;

        [Column]
        private string _HomeDB;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [Column]
        bool? _stopGPS;
        #endregion

        #region Default constructor

        public UserProfile() {
            this._stopGPS = false;
            this.DefaultIUGeographyAnalysis = false;
            this.ToolbarIcons = "small";
        }

        public UserProfile(String name)
            :this()
        {
            this.CombinedNameCache = name;
        }

        #endregion

        #region Properties

        public int? UserProfileID { get { return _UserProfileID; } set { _UserProfileID = value; } }

        public string CombinedNameCache { get { return _CombinedNameCache; } set { _CombinedNameCache = value; } }
        public string LoginName { get { return _LoginName; } set { _LoginName = value; } }
        public string HomeDB { get { return _HomeDB; } set { _HomeDB = value; } }
        public string AgentURI { get { return _AgentURI; } set { _AgentURI = value; } }

        public bool? StopGPS{get{return _stopGPS;} set {_stopGPS=value;}}
        public bool? DefaultIUGeographyAnalysis { get { return _DefaultIUGeographyAnalysis; } set { _DefaultIUGeographyAnalysis = value; } }
        public string ToolbarIcons { get { return _ToolbarIcons; } set { _ToolbarIcons = value; } }
        public int? ProjectID { get { return _ProjectID; } set { _ProjectID = value; } }
        public string ProjectName { get { return _ProjectName; } set { _ProjectName = value; } }
        public int? LastAnalysisID { get { return _LastAnalysisID; } set { _LastAnalysisID = value; } }
        public DisplayLevel Displaylevel { get { return  new DisplayLevel(_DisplayLevel); } set { _DisplayLevel =value.DisplayLevelString; } }
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Einträge der Tabelle werden nicht synchronisiert. Sie werden nur benötigt um die Schnittstelle zu implementieren.

        public string LanguageContext { get { return _LanguageContext; } set { _LanguageContext = value; } }
        public string Context { get { return _Context; } set { _Context = value; } }
        
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        #endregion

        #region ToString override

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("UserProfile: ");
            if (this.LoginName != null)
            {
                sb.Append(this.LoginName).Append(",");
            }
            if (this._CombinedNameCache != null)
            {
                sb.Append(this._CombinedNameCache).Append(",");
            }
            if (this._ProjectName!= null)
                sb.Append(this._ProjectName).Append(",");
            if (this.Context != null)
            {
                sb.Append(this.Context);
            }
            return sb.ToString();
        }

        #endregion
    }
}
