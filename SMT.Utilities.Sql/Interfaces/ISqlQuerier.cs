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
        T[] ExecuteReader<T>(string sql) where T : new();
        T[] ExecuteReader<T>(string sql, IDbDataParameter[] parameters) where T : new();
        T[] ExecuteReader<T>(string sql, BuildObjectFromReader<T> getObjectFromRecord);
        T[] ExecuteReader<T>(string sql, IDbDataParameter[] parameters, BuildObjectFromReader<T> getObjectFromRecord);
        int InsertAndGetIdentity(string sql, IDbDataParameter[] parameters);
        int ExecuteNonQuery(string sql, IDbDataParameter[] parameters);

        IDbDataParameter CreateParameter(string name, SqlDbType type, object value);
        IDbDataParameter CreateParameter(string name, SqlDbType type, int size, object value);
    }
}
