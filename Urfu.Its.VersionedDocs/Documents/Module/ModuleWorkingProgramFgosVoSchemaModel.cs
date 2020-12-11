using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.Module.Loaders;
using Urfu.Its.VersionedDocs.Documents.Module.Processors;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;
using Urfu.Its.VersionedDocs.Documents.Shared.Loaders;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;

namespace Urfu.Its.VersionedDocs.Documents.Module
{
    public class ModuleWorkingProgramFgosVoSchemaModel
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
        /// Направления. Блок рашрашен. Если изменения вносятся здесь, то также их нужно внести в <see cref="DisciplineWorkingProgramFgosVoSchemaModel"/>
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
        public ICollection<RequisitesOrderFgosInfo> RequisitesOrders { get; set; } = new List<RequisitesOrderFgosInfo>();

        #endregion

        /// <summary>
        /// Параметры таблиц. Блок расшарен с РПД
        /// </summary>
        [Block(ProcessorType = typeof(FdpsProcessor))]
        [DependentBlock(nameof(DisciplineCompetences))]
        public ICollection<FamilirizationTypeDirectionPlanInfo> Fdps { get; set; } = new List<FamilirizationTypeDirectionPlanInfo>();

        public string Annotation { get; set; }

        /// <summary>
        /// 2.	СТРУКТУРА МОДУЛЯ И РАСПРЕДЕЛЕНИЕ УЧЕБНОГО ВРЕМЕНИ ПО ДИСЦИПЛИНАМ
        /// </summary>
        [Block(LoaderType = typeof(ModuleStructuresLoader))]
        [DependentBlock(nameof(DisciplineCompetences))]
        public ICollection<FdpModuleStructureInfo> ModuleStructures { get; set; } = new List<FdpModuleStructureInfo>();

        [Block(LoaderType = typeof(DisciplineSequenceLoader))]
        public DisciplineSequenceInfo DisciplineSequence { get; set; } = new DisciplineSequenceInfo();

        /// <summary>
        /// 4.1.	Планируемые результаты освоения модуля и составляющие их компетенции
        /// </summary>
        [DependentBlock(nameof(DisciplineCompetences))]
        public ICollection<PlannedResultItemInfo> PlannedResults { get; set; } = new List<PlannedResultItemInfo>();

        /// <summary>
        /// 4.2.Распределение формирования компетенций по дисциплинам модуля
        /// </summary>
        [Block(LoaderType = typeof(DisciplineCompetencesLoader), ProcessorType = typeof(DisciplineCompetencesProcessor))]
        public ICollection<FdpDisciplineCompetencesInfo> DisciplineCompetences { get; set; } = new List<FdpDisciplineCompetencesInfo>();

        /// <summary>
        /// 5.2. Форма промежуточной аттестации по модулю:
        /// </summary>
        [Block(LoaderType = typeof(ModuleIntermediateCertificationFormLoader))]
        public ICollection<FdpModuleIntermediateCertificationFormInfo> ModuleIntermediateCertificationForms { get; set; }

        /// <summary>
        /// Таблица из приложения 1. 5.3. Фонд оценочных средств...
        /// </summary>
        public ControlEventsEstimationCriteriasInfo ControlEventsEstimationCriterias { get; set; } = new ControlEventsEstimationCriteriasInfo();

        /// <summary>
        /// 5.3.2.1. Перечень примерных  вопросов для интегрированного экзамена по модулю список.
        /// </summary>
        public ICollection<EvaluationToolInfo> IntegratedExamQuestions { get; set; } = new List<EvaluationToolInfo>();

        /// <summary>
        /// 5.3.2.2. Перечень примерных  тем итоговых проектов по модулю список.
        /// </summary>
        public string ModuleProjectThemes { get; set; }

        /// <summary>
        /// 6. ЛИСТ РЕГИСТРАЦИИ ИЗМЕНЕНИЙ В РАБОЧЕЙ ПРОГРАММЕ МОДУЛЯ
        /// </summary>
        public ICollection<ChangesListItem> ChangesList { get; set; } = new List<ChangesListItem>();
    }

    public class FdpDisciplineCompetencesInfo
    {
        public string FdpId { get; set; }
        public ICollection<DisciplineCompetencesInfo> Items { get; set; } = new List<DisciplineCompetencesInfo>();
    }

    public class DisciplineCompetencesInfo
    {
        public string DisciplineId { get; set; }
        public string DisciplineName { get; set; }
        public string DisciplineDisplayName { get; set; }
        public ICollection<int> CompetenceIds { get; set; } = new List<int>();
        public ICollection<CompetenceInfo> OkCompetences { get; set; } = new List<CompetenceInfo>();
        public ICollection<CompetenceInfo> OpkCompetences { get; set; } = new List<CompetenceInfo>();
        public ICollection<CompetenceInfo> PkCompetences { get; set; } = new List<CompetenceInfo>();        
    }

    public class EduResultCompetencesInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CodeNumber { get; set; }

        public string Description { get; set; }

        public string ProfileId { get; set; }

        public string DisplayDescription { get { return Name + ", " + Description; } }

        public ICollection<CompetenceInfo> Competences { get; set; } = new List<CompetenceInfo>();
    }

    public class FdpModuleStructureInfo
    {
        public string FdpId { get; set; }

        public ICollection<ModuleStructureItemInfo> Items { get; set; } = new List<ModuleStructureItemInfo>();
    }

    public class ModuleStructureItemInfo
    {
        /// <summary>
        /// часть образовательной программы  (Б, ВВ, ВС)
        /// </summary>
        public string EducationalProgramPart { get; set; }

        public string DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public string Semesters { get; set; }

        public decimal? Lections { get; set; }
        public decimal? Practices { get; set; }
        public decimal? Labs { get; set; }
        public decimal? AuditoryTotal { get; set; }
        public decimal? SelfWork { get; set; }
        public string IntermediateCertification { get; set; }
        public decimal? TotalTime { get; set; }
        public decimal? TotalUnits { get; set; }
    }

    public class FdpModuleIntermediateCertificationFormInfo
    {
        public string FdpId { get; set; }
        public string Form { get; set; }
    }

    public class ChangesListItem
    {
        public string Number { get; set; }
        public string ProtocolNumber { get; set; }
        public string Date { get; set; }
        public string ListCount { get; set; }        
    }

    public class DisciplineSequenceInfo
    {
        public bool NoRequirements { get; set; }
        public ICollection<DisciplineSequenceItemInfo> Items { get; set; } = new List<DisciplineSequenceItemInfo>();
    }

    public class DisciplineSequenceItemInfo
    {
        public int Number { get; set; }
        public string DisciplineId { get; set; }
        public string DisciplineName { get; set; }        
    }
}