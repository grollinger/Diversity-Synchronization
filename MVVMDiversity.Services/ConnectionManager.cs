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
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.SyncBase;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using log4net;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;

namespace MVVMDiversity.Services
{
    public class ConnectionManager : IConnectionManagementService, IConnectionProvider
    {
        [Dependency]
        public IMessenger MessengerInstance { get; set; }

        private const string REPOSITORY_PREFIX = "[{0}].[dbo].";
        private const string SYNCDB_PREFIX = "[{0}].";
        private const string SYNCDB_CATALOG = "Synchronisation_Test";


        public Serializer Repository { get; private set; }

        public Serializer Definitions { get; private set; }

        public Serializer Synchronization { get; private set; }

        public Serializer MobileDB { get; private set; }

        public Serializer MobileTaxa { get; private set; }

        private ConnectionState _state = ConnectionState.None;
        public ConnectionState State
        {
            get
            {

                return _state;
            }
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    if (MessengerInstance != null)
                        MessengerInstance.Send<ConnectionStateChanged>(new ConnectionStateChanged(value));
                }
            }
        }

        private string _userName;

        private ILog _Log = log4net.LogManager.GetLogger(typeof(ConnectionManager));

        public ConnectionManager()
        {
            State = ConnectionState.None;
        }

        #region Public Methods

        public bool truncateSyncTable()
        {
            if (stateHasFlag(State, ConnectionState.ConnectedToSynchronization) && Synchronization != null)
            {
                using (var sync = Synchronization.CreateConnection())
                {
                    using (var truncate = sync.CreateCommand())
                    {
                        truncate.CommandText = String.Format("TRUNCATE TABLE [{0}].[{1}].[SyncItem];", SYNCDB_CATALOG, _userName);
                        int rowsAffected;
                        try
                        {
                            sync.Open();
                            rowsAffected = truncate.ExecuteNonQuery();
                        }
                        finally
                        {
                            sync.Close();
                        }

                        _Log.InfoFormat("Sync table truncated. {0} rows affected", rowsAffected);

                        return true;
                    }
                }
            }
            else
                _Log.Info("Cannot Truncate SyncTable, No Connection");

            return false;
        }

        #region Repository Connection
        public void connectRepositorySqlAuth(ConnectionProfile repo, string user, string password)
        {
            if (repo != null)
            {
                if (user != null && password != null)
                {
                    _userName = user;
                    Repository = new MS_SqlServerIPSerializer(user, password, repo.IPAddress, repo.Port, repo.InitialCatalog,
                        string.Format(REPOSITORY_PREFIX, repo.InitialCatalog));
                    Definitions = new MS_SqlServerIPSerializer(user, password, repo.IPAddress, repo.Port, repo.TaxonNamesInitialCatalog,
                        string.Format(REPOSITORY_PREFIX, repo.TaxonNamesInitialCatalog));
                    Synchronization = new MS_SqlServerIPSerializer(user, password, repo.IPAddress, repo.Port, SYNCDB_CATALOG,
                        string.Format(SYNCDB_PREFIX, SYNCDB_CATALOG));

                    connectRepository(repo);
                }
                else
                    _Log.Info("Login Credentials incomplete");
            }
            else
                _Log.Info("No Connection Configured");
        }

        public void connectRepositoryWinAuth(ConnectionProfile repo)
        {
            if (repo != null)
            {
                _userName = Environment.UserName;
                Repository = new MS_SqlServerWASerializier(repo.IPAddress + "," + repo.Port, repo.InitialCatalog,
                    string.Format(REPOSITORY_PREFIX, repo.InitialCatalog));
                Definitions = new MS_SqlServerWASerializier(repo.IPAddress + "," + repo.Port, repo.TaxonNamesInitialCatalog,
                    string.Format(REPOSITORY_PREFIX, repo.TaxonNamesInitialCatalog));
                Synchronization = new MS_SqlServerWASerializier(repo.IPAddress + "," + repo.Port, SYNCDB_CATALOG,
                    string.Format(SYNCDB_PREFIX, SYNCDB_CATALOG));


                connectRepository(repo);
            }
            else
                _Log.Info("No Connection Configured");
        }


        #endregion

        #region Mobile Connection
        public void connectToMobileDB(DBPaths paths)
        {

            int i = 0;
            try
            {
                MobileDB = new MS_SqlCeSerializer(paths.MobileDB);
                i++;//1
                MobileDB.RegisterTypes(divMobiTypes());
                i++;
                MobileDB.RegisterType(typeof(UserProfile));
                i++;
                MobileDB.Activate();
                i++;



                //mobile Tax Serializer erzeugen
                try
                {
                    MobileTaxa = new MS_SqlCeSerializer(paths.MobileTaxa);

                    i++;
                    MobileTaxa.RegisterType(typeof(TaxonNames));
                    MobileTaxa.RegisterType(typeof(PropertyNames));
                    i++;//9
                }
                catch
                {
                    MobileTaxa = null;
                }
            }
            catch (Exception mobileDBEx)
            {
                _Log.ErrorFormat("ConnectionError {0} {1}", i, mobileDBEx.Message != null ? mobileDBEx.Message : "");
                MobileDB = null;
            }
            finally
            {
                i = 0;
                if (MobileDB != null)
                {
                    State |= ConnectionState.ConnectedToMobile;
                    i = 10;
                }
                else
                {
                    State &= ~ConnectionState.ConnectedToMobile;
                    i = 20;
                }
                if (MobileTaxa != null)
                {
                    State |= ConnectionState.ConnectedToMobileTax;
                    i = i + 100;
                }
                else
                {
                    State &= ~ConnectionState.ConnectedToMobileTax;
                    i = i + 200;
                }
                if (i != 110)
                    _Log.ErrorFormat("Final Result: {0}", i);
            }
        }

        private void connectRepository(ConnectionProfile repo)
        {
            //Verbindung zum Repository herstellen
            try
            {
                Repository.RegisterTypes(divMobiTypes());
                Repository.RegisterType(typeof(UserProxy));
                Repository.Activate();
                try
                {

                    String syncIt = "[" + _userName + "].SyncItem";
                    String fieldSt = "[" + _userName + "].FieldState";
                    if(!MappingDictionary.Mapping.ContainsKey(typeof(SyncItem)))
                        MappingDictionary.Mapping.Add(typeof(SyncItem), syncIt);
                    if (!MappingDictionary.Mapping.ContainsKey(typeof(FieldState)))
                        MappingDictionary.Mapping.Add(typeof(FieldState), fieldSt);

                    Synchronization.RegisterType(typeof(SyncItem));
                    Synchronization.RegisterType(typeof(FieldState));
                    Synchronization.Activate();

                }
                catch// (Exception syncDBEx)
                {
                    Synchronization = null;
                    MappingDictionary.Mapping.Clear();
                }

                try
                {
                    //Taxon Serializer erstellen                

                    Definitions.RegisterType(typeof(TaxonNames));
                    Definitions.RegisterType(typeof(PropertyNames));
                }
                catch //(Exception repTaxEx)
                {
                    Definitions = null;
                }

            }
            catch// (Exception repositoryEx)
            {
                Repository = null;
            }

            var state = State;
            if (Repository != null)
                state |= ConnectionState.ConnectedToRepository;
            if (Definitions != null)
                state |= ConnectionState.ConnectedToRepTax;
            if (Synchronization != null)
                state |= ConnectionState.ConnectedToSynchronization;
            State = state;
        }

        public void disconnectFromRepository()
        {
            if (Synchronization != null)
            {
                Synchronization.Dispose();
                Synchronization = null;
            }
            if (Repository != null)
            {
                Repository.Dispose();
                Repository = null;
            }
            if (Definitions != null)
            {
                Definitions.Dispose();
                Definitions = null;
            }
            State &= ~ConnectionState.RepositoriesConnected;
        }



        public void disconnectFromMobileDB()
        {
            if (MobileDB != null)
            {
                MobileDB.Dispose();
                MobileDB = null;
            }
            if (MobileTaxa != null)
            {
                MobileTaxa.Dispose();
                MobileTaxa = null;
            }
            State &= ~ConnectionState.MobileConnected;
        }
        #endregion



        public void disconnectEverything()
        {
            disconnectFromRepository();
            disconnectFromMobileDB();
        }



        #endregion


        private List<Type> divMobiTypes()
        {
            return new List<Type>()
            {
                typeof(Property),
                typeof(Analysis),
                typeof(AnalysisTaxonomicGroup),
                typeof(CollectionAgent),
                typeof(CollectionEvent),
                typeof(CollectionEventImage),
                typeof(CollectionEventLocalisation),
                typeof(CollectionEventProperty),
                typeof(CollectionSpecimen),
                typeof(CollectionSpecimenImage),
                typeof(CollEventImageType_Enum),
                typeof(CollSpecimenImageType_Enum),
                typeof(CollTaxonomicGroup_Enum),
                typeof(Identification),
                typeof(IdentificationUnit),
                typeof(IdentificationUnitAnalysis),
                typeof(LocalisationSystem),
                typeof(CollCircumstances_Enum),
                typeof(CollUnitRelationType_Enum),
                typeof(CollectionEventSeries),
                //Bis hier: Korrepondiert zu DBVersion 20
                //typeof(CollectionEventSeriesImage),
                //typeof(CollEventSeriesImageType_Enum),
                ////Bis hier: Korrepondiert zu DBVersion 22
                typeof(CollIdentificationCategory_Enum),
                //typeof(CollTypeStatus_Enum),
                //typeof(CollIdentificationQualifier_Enum),
                ////Bis hier: Korrepondiert zu DBVersion 25
                //typeof(CollLabelTranscriptionState_Enum),
                //typeof(CollLabelType_Enum),
                ////Bis hier: Korrepondiert zu DBVersion 27
                //typeof(Collection),
                //typeof(CollectionProject),
                //typeof(CollectionSpecimenPart),
                //typeof(CollMaterialCategory_Enum),
                //Bis hier: Korrepondiert zu DBVersion 31
                typeof(IdentificationUnitGeoAnalysis),
                typeof(AnalysisResult),

                typeof(UserTaxonomicGroupTable),
                //Bis hier: Korrepondiert zu DBVersion 34

                
            };
        }

        private List<Type> syncTypes()
        {
            return new List<Type>()
            {
                typeof(SyncItem),
                typeof(FieldState)
            };
        }


        private bool stateHasFlag(ConnectionState s, ConnectionState f)
        {
            return (s & f) == f;
        }





    }
}
