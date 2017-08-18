using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.FogBugz.Json.ApiInterfaces
{
	public interface IFogbugzLoginApi
	{
		IFogbugzToken Login(object[] inputs);
		void Logoff(object token);
	}
}
