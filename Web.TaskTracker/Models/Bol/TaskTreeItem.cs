using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.TaskTracker.Models.Bol
{
    public class TaskTreeItem
    {
        public TaskTreeItem[] Children;
        public TaskItem Item;

        public TaskTreeItem(TaskItem item)
        {
            this.Item = item;
        }
    }
}