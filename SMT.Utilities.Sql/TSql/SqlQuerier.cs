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

        public T[] ExecuteReader<T>(string sql, params IDbDataParameter[] parameters) where T : new()
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

            var items = ExecuteReader<T>(sql,
                r =>
                {
                    var item = new T();
                    foreach (var property in properties)
                    {
                        var attribute = property.GetCustomAttributes(typeof(DBColumnAttribute), false).First() as DBColumnAttribute;
                        object dbValue = ConvertFromDbObject(r[attribute.ColumnName], property.PropertyType);

                        property.SetValue(item, dbValue);
                    }

                    foreach (var field in fields)
                    {
                        var attribute = field.GetCustomAttributes(typeof(DBColumnAttribute), false).First() as DBColumnAttribute;
                        object dbValue = ConvertFromDbObject(r[attribute.ColumnName], field.FieldType);

                        field.SetValue(item, dbValue);
                    }

                    return item;
                }, parameters);

            return items;
        }

        //fuckin woo enums!
        private object ConvertFromDbObject(object dbObject, Type destinationType)
        {
            //convert dbnull to null
            var nullsafeValue = dbObject != DBNull.Value ? dbObject : null;
            var nullableType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
            object dbValue;

            //if it's output is an enum and it's input is a string we can parse it
            if (nullableType.IsEnum && dbObject is string)
            {
                dbValue = Enum.Parse(nullableType, nullsafeValue as string, true);
            }
            //if it's output is an enum and it's input is an int
            else if (nullableType.IsEnum && dbObject is int)
            {
                dbValue = Enum.ToObject(nullableType, dbObject);
            }
            //if it's output is a value type and it's input is null, create a default
            else if (destinationType.IsValueType && nullsafeValue == null)
            {
                dbValue = Activator.CreateInstance(destinationType);
            }
            //if it's none of those other scenarios, do a nullable base type conversion from db
            else
            {
                dbValue = Convert.ChangeType(nullsafeValue, nullableType);
            }

            return dbValue;
        }

        public T[] ExecuteReader<T>(string sql, BuildObjectFromReader<T> getObjectFromRecord, params IDbDataParameter[] parameters)
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

        public T ExecuteScalar<T>(string sql, params IDbDataParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = BuildCommand(sql, parameters, connection);
                connection.Open();
                try
                {
                    return (T)ConvertFromDbObject(command.ExecuteScalar(), typeof(T));
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
        }

        public int InsertAndGetIdentity(string sql, params IDbDataParameter[] parameters)
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

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] parameters)
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
            if (value == null)
            {
                value = DBNull.Value;
            }
            param.Value = value;

            return param;
        }

        public IDbDataParameter CreateParameter(string name, SqlDbType type, string customTypeName, object value)
        {
            var param = new SqlParameter(name, type);
            param.Value = value ?? DBNull.Value;
            param.TypeName = customTypeName;
            return param;
        }

        public IDbDataParameter CreateParameter(string name, SqlDbType type, int size, object value)
        {
            var param = new SqlParameter(name, type, size);
            if (value == null)
            {
                value = DBNull.Value;
            }
            param.Value = value;

            return param;
        }
    }
}
