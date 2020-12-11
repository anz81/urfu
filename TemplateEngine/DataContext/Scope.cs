using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using TemplateEngine.DataContext.PropertyProviders;

namespace TemplateEngine.DataContext
{
    public class Scope : IScope
    {
        private readonly IExpressionEvaluator _evaluator;
        private readonly IEnumerable<IProperty> _properties;

        public Scope(IScope parentScope, IExpressionEvaluator evaluator, object data, IDictionary<string, object> variables = null)
        {
            _evaluator = evaluator;
            ParentScope = parentScope;
            Variables = new Dictionary<string, object>(variables ?? new Dictionary<string, object>());
            Data = data;

            IPropertyProvider propertyProvider;
            if (data is JObject jData)
            {
                propertyProvider = new JObjectPropertyProvider(jData);
            }
            else
            {
                propertyProvider = new ObjectPropertyProvider(data);
            }

            // объект данных должнен быть простым объектом
            Debug.Assert(!(data is DynamicClass));
            Debug.Assert(!(data is IScope));

            _properties = propertyProvider.GetProperties();
        }

        public object Data { get; }

        public IScope ParentScope { get; }

        public Dictionary<string, object> Variables { get; }

        public object Eval(string path)
        {            
            var keyValuePairs = GetParameters();
            var vars = keyValuePairs.ToDictionary(v=>v.Key, v=>v.Value);            
            var value = _evaluator.Eval(path, vars);
            if (value is JValue jValue)
                value = jValue.Value;
            if (value is IScope scope)
                value = scope.Data;
            return value;
        }

        public virtual IEnumerable<KeyValuePair<string, object>> GetParameters()
        {
            var dic = new Dictionary<string, object>
            {
                {VariableNames.Parent, ParentScope?.Data},
                {VariableNames.Data, Data}
            };
            foreach (var variable in Variables)
                dic.Add(variable.Key, variable.Value);
            foreach (var p in _properties)
            {
                if(!dic.ContainsKey(p.Name))
                    dic.Add(p.Name, p.GetValue(Data));
            }
            return dic;
        }

        public ILifetime AddVars(IDictionary<string, object> vars)
        {
            foreach (var @var in vars)
            {
                Variables.Add(@var.Key, @var.Value);
            }

            return new Lifetime(() =>
            {
                foreach (var @var in vars)
                {
                    RemoveVar(@var.Key);
                }
            });
        }

        public void RemoveVar(string name)
        {
            Variables.Remove(name);
        }
    }    
}