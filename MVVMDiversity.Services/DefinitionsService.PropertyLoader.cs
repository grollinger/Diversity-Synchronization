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
using MVVMDiversity.Model;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using log4net;
using System.Data.Common;
using System.Data.SqlServerCe;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;
using System.Data;

namespace MVVMDiversity.Services
{
    public partial class DefinitionsService
    {
        private class PropertyLoader
        {
            DefinitionsService _owner;
            BackgroundOperation _progress;
            int _progressPerProperty = 50;
            ILog _Log = LogManager.GetLogger(typeof(PropertyLoader));

            string _prefix;
            IDbConnection _connDefinitions;
            IDbConnection _connMobileDefinitions;

            public PropertyLoader(DefinitionsService owner)
            {
                _owner = owner;
            }

            public void updateProperties(BackgroundOperation progress)
            {
                _progress = progress;
                _progress.ProgressDescriptionID = "Services_Definitions_LoadingProperties";

                if (_owner.Connections != null)
                {
                    var definitions = _owner.Connections.Definitions;
                    if (definitions != null)
                    {
                        _prefix = definitions.Praefix;
                        //IRestriction r = RestrictionFactory.TypeRestriction(typeof(PropertyNames));
                        //properties = taxonrepSerializer.Connector.LoadList<PropertyNames>(r);//geht nicht , weil auf der Sicht keine GUID definiert ist
                        _connDefinitions = definitions.CreateConnection();                     
                    }
                    else
                        _Log.Error("Definitions serializer not available!");
                    var mobile = _owner.Connections.MobileTaxa;
                    if (mobile != null)
                    {
                        _connMobileDefinitions = mobile.CreateConnection();
                    }
                    else
                        _Log.Error("Mobile Definitions serializer not available!");
                }
                else
                    _Log.Error("Connections provider not available.");

                if (_connDefinitions != null && _connMobileDefinitions != null)
                {
                    updateProperties("LebensraumTypen", "LebensraumTypenLfU");

                    updateProperties("Pflanzengesellschaften", "Pflanzengesellschaften");
                }

            }

            //Überträgt die Namen von Properties vom Repository in die mobile Datenbank. Um doppelte Einträge zu vermeiden werden zunächst Einträge in der Mobilen Datenbank
            //gelöscht. Kann mit Reflexion verallgemeinert und mit updateTaxonNames kombiniert werden.
            public void updateProperties(string sourceTable, string targetTable)
            {
                if (MappingDictionary.Mapping.ContainsKey(typeof(PropertyNames)))
                    MappingDictionary.Mapping[typeof(PropertyNames)] = sourceTable;
                else
                    MappingDictionary.Mapping.Add(typeof(PropertyNames), sourceTable);
                
                //Neue Properties holen
                IList<PropertyNames> properties = new List<PropertyNames>();
                
                IDbCommand select = _connDefinitions.CreateCommand();
                IDbCommand count = _connDefinitions.CreateCommand();

                select.CommandText = String.Format("SELECT * FROM {0}", _prefix + sourceTable);
                count.CommandText = String.Format("SELECT COUNT(*) FROM {0}", _prefix + sourceTable);
                IDataReader reader = null;
                
                int rowCount = 1;
                float progressPerRow = 1;
                try
                {
                    _connDefinitions.Open();
                    rowCount = (int)count.ExecuteScalar();
                    progressPerRow = ((float)_progressPerProperty) / 2*rowCount;

                    reader = select.ExecuteReader();

                    while (reader.Read())
                    {
                        PropertyNames prop = new PropertyNames();
                        prop.PropertyID = reader.GetInt32(0);
                        if (!reader.IsDBNull(1))
                            prop.PropertyURI = reader.GetString(1);
                        if (!reader.IsDBNull(2))
                            prop.DisplayText = reader.GetString(2);
                        if (!reader.IsDBNull(3))
                            prop.HierarchyCache = reader.GetString(3);
                        //primary key
                        prop.TermID = reader.GetInt32(4);
                        if (!reader.IsDBNull(5))
                            prop.BroaderTermID = reader.GetInt32(5);
                        properties.Add(prop);

                        
                    }
                }
                catch (Exception e)
                {
                    _Log.ErrorFormat("Exception reading Properties: [{0}]", e);
                    return;
                }
                finally
                {
                    _connDefinitions.Close();
                }

                
                _connMobileDefinitions.Open();
                IDbTransaction trans = null;
                try
                {
                    trans = _connMobileDefinitions.BeginTransaction();                   


                    //Alte Taxa löschen


                    IDbCommand commandMobile = _connMobileDefinitions.CreateCommand();
                    commandMobile.CommandText = String.Format("DELETE FROM {0}", targetTable);
                    commandMobile.ExecuteNonQuery();

                    //Taxa eintragen            



                    foreach (PropertyNames prop in properties)
                    {
                        var sb = new StringBuilder(); //Alternativ mobileDBSerializer.Connector.Save(taxon)
                        sb.Append("Insert INTO ").Append(targetTable).Append(" (PropertyID,PropertyURI,DisplayText,HierarchyCache,TermID,BroaderTermID) VALUES (");
                        sb.Append(SqlUtil.SqlConvert(prop.PropertyID)).Append(",");
                        sb.Append(SqlUtil.SqlConvert(prop.PropertyURI)).Append(",").Append(SqlUtil.SqlConvert(prop.DisplayText)).Append(",").Append(SqlUtil.SqlConvert(prop.HierarchyCache)).Append(",").Append(prop.TermID).Append(",").Append(prop.BroaderTermID).Append(")");
                        IDbCommand insert = _connMobileDefinitions.CreateCommand();
                        insert.CommandText = @sb.ToString();
                        insert.ExecuteNonQuery();

                        
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();

                    _Log.ErrorFormat("Exception writing Properties to mobile DB: [{0}]", ex);
                    return;
                }
                finally
                {
                    _connMobileDefinitions.Close();
                }
            }
        }
    }
}
