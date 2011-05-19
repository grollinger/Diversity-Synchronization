using System;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncPrimaryKeyAttribute : PrimaryKeyAttribute
    {
        public AutoIncPrimaryKeyAttribute() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoIncPrimaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="columnMapping">The name of the column in the data base.</param>
        public AutoIncPrimaryKeyAttribute(string columnMapping) : base(columnMapping) { }
    }
}
