using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SMT.Utilities.Configuration
{
    //entry point
    class Program
    {
        static void Main(string[] args)
        {
            var config = new LocalConfiguration();

            Console.WriteLine(config.TestValue1);
            Console.WriteLine(config.TestValue2);
            Console.WriteLine(config.TestValue3);
        }
    }

    //the implementation found in at the highest level application entry point
    class LocalConfiguration : ConfigurationBase
    {
        [AppSettings("Test1")]
        public int TestValue1;

        [AppSettings("Test2")]
        public bool TestValue2;

        [AppSettings("Test3")]
        public string TestValue3;
    }

    //the utility that lives in SMT.Utilities.Configuration
    public abstract class ConfigurationBase
    {
        protected ConfigurationBase()
        {
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttributes(typeof(AppSettingsAttribute)).Single() as AppSettingsAttribute;
                if (attribute != null)
                {
                    field.SetValue(this, Convert.ChangeType(ConfigurationManager.AppSettings[attribute.Key], field.FieldType));
                }
            }
        }
    }

    //the lookup attribute
    public class AppSettingsAttribute : System.Attribute
    {
        public readonly string Key;

        public AppSettingsAttribute(string key)
        {
            Key = key;
        }
    }
}

