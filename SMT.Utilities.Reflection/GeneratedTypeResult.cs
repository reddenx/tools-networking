using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Reflection
{
    public class GeneratedTypeResult<T>
    {
        public T InterceptedInstance { get; private set; }
        public Interceptor Interceptor { get; private set; }
        public Type GeneratedType { get; private set; }

        internal GeneratedTypeResult(T maskedType, Interceptor interceptor, Type generatedType)
        {
            this.InterceptedInstance = maskedType;
            this.Interceptor = interceptor;
            this.GeneratedType = generatedType;
        }
    }
}
