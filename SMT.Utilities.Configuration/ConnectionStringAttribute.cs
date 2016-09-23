
namespace SMT.Utilities.Configuration
{
    public class ConnectionStringAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public ConnectionStringAttribute(string name)
        {
            Name = name;
        }
    }
}
