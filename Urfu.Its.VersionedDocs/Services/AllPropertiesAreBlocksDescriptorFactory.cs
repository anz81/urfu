using System;
using System.Collections.Generic;
using System.Reflection;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;

namespace Urfu.Its.VersionedDocs.Services
{
    public class AllPropertiesAreBlocksDescriptorFactory<TModel> : AllPropertiesAreBlocksDescriptorFactory, IVersionedDocumentModelDescriptorFactory<TModel>
    {
        public AllPropertiesAreBlocksDescriptorFactory() : base(typeof(TModel))
        {
        }
    }

    public class AllPropertiesAreBlocksDescriptorFactory : ModelVersionedDocumentDescriptorFactory
    {
        public AllPropertiesAreBlocksDescriptorFactory(Type type) : base(type)
        {
        }


        protected override bool TryCreateBlockDescriptor(PropertyInfo property, out string name, out Type loader, out Type saver)
        {
            if (!property.CanWrite)
            {
                name = null;
                loader = null;
                saver = null;                
                return false;
            }
            name = property.Name;
            var blockAttribute = property.GetCustomAttribute<BlockAttribute>();
            loader = blockAttribute?.LoaderType;
            saver = blockAttribute?.ProcessorType;            
            return true;
        }
    }
}