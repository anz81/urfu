using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Urfu.Its.VersionedDocs.Core
{
    public static class VersionedDocumentDescriptorExtensions
    {
        public static string GenerateSchemaString(this VersionedDocumentDescriptor descriptor)
        {
            return GenerateSchema(descriptor).ToString();
        }

        public static JSchema GenerateSchema(this VersionedDocumentDescriptor descriptor)
        {
            var schema = new JSchema
            {
                Type = JSchemaType.Object,
                AllowAdditionalProperties = false                
            };
            foreach (var blockDescriptor in descriptor.Blocks)
            {
                schema.Properties.Add(blockDescriptor.Name, GenerateSchema(blockDescriptor));
                schema.Required.Add(blockDescriptor.Name);
            }

            return schema;
        }

        public static JSchema GenerateSchema(this IVersionedDocumentBlockItemDescriptor itemDescriptor)
        {
            var schema = new JSchema {Type = (JSchemaType?) itemDescriptor.Kind};

            if (itemDescriptor.Kind.HasFlag(VersionedDocumentBlockItemKind.Array))
            {
                var itemSchema = GenerateSchema(itemDescriptor.Items);
                schema.Items.Add(itemSchema);
            }
            else if (itemDescriptor.Kind.HasFlag(VersionedDocumentBlockItemKind.Object))
            {
                schema.AllowAdditionalProperties = false;
                foreach (var propertyDescriptor in itemDescriptor.Properties)
                {
                    var propertySchema = GenerateSchema(propertyDescriptor);
                    schema.Properties.Add(propertyDescriptor.Name, propertySchema);
                    schema.Required.Add(propertyDescriptor.Name);
                }
            }

            return schema;
        }
    }
}