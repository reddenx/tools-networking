//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Text;
//using System.Threading.Tasks;

//namespace SMT.Utilities.Reflection
//{
//    public class TypeFactory
//    {
//        private TypeBuilder TypeBuilderInstance;

//        public TypeFactory(string className, Type parentType, Type[] interfaces)
//        {
//            var assemblyName = Assembly.GetCallingAssembly().GetName();
//            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
//            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

//            TypeBuilderInstance = moduleBuilder.DefineType(className,
//                TypeAttributes.Public |
//                TypeAttributes.Class |
//                TypeAttributes.AutoClass |
//                TypeAttributes.AnsiClass |
//                TypeAttributes.BeforeFieldInit |
//                TypeAttributes.AutoLayout,
//                parentType);

//            if (interfaces != null && interfaces.Any())
//            {
//                foreach (var interfaceDefinition in interfaces)
//                {
//                    TypeBuilderInstance.AddInterfaceImplementation(interfaceDefinition);
//                }
//            }
//        }

//        public void OverrideMethod<MethodDefinition>(string methodName, Type overridingType, string methodToOverride, Expression<MethodDefinition> expressionBody)
//        {
//            var method = CompileExpressionToMethod(methodName, expressionBody);

//            //var methodBuilderStatic = TypeBuilderInstance.DefineMethod("_" + methodName,
//            //    MethodAttributes.Public |
//            //    MethodAttributes.Static);
//            //expressionBody.CompileToMethod(methodBuilderStatic);
            
//            //var methodBuilder = TypeBuilderInstance.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Virtual);

//            TypeBuilderInstance.DefineMethodOverride(method, overridingType.GetMethod(methodToOverride));

//            //methodBuilder.SetReturnType(expressionBody.ReturnType);
//            //
//            //var ilGenerator = methodBuilder.GetILGenerator();
//            //
//            //var types = expressionBody.Parameters.Select(p => p.Type).ToArray();
//            //methodBuilder.SetParameters(types);
//            //
//            //for (Int32 i = 1; i <= types.Length; ++i)
//            //{
//            //    ilGenerator.Emit(OpCodes.Ldarg, i);
//            //}
//            //
//            //ilGenerator.EmitCall(OpCodes.Call, methodBuilderStatic, null);
//            //ilGenerator.Emit(OpCodes.Ret);
//        }

//        public void AppendMethod<MethodDefinition>(string methodName, Expression<MethodDefinition> expressionBody)
//        {
//            var methodBuilder = CompileExpressionToMethod(methodName, expressionBody);

//            //var methodInfo = (lambda as Delegate);
//            //if (methodInfo == null)
//            //{
//            //    throw new ArgumentException("lambda must be a delegate", "lambda");
//            //}
//            //
//            //var returnType = methodInfo.Method.ReturnType;
//            //var parameters = methodInfo.Method.GetParameters().Select(p => p.ParameterType).ToArray();
//            //
//            //var methodBuilder = TypeBuilderInstance.DefineMethod(methodName, 
//            //    MethodAttributes.Public | MethodAttributes.Static,
//            //    returnType,
//            //    parameters);
//            //
//            //var il = methodInfo.Method.GetMethodBody().GetILAsByteArray();
//            //methodBuilder.CreateMethodBody(il, il.Length);
//            //
//            //var ilGenerator = methodBuilder.GetILGenerator();



//            //turn expression into private static method
//            //var methodBuilderStatic = TypeBuilderInstance.DefineMethod("_" + methodName,
//            //    MethodAttributes.Public |
//            //    MethodAttributes.Static);
//            //expressionBody.CompileToMethod(methodBuilderStatic);
            
//            //var methodBuilder = TypeBuilderInstance.DefineMethod(methodName,
//            //    MethodAttributes.Public);
            
            
//            //methodBuilder.SetReturnType(expressionBody.ReturnType);
            
//            //var ilGenerator = methodBuilder.GetILGenerator();

//            //var types = expressionBody.Parameters.Select(p => p.Type).ToArray();
//            //methodBuilder.SetParameters(types);

//            //for (Int32 i = 1; i <= types.Length; ++i)
//            //{
//            //    ilGenerator.Emit(OpCodes.Ldarg, i);
//            //}

//            //ilGenerator.EmitCall(OpCodes.Call, methodBuilderStatic, null);
//            //ilGenerator.Emit(OpCodes.Ret);


//            //var ilGen = methodBuilder.GetILGenerator();
//            //ilGen.Emit(




//            //var methodBuilder = typeBuilder.DefineMethod(
//            //    method.Name,
//            //    MethodAttributes.Virtual |
//            //    MethodAttributes.Public);

//            //var ilGenerator = methodBuilder.GetILGenerator();
//            //ilGenerator.Emit(OpCodes.pop






//            //TODO-SM only one method override currently supported
//            //var interfaceMethod = interfaces.GetMethod(method.Name);
//            //typeBuilder.DefineMethodOverride(method, interfaceMethod);
//        }

//        private MethodBuilder CompileExpressionToMethod<T>(string methodName, Expression<T> expressionBody)
//        {
//            var methodBuilderStatic = TypeBuilderInstance.DefineMethod("_" + methodName,
//                MethodAttributes.Public |
//                MethodAttributes.Static);
//            expressionBody.CompileToMethod(methodBuilderStatic);

//            var methodBuilder = TypeBuilderInstance.DefineMethod(methodName,
//                MethodAttributes.Public | MethodAttributes.Virtual);


//            methodBuilder.SetReturnType(expressionBody.ReturnType);

//            var ilGenerator = methodBuilder.GetILGenerator();

//            var types = expressionBody.Parameters.Select(p => p.Type).ToArray();
//            methodBuilder.SetParameters(types);

//            for (Int32 i = 1; i <= types.Length; ++i)
//            {
//                ilGenerator.Emit(OpCodes.Ldarg, i);
//            }

//            ilGenerator.EmitCall(OpCodes.Call, methodBuilderStatic, null);
//            ilGenerator.Emit(OpCodes.Ret);

//            return methodBuilder;
//        }

//        public Type Generate()
//        {
//            return this.TypeBuilderInstance.CreateType();
//        }
//    }
//}
