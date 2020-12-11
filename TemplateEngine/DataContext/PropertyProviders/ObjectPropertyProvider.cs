using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace TemplateEngine.DataContext.PropertyProviders
{
    public class ObjectPropertyProvider : IPropertyProvider
    {
        private readonly List<IProperty> _properties;

        public ObjectPropertyProvider(object obj)
        {
            _properties = obj.GetType().GetProperties()
                .Where(p =>
                    p.DeclaringType != typeof(JObject)
                    && p.DeclaringType != typeof(JContainer)
                    && p.DeclaringType != typeof(JToken)
                    && p.DeclaringType != typeof(JValue)
                    && p.DeclaringType != typeof(string)
                    && p.DeclaringType != typeof(Array)
                    && p.DeclaringType != typeof(DynamicClass)
                    && (!p.DeclaringType.IsGenericType || p.DeclaringType.GetGenericTypeDefinition() != typeof(List<>)))
                .Select(p => new ReflectionProperty(p))
                .Cast<IProperty>()
                .ToList();
        }

        public IEnumerable<IProperty> GetProperties()
        {
            return _properties;
        }

        private class ReflectionProperty : IProperty
        {
            private readonly PropertyInfo _property;

            public ReflectionProperty(PropertyInfo property)
            {
                _property = property;                
            }

            public string Name => _property.Name;

            public object GetValue(object instance)
            {
                return _property.GetValue(instance);
            }
        }
    }
}