using System.Collections.Generic;

namespace TemplateEngine.DataContext
{
    public interface IExpressionEvaluator
    {
        object Eval(string path, IDictionary<string, object> variables);
    }
}