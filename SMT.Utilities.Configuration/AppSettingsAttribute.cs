
namespace SMT.Utilities.Configuration
{
    /// <summary>
    /// Marks a field or property to be populated by an entry in configuration.appsettings from a web or app config
    /// </summary>
    public class AppSettingsAttribute : System.Attribute
    {
        /// <summary>
        /// the name of the entry in appsettings
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Marks a field to be populated by an entry in configuration.appsetings from a web or app config
        /// </summary>
        /// <param name="name">The name of the entry in appsettings</param>
        public AppSettingsAttribute(string name)
        {
            Name = name;
        }
    }
}
