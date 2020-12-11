using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public abstract class DocumentViewModel : DocumentPartViewModel
    {
        protected DocumentViewModel(VersionedDocument document, IVersionedDocumentService documentService)
        {
            Document = document;
            _documentService = documentService;
        }

        private readonly IVersionedDocumentService _documentService;
        public bool IsSchemaActual => _documentService.IsSchemaActual(Document);
        protected VersionedDocument Document { get; }

    }

    public abstract class DocumentPartViewModel
    {
        private string _detailedDisplayName;
        private readonly ObservableCollection<DocumentSectionViewModel> _sections = new ObservableCollection<DocumentSectionViewModel>();
        
        protected DocumentPartViewModel()
        {
            _sections.CollectionChanged += _sections_CollectionChanged;
        }

        private void _sections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var section in e.NewItems.OfType<DocumentPartViewModel>())
                        section.Parent = this;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var section in e.OldItems.OfType<DocumentPartViewModel>())
                        section.Parent = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string SystemName { get; set; }
        public string DisplayName { get; set; }

        public string DetailedDisplayName
        {
            get => _detailedDisplayName ?? DisplayName;
            set => _detailedDisplayName = value;
        }

        public abstract int DocumentId { get; }
        public abstract VersionedDocumentType DocumentType { get; }

        public string ViewName { get; set; }   
        
        public DocumentPartViewModel Parent { get; private set; }

        public ICollection<DocumentSectionViewModel> Sections
        {
            get => _sections;
            protected set
            {
                foreach (var section in _sections.ToList())
                    _sections.Remove(section);
                foreach (var item in value)
                    _sections.Add(item);
            }
        }

        public bool AllowEdit { get; set; } = true;

        public bool GetIsEditable()
        {
            if (!AllowEdit)
                return false;
            var p = Parent;
            while (p != null)
            {
                if (!p.AllowEdit)
                    return false;
                p = p.Parent;
            }

            return true;
        }
    }    
}