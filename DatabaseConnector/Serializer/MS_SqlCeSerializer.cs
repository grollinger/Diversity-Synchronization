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
using System.Data.SqlServerCe;
using System.Data.Common;
using System;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class MS_SqlCeSerializer : Serializer
    {
        private string _dbPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCeSerializer"/> class.
        /// </summary>
        public MS_SqlCeSerializer(string dbPath) : base()
        {
            _dbPath = dbPath;
        }

        public override DbConnection CreateConnection()
        {
            
            string connection_string = "Data Source=\"" + _dbPath + "\";Max Database Size=128;Default Lock Escalation=100;";
            SqlCeConnection c=null;
            try
            {
                c = new SqlCeConnection(connection_string);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return c;

        }
    }
}
