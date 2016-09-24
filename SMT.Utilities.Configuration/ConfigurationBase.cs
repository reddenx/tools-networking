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
                var name = appSettingsField.GetCustomAttribute<AppSettingsAttribute>().Name;
                var value = ConfigurationManager.AppSettings[name];
                var convertedValue = GetConvertedValue(value, appSettingsField.FieldType);

                if (convertedValue != null)
                {
                    appSettingsField.SetValue(this, convertedValue);
                }
                else
                {
                    configurationErrorMessages.Add(string.Format("AppSetting:{0}", name));
                }
            }

            var appSettingsProperties = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<AppSettingsAttribute>() != null);
            foreach (var appSettingsProperty in appSettingsProperties)
            {
                var name = appSettingsProperty.GetCustomAttribute<AppSettingsAttribute>().Name;
                var value = ConfigurationManager.AppSettings[name];
                var convertedValue = GetConvertedValue(value, appSettingsProperty.PropertyType);

                if (convertedValue != null)
                {
                    appSettingsProperty.SetValue(this, convertedValue);
                }
                else
                {
                    configurationErrorMessages.Add(string.Format("AppSetting:{0}", name));
                }
            }

            var connectionStringFields = this.GetType().GetFields().Where(f => f.GetCustomAttribute<ConnectionStringAttribute>() != null);
            foreach (var connectionStringField in connectionStringFields)
            {
                var name = connectionStringField.GetCustomAttribute<ConnectionStringAttribute>().Name;
                var value = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                var convertedValue = GetConvertedValue(value, connectionStringField.FieldType);

                if (convertedValue != null)
                {
                    connectionStringField.SetValue(this, convertedValue);
                }
                else
                {
                    configurationErrorMessages.Add(string.Format("ConnectionString:{0}", name));
                }
            }

            var connectionStringProperties = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<ConnectionStringAttribute>() != null);
            foreach(var connectionStringProperty in connectionStringProperties)
            {
                var name = connectionStringProperty.GetCustomAttribute<ConnectionStringAttribute>().Name;
                var value = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                var convertedValue = GetConvertedValue(value, connectionStringProperty.PropertyType);

                if (convertedValue != null)
                {
                    connectionStringProperty.SetValue(this, convertedValue);
                }
                else
                {
                    configurationErrorMessages.Add(string.Format("ConnectionString:{0}", name));
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
	}
}
