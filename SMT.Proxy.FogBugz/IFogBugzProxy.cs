using SMT.Proxy.FogBugz.FogBugzObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Proxy.FogBugz
{
    public interface IFogBugzRepository
    {
        Case GetCard(int caseId);


        //IEnumerable<Case> GetCardsForSprint(Sprint sprint);

        //IEnumerable<Sprint> GetSprints();

        //IEnumerable<WorkInterval> GetWorkIntervals(DateTime start, DateTime end, int userId);

        //IEnumerable<User> GetUsers();
    }
}
