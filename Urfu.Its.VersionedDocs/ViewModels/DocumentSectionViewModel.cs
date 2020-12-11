using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public class DocumentSectionViewModel : DocumentPartViewModel
    {
        public DocumentSectionViewModel(string systemName, string displayName)
        {
            SystemName = systemName;
            DisplayName = displayName;
        }
        
        public override int DocumentId => Parent.DocumentId;
        public override VersionedDocumentType DocumentType => Parent.DocumentType;        
    }
}