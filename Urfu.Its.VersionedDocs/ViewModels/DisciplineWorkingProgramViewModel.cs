using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public class DisciplineWorkingProgramViewModel : DocumentViewModel
    {
        public DisciplineWorkingProgramViewModel(DisciplineWorkingProgram wp, IVersionedDocumentService documentService) : base(wp.VersionedDocument, documentService)
        {
            _wp = wp;
            DisplayName = $"Дисциплина «{wp.Discipline.title}», рабочая программа, версия {wp.Version}";

            ModuleWorkingProgramId = _wp.ModuleWorkingProgram.VersionedDocumentId;

            var sectionDetails = $"Дисциплина «{wp.Discipline.title}», {wp.Discipline.number}";

            Sections = new List<DocumentSectionViewModel>
            {
                new DocumentSectionViewModel("FrontPage", "ТИТУЛЬНЫЙ ЛИСТ"),
                new DocumentSectionViewModel("CommonCharacteristics", "1. ОБЩАЯ ХАРАКТЕРИСТИКА ДИСЦИПЛИНЫ"),
                new DocumentSectionViewModel("Content", "2. СОДЕРЖАНИЕ ДИСЦИПЛИНЫ"),
                new DocumentSectionViewModel("TimeDistribution", "3. РАСПРЕДЕЛЕНИЕ УЧЕБНОГО ВРЕМЕНИ"),
                new DocumentSectionViewModel("PracticesAndHomeworks", "4. ОРГАНИЗАЦИЯ ПРАКТИЧЕСКИХ ЗАНЯТИЙ, САМОСТОЯТЕЛЬНОЙ РАБОТЫ ПО ДИСЦИПЛИНЕ"),
                new DocumentSectionViewModel("LearningMethods", "5. СООТНОШЕНИЕ РАЗДЕЛОВ, ТЕМ ДИСЦИПЛИНЫ И ПРИМЕНЯЕМЫХ ТЕХНОЛОГИЙ ОБУЧЕНИЯ"),
                new DocumentSectionViewModel("Application1", "6. ПРОЦЕДУРЫ КОНТРОЛЯ И ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ОБУЧЕНИЯ (Приложение 1)"),
                new DocumentSectionViewModel("Application2", "7. ПРОЦЕДУРЫ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ОБУЧЕНИЯ В РАМКАХ НЕЗАВИСИМОГО ТЕСТОВОГО КОНТРОЛЯ (Приложение 2)"),
                new DocumentSectionViewModel("Application3", "8. ФОНД ОЦЕНОЧНЫХ СРЕДСТВ ДЛЯ ПРОВЕДЕНИЯ ТЕКУЩЕЙ И ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО ДИСЦИПЛИНЕ (Приложение 3)"),
                new DocumentSectionViewModel("InformationSupport", "9. УЧЕБНО-МЕТОДИЧЕСКОЕ И ИНФОРМАЦИОННОЕ ОБЕСПЕЧЕНИЕ ДИСЦИПЛИНЫ"),
                new DocumentSectionViewModel("TechnicalSupport", "10. МАТЕРИАЛЬНО-ТЕХНИЧЕСКОЕ ОБЕСПЕЧЕНИЕ ДИСЦИПЛИНЫ"),
            };

            foreach (var section in Sections)
                section.DetailedDisplayName = section.DisplayName + ". " + sectionDetails;

            foreach (var s in Sections)
                s.ViewName = "EditablePart";

            ViewName = "DisciplineNavigationPart";
            SystemName = "DisciplineWorkingProgram";
        }

        private readonly DisciplineWorkingProgram _wp;

        public override int DocumentId => _wp.VersionedDocumentId;
        public override VersionedDocumentType DocumentType => VersionedDocumentType.DisciplineWorkingProgram;
        public int ModuleWorkingProgramId { get; set; }
    }
}