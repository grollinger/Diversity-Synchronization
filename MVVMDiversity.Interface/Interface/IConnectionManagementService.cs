using System;
using MVVMDiversity.Model;
using System.Data;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;
namespace MVVMDiversity.Interface
{
    public interface IConnectionManagementService
    {
        void connectRepositorySqlAuth(MVVMDiversity.Model.ConnectionProfile repo, string user, string password);
        void connectRepositoryWinAuth(MVVMDiversity.Model.ConnectionProfile repo);
        void connectToMobileDB(DBPaths paths);
        void disconnectEverything();
        void disconnectFromMobileDB();
        void disconnectFromRepository();
        Model.ConnectionState State { get; }
        bool truncateSyncTable();
        
        
    }
}
