using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.Sql.Interfaces;

namespace SMT.Utilities.Sql.SqlCe
{
    public class SqlCeQuerier : ISqlQuerier
    {
        private readonly string ConnectionString;

        private SqlCeQuerier(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public static ISqlQuerier Get(string connectionString)
        {
            return new SqlCeQuerier(connectionString);
        }

        public T[] ExecuteReader<T>(BuildObjectFromReader<T> getObjectFromRecord, string sql, IDbDataParameter[] parameters)
        {
            var data = new List<T>();


            using (var connection = new SqlCeConnection(ConnectionString))
            {
                var command = BuildCommand(sql, parameters, connection);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(getObjectFromRecord(reader));
                    }
                }

                connection.Close();
            }

            return data.ToArray();
        }

        public int InsertAndGetIdentity(string sql, IDbDataParameter[] parameters)
        {
            using (var connection = new SqlCeConnection(ConnectionString))
            {
                var command = BuildCommand(sql, parameters, connection);

                SqlCeCommand command2 = new SqlCeCommand(@"select @@identity", connection);

                connection.Open();

                command.ExecuteNonQuery();

                var identity = command2.ExecuteScalar();
                return Convert.ToInt32(identity);
            }
        }

        public int ExecuteNonQuery(string sql, IDbDataParameter[] parameters)
        {
            using (var connection = new SqlCeConnection(ConnectionString))
            {
                var command = BuildCommand(sql, parameters, connection);

                return command.ExecuteNonQuery();
            }
        }

        private SqlCeCommand BuildCommand(string sql, IDbDataParameter[] parameters, SqlCeConnection connection)
        {
            var command = new SqlCeCommand(sql, connection);

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter as SqlCeParameter);
            }

            return command;
        }

        public IDbDataParameter CreateParameter(string name, SqlDbType type, int size, object value)
        {
            var param = new SqlCeParameter(name, type, size);
            param.Value = value;

            return param;
        }

        public IDbDataParameter CreateParameter(string name, SqlDbType type, object value)
        {
            var param = new SqlCeParameter(name, type);
            param.Value = value ?? DBNull.Value;

            return param;
        }


    }
}
