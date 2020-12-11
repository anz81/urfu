using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Services
{
    public class VersionedDocumentSchemaService : IVersionedDocumentSchemaService
    {
        private readonly JSchemaObjectActivator _activator = new JSchemaObjectActivator();
        private static readonly ConcurrentDictionary<string, JSchema> CachedSchemas = new ConcurrentDictionary<string, JSchema>();

        public object Create(string schema, string data = null)
        {
            var jSchema = CachedSchemas.GetOrAdd(schema, s => JSchema.Parse(schema));
            var obj = _activator.Create(data ?? "{}", jSchema);
            return obj;
        }

        public bool IsSchemaMatched(string schema1, string schema2)
        {
            var s1 = JSchema.Parse(schema2);
            var s2 = JSchema.Parse(schema1);

            var isSchemaMatched = IsSchemaMatched(s2, s1);
            return isSchemaMatched;
        }

        public bool ValidateBySchema(string data, string schema, out ValidationError[] validationErrors)
        {
            var jModel = JObject.Parse(data);
            var jSchema = JSchema.Parse(schema);

            var isValid = true;
            IList<ValidationError> schemaErrors;
            if (!jModel.IsValid(jSchema, out schemaErrors))
            {
                isValid = false;
                validationErrors = schemaErrors.ToArray();
            }
            else
            {
                validationErrors = new ValidationError[] { };
            }

            return isValid;
        }

        private bool IsSchemaMatched(JSchema s1, JSchema s2)
        {
            var type = s1.Type.GetValueOrDefault();

            if (s1.Type != s2.Type)
                return false;

            if (type.HasFlag(JSchemaType.Object))
            {
                return s1.Properties.Count == s2.Properties.Count                    
                    && s1.Properties.All(p1 => s2.Properties.TryGetValue(p1.Key, out var value2) && IsSchemaMatched(p1.Value, value2));
            }

            if (type.HasFlag(JSchemaType.Array))
            {
                return s1.Items.Count == s2.Items.Count                     
                    && !s1.Items.Where((item1, index) => !IsSchemaMatched(item1, s2.Items[index])).Any();
            }

            return true;
        }
    }
}