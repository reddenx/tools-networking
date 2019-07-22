using SMT.Utilities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.TestingGrounds
{
    public class PerformanceTest
    {
        public static void Run()
        {
            while (true)
            {
                var config = new TestConfigClass2();
            }
        }

        private class TestConfigClass2 : ConfigurationBase
        {
            [AppSettings("Test1")]
            public readonly int ATestIntegerField;

            [AppSettings("Test2")]
            public readonly bool ATestBooleanField;

            [AppSettings("Test2b")]
            public decimal ATestDecimalField;

            [AppSettings("Test3")]
            public readonly string ATestStringField;

            [AppSettings("Test4")]
            public int TestIntegerProperty { get; protected set; }

            [AppSettings("Test5")]
            public bool TestBooleanProperty { get; protected set; }

            [AppSettings("Test5b")]
            public decimal ATestDecimalProperty { get; protected set; }

            [AppSettings("Test6")]
            public string TestStringProperty { get; protected set; }

            [ConnectionString("TestDataConnectionString")]
            public readonly string TestConnectionStringField;

            [ConnectionString("TestDataConnectionString")]
            public string TestConnectionStringProperty { get; protected set; }

        }
    }
}
