using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Configuration
{
    public class JsonConfigurationReader : IConfigurationReader
    {
        private string _jsonBody = null;
        private JObject _jsonRootObject = null;

        public JsonConfigurationReader(string JsonTextBody)
        {
            this._jsonBody = JsonTextBody;
            this._jsonRootObject = JObject.Parse(JsonTextBody);
        }

        public static JsonConfigurationReader FromFile(string JsonFile)
        {
            var stream = File.OpenRead(JsonFile);
            var reader = new StreamReader(stream);
            string jsonBody = reader.ReadToEnd();
            reader.Close();

            return new JsonConfigurationReader(jsonBody);
        }

        public string GetValueFromPath(string Path, bool ErrorIfNotFound = false)
        {
            var token = this._jsonRootObject.SelectToken(Path, ErrorIfNotFound);
            return token.Value<string>();
        }

        public T GetFromPath<T>(string Path, bool ErrorIfNotFound = false) where T: class, new()
        {
            var token = this._jsonRootObject.SelectToken(Path, ErrorIfNotFound);

            T item = new T();
            PropertyInfo[] props = typeof(T).GetProperties();    
        
            foreach (var prop in props)
            {
                var itemToken = token.Children().Where(
                    c => string.Compare(c.Path.Replace(token.Path + ".", ""), prop.Name, true) == 0);

                if (itemToken.Any())
                    prop.SetValue(item, itemToken.First().ToObject(prop.PropertyType), null);
            }

            return item;
        }

        public string GetConnectionStringFromName(string Name)
        {
            var token = this._jsonRootObject.SelectToken("connectionStrings." + Name);

            if (token == null)
                return null;
            else
                return token.Value<string>();
        }

        public string GetSettingFromName(string Name)
        {
            var token = this._jsonRootObject.SelectToken("appSettings." + Name);

            if (token == null)
                return null;
            else
                return token.Value<string>();
        }
    }
}
