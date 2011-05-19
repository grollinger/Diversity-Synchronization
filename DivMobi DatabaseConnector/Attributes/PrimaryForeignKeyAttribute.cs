using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryForeignKeyAttribute : ColumnAttribute, IPrimaryForeignKeyAttribute
    {
        private string[] _foreignKeyMapping;

        public PrimaryForeignKeyAttribute() : base()
        {
            _foreignKeyMapping = null;
        }

        public PrimaryForeignKeyAttribute(string columnMapping, string[] foreignKeyMapping) : base(columnMapping)
        {
            this._foreignKeyMapping = foreignKeyMapping;
        }


        public string[] ForeignKeyMapping { get { return _foreignKeyMapping; } }
    }
}
