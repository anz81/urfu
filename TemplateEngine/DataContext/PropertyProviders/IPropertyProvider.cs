using System.Collections.Generic;

namespace TemplateEngine.DataContext.PropertyProviders
{
    public interface IPropertyProvider
    {
        IEnumerable<IProperty> GetProperties();
    }
}