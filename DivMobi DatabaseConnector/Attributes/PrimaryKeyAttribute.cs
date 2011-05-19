using System;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : ColumnAttribute, IPrimaryKeyAttribute
    {
        public PrimaryKeyAttribute() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="columnMapping">The name of the column in the data base.</param>
        public PrimaryKeyAttribute(string columnMapping) : base(columnMapping) { }
    }
}
