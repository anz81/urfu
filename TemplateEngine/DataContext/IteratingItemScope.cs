using System.Collections.Generic;
using System.Linq;

namespace TemplateEngine.DataContext
{
    public class IteratingItemScope : Scope
    {
        public int Index { get; }

        public IteratingItemScope(IScope parentScope, IExpressionEvaluator evaluator, object data, int index) 
            : base(parentScope, evaluator, data, new Dictionary<string, object>
            {
                [VariableNames.Index] = index,
                [VariableNames.Number] = index+1
            })
        {
            Index = index;
        }        
    }
}