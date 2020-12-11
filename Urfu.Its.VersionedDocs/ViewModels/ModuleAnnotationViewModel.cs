using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public class ModuleAnnotationViewModel : DocumentViewModel
    {
        public ModuleAnnotationViewModel(ModuleAnnotation annotation, IVersionedDocumentService documentService) : base(annotation.VersionedDocument, documentService)
        {
            _annotation = annotation;
            DisplayName = $"Образовательная программа «{annotation.BasicCharacteristicOP.Info.Profile.NAME}», аннотация модулей";

            var sectionDetails = $"Образовательная программа «{annotation.BasicCharacteristicOP.Info.Profile.NAME}»";

            Sections = new []
            {
                new DocumentSectionViewModel("Annotation", "Аннотация модулей")
            };

            foreach (var section in Sections)
                section.DetailedDisplayName = section.DisplayName + ". " + sectionDetails;

            foreach (var s in Sections)
                s.ViewName = "EditablePart";

            ViewName = "NavigationPart";
            SystemName = "ModuleAnnotation";
        }

        private readonly ModuleAnnotation _annotation;

        public override int DocumentId => _annotation.VersionedDocumentId;
        public override VersionedDocumentType DocumentType => VersionedDocumentType.ModuleAnnotation;
    }
}