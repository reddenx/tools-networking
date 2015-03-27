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
                var attribute = field.GetCustomAttributes(typeof(AppSettingsAttribute)).Single() as AppSettingsAttribute;
                if (attribute != null)
                {
                    field.SetValue(this, Convert.ChangeType(ConfigurationManager.AppSettings[attribute.Name], field.FieldType));
                }
            }
        }
    }
}
