using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SMT.Utilities.Sql.Interfaces;
using SMT.Utilities.Sql.SqlCe;
using Web.TaskTracker.Models.Bol;

namespace Web.TaskTracker.Models.Dal
{
    public class TaskRepository
    {
        private readonly ISqlQuerier Querier;

        public TaskRepository(ISqlQuerier querier = null)
        {
            this.Querier = querier ?? SqlCeQuerier.Get(DataConfiguration.TaskConnectionString);
        }

        public IEnumerable<TaskItem> GetTasksForAccount(int accountId)
        {
            var sql = @"TODO-SM";

            var parameters = new IDbDataParameter[] 
            {
                //TODO-SM I can't remember if this needs the @ prepended to it
                Querier.CreateParameter("AccountId", SqlDbType.Int, accountId),
            };

            return Querier.ExecuteReader((reader) =>
            {
                return new TaskItem(
                    Convert.ToInt32(reader["TaskId"]),
                    Convert.ToInt32(reader["AccountId"]),
                    reader["ParentTaskId"] == DBNull.Value ? null : new Nullable<int>(Convert.ToInt32(reader["ParentTaskId"])),
                    Convert.ToString(reader["Taskname"]),
                    (TaskStatus)Convert.ToInt32(reader["CurrentStatus"]),
                    Convert.ToDateTime(reader["DateCreated"]),
                    reader["DateCompleted"] == DBNull.Value ? null : new Nullable<DateTime>(Convert.ToDateTime(reader["DateCompleted"])));
            }, sql, parameters);
        }
    }
}