using System.Collections.Generic;
using System.IO;
using System.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Loaders;
using Urfu.Its.VersionedDocs.Documents.CompetencePassports.Loaders;
using Urfu.Its.VersionedDocs.Documents.Shared;

namespace Urfu.Its.VersionedDocs.Documents.CompetencePassports
{
    public class CompetencePassportSchemaModel
    {
        /// <summary>
        /// Название документа для формирования печатных форм. Без расширения.
        /// </summary>
        [Block(LoaderType = typeof(Loaders.FileNameLoader))]
        public string FileName { get; set; }

        public EduProgramInfo EduProgramInfo { get; set; } = new EduProgramInfo();

        public InstituteInfo Institute { get; set; } = new InstituteInfo();

        public DirectionInfo2 Direction { get; set; } = new DirectionInfo2();

        public ProfileTrajectoriesInfo Profile { get; set; } = new ProfileTrajectoriesInfo();

        /// <summary>
        /// Результаты обучения
        /// </summary>
        [Block(LoaderType = typeof(EduResultsLoader))]
        public EduResults EduResults { get; set; }
    }
}