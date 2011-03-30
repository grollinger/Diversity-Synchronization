using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using MVVMDiversity.Model;
using log4net;
using System.Data;

namespace MVVMDiversity.Services
{
    public class TaxonListService : ITaxonListService
    {
        [Dependency]
        public IConnectionProvider Connections { get; set; }

        [Dependency]
        public IUserOptionsService Settings { get; set; }

        ILog _Log = LogManager.GetLogger(typeof(TaxonListService));

        public IList<Model.TaxonList> getAvailableTaxonLists()
        {
            ConnectionProfile connectionProfile = null;
            string userName = null;
            IDbConnection definitionsConnection = null;
            IList<TaxonList>  res = new List<TaxonList>();

            if (Settings != null)
            {
                var userSettings = Settings.getOptions();
                if (userSettings != null)
                {
                    connectionProfile = userSettings.CurrentConnection;
                    userName = userSettings.Username;
                }
                else
                    _Log.Error("UserOptions are empty.");
            }
            else
                _Log.Error("UserOptionsService not available.");

            if (Connections != null)
            {
                if (Connections.Definitions != null)
                    definitionsConnection = Connections.Definitions.CreateConnection();
                else
                    _Log.Error("Definitions Serializer empty.");
            }
            else
                _Log.Error("ConnectionsProvider unavailable.");


            if (connectionProfile != null && userName != null && definitionsConnection != null)
            {
                var taxonListsForUser = "[" + connectionProfile.TaxonNamesInitialCatalog + "].[dbo].[TaxonListsForUser]('" + userName + "')";
                string sql = String.Format("SELECT * FROM {0};", taxonListsForUser);                
                    
                try
                {
                    using (var cmd = definitionsConnection.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        definitionsConnection.Open();
                        var rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            TaxonList tl = new TaxonList();
                            tl.DataSource = rdr["Datasource"].ToString();
                            tl.DisplayText = rdr["DisplayText"].ToString();
                            //tl.IsSelected = false;
                            tl.TaxonomicGroup = rdr["TaxonomicGroup"].ToString();
                            res.Add(tl);
                        }
                    }
                }
                catch (Exception e)
                {
                    _Log.ErrorFormat("An Error Occured while retrieving available Taxon Lists [{0}]",e);
                }
                finally
                {
                    definitionsConnection.Close();
                    definitionsConnection.Dispose();
                }
            }
            return res;
        }     
    }
}
