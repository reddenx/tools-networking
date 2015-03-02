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
            get { return _taskMgr ?? (_taskMgr = new TaskManager()); }
        }

        public ActionResult Summary()
        {
            var builder = new TaskManager();
            var items = builder.GetTaskTreeForAccount(1);
            var model = new TaskListViewModel() { RootItems = items };

            return View(model);
        }

        public JsonResult SaveTask(SaveTaskInput input)
        {
            TaskItem task;

            try
            {
                if (!input.TaskId.HasValue)
                {
                    task = TaskMgr.CreateTask(input.TaskName, 1, input.ParentTaskId, input.Description); //TODO-SM hardCoded id
                }
                else
                {
                    task = TaskMgr.UpdateTask(input.TaskId.Value, input.TaskName, input.CurrentStatus, input.Description);
                }
            }
            catch(Exception e)
            {
                return Json(new AjaxResult(
                    success: false,
                    error: e.Message), JsonRequestBehavior.AllowGet);
            }

            var result = new AjaxResult(
                success: true,
                data: task);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
