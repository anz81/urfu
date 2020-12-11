using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.ViewModels
{
    public class GiaWorkingProgramViewModel : DocumentViewModel
    {
        public GiaWorkingProgramViewModel(ModuleWorkingProgram wp, IVersionedDocumentService documentService) : base(wp.VersionedDocument, documentService)
        {
            _wp = wp;
            // TODO поправить название
            DisplayName = $"ГИА «{wp.Module.title}», рабочая программа, версия {wp.Version}";

            var sectionDetails = $"ГИА «{wp.Module.title}», {wp.Module.number}";

            Sections = new List<DocumentSectionViewModel>
            {
                new DocumentSectionViewModel("FrontPage", "ТИТУЛЬНЫЙ ЛИСТ"),
                new DocumentSectionViewModel("TableParameters", "ПАРАМЕТРЫ ТАБЛИЦ"),
                new DocumentSectionViewModel("CommonCharacteristics", "1. ОБЩАЯ ХАРАКТЕРИСТИКА ГОСУДАРСТВЕННОЙ ИТОГОВОЙ АТТЕСТАЦИИ"),
                new DocumentSectionViewModel("ContentRequirement", "2. ТРЕБОВАНИЯ К СОДЕРЖАНИЮ ГОСУДАРСТВЕННОЙ ИТОГОВОЙ АТТЕСТАЦИИ"),
                new DocumentSectionViewModel("Manuals", "3. УЧЕБНО-МЕТОДИЧЕСКОЕ И ИНФОРМАЦИОННОЕ ОБЕСПЕЧЕНИЕ ГОСУДАРСТВЕННОЙ ИТОГОВОЙ АТТЕСТАЦИИ"),
                new DocumentSectionViewModel("MatTechSupport", "4. МАТЕРИАЛЬНО-ТЕХНИЧЕСКОЕ ОБЕСПЕЧЕНИЕ ГОСУДАРСТВЕННОЙ ИТОГОВОЙ АТТЕСТАЦИИ"),
                
            };

            foreach (var section in Sections)
                section.DetailedDisplayName = section.DisplayName + ". " + sectionDetails;

            foreach (var s in Sections)
                s.ViewName = "EditablePart"; // название представления из папки Urfu.Its.Web\Views\Document, 
            // которое нужно отобразить для модели представления

            // EditablePart - представление редактируемого раздела
            // NavigationPart - представление навигационной страницы, на которой отображается список разделов/подразделов и кнопка печати

            ViewName = "NavigationPart";

            SystemName = "GiaWorkingProgram"; // название папки, в которой лежат js-файлы представлений разделов
        }

        private readonly ModuleWorkingProgram _wp;

        public override int DocumentId => _wp.VersionedDocumentId;
        public override VersionedDocumentType DocumentType => VersionedDocumentType.GiaWorkingProgram;        
    }
}