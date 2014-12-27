using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Configuration
{
    public interface IConfigurationReader
    {
        string GetValueFromPath(string Path, bool ErrorIfNotFound = false);
        T GetFromPath<T>(string Path, bool ErrorIfNotFound = false) where T : class, new();
        string GetConnectionStringFromName(string Name);
        string GetSettingFromName(string Name);
    }
}
