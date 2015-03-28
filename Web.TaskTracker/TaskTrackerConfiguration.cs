using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMT.Utilities.Configuration;

namespace Web.TaskTracker
{
    public class TaskTrackerConfiguration : ConfigurationBase
    {
        [ConnectionString("TaskConnectionString")]
        public string TaskConnectionString;

    }
}