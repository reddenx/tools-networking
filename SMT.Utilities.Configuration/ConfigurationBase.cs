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
			var configurationErrorMessages = new List<string>();
			var fields = this.GetType().GetFields();

			foreach (var field in fields)
			{
				//try app settings
				var appSetting = field.GetCustomAttributes(typeof(AppSettingsAttribute)).FirstOrDefault() as AppSettingsAttribute;
				if (appSetting != null)
				{
					var configValue = ConfigurationManager.AppSettings[appSetting.Name];
					if (configValue == null)
					{
						configurationErrorMessages.Add(
							string.Format("AppSetting:{0}", appSetting.Name));
					}
					else
					{
						field.SetValue(this, Convert.ChangeType(configValue, field.FieldType));
					}
				}
				else //try connection strings
				{
					var connectionString = field.GetCustomAttributes(typeof(ConnectionStringAttribute)).FirstOrDefault() as ConnectionStringAttribute;
					if (connectionString != null)
					{
						var configValue = ConfigurationManager.ConnectionStrings[connectionString.Name];
						if (configValue == null)
						{
							configurationErrorMessages.Add(
								string.Format("AppSetting:{0}", appSetting.Name));
						}
						else
						{
							field.SetValue(this, Convert.ChangeType(ConfigurationManager.ConnectionStrings[connectionString.Name].ConnectionString, field.FieldType));
						}
					}
				}
			}

			if(configurationErrorMessages.Any())
			{
				var compositeMessage = string.Format("Missing Configuration Value(s): {0}", string.Join(", ", configurationErrorMessages));
				throw new ConfigurationException(compositeMessage);
			}
		}
	}
}
