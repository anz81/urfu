using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Documents.Discipline.Loaders;
using Urfu.Its.VersionedDocs.Documents.Discipline.Processors;
using Urfu.Its.VersionedDocs.Documents.Module.Processors;
using Urfu.Its.VersionedDocs.Documents.Practices.Loaders;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;
using Urfu.Its.VersionedDocs.Documents.Shared.Loaders;
using FileNameLoader = Urfu.Its.VersionedDocs.Documents.Practices.Loaders.FileNameLoader;
using Urfu.Its.VersionedDocs.Documents.Practices.Processors;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;

namespace Urfu.Its.VersionedDocs.Documents.Practices
{
    public class PracticesWorkingProgramFgosVoSchemaModel
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
        public WorkingProgramPersonInfo Head { get; set; } = new WorkingProgramPersonInfo();

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

        /// <summary>
        /// 1.	ОБЩАЯ ХАРАКТЕРИСТИКА ПРАКТИК
        /// </summary>
        public string Annotation { get; set; }

        [Block(LoaderType = typeof(PracticeStructuresLoader))]
        public ICollection<FdpPracticeStructureInfo> PracticeStructures { get; set; } = new List<FdpPracticeStructureInfo>();

        [Block(LoaderType = typeof(FdpPracticeWaysLoader))]
        public ICollection<FdpPracticeWaysInfo> PracticeWays { get; set; } = new List<FdpPracticeWaysInfo>();

        [Block(LoaderType = typeof(PracticeResultInfosLoader))]
        public ICollection<PlannedResultPracticeInfo> PlannedResultPracticeInfos { get; set; }
        
        [Block(LoaderType = typeof(PracticeResultsLoader))]
        public ICollection<PracticeResultStructure> PracticeResults { get; set; }

        /// <summary>
        /// 2. СОДЕРЖАНИЕ ПРАКТИК
        /// </summary>
        [Block(LoaderType = typeof(PracticeSectionsLoader), ProcessorType = typeof(PracticeSectionsProcessor))]
        public ICollection<PracticeSectionsStructure> PracticeSectionsStructure { get; set; } = new List<PracticeSectionsStructure>();

        /// <summary>
        /// 3. ОЦЕНИВАНИЕ УЧЕБНОЙ ДЕЯТЕЛЬНОСТИ СТУДЕНТОВ И ЕЕ ДОСТИЖЕНИЙ В ХОДЕ ПРОХОЖДЕНИЯ ПРАКТИК
        /// </summary>
        [Block(LoaderType = typeof(PracticeEvalutionStudentPracticeLoader), ProcessorType = typeof(PracticeEvalutionStudentPracticeProcessor))]
        public ICollection<PracticeEvalutionStudentPracticeStructure> PracticeEvalutionStudentPracticeStructure { get; set; } = new List<PracticeEvalutionStudentPracticeStructure>();

        /// <summary>
        /// 4.  ФОНД ОЦЕНОЧНЫХ СРЕДСТВ ДЛЯ ПРОВЕДЕНИЯ ТЕКУЩЕЙ И ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО ПРАКТИКАМ
        /// </summary>
        [Block(LoaderType = typeof(PracticeEvalutionToolsLoader))]
        public ICollection<PracticeEvalutionToolsStructure> PracticeEvalutionToolsStructure { get; set; } = new List<PracticeEvalutionToolsStructure>();

        /// <summary>
        /// 5.  УЧЕБНО-МЕТОДИЧЕСКОЕ И ИНФОРМАЦИОННОЕ ОБЕСПЕЧЕНИЕ ПРОХОЖДЕНИЯ ПРАКТИК
        /// </summary>
        [Block(LoaderType = typeof(PracticeManualsLoader))]
        public ICollection<PracticeManualsStructure> PracticeManualsStructure { get; set; } = new List<PracticeManualsStructure>();
        
        /// <summary>
        /// 6.  МАТЕРИАЛЬНО-ТЕХНИЧЕСКОЕ ОБЕСПЕЧЕНИЕ ПРАКТИКИ
        /// </summary>
        public string PracticeTypes { get; set; }

        [Block(LoaderType = typeof(PracticeMatTechSupportsLoader))]
        public ICollection<PracticeMatTechSupportStructure> PracticeMatTechSupportStructure { get; set; } = new List<PracticeMatTechSupportStructure>();
    }
	
    public class PracticeResultStructure
    {
        public string DirectionId { get; set; }
        public string DirectionCode { get; set; }
        public ICollection<PracticeResult> Results { get; set; }
    }

    public class PracticeResult
    {
        public string DisciplineUid { get; set; }
        public string Title { get; set; }
        public string AdditionalType { get; set; }
        
        public string MustDo { get; set; }

        public string MustShow { get; set; }
    }

    public class PracticeSectionsStructure
    {
        public string DirectionId { get; set; }
        public string DirectionCode { get; set; }
        public ICollection<PracticeSection> Sections { get; set; } = new List<PracticeSection>();
    }

    public class PracticeSection
    {
        public string DisciplineUid { get; set; }
        public string Title { get; set; }
        public string AdditionalType { get; set; }
        
        public ICollection<PracticeSectionInfo> SectionInfo { get; set; } = new List<PracticeSectionInfo>();
    }

    public class PracticeSectionInfo
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class PracticeEvalutionStudentPracticeStructure
    {
        public string DirectionId { get; set; }
        public string DirectionCode { get; set; }

        public ICollection<PracticeEvalutionStudentPracticeItem> Items { get; set; } = new List<PracticeEvalutionStudentPracticeItem>();
    }

    public class PracticeEvalutionStudentPracticeItem
    {
        public string DisciplineUid { get; set; }
        public string Title { get; set; }
        public string AdditionalType { get; set; }
        public string DisciplineName { get; set; }
        public TechCardPracticeCertificationInfo TechCardDisciplineCertification { get; set; } = new TechCardPracticeCertificationInfo();

        public ICollection<TechCardSemesterSignificanceCoefficient> TechCardSemesterSignificanceCoefficients { get; set; } = new List<TechCardSemesterSignificanceCoefficient>();

        public bool IsEmpty
        {
            get
            {
                return TechCardDisciplineCertification.EduLoad.Controls.Count == 0
                && TechCardDisciplineCertification.EduLoad.CurrentCoefficient == TechCardCertificationItemInfo.CoefficientNotProvidedText
                && TechCardDisciplineCertification.EduLoad.IntermediateCertification == TechCardCertificationItemInfo.IntermediateCertificationNotProvidedText
                && TechCardDisciplineCertification.EduLoad.IntermediateCoefficient == TechCardCertificationItemInfo.CoefficientNotProvidedText
                && TechCardSemesterSignificanceCoefficients.Count == 0;
            }
        }
    }

    public class PlannedResultPracticeInfo
    {
        public string ProfileId { get; set; }
        public string ProfileCode { get; set; }
        public string DisciplineId { get; set; }
        public string DisciplineName { get; set; }
        public ICollection<EduResultCompetencesInfo> Results { get; set; }
    }

    public class PracticeEduResultItemInfo
    {
        public string DisciplineId { get; set; }
        public string DisciplineName { get; set; }
        public ICollection<EduResultCompetencesInfo> Results { get; set; }
    }

    public class PracticeEvalutionToolsStructure
    {
        public string DirectionId { get; set; }
        public string DirectionCode { get; set; }
        public ICollection<PracticeEvalutionToolsItem> Items { get; set; }
    }

    public class PracticeEvalutionToolsItem
    {
        public string DisciplineUid { get; set; }
        public string Title { get; set; }
        public string AdditionalType { get; set; }
        public string EvalutionTools { get; set; }
    }


    public class PracticeManualsStructure
    {
        public string DisciplineUid { get; set; }
        public string Title { get; set; }
        public string AdditionalType { get; set; }
        
        /// <summary>
        /// 5.1 Другая литература
        /// </summary>
        public LiteratureInfo Literature { get; set; }
        
        /// <summary>
        /// 5.2.Методические разработки
        /// </summary>
        public string MethodicalSupport { get; set; }

        ///// <summary>
        ///// 5.3.Программное обеспечение
        ///// </summary>
        //public SoftwareInfo Software { get; set; } = new SoftwareInfo();

        public bool SoftwareNotUsed { get; set; }

        public ICollection<SoftwareItemInfo> SoftwareSystemOrOffice { get; set; } = new List<SoftwareItemInfo>(); 

        /// <summary>
        /// 5.4. Базы данных, информационно-справочные и поисковые системы
        /// </summary>
        public string Databases { get; set; }

        /// <summary>
        /// 5.5.Электронные образовательные ресурсы
        /// </summary>
        public string ElectronicEducationalResources { get; set; }
    }

    public class PracticeMatTechSupportStructure
    {
        public string DisciplineUid { get; set; }
        public string Title { get; set; }
        public string MatTechSupport { get; set; }

        public string AdditionalType { get; set; }
        
    }

    public class FdpPracticeStructureInfo
    {
        public string FdpId { get; set; }

        public ICollection<FdpPracticeStructureItemInfo> Items { get; set; } = new List<FdpPracticeStructureItemInfo>();
    }

    public class FdpPracticeStructureItemInfo
    {
        public string DisciplineId { get; set; }
        public string CatalogDisciplineId { get; set; }
        public string Title { get; set; }
        public string DisciplineName { get; set; }
        public string AdditionalType { get; set; }
        public int? AdditionalWeeks { get; set; }
        public decimal? TotalTime { get; set; }
        public decimal? TotalUnits { get; set; }
        public string Semesters { get; set; }
    }
    public class FdpPracticeWaysInfo
    {
        public string DisciplineId { get; set; }
        public string DisciplineTitle { get; set; }
        public string AdditionalType { get; set; }
        public string PracticeWay { get; set; }
        public string PracticeMethod { get; set; }
    }

    public class TechCardPracticeCertificationInfo
    {
        public string Year { get; set; }
        public string Semester { get; set; }
        public string DisciplineName { get; set; }
        public string GroupId { get; set; }

        public TechCardCertificationItemInfo EduLoad { get; set; } = new TechCardCertificationItemInfo();
    }
}