using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.TaskTracker.Models.Bol;
using Web.TaskTracker.Models.Dal;

namespace Web.TaskTracker.Models.Bll
{
    public class TaskManager
    {
        private readonly TaskRepository TaskRepo;

        public TaskManager(TaskRepository taskRepository = null)
        {
            TaskRepo = taskRepository ?? new TaskRepository();
        }

        public TaskTreeItem[] GetTaskTreeForAccount(int accountId)
        {
            var tasks = TaskRepo.GetTasksForAccount(accountId);

            var root = new TaskTreeItem(null);
            BuildTree(root, tasks.ToArray());

            return root.Children;
        }

        public TaskItem CreateTask(string taskName, int accountId, int? parentTaskId, string description)
        {
            var taskId = TaskRepo.CreateTask(taskName, accountId, parentTaskId, description);
            return TaskRepo.GetTaskById(taskId);
        }

        public TaskItem UpdateTask(int taskId, string taskName, TaskStatus currentStatus, string description)
        {
            TaskRepo.UpdateTask(taskId, taskName, currentStatus, description);
            return TaskRepo.GetTaskById(taskId);
        }

        private TaskItem[] BuildTree(TaskTreeItem parent, TaskItem[] items)
        {
            var newChildren = new List<TaskTreeItem>();
            foreach (var item in items)
            {
                if ((parent.Item != null && item.ParentTask.Value == parent.Item.TaskId)
                    || (parent.Item == null && !item.ParentTask.HasValue))
                {
                    var treeItem = new TaskTreeItem(item);
                    newChildren.Add(treeItem);
                }
            }
            parent.Children = newChildren.ToArray();

            var leftovers = new List<TaskItem>(items);
            foreach (var child in parent.Children)
            {
                leftovers.RemoveAll(item => item.TaskId == child.Item.TaskId);
            }

            var postChildrenLeftovers = new List<TaskItem>();
            foreach (var child in parent.Children)
            {
                postChildrenLeftovers.AddRange(BuildTree(child, leftovers.ToArray()));
            }
            return postChildrenLeftovers.ToArray();
        }

        public TaskItem GetTask(int taskId)
        {
            return TaskRepo.GetTaskById(taskId);
        }
    }
}