using System.Collections.Generic;
using System.IO;
using System.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Loaders;
using Urfu.Its.VersionedDocs.Documents.CompetencePassports.Loaders;
using Urfu.Its.VersionedDocs.Documents.ModuleAnnotations.Loaders;
using Urfu.Its.VersionedDocs.Documents.Shared;

namespace Urfu.Its.VersionedDocs.Documents.ModuleAnnotations
{
    public class ModuleAnnotationSchemaModel
    {
        /// <summary>
        /// Название документа для формирования печатных форм. Без расширения.
        /// </summary>
        [Block(LoaderType = typeof(Loaders.FileNameLoader))]
        public string FileName { get; set; }

        public InstituteInfo Institute { get; set; } = new InstituteInfo();

        public DirectionInfo2 Direction { get; set; } = new DirectionInfo2();

        public ProfileTrajectoriesInfo Profile { get; set; } = new ProfileTrajectoriesInfo();

        [Block(LoaderType = typeof(EduProgramHeadLoader))]
        public AuthorInfo EduProgramHead { get; set; } = new AuthorInfo();

        [Block(LoaderType = typeof(PlanLoader))]
        public PlanShortInfo Plan { get; set; } = new PlanShortInfo();

        [Block(LoaderType = typeof(DescriptionLoader))]
        public string Description { get; set; }
        
        [Block(LoaderType = typeof(ModulesLoader))]
        public ModuleAnnotationStructure ModuleAnnotations { get; set; } = new ModuleAnnotationStructure();
    }
}