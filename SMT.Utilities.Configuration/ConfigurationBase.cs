using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SMT.Utilities.Configuration
{
    public abstract class ConfigurationBase
    {
        private static readonly Dictionary<Type, ConfigurationFieldInfo[]> _cache = new Dictionary<Type, ConfigurationFieldInfo[]>();

        private class ConfigurationFieldInfo
        {
            public FieldInfo field;
            public PropertyInfo prop;
            public AppSettingsAttribute appsetting;
            public ConnectionStringAttribute connection;
        }

        protected ConfigurationBase()
        {
            //get all appsettings fields
            var configurationErrorMessages = new List<string>();

            var type = this.GetType();
            ConfigurationFieldInfo[] data;

            if (_cache.ContainsKey(type))
            {
                data = _cache[type];
            }
            else
            {
                data = type.GetFields().Select(f => new ConfigurationFieldInfo
                {
                    field = f,
                    appsetting = f.GetCustomAttribute<AppSettingsAttribute>(),
                    connection = f.GetCustomAttribute<ConnectionStringAttribute>(),
                }).Concat(type.GetProperties().Select(f => new ConfigurationFieldInfo
                {
                    prop = f,
                    appsetting = f.GetCustomAttribute<AppSettingsAttribute>(),
                    connection = f.GetCustomAttribute<ConnectionStringAttribute>(),
                })).ToArray();
                _cache[type] = data;
            }

            //fields
            foreach (var item in data)
            {
                if (item.field != null)
                {
                    if (item.appsetting != null)
                    {
                        if (!TrySetValueFromConfig(item.appsetting.Name, item.appsetting.ErrorIfMissing, item.appsetting.DefaultValue, (s) => ConfigurationManager.AppSettings[s], item.field.FieldType, item.field.SetValue))
                        {
                            configurationErrorMessages.Add(string.Format("AppSetting:{0}", item.appsetting.Name));
                        }
                    }
                    if (item.connection != null)
                    {
                        if (!TrySetValueFromConfig(item.connection.Name, item.connection.ErrorIfMissing, item.connection.DefaultValue, (s) => ConfigurationManager.ConnectionStrings[s].ConnectionString, item.field.FieldType, item.field.SetValue))
                        {
                            configurationErrorMessages.Add(string.Format("ConnectionString:{0}", item.connection.Name));
                        }
                    }
                }
                if (item.prop != null)
                {
                    if (item.appsetting != null)
                    {
                        if (!TrySetValueFromConfig(item.appsetting.Name, item.appsetting.ErrorIfMissing, item.appsetting.DefaultValue, (s) => ConfigurationManager.AppSettings[s], item.prop.PropertyType, item.prop.SetValue))
                        {
                            configurationErrorMessages.Add(string.Format("AppSetting:{0}", item.appsetting.Name));
                        }
                    }
                    if (item.connection != null)
                    {
                        if (!TrySetValueFromConfig(item.connection.Name, item.connection.ErrorIfMissing, item.connection.DefaultValue, (s) => ConfigurationManager.ConnectionStrings[s].ConnectionString, item.prop.PropertyType, item.prop.SetValue))
                        {
                            configurationErrorMessages.Add(string.Format("ConnectionString:{0}", item.connection.Name));
                        }
                    }
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

        private bool TrySetValueFromConfig(string name, bool failIfMissing, string defaultValue, Func<string, string> getFromConfig, Type destinationType, Action<object, object> setValue)
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
