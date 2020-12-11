using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public class BasicCharacteristicViewModel : DocumentViewModel
    {
        public BasicCharacteristicViewModel(BasicCharacteristicOP ohop, IVersionedDocumentService documentService) : base(ohop.VersionedDocument, documentService)
        {
            _ohop = ohop;
            DisplayName = $"Образовательная программа «{ohop.Info.Profile.NAME}», ОХОП, версия {ohop.Version}";

            var sectionDetails = $"Образовательная программа «{ohop.Info.Profile.NAME}»";

            Sections = new []
            {
                new DocumentSectionViewModel("FrontPage", "ТИТУЛЬНЫЙ ЛИСТ"),
                new DocumentSectionViewModel("Dictionary", "ТЕРМИНЫ И ОПРЕДЕЛЕНИЯ"),
                new DocumentSectionViewModel("CommonCharacteristics", "1. Общие положения"),
                new DocumentSectionViewModel("ProfActivityCharacteristics", "2. Характеристика профессиональной деятельности выпускников и описание траекторий образовательной программы"),
                new DocumentSectionViewModel("PlannedResults", "3. Планируемые результаты освоения образовательной программы"),
                new DocumentSectionViewModel("ModuleStructure", "4. Структура образовательной программы"),
                new DocumentSectionViewModel("Conditions", "5. Условия реализации образовательной программы"),
                new DocumentSectionViewModel("ProfStandartsList", "ПРИЛОЖЕНИЕ 1"),
                new DocumentSectionViewModel("ApprovalActs", "ПРИЛОЖЕНИЕ 2"),
                new DocumentSectionViewModel("Files", "ПРИЛОЖЕНИЕ 3"),
            };

            foreach (var section in Sections)
                section.DetailedDisplayName = section.DisplayName + ". " + sectionDetails;

            foreach (var s in Sections)
                s.ViewName = "EditablePart";

            ViewName = "NavigationPart";
            SystemName = "BasicCharacteristicOP";
        }

        private readonly BasicCharacteristicOP _ohop;

        public override int DocumentId => _ohop.VersionedDocumentId;
        public override VersionedDocumentType DocumentType => VersionedDocumentType.BasicCharacteristicOP;
    }
}