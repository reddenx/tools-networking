using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Sql.Interfaces
{
    public delegate T BuildObjectFromReader<T>(IDataReader record);

    public interface ISqlQuerier
    {
        T[] ExecuteReader<T>(string sql, params IDbDataParameter[] parameters) where T : new();
        T[] ExecuteReader<T>(string sql, BuildObjectFromReader<T> getObjectFromRecord, params IDbDataParameter[] parameters);
        T ExecuteScalar<T>(string sql, params IDbDataParameter[] parameters);
        int InsertAndGetIdentity(string sql, params IDbDataParameter[] parameters);
        int ExecuteNonQuery(string sql, params IDbDataParameter[] parameters);

        IDbDataParameter CreateParameter(string name, SqlDbType type, object value);
        IDbDataParameter CreateParameter(string name, SqlDbType type, int size, object value);
    }
}
