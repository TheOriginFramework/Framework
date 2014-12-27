using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Configuration
{
    public static class ConfigurationProvider
    {
        private static Dictionary<string, IConfigurationReader> gConfigurationReaders = null;
        private static string gAppConfigurationKey = "config.json";
        private static string gFrameworkConfigurationKey = "framework.json";

        static ConfigurationProvider()
        {
            gConfigurationReaders = new Dictionary<string, IConfigurationReader>();
        }

        public static void SetConfigurationReader(string Key, IConfigurationReader Reader)
        {
            if (gConfigurationReaders.ContainsKey(Key))
                gConfigurationReaders[Key] = Reader;
            else
                gConfigurationReaders.Add(Key, Reader);
        }

        public static IConfigurationReader GetReader(string Key)
        {
            if (!gConfigurationReaders.ContainsKey(Key))
                throw new InvalidOperationException("ERROR_CONFIGURATION_PROVIDER_NOT_INITED_AT_STARTUP");

            return gConfigurationReaders[Key];
        }

        public static void SetAppConfigurationKey(string Key)
        {
            gAppConfigurationKey = Key;
        }

        public static void SetFrameworkConfigurationKey(string Key)
        {
            gFrameworkConfigurationKey = Key;
        }

        public static IConfigurationReader GetAppConfigurationReader()
        {
            return GetReader(gAppConfigurationKey);
        }

        public static IConfigurationReader GetFrameworkConfiguraitonReader()
        {
            return GetReader(gFrameworkConfigurationKey);
        }
    }
}
