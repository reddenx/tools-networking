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
            return BuildBlanks();



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

        private IEnumerable<TaskItem> BuildBlanks()
        {
            return new TaskItem[] 
            {
                new TaskItem(0,  1, null, "testr1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(1,  1, null, "testr2", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(2,  1, null, "testr3", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(3,  1, null, "testr4", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(4,  1, null, "testr5", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(5,  1, null, "testr6", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(6,  1, 1, "testr1_1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(7,  1, 1, "testr1_2", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(8,  1, 1, "testr1_3", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(9,  1, 2, "testr2_1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(10, 1, 2, "testr2_2", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(11, 1, 6, "testr1_1_1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(12, 1, 6, "testr1_1_2", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(13, 1, 11, "testr1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(14, 1, 11, "testr1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(15, 1, 13, "testr1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(16, 1, null, "testr1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(17, 1, null, "testr1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(18, 1, null, "testr1", TaskStatus.Complete, DateTime.Now, null),
                new TaskItem(19, 1, null, "testr1", TaskStatus.Complete, DateTime.Now, null),
            };
        }
    }
}