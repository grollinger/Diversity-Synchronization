using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes
{
    public interface IForeignKeyAttribute
    {
        string[] ForeignKeyMapping { get; }
    }
}
