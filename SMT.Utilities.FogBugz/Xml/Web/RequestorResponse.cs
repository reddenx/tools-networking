using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.FogBugz.Web
{
    internal class RequestorResponse<T>
        where T : class
    {
        public bool Success;
        public string ErrorMessage;
        public T Data;

        private RequestorResponse(bool success, T responseData, string errorMessage)
        {
            Success = success;
            Data = responseData;
            ErrorMessage = errorMessage;
        }

        public static RequestorResponse<T> GetSuccess(T data)
        {
            return new RequestorResponse<T>(true, data, string.Empty);
        }

        public static RequestorResponse<T> GetFailure(string errorMessage)
        {
            return new RequestorResponse<T>(false, null, errorMessage);
        }
    }
}
