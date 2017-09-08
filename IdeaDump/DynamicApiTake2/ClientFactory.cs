using SMT.Utilities.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaDump.DynamicApiTake2
{
    class ClientFactory
    {
        internal static Contract BuildProxy<Contract>(string v)
        {
            //do some validation
            var contractInterfaceType = typeof(Contract);
            if (!contractInterfaceType.IsInterface)
                throw new ArgumentException("contract type must be a public interface");

            if (contractInterfaceType.IsGenericType)
                throw new ArgumentException("contract type cannot be generic");

            var allMethodsRouted = contractInterfaceType.GetMethods().All(method => method.GetCustomAttributes(typeof(ContractRouteAttribute), false).Any());
            if (!allMethodsRouted)
                throw new ArgumentException("contract type must have all methods routed");

            var contractBaseRoute = contractInterfaceType.GetCustomAttributes(typeof(ContractRouteAttribute), false).FirstOrDefault() as ContractRouteAttribute;
            if (string.IsNullOrWhiteSpace(contractBaseRoute?.Route))
                throw new ArgumentException("contract type must have a base route");

            if (contractInterfaceType.GetProperties().Any() || contractInterfaceType.GetFields().Any())
                throw new ArgumentException("contract must be composed entirely of routed methods");
            


            //no fields or properties, has all its methods routed and contains a base url
            var routedMethods = contractInterfaceType.GetMethods().Where(method => method.GetCustomAttributes(typeof(ContractRouteAttribute), false).Any());

            var typeFactory = new TypeFactory($"DynamicContract_{contractInterfaceType.Name}_{Guid.NewGuid()}", typeof(BaseProxy), new[] { contractInterfaceType });


            //build methods
            foreach (var method in routedMethods)
            {
                var routeAttribute = method.GetCustomAttributes(typeof(ContractRouteAttribute), false).Single() as ContractRouteAttribute;

                //typeFactory.AppendMethod
            }

            var builtType = typeFactory.Generate();
            return (Contract)Activator.CreateInstance(builtType);
        }
    }

    class BaseProxy
    {
        internal void DoStuff()
        { }
    }

    class ContractRouteAttribute : Attribute
    {
        internal string Route;

        public ContractRouteAttribute(string route)
        { }
    }
}
