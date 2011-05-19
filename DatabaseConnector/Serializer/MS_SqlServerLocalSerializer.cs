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
using System.Data.Common;
using System;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class MS_SqlServerLocalSerializer : Serializer
    {
        private String _catalog;
        private String _IP;
        private String _Port;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCeSerializer"/> class.
        /// </summary>
        public MS_SqlServerLocalSerializer(String ip, String port, String catalog)
            : base()
        {
            this._catalog = catalog;
            this._IP = ip;
            this._Port = port;
        }

        public override DbConnection CreateConnection()
        {

            string connection_string = "Data Source="+ this._IP+","+this._Port+";Initial Catalog="+this._catalog+";Integrated Security=True";
            Object conn = new System.Data.SqlClient.SqlConnection(connection_string);
            return (DbConnection)conn;

        }
    }
}
