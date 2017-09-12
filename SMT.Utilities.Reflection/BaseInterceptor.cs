using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Reflection
{
    public class BaseInterceptor : Interceptor
    {
        private readonly Dictionary<int, InterceptMethod> MethodImplementations;
        private readonly Type TargetType;

        public BaseInterceptor(Type targetType)
        {
            MethodImplementations = new Dictionary<int, InterceptMethod>();
            TargetType = targetType;
        }

        public void Action(object[] inputs, int methodIndex)
        {
            if (MethodImplementations.ContainsKey(methodIndex))
            {
                var result = MethodImplementations[methodIndex](inputs);
                //eat the result if there is one, we have...
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public object Func(object[] inputs, int methodIndex)
        {
            if (MethodImplementations.ContainsKey(methodIndex))
            {
                var result = MethodImplementations[methodIndex](inputs);
                return result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void SetImplementation(string methodName, Type[] parameters, InterceptMethod methodBody)
        {
            //search method
            var targetMethod = TargetType.GetMethod(methodName, parameters);
            if (targetMethod != null)
            {
                var targetMethods = TargetType.GetMethods();
                for (int i = 0; i < targetMethods.Length; ++i)
                {
                    if (targetMethods[i] == targetMethod)
                    {
                        if (MethodImplementations.ContainsKey(i))
                        {
                            throw new ArgumentException("The method specified already has an implementation");
                        }
                        else
                        {
                            MethodImplementations.Add(i, methodBody);
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException($"could not find method {methodName} with inputs {string.Join(",", parameters.Select(t => t.Name))}");
            }
        }

        public void SetImplementation<T>(string methodName, T implementation)
        {
            var delgenate = implementation as Delegate;
            if (delgenate == null)
                throw new ArgumentException("implementation must be a delegate");

            var inputTypes = delgenate.Method.GetParameters().Select(p => p.ParameterType).ToArray();
            this.SetImplementation(methodName, inputTypes, inputs => delgenate.DynamicInvoke(inputs));
        }
    }
}
