using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.TaskTracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Summary", "TaskList");
        }

        public ActionResult Mock()
        {
            return View();
        }

        public ActionResult Mock2()
        {
            return View();
        }
    }
}
