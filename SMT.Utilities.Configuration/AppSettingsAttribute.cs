
namespace SMT.Utilities.Configuration
{
    public class AppSettingsAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public AppSettingsAttribute(string name)
        {
            Name = name;
        }
    }
}
