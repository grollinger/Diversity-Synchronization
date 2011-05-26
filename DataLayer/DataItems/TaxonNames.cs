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
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;

namespace UBT.AI4.Bio.DivMobi.DataLayer.DataItems
{
    
    public class TaxonNames : ISerializableObject
    {
        #region Instance Data
        [ID]
        [Column]
        private string _NameURI;

        [Column]
        private string _TaxonNameCache;

        [Column]
        private string _Synonymy;

        [Column]
        private string _Family;

        [Column]
        private string _Order;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;


        #endregion

        public TaxonNames()
        {
        }

            

        #region Properties

        public string NameURI { get { return _NameURI; } set { _NameURI = value; } }

        public string TaxonNameCache { get { return _TaxonNameCache; } set { _TaxonNameCache = value; } }

        public string Synonymy { get { return _Synonymy; } set { _Synonymy = value; } }
        
        public string Family {get {return _Family;} set {_Family=value;}}

        public string Order { get { return _Order; } set { _Order = value; } }
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Tabelle wird nur vom Repository aufs Mobilgerät geschrieben. Etwaige Änderungen sollen bis zum nächsten Clean ignoriert werden.. Deswegen wird defaultmäßig der 1.Januar 1900 zurückgegeben.
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        #endregion

        #region ToString override

        public override string ToString()
        {
            if (this._TaxonNameCache != null)
            {
                String text = "TaxonNames: " + this._TaxonNameCache;
                return text;
            }
            else return AttributeWorker.ToStringHelper(this, 30);
        }

        #endregion

    }
}
