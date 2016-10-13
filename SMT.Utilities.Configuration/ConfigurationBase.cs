using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SMT.Utilities.Configuration
{
	public abstract class ConfigurationBase
	{
		protected ConfigurationBase()
		{
            //get all appsettings fields
            var configurationErrorMessages = new List<string>();

            //should DRY this up eventually, too bad there's a 4x permutation on field/property type and appsetting/connection strings
            var appSettingsFields = this.GetType().GetFields().Where(f => f.GetCustomAttribute<AppSettingsAttribute>() != null);
            foreach (var appSettingsField in appSettingsFields)
            {
                var attr = appSettingsField.GetCustomAttribute<AppSettingsAttribute>();
				if (!TrySetValueFromConfig(attr.Name, attr.ErrorIfMissing, attr.DefaultValue, (s) => ConfigurationManager.AppSettings[s], appSettingsField.FieldType, appSettingsField.SetValue))
				{
                    configurationErrorMessages.Add(string.Format("AppSetting:{0}", attr.Name));
				}
            }

            var appSettingsProperties = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<AppSettingsAttribute>() != null);
            foreach (var appSettingsProperty in appSettingsProperties)
            {
				var attr = appSettingsProperty.GetCustomAttribute<AppSettingsAttribute>();
				if (!TrySetValueFromConfig(attr.Name, attr.ErrorIfMissing, attr.DefaultValue, (s) => ConfigurationManager.AppSettings[s], appSettingsProperty.PropertyType, appSettingsProperty.SetValue))
				{
					configurationErrorMessages.Add(string.Format("AppSetting:{0}", attr.Name));
				}
            }

            var connectionStringFields = this.GetType().GetFields().Where(f => f.GetCustomAttribute<ConnectionStringAttribute>() != null);
            foreach (var connectionStringField in connectionStringFields)
            {
                var attr = connectionStringField.GetCustomAttribute<ConnectionStringAttribute>();
				if (!TrySetValueFromConfig(attr.Name, attr.ErrorIfMissing, attr.DefaultValue, (s) => ConfigurationManager.ConnectionStrings[s].ConnectionString, connectionStringField.FieldType, connectionStringField.SetValue))
				{
					configurationErrorMessages.Add(string.Format("ConnectionString:{0}", attr.Name));
				}
			}

            var connectionStringProperties = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<ConnectionStringAttribute>() != null);
            foreach(var connectionStringProperty in connectionStringProperties)
            {
                var attr = connectionStringProperty.GetCustomAttribute<ConnectionStringAttribute>();
				if (!TrySetValueFromConfig(attr.Name, attr.ErrorIfMissing, attr.DefaultValue, (s) => ConfigurationManager.ConnectionStrings[s].ConnectionString, connectionStringProperty.PropertyType, connectionStringProperty.SetValue))
				{
					configurationErrorMessages.Add(string.Format("ConnectionString:{0}", attr.Name));
				}
			}

            if (configurationErrorMessages.Any())
            {
                var compositeMessage = string.Format("Missing Configuration Value(s): {0}", string.Join(", ", configurationErrorMessages));
                throw new ConfigurationErrorsException(compositeMessage);
            }
		}

        private object GetConvertedValue(string stringValue, Type type)
        {
            try
            {
                return Convert.ChangeType(stringValue, type);
            }
            catch
            {
                return null;
            }
        }

		private bool TrySetValueFromConfig(string name, bool failIfMissing, string defaultValue, Func<string,string> getFromConfig, Type destinationType, Action<object, object> setValue)
		{
			var value = getFromConfig(name);
			var convertedValue = GetConvertedValue(value ?? defaultValue, destinationType);

			if (convertedValue != null)
			{
				setValue(this, convertedValue);
			}
			else if (failIfMissing)
			{
				return false;
			}
			return true;
		}
	}
}
