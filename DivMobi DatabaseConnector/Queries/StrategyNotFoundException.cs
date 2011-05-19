using System;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Queries
{
    public class StrategyNotFoundException : Exception
    {
        public StrategyNotFoundException() : base() { }
        public StrategyNotFoundException(String message) : base(message) { }
        public StrategyNotFoundException(String message, Exception innerException) : base(message, innerException) { }
    }
}
