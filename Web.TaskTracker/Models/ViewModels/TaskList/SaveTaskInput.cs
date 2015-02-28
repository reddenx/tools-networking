using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.TaskTracker.Models.Bol;

namespace Web.TaskTracker.Models.ViewModels.TaskList
{
    public class SaveTaskInput
    {
        public int? TaskId { get; set; }
        public int? ParentTaskId { get; set; }
        public string TaskName { get; set; }
        public TaskStatus CurrentStatus { get; set; }
    }
}