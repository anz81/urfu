using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Services;

namespace Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders
{
    public class JsonContentBuilder : IBlockItemContentBuilder
    {
        public JsonContentBuilder(IVersionedDocumentBlockItemDescriptor blockDescriptor)
        {
            _blockDescriptor = blockDescriptor;
        }

        private static readonly JSchemaObjectActivator Activator = new JSchemaObjectActivator();
        private readonly IVersionedDocumentBlockItemDescriptor _blockDescriptor;
        
        public object BuildDefaultContent()
        {
            var data = _blockDescriptor.DefaultContent;
            switch (_blockDescriptor.Kind)
            {
                case VersionedDocumentBlockItemKind.Boolean:
                    return bool.Parse(data);
                case VersionedDocumentBlockItemKind.Boolean | VersionedDocumentBlockItemKind.Null:
                    if(bool.TryParse(data, out var b))
                        return b;
                    return null;
                case VersionedDocumentBlockItemKind.Integer:
                    return int.Parse(data);
                case VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null:
                    if (int.TryParse(data, out var i))
                        return i;
                    return null;
                case VersionedDocumentBlockItemKind.Number:
                    return decimal.Parse(data);
                case VersionedDocumentBlockItemKind.Number | VersionedDocumentBlockItemKind.Null:
                    if (decimal.TryParse(data, out var n))
                        return n;
                    return null;
                case VersionedDocumentBlockItemKind.String:
                    return data;
            }

            var schema = _blockDescriptor.GenerateSchema();
            var obj = Activator.Create(data, schema);
            return obj;
        }
    }

    public class StructuralContentBuilder : IBlockItemContentBuilder
    {
        private static readonly JSchemaObjectActivator Activator = new JSchemaObjectActivator();
        private readonly IVersionedDocumentBlockItemDescriptor _itemDescriptor;
        
        public StructuralContentBuilder(IVersionedDocumentBlockItemDescriptor itemDescriptor)
        {
            _itemDescriptor = itemDescriptor;
        }

        public object BuildDefaultContent()
        {
            switch (_itemDescriptor.Kind)
            {
                case VersionedDocumentBlockItemKind.Boolean:
                    return default(bool);
                case VersionedDocumentBlockItemKind.Boolean | VersionedDocumentBlockItemKind.Null:
                    return default(bool?);
                case VersionedDocumentBlockItemKind.Integer:
                    return default(int);
                case VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null:
                    return default(int?);
                case VersionedDocumentBlockItemKind.Number:
                    return default(decimal);
                case VersionedDocumentBlockItemKind.Number | VersionedDocumentBlockItemKind.Null:
                    return default(decimal?);
                case VersionedDocumentBlockItemKind.String:
                    return default(string);
            }

            if (_itemDescriptor.Kind.HasFlag(VersionedDocumentBlockItemKind.Null))
                return null;

            string json = null;
            switch (_itemDescriptor.Kind)
            {
                case VersionedDocumentBlockItemKind.Object:
                    json = "{}";
                    break;
                case VersionedDocumentBlockItemKind.Array:
                    json = "[]";
                    break;
            }

            var schema = _itemDescriptor.GenerateSchema();
            var obj = Activator.Create(json, schema);            

            return obj;
        }
    }
}