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
using System.Data.Common;

using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using System.Reflection;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class MS_SqlServerIPSerializer : Serializer
    {
        private String _user;
        private String _password;
        private String _catalog;
        private String _IP;
        private String _Port;


        public MS_SqlServerIPSerializer(String user, String password, String ip, String port, String catalog, String praefix)
        {
            _user = user;
            _password = password;
            _IP = ip;
            _Port = port;
            _catalog = catalog;
            _praefix = praefix;
        }

        public override DbConnection CreateConnection()
        {
            StringBuilder connectionString = new StringBuilder();

            connectionString.Append("Data Source=").Append(_IP).Append(",");
            connectionString.Append(_Port).Append(";");
            connectionString.Append("Network Library=DBMSSOCN").Append(";");
            connectionString.Append("Initial Catalog=").Append(_catalog).Append(";");
            connectionString.Append("Persist Security Info=True;"); //Benötigt?
            connectionString.Append("User ID=").Append(_user).Append(";");
            connectionString.Append("Password=").Append(_password).Append(";");

            //Aus unbekannten Gründen is ein direkter oder impliziter Cast
            //con SqlConnection nach DbConnection im CompactFramework nicht
            //möglich, daher der Umweg über Object
            Object conn = new System.Data.SqlClient.SqlConnection(connectionString.ToString());

            return (DbConnection)conn;
        }

        #region sqlstring generator
        private const String SELECT = "SELECT * FROM ";
        private const String COUNT = "SELECT COUNT(*) FROM ";
        private const String WHERE = " WHERE ";

        //Erzeugt eine SQL Anfrage in der der Tabellenname übergeben wird
        //ACHTUNG: Praefix_Handling Verbessern. Insbesondere die mehrfachen Wege zum Erreichen von Tabellen korrigieren.

        #endregion

    }
}
