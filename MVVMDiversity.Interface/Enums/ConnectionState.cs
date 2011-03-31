using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMDiversity.Model
{
    [Flags]
    public enum ConnectionState
    {           
        None = 0x00,
        ConnectedToRepository = 0x01,
        ConnectedToRepTax = 0x02,
        ConnectedToMobile = 0x04,
        ConnectedToMobileTax = 0x08,
        ConnectedToSynchronization = 0x10,



        FullyConnected = ConnectedToRepository | ConnectedToRepTax | ConnectedToMobile | ConnectedToMobileTax | ConnectedToSynchronization,
        RepositoriesConnected = ConnectedToRepository | ConnectedToRepTax | ConnectedToSynchronization,
        MobileConnected = ConnectedToMobile | ConnectedToMobileTax,        
    }
}
