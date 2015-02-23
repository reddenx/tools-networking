using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.TaskTracker.Models.Bll;
using Web.TaskTracker.Models.Bol;
using Web.TaskTracker.Models.ViewModels.TaskList;

namespace Web.TaskTracker.Controllers
{
    public class TaskListController : Controller
    {
        //
        // GET: /TaskList/

        public ActionResult Summary()
        {
            var builder = new TaskTreeBuilder();
            var items = builder.GetTaskTreeForAccount(1);
            var model = new TaskListViewModel() { RootItems = items };

            return View(model);
        }
    }
}
