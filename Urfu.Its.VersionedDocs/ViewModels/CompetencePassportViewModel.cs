using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public class CompetencePassportViewModel : DocumentViewModel
    {
        public CompetencePassportViewModel(CompetencePassport passport, IVersionedDocumentService documentService) : base(passport.VersionedDocument, documentService)
        {
            _passport = passport;
            DisplayName = $"Образовательная программа «{passport.BasicCharacteristicOP.Info.Profile.NAME}», паспорт компетенций, версия {passport.Version}";

            var sectionDetails = $"Образовательная программа «{passport.BasicCharacteristicOP.Info.Profile.NAME}»";

            Sections = new []
            {
                new DocumentSectionViewModel("Passport", "Результаты обучения для УК и ОПК"),
                new DocumentSectionViewModel("PassportPC", "Результаты обучения для ПК")
            };

            foreach (var section in Sections)
                section.DetailedDisplayName = section.DisplayName + ". " + sectionDetails;

            foreach (var s in Sections)
                s.ViewName = "EditablePart";

            ViewName = "NavigationPart";
            SystemName = "CompetencePassport";
        }

        private readonly CompetencePassport _passport;

        public override int DocumentId => _passport.VersionedDocumentId;
        public override VersionedDocumentType DocumentType => VersionedDocumentType.CompetencePassport;
    }
}