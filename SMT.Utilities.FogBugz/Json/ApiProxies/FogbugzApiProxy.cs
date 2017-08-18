using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.FogBugz.Json.ApiInterfaces;
using SMT.Utilities.FogBugz.Json.ObjectInterfaces;

namespace SMT.Utilities.FogBugz.Json.ApiProxies
{
	internal class FogbugzApiProxy : IFogbugzCaseApi
	{
		private FogbugzRequestor Requestor;

		internal FogbugzApiProxy(FogbugzRequestor requestor)
		{
			Requestor = requestor;
		}

		public IFogbugzCase[] ListCases(object[] inputs)
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase[] SearchCases(object[] inputs)
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase ViewCase(object[] inputs)
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase CreateCase(object caseData)
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase EditCase(object caseData)
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase AssignCase(object person)
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase ResolveCase()
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase ReactivateCase()
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase CloseCase()
		{
			throw new NotImplementedException();
		}

		public IFogbugzCase ReopenCase()
		{
			throw new NotImplementedException();
		}
	}
}
