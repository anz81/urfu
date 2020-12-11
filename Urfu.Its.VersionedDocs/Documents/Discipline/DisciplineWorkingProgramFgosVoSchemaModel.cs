using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline.Loaders;
using Urfu.Its.VersionedDocs.Documents.Discipline.Processors;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;
using Urfu.Its.VersionedDocs.Documents.Shared.Loaders;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;

namespace Urfu.Its.VersionedDocs.Documents.Discipline
{
    public class DisciplineWorkingProgramFgosVoSchemaModel
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
        /// Направления. Блок рашрашен. Если изменения вносятся здесь, то также их нужно внести в <see cref="ModuleWorkingProgramFgosVoSchemaModel"/>
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
        
        [Block(LoaderType = typeof(EducationalMethodicalCouncilLoader))]
        public EducationalMethodicalCouncilInfo Council { get; set; } = new EducationalMethodicalCouncilInfo();

        /// <summary>
        /// Дирекция образовательных программ
        /// </summary>
        [Block(LoaderType = typeof(WorkingProgramPersonLoader))]
        public WorkingProgramPersonInfo Direction { get; set; } = new WorkingProgramPersonInfo();

        [Block(LoaderType = typeof(RequisitesOrdersFgosLoader))]
        public ICollection<RequisitesOrderFgosInfo> RequisitesOrders { get; set; } = new List<RequisitesOrderFgosInfo>();

        #endregion

        [Block(LoaderType = typeof(DisciplineNameLoader))]
        public string Name { get; set; }

        public string Annotation { get; set; }

        public string Language { get; set; }

        [Block(LoaderType = typeof(PlannedLearningOutcomesLoader))]
        public PlannedLearningOutcomesInfo PlannedLearningOutcomes { get; set; } = new PlannedLearningOutcomesInfo();

        // TODO блок расшарен. Сохранять его отсюда нельзя, нужен атрибут и проверка
        //[Block(LoaderType = typeof(FdpsLoader))]
        [DependentBlock(nameof(DisciplineScopes))]
        [DependentBlock(nameof(Labs))]
        [DependentBlock(nameof(Practices))]
        public ICollection<FamilirizationTypeDirectionPlanInfo> Fdps { get; set; } = new List<FamilirizationTypeDirectionPlanInfo>();

        [Block(ProcessorType = typeof(SectionsProcessor))]
        [DependentBlock(nameof(LearningMethods))]
        public ICollection<DisciplineSectionInfo> Sections { get; set; } = new List<DisciplineSectionInfo>();

        /// <summary>
        /// Таблицы 1.4
        /// </summary>
        [Block(LoaderType = typeof(DisciplineScopesLoader))]
        public ICollection<DisciplineScopeInfo> DisciplineScopes { get; set; } = new List<DisciplineScopeInfo>();

        [Block(LoaderType = typeof(TimeDistibutionsLoader), ProcessorType = typeof(TimeDistributuionsProcessor))]
        public ICollection<FdpTimeDistributionInfo> TimeDistributions { get; set; } = new List<FdpTimeDistributionInfo>();

        /// <summary>
        /// 4.1. Лабораторные работы (по направлениям и формам)
        /// </summary>
        [Block(LoaderType = typeof(FdpsLessonsLoader))]
        public ICollection<FdpLessonsInfo> Labs { get; set; } = new List<FdpLessonsInfo>();

        /// <summary>
        /// 4.2. Практические занятия (по направлениям и формам)
        /// </summary>
        [Block(LoaderType = typeof(FdpsLessonsLoader))]
        public ICollection<FdpLessonsInfo> Practices { get; set; } = new List<FdpLessonsInfo>();

        /// <summary>
        /// 4.2. Практические занятия (по направлениям и формам)
        /// </summary>
        [Block(LoaderType = typeof(FdpsSelfWorkThemesLoader))]
        public ICollection<FdpSelfWorkThemesInfo> SelfWorkThemes { get; set; } = new List<FdpSelfWorkThemesInfo>();

        /// <summary>
        /// 5. СООТНОШЕНИЕ РАЗДЕЛОВ, ТЕМ ДИСЦИПЛИНЫ И ПРИМЕНЯЕМЫХ ТЕХНОЛОГИЙ ОБУЧЕНИЯ
        /// </summary>
        [Block(ProcessorType = typeof(LearningMethodsProcessor))]
        public ICollection<LearningMethodsInfo> LearningMethods { get; set; } = new List<LearningMethodsInfo>();

        /// <summary>
        /// Расшаренный блок. Сохранять его отсюда не нужно.
        /// </summary>
        public ControlEventsEstimationCriteriasInfo ControlEventsEstimationCriterias { get; set; } = new ControlEventsEstimationCriteriasInfo();

        [Block(ProcessorType = typeof(TechCardDisciplineCertificationProcessor), LoaderType = typeof(TechCardDisciplineCertificationLoader))]
        public TechCardDisciplineCertificationInfo TechCardDisciplineCertification { get; set; } = new TechCardDisciplineCertificationInfo();

        [Block(ProcessorType = typeof(TechCardCourseWorksCertificationProcessor), LoaderType = typeof(TechCardCourseWorkCertificationLoader))]
        public TechCardCertificationItemInfo TechCardCourseWorksCertification { get; set; } = new TechCardCertificationItemInfo();

        public ICollection<TechCardSemesterSignificanceCoefficient> TechCardSemesterSignificanceCoefficients { get; set; } = new List<TechCardSemesterSignificanceCoefficient>();

        /// <summary>
        /// 7. ПРОЦЕДУРЫ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ОБУЧЕНИЯ В РАМКАХ НЕЗАВИ-СИМОГО ТЕСТОВОГО КОНТРОЛЯ
        /// Блок ФЭПО
        /// </summary>
        public SmudsTestsInfo SmudsTests { get; set; } = new SmudsTestsInfo();

        /// <summary>
        /// 7. ПРОЦЕДУРЫ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ОБУЧЕНИЯ В РАМКАХ НЕЗАВИ-СИМОГО ТЕСТОВОГО КОНТРОЛЯ
        /// Блок ФЭПО
        /// </summary>
        public FepoTestsInfo FepoTests { get; set; } = new FepoTestsInfo();

        /// <summary>
        /// 7. ПРОЦЕДУРЫ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ОБУЧЕНИЯ В РАМКАХ НЕЗАВИ-СИМОГО ТЕСТОВОГО КОНТРОЛЯ
        /// Блок Интернет-тренажеров
        /// </summary>
        public InternetTrainerTestsInfo InternetTrainerTests { get; set; } = new InternetTrainerTestsInfo();

        /// <summary>
        /// Комментарий в конце 7го раздела
        /// </summary>
        public TestsCommentsInfo TestsComments { get; set; }

        /// <summary>
        /// 8.3.1. Примерные  задания для проведения мини-контрольных в рамках учебных занятий
        /// </summary>
        public ICollection<EvaluationToolInfo> MiniControlWorkThemes { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 8.3.2. Примерные  контрольные задачи в рамках учебных занятий
        /// </summary>
        public ICollection<EvaluationToolInfo> ControlWorkThemes { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 8.3.3. Примерные  контрольные кейсы
        /// </summary>
        public ICollection<EvaluationToolInfo> ControlKeys { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 8.3.4. Перечень примерных  вопросов для зачета
        /// </summary>
        public ICollection<EvaluationToolInfo> TestQuestions { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 8.3.5. Перечень примерных  вопросов для экзамена
        /// </summary>
        public ICollection<EvaluationToolInfo> ExamQuestions { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 8.3.6. Ресурсы АПИМ УрФУ, СКУД УрФУ для проведения тестового контроля в рамках текущей и промежуточной аттестации
        /// </summary>
        public ICollection<EvaluationToolInfo> UrfuResources { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 8.3.7. Ресурсы ФЭПО для проведения независимого тестового контроля
        /// </summary>
        public ICollection<EvaluationToolInfo> FepoResources { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 8.3.8. Интернет-тренажеры
        /// </summary>
        public ICollection<EvaluationToolInfo> InternetTrainers { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 8.3.9…..указать иные наименования оценочных средств, не представленных в списке
        /// </summary>
        public ICollection<EvaluationToolInfo> OtherEvaluationTools { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 9.1 Другая литература
        /// </summary>
        public LiteratureInfo Literature { get; set; }

        /// <summary>
        /// 9.2.Методические разработки
        /// </summary>
        public string MethodicalSupport { get; set; }

        /// <summary>
        /// 9.3.Программное обеспечение
        /// </summary>
        public SoftwareInfo Software { get; set; } = new SoftwareInfo();

        /// <summary>
        /// 9.4. Базы данных, информационно-справочные и поисковые системы
        /// </summary>
        public string Databases { get; set; }

        /// <summary>
        /// 9.5.Электронные образовательные ресурсы
        /// </summary>
        public string ElectronicEducationalResources { get; set; }

        /// <summary>
        /// 10. МАТЕРИАЛЬНО-ТЕХНИЧЕСКОЕ  ОБЕСПЕЧЕНИЕ ДИСЦИПЛИНЫ
        /// </summary>
        public string TechnicalSupport { get; set; }
    }

    public class TestsCommentsInfo
    {
        public bool UseStandardText { get; set; }
        public string CustomText { get; set; }
    }

    public class LiteratureInfo
    {
        public ICollection<LiteratureItemInfo> MainLiterature { get; set; } = new List<LiteratureItemInfo>();
        public ICollection<LiteratureItemInfo> AdditionalLiterature { get; set; } = new List<LiteratureItemInfo>();
        public string OtherLiterature { get; set; }
    }

    public class LiteratureItemInfo
    {
        public string recordid { get; set; }
        public string recorddata { get; set; }
        public string bookscount { get; set; }
        public string barcode { get; set; }
    }

    public class InternetTrainerTestsInfo
    {
        public ICollection<InternetTrainerTestInfo> Tests { get; set; } = new List<InternetTrainerTestInfo>();
        public ICollection<InternetTrainerKeyTaskInfo> KeysTasks { get; set; } = new List<InternetTrainerKeyTaskInfo>();

        public int? TaskCount { get; set; }
        public int? TestTime { get; set; }
    }

    public class InternetTrainerTestInfo
    {
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string ThemeCode { get; set; }
        public string Theme { get; set; }
        public int? TaskCount { get; set; }
    }

    public class InternetTrainerKeyTaskInfo
    {
        public int? TaskCount { get; set; }
    }

    public class FepoTestsInfo
    {
        /// <summary>
        /// Блок 1. Темы
        /// </summary>
        public ICollection<FepoTestInfo> Block1 { get; set; } = new List<FepoTestInfo>();

        /// <summary>
        /// Блок 2. Модули
        /// </summary>
        public ICollection<FepoTestInfo> Block2 { get; set; } = new List<FepoTestInfo>();

        /// <summary>
        /// Блок 3. Кейс-задания
        /// </summary>
        public ICollection<FepoKeyTaskInfo> Block3 { get; set; } = new List<FepoKeyTaskInfo>();

        public int? TestTime { get; set; }
        public int? TaskCount { get; set; }
    }

    public class FepoTestInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? TaskCount { get; set; }
        public decimal? PointCount { get; set; }
    }

    public class FepoKeyTaskInfo
    {
        public int? TaskCount { get; set; }
        public decimal? PointCount { get; set; }
    }

    public class SmudsTestsInfo
    {
        public ICollection<SmudsTestInfo> Tests { get; set; } = new List<SmudsTestInfo>();
        public string SpecificationNumber { get; set; }
        public int? TestTime { get; set; }
        public int? TaskCount { get; set; }
    }

    public class SmudsTestInfo
    {
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string ThemeCode { get; set; }
        public string Theme { get; set; }
        public string VariationIndex { get; set; }
        public string VariationName { get; set; }
        public int? TaskCount { get; set; }
    }

    public class FdpTimeDistributionInfo
    {
        public string FdpId { get; set; }

        public ICollection<TimeDistributionSectionInfo> Sections { get; set; } = new List<TimeDistributionSectionInfo>();

        public decimal? TotalTime { get; set; }
        public decimal? TotalAuditoryTime { get; set; }
        public decimal? TotalHomeworkTime { get; set; }
        public decimal? TestTime { get; set; }
        public decimal? ExamTime { get; set; }
        public decimal? IntegratedExamTime { get; set; }
        public decimal? ModuleProjectTime { get; set; }
        public decimal? ModuleUnits { get; set; }
        public decimal? DisciplineUnits { get; set; }
    }

    public class TimeDistributionSectionInfo
    {
        public string SectionId { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public decimal? TotalTime { get; set; }
        public decimal? TotalAuditoryTime { get; set; }
        public decimal? LectionsTime { get; set; }
        public decimal? PracticesTime { get; set; }
        public decimal? LabsTime { get; set; }
        public decimal? TotalHomeworkTime { get; set; }
        public decimal? TotalPreparationTime { get; set; }
        public decimal? PreparationLectionsTime { get; set; }
        public decimal? PreparationPracticesTime { get; set; }
        public decimal? PreparationLabsTime { get; set; }
        public decimal? PreparationSeminarsTime { get; set; }
        public decimal? TotalOutOfDoreTime { get; set; }
        public decimal? HomeworkCount { get; set; }
        public decimal? GraphicsWorkCount { get; set; }
        public decimal? ReferatsCount { get; set; }
        public decimal? ProjectWorkCount { get; set; }
        public decimal? CalcWorkCount { get; set; }
        public decimal? CalcGraphicsWorkCount { get; set; }
        public decimal? ForeignLanguageWorkCount { get; set; }
        public decimal? TranslationWorkCount { get; set; }
        public decimal? CourseWorkCount { get; set; }
        public decimal? CourseProjectCount { get; set; }
        public decimal? HomeworkTime { get; set; }
        public decimal? GraphicsWorkTime { get; set; }
        public decimal? ReferatsTime { get; set; }
        public decimal? ProjectWorkTime { get; set; }
        public decimal? CalcWorkTime { get; set; }
        public decimal? CalcGraphicsWorkTime { get; set; }
        public decimal? ForeignLanguageWorkTime { get; set; }
        public decimal? TranslationWorkTime { get; set; }
        public decimal? CourseWorkTime { get; set; }
        public decimal? CourseProjectTime { get; set; }
        public decimal? TotalControlWorkTime { get; set; }
        public decimal? ControlWorkCount { get; set; }
        public decimal? ColloquiumCount { get; set; }
        public decimal? ControlWorkTime { get; set; }
        public decimal? ColloquiumTime { get; set; }
    }

    public class FdpSelfWorkThemesInfo
    {
        public string FdpId { get; set; }
        public string HomeworkThemes { get; set; }
        public string GraphicsWorkThemes { get; set; }
        public string ReferatThemes { get; set; }
        public string ProjectThemes { get; set; }
        public string CalcWorkThemes { get; set; }
        public string CalcGraphicsWorkThemes { get; set; }
        public string CourseThemes { get; set; }
        public string ControlWorkThemes { get; set; }
        public string ColloquiumThemes { get; set; }
    }

    public class LessonInfo
    {
        public string SectionId { get; set; }
        public string SectionCode { get; set; }
        public string Name { get; set; }
        public decimal Duration { get; set; }
    }

    public class FdpLessonsInfo
    {
        public string FdpId { get; set; }
        public ICollection<LessonInfo> Lessons { get; set; } = new List<LessonInfo>();
    }

    public class LearningMethodsInfo
    {
        public string SectionId { get; set; }
        public bool ProjectWork { get; set; }
        public bool KeysAnalysis { get; set; }
        public bool BusinessGames { get; set; }
        public bool ProblemTraining { get; set; }
        public bool CommandWork { get; set; }
        public string OtherActiveMethods { get; set; }
        public bool NetworkCourses { get; set; }
        public bool VirtualPractices { get; set; }
        public bool Webinars { get; set; }
        public bool AsyncWebConferences { get; set; }
        public bool Collaboration { get; set; }
        public string OtherDistanceMethods { get; set; }
    }

    public class ControlEventsEstimationCriteriasInfo
    {
        public string HighKnowledgeLevel { get; set; }
        public string ElevatedKnowledgeLevel { get; set; }
        public string ThresholdKnowledgeLevel { get; set; }
        public string HighSkillsLevel { get; set; }
        public string ElevatedSkillsLevel { get; set; }
        public string ThresholdSkillsLevel { get; set; }
        public string HighPersonalQualitiesLevel { get; set; }
        public string ElevatedPersonalQualitiesLevel { get; set; }
        public string ThresholdPersonalQualitiesLevel { get; set; }
    }

    public class DisciplineScopeInfo
    {
        public string FdpId { get; set; }
        public int StartSemester { get; set; }
        public PlanAdditionalItemInfo Auditory { get; set; } = new PlanAdditionalItemInfo();
        public PlanAdditionalItemInfo Lections { get; set; } = new PlanAdditionalItemInfo();
        public PlanAdditionalItemInfo Practices { get; set; } = new PlanAdditionalItemInfo();
        public PlanAdditionalItemInfo Labs { get; set; } = new PlanAdditionalItemInfo();
        public PlanAdditionalItemInfo SelfWork { get; set; } = new PlanAdditionalItemInfo();
        public PlanAdditionalIntermediateItemInfo Intermediate { get; set; } = new PlanAdditionalIntermediateItemInfo();
        public PlanAdditionalItemInfo TotalAmountTime { get; set; } = new PlanAdditionalItemInfo();
        public PlanAdditionalItemInfo TotalAmountUnits { get; set; } = new PlanAdditionalItemInfo();
    }

    public class PlanAdditionalItemInfo
    {
        public decimal? DisciplineTime { get; set; }
        public decimal? DisciplineContactTime { get; set; }
        [JsonConverter(typeof(SemestersJsonConverter))]
        [JsonDefaultContentAttribute("[null, null, null, null, null, null, null, null]")]
        public ICollection<decimal?> Semesters { get; set; } = new List<decimal?> { null, null, null, null, null, null, null, null };
    }

    public class PlanAdditionalIntermediateItemInfo
    {
        public string DisciplineTime { get; set; }
        public decimal? DisciplineContactTime { get; set; }
        [JsonConverter(typeof(SemestersJsonConverter))]
        [JsonDefaultContentAttribute("[null, null, null, null, null, null, null, null]")]
        public ICollection<string> Semesters { get; set; } = new List<string> { null, null, null, null, null, null, null, null };
    }

    public class DisciplineSectionInfo
    {
        public string ItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class PlannedLearningOutcomesInfo
    {
        public ICollection<CompetencesDescriptionInfo> Items { get; set; } = new List<CompetencesDescriptionInfo>();

        public string MustKnow { get; set; }
        public string MustOwn { get; set; }
        public string MustBeAbleTo { get; set; }
    }

    public class CompetencesDescriptionInfo
    {
        public ICollection<CompetenceInfo> Competences { get; set; } = new List<CompetenceInfo>();
        public string Description { get; set; }
    }

    public class TechCardSemesterSignificanceCoefficient
    {
        public int? SemesterNumber { get; set; }
        public decimal? Coefficient { get; set; }
    }

    public class TechCardCertificationItemInfo
    {
        public string TotalCoefficient { get; set; }
        public string CurrentCoefficient { get; set; }
        public string IntermediateCoefficient { get; set; }
        public string IntermediateCertification { get; set; }
        public ICollection<TechCardControlItemInfo> Controls { get; set; } = new List<TechCardControlItemInfo>();

        public const string IntermediateCertificationNotProvidedText = "нет";

        public const string CoefficientNotProvidedText = "не предусмотрено";

        public void InitDefaults()
        {
            IntermediateCertification = IntermediateCertification ?? IntermediateCertificationNotProvidedText;
            IntermediateCoefficient = IntermediateCoefficient ?? CoefficientNotProvidedText;
            CurrentCoefficient = CurrentCoefficient ?? CoefficientNotProvidedText;
            TotalCoefficient = TotalCoefficient ?? CoefficientNotProvidedText;
        }
    }

    public class TechCardControlItemInfo
    {
        public string Name { get; set; }
        /// <summary>
        /// Семестр
        /// </summary>
        public int? Semester { get; set; }
        /// <summary>
        /// Неделя
        /// </summary>
        public string Week { get; set; }
        public decimal? MaxPoints { get; set; }
    }

    public class TechCardDisciplineCertificationInfo
    {
        public string Year { get; set; }
        public string Semester { get; set; }
        public string DisciplineName { get; set; }
        public string GroupId { get; set; }

        public TechCardCertificationItemInfo Lections { get; set; } = new TechCardCertificationItemInfo();
        public TechCardCertificationItemInfo Practices { get; set; } = new TechCardCertificationItemInfo();
        public TechCardCertificationItemInfo Labs { get; set; } = new TechCardCertificationItemInfo();
    }


}