using System;
using System.Collections.Generic;
using System.Linq;

namespace Urfu.Its.VersionedDocs.Core
{
    public class VersionedDocumentInspectorStopProcessingException : Exception
    {    
        public VersionedDocumentInspectorStopProcessingException(IEnumerable<string> errors, IEnumerable<string> warnings)
        {
            Errors = errors.ToArray();
            Warnings = warnings.ToArray();
        }
        public string[] Errors { get; }
        public string[] Warnings { get; }

        public bool HasErrors => Errors.Any();
        public bool HasWarnings => Warnings.Any();
    }
}