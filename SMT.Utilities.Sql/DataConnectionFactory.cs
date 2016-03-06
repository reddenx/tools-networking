using SMT.Utilities.Sql.Interfaces;
using SMT.Utilities.Sql.SqlCe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Sql
{
    public static class DataConnectionFactory
    {
        public static ISqlQuerier GetSqlCeQuerier(string connectionString)
        {
            return new SqlCeQuerier(connectionString);
        }

        public static ISqlQuerier GetSqlQuerier()
        {
            throw new NotImplementedException("");
        }
    }
}
