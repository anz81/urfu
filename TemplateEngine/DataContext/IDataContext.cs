namespace TemplateEngine.DataContext
{
    public interface IDataContext
    {
        ILifetime Change(ScopeChangeDescriptor descriptor);
        IScope PeekScope();
        IExpressionEvaluator Evaluator { get; }
        object Get(string path);
        IScope GetScope(string variableName);
    }
}