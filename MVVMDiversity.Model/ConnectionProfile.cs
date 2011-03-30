using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMDiversity.Model
{
    public class ConnectionProfile : IComparable<ConnectionProfile>
    {
        public string Name { get; set; }    

        public string IPAddress { get; set; }

        public string Port { get; set; }

        public string InitialCatalog { get; set; }

        public string TaxonNamesInitialCatalog { get; set; }



        public int CompareTo(ConnectionProfile other)
        {
            if (this.Name == other.Name &&
                this.IPAddress == other.IPAddress &&
                this.Port == other.Port &&
                this.InitialCatalog == other.InitialCatalog &&
                this.TaxonNamesInitialCatalog == other.TaxonNamesInitialCatalog)
                return 0;
            return -1;
        }
    }
}
