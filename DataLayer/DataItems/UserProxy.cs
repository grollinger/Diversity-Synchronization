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
    //Diese Klasse wird benötigt um während der Synchronisation ein Userprofil zu erstellen
    public class UserProxy : ISerializableObject
    {
        [Column]
        private string _LoginName;

        [Column]
        private string _CombinedNameCache;

        [Column]
        private string _UserURI;

        [Column]
        private string _AgentURI;

        [RowGuid]
        [Column(Mapping = "rowguid")]
        private Guid _guid;

        public UserProxy()
        {
        }

        public string LoginName{get{return _LoginName;}set{_LoginName=value;}}
        public string CombinedNameCache{get{return _CombinedNameCache;} set{_CombinedNameCache=value;}}
        public string UserURI{get{return _UserURI;}set{_UserURI=value;}}
        public string AgentURI { get { return _AgentURI; } set { _AgentURI = value; } }
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Einträge der Tabelle werden nicht synchronisiert. Sie werden nur benötigt um die Schnittstelle zu implementieren.
        public Guid Rowguid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("UserProxy: ");
            if (this.LoginName != null)
            {
                sb.Append(this.LoginName).Append(",");
            }
            if (this._CombinedNameCache != null)
            {
                sb.Append(this._CombinedNameCache);
            }
            return sb.ToString();
        }
    }
}
