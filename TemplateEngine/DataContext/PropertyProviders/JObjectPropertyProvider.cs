using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TemplateEngine.DataContext.PropertyProviders
{
    public class JObjectPropertyProvider : IPropertyProvider
    {
        private readonly List<IProperty> _properties;

        public JObjectPropertyProvider(JToken obj)
        {
            _properties = obj.Children().OfType<JProperty>().Select(t => new JTokenProperty(t.Name)).Cast<IProperty>().ToList();
        }

        public IEnumerable<IProperty> GetProperties()
        {
            return _properties;
        }

        private class JTokenProperty : IProperty
        {
            public JTokenProperty(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public object GetValue(object instance)
            {
                return ((JObject)instance)[Name];
            }
        }
    }
}