using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.Sql;
using SMT.Utilities.Sql.ObjectMapping;
using System.Configuration;
using SMT.Utilities.Sql.SqlCe;
using System.Data;
using SMT.Utilities.Sql.Interfaces;

namespace App.TestingGrounds
{
    static class SqlTesting
    {
        public static void Run()
        {
            Console.WriteLine("Starting Sql tests");

            //TestQuerier(DataConnectionFactory.GetSqlCeQuerier(ConfigurationManager.ConnectionStrings["TestDataConnectionString"].ConnectionString));
            TestQuerier(DataConnectionFactory.GetSqlQuerier(ConfigurationManager.ConnectionStrings["TestSqlConnection"].ConnectionString));




            Console.WriteLine("End Sql Tests");
            Console.ReadLine();
        }

        static void TestQuerier(ISqlQuerier querier)
        {
            var sql =
@"
declare @Test1 int = @TestNullInputInt
declare @Test2 int = @TestInputInt
declare @Test3 int = @TestNullInputBit
declare @Test4 bit = @TestInputBit
declare @Test5 varchar(200) = @TestInputStringNull
declare @Test6 varchar(200) = @TestInputString

select *
from dbo.DataObjectTesting
";
            var objects = querier.ExecuteReader<TestObjectPropertyBased>(sql,
                querier.CreateParameter("@TestNullInputInt", SqlDbType.Int, null),
                querier.CreateParameter("@TestInputInt", SqlDbType.Int, -1200),
                querier.CreateParameter("@TestNullInputBit", SqlDbType.Bit, null),
                querier.CreateParameter("@TestInputBit", SqlDbType.Bit, true),
                querier.CreateParameter("@TestInputStringNull", SqlDbType.VarChar, 200, null),
                querier.CreateParameter("@TestInputString", SqlDbType.VarChar, 200, "test"));

            foreach (var obj in objects)
            {
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (prop.GetValue(obj) != null)
                    {
                        Console.WriteLine("{0}:{1}", prop.Name, prop.GetValue(obj).ToString());
                    }
                    else
                    {
                        Console.WriteLine("{0}:{1}", prop.Name, "c#null");
                    }
                }
                Console.WriteLine();
            }
        }
    }

    public enum TestEnum
    {
        One = 1,
        NegativeOne = -1
    }

    [DBObject]
    public class TestObjectPropertyBased
    {
        [DBColumn("DataObjectTestingId")]
        public int DataObjectTestingId { get; set; }
        [DBColumn("DateCreated")]
        public DateTime DateCreated { get; set; }
        [DBColumn("IntTestNullable")]
        public int? IntTestNullable { get; set; }
        [DBColumn("IntTest")]
        public int IntTest { get; set; }
        [DBColumn("StringTestNullable")]
        public string StringTestNullable { get; set; }
        [DBColumn("StringTest")]
        public string StringTest { get; set; }
        [DBColumn("BitBoolTestNullable")]
        public bool? BitBoolTestNullable { get; set; }
        [DBColumn("BitBoolTest")]
        public bool BitBoolTest { get; set; }
        [DBColumn("DateTestNullable")]
        public DateTime? DateTestNullable { get; set; }
        [DBColumn("DateTest")]
        public DateTime DateTest { get; set; }
        [DBColumn("EnumTestChar")]
        public TestEnum EnumTestChar { get; set; }
        [DBColumn("EnumTestCharNullable")]
        public TestEnum? EnumTestCharNullable { get; set; }
        [DBColumn("EnumTestInt")]
        public TestEnum EnumTestInt { get; set; }
        [DBColumn("EnumTestIntNullable")]
        public TestEnum? EnumTestIntNullable { get; set; }
    }

    [DBObject]
    public class TestObjectFieldBased
    {
        [DBColumn("DataObjectTestingId")]
        public int DataObjectTestingId;
        [DBColumn("DateCreated")]
        public DateTime DateCreated;
        [DBColumn("IntTestNullable")]
        public int? IntTestNullable;
        [DBColumn("IntTest")]
        public int IntTest;
        [DBColumn("StringTestNullable")]
        public string StringTestNullable;
        [DBColumn("StringTest")]
        public string StringTest;
        [DBColumn("BitBoolTestNullable")]
        public bool? BitBoolTestNullable;
        [DBColumn("BitBoolTest")]
        public bool BitBoolTest;
        [DBColumn("DateTestNullable")]
        public DateTime? DateTestNullable;
        [DBColumn("DateTest")]
        public DateTime DateTest;
        [DBColumn("EnumTestChar")]
        public TestEnum EnumTestChar;
        [DBColumn("EnumTestCharNullable")]
        public TestEnum? EnumTestCharNullable;
        [DBColumn("EnumTestInt")]
        public TestEnum EnumTestInt;
        [DBColumn("EnumTestIntNullable")]
        public TestEnum? EnumTestIntNullable;
    }
}
