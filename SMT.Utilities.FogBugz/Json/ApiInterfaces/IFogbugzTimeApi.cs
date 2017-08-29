using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.FogBugz.Json.ObjectInterfaces;

namespace SMT.Utilities.FogBugz.Json.ApiInterfaces
{
	public interface IFogbugzTimeApi
	{
		void StartWork(object caseId);
		void StopWork();
		object CreateInterval(object[] inputs);
		IFogbugzTimeInterval ViewInterval(object[] inputs);
		IFogbugzTimeInterval[] ListIntervals(object[] inputs);
		IFogbugzTimeInterval EditInterval(object[] inputs);
		void DeleteInterval(object[] inputs);
	}
}
