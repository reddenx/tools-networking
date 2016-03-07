using SMT.Utilities.Sql.Interfaces;
using SMT.Utilities.Sql.ObjectMapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Sql.TSql
{
    internal class SqlQuerier : ISqlQuerier
    {
        private readonly string ConnectionString;

        public SqlQuerier(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public T[] ExecuteReader<T>(string sql) where T : new()
        {
            return ExecuteReader<T>(sql, new IDbDataParameter[] { });
        }

        public T[] ExecuteReader<T>(string sql, IDbDataParameter[] parameters) where T : new()
        {
            var type = typeof(T);
            if (!type.GetCustomAttributes(typeof(DBObjectAttribute), false).Any())
            {
                throw new ArgumentException(string.Format("Output type is not marked with DBObject attribute, type: {0}", type.FullName));
            }

            //get list of attributed properties
            var properties = type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(DBColumnAttribute), false).Any());
            //get list of attributed fields
            var fields = type.GetFields().Where(fi => fi.GetCustomAttributes(typeof(DBColumnAttribute), false).Any());

            var items = ExecuteReader<T>(sql, parameters,
                r =>
                {
                    var item = new T();
                    foreach (var property in properties)
                    {
                        var attribute = property.GetCustomAttributes(typeof(DBColumnAttribute), false).First() as DBColumnAttribute;
                        var dbValue = Convert.ChangeType(r[attribute.ColumnName], property.PropertyType);
                        property.SetValue(item, dbValue);
                    }

                    foreach (var field in fields)
                    {
                        var attribute = field.GetCustomAttributes(typeof(DBColumnAttribute), false).First() as DBColumnAttribute;
                        var dbValue = Convert.ChangeType(r[attribute.ColumnName], field.FieldType);
                        field.SetValue(item, dbValue);
                    }

                    return item;
                });

            return items;
        }

        public T[] ExecuteReader<T>(string sql, BuildObjectFromReader<T> getObjectFromRecord)
        {
            return ExecuteReader<T>(sql, new IDbDataParameter[] { }, getObjectFromRecord);
        }

        public T[] ExecuteReader<T>(string sql, IDbDataParameter[] parameters, BuildObjectFromReader<T> getObjectFromRecord)
        {
            var data = new List<T>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = BuildCommand(sql, parameters, connection);

                connection.Open();

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(getObjectFromRecord(reader));
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return data.ToArray();
        }

        public int InsertAndGetIdentity(string sql, IDbDataParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = BuildCommand(sql, parameters, connection);

                SqlCommand command2 = new SqlCommand(@"select SCOPE_IDENTITY()", connection);
                object identity = null;

                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                    identity = command2.ExecuteScalar();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }

                return Convert.ToInt32(identity);
            }
        }

        public int ExecuteNonQuery(string sql, IDbDataParameter[] parameters)
        {
            int linesModified = -1;
            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = BuildCommand(sql, parameters, connection);

                connection.Open();

                try
                {
                    linesModified = command.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return linesModified;
        }

        private SqlCommand BuildCommand(string sql, IDbDataParameter[] parameters, SqlConnection connection)
        {
            var command = new SqlCommand(sql, connection);

            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        public IDbDataParameter CreateParameter(string name, SqlDbType type, object value)
        {
            var param = new SqlParameter(name, type);
            param.Value = value;

            return param;
        }

        public IDbDataParameter CreateParameter(string name, SqlDbType type, int size, object value)
        {
            var param = new SqlParameter(name, type, size);
            param.Value = value;

            return param;
        }
    }
}
