using IdeaDump.DynamicApiTake2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.TestingGrounds.DTOs;

namespace App.TestingGrounds
{
    public static class IdeaDumpTest
    {
        public static void Run()
        {
            var proxy = ClientFactory.BuildProxy<IContract>("http://localhost/");

            proxy.MethodOne();
        }
    }



    namespace ServerStuff
    {
        class ObjectToBeTurnedIntoApi : IContract
        {
            public void MethodOne()
            {
                throw new NotImplementedException();
            }

            public ExampleDto MethodTwo(InputDto input)
            {
                throw new NotImplementedException();
            }
        }
    }


    namespace DTOs
    {

        [ContractRoute("api")]
        public interface IContract
        {
            [ContractRoute("method1")]
            void MethodOne();

            [ContractRoute("method2")]
            ExampleDto MethodTwo(InputDto input);
        }

        public struct ExampleDto { }
        public struct InputDto { }
    }
}
