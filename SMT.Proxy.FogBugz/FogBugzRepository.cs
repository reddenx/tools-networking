using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SMT.Proxy.FogBugz.FogBugzObjects;

namespace SMT.Proxy.FogBugz
{
    //should be internal and used by the bll 
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

        public Case GetCaseById(int taskId)
        {
            var arguments = new Dictionary<string, string>()
            {
                {"q", taskId.ToString()}
            };

            var response = Requester.MakeRequest<Case>("search", arguments);

            return response;
        }

        public IEnumerable<Case> GetCasesForBranch(string branchName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Case> GetCasesForSprint(string sprintName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Case> GetCasesWorkedOn(int userId, DateTime beginDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}
