using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.TaskTracker.Models.ViewModels
{
    public class AjaxResult
    {
        public bool succes { get; set; }
        public string error { get; set; }
        public object data { get; set; }

        public AjaxResult(bool success = false, string error = null, object data = null)
        {
            this.succes = succes;
            this.error = error;
            this.data = data;
        }
    }
}