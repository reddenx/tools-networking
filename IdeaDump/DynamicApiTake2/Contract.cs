using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaDump.DynamicApiTake2
{
    [ContractRoute("api")]
    interface IContract
    {
        [ContractRoute("method1")]
        void MethodOne();

        [ContractRoute("method2")]
        ExampleDto MethodTwo(InputDto input);
    }

    struct ExampleDto { }
    struct InputDto { }
}
