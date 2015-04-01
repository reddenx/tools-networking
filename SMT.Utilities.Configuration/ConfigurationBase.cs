using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Configuration
{
    public abstract class ConfigurationBase
    {
        protected ConfigurationBase()
        {
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                var appSetting = field.GetCustomAttributes(typeof(AppSettingsAttribute)).FirstOrDefault() as AppSettingsAttribute;
                if (appSetting != null)
                {
                    field.SetValue(this, Convert.ChangeType(ConfigurationManager.AppSettings[appSetting.Name], field.FieldType));
                }
                else
                {
                    var connectionString = field.GetCustomAttributes(typeof(ConnectionStringAttribute)).FirstOrDefault() as ConnectionStringAttribute;
                    if (connectionString != null)
                    {
                        field.SetValue(this, Convert.ChangeType(ConfigurationManager.ConnectionStrings[connectionString.Name].ConnectionString, field.FieldType));
                    }
                }
            }
        }
    }
}
