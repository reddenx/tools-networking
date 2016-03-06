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

namespace App.TestingGrounds
{
    static class SqlTesting
    {
        public static void Run()
        {
            Console.WriteLine("Starting Sql tests");

            var querier = DataConnectionFactory.GetSqlCeQuerier(ConfigurationManager.ConnectionStrings["TestDataConnectionString"].ConnectionString);

            var sql = 
@"
select *
from TestUser
";
            var objects = querier.ExecuteReader<TestUser>(sql);

            Console.WriteLine("End Sql Tests");
        }
    }

    public interface ITestUser
    {
        string Username { get; }
    }

    [DBObject]
    public class TestUser : ITestUser
    {
        [DBColumn("Username")]
        public string Username { get; set; }
        [DBColumn("Description")]
        public string Description;
        [DBColumn("TestUserId")]
        public int UserId;
    }
}
