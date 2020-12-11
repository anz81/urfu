using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public class ModuleWorkingProgramViewModel : DocumentViewModel
    {
        public ModuleWorkingProgramViewModel(ModuleWorkingProgram wp, IVersionedDocumentService documentService) : base(wp.VersionedDocument, documentService)
        {
            _wp = wp;
            DisplayName = $"Модуль «{wp.Module.title}», рабочая программа, версия {wp.Version}";

            var sectionDetails = $"Модуль «{wp.Module.title}», {wp.Module.number}";

            Sections = new []
            {
                new DocumentSectionViewModel("FrontPage", "ТИТУЛЬНЫЙ ЛИСТ"),
                new DocumentSectionViewModel("TableParameters", "ПАРАМЕТРЫ ТАБЛИЦ"),
                new DocumentSectionViewModel("CommonCharacteristics", "1. ОБЩАЯ ХАРАКТЕРИСТИКА МОДУЛЯ"),
                new DocumentSectionViewModel("Structure", "2. СТРУКТУРА МОДУЛЯ И РАСПРЕДЕЛЕНИЕ УЧЕБНОГО ВРЕМЕНИ ПО ДИСЦИПЛИНАМ"),
                new DocumentSectionViewModel("DisciplineSequence", "3. ПОСЛЕДОВАТЕЛЬНОСТЬ ОСВОЕНИЯ ДИСЦИПЛИН В МОДУЛЕ"),
                new DocumentSectionViewModel("PlannedResults", "4. ПЛАНИРУЕМЫЕ РЕЗУЛЬТАТЫ ОСВОЕНИЯ МОДУЛЯ"),
                new DocumentSectionViewModel("IntermediateCertification", "5. ПРОМЕЖУТОЧНАЯ АТТЕСТАЦИЯ ПО МОДУЛЮ"),
                new DocumentSectionViewModel("ChangesList", "6. ЛИСТ РЕГИСТРАЦИИ ИЗМЕНЕНИЙ В РАБОЧЕЙ ПРОГРАММЕ МОДУЛЯ"),
                new DocumentSectionViewModel("Application1", "ПРИЛОЖЕНИЕ 1. 5.3. ФОНД ОЦЕНОЧНЫХ СРЕДСТВ ДЛЯ ПРОВЕДЕНИЯ ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО МОДУЛЮ")
            };

            foreach (var section in Sections)
                section.DetailedDisplayName = section.DisplayName + ". " + sectionDetails;

            foreach (var s in Sections)
                s.ViewName = "EditablePart";

            ViewName = "NavigationPart";
            SystemName = "ModuleWorkingProgram";
        }

        private readonly ModuleWorkingProgram _wp;

        public override int DocumentId => _wp.VersionedDocumentId;
        public override VersionedDocumentType DocumentType => VersionedDocumentType.ModuleWorkingProgram;
    }
}