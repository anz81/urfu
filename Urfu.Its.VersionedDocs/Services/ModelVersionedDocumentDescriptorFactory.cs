using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;

namespace Urfu.Its.VersionedDocs.Services
{
    public abstract class ModelVersionedDocumentDescriptorFactory : IVersionedDocumentDescriptorFactory
    {
        protected ModelVersionedDocumentDescriptorFactory(Type modelType)
        {
            _modelType = modelType;
        }

        private readonly Type _modelType;

        public Type DefaultContentBuilderType => typeof(StructuralContentBuilder);

        public VersionedDocumentDescriptor CreateDocumentDescriptor()
        {
            var blockDescriptors = new Dictionary<PropertyInfo, VersionedDocumentBlockDescriptor>();
            foreach (var propertyInfo in _modelType.GetProperties())
            {
                if (TryCreateBlockDescriptor(propertyInfo, out var name, out var loader, out var saver))
                {
                    var defaultContentAttribute = propertyInfo.GetCustomAttribute<DefaultContentAttribute>(true);
                    var blockDescriptor = new VersionedDocumentBlockDescriptor(name, ConvertType(propertyInfo.PropertyType))
                    {
                        NonVersionedDataLoaderType = loader,
                        ProcessorType = saver                    
                    };
                    FillItemDescriptor(blockDescriptor, propertyInfo.PropertyType, defaultContentAttribute);
                    blockDescriptors.Add(propertyInfo, blockDescriptor);
                }
            }

            foreach (var blockDescriptor in blockDescriptors)
            {
                blockDescriptor.Value.DependentBlocks = blockDescriptor.Key.GetCustomAttributes<DependentBlockAttribute>().Select(d => d.BlockName).ToList();
            }

            var descriptor = new VersionedDocumentDescriptor(blockDescriptors.Values);
            
            return descriptor;
        }

        protected abstract bool TryCreateBlockDescriptor(PropertyInfo property, out string name, out Type loader, out Type saver);

        private void FillItemDescriptor(IVersionedDocumentBlockItemDescriptor descriptor, Type type,
            DefaultContentAttribute defaultContentAttribute)
        {
            descriptor.DefaultContentBuilderType = defaultContentAttribute?.ContentBuilderType;
            if (descriptor.DefaultContentBuilderType == null)
                descriptor.DefaultContentBuilderType = DefaultContentBuilderType;
            descriptor.DefaultContent = defaultContentAttribute?.DefaultContent;

            switch (descriptor.Kind)
            {
                case VersionedDocumentBlockItemKind.Object:
                {
                    foreach (var itemProperty in type.GetProperties())
                    {
                        var childDescriptor = new VersionedDocumentBlockItemDescriptor(itemProperty.Name,
                            ConvertType(itemProperty.PropertyType));
                        var itemDefaultContentAttribute =
                            itemProperty.GetCustomAttribute<DefaultContentAttribute>(true);
                        FillItemDescriptor(childDescriptor, itemProperty.PropertyType, itemDefaultContentAttribute);
                        descriptor.Properties.Add(childDescriptor);
                    }
                    break;
                }
                case VersionedDocumentBlockItemKind.Array:
                {
                    var itemType = type.GetInterfaces().Where(i => i.IsGenericType)
                        .Select(i => new {GenericType = i.GetGenericTypeDefinition(), Type = i})
                        .FirstOrDefault(t => t.GenericType == typeof(IEnumerable<>))
                        ?.Type.GetGenericArguments().First();
                    if (itemType == null)
                        throw new InvalidOperationException("Не удалось получить тип элемента перечисления");
                    var itemDescriptor = new VersionedDocumentBlockItemDescriptor(ConvertType(itemType));
                    var itemDefaultContentAttribute = itemType.GetCustomAttribute<DefaultContentAttribute>(true);
                    FillItemDescriptor(itemDescriptor, itemType, itemDefaultContentAttribute);
                    descriptor.Items = itemDescriptor;
                    break;
                }
            }
        }           

        private VersionedDocumentBlockItemKind ConvertType(Type type)
        {
            if (type == typeof(string))
                return VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null;
            if (type == typeof(int))
                return VersionedDocumentBlockItemKind.Integer;
            if (type == typeof(int?))
                return VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null;
            if (type == typeof(decimal))
                return VersionedDocumentBlockItemKind.Number;
            if (type == typeof(decimal?))
                return VersionedDocumentBlockItemKind.Number | VersionedDocumentBlockItemKind.Null;
            if (type == typeof(bool))
                return VersionedDocumentBlockItemKind.Boolean;
            if (type == typeof(bool?))
                return VersionedDocumentBlockItemKind.Boolean | VersionedDocumentBlockItemKind.Null;
            if (type.GetInterfaces().Contains(typeof(IEnumerable)))
                return VersionedDocumentBlockItemKind.Array;
            if (type.IsEnum)
            {
                if (IsNullableType(type))
                    return VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null;
                return VersionedDocumentBlockItemKind.Integer;
            }
            if (type.IsClass)
                return VersionedDocumentBlockItemKind.Object;
            throw new NotSupportedException($"Тип '{type}' не поддерживается");
        }

        internal static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}