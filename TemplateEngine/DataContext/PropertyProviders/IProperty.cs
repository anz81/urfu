namespace TemplateEngine.DataContext.PropertyProviders
{
    public interface IProperty
    {
        string Name { get; }

        object GetValue(object instance);
    }
}