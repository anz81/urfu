using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TemplateEngine.DataContext
{
    public class DocumentDataContext : IDataContext
    {
        private readonly List<IScope> Scopes = new List<IScope>();

        public DocumentDataContext(IExpressionEvaluator evaluator)
        {
            Evaluator = evaluator;
        }

        public IExpressionEvaluator Evaluator { get; }

        public ILifetime Change(ScopeChangeDescriptor descriptor)
        {
            var currentScope = Scopes.LastOrDefault();
            var scope = descriptor.Scope ?? new Scope(currentScope, Evaluator, descriptor.Data, descriptor.Vars ?? new Dictionary<string, object>());
            
            ILifetime lifetime;
            if (descriptor.Name == null)
            {
                var index = Scopes.Count;
                Scopes.Add(scope);                
                lifetime = new Lifetime(() => Scopes.RemoveAt(index));
            }
            else
            {
                lifetime = currentScope.AddVar(descriptor.Name, scope);
            }
            
            return lifetime;
        }

        public IScope PeekScope()
        {
            return Scopes.Last();
        }

        public object Get(string path)
        {
            var value = Scopes.Last().Eval(path);
            return value;
        }

        public IScope GetScope(string variableName)
        {
            var currentScope = Scopes.Last();

            if (variableName == VariableNames.Parent)
            {
                return currentScope.ParentScope;
            }

            var scope = currentScope.GetParameters().First(p => p.Key == variableName).Value;

            if (!(scope is IScope))
            {
                return new Scope(currentScope, Evaluator, scope);
            }
            
            return scope as IScope;            
        }
    }

    public class ScopeChangeDescriptor
    {
        public ScopeChangeDescriptor(IScope scope, string name = null)
        {
            Name = name;
            Scope = scope;
        }

        public ScopeChangeDescriptor(object dataOrScope, string name = null)
        {
            Name = name;
            Scope = dataOrScope as IScope;
            Data = dataOrScope is IScope ? null : dataOrScope;
        }

        public IScope Scope { get; }
        public object Data { get; }
        public IDictionary<string, object> Vars { get; } = new Dictionary<string, object>();
        public string Name { get; }
    }

    public class Lifetime : ILifetime
    {
        private readonly Action _killAction;

        public Lifetime(Action killAction)
        {
            _killAction = killAction;
        }

        public void Dispose()
        {
            _killAction();
        }
    }

    public class MultiLifetime : ILifetime
    {
        public ILifetime[] Lifetimes { get; }

        public MultiLifetime(IEnumerable<ILifetime> lifetimes)
        {
            Lifetimes = lifetimes.ToArray();
        }

        public void Dispose()
        {
            foreach (var lifetime in Lifetimes)
            {
                lifetime.Dispose();
            }
        }
    }
}