using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TOF.Framework.Configuration
{
    public class ConfigConfigurationReader : IConfigurationReader
    {
        public T GetFromPath<T>(string Path, bool ErrorIfNotFound = false) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public string GetConnectionStringFromName(string Name)
        {
            return ConfigurationManager.AppSettings[Name];
        }

        public string GetSettingFromName(string Name)
        {
            return ConfigurationManager.ConnectionStrings[Name].ConnectionString;
        }

        public string GetValueFromPath(string Path, bool ErrorIfNotFound = false)
        {
            throw new NotImplementedException();
        }
    }
}
