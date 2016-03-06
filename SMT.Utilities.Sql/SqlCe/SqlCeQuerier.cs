﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.Sql.Interfaces;
using SMT.Utilities.Sql.ObjectMapping;

namespace SMT.Utilities.Sql.SqlCe
{
    internal class SqlCeQuerier : ISqlQuerier
    {
        private readonly string ConnectionString;

        internal SqlCeQuerier(string connectionString)
        {
            this.ConnectionString = connectionString;
        }


        public T[] ExecuteReader<T>(string sql)
            where T : new()
        {
            return ExecuteReader<T>(sql, new IDbDataParameter[] { });
        }

        public T[] ExecuteReader<T>(string sql, IDbDataParameter[] parameters)
            where T : new()
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
            return ExecuteReader(sql, new IDbDataParameter[] { }, getObjectFromRecord);
        }

        public T[] ExecuteReader<T>(string sql, IDbDataParameter[] parameters, BuildObjectFromReader<T> getObjectFromRecord)
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

                connection.Close();

                return Convert.ToInt32(identity);
            }
        }

        public int ExecuteNonQuery(string sql, IDbDataParameter[] parameters)
        {
            int linesModified = -1;
            using (var connection = new SqlCeConnection(ConnectionString))
            {
                var command = BuildCommand(sql, parameters, connection);

                connection.Open();

                linesModified = command.ExecuteNonQuery();

                connection.Close();
            }

            return linesModified;
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
