using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    internal static class XmlUtilities
    {
        public static string[] GetXmlElementNamesForType(Type type)
        {
            var members = type.GetMembers();
            var properties = type.GetProperties();

            var elementNames = members.Select(member => member.GetCustomAttributes(true))
                .Concat(properties.Select(property => property.GetCustomAttributes(true)))
                .Where(attributes => attributes.Any(attribute => attribute is XmlElementAttribute))// filter out non xml attribute lists
                .Select(attributes => (XmlElementAttribute)attributes.First(attribute => attribute is XmlElementAttribute)) //get only the xml attribute
                .Select(attribute => attribute.ElementName).ToArray(); //get names from element

            return elementNames;
        }

        public static int? LazyParseInt(string toParse, ref int? backingField)
        {
            if (backingField.HasValue)
            {
                return backingField.Value;
            }

            int testParse;
            if (int.TryParse(toParse, out testParse))
            {
                return (backingField = testParse);
            }

            return null;
        }

        public static DateTime? LazyParseDate(string toParse, ref DateTime? backingField)
        {
            if (backingField.HasValue)
            {
                return backingField.Value;
            }

            DateTime testParse;
            if (DateTime.TryParse(toParse, out testParse))
            {
                return (backingField = testParse);
            }

            return null;
        }
    }
}
