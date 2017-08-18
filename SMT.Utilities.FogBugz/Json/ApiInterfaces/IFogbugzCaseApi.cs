using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.FogBugz.Json.ObjectInterfaces;

namespace SMT.Utilities.FogBugz.Json.ApiInterfaces
{
	public interface IFogbugzCaseApi
	{
		IFogbugzCase[] ListCases(object[] inputs);
		IFogbugzCase[] SearchCases(object[] inputs);
		IFogbugzCase ViewCase(object[] inputs);
		IFogbugzCase CreateCase(object caseData);
		IFogbugzCase EditCase(object caseData);
		IFogbugzCase AssignCase(object person);
		IFogbugzCase ResolveCase();
		IFogbugzCase ReactivateCase();
		IFogbugzCase CloseCase();
		IFogbugzCase ReopenCase();
	}
}
