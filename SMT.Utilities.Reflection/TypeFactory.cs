using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Utilities.Reflection
{
    public static class TypeFactory
    {
        public static GeneratedTypeResult<MaskedType> BuildType<MaskedType>() where MaskedType : class
        {
            return BuildType<MaskedType>(null);
        }

        //what I want out of this
        //a masked type to hand out and
        //an interceptor interface to handle calls
        public static GeneratedTypeResult<MaskedType> BuildType<MaskedType>(MaskedType instance) where MaskedType : class
        {
            //this method is intentionally left as a large procedural block


            var inputType = typeof(MaskedType);

            //inquiry revealed this as necessary for this implementation
            if (!inputType.IsInterface)
                throw new NotImplementedException("only interfaces can be masked");

            if (inputType.IsGenericType || inputType.GetMethods().Any(m => m.IsGenericMethod))
                throw new NotImplementedException("no generics allowed... yet");

            //get and define a home for this new type
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                name: Assembly.GetAssembly(typeof(MaskedType)).GetName(),
                access: AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"TypeInterception.{inputType.Name}");
            var typeBuilder = moduleBuilder.DefineType($"TypeInterception.{inputType.Name}.{Guid.NewGuid()}");

            //setup the heirarchy, parent is the target type, base type is the interceptor
            var baseType = typeof(BaseInterceptor);
            typeBuilder.AddInterfaceImplementation(inputType);
            typeBuilder.SetParent(baseType);

            //generate constructor
            GenerateConstructor(baseType.GetConstructors()[0], typeBuilder);

            //get the methods we want to target in the base type
            var methodWithReturn = baseType.GetMethod(nameof(BaseInterceptor.Func));
            var methodWithoutReturn = baseType.GetMethod(nameof(BaseInterceptor.Action));

            //build up methods (properties included)
            var methods = inputType.GetMethods();
            for (int j = 0; j < methods.Length; ++j)
            {
                GenerateMethod(methods[j], j, typeBuilder, methodWithReturn, methodWithoutReturn);
            }

            //put it all together
            var generatedType = typeBuilder.CreateType();
            var newInstance = Activator.CreateInstance(generatedType, inputType, instance);

            return new GeneratedTypeResult<MaskedType>(
                maskedType: (MaskedType)newInstance,
                interceptor: (Interceptor)newInstance,
                generatedType: generatedType);
        }

        private static void GenerateConstructor(ConstructorInfo constructorTarget, TypeBuilder typeBuilder)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(Type), typeof(object) });
            var ctorIlGen = constructorBuilder.GetILGenerator();

            //build constructor
            ctorIlGen.Emit(OpCodes.Ldarg, 0); //load this (this keyword is always arg0 in an instanced object)
            ctorIlGen.Emit(OpCodes.Ldarg, 1); //load argument 1 (Type targetType)
            ctorIlGen.Emit(OpCodes.Ldarg, 2); //load argument 2 (instance)
            ctorIlGen.Emit(OpCodes.Call, constructorTarget); //make the call
            ctorIlGen.Emit(OpCodes.Ret); //return
        }

        private static void GenerateMethod(MethodInfo method, int methodIndex, TypeBuilder typeBuilder, MethodInfo methodWithReturn, MethodInfo methodWithoutReturn)
        {
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

                //if the parameter requires boxing, box it (value types and enums)
                var parameterType = inputParameters[i].ParameterType;
                AppropriatelyBoxType(parameterType, ilGenerator);

                ilGenerator.Emit(OpCodes.Stelem_Ref); //move object into target index, pops args1-n and 'this' (arg0)
            }

            //make method call
            ilGenerator.Emit(OpCodes.Ldarg_0); //load this
            ilGenerator.Emit(OpCodes.Ldloc, array); //load the array
            ilGenerator.Emit(OpCodes.Ldc_I4, methodIndex); //load the target method index

            if (method.ReturnType != typeof(void))
            {
                ilGenerator.Emit(OpCodes.Callvirt, methodWithReturn); //consumes the object array and index int, targets the Func so we get a return value

                //if the return type requires boxing, unbox it and reload it's value to the stack before returning it
                AppropriatelyUnBoxType(method.ReturnType, ilGenerator);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Callvirt, methodWithoutReturn); //consumes the object array and index int, targets the Action so we don't get a return value
            }

            ilGenerator.Emit(OpCodes.Ret); //finally return, and this method IL is complete
        }

        private static void AppropriatelyUnBoxType(Type type, ILGenerator ilGenerator)
        {
            if (type.IsValueType || type.IsEnum)
            {
                ilGenerator.Emit(OpCodes.Unbox, type);
                ilGenerator.Emit(OpCodes.Ldobj, type);
            }
        }

        private static void AppropriatelyBoxType(Type type, ILGenerator ilGenerator)
        {
            if (type.IsValueType || type.IsEnum)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }
    }

    public delegate object InterceptMethod(object[] parameters);
    public delegate object InterceptPropertyGet();
    public delegate void InterceptPropertySet(object input);
    public interface Interceptor
    {
        void SetMethodImplementation(string methodName, Type[] parameters, InterceptMethod methodBody);
        void SetMethodImplementation<T>(string methodName, T implementation);
        void SetSetPropertyImplementation(string propertyName, InterceptPropertySet setMethod);
        void SetGetPropertyImplementation(string propertyName, InterceptPropertyGet getMethod);
    }
}
