using System.Collections.Generic;
using System.Linq;

namespace Urfu.Its.VersionedDocs.Core
{
    public interface IVersionedDocumentDescriptorService
    {
        IEnumerable<VersionedDocumentDescriptorChange> GetChanges(VersionedDocumentDescriptor sourceDesc, VersionedDocumentDescriptor targetDesc);

        object BuildDefaultContent(IVersionedDocumentBlockItemDescriptor itemDescriptor);
    }
    
    public class VersionedDocumentDescriptorChange
    {
        public VersionedDocumentDescriptorChange(VersionedDocumentSchemaChangeAction action, IEnumerable<IVersionedDocumentBlockItemDescriptor> chain)
        {
            Chain = chain?.ToArray() ?? new IVersionedDocumentBlockItemDescriptor[0];
            Action = action;
        }

        public VersionedDocumentSchemaChangeAction Action { get; }
        public IVersionedDocumentBlockItemDescriptor[] Chain { get; }
        
        public IVersionedDocumentBlockItemDescriptor OldItem { get; set; }      
        public IVersionedDocumentBlockItemDescriptor NewItem { get; set; }        
    }

    public enum VersionedDocumentSchemaChangeAction
    {
        ItemAdded,
        ItemRemoved,
        ItemTypeChanged
    }
}