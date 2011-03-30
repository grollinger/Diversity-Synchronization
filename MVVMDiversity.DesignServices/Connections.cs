using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;
using MVVMDiversity.Model;

namespace MVVMDiversity.DesignServices
{
    public class Connections : IConnectionManagementService, IConnectionProvider
    {
        public void connectRepositorySqlAuth(Model.ConnectionProfile repo, string user, string password)
        {
            State |= Model.ConnectionState.RepositoriesConnected;
        }

        public void connectRepositoryWinAuth(Model.ConnectionProfile repo)
        {
            State |= Model.ConnectionState.RepositoriesConnected;
        }

        public void connectToMobileDB(DBPaths paths)
        {
            State |= Model.ConnectionState.MobileConnected;
        }

        public void disconnectEverything()
        {
            State = Model.ConnectionState.None;
        }

        public void disconnectFromMobileDB()
        {
            State &= ~Model.ConnectionState.MobileConnected;
        }

        public void disconnectFromRepository()
        {
            State &= ~Model.ConnectionState.RepositoriesConnected;
        }
        Model.ConnectionState _state = Model.ConnectionState.FullyConnected;
        public Model.ConnectionState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                Messenger.Default.Send<ConnectionStateChanged>(new ConnectionStateChanged(value));
            }
        }


        public System.Data.IDbConnection MobileDB
        {
            get { throw new NotImplementedException(); }
        }

        UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Serializer IConnectionProvider.MobileDB
        {
            get { throw new NotImplementedException(); }
        }

        public UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Serializer MobileTaxa
        {
            get { throw new NotImplementedException(); }
        }

        public UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Serializer Repository
        {
            get { throw new NotImplementedException(); }
        }

        public UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Serializer Definitions
        {
            get { throw new NotImplementedException(); }
        }


        public UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Serializer Synchronization
        {
            get { throw new NotImplementedException(); }
        }


        public bool truncateSyncTable()
        {
            return true;
        }
    }
}
