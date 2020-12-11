using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;
using TemplateEngine.DataContext.PropertyProviders;

namespace TemplateEngine.DataContext
{
    public class DynamicLinqExpressionEvaluator : IExpressionEvaluator
    {
        private readonly Dictionary<object, Type> _cache = new Dictionary<object, Type>();

        public object Eval(string path, IDictionary<string, object> variables)
        {
            var type = BuildDynamicType(variables);
            dynamic obj = BuildObject(variables, type);

            var result = obj[path]; // предполагаем, что нужное значение уже есть в объекте
            if (result == null) // иначе вычисляем его (в случаях, если есть уловие, обращение к объекту внутри объекта и тд)
            {
                var expr = DynamicExpressionParser.ParseLambda(type, typeof(object), path);
                result = expr.Compile().DynamicInvoke(obj);
            }

            return result;
        }

        private static object BuildObject(IDictionary<string, object> variables, Type type)
        {
            var obj = Activator.CreateInstance(type);
            var properties = GetTypeProps(type).ToDictionary(p => p.Name);
            foreach (var var in variables)
            {
                var value = variables[var.Key];
                var property = properties[var.Key];
                if (value is JObject jValue)
                {
                    var instance = BuildObject(
                        GetTypeProps(property.PropertyType).ToDictionary(p => p.Name, p => (object)jValue[p.Name]),
                        property.PropertyType);
                    
                    value = instance;                    
                }
                else if (value is JArray jArray)
                {
                    var instance = jArray.Select(t=>
                    {
                        var itemProperties = new JObjectPropertyProvider(t).GetProperties().ToDictionary(p => p.Name, p => p.GetValue(t));
                        var item = BuildObject(itemProperties, property.PropertyType);
                        return item;
                    }).ToList();
                    value = instance;
                }
                else if (value is IScope scope)
                    value = BuildObject(scope.GetParameters().ToDictionary(v => v.Key, v => v.Value), property.PropertyType);
                property.SetValue(obj, value);
            }
            return obj;
        }

        private static IEnumerable<PropertyInfo> GetTypeProps(Type type)
        {
            return type.GetProperties().Where(p=>p.Name != "Item");
        }

        /*private static object BuildObject(JObject obj, Type type)
        {
            var properties = new JObjectPropertyProvider(obj).GetProperties().ToDictionary(p => p.Name);
            foreach (var var in variables)
            {
                var value = variables[var.Key];
                var property = properties[var.Key];
                if (value is JObject)
                    value = BuildObject()
                if (value is IScope scope)
                    value = BuildObject(scope.GetParameters().ToDictionary(v => v.Key, v => v.Value), property.PropertyType);
                property.SetValue(obj, value);
            }
            return obj;
        }*/

        private Type BuildDynamicType(IEnumerable<KeyValuePair<string, object>> vars)
        {
            var type = DynamicClassFactory.CreateType(vars.Select(v =>
            {
                Type varType;
                if (v.Value == null)
                    varType = typeof(object);
                else if (!_cache.TryGetValue(v.Value, out varType))
                {
                    if (v.Value is JObject jObject)
                        varType = BuildDynamicType(jObject);
                    else if (v.Value is JArray jArray)
                        varType = typeof(List<object>);
                    else if (v.Value is IScope scope)
                        varType = BuildDynamicType(scope.GetParameters());
                    else
                        varType = v.Value?.GetType();
                    _cache.Add(v.Value, varType);
                }
                return new DynamicProperty(v.Key, varType);
            }).ToArray());
            return type;
        }

        private Type BuildDynamicType(JObject jObject)
        {
            var propertyProvider = new JObjectPropertyProvider(jObject);
            var properties = propertyProvider.GetProperties();
            var type = DynamicClassFactory.CreateType(properties.Select(p =>
            {
                Type varType;
                var value = p.GetValue(jObject);
                var jObjectProperty = value as JObject;
                if (jObjectProperty == null)
                {
                    //var jTokenProperty = value as JToken;
                    //jTokenProperty.
                    varType = typeof(object);
                }
                else
                    varType = BuildDynamicType(jObjectProperty);
                return new DynamicProperty(p.Name, varType);
            }).ToArray());
            return type;
        }
    }
}