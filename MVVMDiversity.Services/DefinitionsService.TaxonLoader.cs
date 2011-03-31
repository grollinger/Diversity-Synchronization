//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Georg Rollinger
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
using log4net;
using MVVMDiversity.Model;
using System.Data;
using System.Data.SqlServerCe;
using System.ComponentModel;
using System.Data.SqlClient;

namespace MVVMDiversity.Services
{
    public partial class DefinitionsService
    {
        private class TaxonLoader
        {
            private ILog _Log = log4net.LogManager.GetLogger(typeof(TaxonLoader));                  

            private IDbConnection _source;
            private SqlCeConnection _destination;
            private IDbConnection _mobile;
            private IEnumerable<TaxonList> _selectedTaxa;
            private string destinationTable;
            private string sourceExpression;

            BackgroundOperation _progress;
            float progressPerTaxonList = 100f;
            DefinitionsService _owner;

            public TaxonLoader(DefinitionsService owner)
            {
                _owner = owner;
            }
            
            
            private const string TAXON_LISTS_TABLE = "TaxonListsForUser";
            private const string TAXONLIST_DATASOURCE_COLUMN = "DataSource";
            private const string TAXONLIST_NAME_COLUMN = "DisplayText";
            private const string TAXONLIST_GROUP_COLUMN = "TaxonomicGroup";

            public void startTaxonDownload(IEnumerable<TaxonList> selectedTaxa, BackgroundOperation progress)
            {
                _progress = progress;
                _selectedTaxa = selectedTaxa;                
           
                try
                {
                    createConnections();

                    openConnections();

                    downloadTaxonLists();

                    fillTaxonListsForUser();                   

                    updateSelectedTaxonLists();

                        
                }
                catch (Exception e)
                {
                    _Log.ErrorFormat("Error while downloading Taxon Lists: [{0}]", e);
                }
                finally
                {
                    closeConnections();
                }


                      
            }
            private void openConnections()
            {
                try
                {
                    if(_source != null)
                        _source.Open();
                }
                catch (Exception e)
                {
                    _Log.ErrorFormat("Exception opening Source: [{0}]", e);
                }
                try
                {
                    if(_destination != null)
                        _destination.Open();
                }
                catch (Exception ex)
                {
                    _Log.ErrorFormat("Exception opening Destination: [{0}]", ex);
                }
                try
                {
                    if(_mobile!= null)
                        _mobile.Open();
                }
                catch (Exception ex)
                {
                    _Log.ErrorFormat("Exception opening Mobile DB: [{0}]", ex);
                }
                
            }

            private void closeConnections()
            {
                if (_source != null)
                {
                    _source.Close();
                    _source = null;
                }
                if(_destination != null)
                {
                    _destination.Close();
                    _destination = null;
                }
                if (_mobile != null)
                {
                    _mobile.Close();
                    _mobile = null;
                }
            }

            private void createConnections()
            {
                var connectionProvider = _owner.Connections;
                if (connectionProvider != null)
                {
                    
                    if (connectionProvider.Definitions != null)
                    {
                        _source = connectionProvider.Definitions.CreateConnection();
                    }
                    else
                        _Log.Error("Definitions Serializer not available");

                    if (connectionProvider.MobileTaxa != null)
                    {
                        _destination = connectionProvider.MobileTaxa.CreateConnection() as SqlCeConnection;
                    }
                    else
                        _Log.Error("Mobile Taxon Serializer not available");

                    if (connectionProvider.MobileDB != null)
                    {
                        _mobile = connectionProvider.MobileDB.CreateConnection();
                    }
                    else
                        _Log.Error("Mobile DB Serializer not available");

                }
                else
                    _Log.Error("ConnectionsProvider not available.");                
               
            }

            private void downloadTaxonLists()
            {
                System.Threading.Thread.Sleep(500);
                progressPerTaxonList = 100 / _selectedTaxa.Count();
            

                foreach (var taxonList in _selectedTaxa)
                {
                    

                    copyTable(taxonList.DataSource);                
                }
            }

            private void updateSelectedTaxonLists()
            {
                var firstListOfEachGroup = from list in _selectedTaxa
                                            group list by list.TaxonomicGroup into g
                                            select g.First();

               
                using (var cmd = _mobile.CreateCommand())
                {                    
                    try
                    {
                        foreach (var list in firstListOfEachGroup)
                        {
                            cmd.CommandText = String.Format("UPDATE [UserTaxonomicGroupTable] SET [TaxonomicTable] = '{0}' WHERE [TaxonomicCode] = '{1}';", list.DataSource, list.TaxonomicGroup);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                cmd.CommandText = String.Format("INSERT INTO [UserTaxonomicGroupTable] ([TaxonomicCode], [TaxonomicTable]) VALUES ('{0}','{1}');", list.TaxonomicGroup, list.DataSource);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _Log.ErrorFormat("Error while updating Selected Taxon Lists: [{0}]", ex);
                    }                    
                }
                
            }

            private void fillTaxonListsForUser()
            {
                try
                {
                    using (var count = _destination.CreateCommand())
                    {
                        using (var insert = _destination.CreateCommand())
                        {
                            foreach (var list in _selectedTaxa)
                            {
                                count.CommandText = String.Format("SELECT COUNT(*) FROM {0} WHERE [DataSource] = '{1}'", TAXON_LISTS_TABLE, list.DataSource);
                                bool exists = ((int)count.ExecuteScalar()) != 0;
                                if (!exists)
                                {
                                    insert.CommandText = String.Format("INSERT INTO {0} ([{1}],[{2}],[{3}]) VALUES ('{4}','{5}','{6}')",
                                        TAXON_LISTS_TABLE, TAXONLIST_DATASOURCE_COLUMN, TAXONLIST_NAME_COLUMN, TAXONLIST_GROUP_COLUMN,
                                        list.DataSource, list.DisplayText, list.TaxonomicGroup);
                                    insert.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Log.ErrorFormat("Error while filling TaxonListsForUser:\n{0}", ex.Message);
                }
            }

        
            private void copyTable(string table)
            {
                //TODO
                sourceExpression = "["+_owner.Settings.getOptions().CurrentConnection.TaxonNamesInitialCatalog+"].[dbo].[" + table + "]";
                destinationTable = table;
                copyDBExpressionToTable();
            }
        

            private void copyDBExpressionToTable()
            {

                dropDestinationTable();
            
                using (var cmd = _destination.CreateCommand())
                {
                    cmd.CommandText = destinationTable;
                    cmd.CommandType = CommandType.TableDirect;

                
                    int rowCount = countRowsToCopy();
                

                    IDbCommand readCmd = _source.CreateCommand();
                    readCmd.CommandText = "SELECT * FROM " + sourceExpression;
                    using (var sourceReader = readCmd.ExecuteReader())
                    {

                        createDestinationTable(sourceReader.GetSchemaTable());
                       
                        using (var destinationResultSet = cmd.ExecuteResultSet(ResultSetOptions.Updatable | ResultSetOptions.Scrollable))
                        {
                            while (sourceReader.Read())
                            {
                                SqlCeUpdatableRecord record = destinationResultSet.CreateRecord();
                                object[] values = new object[sourceReader.FieldCount];
                                sourceReader.GetValues(values);
                                record.SetValues(values);
                                destinationResultSet.Insert(record);                                                                                   
                            }
                        }
                        sourceReader.Close();
                    }
                }
                return;
            }

            private int countRowsToCopy()
            {
                using (IDbCommand readCmd = _source.CreateCommand())
                {
                    readCmd.CommandText = "SELECT COUNT(*) FROM " + sourceExpression;
                    return (int)readCmd.ExecuteScalar();
                }
            }

            private void createDestinationTable(DataTable schema)
            {
                using (var create = _destination.CreateCommand())
                {
                    create.CommandText = GetCreateTableStatement(destinationTable, schema);
                    create.ExecuteNonQuery();
                }
            }

            private void dropDestinationTable()
            {
                bool tableExists = false;
                using (var existenceCheckCmd = _destination.CreateCommand())
                {
                    existenceCheckCmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + destinationTable + "'";
                    tableExists = (Int32)existenceCheckCmd.ExecuteScalar() > 0;                
                }
                if (tableExists)
                {
                    using (var drop = _destination.CreateCommand())
                    {
                        drop.CommandText = "DROP TABLE " + destinationTable;
                        drop.ExecuteNonQuery();           
                    }
                }
            }

                /// <summary>
            /// Genenerates a SQL CE compatible CREATE TABLE statement based on a schema obtained from
            /// a SqlDataReader or a SqlCeDataReader.
            /// </summary>
            /// <param name="tableName">The name of the table to be created.</param>
            /// <param name="schema">The schema returned from reader.GetSchemaTable().</param>
            /// <returns>The CREATE TABLE... Statement for the given schema.</returns>
            public static string GetCreateTableStatement(string tableName, DataTable schema)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(string.Format("CREATE TABLE [{0}] (", tableName));

                foreach (DataRow row in schema.Rows)
                {
                    string typeName = row["DataType"].ToString();
                    Type type = Type.GetType(typeName);

                    string name = (string)row["ColumnName"];
                    int size = (int)row["ColumnSize"];

                    SqlDbType dbType = GetSqlDBTypeFromType(type);
                    string sqlceType = GetSqlServerCETypeName(dbType,size);

                    builder.Append(String.Format("[{0}] {1}, ", name, sqlceType));
                }

                if (schema.Rows.Count > 0) builder.Length = builder.Length - 2;

                builder.Append(");");
                return builder.ToString();
            }
       

            /// <summary>
            /// Gets the correct SqlDBType for a given .NET type. Useful for working with SQL CE.
            /// </summary>
            /// <param name="type">The .Net Type used to find the SqlDBType.</param>
            /// <returns>The correct SqlDbType for the .Net type passed in.</returns>
            public static SqlDbType GetSqlDBTypeFromType(Type type)
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(DbType));
           
                    DbType dbType = (DbType)tc.ConvertFrom(type.Name);
                    // A cheat, but the parameter class knows how to map between DbType and SqlDBType.
                    SqlParameter param = new SqlParameter();
                    param.DbType = dbType;
                    return param.SqlDbType; // The parameter class did the conversion for us!!
           
            }

            /// <summary>
            /// The method gets the SQL CE type name for use in SQL Statements such as CREATE TABLE
            /// </summary>
            /// <param name="dbType">The SqlDbType to get the type name for</param>
            /// <param name="size">The size where applicable e.g. to create a nchar(n) type where n is the size passed in.</param>
            /// <returns>The SQL CE compatible type for use in SQL Statements</returns>
            public static string GetSqlServerCETypeName(SqlDbType dbType, int size)
            {
                // Conversions according to: http://msdn.microsoft.com/en-us/library/ms173018.aspx
                bool max = (size == int.MaxValue) ? true : false;
                bool over4k = (size > 4000) ? true : false;
                switch (dbType)
                {
	                case SqlDbType.BigInt:
                        return "bigint";
                        
                    case SqlDbType.Binary:
                        return String.Format("binary({0})",size);
                        
                    case SqlDbType.Bit:
                        return "bit";
                        
                    case SqlDbType.Char:
                        return (over4k) ? "ntext" : String.Format("nchar({0})",size);
                        
                    case SqlDbType.Date:
                        return "nchar(10)";
                        
                    case SqlDbType.DateTime:
                        return "datetime";
                        
                    case SqlDbType.DateTime2:
                        return "nvarchar(27)";
                        
                    case SqlDbType.DateTimeOffset:
                        return "nvarchar(34)";
                        
                    case SqlDbType.Decimal:
                        break;
                    case SqlDbType.Float:
                        return "float";
                        
                    case SqlDbType.Image:
                        return "image";
                        
                    case SqlDbType.Int:
                        return "int";
                        
                    case SqlDbType.Money:
                        return "money";
                        
                    case SqlDbType.NChar:
                        return String.Format("nchar({0})",size);
                        
                    case SqlDbType.NText:
                        return "ntext";
                        
                    case SqlDbType.NVarChar:
                        return String.Format("nvarchar({0})",size);
                        
                    case SqlDbType.Real:
                        return "real";
                        
                    case SqlDbType.SmallDateTime:
                        return "datetime";
                        
                    case SqlDbType.SmallInt:
                        return "smallint";
                        
                    case SqlDbType.SmallMoney:
                        return "money";
                        
                    case SqlDbType.Structured:
                        break;
                    case SqlDbType.Text:
                        return "ntext";
                        
                    case SqlDbType.Time:
                        return "nvarchar(16)";
                        
                    case SqlDbType.Timestamp:
                        break;
                    case SqlDbType.TinyInt:
                        return "tinyint";
                        
                    case SqlDbType.Udt:
                        break;
                    case SqlDbType.UniqueIdentifier:
                        break;
                    case SqlDbType.VarBinary:
                        break;
                    case SqlDbType.VarChar:
                        break;
                    case SqlDbType.Variant:
                        break;
                    case SqlDbType.Xml:
                        return "ntext";
                        
                    default:
                        break;
                }
                throw new ArgumentException (String.Format("Could Not Convert SQL DataType {0} to SQLCE DataType",dbType.ToString()));
            }
        }
    }
}
