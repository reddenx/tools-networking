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
            var appSettingsFields = this.GetType().GetFields().Where(f => f.GetCustomAttribute<AppSettingsAttribute>() != null);
            foreach (var appSettingsField in appSettingsFields)
            {
                var name = appSettingsField.GetCustomAttribute<AppSettingsAttribute>().Name;
                var value = ConfigurationManager.AppSettings[name];
                appSettingsField.SetValue(this, GetConvertedValue(value, appSettingsField.FieldType));
            }

            var appSettingsProperties = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<AppSettingsAttribute>() != null);
            foreach (var appSettingsProperty in appSettingsProperties)
            {
                var name = appSettingsProperty.GetCustomAttribute<AppSettingsAttribute>().Name;
                var value = ConfigurationManager.AppSettings[name];
                appSettingsProperty.SetValue(this, GetConvertedValue(value, appSettingsProperty.PropertyType));
            }

            var connectionStringFields = this.GetType().GetFields().Where(f => f.GetCustomAttribute<ConnectionStringAttribute>() != null);
            foreach (var connectionStringField in connectionStringFields)
            {
                var name = connectionStringField.GetCustomAttribute<ConnectionStringAttribute>().Name;
                var value = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                connectionStringField.SetValue(this, GetConvertedValue(value, connectionStringField.FieldType));
            }

            var connectionStringProperties = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<ConnectionStringAttribute>() != null);
            foreach(var connectionStringProperty in connectionStringProperties)
            {
                var name = connectionStringProperty.GetCustomAttribute<ConnectionStringAttribute>().Name;
                var value = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                connectionStringProperty.SetValue(this, GetConvertedValue(value, connectionStringProperty.PropertyType));
            }
		}

        private object GetConvertedValue(string stringValue, Type type)
        {
            return Convert.ChangeType(stringValue, type);
        }
	}
}
