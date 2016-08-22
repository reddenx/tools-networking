using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace App.WebTesting.Controllers
{
    public class DynamicApiBaseController : ApiController
    {
		public object RunDynamic()
        {
            return SMT.Utilities.DynamicApi.Api.DynamicApiBaseController.RunDynamic(
                HttpContext.Current.Request.RequestContext.RouteData.Values,
                this.Request.Content.ReadAsStringAsync().Result);
        }
    }
}