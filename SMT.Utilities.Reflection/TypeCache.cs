using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Utilities.Reflection
{
    internal static class TypeCache
    {
        /// <summary>
        /// builds an interceptor around a class for caching purposes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T Intercept<T>(T instance) where T : class
        {
            var type = BuildType<T>();
            var newWrapper = Activator.CreateInstance(type, args: new object[] { typeof(T), instance });
            return newWrapper as T;
        }

        private static Type BuildType<T>()
        {
            var inputType = typeof(T);

            if (!inputType.IsInterface)
                throw new NotImplementedException();

            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                name: Assembly.GetAssembly(typeof(T)).GetName(),
                access: AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("TypeInterception");
            var typeBuilder = moduleBuilder.DefineType($"TypeInterception.{inputType.Name}");

            var baseType = typeof(InterceptBaseType);
            typeBuilder.AddInterfaceImplementation(inputType);
            typeBuilder.SetParent(baseType);

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(Type), inputType });
            var ctorIlGen = constructorBuilder.GetILGenerator();

            //build constructor
            var baseConstructor = baseType.GetConstructors()[0];
            ctorIlGen.Emit(OpCodes.Ldarg, 0);
            ctorIlGen.Emit(OpCodes.Ldarg, 1);
            ctorIlGen.Emit(OpCodes.Ldarg, 2);
            ctorIlGen.Emit(OpCodes.Call, baseConstructor);
            ctorIlGen.Emit(OpCodes.Ret);

            var attemptCallMethod = baseType.GetMethod("AttemptCall");
            var passthroughCall = baseType.GetMethod("PassThrough");

            //build up methods
            var methods = inputType.GetMethods();
            for (int j = 0; j < methods.Length; ++j)
            {
                var method = methods[j];

                var newMethod = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, method.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
                var ilGenerator = newMethod.GetILGenerator();

                var inputParameters = method.GetParameters();


                //build object array
                var array = ilGenerator.DeclareLocal(typeof(object[]));

                ilGenerator.Emit(OpCodes.Ldc_I4, inputParameters.Length);
                ilGenerator.Emit(OpCodes.Newarr, typeof(object));
                ilGenerator.Emit(OpCodes.Stloc, array);

                for (int i = 0; i < inputParameters.Length; ++i)
                {
                    ilGenerator.Emit(OpCodes.Ldloc, array);
                    ilGenerator.Emit(OpCodes.Ldc_I4, i);
                    ilGenerator.Emit(OpCodes.Ldarg, i + 1);
                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                }

                //make method call
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldloc, array);
                ilGenerator.Emit(OpCodes.Ldc_I4, j);

                if (method.ReturnType != typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Callvirt, attemptCallMethod);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Callvirt, passthroughCall);
                }

                ilGenerator.Emit(OpCodes.Ret);
            }

            return typeBuilder.CreateType();
        }

        public class InterceptBaseType
        {
            private readonly Dictionary<string, CachedMethodCall> CachedOutput = new Dictionary<string, CachedMethodCall>();

            private Type TargetType;
            private object Instance;

#warning there's the possibility of losing the reference if the IL doesn't increment the reference counter.... maybe? 

            public InterceptBaseType(Type targetType, object instance)
            {
                TargetType = targetType;
                Instance = instance;
            }

            public void PassThrough(object[] parameters, int methodIndex)
            {
                var method = TargetType.GetMethods()[methodIndex];
                method.Invoke(Instance, parameters);
            }

            public object AttemptCall(object[] parameters, int methodIndex)
            {
                var method = TargetType.GetMethods()[methodIndex];

                var key = GetCacheKey(method.Name, parameters);
                if (CachedOutput.ContainsKey(key))
                {
                    var cacheItem = CachedOutput[key];
                    if (cacheItem.Expires < DateTime.Now)
                    {
                        CachedOutput.Remove(key);
                    }
                    else
                    {
                        return cacheItem.CachedResult;
                    }
                }

                var result = method.Invoke(Instance, parameters);
                CachedOutput.Add(key, new CachedMethodCall() { CachedResult = result, Expires = DateTime.Now.AddMinutes(15) });
                return result;
            }

            private string GetCacheKey(string methodName, object[] parameters)
            {
                var hash = string.Join("_", parameters);
                return $"{methodName}_{hash}";
            }

            private class CachedMethodCall
            {
                public DateTime Expires;
                public object CachedResult;
            }
        }


    }
}
