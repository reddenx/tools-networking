using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.Configuration;

namespace App.TestingGrounds
{
    static class ConfigurationTesting
    {
        public static void Run()
        {
            var testconfig = new TestConfig();

            foreach (var field in testconfig.GetType().GetFields())
            {
                Console.WriteLine("{0} = {1}", field.Name, field.GetValue(testconfig));
            }

            foreach (var prop in testconfig.GetType().GetProperties())
            {
                Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(testconfig));
            }

            Console.ReadLine();
        }
    }

    internal class TestConfig : ConfigurationBase
    {
        [AppSettings("Test1")]
        public readonly int Something;

        [AppSettings("Test2")]
        public readonly bool SomethingElse;

        [AppSettings("Test3")]
        public readonly string AnotherThing;

        [AppSettings("Test1")]
        public int PropSomething { get; protected set; }

        [AppSettings("Test2")]
        public bool PropSomethingElse { get; protected set; }

        [AppSettings("Test3")]
        public string PropAnotherThing { get; protected set; }

        [ConnectionString("TestDataConnectionString")]
        public readonly string ConnectionString;

        [ConnectionString("TestDataConnectionString")]
        public string PropConnectionString { get; protected set; }
    }
}
