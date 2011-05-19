using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data.Common;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class SQLiteSerializer : Serializer
    {
        private string _dbPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCeSerializer"/> class.
        /// </summary>
        public SQLiteSerializer(string dbPath)
            : base()
        {
            _dbPath = dbPath;
        }

        public override DbConnection CreateConnection()
        {

            string connection_string = "Data Source=\"" + _dbPath + "\";Max Database Size=128;Default Lock Escalation=100;";
            SQLiteConnection c = null;
            try
            {
                c = new SQLiteConnection(connection_string);
            }
            catch (Exception ex)
            {
                //TOdo
            }

            return c;

        }

    }
}
