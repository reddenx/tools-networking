using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.FogBugz.Json.ObjectInterfaces;

namespace SMT.Utilities.FogBugz.Json.ApiInterfaces
{
	public interface IFogbugzProjectApi
	{
		IFogbugzProject[] ListProjects(object[] inputs);
		IFogbugzProject ViewProject(object[] inputs);
		IFogbugzProject CreateProject(object[] inputs);

		IFogbugzArea[] ListAreas(object[] inputs);
		IFogbugzArea ViewArea(object[] inputs);
		IFogbugzArea CreateArea(object[] inputs);
	}
}
