using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.TaskTracker.Models.Bol
{
    public class TaskItem
    {
        public int TaskId { get; private set; }
        public int AccountId { get; private set; }
        public int? ParentTask { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TaskStatus CurrentStatus { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateCompleted { get; private set; }

        public TaskItem(
            int taskId,
            int accountId,
            int? parentTask,
            string name,
            string description,
            TaskStatus currentStatus,
            DateTime dateCreated,
            DateTime? dateCompleted)
        {
            this.TaskId = taskId;
            this.AccountId = accountId;
            this.ParentTask = parentTask;
            this.Name = name;
            this.Description = description;
            this.CurrentStatus = currentStatus;
            this.DateCreated = dateCreated;
            this.DateCompleted = dateCompleted;
        }
    }
}