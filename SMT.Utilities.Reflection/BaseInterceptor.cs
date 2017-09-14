using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Reflection
{
    public class BaseInterceptor : Interceptor
    {
        private readonly Dictionary<int, InterceptMethod> MethodImplementations;
        private readonly Type TargetType;
        private readonly object Instance;

        public BaseInterceptor(Type targetType, object instance)
        {
            MethodImplementations = new Dictionary<int, InterceptMethod>();
            TargetType = targetType;
            Instance = instance;
        }

        public void Action(object[] inputs, int methodIndex)
        {
            if (MethodImplementations.ContainsKey(methodIndex))
            {
                var result = MethodImplementations[methodIndex](inputs);
                //eat the result if there is one, we have...
            }
            else if (Instance != null && TargetType.GetMethods().Length > methodIndex)
            {
                var method = TargetType.GetMethods()[methodIndex];
                var result = method.Invoke(Instance, inputs);
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
            else if (Instance != null && TargetType.GetMethods().Length > methodIndex)
            {
                var method = TargetType.GetMethods()[methodIndex];
                var result = method.Invoke(Instance, inputs);
                return result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void SetMethodImplementation(string methodName, Type[] parameters, InterceptMethod methodBody)
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

        public void SetMethodImplementation<T>(string methodName, T implementation)
        {
            var delgenate = implementation as Delegate;
            if (delgenate == null)
                throw new ArgumentException("implementation must be a delegate");

            var inputTypes = delgenate.Method.GetParameters().Select(p => p.ParameterType).ToArray();
            this.SetMethodImplementation(methodName, inputTypes, inputs => delgenate.DynamicInvoke(inputs));
        }

        public void SetSetPropertyImplementation(string propertyName, InterceptPropertySet setMethod)
        {
            var targetProperty = TargetType.GetProperty(propertyName);
            SetMethodImplementation($"set_{propertyName}", new[] { targetProperty?.PropertyType },
                (input) =>
            {
                setMethod(input[0]);
                return null;
            });
        }

        public void SetGetPropertyImplementation(string propertyName, InterceptPropertyGet getMethod)
        {
            SetMethodImplementation($"get_{propertyName}", new Type[] { },
                (input) =>
            {
                return getMethod();
            });
        }

        private int? GetPropertyIndex(PropertyInfo property)
        {
            var allProperties = TargetType.GetProperties();
            for (int j = 0; j < allProperties.Length; ++j)
            {
                if (allProperties[j] == property)
                {
                    return j;
                }
            }
            return null;
        }
    }
}
