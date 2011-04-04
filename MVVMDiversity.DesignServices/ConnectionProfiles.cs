using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;

namespace MVVMDiversity.DesignServices
{
    public class ConnectionProfiles : IConnectionProfilesService
    {
        public IList<ConnectionProfile> getAvailableProfiles()
        {
            return new List<ConnectionProfile>()
            {
                new ConnectionProfile()
                {
                    InitialCatalog = "Init",
                    TaxonNamesInitialCatalog = "Taxon_Init",
                    Name = "Test",
                    Port = "8080",
                    IPAddress = "127.0.0.1"
                }
            };
        }
    }
}
