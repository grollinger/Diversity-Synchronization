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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class IdentificationUnitGeoAnalysis : ISerializableObject
    {
        #region Instance data
        [ID]
        [Column]
        private int _CollectionSpecimenID; 
        [ID]
        [Column]
        private int _IdentificationUnitID;
        
        
        [Column]
        private DateTime _AnalysisDate;

        [Column]
        private String _Geography;

        [Column]
        private String _ResponsibleName;
        [Column]
        private String _ResponsibleAgentURI;
        //[Column]
        //private String _Notes;

        //[Column]
        //private double? _GeoLongitude;

        //[Column]
        //private double? _GeoLatitude;

        //[Column]
        //private double? _GeoAltitude;

        [Column]
        private DateTime _LogUpdatedWhen;

        [ID] 
        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        [ManyToOne]
        [MappedBy("_IdentificationUnitGeoAnalysis")]
        private IdentificationUnit _IdentificationUnit;

        #endregion

        #region Default constructor

        public IdentificationUnitGeoAnalysis()
        {
            this._AnalysisDate = DateTime.Now;
            //this._GeoLongitude = 0.0;
            //this._GeoLatitude = 0.0;
            //this._GeoAltitude = 0.0;
            this._Geography = this.serializeGeography(0, 0, 0);
        }

        public IdentificationUnitGeoAnalysis(double? lat,double? lon,double? alt)
        {
            this._AnalysisDate = DateTime.Now;
            this._Geography = this.serializeGeography(lat, lon, alt);
        }
        public void setGeography(double? lat,double? lon,double? alt)
        {
            this._Geography = this.serializeGeography(lat, lon, alt);
        }
        #endregion

        #region Properties

        public int CollectionSpecimenID { get { return _CollectionSpecimenID; } set { _CollectionSpecimenID = value; } }
        public int IdentificationUnitID { get { return _IdentificationUnitID; } set { _IdentificationUnitID = value; } }
        public DateTime AnalysisDate { get { return _AnalysisDate; } set { _AnalysisDate = value; } }
        public String Geography { 
            get {
                /* create "point_tagged_text" as WKT to use STPointFromText ( 'point_tagged_text' , SRID ) 
                while synchronisation 
                
                point_tagged_text - Is the WKT representation of the geography 
                Point instance you wish to return. point_tagged_text is an nvarchar(max) expression.
                    
                SRID - Is an int expression representing the spatial reference ID (SRID) of the 
                geography Point instance you wish to return.*/
                return this._Geography;
                }
        }
        public String ResponsibleName { get { return _ResponsibleName; } set { _ResponsibleName = value; } }
        public String ResponsibleAgentURI { get { return _ResponsibleAgentURI; } set { _ResponsibleAgentURI = value; } }
        //public String Notes { get { return _Notes; } set { _Notes = value; } }
        public double? GeoLongitude
        {
            get
            {
                String geography = this._Geography;
                String[] geo=geography.Split(' ');
                geography = geo[1];
                geography.Trim();
                geography = geography.Replace('.', ',');//Aufgrund der Culture Settings die in CF nicht geändert werden können . muss die Zahl mit Dezimalkomma formatiert sein.
                try
                {
                    return Double.Parse(geography, System.Globalization.NumberStyles.AllowDecimalPoint);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
                                    
        }

        public double? GeoLatitude
        {
            get
            {
                try
                {
                    String geography = this._Geography;
                    String[] geo = geography.Split(' ');
                    geography = geo[0];
                    geo = geography.Split('(');
                    geography = geo[2];
                    geography.Replace("(", " ");
                    geography.Trim();
                    geography = geography.Replace('.', ',');//Aufgrund der Culture Settings die in CF nicht geändert werden können . muss die Zahl mit Dezimalkomma formatiert sein.
                    return Double.Parse(geography, System.Globalization.NumberStyles.AllowDecimalPoint);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        public double? GeoAltitude
        {
            get
            {
                try
                {
                    String geography = this._Geography;
                    String[] geo = geography.Split(' ');
                    geography = geo[2];
                    geography.Replace(")', 4326)", " ");
                    geography.Trim();
                    geography = geography.Replace('.', ',');//Aufgrund der Culture Settings die in CF nicht geändert werden können . muss die Zahl mit Dezimalkomma formatiert sein.
                    return Double.Parse(geography, System.Globalization.NumberStyles.AllowDecimalPoint);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
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
            StringBuilder sb = new StringBuilder("IdentificationUnitGeoAnalysis: ");

            if (this.Geography != null)
            {
                sb.Append(this.Geography).Append(",");
            }
            if (this._AnalysisDate != null)
            {
                sb.Append(this._AnalysisDate).Append(",");
            }
            if (this._IdentificationUnitID != null)
                sb.Append(this._IdentificationUnitID);
            return sb.ToString();
        }

        #endregion
        public String serializeGeography(double? latitude,double? longitude, double? altitude)
        {
            String longitudeStr = longitude.ToString();
            longitudeStr=longitudeStr.Replace(',', '.');
            String latStr = latitude.ToString();
            latStr= latStr.Replace(',', '.');
            String altStr = altitude.ToString();
            altStr= altStr.Replace(',', '.');
            StringBuilder builder = new StringBuilder("geography::STGeomFromText('POINT(");
            builder.Append(latStr);
            builder.Append(" ");
            builder.Append(longitudeStr);
            builder.Append(" ");
            builder.Append(altStr);
            builder.Append(")', 4326)");
            String s= builder.ToString();
            return builder.ToString();
        }
    }
}
