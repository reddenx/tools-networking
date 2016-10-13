
namespace SMT.Utilities.Configuration
{
    /// <summary>
    /// Marks a field or property to be populated by an entry in configuration.connectionstrings from a web or app config
    /// </summary>
    public class ConnectionStringAttribute : System.Attribute
    {
        /// <summary>
        /// the name of the entry in connectionstrings
        /// </summary>
        public string Name { get; private set; }

		/// <summary>
		/// Flag indicating if the application will throw an error if this entry is missing from the config
		/// </summary>
		public bool ErrorIfMissing { get; private set; }

		/// <summary>
		/// Entry to default to if the entry is missing from the config
		/// </summary>
		public string DefaultValue { get; private set; }

        /// <summary>
        /// Marks a field to be populated by an entry in configuration.connectionstrings from a web or app config
        /// </summary>
        /// <param name="name">The name of the entry in connectionstrings</param>
        public ConnectionStringAttribute(string name, string defaultValue = null, bool errorOnMissing = true)
        {
            Name = name;
			ErrorIfMissing = errorOnMissing;
			DefaultValue = defaultValue;
        }
    }
}
