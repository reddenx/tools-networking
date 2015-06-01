using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.TaskTracker.Models.Bll;
using Web.TaskTracker.Models.Bol;
using Web.TaskTracker.Models.ViewModels;
using Web.TaskTracker.Models.ViewModels.TaskList;

namespace Web.TaskTracker.Controllers
{
    public class TaskListController : Controller
    {
        //
        // GET: /TaskList/
        private TaskManager _taskMgr;
        private TaskManager TaskMgr
        {
            get { return _taskMgr ?? (_taskMgr = TaskManager.Get()); }
        }

        //view
        public ViewResult Summary()
        {
            return View();
        }

        //ajax
        public JsonResult GetTreeItems(int accountId)
        {
            var items = TaskMgr.GetTaskTreeForAccount(accountId);

            return Json(AjaxResponse.GetSuccess(items));
        }

        public JsonResult SaveTask(SaveTaskInput input)
        {
            try
            {
                if (!input.TaskId.HasValue)
                {
                    //TODO-SM hardcoded accountId here
                    var task = TaskMgr.CreateTask(input.TaskName, 1, input.ParentTaskId, input.Description, input.CurrentStatus); //TODO-SM hardCoded id
                    return Json(AjaxResponse.GetSuccess(task));
                }
                else
                {
                    var task = TaskMgr.UpdateTask(input.TaskId.Value, input.TaskName, input.CurrentStatus, input.Description);
                    return Json(AjaxResponse.GetSuccess(task));
                }
            }
            catch(Exception e)
            {
                return Json(AjaxResponse.GetError(e.Message));
            }
        }

        public JsonResult GetAllChildrenForTask(int taskId)
        {
            var tasks = TaskMgr.GetEntireChildTreeForTask(taskId);
            return Json(AjaxResponse.GetSuccess(tasks));
        }
    }
}
