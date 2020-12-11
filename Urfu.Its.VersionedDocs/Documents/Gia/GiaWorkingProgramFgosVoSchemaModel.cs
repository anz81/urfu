using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.Module.Processors;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.VersionedDocs.Documents.Shared.Loaders;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;
using FileNameLoader = Urfu.Its.VersionedDocs.Documents.Gia.Loaders.FileNameLoader;

namespace Urfu.Its.VersionedDocs.Documents.Gia
{
    public class GiaWorkingProgramFgosVoSchemaModel
    {
        /// <summary>
        /// Название документа для формирования печатных форм. Без расширения.
        /// </summary>
        [Block(LoaderType = typeof(FileNameLoader))]
        public string FileName { get; set; }

        #region FrontPage

        [Block(LoaderType = typeof(ModuleInfoLoader))]
        public ModuleInfo Module { get; set; } = new ModuleInfo();

        [Block(LoaderType = typeof(InstituteLoader))]
        [DependentBlock(nameof(Directions))]
        public InstituteInfo Institute { get; set; } = new InstituteInfo();

        /// <summary>
        /// Направления
        /// </summary>
        [Block(LoaderType = typeof(DirectionsLoader))]
        [DependentBlock(nameof(Profiles))]
        public ICollection<DirectionInfo> Directions { get; set; } = new List<DirectionInfo>();

        [Block(LoaderType = typeof(ProfilesLoader))]
        public ICollection<ProfileTrajectoriesInfo> Profiles { get; set; } = new List<ProfileTrajectoriesInfo>();

        [Block(LoaderType = typeof(AuthorsLoader))]
        public ICollection<AuthorInfo> Authors { get; set; } = new List<AuthorInfo>();

        [Block(LoaderType = typeof(WorkingProgramPersonLoader))]
        public WorkingProgramPersonInfo EduProgramHead { get; set; } = new WorkingProgramPersonInfo();


        [Block(LoaderType = typeof(EducationalMethodicalCouncilLoader))]
        public EducationalMethodicalCouncilInfo Council { get; set; } = new EducationalMethodicalCouncilInfo();

        /// <summary>
        /// Дирекция образовательных программ
        /// </summary>
        [Block(LoaderType = typeof(WorkingProgramPersonLoader))]
        public WorkingProgramPersonInfo Direction { get; set; } = new WorkingProgramPersonInfo();

        [Block(LoaderType = typeof(RequisitesOrdersFgosLoader))]
        public ICollection<RequisitesOrderFgosInfo> RequisitesOrders { get; set; } =
            new List<RequisitesOrderFgosInfo>();

        #endregion
        [Block(ProcessorType = typeof(FdpsProcessor))]
        public ICollection<FamilirizationTypeDirectionPlanInfo> Fdps { get; set; } = new List<FamilirizationTypeDirectionPlanInfo>();

        #region CommonCharacteristics 

        /// <summary>
        /// 1.1.    Цель государственной итоговой аттестации
        /// </summary>

        public ICollection<PlannedResultItemInfo> PlannedResults { get; set; } = new List<PlannedResultItemInfo>();

        public GiaStructure GiaStructure { get; set; } = new GiaStructure();
        public string TotalLabor { get; set; }
        public string TimeOfGia { get; set; }
        public string ProcedureRequirement { get; set; }
        [Block(LoaderType = typeof(ProtocolLoader))]
        public Protocol EvalutionReuqirementProtocol { get; set; } = new Protocol();
        #endregion

        #region ContentRequirement
        /// <summary>
        /// 2.1.	Тематика государственного экзамена .
        /// </summary>
        public ICollection<EvaluationToolInfo> ExamSubject { get; set; } = new List<EvaluationToolInfo>();
        /// <summary>
        /// 2.2. Тематика выпускных квалификационных работ 
        /// </summary>
        public string QualificationWorkSubject { get; set; }
        #endregion

        #region Manuals

        /// <summary>
        /// 3.1 Рекомендуемая литература
        /// </summary>
        public LiteratureInfo Literature { get; set; }
        /// <summary>
        /// 3.2 Методические разработки
        /// </summary>
        public string MethodicalSupport { get; set; }
        /// <summary>
        /// 3.3.Программное обеспечение
        /// </summary>
        public SoftwareInfo Software { get; set; } = new SoftwareInfo();
        /// <summary>
        /// 3.4. Базы данных, информационно-справочные и поисковые системы
        /// </summary>
        public string Databases { get; set; }

        /// <summary>
        /// 3.5 Электронные образовательные ресурсы
        /// </summary>
        public string ElectronicEducationalResources { get; set; }
        #endregion

        #region MatTechSupport

        public string MatTechSupport { get; set; }

        #endregion
    }

    public class GiaStructure
    {
        public Protocol GiaStructureProtocol { get; set; } = new Protocol();
        public string QualificationWork { get; set; }
    }
}