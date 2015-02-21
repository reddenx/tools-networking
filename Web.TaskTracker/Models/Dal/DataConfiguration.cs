using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Web.TaskTracker.Models.Dal
{
    public static class DataConfiguration
    {
        public static string TaskConnectionString { get { return ConfigurationManager.ConnectionStrings["TaskConnectionString"].ConnectionString; } }
    }
}