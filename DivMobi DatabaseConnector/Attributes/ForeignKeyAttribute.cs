using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : ColumnAttribute, IForeignKeyAttribute
    {
        private string[] _foreignKeyMapping;
        
        public ForeignKeyAttribute() : base()
        {
            _foreignKeyMapping = null;
        }

        public ForeignKeyAttribute(string columnMapping, string[] foreignKeyMapping) : base(columnMapping)
        {
            this._foreignKeyMapping = foreignKeyMapping;
        }


        public string[] ForeignKeyMapping { get { return _foreignKeyMapping; } }
    }
}
