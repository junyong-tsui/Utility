using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;

namespace Utility
{
    public class Setting
    {
        public static string getAppSetting(string key)
        {
            //Load the appsettings
            Configuration config = ConfigurationManager.OpenExeConfiguration(
                                    System.Reflection.Assembly.GetCallingAssembly().Location);
            //Return the value which matches the key
            return config.AppSettings.Settings[key].Value;
        }

        public static void setAppSetting(string key, string value)
        {
            //Load appsettings
            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetCallingAssembly().Location);
            //Check if key exists in the settings
            if (config.AppSettings.Settings[key] != null)
            {
                //If key exists, delete it
                config.AppSettings.Settings.Remove(key);
            }
            //Add new key-value pair
            config.AppSettings.Settings.Add(key, value);
            //Save the changed settings
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static string getWebAppSetting(string key)
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection appSettingsSection = (AppSettingsSection)configuration.GetSection("appS ettings");
            if (appSettingsSection != null)
            {
                return appSettingsSection.Settings[key].Value;
            }

            return string.Empty;
        }

        public static void setWebAppSetting(string key, string value)
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection appSettingsSection = (AppSettingsSection)configuration.GetSection("appS ettings");
            if (appSettingsSection != null)
            {
                appSettingsSection.Settings[key].Value = value;
                configuration.Save();
            }
        }
    }
}
