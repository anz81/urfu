using System.Collections.Generic;

namespace TemplateEngine.DataContext
{
    public interface IScope
    {
        ILifetime AddVars(IDictionary<string, object> vars);
        void RemoveVar(string name);
        IEnumerable<KeyValuePair<string, object>> GetParameters();
        object Eval(string path);
        object Data { get; }
        IScope ParentScope { get; }
    }

    public static class ExpandableScopeExtensions
    {
        public static ILifetime AddVar(this IScope scope, string name, object value)
        {
            return scope.AddVars(new Dictionary<string, object>
            {
                [name] = value
            });
        }
    }
}