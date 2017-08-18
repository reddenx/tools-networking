using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.FogBugz.Json.ObjectInterfaces;

namespace SMT.Utilities.FogBugz.Json
{
	internal class FogbugzRequestor
	{
		private readonly string FogbugzBaseUrl;

		public FogbugzRequestor(string fogbugzBaseUrl)
		{
			FogbugzBaseUrl = fogbugzBaseUrl;
		}

		internal FogbugzApiReponse<T> Request<T>(object requestPayload)
		{
			/* all fogbugz requests have a cmd field
			 * if files are being uploaded, muse use enctype="multipart/form-data" POST
			 * all requests/responses are UTF-8
			 * all dates should be ISO 8601 UTC */

			var request = WebRequest.Create(FogbugzBaseUrl);


			throw new NotImplementedException();
		}
	}

	internal class FogbugzApiReponse<T>
	{
		internal T ResponseData;
		internal bool Success;
		internal HttpStatusCode RequestStatusCode;
		internal FogbugzApiErrorCodes FogbugzApiErrorCode;
	}

	internal class RawFogbugzApiResponse<T>
	{
		public T Data;
		public RawFogbugzApiError Errors;
	}

	internal class RawFogbugzApiError
	{
		public string Message;
		public string Detail;
		public string Code;
	}
}
