using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.FogBugz.Json.ObjectInterfaces;

namespace SMT.Utilities.FogBugz.Json.ApiInterfaces
{
	public interface IFogbugzStatusApi
	{
		IFogbugzStatus[] ListStatuses(object[] inputs);
		IFogbugzStatus ViewStatus(object[] inputs);
	}
}
