using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class ReaderPositionException : SerializerException
    {
        public ReaderPositionException() : base() { }
        public ReaderPositionException(String message) : base(message) { }
        public ReaderPositionException(String message, Exception innerException) : base(message, innerException) { }
    }
}
