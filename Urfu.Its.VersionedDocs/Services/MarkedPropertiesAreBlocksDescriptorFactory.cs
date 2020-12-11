using System;
using System.Reflection;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;

namespace Urfu.Its.VersionedDocs.Services
{
    public class MarkedPropertiesAreBlocksDescriptorFactory<TModel> : MarkedPropertiesAreBlocksDescriptorFactory,
        IVersionedDocumentModelDescriptorFactory<TModel>
    {
        public MarkedPropertiesAreBlocksDescriptorFactory() : base(typeof(TModel))
        {
        }
    }

    public class MarkedPropertiesAreBlocksDescriptorFactory : ModelVersionedDocumentDescriptorFactory
    {
        public MarkedPropertiesAreBlocksDescriptorFactory(Type type): base(type)
        {
        }

        protected override bool TryCreateBlockDescriptor(PropertyInfo property, out string name, out Type loader, out Type saver)
        {            
            var attribute = property.GetCustomAttribute<BlockAttribute>();
            name = property.Name;
            loader = attribute?.LoaderType;
            saver = attribute?.ProcessorType;            
            return attribute != null;
        }        
    }
}