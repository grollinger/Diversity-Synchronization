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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
using UBT.AI4.Bio.DivMobi.DataLayer.SyncAttributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    public class CollCircumstances_Enum : ISerializableObject
    {
        #region Instance Data
        [ID]
        [Column]
        [DirectSync]
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

        private DateTime _LogUpdatedWhen;

        //[OneToMany]
        //[JoinColums(DefColumn = "_Code", JoinColumn = "_Circumstances")]
        //private IDirectAccessIterator<IdentificationUnit> _IdentificationUnits;

        #endregion

        #region DefaulConstructor

        public CollCircumstances_Enum()
        {
            _LogUpdatedWhen = new DateTime(1900, 1, 1);//Die Tabelle wird nur vom Repository aufs Mobilgerät geschrieben. Etwaige Änderungen sollen bis zum nächsten Clean ignoriert werden.. Deswegen wird defaultmäßig der 1.Januar 1900 zurückgegeben.
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
        public DateTime LogTime { get { return _LogUpdatedWhen; } set { _LogUpdatedWhen = value; } }
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        //public IDirectAccessIterator<IdentificationUnit> IdentificationUnits
        //{
        //    get { return _IdentificationUnits; }
        //}

        #endregion

        #region ToString Override

        public override string ToString()
        {
            if (this.DisplayText != null)
            {
                String text ="CollCircumstances: "+ this.DisplayText;
                return text;
            }
            else return AttributeWorker.ToStringHelper(this, 30);
        }

        //public String toSql()
        //{
        //    String s = new String();
        //}

        #endregion
    }
}
