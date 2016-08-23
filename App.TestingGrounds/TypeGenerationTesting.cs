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
