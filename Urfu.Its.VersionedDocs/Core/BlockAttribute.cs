using System;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;

namespace Urfu.Its.VersionedDocs.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BlockAttribute : Attribute
    {
        public Type LoaderType { get; set; }
        public Type ProcessorType { get; set; }
        // Это задел для хранения структуры документа по разделам. Пока решили что не нужно.
        /*public string SectionName { get; set; }        
        public int Order { get; set; }
        public string DisplayName { get; set; }*/
    }

    /*[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SectionAttribute : Attribute
    {
        public SectionAttribute(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }                
    }*/

    public abstract class DefaultContentAttribute : Attribute
    {
        public Type ContentBuilderType { get; }
        public string DefaultContent { get; }

        public DefaultContentAttribute(string defaultContent, Type contentBuilderType)
        {
            ContentBuilderType = contentBuilderType;
            DefaultContent = defaultContent;
        }
    }

    public class CustomDefaultContentAttribute : DefaultContentAttribute
    {
        public CustomDefaultContentAttribute(Type contentBuilderType) : base(null, contentBuilderType)
        {
        }
    }

    public class StructuralDefaultContentAttribute : DefaultContentAttribute
    {
        public StructuralDefaultContentAttribute() : base(null, typeof(StructuralContentBuilder))
        {
        }
    }

    public class JsonDefaultContentAttribute : DefaultContentAttribute
    {
        public JsonDefaultContentAttribute(string defaultContent) : base(defaultContent, typeof(JsonContentBuilder))
        {
        
        }
    }
}