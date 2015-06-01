using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.TaskTracker.Models.Bol
{
    public class AjaxResponse
    {
        public object Data { get; private set; }
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }

        private AjaxResponse(bool success, string errorMessage, object data)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
            this.Data = data;
        }

        public static AjaxResponse GetSuccess(object data)
        {
            return new AjaxResponse(true, null, data);
        }

        public static AjaxResponse GetError(string errorMessage)
        {
            return new AjaxResponse(false, errorMessage, null);
        }
    }
}