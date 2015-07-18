using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Proxy.FogBugz.FogBugzObjects;

namespace SMT.Proxy.FogBugz
{
    public interface IFogBugzRepository
    {
        Case GetCaseById(int taskId);
        IEnumerable<Case> GetCasesForBranch(string branchName);
        IEnumerable<Case> GetCasesForSprint(string sprintName);
        IEnumerable<Case> GetCasesWorkedOn(int userId, DateTime beginDate, DateTime endDate);
    }
}
