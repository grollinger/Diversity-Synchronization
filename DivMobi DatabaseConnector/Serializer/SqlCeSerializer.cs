using System.Data.SqlServerCe;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class SqlCeSerializer : Serializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCeSerializer"/> class.
        /// </summary>
        public SqlCeSerializer()
        {

        }

        protected override System.Data.Common.DbConnection GetConnection()
        {
            string connection_string = "Data Source=\"" + _db_path + "\";Max Database Size=128;Default Lock Escalation=100;";
            return new SqlCeConnection(connection_string);   
        }
    }
}
