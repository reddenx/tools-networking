using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.FogBugz.Json.ObjectInterfaces;

namespace SMT.Utilities.FogBugz.Json.ApiInterfaces
{
	public interface IFogbugzMilestoneApi
	{
		IFogbugzMilestone[] ListMilestones(object[] inputs);
		IFogbugzMilestone ViewMilestone(object[] inputs);
		IFogbugzMilestone CreateMilestone(object[] inputs);
		IFogbugzMilestone EditMilestone(object[] inputs);
		void AddMilestoneDependency(object milestone, object dependency);
		void RemoveMilestoneDependency(object milestone, object dependency);
	}
}
