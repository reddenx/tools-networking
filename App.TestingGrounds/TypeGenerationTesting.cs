using SMT.Utilities.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace App.TestingGrounds
{
    public static class TypeGenerationTesting
    {
        public static void Run()
        {
            //purpose, does a searched method match it's indexed counterpart from GetMethod and GetMethods
            //var type = typeof(MethodBag);
            //var methodThreeArray = type.GetMethod(nameof(MethodBag.MethodThree), new[] { typeof(object[]) });
            //var allMethods = type.GetMethods();
            //for (int i = 0; i < allMethods.Length; ++i)
            //{
            //    if (allMethods[i] == methodThreeArray)
            //    {
            //    }
            //}



            var testResult = TypeInterceptor.BuildInterceptType<IMethodBag>();
            //testResult.Interceptor.SetImplementation(nameof(MethodBag.MethodOne), new[] { typeof(string) }, (inputs) =>
            //{
            //    Console.WriteLine($"it was intercepted {inputs[0]}");
            //    return null;
            //});

            testResult.Interceptor.SetImplementation<Action<string>>(nameof(MethodBag.MethodOne), 
                s => 
            {
                Console.WriteLine($"it was intercepted {s}");
            });

            testResult.InterceptedInstance.MethodOne("hello world");

            Console.ReadLine();

        }

        public interface IMethodBag
        {
            void MethodOne(string test);
        }

        public class MethodBag : IMethodBag
        {
            public void MethodOne(string test)
            { }

            public void MethodTwo()
            { }

            public object MethodThree(object[] inputs)
            {
                return default(object);
            }

            public object MethodThree(object input1, object input2)
            {
                return default(object);
            }
        }

        public static void Run2()
        {
            //var factory = new TypeFactory("MyNewClass", typeof(TestClass), new[] { typeof(ITestInterface) });
            //factory.AppendMethod<Action<string>>("GeneratedMethod", (input) => Console.WriteLine($"Generated method called with input {input}"));

            //factory.OverrideMethod<Action>("SomeMethod", typeof(ITestInterface), "SomeMethod", () => StaticGrounder());
            //var type = factory.Generate();

            //var instance = Activator.CreateInstance(type) as ITestInterface;

            //instance.SomeMethod();
            //(instance as TestClass).SomeMethod();
            //type.GetMethod("GeneratedMethod").Invoke(instance, new[] { "<test input>" });

            //Console.ReadLine();



            var typeFactory = new TypeFactory("SomeClassTest", typeof(TestClass), new[] { typeof(ITestInterface) });
            typeFactory.AppendMethod<Func<string, string, string>>("Derp",
                (s, s2) => s + " and " + s2
            );

            typeFactory.OverrideMethod<Action>("SomeMethod", typeof(ITestInterface), "SomeMethod",
                () => Console.WriteLine("derp"));

            var type = typeFactory.Generate();

            var instance = Activator.CreateInstance(type);
            var test = type.GetMethod("Derp").Invoke(instance, new object[] { "sdasd", "secods" });
            Console.WriteLine(test);


            type.GetMethod("SomeMethod").Invoke(instance, new object[] { });

            Console.ReadLine();
        }

        static void StaticGrounder()
        {
            Console.WriteLine("called grounder");
        }
    }

    public interface ITestInterface
    {
        void SomeMethod();
    }

    public class TestClass
    {
        public virtual void SomeMethod()
        {
            Console.WriteLine("Ermagerd it works");
            return;
        }
    }
}
