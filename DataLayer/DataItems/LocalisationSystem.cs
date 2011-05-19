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
using UBT.AI4.Bio.DivMobi.DataLayer.SyncAttributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class LocalisationSystem : ISerializableObject
    {
        #region Instance Data
        [ID]
        [DirectSync]
        [Column]
        private int _LocalisationSystemID;
        [Column]
        private int? _LocalisationSystemParentID;
        [Column]
        private string _LocalisationSystemName;
        [Column]
        private string _DefaultAccuracyOfLocalisation;
        [Column]
        private string _DefaultMeasurementUnit;
        [Column]
        private string _ParsingMethodName;
        [Column]
        private string _DisplayText;
        [Column]
        private bool? _DisplayEnable;
        [Column]
        private short? _DisplayOrder;
        [Column]
        private string _Description;
        [Column]
        private string _DisplayTextLocation1;
        [Column]
        private string _DescriptionLocation1;
        [Column]
        private string _DisplayTextLocation2;
        [Column]
        private string _DescriptionLocation2;

        //[ColumnNew(Mapping = "xx_DiversityModule")]
        //private string _DiversityModule;
        //[ColumnNew(Mapping = "xx_ParsingMethod")]
        //private string _ParsingMethod;
        //[ColumnNew(Mapping = "xx_MeasurementUnit")]
        //private string _MeasurementUnit;
        //[ColumnNew(Mapping="DefaultMeasurementUnit", Target="server")]
        //[ColumnNew(Mapping = "xx_DefaultMeasurementUnit1")]
        //private string  _DefaultMeasurementUnit1;
        

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        //[OneToMany]
        //[JoinColums(DefColumn = "_LocalisationSystemID", JoinColumn = "_LocalisationSystemID")]
        //private IDirectAccessIterator<CollectionEventLocalisation> _CollectionEventLocalisations;

        //[OneToMany]
        //[JoinColums(DefColumn = "_LocalisationSystemID", JoinColumn = "_LocalisationSystemParentID")]
        //private IDirectAccessIterator<LocalisationSystem> _LocalisationSystemChildren;

        //[ManyToOne]
        //[MappedBy("_LocalisationSystemChildren")]
        //private LocalisationSystem _LocalisationSystemParent;

        #endregion


        #region Default constructor

        public LocalisationSystem()
        {
            this.LocalisationSystemName = @"unknown";
        }

        #endregion


        #region Properties

        public int LocalisationSystemID { get { return _LocalisationSystemID; } set { _LocalisationSystemID = value; } }
        public int? LocalisationSystemParentID { get { return _LocalisationSystemParentID; } set { _LocalisationSystemParentID = value; } }
        public string LocalisationSystemName { get { return _LocalisationSystemName; } set { _LocalisationSystemName = value; } }
        public string DefaultAccuracyOfLocalisation { get { return _DefaultAccuracyOfLocalisation; } set { _DefaultAccuracyOfLocalisation = value; } }
        public string DefaultMeasurementUnit { get { return _DefaultMeasurementUnit; } set { _DefaultMeasurementUnit = value; } }
        public string ParsingMethodName { get { return _ParsingMethodName; } set { _ParsingMethodName = value; } }
        public string DisplayText { get { return _DisplayText; } set { _DisplayText = value; } }
        public bool? DisplayEnable { get { return _DisplayEnable; } set { _DisplayEnable = value; } }
        public short? DisplayOrder { get { return _DisplayOrder; } set { _DisplayOrder = value; } }
        public string Description { get { return _Description; } set { _Description = value; } }
        public string DisplayTextLocation1 { get { return _DisplayTextLocation1; } set { _DisplayTextLocation1 = value; } }
        public string DescriptionLocation1 { get { return _DescriptionLocation1; } set { _DescriptionLocation1 = value; } }
        public string DisplayTextLocation2 { get { return _DisplayTextLocation2; } set { _DisplayTextLocation2 = value; } }
        public string DescriptionLocation2 { get { return _DescriptionLocation2; } set { _DescriptionLocation2 = value; } }
        //public string DiversityModule { get { return _DiversityModule; } set { _DiversityModule = value; } }
        //public string ParsingMethod { get { return _ParsingMethod; } set { _ParsingMethod = value; } }
        //public string MeasurementUnit { get { return _MeasurementUnit; } set { _MeasurementUnit = value; } }
        //public string DefaultMeasurementUnit1 { get { return _DefaultMeasurementUnit1; } set { _DefaultMeasurementUnit1 = value; } }
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Tabelle wird nur vom Repository aufs Mobilgerät geschrieben. Etwaige Änderungen sollen bis zum nächsten Clean ignoriert werden.. Deswegen wird defaultmäßig der 1.Januar 1900 zurückgegeben.
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        //public IDirectAccessIterator<CollectionEventLocalisation> CollectionEventLocalisation
        //{
        //    get { return _CollectionEventLocalisations; }
        //}
        //public IDirectAccessIterator<LocalisationSystem> LocalisationSystemChildren
        //{
        //    get { return _LocalisationSystemChildren; }
        //}
        //public LocalisationSystem LocalisationSystemParent
        //{
        //    get { return _LocalisationSystemParent; }
        //    set { _LocalisationSystemParent = value; }
        //}

        #endregion

        #region ToString override

        public override string ToString()
        {
            if (this.DisplayText != null)
            {
                String text = "LocalisationSystem: " + this.DisplayText;
                return text;
            }
            else return AttributeWorker.ToStringHelper(this, 30);
        }

        #endregion
    }
}
