using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaDump.DynamicApiTake2
{
    class ObjectThatWantsToUseApi
    {
        public void SomeMethod()
        {
            var proxyClient = ClientFactory.BuildProxy<IContract>("http://dynamicapi.domain.com");
            var result = proxyClient.MethodTwo(new InputDto { });
        }
    }
}
