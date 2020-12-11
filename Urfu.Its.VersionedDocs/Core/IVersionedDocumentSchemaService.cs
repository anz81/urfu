using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace Urfu.Its.VersionedDocs.Core
{
    public interface IVersionedDocumentSchemaService
    {
        object Create(string schema, string data = null);

        bool IsSchemaMatched(string schema1, string schema2);

        bool ValidateBySchema(string data, string schema, out ValidationError[] validationErrors);
    }    
}