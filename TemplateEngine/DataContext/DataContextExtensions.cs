using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TemplateEngine.DataContext
{
    public static class DataContextExtensions
    {
        public static ILifetime AddIteratingItemToScope(this IDataContext dataContext, int index, object data, string name = null)
        {
            var iteratorValueProvider = new IteratingItemScope(dataContext.PeekScope(), dataContext.Evaluator, data, index);
            var lifetime = dataContext.Change(new ScopeChangeDescriptor(iteratorValueProvider, name));
            
            return lifetime;
        }
    }
}