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
			try
			{
				var testconfig = new TestConfigClass();

				foreach (var field in testconfig.GetType().GetFields())
				{
					Console.WriteLine("{0} = {1}", field.Name, field.GetValue(testconfig));
				}

				foreach (var prop in testconfig.GetType().GetProperties())
				{
					Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(testconfig));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("TEST FAIL EXCEPTION: {0}", e.ToString());
			}



			Console.ReadLine();
		}
	}

	public class TestConfigClass : ConfigurationBase
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
