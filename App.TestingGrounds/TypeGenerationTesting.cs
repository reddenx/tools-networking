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
    static class TypeGenerationTesting
    {
        public static void Run()
        {

            var typeFactory = new TypeFactory("SomeClassTest", null, null);// new[] { typeof(ITestInterface) });
            typeFactory.AppendStaticMethod<Func<string, string, string>>("Derp",
                (s, s2) => s + " and " + s2
            );

            var type = typeFactory.Generate();

            var instance = Activator.CreateInstance(type);
            var test = type.GetMethod("Derp").Invoke(instance, new object[] { "sdasd", "secods" });

            Console.WriteLine(test);

            Console.ReadLine();
        }
    }

    interface ITestInterface
    {
        void SomeMethod();
    }

    class TestClass
    {
        public void SomeMethod()
        {
            Console.WriteLine("Ermagerd it works");
            return;
        }
    }
}
