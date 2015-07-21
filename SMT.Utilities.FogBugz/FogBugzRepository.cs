using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SMT.Utilities.FogBugz.FogBugzObjects;
using SMT.Utilities.FogBugz.Web;

namespace SMT.Utilities.FogBugz
{
    public interface IFogBugzRepository
    {
        ICase[] SearchCases(string queryString, int maxAmount);

        ITimeInterval[] GetIntervalsForCase(int caseId);
        ITimeInterval[] GetIntervalsForCase(int caseId, int userId);
        ITimeInterval[] GetIntervalsForDates(DateTime start, DateTime end);
        ITimeInterval[] GetIntervalsForDates(DateTime start, DateTime end, int userId);

        IMilestone[] GetAllMilestones();

        //IUser[] GetUsers();
    }

    public class FogBugzRepository : IFogBugzRepository
    {
        private readonly string SecurityToken;
        private readonly string BaseUrl;
        private FogBugzRequester Requester;

        public FogBugzRepository(string securityToken, string baseUrl)
        {
            SecurityToken = securityToken;
            BaseUrl = baseUrl;
            Requester = new FogBugzRequester(baseUrl, SecurityToken);
        }

        public ICase[] SearchCases(string queryString, int maxAmount)
        {
            var arguments = new Dictionary<string, string>()
            {
                {"q", queryString},
                {"cols", string.Join(",",XmlUtilities.GetXmlElementNamesForType(typeof(Case)))},
                {"max", maxAmount.ToString()}
            };

            var response = Requester.MakeRequest<CaseResponseRoot>("search", arguments);

            if (response.Success)
            {
                return response.Data.CaseList.Cases;
            }
            return null;
        }

        private ITimeInterval[] GetIntervals(Dictionary<string, string> arguments)
        {
            var response = Requester.MakeRequest<TimeIntervalResponseRoot>("listIntervals", arguments);
            if (response.Success)
            {
                return response.Data.IntervalList.Intervals;
            }
            return null;
        }

        public ITimeInterval[] GetIntervalsForCase(int caseId)
        {
            var arguments = new Dictionary<string, string>()
            {
                {"ixBug", caseId.ToString()}
            };

            return GetIntervals(arguments);
        }

        public ITimeInterval[] GetIntervalsForCase(int caseId, int userId)
        {
            var arguments = new Dictionary<string, string>()
            {
                {"ixBug", caseId.ToString()},
                {"ixPerson", userId.ToString()},
            };

            return GetIntervals(arguments);
        }

        public ITimeInterval[] GetIntervalsForDates(DateTime start, DateTime end)
        {
            var arguments = new Dictionary<string, string>()
            {
                {"dtStart", start.ToString("o")},
                {"dtEnd", end.ToString("o")},
                {"ixPerson", "1"}
            };

            return GetIntervals(arguments);
        }

        public ITimeInterval[] GetIntervalsForDates(DateTime start, DateTime end, int userId)
        {
            var arguments = new Dictionary<string, string>()
            {
                {"dtStart", start.ToString("o")},
                {"dtEnd", end.ToString("o")},
                {"ixPerson", userId.ToString()},
            };

            return GetIntervals(arguments);
        }

        public IMilestone[] GetAllMilestones()
        {
            var arguments = new Dictionary<string, string>();

            var response = Requester.MakeRequest<MilestoneResponseRoot>("listFixFors", arguments);

            if (response.Success)
            {
                return response.Data.MilestoneList.Milestones;
            }
            return null;
        }
    }
}
