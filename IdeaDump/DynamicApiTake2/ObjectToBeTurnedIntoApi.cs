using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaDump.DynamicApiTake2
{
    class ObjectToBeTurnedIntoApi : IContract
    {
        public void MethodOne()
        {
            throw new NotImplementedException();
        }

        public ExampleDto MethodTwo(InputDto input)
        {
            throw new NotImplementedException();
        }
    }
}
