using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.FogBugz.Json.ObjectInterfaces;

namespace SMT.Utilities.FogBugz.Json.ApiInterfaces
{
	public interface IFogbugzUserApi
	{
		IFogbugzUser[] ListUsers(object[] inputs);
		IFogbugzUser CreateUser(object[] inputs);
		IFogbugzUser ViewUser(object[] inputs);
		IFogbugzUser EditUser(object[] inputs);
	}
}
