using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public class PracticesWorkingProgramViewModel : DocumentViewModel
    {
        public PracticesWorkingProgramViewModel(ModuleWorkingProgram wp, IVersionedDocumentService documentService) : base(wp.VersionedDocument, documentService)
        {
            _wp = wp;
            // TODO поправить название
            DisplayName = $"Практики «{wp.Module.title}», рабочая программа, версия {wp.Version}";

            var sectionDetails = $"Практика «{wp.Module.title}», {wp.Module.number}";

            Sections = new List<DocumentSectionViewModel>
            {
                new DocumentSectionViewModel("FrontPage", "ТИТУЛЬНЫЙ ЛИСТ"),
                new DocumentSectionViewModel("TableParameters", "ПАРАМЕТРЫ ТАБЛИЦ"),
                new DocumentSectionViewModel("CommonCharacteristics", "1. ОБЩАЯ ХАРАКТЕРИСТИКА ПРАКТИК"),
                new DocumentSectionViewModel("Content", "2. СОДЕРЖАНИЕ ПРАКТИК"),
                new DocumentSectionViewModel("EvalutionStudentPractice", "3 .ОЦЕНИВАНИЕ УЧЕБНОЙ ДЕЯТЕЛЬНОСТИ СТУДЕНТОВ И ЕЕ ДОСТИЖЕНИЙ В ХОДЕ ПРОХОЖДЕНИЯ ПРАКТИК"),
                new DocumentSectionViewModel("EvalutionTools", "4. ФОНД ОЦЕНОЧНЫХ СРЕДСТВ ДЛЯ ПРОВЕДЕНИЯ ТЕКУЩЕЙ И ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО ПРАКТИКАМ "),
                new DocumentSectionViewModel("Manuals", "5.УЧЕБНО-МЕТОДИЧЕСКОЕ И ИНФОРМАЦИОННОЕ ОБЕСПЕЧЕНИЕ ПРОХОЖДЕНИЯ ПРАКТИК"),
                new DocumentSectionViewModel("MatTechSupport", "6. МАТЕРИАЛЬНО-ТЕХНИЧЕСКОЕ ОБЕСПЕЧЕНИЕ ПРАКТИКИ"),

                // TODO определить остальные разделы
                // NOTE SystemName у DocumentSectionViewModel является названием js файла в папке
                // Urfu.Its.Web\Scripts\VersionedDocs\PracticesWorkingProgram, который будет подгружаться при открытии раздела
            };

            foreach (var section in Sections)
                section.DetailedDisplayName = section.DisplayName + ". " + sectionDetails;

            foreach (var s in Sections)
                s.ViewName = "EditablePart"; // название представления из папки Urfu.Its.Web\Views\Document, 
            // которое нужно отобразить для модели представления

            // EditablePart - представление редактируемого раздела
            // NavigationPart - представление навигационной страницы, на которой отображается список разделов/подразделов и кнопка печати

            ViewName = "NavigationPart";

            SystemName = "PracticesWorkingProgram"; // название папки, в которой лежат js-файлы представлений разделов
        }

        private readonly ModuleWorkingProgram _wp;

        public override int DocumentId => _wp.VersionedDocumentId;
        public override VersionedDocumentType DocumentType => VersionedDocumentType.PracticesWorkingProgram;        
    }
}