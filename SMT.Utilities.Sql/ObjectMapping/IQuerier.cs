using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Sql.ObjectMapping
{
    public static class Sql
    {
        public static void Mocking()
        {
            //var results = Sql.Cmd<string>("select @Test", new { Test = "12345" })
            //    .Cmd<int>("test", o => new { id = o })
            //    .Sproc<int>("testsproc", p => new { id = p })
            //    .Execute("connection string");

            //var a = 1 as object;

            //results[0];

            //get a single customer
            //get all of that customer's accounts

            //var results = Sql
            //    .CmdScalar<int>("select CustomerId from customer where name = @Name", new { Name = "Sean" })
            //    .CmdList<int>("select count(*) from account where CustomerId = @CustomerId", p => new { CustomerId = p })
            //    .Execute("connection string");
        }

        public static dynamic CmdScalar<T>(string sql, object parameters)
        {
            throw new NotImplementedException();
        }

        static ICommandChained<T> Cmd<T>(string sql, object parameters)
        {
            throw new NotImplementedException();
        }
    }

    //multi, data, scalar
    //sproc, query




    //first execution
    interface ICommand<Tcurrent>
    {
        ICommandChained<Tnext> Cmd<Tnext>(string sql, object parameters);
        ICommandChained<Tnext> Sproc<Tnext>(string sql, object parameters);
    }

    interface ICommandChained<Tcurrent> : ICommand<Tcurrent>
    {
        ICommandChained<Tnext> Cmd<Tnext>(string sql, Func<Tcurrent, object> dependentParameters);
        ICommandChained<Tnext> Sproc<Tnext>(string sproc, Func<Tcurrent, object> dependentParameters);
    }
}
