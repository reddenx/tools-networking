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
    public static class TypeInterceptor
    {
        //what I want out of this
        //a masked type to hand out and
        //an interceptor interface to handle calls
        public static GeneratedTypeResult<MaskedType> BuildInterceptType<MaskedType>()
        {
            var inputType = typeof(MaskedType);

            //TODO, is this necessary?
            if (!inputType.IsInterface)
                throw new NotImplementedException();

            //get and define a home for this new type
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                name: Assembly.GetAssembly(typeof(MaskedType)).GetName(),
                access: AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("TypeInterception");
            var typeBuilder = moduleBuilder.DefineType($"TypeInterception.{inputType.Name}.{Guid.NewGuid()}");

            //setup the heirarchy, parent is the target type, base type is the interceptor
            var baseType = typeof(BaseInterceptor);
            typeBuilder.AddInterfaceImplementation(inputType);
            typeBuilder.SetParent(baseType);

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(Type) });
            var ctorIlGen = constructorBuilder.GetILGenerator();

            //build constructor
            var baseConstructor = baseType.GetConstructors()[0];
            ctorIlGen.Emit(OpCodes.Ldarg, 0); //load this (this keyword is always arg0 in an instanced object)
            ctorIlGen.Emit(OpCodes.Ldarg, 1); //load argument 1 (Type targetType)
            ctorIlGen.Emit(OpCodes.Call, baseConstructor); //make the call
            ctorIlGen.Emit(OpCodes.Ret); //return

            //get the methods we want to target in the base type
            var methodWithReturn = baseType.GetMethod("Func");
            var methodWithoutReturn = baseType.GetMethod("Action");

            //build up methods
            var methods = inputType.GetMethods();
            for (int j = 0; j < methods.Length; ++j)
            {
                var method = methods[j];

                //define a new method based on the target method and prime the ilgeneration
                var newMethod = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, method.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
                var ilGenerator = newMethod.GetILGenerator();
                var inputParameters = method.GetParameters();


                //the purpose of this IL is to take the argument list and turn it into an object array to be consumed by the interceptor's Action and Func methods;
                var array = ilGenerator.DeclareLocal(typeof(object[])); //declare a local array of type object

                ilGenerator.Emit(OpCodes.Ldc_I4, inputParameters.Length); //load arbitrary int of array length
                ilGenerator.Emit(OpCodes.Newarr, typeof(object)); //new up array of type object, consumes the int
                ilGenerator.Emit(OpCodes.Stloc, array); //set this pops the new array and sets it into the local variable declared earlier

                for (int i = 0; i < inputParameters.Length; ++i)
                {
                    ilGenerator.Emit(OpCodes.Ldloc, array); //loads the array
                    ilGenerator.Emit(OpCodes.Ldc_I4, i); //loads an arbitrary int (the index for the next command)
                    ilGenerator.Emit(OpCodes.Ldarg, i + 1); //loads an input argument
                    ilGenerator.Emit(OpCodes.Stelem_Ref); //gets the 
                }

                //make method call
                ilGenerator.Emit(OpCodes.Ldarg_0); //load this
                ilGenerator.Emit(OpCodes.Ldloc, array); //load the array
                ilGenerator.Emit(OpCodes.Ldc_I4, j); //load the target method index

                if (method.ReturnType != typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Callvirt, methodWithReturn); //consumes the object array and index int, targets the Func so we get a return value
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Callvirt, methodWithoutReturn); //consumes the object array and index int, targets the Action so we don't get a return value
                }

                ilGenerator.Emit(OpCodes.Ret); //finally return, and this method IL is complete
            }

            var generatedType = typeBuilder.CreateType();
            var instance = Activator.CreateInstance(generatedType, inputType);

            return new GeneratedTypeResult<MaskedType>(
                maskedType: (MaskedType)instance,
                interceptor: (Interceptor)instance,
                generatedType: generatedType);
        }

        //TODO maybe this can become private somehow
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
                // I NEED SOME WAY TO IDENTIFY THE METHOD....
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
                    throw new NotImplementedException($"could not find method {methodName} with inputs {string.Join(",", parameters.Select(t => t.Name))}");
                }
            }
        }
    }


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

    public delegate object InterceptMethod(object[] parameters);
    public interface Interceptor
    {
        void SetImplementation(string methodName, Type[] parameters, InterceptMethod methodBody);
    }
}
