using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;

namespace MVVMDiversity.Interface
{
    public interface IConnectionProvider
    {
        Serializer MobileDB { get; }
        Serializer MobileTaxa { get; }
        Serializer Repository { get; }
        Serializer Definitions { get; }
        Serializer Synchronization { get; }

    }
}
