using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Urfu.Its.Web.Models;
using Toolbelt.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// ReSharper disable InconsistentNaming
namespace Urfu.Its.Web.DataContext
{

    public class EduProgram
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Название версии ОП")]
        public string Name { get; set; }

        [DisplayName("ФИО руководителя")]
        public string HeadFullName { get; set; }

        [Required]
        [DisplayName("Направление")]
        public string directionId { get; set; }

        [ForeignKey("directionId")]
        [DisplayName("Направление")]
        public virtual Direction Direction { get; set; }

        [DisplayName("Уровень обучения")]
        [Required]
        public string qualification { get; set; }

        [DisplayName("Год")]
        public int Year { get; set; }

        public virtual ICollection<Variant> Variants { get; set; }

        [DisplayName("Подразделение")]
        [Required]
        public string divisionId { get; set; }

        [ForeignKey("divisionId")]
        public virtual Division Division { get; set; }

        [DisplayName("Департамент")]
        public string departmentId { get; set; }

        [ForeignKey("departmentId")]
        public virtual Division Department { get; set; }

        [DisplayName("Кафедра")]
        [Required]
        public string chairId { get; set; }

        [ForeignKey("chairId")]
        public virtual Division Chair { get; set; }

        [DisplayName("Состояние")]
        public VariantState State { get; set; }

        [DisplayName("Версия плана")]
        public int PlanVersionNumber { get; set; }

        [DisplayName("Номер плана")]
        public int? PlanNumber { get; set; }

        [DisplayName("Профиль")]
        [Required]
        public string profileId { get; set; }

        [ForeignKey("profileId")]
        public virtual Profile Profile { get; set; }

        [DisplayName("Сетевая программа")]
        public bool IsNetwork { get; set; }

        [DisplayName("Форма освоения")]
        public string familirizationType { get; set; }

        [DisplayName("Технология освоения")]
        public string familirizationTech { get; set; }

        [DisplayName("Условие освоения")]
        public string familirizationCondition { get; set; }

        [DisplayName("Направление и программа")]
        public string DirectionAndName
        {
            get { return Direction.okso + " " + Name; }
        }

        public string FullName
        {
            get { return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", Direction != null ? (Direction.okso + " " + Direction.title) : Profile.CODE, Name, familirizationType, familirizationCondition, Division.shortTitle, Year); }
        }

        public int? VariantId { get; set; }

        [ForeignKey("VariantId")]
        public virtual Variant Variant { get; set; }

        public void CheckProgramVariant(ApplicationDbContext db)
        {
            if (VariantId == null)
            {
                var variant = new Variant
                {
                    Name = "Основной вариант",
                    CreateDate = DateTime.Now,
                    IsBase = true,
                    Program = this,
                    EduProgramId = Id
                };
                Variant = variant;
                VariantId = variant.Id;
                db.SaveChanges();
            }
        }


        public void OnChanged()
        {
            Logger.Info("Изменена ОП '{0}' Id '{1}' статус '{2}'", Name, Id, State);

            Task.Run((Action)ExportProgram);
        }

        private void ExportProgram()
        {
            var db = ApplicationDbContext.Create();
            try
            {
                var mce = new AutoMapper.Configuration.MapperConfigurationExpression();
                var config = new MapperConfiguration(mce);
                var mapper = new Mapper(config);

                (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
                var dto = mapper.Map<ProgramApiDto>(db.EduPrograms.Find(Id));
                FillTeachers(db, dto.variants);
                PersonalCabinetService.PostPrograms(new[] { dto });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        public static void FillTeachers(ApplicationDbContext db, List<VariantApiDto> list)
        {
            var variantIds = list.Select(v => v.id).ToList();
            foreach (var group in db.PlanTeachers.Include(pt => pt.Teacher).Where(pt => pt.Teacher != null).Where(pt => variantIds.Contains(pt.variantId)).GroupBy(pt => pt.variantId).ToList())
            {
                var variant = list.FirstOrDefault(v => v.id == @group.Key);
                if (variant == null)
                    continue;
                var teachers = @group.ToList();
                foreach (var module in variant.modules)
                {
                    foreach (var plan in module.plans)
                    {
                        plan.teachers =
                        teachers
                            .Where(
                                t => t.catalogDisciplineUuid == plan.catalogDisciplineUUID
                                     && t.eduplanUuid == plan.eduplanUUID
                                     && t.moduleId == module.moduleUuid)
                            .Select(pt => new
                            {
                                dto = new TeacherApiDto
                                {
                                    division = pt.Teacher.division,
                                    firstName = pt.Teacher.firstName,
                                    initials = pt.Teacher.initials,
                                    lastName = pt.Teacher.lastName,
                                    middleName = pt.Teacher.middleName,
                                    pkey = pt.Teacher.pkey,
                                    post = pt.Teacher.post,
                                    selectable = pt.Selectable,
                                    workPlace = pt.Teacher.workPlace
                                },
                                pt.load
                            })
                            .GroupBy(o => o.load)
                            .ToDictionary(o => o.Key, g => g.Select(o => o.dto).ToList());
                    }
                }
            }
        }

        [NotMapped]
        private Dictionary<string, string> _familirizationTypes = new Dictionary<string, string>(){
                {"Очная","очной"},
                {"Заочная","заочной"},
                {"Очно-заочная","очно-заочной"}};

        public string GetFamilirizationTypeGenitive()
        {
            string type = _familirizationTypes.ContainsKey(familirizationType) ? _familirizationTypes[familirizationType] : "";
            return type;
        }
    }

    [Table("Directions")]
    public class Direction
    {
        [Key]
        public string uid { get; set; }

        [DisplayName("ОКСО")]
        [Index("IX_Direction_okso")]
        [MaxLength(64)]
        public string okso { get; set; }

        [DisplayName("Название направления")]
        public string title { get; set; }

        [DisplayName("Код")]
        public string ministerialCode { get; set; }

        [DisplayName("УГН")]
        public string ugnTitle { get; set; }

        [DisplayName("Стандарт")]
        public string standard { get; set; }

        [DisplayName("Квалификация")]
        public string qualifications { get; set; }

        [DisplayName("Квалификация в дипломе")]
        public string diplomaQualification { get; set; }

        public int? AreaEducationId { get; set; }

        [ForeignKey(nameof(AreaEducationId))]
        public virtual AreaEducation AreaEducation { get; set; }

        [NotMapped]
        [DisplayName("ОКСО и название")]
        public string OksoAndTitle
        {
            get { return string.Format("{0} - {1}", okso, title); }
        }

        [NotMapped]
        [DisplayName("ОКСО, название, стандарт")]
        public string OksoAndTitleStandard
        {
            get { return string.Format("{0} - {1} ({2})", okso, title, standard); }
        }

        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<UserDirection> Users { get; set; }
        public virtual ICollection<EduProgram> Programs { get; set; }
        public virtual ICollection<DirectionOrder> Orders { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
        public List<ModulesInDirections> ModulesInDirections { get; set; }
    }

    [Table("Profiles")]
    public class Profile
    {
        [Key, MaxLength(127)]
        public string ID { get; set; }
        public string CODE { get; set; }
        [DisplayName("Название профиля")]
        public string NAME { get; set; }
        public string CHAIR_ID { get; set; }
        [ForeignKey(nameof(CHAIR_ID))]
        public Division Division { get; set; }

        public string QUALIFICATION { get; set; }
        public string DIRECTION_ID { get; set; }

        [ForeignKey("DIRECTION_ID")]
        public virtual Direction Direction { get; set; }

        public string FOREIGN_CONTENT { get; set; }

        public bool remove { get; set; }

        [NotMapped]
        [DisplayName("Код и название")]
        public string OksoAndTitle
        {
            get { return string.Format("{0} - {1}", CODE, NAME); }
        }

        [NotMapped]
        private Dictionary<string, string> _qualificationLevelsGenitive = new Dictionary<string, string>(){
                {"Бакалавр","бакалавриата"},
                {"Магистр","магистратуры"},
                {"Специалист","специалитета"},
                {"Аспирант","аспирантуры"},
                {"Подготовительное отделение","подготовительного отделения"},
                {"Прикладной бакалавриат","прикладного бакалавриата"}
        };

        [NotMapped]
        private Dictionary<string, string> _qualificationLevels = new Dictionary<string, string>(){
                {"Бакалавр","бакалавриат"},
                {"Магистр","магистратура"},
                {"Специалист","специалитет"},
                {"Аспирант","аспирантура"},
                {"Подготовительное отделение","подготовительное отделение"},
                {"Прикладной бакалавриат","прикладной бакалавриат"}
        };

        public string GetEducationLevelGenitive()
        {
            string qual = _qualificationLevelsGenitive.ContainsKey(QUALIFICATION) ? _qualificationLevelsGenitive[QUALIFICATION] : "";
            return qual;
        }

        public string GetEducationLevel()
        {
            string level = _qualificationLevels.ContainsKey(QUALIFICATION) ? _qualificationLevels[QUALIFICATION] : "";
            return level;
        }
    }


    public class Module
    {
        [Key]
        public string uuid { get; set; }

        [DisplayName("Название модуля")]
        public string title { get; set; }

        [DisplayName("Короткое название модуля")]
        public string shortTitle { get; set; }

        [DisplayName("Координатор")]
        public string coordinator { get; set; }

        [DisplayName("Тип")]
        public string type { get; set; }

        [DisplayName("Компетенции")]
        public string competence { get; set; }

        [DisplayName("Зачётные единицы")]
        public int testUnits { get; set; }

        [DisplayName("Приоритет")]
        public decimal priority { get; set; }

        [DisplayName("Состояние")]
        public string state { get; set; }

        [DisplayName("Дата утверждения")]
        public DateTime? approvedDate { get; set; }

        [DisplayName("Комментарий")]
        public string comment { get; set; }

        [DisplayName("Аннотация")]
        public string annotation { get; set; }

        [DisplayName("Файл")]
        public string file { get; set; }

        [DisplayName("Направления")]
        public string specialities { get; set; }

        [DisplayName("Номер")]
        public int? number { get; set; }

        /// <summary>
        /// Источники: uni, project. Для обозначения используется статический класс Urfu.Its.Web.Models.Source.
        /// </summary>
        public string Source { get; set; } = Urfu.Its.Web.Models.Source.Uni;

        /// <summary>
        /// Уровнь модуля для проектного обучения. 
        /// Возможные значения - null, А, Б, С (по-русски)
        /// </summary>
        public string Level { get; set; }

        public bool IncludeInCore { get; set; }

        public virtual IList<Discipline> disciplines { get; set; }
        
        [DisplayName("Направления")]
        public virtual ICollection<Direction> Directions { get; set; }

        public virtual ICollection<VariantContent> UsedInVariantContents { get; set; }

        public virtual ICollection<Plan> Plans { get; set; }

        public virtual ICollection<UserMinor> Users { get; set; }

        //public virtual ICollection<MUPDisciplineConnection> MUPConnections { get; set; }

        public virtual Minor Minor { get; set; }
        public virtual SectionFK SectionFk { get; set; }

        public virtual Project Project { get; set; }

        public virtual MUP MUP { get; set; }

        [DisplayName("Требуется для майноров")]
        public virtual ICollection<Minor> RequiredFor { get; set; }

        [NotMapped]
        [DisplayName("Номер и название")]
        public string numberAndTitle
        {
            get { return string.Format("{0} {1}", number, title); }
        }

        public ForeignLanguage ForeignLanguage { get; set; }

        public List<MinorDiscipline> GetMinorDisciplines()
        {
            if (Minor == null)
                return disciplines
                    .Select(d => new MinorDiscipline { Discipline = d })
                    .ToList();
            else
                return disciplines
                    .Select(d => Minor.Disciplines.FirstOrDefault(f => f.DisciplineUid == d.uid) ?? new MinorDiscipline { Discipline = d })
                    .ToList();
        }

        public bool GetShowInLC()
        {
            return Minor?.ShowInLC ?? ForeignLanguage?.ShowInLC ?? false;
        }

        public string GetTechName()
        {
            return Minor?.Tech.Name ?? ForeignLanguage?.Tech.Name ?? SectionFk?.Tech.Name;
        }
        public List<ModuleDisciplineMapping> ModuleDisciplineMapping { get; set; }
        public List<ModulesInDirections> ModulesInDirections { get; set; }
    }
    public class ModulesInDirections
    {
        public int DirectionId { get; set; }
        public Module Modules { get; set; }
        public Direction Direction { get; set; }
        public int ModuleId { get; set; }
    }


    public class Discipline
    {
        [Key]
        public string uid { get; set; }

        [Required, MaxLength(32)]
        public string pkey { get; set; } // UUID справочной дисциплины (uni)

        [DisplayName("Название дисциплины")]
        public string title { get; set; }

        [DisplayName("Тип")]
        public string section { get; set; }

        [DisplayName("Зачётные единицы")]
        public decimal testUnits { get; set; }

        [DisplayName("Файл")]
        public string file { get; set; }

        [DisplayName("Номер")]
        public int? number { get; set; }

        public virtual ICollection<Module> Modules { get; set; }

        public virtual ICollection<WorkingProgramResponsiblePerson> WorkingProgramResponsiblePersons { get; set; }
        public List<ModuleDisciplineMapping> ModuleDisciplineMapping { get; set; }
    }
    public class ModuleDisciplineMapping
    {
        public string DId { get; set; }
        public Discipline Discipline { get; set; }
        public string MId { get; set; }
        public Module Modules { get; set; }
    }
    public class Division
    {
        [Key, MaxLength(127)]
        public string uuid { get; set; }
        [DisplayName("Название подразделения")]
        public string title { get; set; }
        [DisplayName("Тип подразделения")]
        public string typeTitle { get; set; }
        [DisplayName("Подразделение")]
        public string shortTitle { get; set; }
        public string typeCode { get; set; }

        public string parent { get; set; }

        [NotMapped]
        public Division ParentDivision { get; set; }
        //возвращаем название филиала, института
        public string ParentName()
        {
            if (ParentDivision == null)
                return null;

            if (ParentDivision.typeCode == "institute" || ParentDivision.typeCode == "branch")
                return $"{ParentDivision.typeTitle} {ParentDivision.title}";

            return ParentDivision.ParentName();
        }

        public string ParentShortName()
        {
            if (ParentDivision == null)
                return null;

            if (ParentDivision.typeCode == "institute" || ParentDivision.typeCode == "branch")
                return $"({ParentDivision.shortTitle})";

            return ParentDivision.ParentShortName();
        }

        public virtual ICollection<UserDivision> Users { get; set; }
        public virtual ICollection<MinorDisciplineTmerPeriod> MinorDisciplineTmerPeriod { get; set; }
        public virtual ICollection<SectionFKDisciplineTmerPeriod> SectionFKDisciplineTmerPeriod { get; set; }
        public virtual ICollection<ForeignLanguageDisciplineTmerPeriod> ForeignLanguageDisciplineTmerPeriods { get; set; }
        public virtual ICollection<ProjectDisciplineTmerPeriod> ProjectDisciplineTmerPeriods { get; set; }
        public virtual ICollection<MUPDisciplineTmerPeriod> MUPDisciplineTmerPeriods { get; set; }
        public List<MinorDisciplineTmerPeriodDivision> MinorDisciplineTmerPeriodDivision { get; set; }
        public List<SectionFKDisciplineTmerPeriodDivision> SectionFKDisciplineTmerPeriodDivision { get; set; }
        public List<ForeignLanguageDisciplineTmerPeriodDivision> ForeignLanguageDisciplineTmerPeriodDivision { get; set; }
        public List<MUPDisciplineTmerPeriodDivision> MUPDisciplineTmerPeriodDivision { get; set; }
        public List<ProjectDisciplineTmerPeriodDivision> ProjectDisciplineTmerPeriodDivision { get; set; }
    }
    public class MinorDisciplineTmerPeriodDivision
    {
        public int DivisionId { get; set; }
        public Division Divisions { get; set; }
        public int DisciplineTmerPeriodId { get; set; }
        public MinorDisciplineTmerPeriod MinorDisciplineTmerPeriod { get; set; }
    }


    public class Plan
    {
        public string eduplanUUID { get; set; }

        [DisplayName("Номер")]
        public int? eduplanNumber { get; set; }

        public string faculty { get; set; }

        [ForeignKey("faculty")]
        public Division FacultyDivision { get; set; }

        [DisplayName("Версия")]
        public int versionNumber { get; set; }
        [DisplayName("Версия")]
        public string versionTitle { get; set; }

        public bool versionActive { get; set; }

        public string disciplineUUID { get; set; }
        public string additionalUUID { get; set; }

        [MaxLength(127)]
        public string versionUUID { get; set; }

        [DisplayName("Название дисциплины")]
        public string disciplineTitle { get; set; }

        [Index("IX_Plan_moduleUUID"), MaxLength(127)]
        [Index("IX_Plan_QueryIndex", 0)]
        public string moduleUUID { get; set; }

        [DisplayName("Ид Направления")]
        [Index("IX_Plan_QueryIndex", 1), MaxLength(127)]
        public string directionId { get; set; }

        [DisplayName("Контрольные мероприятия")]
        public string controls { get; set; }

        [DisplayName("Нагрузка")]
        public string loads { get; set; }

        [DisplayName("Семестры")]
        public string terms { get; set; }

        [DisplayName("Утверждено")]
        public bool active { get; set; }

        [DisplayName("Статус")]
        public string versionStatus { get; set; }

        [DisplayName("Зачётные единицы по семестрам")]
        public string testUnitsByTerm { get; set; }

        public string allTermsExtracted { get; set; }
        public string catalogDisciplineUUID { get; set; }

        public Module Module { get; set; }

        [Index("IX_Plan_QueryIndex", 3), MaxLength(127)]
        public string familirizationType { get; set; }

        [Index("IX_Plan_QueryIndex", 4), MaxLength(127)]
        public string familirizationTech { get; set; }

        [Index("IX_Plan_QueryIndex", 5), MaxLength(127)]
        public string familirizationCondition { get; set; }

        [Index("IX_Plan_QueryIndex", 2), MaxLength(127)]
        public string qualification { get; set; }

        [Index("IX_Plan_AdditionalType"), MaxLength(127)]
        public string additionalType { get; set; }

        /// <summary>
        /// Образовательная программа
        /// </summary>
        public string learnProgramUUID { get; set; }
        public string learnProgramTitle { get; set; }
        public string learnProgramCode { get; set; }

        public bool remove { get; set; }
        public int? additionalWeeks { get; set; }

        public string moduleGroupType { get; set; }

        public string moduleSubgroupType { get; set; }

        public IEnumerable<string> GetLoadKeys()
        {
            if (controls != null)
                foreach (var dictionary in JsonConvert.DeserializeObject<List<Dictionary<string, List<int>>>>(controls))
                {
                    foreach (var kvp in dictionary)
                    {
                        yield return kvp.Key;
                    }
                }
            if (loads != null)
                foreach (var str in JsonConvert.DeserializeObject<List<string>>(loads))
                {
                    yield return str;
                }
        }

        [NotMapped]
        public IEnumerable<PlanTeacher> Teachers { get; set; }


        private Dictionary<string, int> _testUnitsByTermObject { get; set; }

        public int GetTermTestUnits(int term)
        //TODO: AB: это плохо, что у нас расчётная логика описывается в классах модели. Её бы нужно перенести во ViewModel
        //хотя в принципе эта логика относится к декодированию данных БД
        {
            if (testUnitsByTerm == null)
                return 0;
            if (_testUnitsByTermObject == null)
                _testUnitsByTermObject = JsonConvert.DeserializeObject<Dictionary<string, int>>(testUnitsByTerm);
            int testUnits = 0;
            if (_testUnitsByTermObject != null) _testUnitsByTermObject.TryGetValue(term.ToString(), out testUnits);
            return testUnits;
        }
    }

    //информация о семестрах для данной дисциплины, разветнутое и просчитанное поле allTermsExtracted в Plans
    [Table("PlanDisciplineTerms")]
    public class PlanDisciplineTerm
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string DisciplineUUID { get; set; }

        public int Term { get; set; }

        public int Course { get; set; }

        public int SemesterID { get; set; }
    }

    //общая информация о семестрах в eduplanUUID
    [Table("PlanTerms")]
    public class PlanTerm
    {
        [Key]
        public int Id { get; set; }

        [Index("IX_PlanTerms_QueryIndex", IsUnique = false), MaxLength(128)]
        public string eduplanUUID { get; set; }

        [DisplayName("Порядковый номер года")]
        public int Year { get; set; }

        [DisplayName("Количество семестров в году")]
        public int TermsCount { get; set; }
    }

    [Table("PlanAdditionals")]
    public class PlanAdditional
    {
        public string disciplineUUID { get; set; }
        public string discipline { get; set; }
        public string versionUUID { get; set; }
        public int? allload { get; set; }
        public int? allaudit { get; set; }
        public int? self { get; set; }
        public int? lecture { get; set; }
        public int? practice { get; set; }
        public int? labs { get; set; }
        public string controls { get; set; }
        public decimal? contactTotal { get; set; }
        public decimal? contactSelf { get; set; }
        public decimal? contactControl { get; set; }
        public decimal? contactLecture { get; set; }
        public decimal? contactPractice { get; set; }
        public decimal? contactLabs { get; set; }
        public string tl1 { get; set; }
        public string tl2 { get; set; }
        public string tl3 { get; set; }
        public string tl4 { get; set; }
        public string tl5 { get; set; }
        public string tl6 { get; set; }
        public string tl7 { get; set; }
        public string tl8 { get; set; }
        public string tl9 { get; set; }
        public string tl10 { get; set; }
        public string tl11 { get; set; }
        public string tl12 { get; set; }
        public int gosLoadInTestUnits { get; set; }
        public int ttu1 { get; set; }
        public int ttu2 { get; set; }
        public int ttu3 { get; set; }
        public int ttu4 { get; set; }
        public int ttu5 { get; set; }
        public int ttu6 { get; set; }
        public int ttu7 { get; set; }
        public int ttu8 { get; set; }
        public int ttu9 { get; set; }
        public int ttu10 { get; set; }
        public int ttu11 { get; set; }
        public int ttu12 { get; set; }
    }

    [Table("Groups")]
    public class Group
    {
        [Key]
        public string Id { get; set; }

        [DisplayName("Название группы")]
        public string Name { get; set; }

        [DisplayName("Профиль")]
        public string ProfileId { get; set; }

        [DisplayName("Год")]
        public string Year { get; set; }
        public int Course { get; set; }
        [DisplayName("Кафедра")]
        public string ChairId { get; set; }
        [DisplayName("Формирующее подразделение")]
        public string FormativeDivisionId { get; set; }
        [DisplayName("Родитель формирующего подразделения")]
        public string FormativeDivisionParentId { get; set; }
        [DisplayName("Управляющее подразделение")]
        public string ManagingDivisionId { get; set; }
        [DisplayName("Родитель управляющего подразделения")]
        public string ManagingDivisionParentId { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }

        public virtual ICollection<SectionFKCompetitionGroup> SectionFkCompetitionGroups { get; set; }
        public virtual ICollection<ForeignLanguageCompetitionGroup> ForeignLanguageCompetitionGroups { get; set; }
        public virtual ICollection<ProjectCompetitionGroup> ProjectCompetitionGroups { get; set; }
        public virtual ICollection<MUPCompetitionGroup> MUPCompetitionGroups { get; set; }

        public string FamType { get; set; }

        public string FamTech { get; set; }

        public string FamCond { get; set; }

        public string FamPeriod { get; set; }

        public string Qual { get; set; }
        public List<SectionFKCompetitionGroupContents> SectionFKCompetitionGroupContents { get; set; }
        public List<MUPCompetitionGroupContents> MUPCompetitionGroupContents { get; set; }
        public List<ProjectCompetitionGroupContents> ProjectCompetitionGroupContents { get; set; }
        public List<ForeignLanguageCompetitionGroupContents> ForeignLanguageCompetitionGroupContents { get; set; }
    }

    [Table("GroupsHistory")]
    public class GroupsHistory
    {
        [Key]
        public string Id { get; set; }

        [StringLength(32)]
        public string GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }

        [DisplayName("Название группы")]
        [StringLength(255)]
        public string Name { get; set; }

        [DisplayName("Профиль")]
        [StringLength(32)]
        public string ProfileId { get; set; }

        [DisplayName("Год")]
        public int YearHistory { get; set; }
        public int Course { get; set; }
        [DisplayName("Кафедра")]
        [StringLength(32)]
        public string ChairId { get; set; }

        [DisplayName("Управляющее подразделение")]
        [StringLength(32)]
        public string ManagingDivisionId { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }
        [StringLength(32)]

        public string FamType { get; set; }
        [StringLength(32)]

        public string FamTech { get; set; }

        [StringLength(32)]
        public string Qual { get; set; }
    }

    public enum StudentRatingType
    {
        /// <summary>
        /// Егэ
        /// </summary>
        [Display(Name = "ЕГЭ")]
        School = 0,
        /// <summary>
        /// Обычный
        /// </summary>
        [Display(Name = "БРС")]
        Regular = 1,
        [Display(Name = "Средний балл из uni")]
        UniAvg = 2
    }

    [Table("Students")]
    public class Student
    {
        [Key]
        public string Id { get; set; }

        [DisplayName("Персона")]
        [Index("IX_Student_PersonId"), MaxLength(127)]
        public string PersonId { get; set; }

        [DisplayName("Статус")]
        public string Status { get; set; }

        [DisplayName("Личный номер студента")]
        public string PersonalNumber { get; set; }

        [DisplayName("Группа")]
        [Index("IX_Student_GroupId"), MaxLength(127)]
        public string GroupId { get; set; }

        [DisplayName("Телефон домашний")]
        public string PhoneHome { get; set; }

        [DisplayName("Телефон мобильный")]
        public string PhoneMobile { get; set; }

        public bool Male { get; set; }

        [DisplayName("Телефон рабочий")]
        public string PhoneWork { get; set; }

        public string Email { get; set; }
        public string Icq { get; set; }

        [DisplayName("Рейтинг")]
        public decimal? Rating { get; set; }

        [DisplayName("Балл за тест по ИЯ")]
        public decimal? ForeignLanguageRating { get; set; }
        [DisplayName("Уровень ИЯ")]
        public string ForeignLanguageLevel { get; set; }
        [DisplayName("Желаемый уровень ИЯ")]
        public string ForeignLanguageTargetLevel { get; set; }

        public int? planVerion { get; set; }
        public int? versionNumber { get; set; }

        [DisplayName("Семестры задолженности по секциям ФК")]
        public string sectionFKDebtTerms { get; set; }

        [DisplayName("Выбор студента"), MaxLength(128 * 1024)]
        public string SelectionJson { get; set; }

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public virtual ICollection<StudentVariantSelection> Selections { get; set; }

        public virtual ICollection<StudentSelectionMinorPriority> MinorSelections { get; set; }
        public virtual ICollection<SectionFKStudentSelectionPriority> SectionFKSelections { get; set; }
        public virtual ICollection<ForeignLanguageStudentSelectionPriority> ForeignLanguageSelections { get; set; }
        public virtual ICollection<ProjectStudentSelectionPriority> ProjectSelections { get; set; }

        [DisplayName("Тип рейтинга")]
        public StudentRatingType? RatingType { get; set; }

        [DisplayName("Целевой")]
        public bool IsTarget { get; set; }
        [DisplayName("Иностранный студент")]
        public bool IsInternational { get; set; }
        [DisplayName("Вид возмещения затрат")]
        public string Compensation { get; set; }

        public virtual ICollection<ModuleAdmission> ModuleAdmissions { get; set; }
        public virtual ICollection<VariantAdmission> VariantAdmissions { get; set; }
        public virtual ICollection<MinorAdmission> MinorAdmissions { get; set; }
        public virtual ICollection<SectionFKAdmission> SectionFKAdmissions { get; set; }
        public virtual ICollection<ForeignLanguageAdmission> ForeignLanguageAdmissions { get; set; }
        public virtual ICollection<ProjectAdmission> ProjectAdmissions { get; set; }
        public virtual ICollection<MUPAdmission> MUPAdmissions { get; set; }
        public virtual ICollection<Practice> Practices { get; set; }

        public int? GetMinroPeriodPriority(int minorPeriodId)
        {
            return MinorSelections?.FirstOrDefault(s => s.minorPeriodId == minorPeriodId)?.priority;
        }

        internal AdmissionStatus GetMinorAddmissionStatus(int minorPeriodId)
        {
            return MinorAdmissions?.FirstOrDefault(a => a.minorPeriodId == minorPeriodId)?.Status ?? AdmissionStatus.Indeterminate;
        }

        public bool Sportsman { get; set; }

        public string Citizenship { get; set; }
    }

    public static class StudentsExtension
    {
        public static IQueryable<Student> OnlyActive(this IQueryable<Student> src)
        {
            return src.Where(ActivityPredicate);
        }
        public static IQueryable<Student> ActiveOrGraduated(this IQueryable<Student> src)
        {
            return src.Where(ActivityOrGraduatedPredicate);
        }

        const string active = "Активный";
        const string otp1 = "Отп.с.посещ.";
        const string otp2 = "Отп.дород.послерод.";
        const string graduated1 = "Зак. с дипломом";
        const string graduated2 = "Окончил обучение";

        public static string[] ActiveStatuses = { active, otp1, otp2 };
        public static string[] GraduatedStatuses = { graduated1, graduated2 };

        public static Expression<Func<Student, bool>> ActivityPredicate
        {
            get
            {
                return s => s.Status == active || s.Status == otp1 || s.Status == otp2;
            }
        }

        public static Expression<Func<Student, bool>> ActivityOrGraduatedPredicate
        {
            get
            {
                return s => s.Status == active || s.Status == otp1 || s.Status == otp2 || s.Status == graduated1 || s.Status == graduated2;
            }
        }
    }

    [Table("Persons")] // to avoid pluralization to People
    public class Person
    {
        [Key]
        public string Id { get; set; }

        [DisplayName("Фамилия")]
        public string Surname { get; set; }

        [DisplayName("Имя")]
        public string Name { get; set; }

        [DisplayName("Отчество")]
        public string PatronymicName { get; set; }

        [DisplayName("Телефон")]
        public string Phone { get; set; }

        public string EMail { get; set; }

        [DisplayName("Дата рождения")]
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        public string FullName()
        {
            return $"{Surname} {Name} {PatronymicName}";
        }

        public string ShortName()
        {
            var n = !string.IsNullOrEmpty(Name)
                ? Name[0] + "."
                : "";
            var p = !string.IsNullOrEmpty(PatronymicName)
                ? PatronymicName[0] + "."
                : "";

            return $"{Surname} {n}{p}";
        }

        public int? Age()
        {
            int? result = null;
            if (DateOfBirth != null)
            {
                DateTime now = DateTime.Today;
                int age = now.Year - DateOfBirth.Value.Year;
                if (DateOfBirth > now.AddYears(-age)) age--;
                result = age;
            }
            return result;
        }
    }

    public enum VariantState
    {
        [Display(Name = "Формируется")]
        Development = 0,
        [Display(Name = "На согласовании")]
        Review,
        [Display(Name = "Утверждена")]
        Approved
    }

    public class Variant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Название траектории")]
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string DocumentName { get; set; }
        [DisplayName("Состояние")]
        public VariantState State { get; set; }

        [DisplayName("Дата создания")]
        public DateTime? CreateDate { get; set; }

        public virtual ICollection<VariantGroup> Groups { get; set; }
        public virtual ICollection<VariantSelectionGroup> SelectionGroups { get; set; }

        [DisplayName("Дата окончания выбора")]
        public DateTime? SelectionDeadline { get; set; }

        public int EduProgramId { get; set; }

        [ForeignKey("EduProgramId")]
        public virtual EduProgram Program { get; set; }

        public virtual ICollection<VariantLimit> Limits { get; set; }

        public virtual ICollection<EduProgramLimit> ProgramLimits { get; set; }

        [DisplayName("Основной")]
        public bool IsBase { get; set; }

        [DisplayName("Лимит студентов")]
        public int StudentsLimit { get; set; }

        public void OnChanged()
        {
            Logger.Info("Изменён варинат '{0}' Id '{1}' статус '{2}'", Name, Id, State);

            Task.Run((Action)ExportVariant);

            if (Program != null)
                Program.OnChanged();
            else
                ApplicationDbContext.Create().EduPrograms.Find(EduProgramId).OnChanged();
        }

        private void ExportVariant()
        {
            try
            {
                ApplicationDbContext db = ApplicationDbContext.Create();
                var mce = new AutoMapper.Configuration.MapperConfigurationExpression();
                var config = new MapperConfiguration(mce);
                var mapper = new Mapper(config);

                (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
                var dto = mapper.Map<VariantApiDto>(db.Variants.Find(Id));
                EduProgram.FillTeachers(db, new List<VariantApiDto> { dto });
                PersonalCabinetService.PostVariant(dto);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public bool IsFgosVo()
        {
            return Program?.Direction?.standard == "ФГОС ВО";
        }
    }

    public class VariantGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Тип группы")]
        public VariantGroupType GroupType { get; set; }

        /// <summary>
        /// Из плана subgroupType
        /// </summary>
        [DisplayName("Тип подгруппы")]
        public VariantGroupType? SubgroupType { get; set; }

        [DisplayName("Зачётные единицы")]
        public int TestUnits { get; set; }

        [Required]
        [DisplayName("Траектория")]
        [Index("IX_VariantGroup_VariantId")]
        public int VariantId { get; set; }

        [ForeignKey("VariantId")]
        [DisplayName("Траектория")]
        public virtual Variant Variant { get; set; }

        public virtual ICollection<VariantContent> Contents { get; set; }


        [NotMapped]
        [DisplayName("Название группы")]
        public string Name
        {
            get { return GroupType.GetName(); }
        }
    }

    public enum VariantGroupType
    {
        [Display(Name = "Базовая часть")]
        Base = 0,
        [Display(Name = "Вариативная часть ВУЗа")]
        Variative = 1,
        [Display(Name = "Контроль")]
        Control = 2,
        [Display(Name = "По выбору студента")]
        Selectable = 3,
        [Display(Name = "Итоговая государственная аттестация")]
        Attestation = 6,
        [Display(Name = "Учебная и производственная практики")]
        Practiсe = 5,
        [Display(Name = "Контроль(базовая часть)")]
        BaseControl = 7,
        [Display(Name = "Контроль(вариативная часть)")]
        VariativeControl = 8,
        [Display(Name = "Контроль(По выбору)")]
        SelectionControl = 9,
        [Display(Name = "Обязательная часть")]
        Required = 10,
        [Display(Name = "Формируемая участниками образовательных отношений")]
        Formed = 11,
    }

    public static class VariantGroupTypeHelpers
    {
        private static Dictionary<string, VariantGroupType> names2enums = new Dictionary<string, VariantGroupType>();
        private static Dictionary<VariantGroupType, string> enums2names = new Dictionary<VariantGroupType, string>();

        static VariantGroupTypeHelpers()
        {
            foreach (VariantGroupType value in Enum.GetValues(typeof(VariantGroupType)))
            {
                var displayValue = EnumHelper<VariantGroupType>.GetDisplayValue(value);
                names2enums[displayValue] = value;
                enums2names[value] = displayValue;
            }
        }


        public static string GetName(this VariantGroupType src)
        {
            return enums2names[src];
        }

        public static VariantGroupType Parse(string src)
        {
            return names2enums[src];
        }

        public static VariantGroupType? TryParse(string src)
        {
            if (string.IsNullOrWhiteSpace(src))
                return null;/*
            if(src == "По выбору")
                return VariantGroupType.Selectable;*/
            VariantGroupType type;
            if (names2enums.TryGetValue(src, out type))
                return type;
            return null;
        }
    }

    public enum VariantContentType
    {
        [Display(Name = "Унифицированный")]
        Shared,
        [Display(Name = "Профессиональный")]
        Professional,
        [Display(Name = "По выбору (профессиональный)")]
        SelectableProfessional,
        [Display(Name = "По выбору (майонор)")]
        SelectableMinor,
        [Display(Name = "По выбору (альтернативный)")]
        SelectableAlternative,
    }

    public class VariantModuleType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Тип модуля")]
        public string Name { get; set; }
    }

    public class VariantSelectionGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Название группы выбора")]
        public string Name { get; set; }

        [DisplayName("Зачётные единицы")]
        public int TestUnits { get; set; }

        [DisplayName("Дата окончания выбора")]
        public DateTime? SelectionDeadline { get; set; }

        [Required]
        [DisplayName("Траектория")]
        [Index("IX_VariantSelectionGroup_VariantId")]
        public int VariantId { get; set; }

        [ForeignKey("VariantId")]
        [DisplayName("Траектория")]
        public virtual Variant Variant { get; set; }

        public virtual ICollection<VariantContent> Contents { get; set; }
    }

    public class VariantContent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Модуль")]
        [Index("IX_VariantContent_auto1", 0)]
        public string moduleId { get; set; }

        [ForeignKey("moduleId")]
        [DisplayName("Модуль")]
        public virtual Module Module { get; set; }

        [DisplayName("Включить модуль в траекторию")]
        [Index("IX_VariantContent_auto1", 1)]
        public bool Selected { get; set; }

        [DisplayName("Признак 'по выбору'")]
        public bool Selectable { get; set; }

        [DisplayName("Группа")]
        [Index("IX_VariantContent_VariantGroupId")]
        public int VariantGroupId { get; set; }

        [ForeignKey("VariantGroupId")]
        [DisplayName("Группа")]
        public virtual VariantGroup Group { get; set; }

        [DisplayName("Тип модуля")]
        public VariantContentType ContentType { get; set; }

        public int ModuleTypeId { get; set; }

        [DisplayName("Тип модуля")]
        [ForeignKey("ModuleTypeId")]
        public VariantModuleType ModuleType { get; set; }

        [DisplayName("Группа выбора")]
        [Index("IX_VariantSelectionContent_VariantGroupId")]
        public int? VariantSelectionGroupId { get; set; }

        [ForeignKey("VariantSelectionGroupId")]
        [DisplayName("Группа выбора")]
        public virtual VariantSelectionGroup SelectionGroup { get; set; }

        [DisplayName("Пререквизиты")]
        public virtual ICollection<VariantContent> Requirments { get; set; }

        public virtual ICollection<Subgroup> Subgroups { get; set; }

        public int[] RequirmentsIds { get; set; }

        public void UpdateRequirements(ApplicationDbContext db)
        {
            if (Requirments == null)
                Requirments = new List<VariantContent>();
            Requirments.Clear();
            if (RequirmentsIds != null && RequirmentsIds.Any())
                foreach (var requirement in db.VariantContents.Where(vc => RequirmentsIds.Contains(vc.Id)).ToList())
                {
                    Requirments.Add(requirement);
                }
        }

        public List<VariantContent> LoadEffectiveRequirements(ApplicationDbContext db)
        {
            var result = new List<VariantContent>();
            if (Requirments == null)
                return result;
            Action<IEnumerable<VariantContent>> recuresiveLookUp = null;
            recuresiveLookUp = (set) =>
            {
                List<int> extraLoads = new List<int>();
                foreach (var item in set)
                {

                    if (result.Any(r => r.Id == item.Id))
                        continue;
                    result.Add(item);
                    extraLoads.Add(item.Id);
                }
                foreach (var id in extraLoads)
                {
                    recuresiveLookUp(db.VariantContents.Where(vc => vc.Id == id).SelectMany(vc => vc.Requirments));
                }
            };
            recuresiveLookUp(Requirments);
            result = result.OrderBy(o => o.Module.shortTitle).ToList();
            return result;
        }

        public List<VariantContent> LoadPossibleRequirements(ApplicationDbContext db)
        {
            var result = Group.Variant.Groups.SelectMany(g => g.Contents).ToList();
            if (Requirments == null)
                return result;

            Func<VariantContent, IEnumerable<VariantContent>> getPostfixes = (vc) =>
            {
                return result.Where(r => r.Requirments.Contains(vc)).ToList();
            };


            Action<VariantContent> removeRecursive = null;
            removeRecursive = (vc) =>
            {
                if (result.Remove(vc))
                {
                    foreach (var postfix in getPostfixes(vc))
                    {
                        removeRecursive(postfix);
                    }
                }
            };

            removeRecursive(this);

            result = result.OrderBy(o => o.Module.shortTitle).ToList();
            return result;
        }
        public List<VariantContentRequirements> VariantContentRequirements { get; set; }
    }
    public class VariantContentRequirements
    {
        public int RequiredForId { get; set; }
        public VariantContent VariantContent { get; set; }
        public int RequirementId { get; set; }
    }


    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_Logs_Date")]
        public DateTime Date { get; set; }

        public string Ip { get; set; }
        public string HttpUser { get; set; }

        [MaxLength(8000)]
        public string Message { get; set; }

        [MaxLength(8000)]
        public string Exception { get; set; }
    }


    public class UserMinor
    {
        [Key, MaxLength(127), ForeignKey("User"), Column(Order = 0)]
        public string UserName { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Key, MaxLength(127), ForeignKey("Module"), Column(Order = 1)]
        public string ModuleId { get; set; }

        public virtual Module Module { get; set; }
    }

    public class UserDirection
    {
        [Key, MaxLength(127), ForeignKey("User"), Column(Order = 0)]
        public string UserName { get; set; }

        public ApplicationUser User { get; set; }

        [Key, MaxLength(127), ForeignKey("Direction"), Column(Order = 1)]
        public string DirectionId { get; set; }

        public Direction Direction { get; set; }
    }

    public class UserDivision
    {
        [Key, MaxLength(127), ForeignKey("User"), Column(Order = 0)]
        public string UserName { get; set; }

        public ApplicationUser User { get; set; }

        [Key, MaxLength(127), ForeignKey("Division"), Column(Order = 1)]
        public string DivisionId { get; set; }

        public Division Division { get; set; }
    }

    public class Semester
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(127)]
        public string Name { get; set; }
    }


    //Форма освоения модуля
    public class ModuleTech
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(127)]
        public string Name { get; set; }
    }

    //Дополнительные атрибуты для майнора
    public class Minor
    {
        [Key, MaxLength(127), ForeignKey("Module")]
        public string ModuleId { get; set; }

        public virtual Module Module { get; set; }

        [ForeignKey("Tech")]
        public int MinorTechId { get; set; }

        [DisplayName("Форма освоения майнора")]
        public virtual ModuleTech Tech { get; set; }


        [DisplayName("Отображать в личном кабинете студента")]
        public bool ShowInLC { get; set; }

        [DisplayName("Пререквизиты")]
        public virtual ICollection<Module> Requirments { get; set; }

        public virtual List<MinorPeriod> Periods { get; set; }

        public virtual ICollection<MinorDiscipline> Disciplines { get; set; }

        //свойство для Web-a в базу не пишем
        [NotMapped]
        public string RequirmentId { get; set; }

        //свойство для Web-a в базу не пишем
        [NotMapped]
        public string Requirment
        {
            get { return GetRequirmentTitle(); }
        }

        public string GetRequirmentId()
        {
            return Requirments?.FirstOrDefault()?.uuid;
        }

        public string GetRequirmentTitle()
        {
            return Requirments?.FirstOrDefault()?.title;
        }
        public List<MinorRequirements> MinorRequirements { get; set; }
    }
    public class MinorRequirements
    {
        public int RequiredForId { get; set; }
        public Minor Minor { get; set; }
        public int RequirementId { get; set; }
    }

    public class MinorPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Minor")]
        public string ModuleId { get; set; }

        [DisplayName("Год")]
        public int Year { get; set; }

        [ForeignKey("Semester")]
        public int SemesterId { get; set; }

        public DateTime? SelectionDeadline { get; set; }

        [DisplayName("Минимальное количество обучающихся")]
        public int MinStudentsCount { get; set; }

        [DisplayName("Максимальное количество обучающихся")]
        public int MaxStudentsCount { get; set; }


        public virtual Minor Minor { get; set; }

        [DisplayName("Семестр")]
        public virtual Semester Semester { get; set; }

    }

    public class MinorDiscipline
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Minor")]
        public string MinorId { get; set; }

        public virtual Minor Minor { get; set; }

        [ForeignKey("Discipline")]
        public string DisciplineUid { get; set; }

        public virtual Discipline Discipline { get; set; }

        public virtual IList<MinorDisciplineTmer> Tmers { get; set; }
    }

    public class MinorDisciplineTmer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Discipline")]
        public int MinorDisciplineId { get; set; }

        public virtual MinorDiscipline Discipline { get; set; }

        [ForeignKey("Tmer")]
        public string TmerId { get; set; }

        [DisplayName("Нагрузка")]
        public virtual Tmer Tmer { get; set; }

        [DisplayName("Периоды")]
        public virtual IList<MinorDisciplineTmerPeriod> Periods { get; set; }
    }

    public class MinorDisciplineTmerPeriod : IDisciplineTmerPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tmer")]
        public int MinorDisciplineTmerId { get; set; }

        public virtual MinorDisciplineTmer Tmer { get; set; }

        [ForeignKey("Period")]
        public int MinorPeriodId { get; set; }

        public virtual MinorPeriod Period { get; set; }

        public virtual ICollection<Division> Divisions { get; set; }

        public string GetDivisionsStr()
        {
            return string.Join(", ", Divisions.Select(d => $"{d.ParentShortName()}{d.typeTitle} {d.title}"));
        }

        [DisplayName("Количество подгрупп")]
        public int GroupCount { get; set; }

        public virtual ICollection<MinorSubgroup> Subgroups { get; set; }

        [DisplayName("Распределение")]
        public string Distribution { get; set; }

        public int[] ExtractDistribution()
        {
            CleanDistribution();
            if (string.IsNullOrWhiteSpace(Distribution))
                return new[] { 1 };
            var array = Distribution.Split(',').Select(CleanDistributionToken).ToArray();
            if (array.Length == 0)
                return new[] { 1 };
            return array;
        }

        private static int CleanDistributionToken(string arg)
        {
            int res;
            int.TryParse(arg, out res);
            if (res < 1)
                res = 1;
            return res;
        }

        public void CleanDistribution()
        {
            if (string.IsNullOrWhiteSpace(Distribution) || (Tmer.Tmer.kmer.ToLower() != "tlekc" && Tmer.Tmer.kmer.ToLower() != "tprak" && Tmer.Tmer.kmer.ToLower() != "tlab"))
            {
                Distribution = null;
                return;
            }
            var sb = new StringBuilder();
            bool commaIsPossible = false;
            foreach (var c in Distribution)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    commaIsPossible = true;
                }
                if (c == ',' && commaIsPossible)
                {
                    sb.Append(c);
                    commaIsPossible = false;
                }
            }
            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
            Distribution = sb.ToString();

            if (string.IsNullOrWhiteSpace(Distribution))
            {
                Distribution = null;
            }
        }
        public List<MinorDisciplineTmerPeriodDivision> MinorDisciplineTmerPeriodDivision { get; set; }
    }

    public class MinorAdmission
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }
        [Key, Column(Order = 1)]
        public int minorPeriodId { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("minorPeriodId")]
        public virtual MinorPeriod MinorPeriod { get; set; }

        public bool Published { get; set; }

        [DisplayName("Статус")]
        public AdmissionStatus Status { get; set; }
    }

    public class MinorSubgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Индекс")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Лимит")]
        public int Limit { get; set; }

        public int InnerNumber { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        public virtual MinorSubgroup Parent { get; set; }

        [ForeignKey("Meta")]
        public int MetaSubgroupId { get; set; }

        public virtual MinorDisciplineTmerPeriod Meta { get; set; }

        public double? ExpectedChildCount { get; set; }

        public virtual ICollection<MinorSubgroupMembership> Students { get; set; }
        public virtual ICollection<MinorSubgroup> Subgroups { get; set; }

        public bool MarksFrozen { get; set; }
        public string TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        [DisplayName("Преподаватель")]
        public Teacher Teacher { get; set; }
    }

    public class MinorSubgroupMembership
    {
        [Key, Column(Order = 1)]
        public string studentId { get; set; }

        [ForeignKey("studentId")]
        public Student Student { get; set; }

        [Key, Column(Order = 0)]
        public int SubgroupId { get; set; }

        [ForeignKey("SubgroupId")]
        public virtual MinorSubgroup Subgroup { get; set; }

        public decimal? Score { get; set; }

        public string Mark { get; set; }
    }

    public class FamilirizationType
    {
        [Key, MaxLength(127)]
        public string Name { get; set; }
    }

    public class FamilirizationTech
    {
        [Key, MaxLength(127)]
        public string Name { get; set; }
    }

    public class FamilirizationCondition
    {
        [Key, MaxLength(127)]
        public string Name { get; set; }
    }


    public class Qualification
    {
        [Key, MaxLength(127)]
        public string Name { get; set; }
    }

    public class EduProgramLimit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Index("IX_EduProgramLimit_Unique", 0, IsUnique = true)]
        public int VariantId { get; set; }

        public virtual Variant Variant { get; set; }

        [Required]
        [DisplayName("Модуль")]
        [Display(Name = "Модуль")]
        [Index("IX_EduProgramLimit_Unique", 1, IsUnique = true)]
        public string ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        [Display(Name = "Количество студентов")]
        public int StudentsCount { get; set; }
    }

    public class VariantLimit
    {
        [Key, Column(Order = 0)]
        public int LimitId { get; set; }
        [Key, Column(Order = 1)]
        public int VariantId { get; set; }
        [Display(Name = "Количество студентов")]
        public int StudentsCount { get; set; }

        [ForeignKey("VariantId")]
        public virtual Variant Variant { get; set; }
    }

    [Table("Teachers")]
    public class Teacher
    {
        [Key, MaxLength(127)]
        public string pkey { get; set; }
        [DisplayName("Подразделение")]
        public string workPlace { get; set; }
        [DisplayName("Отчество")]
        public string middleName { get; set; }
        [DisplayName("Фамилия")]
        public string lastName { get; set; }
        [DisplayName("Должность")]
        public string post { get; set; }
        [DisplayName("Сокращённое ФИО")]
        public string initials { get; set; }
        [DisplayName("Имя")]
        public string firstName { get; set; }
        [DisplayName("Код подразделения")]
        public string division { get; set; }

        /// <summary>
        /// Ученое звание. Поле сейчас не участвует в мэппинге
        /// </summary>
        public string academicTitle { get; set; }

        /// <summary>
        /// Ученая степень. Поле сейчас не участвует в мэппинге
        /// </summary>
        public string academicDegree { get; set; }

        public string UserId { get; set; }

        public string AccountancyGuid { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<SectionFKProperty> SectionFKProperties { get; set; }
        public virtual ICollection<ForeignLanguageProperty> ForeignLanguageProperties { get; set; }
        public virtual ICollection<MUPProperty> MUPProperties { get; set; }

        public virtual ICollection<PracticeTeacher> Practices { get; set; }

        //[DisplayName("Проекты")]
        //public virtual ICollection<ProjectUser> Projects { get; set; }

        [NotMapped]
        public string BigName
        {
            get { return $"{lastName} {firstName} {middleName} ({post}, {workPlace})"; }
        }

        [NotMapped]
        public string FullName
        {
            get { return $"{lastName} {firstName} {middleName}"; }
        }
        public List<MUPTeachers> MUPTeachers { get; set; }
        public List<SectionFKTeachers> SectionFKTeachers { get; set; }
        public List<ForeignLanguageTeachers> ForeignLanguageTeachers { get; set; }
    }

    public class PlanTeacher
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Вид нагрузки")]
        [Index("IX_PlanTeacher_Contents", 4), MaxLength(127)]
        public string load { get; set; }
        [Index("IX_PlanTeacher_Contents", 1), MaxLength(127)]
        public string moduleId { get; set; }
        [Index("IX_PlanTeacher_Contents", 2), MaxLength(127)]
        public string eduplanUuid { get; set; }
        [Index("IX_PlanTeacher_Contents", 0)]
        public int variantId { get; set; }
        [Index("IX_PlanTeacher_Contents", 3), MaxLength(127)]
        public string catalogDisciplineUuid { get; set; }
        public string TeacherPkey { get; set; }
        [ForeignKey("TeacherPkey")]
        public virtual Teacher Teacher { get; set; }
        [DisplayName("Признак выборности")]
        public bool Selectable { get; set; }
        [ForeignKey("moduleId")]
        public Module Module { get; set; }
        [ForeignKey("variantId")]
        public virtual Variant Variant { get; set; }
    }

    public enum AdmissionStatus
    {
        [Display(Name = "Нет решения")]
        Indeterminate = 0, //Статуса нет
        [Display(Name = "Зачислен")]
        Admitted = 1, //Зачислен
        [Display(Name = "Не зачислен")]
        Denied = 2, // 
        //ResetSelection // После публикации
    }

    public class VariantAdmission
    {
        [Key, MaxLength(127), Column(Order = 0)]
        [Index("IX_VariantAdmission_Contents1", 1)]
        [Index("IX_VariantAdmission_Contents2", 0)]
        public string studentId { get; set; }
        [Key, Column(Order = 1)]
        [Index("IX_VariantAdmission_Contents1", 0)]
        [Index("IX_VariantAdmission_Contents2", 1)]
        public int variantId { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("variantId")]
        public virtual Variant Variant { get; set; }

        public bool Published { get; set; }
        [DisplayName("Статус")]
        [Index("IX_VariantAdmission_Contents1", 2)]
        [Index("IX_VariantAdmission_Contents2", 2)]
        public AdmissionStatus Status { get; set; }
    }

    public class ModuleAdmission
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }
        [Key, MaxLength(127), Column(Order = 1)]
        public string moduleId { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }
        [ForeignKey("moduleId")]
        public virtual Module Module { get; set; }

        public bool Published { get; set; }
        [DisplayName("Статус")]
        public AdmissionStatus Status { get; set; }
    }

    public class StudentVariantSelection
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }

        [DisplayName("Приоритет")]
        public int selectedVariantPriority { get; set; }

        [Key, Column(Order = 1)]
        public int selectedVariantId { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("selectedVariantId")]
        public virtual Variant Variant { get; set; }

    }

    public class StudentSelectionTeacher
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }

        [Key, Column(Order = 1)]
        public int selectedVariantPriority { get; set; }
        [Key, MaxLength(127), Column(Order = 2)]
        public string disciplineUUID { get; set; }
        [Key, MaxLength(127), Column(Order = 3)]
        public string control { get; set; }

        public string pkey { get; set; }

        [ForeignKey("pkey")]
        public virtual Teacher Teacher { get; set; }

        [ForeignKey("disciplineUUID")]
        public virtual Discipline Discipline { get; set; }
    }

    public class StudentSelectionPriority
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }

        [Key, Column(Order = 1)]
        public int variantId { get; set; }

        [ForeignKey("variantId")]
        public virtual Variant Variant { get; set; }

        [Key, Column(Order = 2)]
        public int variantContentId { get; set; }

        [ForeignKey("variantContentId")]
        public VariantContent VariantContent { get; set; }

        public int proprity { get; set; }
    }

    public class StudentSelectionMinorPriority
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }

        [Key, Column(Order = 1)]
        public int minorPeriodId { get; set; }

        [ForeignKey("minorPeriodId")]
        public virtual MinorPeriod MinorPeriod { get; set; }

        public int minornumber { get; set; }

        public int priority { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }
    }

    public class Subgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Индекс")]
        [Required]
        public string Name { get; set; }


        [DisplayName("Лимит")]
        public int Limit { get; set; }

        public int InnerNumber { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Subgroup Parent { get; set; }

        public int MetaSubgroupId { get; set; }

        [ForeignKey("MetaSubgroupId")]
        public virtual MetaSubgroup Meta { get; set; }

        public double? ExpectedChildCount { get; set; }

        public bool Removed { get; set; }

        public virtual ICollection<SubgroupMembership> Students { get; set; }

    }

    public class MetaSubgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string groupId { get; set; }

        [ForeignKey("groupId")]
        public Group Group { get; set; }

        public string moduleId { get; set; }

        [DisplayName("Семестр")]
        public int Term { get; set; }

        public Int32 Year { get; set; }

        [ForeignKey("moduleId")]
        public virtual Module Module { get; set; }

        public int programId { get; set; }

        [ForeignKey("programId")]
        public virtual EduProgram Program { get; set; }

        public string kmer { get; set; }

        [ForeignKey("kmer")]
        public virtual Tmer Tmer { get; set; }

        [MaxLength(127)]
        public string catalogDisciplineUuid { get; set; }

        [MaxLength(127)]
        public string disciplineUUID { get; set; }
        [MaxLength(127)]
        public string additionalUUID { get; set; }

        [DisplayName("Количество подгрупп")]
        public int Count { get; set; }

        [DisplayName("Выборные подгруппы")]
        public bool Selectable { get; set; }

        public virtual ICollection<Subgroup> Subgroups { get; set; }

        [DisplayName("Распределение")]
        public string Distribution { get; set; }

        public int[] ExtractDistribution()
        {
            CleanDistribution();
            if (string.IsNullOrWhiteSpace(Distribution))
                return new[] { 1 };
            var array = Distribution.Split(',').Select(CleanDistributionToken).ToArray();
            if (array.Length == 0)
                return new[] { 1 };
            return array;
        }

        private static int CleanDistributionToken(string arg)
        {
            int res;
            int.TryParse(arg, out res);
            if (res < 1)
                res = 1;
            return res;
        }

        public void CleanDistribution()
        {
            if (string.IsNullOrWhiteSpace(Distribution) || (kmer != "tlekc" && kmer != "tprak" && kmer != "tlab"))
            {
                Distribution = null;
                return;
            }
            var sb = new StringBuilder();
            bool commaIsPossible = false;
            foreach (var c in Distribution)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    commaIsPossible = true;
                }
                if (c == ',' && commaIsPossible)
                {
                    sb.Append(c);
                    commaIsPossible = false;
                }
            }
            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
            Distribution = sb.ToString();

            if (string.IsNullOrWhiteSpace(Distribution))
            {
                Distribution = null;
            }
        }
    }

    public class SubgroupMembership
    {
        [Key, Column(Order = 1)]
        public string studentId { get; set; }

        [ForeignKey("studentId")]
        public Student Student { get; set; }

        [Key, Column(Order = 0)]
        public int SubgroupId { get; set; }

        [ForeignKey("SubgroupId")]
        public virtual Subgroup Subgroup { get; set; }
    }

    [Table("Apploads")]
    public class Appload
    {
        public int labSubgroups { get; set; }
        public string actionTitle { get; set; }
        [Index("IX_ApploadDto_eduDiscipline", 0), MaxLength(256)]
        [Index("IX_ApploadDto_auto1", 4)]
        public string eduDiscipline { get; set; }
        public string duModule { get; set; }
        public int? lectureFlows { get; set; }
        public string chair { get; set; }
        [Index("IX_ApploadDto_eduDiscipline", 1), MaxLength(256)]
        [Index("IX_ApploadDto_discipline", 1)]
        [Index("IX_ApploadDto_auto1", 0)]
        public string grp { get; set; }
        public string action { get; set; }
        public decimal value { get; set; }
        [Index("IX_ApploadDto_eduDiscipline", 2), MaxLength(256)]
        public string dckey { get; set; }
        public int? practiceFlows { get; set; }
        [Key, MaxLength(256)]
        public string uuid { get; set; }
        [Index("IX_ApploadDto_discipline", 0), MaxLength(256)]
        public string discipline { get; set; }
        public string detailDiscipline { get; set; }

        [DefaultValue(2015)]
        [Index("IX_ApploadDto_auto1", 3)]
        public int year { get; set; }
        [DefaultValue(1)]
        [Index("IX_ApploadDto_discipline", 3)]
        [Index("IX_ApploadDto_auto1", 5)]
        public int term { get; set; }

        [Index("IX_ApploadDto_auto1", 2)]
        public bool removed { get; set; }

        /// <summary>
        /// Уровень, используется для дисциплин по проектному обучению 
        /// </summary>
        public string Level { get; set; }

        [Index("IX_ApploadDto_eduDiscipline", 4)]
        [Index("IX_ApploadDto_discipline", 4)]
        [Index("IX_ApploadDto_auto1", 1)]
        public ApploadStatus status { get; set; }

        public DisciplineType DisciplineType { get; set; }
    }
    public enum DisciplineType
    {
        Other,
        SectionFK,
        ForeignLanguage,
        Minor,
        Project,

        /// <summary>
        /// Парный модуль
        /// </summary>
        PairedModule,

        MUP
    }

    public enum ModuleTypeParam
    {

        All = 0,
        SectionFk = 1,
        ForeignLanguage = 2,
        Minor = 3,
        Project = 4,
        PairedModule = 5,
        MUP = 6,
        ITS = 20
    }
    public enum ModuleType
    {
        SectionFk = 1,
        ForeignLanguage = 2,
        Minor = 3,
        Project = 4
    }
    public enum ApploadStatus
    {
        Development = 0,
        Review = 1,
        Approved = 2,
        Rejected = 3,
    }

    [Table("Tmer")]
    public class Tmer
    {
        public int? kgmer { get; set; }

        [Key]
        public string kmer { get; set; }

        [DisplayName("Нагрузка")]
        public string rmer { get; set; }
        public int kunr { get; set; }

        public int? ktur { get; set; }
        public int? kediz { get; set; }
        public int npp { get; set; }

        public int lnormzd { get; set; }
        public bool techLoad { get; set; }

        public bool techControl { get; set; }

        public bool techSingle { get; set; }

    }

    public class RoleSet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Название набора")]
        [MaxLength(255)]
        public string Name { get; set; }

        public virtual ICollection<RoleSetContent> Contents { get; set; }
    }

    public class RoleSetContent
    {
        [Key, Column(Order = 0)]
        public int RoleSetId { get; set; }

        [Key, Column(Order = 1)]
        public string RoleId { get; set; }

        [ForeignKey("RoleSetId")]
        public virtual RoleSet RoleSet { get; set; }

        [ForeignKey("RoleId")]
        public virtual IdentityRole Role { get; set; }
    }

    public class MinorAutoAdmissionReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        public ModuleType ModuleType { get; set; }
    }
    public class SectionFK
    {
        [Key, MaxLength(127), ForeignKey("Module")]
        public string ModuleId { get; set; }

        public virtual Module Module { get; set; }


        public int ModuleTechId { get; set; }

        [DisplayName("Форма освоения Секции ФК")]
        public virtual ModuleTech Tech { get; set; }

        [DisplayName("Отображать в личном кабинете студента")]
        public bool ShowInLC { get; set; }

        [DisplayName("Без выбора")]
        public bool WithoutPriorities { get; set; }

        public virtual List<SectionFKPeriod> Periods { get; set; }

        public virtual ICollection<SectionFKDiscipline> Disciplines { get; set; }


    }

    public class SectionFKAdmission
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }
        [Key, Column(Order = 1)]
        [Index("IX_SectionFKAdmission_Count", 1)]
        [Index("IX_SectionFKAdmission_SectionFKCompetitionGroupId", 0)]
        public int SectionFKCompetitionGroupId { get; set; }

        [Key, Column(Order = 2)]
        [Index("IX_SectionFKAdmission_Count", 0)]
        public string SectionFKId { get; set; }

        [ForeignKey("SectionFKId")]
        public virtual SectionFK SectionFK { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("SectionFKCompetitionGroupId")]
        public virtual SectionFKCompetitionGroup SectionFKCompetitionGroup { get; set; }

        public bool Published { get; set; }

        [DisplayName("Статус")]
        [Index("IX_SectionFKAdmission_Count", 2)]
        public AdmissionStatus Status { get; set; }
    }

    public class SectionFKAutoAdmissionReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "ntext")]
        public string Content { get; set; }
    }

    public class SectionFKAutoMoveReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string FileName { get; set; }

        public byte[] Content { get; set; }
    }

    public class SectionFKPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SectionFK")]
        public string SectionFKId { get; set; }

        [DisplayName("Год")]
        public int Year { get; set; }

        public bool? Male { get; set; }

        [ForeignKey("Semester")]
        public int SemesterId { get; set; }

        public DateTime? SelectionBegin { get; set; }
        public DateTime? SelectionDeadline { get; set; }

        public int? Course { get; set; }

        public virtual SectionFK SectionFK { get; set; }

        [DisplayName("Семестр")]
        public virtual Semester Semester { get; set; }

    }

    public class SectionFKDiscipline
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SectionFK")]
        public string SectionFKId { get; set; }

        public virtual SectionFK SectionFK { get; set; }

        [ForeignKey("Discipline")]
        public string DisciplineUid { get; set; }

        public virtual Discipline Discipline { get; set; }

        public virtual IList<SectionFKDisciplineTmer> Tmers { get; set; }
    }

    public class SectionFKDisciplineTmer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Discipline")]
        public int SectionFKDisciplineId { get; set; }

        public virtual SectionFKDiscipline Discipline { get; set; }

        [ForeignKey("Tmer")]
        public string TmerId { get; set; }

        [DisplayName("Нагрузка")]
        public virtual Tmer Tmer { get; set; }

        [DisplayName("Периоды")]
        public virtual IList<SectionFKDisciplineTmerPeriod> Periods { get; set; }
    }

    public class SectionFKDisciplineTmerPeriod : IDisciplineTmerPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tmer")]
        public int SectionFKDisciplineTmerId { get; set; }

        public virtual SectionFKDisciplineTmer Tmer { get; set; }

        [ForeignKey("Period")]
        public int SectionFKPeriodId { get; set; }

        public virtual SectionFKPeriod Period { get; set; }

        public virtual ICollection<Division> Divisions { get; set; }
        public virtual ICollection<SectionFKSubgroupCount> SectionFKSubgroupCounts { get; set; }

        public string GetDivisionsStr()
        {
            return string.Join(", ", Divisions.Select(d => $"{d.ParentShortName()}{d.typeTitle} {d.title}"));
        }



        [DisplayName("Распределение")]
        public string Distribution { get; set; }

        public int[] ExtractDistribution()
        {
            CleanDistribution();
            if (string.IsNullOrWhiteSpace(Distribution))
                return new[] { 1 };
            var array = Distribution.Split(',').Select(CleanDistributionToken).ToArray();
            if (array.Length == 0)
                return new[] { 1 };
            return array;
        }

        private static int CleanDistributionToken(string arg)
        {
            int res;
            int.TryParse(arg, out res);
            if (res < 1)
                res = 1;
            return res;
        }

        public void CleanDistribution()
        {
            if (string.IsNullOrWhiteSpace(Distribution) || (Tmer.Tmer.kmer.ToLower() != "tlekc" && Tmer.Tmer.kmer.ToLower() != "tprak" && Tmer.Tmer.kmer.ToLower() != "tlab"))
            {
                Distribution = null;
                return;
            }
            var sb = new StringBuilder();
            bool commaIsPossible = false;
            foreach (var c in Distribution)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    commaIsPossible = true;
                }
                if (c == ',' && commaIsPossible)
                {
                    sb.Append(c);
                    commaIsPossible = false;
                }
            }
            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
            Distribution = sb.ToString();

            if (string.IsNullOrWhiteSpace(Distribution))
            {
                Distribution = null;
            }
        }
        public List<SectionFKDisciplineTmerPeriodDivision> SectionFKDisciplineTmerPeriodDivision { get; set; }
    }
    public class SectionFKDisciplineTmerPeriodDivision
    {
        public string SectionFKDisciplineTmerPeriodId { get; set; }
        public Division Divisions { get; set; }
        public SectionFKDisciplineTmerPeriod SectionFKDisciplineTmerPeriod { get; set; }
        public string DivisionId { get; set; }
    }

    [Table("SectionFKSubgroupCounts")]
    public class SectionFKSubgroupCount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SectionFKDisciplineTmerPeriodId { get; set; }
        [ForeignKey("SectionFKDisciplineTmerPeriodId")]
        public SectionFKDisciplineTmerPeriod SectionFKDisciplineTmerPeriod { get; set; }

        public int CompetitionGroupId { get; set; }
        [ForeignKey("CompetitionGroupId")]
        public SectionFKCompetitionGroup CompetitionGroup { get; set; }
        [DisplayName("Колличество подгрупп")]
        public int GroupCount { get; set; }

        public virtual ICollection<SectionFKSubgroup> Subgroups { get; set; }
    }
    [Table("SectionFKStudentSelectionPriorities")]
    public class SectionFKStudentSelectionPriority
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }

        [Key, Column(Order = 1)]
        public int competitionGroupId { get; set; }

        [ForeignKey("competitionGroupId")]
        public virtual SectionFKCompetitionGroup CompetitionGroup { get; set; }

        [Key, Column(Order = 2)]
        public string sectionId { get; set; }

        [ForeignKey("sectionId")]
        public virtual SectionFK Section { get; set; }

        public int? priority { get; set; }

        public int? changePriority { get; set; }
        
        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        public DateTime modified { get; set; }
    }

    public class SectionFKSubgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Индекс")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Лимит")]
        public int Limit { get; set; }

        public int InnerNumber { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        public virtual SectionFKSubgroup Parent { get; set; }

        [ForeignKey("Meta")]
        public int SubgroupCountId { get; set; }

        public virtual SectionFKSubgroupCount Meta { get; set; }

        public double? ExpectedChildCount { get; set; }

        public virtual ICollection<SectionFKSubgroupMembership> Students { get; set; }

        public virtual ICollection<SectionFKSubgroup> Subgroups { get; set; }

        [ForeignKey("Teacher")]
        public string TeacherId { get; set; }

        [DisplayName("Преподаватель")]
        public virtual Teacher Teacher { get; set; }
    }

    public class SectionFKSubgroupMembership
    {
        [Key, Column(Order = 1)]
        public string studentId { get; set; }

        [ForeignKey("studentId")]
        public Student Student { get; set; }

        [Key, Column(Order = 0)]
        public int SubgroupId { get; set; }

        [ForeignKey("SubgroupId")]
        public virtual SectionFKSubgroup Subgroup { get; set; }
    }

    public class SectionFKCompetitionGroup : ICompetitionGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        [Range(1, 6)]
        public int StudentCourse { get; set; }
        [Required]
        [Range(2012, Int32.MaxValue)]
        public int Year { get; set; }
        [Required]
        public int SemesterId { get; set; }
        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }
        public virtual ICollection<SectionFKProperty> SectionFkProperties { get; set; }
        public virtual ICollection<Group> Groups { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Semester.Name.ToLower()} семестр, {Year}";
        }
        public List<SectionFKCompetitionGroupContents> SectionFKCompetitionGroupContents { get; set; }
    }
    public class SectionFKCompetitionGroupContents
    {
        public int GroupId { get; set; }
        public Group Groups { get; set; }
        public string SectionFKCompetitionGroupId { get; set; }
        public SectionFKCompetitionGroup SectionFkCompetitionGroups { get; set; }
    }


    public class SectionFKProperty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SectionFKCompetitionGroupId { get; set; }
        [ForeignKey("SectionFKCompetitionGroupId")]
        public virtual SectionFKCompetitionGroup SectionFkCompetitionGroup { get; set; }
        public String SectionFKId { get; set; }
        [ForeignKey("SectionFKId")]
        public virtual SectionFK SectionFk { get; set; }
        public int Limit { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
        public virtual ICollection<FirstTrainingPlaceFK> TrainingPlaces { get; set; }
        public List<SectionFKTeachers> SectionFKTeachers { get; set; }
        public List<SectionFKTrainingPlace> SectionFKTrainingPlace { get; set; }
    }
    public class SectionFKTrainingPlace
    {
        public int SectionFKPropertyId { get; set; }
        public SectionFKProperty SectionFKProperties { get; set; }
        public int TrainingPlaceId { get; set; }
        public FirstTrainingPlaceFK TrainingPlaces { get; set; }
    }
    public class SectionFKTeachers
    {
        public int SectionFKPropertyId { get; set; }
        public SectionFKProperty SectionFKProperties { get; set; }
        public string TeacherId { get; set; }
        public Teacher Teachers { get; set; }
    }

    public class FirstTrainingPlaceFK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Адрес")]
        public string Address { get; set; }
        [DisplayName("Описание")]
        public string Description { get; set; }
        public virtual ICollection<SectionFKProperty> SectionFKProperties { get; set; }
        public List<SectionFKTrainingPlace> SectionFKTrainingPlace { get; set; }
    }



    public class ForeignLanguage
    {
        [Key, MaxLength(127), ForeignKey("Module")]
        public string ModuleId { get; set; }

        public virtual Module Module { get; set; }

        public int ModuleTechId { get; set; }

        [DisplayName("Форма освоения модуля ИЯ")]
        public virtual ModuleTech Tech { get; set; }

        [DisplayName("Отображать в личном кабинете студента")]
        public bool ShowInLC { get; set; }
        public virtual List<ForeignLanguagePeriod> Periods { get; set; }

        public virtual ICollection<ForeignLanguageDiscipline> Disciplines { get; set; }
    }


    public class ForeignLanguageAdmission
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }
        [Key, Column(Order = 1)]
        [Index("IX_ForeignLanguageAdmission_Count", 1)]
        [Index("IX_ForeignLanguageAdmission_ForeignLanguageCompetitionGroupId", 0)]
        public int ForeignLanguageCompetitionGroupId { get; set; }

        [Key, Column(Order = 2)]
        [Index("IX_ForeignLanguageAdmission_Count", 0)]
        public string ForeignLanguageId { get; set; }

        [ForeignKey("ForeignLanguageId")]
        public virtual ForeignLanguage ForeignLanguage { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("ForeignLanguageCompetitionGroupId")]
        public virtual ForeignLanguageCompetitionGroup ForeignLanguageCompetitionGroup { get; set; }

        public bool Published { get; set; }

        [DisplayName("Статус")]
        [Index("IX_ForeignLanguageAdmission_Count", 2)]
        public AdmissionStatus Status { get; set; }
    }
    public class ForeignLanguageAutoAdmissionReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "ntext")]
        public string Content { get; set; }
    }
    public class ForeignLanguagePeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ForeignLanguage")]
        public string ForeignLanguageId { get; set; }

        [DisplayName("Год")]
        public int Year { get; set; }

        [ForeignKey("Semester")]
        public int SemesterId { get; set; }

        public DateTime? SelectionDeadline { get; set; }

        public virtual ForeignLanguage ForeignLanguage { get; set; }

        [DisplayName("Семестр")]
        public virtual Semester Semester { get; set; }

        [DisplayName("Курс")]
        public int? Course { get; set; }

    }

    public class ForeignLanguageDiscipline
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ForeignLanguage")]
        public string ForeignLanguageId { get; set; }

        public virtual ForeignLanguage ForeignLanguage { get; set; }

        [ForeignKey("Discipline")]
        public string DisciplineUid { get; set; }

        public virtual Discipline Discipline { get; set; }

        public virtual IList<ForeignLanguageDisciplineTmer> Tmers { get; set; }
    }

    public class ForeignLanguageDisciplineTmer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Discipline")]
        public int ForeignLanguageDisciplineId { get; set; }

        public virtual ForeignLanguageDiscipline Discipline { get; set; }

        [ForeignKey("Tmer")]
        public string TmerId { get; set; }

        [DisplayName("Нагрузка")]
        public virtual Tmer Tmer { get; set; }

        [DisplayName("Периоды")]
        public virtual IList<ForeignLanguageDisciplineTmerPeriod> Periods { get; set; }
    }

    public class ForeignLanguageDisciplineTmerPeriod : IDisciplineTmerPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tmer")]
        public int ForeignLanguageDisciplineTmerId { get; set; }

        public virtual ForeignLanguageDisciplineTmer Tmer { get; set; }

        [ForeignKey("Period")]
        public int ForeignLanguagePeriodId { get; set; }

        public virtual ForeignLanguagePeriod Period { get; set; }

        public virtual ICollection<Division> Divisions { get; set; }
        public virtual ICollection<ForeignLanguageSubgroupCount> ForeignLanguageSubgroupCounts { get; set; }

        public string GetDivisionsStr()
        {
            return string.Join(", ", Divisions.Select(d => $"{d.ParentShortName()}{d.typeTitle} {d.title}"));
        }



        [DisplayName("Распределение")]
        public string Distribution { get; set; }

        public int[] ExtractDistribution()
        {
            CleanDistribution();
            if (string.IsNullOrWhiteSpace(Distribution))
                return new[] { 1 };
            var array = Distribution.Split(',').Select(CleanDistributionToken).ToArray();
            if (array.Length == 0)
                return new[] { 1 };
            return array;
        }

        private static int CleanDistributionToken(string arg)
        {
            int res;
            int.TryParse(arg, out res);
            if (res < 1)
                res = 1;
            return res;
        }

        public void CleanDistribution()
        {
            if (string.IsNullOrWhiteSpace(Distribution) || (Tmer.Tmer.kmer.ToLower() != "tlekc" && Tmer.Tmer.kmer.ToLower() != "tprak" && Tmer.Tmer.kmer.ToLower() != "tlab"))
            {
                Distribution = null;
                return;
            }
            var sb = new StringBuilder();
            bool commaIsPossible = false;
            foreach (var c in Distribution)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    commaIsPossible = true;
                }
                if (c == ',' && commaIsPossible)
                {
                    sb.Append(c);
                    commaIsPossible = false;
                }
            }
            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
            Distribution = sb.ToString();

            if (string.IsNullOrWhiteSpace(Distribution))
            {
                Distribution = null;
            }
        }
        public List<ForeignLanguageDisciplineTmerPeriodDivision> ForeignLanguageDisciplineTmerPeriodDivision { get; set; }
    }
    public class ForeignLanguageDisciplineTmerPeriodDivision
    {
        public int DivisionId { get; set; }
        public Division Divisions { get; set; }
        public int ForeignLanguageDisciplineTmerPeriodId { get; set; }
        public ForeignLanguageDisciplineTmerPeriod ForeignLanguageDisciplineTmerPeriods { get; set; }
    }

    [Table("ForeignLanguageSubgroupCounts")]
    public class ForeignLanguageSubgroupCount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ForeignLanguageDisciplineTmerPeriodId { get; set; }
        [ForeignKey("ForeignLanguageDisciplineTmerPeriodId")]
        public ForeignLanguageDisciplineTmerPeriod ForeignLanguageDisciplineTmerPeriod { get; set; }

        public int CompetitionGroupId { get; set; }
        [ForeignKey("CompetitionGroupId")]
        public ForeignLanguageCompetitionGroup CompetitionGroup { get; set; }
        [DisplayName("Колличество подгрупп")]
        public int GroupCount { get; set; }

        public virtual ICollection<ForeignLanguageSubgroup> Subgroups { get; set; }
    }
    [Table("ForeignLanguageStudentSelectionPriorities")]
    public class ForeignLanguageStudentSelectionPriority
    {
        public ForeignLanguageStudentSelectionPriority()
        {
            modified = DateTime.Now;
        }
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }

        [Key, Column(Order = 1)]
        public int competitionGroupId { get; set; }

        [ForeignKey("competitionGroupId")]
        public virtual ForeignLanguageCompetitionGroup CompetitionGroup { get; set; }

        public string sectionId { get; set; }

        [ForeignKey("sectionId")]
        public virtual ForeignLanguage ForeignLanguage { get; set; }


        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        public DateTime modified { get; set; }
    }

    public class ForeignLanguageSubgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Индекс")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Лимит")]
        public int Limit { get; set; }

        public int InnerNumber { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        public virtual ForeignLanguageSubgroup Parent { get; set; }

        [ForeignKey("Meta")]
        public int SubgroupCountId { get; set; }

        public virtual ForeignLanguageSubgroupCount Meta { get; set; }

        public double? ExpectedChildCount { get; set; }

        public virtual ICollection<ForeignLanguageSubgroupMembership> Students { get; set; }
        public virtual ICollection<ForeignLanguageSubgroup> Subgroups { get; set; }

        [ForeignKey("Teacher")]
        public string TeacherId { get; set; }

        [DisplayName("Преподаватель")]
        public virtual Teacher Teacher { get; set; }
        [DisplayName("Комментарий")]
        public String Description { get; set; }
    }

    public class ForeignLanguageSubgroupMembership
    {
        [Key, Column(Order = 1)]
        public string studentId { get; set; }

        [ForeignKey("studentId")]
        public Student Student { get; set; }

        [Key, Column(Order = 0)]
        public int SubgroupId { get; set; }

        [ForeignKey("SubgroupId")]
        public virtual ForeignLanguageSubgroup Subgroup { get; set; }
    }

    public class ForeignLanguageCompetitionGroup : ICompetitionGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        [Range(1, 6)]
        public int StudentCourse { get; set; }
        [Required]
        [Range(2012, Int32.MaxValue)]
        public int Year { get; set; }
        [Required]
        public int SemesterId { get; set; }
        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }
        public virtual ICollection<ForeignLanguageProperty> ForeignLanguageProperties { get; set; }
        public virtual ICollection<Group> Groups { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Semester.Name.ToLower()} семестр, {Year}";
        }
        public List<ForeignLanguageCompetitionGroupContents> ForeignLanguageCompetitionGroupContents { get; set; }
    }
    public class ForeignLanguageCompetitionGroupContents
    {
        public int ForeignLanguageCompetitionGroupId { get; set; }
        public ForeignLanguageCompetitionGroup ForeignLanguageCompetitionGroups { get; set; }
        public string GroupId { get; set; }
        public Group Groups { get; set; }
    }

    public class ForeignLanguageProperty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ForeignLanguageCompetitionGroupId { get; set; }
        [ForeignKey("ForeignLanguageCompetitionGroupId")]
        public virtual ForeignLanguageCompetitionGroup ForeignLanguageCompetitionGroup { get; set; }
        public String ForeignLanguageId { get; set; }
        [ForeignKey("ForeignLanguageId")]
        public virtual ForeignLanguage ForeignLanguage { get; set; }
        public int Limit { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
        public List<ForeignLanguageTeachers> ForeignLanguageTeachers { get; set; }
    }
    public class ForeignLanguageTeachers
    {
        public int ForeignLanguagePropertyId { get; set; }
        public ForeignLanguageProperty ForeignLanguageProperties { get; set; }
        public string TeacherId { get; set; }
        public Teacher Teachers { get; set; }
    }

    [Table("StudentPlans")]
    public class StudentPlan
    {
        [Required]
        [Key, Column(Order = 0)]
        public string StudentId { get; set; }
        //[ForeignKey("StudentId")]
        //public Student Student { get; set; }
        [Required]
        [Key, Column(Order = 1)]
        public int PlanNumber { get; set; }
        [Required]
        [Key, Column(Order = 2)]
        public int VersionNumber { get; set; }
    }


    [Table("Companies")]
    public class Company
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Наименование"), MaxLength(255)]
        public string Name { get; set; }

        [DisplayName("Краткое наименование"), MaxLength(255)]
        public string ShortName { get; set; }

        [DisplayName("Тип собственности")]
        public int? OwnershipTypeId { get; set; }

        [DisplayName("ИНН"), MaxLength(20)]
        public string INN { get; set; }

        [DisplayName("Руководитель"), MaxLength(255)]
        public string Director { get; set; }

        [DisplayName("Инициалы руководителя"), MaxLength(40)]
        public string DirectorInitials { get; set; }

        [DisplayName("ФИО руководителя предприятия в родительном падеже"), MaxLength(255)]
        public string DirectorGenitive { get; set; }

        [DisplayName("Должность руководителя предприятия"), MaxLength(255)]
        public string PostOfDirector { get; set; }

        [DisplayName("Должность руководителя предприятия в родительном падеже"), MaxLength(255)]
        public string PostOfDirectorGenitive { get; set; }

        [DisplayName("Ответственное лицо"), MaxLength(255)]
        public string PersonInCharge { get; set; }

        [DisplayName("Инициалы ответственного лица"), MaxLength(40)]
        public string PersonInChargeInitials { get; set; }

        [DisplayName("Должность ответственного лица"), MaxLength(255)]
        public string PostOfPersonInCharge { get; set; }

        public int? CompanyLocationId { get; set; }

        [DisplayName("Адрес"), MaxLength(255)]
        public string Address { get; set; }

        [DisplayName("Телефон компании"), MaxLength(255)]
        public string CompanyPhoneNumber { get; set; }

        [DisplayName("Телефон ответственного лица"), MaxLength(255)]
        public string PhoneNumber { get; set; }

        [DisplayName("Почта"), MaxLength(255)]
        public string Email { get; set; }

        [DisplayName("Сайт"), MaxLength(255)]
        public string Site { get; set; }

        [DisplayName("Комментарий"), MaxLength(1024)]
        public string Comment { get; set; }

        public int? FileStorageId { get; set; }
        /// <summary>
        /// Источники: practice, project. Для обозначения используется статический класс Source
        /// </summary>
        public string Source { get; set; } = Urfu.Its.Web.Models.Source.Practice;

        [DisplayName("Статус подтвержден")]
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// Id с сайта партнера
        /// </summary>
        public string PartnerSiteId { get; set; }

        [ForeignKey("CompanyLocationId")]
        public virtual CompanyLocation Location { get; set; }

        [ForeignKey("OwnershipTypeId")]
        public virtual OwnershipTypes OwnershipType { get; set; }

        [ForeignKey("FileStorageId")]
        public virtual FileStorage FileStorage { get; set; }
    }

    public class OwnershipTypes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FullName { get; set; }

        public string ShortName { get; set; }
    }

    [Table("CompanyLocations")]
    public class CompanyLocation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ParentId { get; set; }

        public int Level { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Id с сайта партнера
        /// </summary>
        public int? PartnerSiteId { get; set; }

        [ForeignKey("ParentId")]
        public virtual CompanyLocation Parent { get; set; }

        public virtual ICollection<CompanyLocation> Childs { get; set; }

        public string City()
        {
            if (Level == 3)
                return Name;

            return null;
        }

        public string Country()
        {
            switch (Level)
            {
                case 3:
                    return Parent?.Parent?.Name;
                case 2:
                    return Parent?.Name;
                case 1:
                    return Name;
            }

            return null;
        }

        public string FullLocation()
        {
            string fullLocation = "";
            try
            {
                string country = Parent?.Parent?.Name;
                string region = Parent?.Name;
                string city = Name;

                fullLocation += country != null ? country + ", " : "";
                fullLocation += region != null ? region + ", " : "";
                fullLocation += city != null ? city : "";
            }
            catch { }
            return fullLocation;
        }
    }

    [Table("Contracts")]
    public class Contract
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        [DisplayName("Номер договора")]
        public string Number { get; set; }

        /// <summary>
        /// Порядковый номер к/с договора (для автоматической генерации номера)
        /// </summary>
        public int? SerialNumber { get; set; }

        /// <summary>
        /// Учебный год
        /// </summary>
        public int? Year { get; set; } 

        [DisplayName("Дата договора")]
        public DateTime? ContractDate { get; set; }

        [DisplayName("Начало действия договора")]
        public DateTime? StartDate { get; set; }

        [DisplayName("Окончание действия договора")]
        public DateTime? FinishDate { get; set; }

        [DisplayName("Руководитель"), MaxLength(255)]
        public string Director { get; set; }

        [DisplayName("Инициалы руководителя"), MaxLength(40)]
        public string DirectorInitials { get; set; }

        [DisplayName("ФИО руководителя предприятия в родительном падеже"), MaxLength(255)]
        public string DirectorGenitive { get; set; }

        [DisplayName("Должность руководителя предприятия"), MaxLength(255)]
        public string PostOfDirector { get; set; }

        [DisplayName("Должность руководителя предприятия в родительном падеже"), MaxLength(255)]
        public string PostOfDirectorGenitive { get; set; }

        [DisplayName("Ответственное лицо"), MaxLength(255)]
        public string PersonInCharge { get; set; }

        [DisplayName("Инициалы ответственного лица"), MaxLength(40)]
        public string PersonInChargeInitials { get; set; }

        [DisplayName("Должность ответственного лица"), MaxLength(255)]
        public string PostOfPersonInCharge { get; set; }

        [DisplayName("Телефон"), MaxLength(255)]
        public string PhoneNumber { get; set; }

        [DisplayName("Почта"), MaxLength(255)]
        public string Email { get; set; }

        public bool IsShortDated { get; set; }
        public bool IsEndless { get; set; }

        public int? Limit { get; set; }

        [DisplayName("Комментарий"), MaxLength(1024)]
        public string Comment { get; set; }

        [DisplayName("Комментарий учебного отдела"), MaxLength(1024)]
        public string PersonalComment { get; set; }

        public int? FolderNumber { get; set; }

        public int? FileStorageId { get; set; }
        /// <summary>
        /// Подразделение внутри компании
        /// </summary>
        public string Division { get; set; }

        /// <summary>
        /// Id с сайта партнера
        /// </summary>
        public string PartnerSiteId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        public virtual ICollection<ContractPeriod> Periods { get; set; }

        [ForeignKey("FileStorageId")]
        public virtual FileStorage FileStorage { get; set; }
    }


    [Table("ContractLimits")]
    public class ContractLimit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ContractPeriodId { get; set; }

        public string QualificationName { get; set; }

        public int Course { get; set; }

        public int Limit { get; set; }

        public string DirectionId { get; set; }
        
        [MaxLength(127)]
        public string ProfileId { get; set; }

        /// <summary>
        /// Id с сайта партнера
        /// </summary>
        public int? PartnerSiteId { get; set; }

        [ForeignKey("QualificationName")]
        public virtual Qualification Qualification { get; set; }

        [ForeignKey("DirectionId")]
        public virtual Direction Direction { get; set; }

        [ForeignKey("ContractPeriodId")]
        public virtual ContractPeriod Period { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }

        public int Prioritet()
        {
            var p = 0;
            p += Course == 0 ? 0 : 8;
            p += QualificationName == null ? 0 : 4;
            p += ProfileId == null ? 0 : 2;
            p += DirectionId == null ? 0 : 1;
            return p;
        }
    }

    [Table("ContractPeriods")]
    public class ContractPeriod
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ContractId { get; set; }

        public int Year { get; set; }

        public int SemesterId { get; set; }

        /// <summary>
        /// Id заявки с сайта партнера
        /// </summary>
        public int? RequestId { get; set; }

        /// <summary>
        /// Номер заявки с сайта партнера
        /// </summary>
        public string RequestNumber { get; set; }

        /// <summary>
        /// Вид деятельности отдела, где будет проходить практика
        /// </summary>
        public string DivisionDescription { get; set; }
        
        /// <summary>
        /// Дополнительные условия прохождения практики
        /// </summary>
        public string AdditionalTerms { get; set; }

        public int? FileStorageId { get; set; }

        [ForeignKey(nameof(FileStorageId))]
        public virtual FileStorage FileStorage { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }

        public virtual ICollection<ContractLimit> Limits { get; set; }
    }

    [Table("PracticeThemes")]
    public class PracticeTheme
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        public string DisciplineUUID { get; set; }

        public int Year { get; set; }

        public int SemesterId { get; set; }

        public string GroupHistoryId { get; set; }

        [StringLength(1000)]
        public string Theme { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }

        [ForeignKey("GroupHistoryId")]
        public virtual GroupsHistory Group { get; set; }

    }

    [Table("PracticeTeachers")]
    public class PracticeTeacher
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        public string DisciplineUUID { get; set; }

        public string GroupHistoryId { get; set; }

        public int Year { get; set; }

        public int SemesterId { get; set; }

        [MaxLength(127)]
        public string TeacherPKey { get; set; }

        public string Email { get; set; }

        [ForeignKey("TeacherPKey")]
        public virtual Teacher Teacher { get; set; }

        [ForeignKey("GroupHistoryId")]
        public virtual GroupsHistory Group { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }
    }


    [Table("PracticeInfo")]
    public class PracticeInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_PracticeInfo_DisciplineUUID", IsUnique = false)]
        [MaxLength(128)]
        public string DisciplineUUID { get; set; }

        public string GroupId { get; set; }

        public int SemesterId { get; set; }

        public int? PracticeWayId { get; set; }

        public int? PracticeTimeId { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        [DisplayName("ФИО исполнителя приказа"), MaxLength(255)]
        public string ExecutorName { get; set; }

        [DisplayName("Телефон исполнителя приказа"), MaxLength(255)]
        public string ExecutorPhone { get; set; }

        [DisplayName("Email исполнителя приказа"), MaxLength(255)]
        public string ExecutorEmail { get; set; }

        public string ROPInitials { get; set; }

        [MaxLength(255)]
        [DisplayName("Подразделение в УрФУ")]
        public string Subdivision { get; set; }

        [DisplayName("Форма освоения")]
        [ForeignKey("PracticeWayId")]
        public virtual PracticeWay Way { get; set; }

        [DisplayName("Способ проведения")]
        [ForeignKey("PracticeTimeId")]
        public virtual PracticeTime Time { get; set; }

        [ForeignKey("GroupId")]
        public virtual GroupsHistory Group { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }

        public DateTime? ReportBeginDate { get; set; }
        public DateTime? ReportEndDate { get; set; }
    }

    [Table("Practices")]
    public class Practice
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string StudentId { get; set; }

        [MaxLength(128)]
        public string DisciplineUUID { get; set; }

        public string GroupHistoryId { get; set; }

        public int Year { get; set; }

        public int SemesterId { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }


        [StringLength(1000)]
        public string FinishTheme { get; set; }

        public bool IsExternal { get; set; }

        public DateTime? ExternalBeginDate { get; set; }

        public DateTime? ExternalEndDate { get; set; }

        /// <summary>
        /// Содержит договор или нет (сделано для Нечепуренко) 
        /// </summary>
        public bool ExistContract { get; set; }

        public bool remove { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("GroupHistoryId")]
        public virtual GroupsHistory Group { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }

        public virtual ICollection<PracticeAdmission> Admissions { get; set; }

        public virtual ICollection<PracticeAdmissionCompany> AdmissionCompanys { get; set; }
        public virtual ICollection<PracticeDocument> Documents { get; set; }

        public virtual PracticeReview Review { get; set; }

        public DateTime? ReportBeginDate { get; set; }
        public DateTime? ReportEndDate { get; set; }
        public bool TakeDatesfromGroup { get; set; }
        public bool TakeReportDatesfromGroup { get; set; }
    }

    [Table("PracticeAdmissions")]
    public class PracticeAdmission
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PracticeId { get; set; }

        [MaxLength(127)]
        [DisplayName("Руководитель")]
        public string TeacherPKey { get; set; }

        [MaxLength(127)]
        [DisplayName("Соруководитель")]
        public string TeacherPKey2 { get; set; }

        public int? PracticeThemeId { get; set; }

        public AdmissionStatus Status { get; set; }

        [NotMapped]
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case AdmissionStatus.Admitted: return "Согласовано";
                    case AdmissionStatus.Denied: return "Отклонено";
                    case AdmissionStatus.Indeterminate: return "Формируется";
                }
                return Status.ToString();
            }
        }

        public DateTime CreateDate { get; set; }

        public string ReasonOfDeny { get; set; }

        [MaxLength(255)]
        [DisplayName("Подразделение в УрФУ")]
        public string Subdivision { get; set; }

        public bool remove { get; set; }

        /// <summary>
        /// Даты проведения практики
        /// json - сериализованный List of Dates (PracticeViewModel.cs class Dates)
        /// </summary>
        public string Dates { get; set; }

        [ForeignKey("PracticeId")]
        public virtual Practice Practice { get; set; }

        [ForeignKey("PracticeThemeId")]
        public virtual PracticeTheme Theme { get; set; }

        [ForeignKey("TeacherPKey")]
        public virtual Teacher Teacher { get; set; }

        [ForeignKey("TeacherPKey2")]
        public virtual Teacher Teacher2 { get; set; }

    }

    public enum PtraciceDecreeStatus
    {
        [Display(Name = "Нет приказа")]
        None = 0,
        [Display(Name = "Сформирован")]
        Create = 1,
        [Display(Name = "В обработке")]
        Sended = 2,
        [Display(Name = "В работе")]
        Processed = 3,
        [Display(Name = "На доработку")]
        Revision = 4,
        [Display(Name = "Согласован")]
        Sign = 5,
        [Display(Name = "Ошибка в СЭД")]
        ErorrSED = 6
    }

    [Table("PracticeDecreeNumbers")]
    public class PracticeDecreeNumber
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Year { get; set; }

        [MaxLength(20)]
        public string Number { get; set; }

        public DateTime? DecreeDate { get; set; }

        public string ChangedDecreeNumber { get; set; }
        public DateTime? ChangedDecreeDate { get; set; }
    }

    [Table("PracticeDecrees")]
    public class PracticeDecree
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        public string GroupId { get; set; }

        //простые семестры
        public int? SemesterID { get; set; }

        //сквозной семестр
        public int? Term { get; set; }

        [MaxLength(128)]
        public string DisciplineUUID { get; set; }

        public PtraciceDecreeStatus Status { get; set; }

        [MaxLength(20)]
        public string DecreeNumber { get; set; }

        public DateTime? DecreeDate { get; set; }

        public int? SedId { get; set; }

        public string Comment { get; set; }

        [ForeignKey("GroupId")]
        public virtual GroupsHistory Group { get; set; }
        
        public DateTime? DateExportToSed { get; set; }

        [NotMapped]
        public string StatusName
        {
            get
            {
                return DecreeStatusNames[Status];
            }
        }

        [NotMapped]
        public static Dictionary<PtraciceDecreeStatus, string> DecreeStatusNames = new Dictionary<PtraciceDecreeStatus, string>()
        {
            { PtraciceDecreeStatus.Create, "Сформирован" },
            { PtraciceDecreeStatus.Sended, "Отправлен в СЭД" },
            { PtraciceDecreeStatus.Processed, "В работе" },
            { PtraciceDecreeStatus.Revision, "На доработку" },
            { PtraciceDecreeStatus.Sign, "Согласован" },
            { PtraciceDecreeStatus.ErorrSED, "Ошибка в СЭД" }
        };

        public int? FileStorageId { get; set; }

        [ForeignKey("FileStorageId")]
        public FileStorage FileStorage { get; set; }
    }

    [Table("PracticeAdmissionCompanys")]
    public class PracticeAdmissionCompany
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PracticeId { get; set; }

        public int ContractId { get; set; }

        public AdmissionStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string ReasonOfDeny { get; set; }

        public bool remove { get; set; }

        /// <summary>
        /// Даты проведения практики
        /// json - сериализованный List of Dates (PracticeViewModel.cs class Dates)
        /// </summary>
        public string Dates { get; set; }

        [ForeignKey("PracticeId")]
        public virtual Practice Practice { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }

        public bool Agreement { get; set; }

        [NotMapped]
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case AdmissionStatus.Admitted: return "Согласовано";
                    case AdmissionStatus.Denied: return "Отклонено";
                    case AdmissionStatus.Indeterminate: return "Формируется";
                }
                return Status.ToString();
            }
        }

    }

    //парочка справочников

    //Способ проведения: стационарная; выездная; стационарная, выездная; полевая
    [Table("PracticeWays")]
    public class PracticeWay
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Description { get; set; }
    }

    //Форма освоения: частично рассредоточенная, рассредоточенная, непрерывная
    [Table("PracticeTimes")]
    public class PracticeTime
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public enum PracticeDocumentType
    {
        [Display(Name = "Договор с предприятием")]
        Contract = 1,

        [Display(Name = "Резюме")]
        Resume = 2,

        [Display(Name = "Письмо направление на предприятие")]
        Referral = 3, // 

        [Display(Name = "Командирововчное удостоверение")]
        Trip = 4,

        [Display(Name = "Заявление студента о переносе практики")]
        Postponement = 5,

        [Display(Name = "Извещение о прохождении практики")]
        Notice = 6,

        [Display(Name = "Задание на практику")]
        Task = 7,

        [Display(Name = "Отчет по практике")]
        Report = 8,

        [Display(Name = "Отзыв руководителя")]
        Review = 9,

        [Display(Name = "Отказное письмо")]
        Rejection = 10,

        [Display(Name = "Договор с предприятием")]
        DistantContract = 11,

        [Display(Name = "Письмо направление на предприятие")]
        DistantReferral = 12,

        [Display(Name = "Задание на практику")]
        DistantTask = 13
    }

    public class PracticFileDescriptor
    {
        public PracticeDocumentType Type { get; set; }
        public string TypeName { get; set; }

        //имя файла для скачивания
        public string FileName { get; set; }
        //шаблон
        public string TemplateName { get; set; }

        /// <summary>
        /// Всплывающая подсказка при наведении
        /// </summary>
        public string Title { get; set; }

        private static List<PracticFileDescriptor> _files = new List<PracticFileDescriptor>
        {
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Contract,
                     TypeName = "Договор с предприятием",
                     FileName ="Договор о проведении практики.docx",
                     TemplateName ="dogovor.docx",
                     Title = "При условии отсутствия предприятия (организации) в предложенном перечне предприятий (организаций)"
                 },
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Resume,
                     TypeName = "Резюме",
                     FileName ="Резюме.docx",
                     TemplateName ="resume.docx"
                 },
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Referral,
                     TypeName = "Письмо-направление на предприятие",
                     FileName ="Письмо-направление на предприятие.docx",
                     TemplateName ="referral.docx",
                     Title = "Для краткосрочного договора"
                 },
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Trip,
                     TypeName = "Направление на практику",
                     FileName ="Направление на практику.docx",
                     TemplateName ="trip.docx",
                     Title = "Если практика с выездом за пределы Екатеринбурга"
                 },
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Postponement,
                     TypeName = "Заявление студента о переносе практики",
                     FileName ="Заявление о переносе практики.docx",
                     TemplateName ="postponement.docx"
                 },
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Notice,
                     TypeName = "Извещение о руководителе практики от организации",
                     FileName ="Извещение о руководителе практики от организации.docx",
                     TemplateName ="notice.docx",
                     Title = "Если нет иного документа: приказа, распоряжения, письма, подтверждающего назначение руководителя практики от организации"
                 },
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Task,
                     TypeName = "Задание на практику",
                     FileName ="Индивидуальное задание на учебную производственную(преддипломную) практику студента.docx",
                     TemplateName ="task.docx"
                 },
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Report,
                     TypeName = "Отчет по практике",
                     FileName ="Отчет по практике.docx",
                     TemplateName ="report.docx"
                 },
                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Review,
                     TypeName = "Отзыв руководителя",
                     FileName ="Отзыв руководителя.docx",
                     TemplateName ="review.docx"
                 },

                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.Rejection,
                     TypeName = "Отказное письмо",
                 },

                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.DistantContract,
                     TypeName = "Договор с предприятием",
                     FileName ="Договор о проведении практики для дистанционной формы.docx",
                     TemplateName ="distantdogovor.docx",
                     Title = "При условии отсутствия предприятия (организации) в предложенном перечне предприятий (организаций)"
                 },

                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.DistantReferral,
                     TypeName = "Письмо-направление на предприятие",
                     FileName ="Письмо-направление на предприятие для дистанционной формы.docx",
                     TemplateName ="distantreferral.docx",
                     Title = "Для краткосрочного договора"
                 },

                 new PracticFileDescriptor
                 {
                     Type = PracticeDocumentType.DistantTask,
                     TypeName = "Задание на практику",
                     FileName ="Индивидуальное задание на учебную производственную(преддипломную) практику студента для дистанционной формы.docx",
                     TemplateName ="distanttask.docx"
                 },
        };

        public static PracticFileDescriptor Get(PracticeDocumentType type)
        {
            return _files.FirstOrDefault(f => f.Type == type);
        }

        public static List<PracticFileDescriptor> Before()
        {
            return new List<PracticFileDescriptor> {
                Get(PracticeDocumentType.Contract),
                Get(PracticeDocumentType.Resume),
                Get(PracticeDocumentType.Referral),
                Get(PracticeDocumentType.Trip),
                Get(PracticeDocumentType.Postponement),
                Get(PracticeDocumentType.Rejection),
                Get(PracticeDocumentType.Task)
            };
        }

        public static List<PracticFileDescriptor> After()
        {
            return new List<PracticFileDescriptor> {
                Get(PracticeDocumentType.Notice),
                Get(PracticeDocumentType.Report),
                Get(PracticeDocumentType.Review),
            };
        }

        public static List<PracticFileDescriptor> Distant()
        {
            return new List<PracticFileDescriptor> {
                Get(PracticeDocumentType.DistantContract),
                Get(PracticeDocumentType.DistantReferral),
                Get(PracticeDocumentType.DistantTask)
            };
        }
    }

    [Table("PracticeDocuments")]
    public class PracticeDocument
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PracticeId { get; set; }
        public PracticeDocumentType DocumentType { get; set; }

        public AdmissionStatus Status { get; set; }

        [DisplayName("Комментарий"), MaxLength(1024)]
        public string Comment { get; set; }

        public int? FileStorageId { get; set; }

        [ForeignKey("PracticeId")]
        public virtual Practice Practice { get; set; }

        [ForeignKey("FileStorageId")]
        public FileStorage FileStorage {get;set;}
    }

    [Table("RequisitesOrdersFGOS")]
    public class RequisiteOrderFGOS
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DirectionId { get; set; }
        [ForeignKey(nameof(DirectionId))]
        public Direction Direction { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public string Order { get; set; }
    }

    [Table("Competences")]
    public class Competence
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Code { get; set; }

        public int Order { get; set; }

        [MaxLength(4000)]
        public string Content { get; set; }

        [Required]
        public string Type { get; set; }

        [ForeignKey(nameof(Type))]
        public virtual CompetenceType TypeInfo { get; set; }

        public string DirectionId { get; set; }

        [ForeignKey(nameof(DirectionId))]
        public virtual Direction Direction { get; set; }

        [ForeignKey(nameof(ProfileId))]
        public virtual Profile Profile { get; set; }
        public string ProfileId { get; set; }

        public string Standard { get; set; }

        [ForeignKey(nameof(Standard))]
        public virtual Standard StandardInfo { get; set; }

        [MaxLength(8)]
        public string Okso { get; set; }

        public int? ExternalId { get; set; }
        [Column(TypeName = "date")]
        public DateTime ApprovedDate { get; set; }

        public bool IsDeleted { get; set; }
        public int? KindActionId { get; set; }
        [ForeignKey("KindActionId")]
        public virtual KindAction KindAction { get; set; }

        public int? AreaEducationId { get; set; }
        [ForeignKey("AreaEducationId")]
        public virtual AreaEducation AreaEducation { get; set; }

        public int? CompetenceGroupId { get; set; }
        [ForeignKey(nameof(CompetenceGroupId))]
        public virtual CompetenceGroup CompetenceGroup { get; set; }

        public string QualificationName { get; set; }

        public virtual ICollection<EduResult2> EduResults { get; set; }
    }

    [Table("KindAction")]
    public class KindAction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Okso { get; set; }

        public string DirectionId { get; set; }
        [ForeignKey("DirectionId")]
        public Direction Direction { get; set; }

        public string Name { get; set; }
        public int? ExternalId { get; set; }

    }

    [Table("CompetenceTypes")]
    public class CompetenceType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; }

        public bool IsStandard { get; set; }
    }

    [Table("Standards")]
    public class Standard
    {
        [Key, MaxLength(20)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Результат обучения (РО)
    /// </summary>
    [Table("EduResults")]
    public class EduResult
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CodeNumber { get; set; }

        [Required, MaxLength(1000)]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public string ProfileId { get; set; }

        [ForeignKey(nameof(ProfileId))]
        public virtual Profile Profile { get; set; }
    }

    /// <summary>
    /// Типы версионизированных документов
    /// </summary>
    public enum VersionedDocumentType
    {
        [Display(Name = "РПМ")]
        ModuleWorkingProgram = 1,
        [Display(Name = "РПД")]
        DisciplineWorkingProgram = 2,
        [Display(Name = "ГИА")]
        GiaWorkingProgram = 3,
        [Display(Name = "РПП")]
        PracticesWorkingProgram = 4,
        [Display(Name = "ЛИ")]
        ModuleChangeList = 5,
        [Display(Name = "ОХОП")]
        BasicCharacteristicOP = 6,
        //ModuleWorkingProgramFgosVo3 = 7,
        //DisciplineWorkingProgramFgosVo3 = 8,
        //GiaWorkingProgramFgosVo3 = 9,
        //PracticesWorkingProgramFgosVo3 = 10,
        [Display(Name = "Пасспорт компетенций")]
        CompetencePassport = 11,
        [Display(Name = "Аннотация модулей")]
        ModuleAnnotation = 12
    }

    /// <summary>
    /// Шаблоны версионизированных документов
    /// </summary>
    [Table("VersionedDocumentTemplates")]
    public class VersionedDocumentTemplate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Вордовский документ
        /// </summary>
        [Required]
        public byte[] Data { get; set; }

        /// <summary>
        /// Схема данных документа
        /// </summary>
        [Required]
        public string Schema { get; set; }

        public VersionedDocumentType DocumentType { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<VersionedDocument> Documents { get; set; }

        public int Version { get; set; }

        public int? PreviousTemplateId { get; set; }

        [ForeignKey(nameof(PreviousTemplateId))]
        public virtual VersionedDocumentTemplate PreviousTemplate { get; set; }
    }

    [Table("VersionedDocuments")]
    public class VersionedDocument
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public virtual VersionedDocumentTemplate Template { get; set; }

        public virtual ICollection<VersionedDocumentBlockLink> BlockLinks { get; set; }
    }

    /// <summary>
    /// Связь многие ко многим между документом и блоком данных
    /// </summary>
    [Table("VersionedDocumentBlockLinks")]
    public class VersionedDocumentBlockLink
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public virtual VersionedDocument Document { get; set; }

        public int DocumentBlockId { get; set; }

        [ForeignKey(nameof(DocumentBlockId))]
        public virtual VersionedDocumentBlock DocumentBlock { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    [Table("VersionedDocumentBlocks")]
    public class VersionedDocumentBlock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Сериализованные данные
        /// </summary>
        [Required]
        public string Data { get; set; }

        [Required]
        public string Name { get; set; }

        public int Version { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? PreviousBlockId { get; set; }

        [ForeignKey(nameof(PreviousBlockId))]
        public virtual VersionedDocumentBlock PreviousBlock { get; set; }

        public virtual ICollection<VersionedDocumentBlockLink> Links { get; set; }
    }

    [Table("UPOPStatuses")]
    public class UPOPStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool CanSend2Upop()
        {
            if (Name.Equals("Новый", StringComparison.InvariantCultureIgnoreCase) ||
                Name.Equals("Отклонен куратором ОП",
                    StringComparison.InvariantCultureIgnoreCase) ||
                Name.Equals("Не согласован", StringComparison.InvariantCultureIgnoreCase) ||
                Name.Equals("Не подписан", StringComparison.InvariantCultureIgnoreCase) ||
                Name.Equals("Формируется", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            return false;

        }

        public bool IsSigned()
        {
            return Name.Equals("Подписан", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanEdit()
        {
            return Name.Equals("Формируется", StringComparison.InvariantCultureIgnoreCase) || Name.Equals("Не подписан", StringComparison.InvariantCultureIgnoreCase);
        }

        public static UPOPStatus GetDefaultStatus(ApplicationDbContext db)
        {
            return db.UpopStatuses.FirstOrDefault(_=>_.Name.Equals("Формируется",StringComparison.InvariantCultureIgnoreCase));
        }
    }
    [Table("PlanTermWeeks")]
    public class PlanTermWeek
    {
        /// <summary>
        /// Ключ
        /// </summary>
        [MaxLength(128)]
        public string eduplanUUID { get; set; }

        /// <summary>
        /// Ключ, Семестр
        /// </summary>
        public int Term { get; set; }

        /// <summary>
        /// Количество недель
        /// </summary>
        public int WeeksCount { get; set; }
    }

    /// <summary>
    /// Связка модуля с РПМ. 
    /// TODO нужно понять какой тип связи нужен и нужны ли какие-то доп. свойства связи. Пока расчитано, что когда-нибудь появятся доп. свойства и связь сделана один ко многим.
    /// </summary>
    [Table("ModuleWorkingProgram")]
    public class ModuleWorkingProgram
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VersionedDocumentId { get; set; }

        [ForeignKey(nameof(VersionedDocumentId))]
        public virtual VersionedDocument VersionedDocument { get; set; }

        public string ModuleId { get; set; }

        [ForeignKey(nameof(ModuleId))]
        public virtual Module Module { get; set; }
        [ForeignKey("UpopStatusId")]
        public UPOPStatus Status { get; set; }
        public int? UpopStatusId { get; set; }

        public DateTime StatusChangeTime { get; set; }

        public virtual ICollection<DisciplineWorkingProgram> DisciplineWorkingPrograms { get; set; }

        [Required]
        public int Version { get; set; }

        [Required]
        public string StandardName { get; set; }

        [ForeignKey(nameof(StandardName))]
        public virtual Standard Standard { get; set; }

        public int? BasedOnId { get; set; }

        [ForeignKey(nameof(BasedOnId))]
        public virtual ModuleWorkingProgram BasedOn { get; set; }

        public virtual ICollection<ModuleWorkingProgramChangeList> TargetChangeLists { get; set; }
        public virtual ICollection<ModuleWorkingProgramChangeList> SourceChangeLists { get; set; }

        public virtual ICollection<BasicCharacteristicOP> BasicCharacteristicOPs { get; set; }
        public List<BasicCharacteristicOPMapping> BasicCharacteristicOPMapping { get; set; }
    }

    public enum ModuleWorkingProgramStatus
    {
        Draft
    }

    [Table("DisciplineWorkingPrograms")]
    public class DisciplineWorkingProgram
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VersionedDocumentId { get; set; }

        [ForeignKey(nameof(VersionedDocumentId))]
        public virtual VersionedDocument VersionedDocument { get; set; }

        // TODO сделать не нулэйблом. Теперь РПМ обязательна для РПД (01.02.2018, скайп)
        public int? ModuleWorkingProgramId { get; set; }

        [ForeignKey(nameof(ModuleWorkingProgramId))]
        public virtual ModuleWorkingProgram ModuleWorkingProgram { get; set; }

        public string DisciplineId { get; set; }

        [ForeignKey(nameof(DisciplineId))]
        public virtual Discipline Discipline { get; set; }

        [Required]
        public int Version { get; set; }

        [Required]
        public string StandardName { get; set; }

        [ForeignKey(nameof(StandardName))]
        public virtual Standard Standard { get; set; }
        public int? BasedOnId { get; set; }

        [ForeignKey(nameof(BasedOnId))]
        public virtual DisciplineWorkingProgram BasedOn { get; set; }
    }

    [Table("WorkingProgramAuthors")]
    public class WorkingProgramAuthor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Ученая степень
        /// </summary>
        [MaxLength(100)]
        public string AcademicDegree { get; set; }

        /// <summary>
        /// Ученое звание
        /// </summary>
        [MaxLength(100)]
        public string AcademicTitle { get; set; }

        [MaxLength(100)]
        public string Post { get; set; }

        [MaxLength(100)]
        public string Workplace { get; set; }
    }

    public class ModuleWorkingProgramChangeList
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VersionedDocumentId { get; set; }

        [ForeignKey(nameof(VersionedDocumentId))]
        public virtual VersionedDocument VersionedDocument { get; set; }

        public int SourceId { get; set; }

        [ForeignKey(nameof(SourceId))]
        public virtual ModuleWorkingProgram Source { get; set; }

        public int TargetId { get; set; }

        [ForeignKey(nameof(TargetId))]
        public virtual ModuleWorkingProgram Target { get; set; }
    }

    [Table("WorkingProgramResponseblePersons")]
    public class WorkingProgramResponsiblePerson
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        public string DisciplineId { get; set; }

        [ForeignKey(nameof(DisciplineId))]
        public virtual Discipline Discipline { get; set; }

        [Required, MaxLength(128)]
        public string ModuleId { get; set; }

        [ForeignKey(nameof(ModuleId))]
        public virtual Module Module { get; set; }

        [Required, MaxLength(128)]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }

    [Table("Directors")]
    public class Director
    {
        [Key]
        public string DivisionUuid { get; set; }

        [DisplayName("Фамилия")]
        public string Surname { get; set; }

        [DisplayName("Имя")]
        public string Name { get; set; }

        [DisplayName("Отчество")]
        public string PatronymicName { get; set; }

        [ForeignKey("DivisionUuid")]
        public virtual Division Division { get; set; }

        public string FullName()
        {
            return $"{Surname} {Name} {PatronymicName}";
        }

        /// <summary>
        /// Фимилия И.О.
        /// </summary>
        /// <returns></returns>
        public string ShortName()
        {
            var n = !string.IsNullOrEmpty(Name)
                ? Name[0] + "."
                : "";
            var p = !string.IsNullOrEmpty(PatronymicName)
                ? PatronymicName[0] + "."
                : "";

            return $"{Surname} {n}{p}";
        }

        /// <summary>
        /// И.О. Фамилия
        /// </summary>
        /// <returns></returns>
        public string ShortName2()
        {
            var n = Name != null
                ? Name[0] + "."
                : "";
            var p = PatronymicName != null
                ? PatronymicName[0] + "."
                : "";

            return $"{n}{p} {Surname}";
        }
    }

    [Table("PracticeChangedDecrees")]
    public class PracticeChangedDecree
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MainDecreeId { get; set; }

        public PtraciceDecreeStatus Status { get; set; }

        [MaxLength(20)]
        public string DecreeNumber { get; set; }

        public DateTime? DecreeDate { get; set; }

        public int SerialNumber { get; set; }

        public int? SedId { get; set; }

        public string Comment { get; set; }
        public int? FileStorageId { get; set; }

        [ForeignKey("MainDecreeId")]
        public virtual PracticeDecree MainDecree { get; set; }

        [ForeignKey("FileStorageId")]
        public  virtual FileStorage FileStorage { get; set; }

        public DateTime? DateExportToSed { get; set; }

        public string ExecutorName { get; set; }

        public string ExecutorPhone { get; set; }

        public string ExecutorEmail { get; set; }

        public string ROPInitials { get; set; }

        [NotMapped]
        public string StatusName
        {
            get
            {
                return PracticeDecree.DecreeStatusNames[Status];
            }
        }
    }

    [Table("PracticeChangedDecreeReasons")]
    public class PracticeChangedDecreeReason
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Reason { get; set; }
    }

    [Table("PracticeChangedDecreeStudents")]
    public class PracticeChangedDecreeStudent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string StudentId { get; set; }

        public int ChangedDecreeId { get; set; }

        /// <summary>
        /// Дата восстановления
        /// </summary>
        public DateTime? RecoveryDate { get; set; }

        public int? ReasonId { get; set; }

        [ForeignKey("ChangedDecreeId")]
        public virtual PracticeChangedDecree ChangedDecree { get; set; }

        [ForeignKey("ReasonId")]
        public virtual PracticeChangedDecreeReason Reason { get; set; }
    }

     [Table("LettersOfAttorney")]
     public class LettersOfAttorney
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  
        
        [MaxLength(20)]
        public string Number { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

     [Table("PracticeAgreements")]
     public class PracticeAgreement
     {
        [Key, ForeignKey("FileStorage")] 
        public int FileStorageId { get; set; }
        public int Year { get; set;}

        public FileStorage FileStorage { get; set; }
     }

    public class Project
    {
        [Key, MaxLength(127), ForeignKey("Module")]
        public string ModuleId { get; set; }

        public virtual Module Module { get; set; }

        public int ModuleTechId { get; set; }

        [DisplayName("Форма освоения Проекта")]
        public virtual ModuleTech Tech { get; set; }

        [DisplayName("Отображать в личном кабинете студента")]
        public bool ShowInLC { get; set; }

        [DisplayName("Без выбора")]
        public bool WithoutPriorities { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }
        public string Target { get; set; }

        /// <summary>
        /// Id из системы с проектами
        /// </summary>
        public int? EmployersId { get; set; }

        public int? ContractId { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }

        public virtual List<ProjectPeriod> Periods { get; set; }

        public virtual ICollection<ProjectDiscipline> Disciplines { get; set; }

        public virtual ICollection<ProjectCompetence> Competences { get; set; }

        public virtual ICollection<ProjectRole> Roles { get; set; }
    }

    public class ProjectAutoMoveReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string FileName { get; set; }

        public byte[] Content { get; set; }
    }

    public class ProjectPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Project")]
        public string ProjectId { get; set; }

        [DisplayName("Год")]
        public int Year { get; set; }

        [ForeignKey("Semester")]
        public int SemesterId { get; set; }

        public DateTime? SelectionBegin { get; set; }
        public DateTime? SelectionDeadline { get; set; }

        public int? Course { get; set; }

        public virtual Project Project { get; set; }

        [DisplayName("Семестр")]
        public virtual Semester Semester { get; set; }

    }

    public class ProjectDiscipline
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Project")]
        public string ProjectId { get; set; }

        public virtual Project Project { get; set; }

        [ForeignKey("Discipline")]
        public string DisciplineUid { get; set; }

        public virtual Discipline Discipline { get; set; }

        public virtual IList<ProjectDisciplineTmer> Tmers { get; set; }
    }

    public class ProjectDisciplineTmer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Discipline")]
        public int ProjectDisciplineId { get; set; }

        public virtual ProjectDiscipline Discipline { get; set; }

        [ForeignKey("Tmer")]
        public string TmerId { get; set; }

        [DisplayName("Нагрузка")]
        public virtual Tmer Tmer { get; set; }

        [DisplayName("Периоды")]
        public virtual IList<ProjectDisciplineTmerPeriod> Periods { get; set; }
    }

    public class ProjectDisciplineTmerPeriod : IDisciplineTmerPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tmer")]
        public int ProjectDisciplineTmerId { get; set; }

        public virtual ProjectDisciplineTmer Tmer { get; set; }

        [ForeignKey("Period")]
        public int ProjectPeriodId { get; set; }

        public virtual ProjectPeriod Period { get; set; }

        public virtual ICollection<Division> Divisions { get; set; }
        public virtual ICollection<ProjectSubgroupCount> ProjectSubgroupCounts { get; set; }

        public string GetDivisionsStr()
        {
            return string.Join(", ", Divisions.Select(d => $"{d.ParentShortName()}{d.typeTitle} {d.title}"));
        }

        [DisplayName("Распределение")]
        public string Distribution { get; set; }

        public int[] ExtractDistribution()
        {
            CleanDistribution();
            if (string.IsNullOrWhiteSpace(Distribution))
                return new[] { 1 };
            var array = Distribution.Split(',').Select(CleanDistributionToken).ToArray();
            if (array.Length == 0)
                return new[] { 1 };
            return array;
        }

        private static int CleanDistributionToken(string arg)
        {
            int res;
            int.TryParse(arg, out res);
            if (res < 1)
                res = 1;
            return res;
        }

        public void CleanDistribution()
        {
            if (string.IsNullOrWhiteSpace(Distribution) || (Tmer.Tmer.kmer.ToLower() != "tlekc" && Tmer.Tmer.kmer.ToLower() != "tprak" && Tmer.Tmer.kmer.ToLower() != "tlab"))
            {
                Distribution = null;
                return;
            }
            var sb = new StringBuilder();
            bool commaIsPossible = false;
            foreach (var c in Distribution)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    commaIsPossible = true;
                }
                if (c == ',' && commaIsPossible)
                {
                    sb.Append(c);
                    commaIsPossible = false;
                }
            }
            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
            Distribution = sb.ToString();

            if (string.IsNullOrWhiteSpace(Distribution))
            {
                Distribution = null;
            }
        }
        public List<ProjectDisciplineTmerPeriodDivision> ProjectDisciplineTmerPeriodDivision { get; set; }
    }
    public class ProjectDisciplineTmerPeriodDivision
    {
        public int ProjectDisciplineTmerPeriodId { get; set; }
        public ProjectDisciplineTmerPeriod ProjectDisciplineTmerPeriods { get; set; }
        public string DivisionId { get; set; }
        public Division Divisions { get; set; }
    }


    public class ProjectCompetitionGroup : ICompetitionGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ShortName { get; set; }

        [Required]
        [Range(0, 6)]
        public int StudentCourse { get; set; }

        [Required]
        [Range(2012, Int32.MaxValue)]
        public int Year { get; set; }

        [Required]
        public int SemesterId { get; set; }
        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }
        
        public virtual ICollection<Group> Groups { get; set; }

        public virtual ICollection<ProjectProperty> ProjectProperties { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Semester.Name.ToLower()} семестр, {Year}";
        }
        public List<ProjectCompetitionGroupContents> ProjectCompetitionGroupContents { get; set; }
    }
    public class ProjectCompetitionGroupContents
    {
        public int ProjectCompetitionGroupId { get; set; }
        public ProjectCompetitionGroup ProjectCompetitionGroups { get; set; }
        public string GroupId { get; set; }
        public Group Groups { get; set; }
    }


    public class ProjectProperty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProjectCompetitionGroupId { get; set; }
        [ForeignKey("ProjectCompetitionGroupId")]
        public virtual ProjectCompetitionGroup ProjectCompetitionGroup { get; set; }

        public String ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }

    public class ProjectSubgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Индекс")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Лимит")]
        public int Limit { get; set; }

        public int InnerNumber { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        public virtual ProjectSubgroup Parent { get; set; }

        [ForeignKey("Meta")]
        public int SubgroupCountId { get; set; }

        public virtual ProjectSubgroupCount Meta { get; set; }

        public double? ExpectedChildCount { get; set; }

        public virtual ICollection<ProjectSubgroupMembership> Students { get; set; }

        public virtual ICollection<ProjectSubgroup> Subgroups { get; set; }

        [ForeignKey("Teacher")]
        public string TeacherId { get; set; }

        [DisplayName("Преподаватель")]
        public virtual Teacher Teacher { get; set; }
    }

    [Table("ProjectSubgroupCounts")]
    public class ProjectSubgroupCount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProjectDisciplineTmerPeriodId { get; set; }
        [ForeignKey("ProjectDisciplineTmerPeriodId")]
        public virtual ProjectDisciplineTmerPeriod ProjectDisciplineTmerPeriod { get; set; }

        public int CompetitionGroupId { get; set; }
        [ForeignKey("CompetitionGroupId")]
        public ProjectCompetitionGroup CompetitionGroup { get; set; }

        [DisplayName("Колличество подгрупп")]
        public int GroupCount { get; set; }

        public virtual ICollection<ProjectSubgroup> Subgroups { get; set; }
    }

    public class ProjectSubgroupMembership
    {
        [Key, Column(Order = 1)]
        public string studentId { get; set; }

        [ForeignKey("studentId")]
        public Student Student { get; set; }

        [Key, Column(Order = 0)]
        public int SubgroupId { get; set; }

        [ForeignKey("SubgroupId")]
        public virtual ProjectSubgroup Subgroup { get; set; }
    }

    public class ProjectAdmission
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }

        [Key, Column(Order = 1)]
        [Index("IX_ProjectAdmission_Count", 1)]
        [Index("IX_ProjectAdmission_ProjectCompetitionGroupId", 0)]
        public int ProjectCompetitionGroupId { get; set; }

        [Key, Column(Order = 2)]
        [Index("IX_ProjectAdmission_Count", 0)]
        public string ProjectId { get; set; }

        public int? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual ProjectRole Role {get;set;}

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("ProjectCompetitionGroupId")]
        public virtual ProjectCompetitionGroup ProjectCompetitionGroup { get; set; }

        public bool Published { get; set; }

        [DisplayName("Статус")]
        [Index("IX_ProjectAdmission_Count", 2)]
        public AdmissionStatus Status { get; set; }
    }

    [Table("ProjectStudentSelectionPriorities")]
    public class ProjectStudentSelectionPriority
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }

        [Key, Column(Order = 1)]
        public int competitionGroupId { get; set; }

        [ForeignKey("competitionGroupId")]
        public virtual ProjectCompetitionGroup CompetitionGroup { get; set; }

        [Key, Column(Order = 2)]
        public string projectId { get; set; }

        [ForeignKey("projectId")]
        public virtual Project Project { get; set; }

        public int? priority { get; set; }

        public int? roleId { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("roleId")]
        public virtual ProjectRole Role { get; set; }

        public DateTime modified { get; set; }

        public string Comment { get; set; }

    }

    public enum ProjectUserType
    {
        [Display(Name = "Руководитель образовательной программы")]
        ROP = 0,
        [Display(Name = "Куратор")]
        Curator = 1
    }

    [Table("ProjectUsers")]
    public class ProjectUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int? ProjectPropertyId { get; set; }
        [ForeignKey("ProjectPropertyId")]
        public virtual ProjectProperty ProjectProperty { get; set; }
        
        public string ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [Required]
        public string TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }

        public ProjectUserType Type { get; set; }

        public bool IsChief { get; set; }
    }

    [Table("ModuleRelations")]
    public class ModuleRelation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string MainModuleUUID { get; set; }

        public string PairedModuleUUID { get; set; }

        [ForeignKey("MainModuleUUID")]
        public virtual Module MainModule { get; set; }

        [ForeignKey("PairedModuleUUID")]
        public virtual Module PairedModule { get; set; }

        /// <summary>
        /// Номер связи в рамках одного учебного плана
        /// </summary>
        public int Group { get; set; }

        public int eduplanNumber { get; set; }
        public int versionNumber { get; set; }
    }

    [Table("ProjectROPProfiles")]
    public class ProjectROPProfile
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProjectUserId { get; set; }

        [ForeignKey("ProjectUserId")]
        public virtual ProjectUser ProjectUser { get; set; }

        public string ProfileId { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }
    }

    [Table("ProjectCompetences")]
    public class ProjectCompetence
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public int CompetenceId { get; set; }
        
        [ForeignKey("CompetenceId")]
        public virtual Competence Competence { get; set; }

        public string ProfileId { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }
    }

    [Table("ModuleAgreements")]
    public class ModuleAgreement
    {
        public string UniId { get; set; }
        public string ModuleUUID { get; set; }
        public string DisciplineUUID { get; set; }
        public string CourseTitle { get; set; }
        public string CourseType { get; set; }
        public int EduYear { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
        public int SemesterId { get; set; }
        public string URFUInfoURL { get; set; }
        public string CourseURL { get; set; }
        public string UniversityTitle { get; set; }
        public string UniversityShortTitle { get; set; }

        [ForeignKey("ModuleUUID")]
        public virtual Module Module { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }
    }

    [Table("ProjectRoles")]
    public class ProjectRole
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ProjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int EmployersId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }

    [Table("BasicCharacteristicOP")]
    public class BasicCharacteristicOP
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VersionedDocumentId { get; set; }

        [ForeignKey(nameof(VersionedDocumentId))]
        public virtual VersionedDocument VersionedDocument { get; set; }

        public int InfoId { get; set; }

        [ForeignKey(nameof(InfoId))]
        public virtual BasicCharacteristicOPInfo Info { get; set; }

        [ForeignKey("UpopStatusId")]
        public virtual UPOPStatus Status { get; set; }
        public int? UpopStatusId { get; set; }

        public DateTime StatusChangeTime { get; set; }

        [Required]
        public int Version { get; set; }

        public int? BasedOnId { get; set; }

        public string Comment { get; set; }

        public int? FileStorageDocxId { get; set; }

        [ForeignKey(nameof(FileStorageDocxId))]
        public virtual FileStorage FileStorageDocx { get; set; }

        public int? FileStorageZipId { get; set; }

        [ForeignKey(nameof(FileStorageZipId))]
        public virtual FileStorage FileStorageZip { get; set; }

        [ForeignKey(nameof(BasedOnId))]
        public virtual BasicCharacteristicOP BasedOn { get; set; }

        public virtual ICollection<ModuleWorkingProgram> ModuleWorkingPrograms { get; set; }

        public virtual ICollection<CompetencePassport> CompetencePassports { get; set; }
        public List<BasicCharacteristicOPMapping> BasicCharacteristicOPMapping { get; set; }
    }
    public class BasicCharacteristicOPMapping
    {
        public int ModuleWorkingProgramId { get; set; }
        public ModuleWorkingProgram ModuleWorkingPrograms { get; set; }
        public int BasicCharacteristicOPId { get; set; }
        public BasicCharacteristicOP BasicCharacteristicOPs { get; set; }
    }

    [Table("TrainingDurations")]
    public class TrainingDuration
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DivisionUuid { get; set; }

        [ForeignKey(nameof(DivisionUuid))]
        public virtual Division Division { get; set; }

        public string Qualification { get; set; }

        public string DirectionUid { get; set; }

        [ForeignKey(nameof(DirectionUid))]
        public virtual Direction Direction { get; set; }

        public string FamilirizationType { get; set; }
        public Decimal Duration { get; set; }

        public decimal? DurationSPO { get; set; }
        public decimal? DurationSPOUnsuitableProfile { get; set; }
        public decimal? DurationVPO { get; set; }
        public decimal? DurationVPOUnsuitableProfile { get; set; }
    }

    [Table("AreaEducation")]
    public class AreaEducation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public virtual ICollection<AreaEducationOrder> Orders { get; set; }
        public virtual ICollection<Competence> Competences { get; set; }
    }

    [Table("AreaEducationOrders")]
    public class AreaEducationOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AreaEducationId { get; set; }

        [ForeignKey(nameof(AreaEducationId))]
        public virtual AreaEducation AreaEducation { get; set; }

        public string Number { get; set; }
        public DateTime? Date { get; set; }

        public string QualificationName { get; set; }
        
        [ForeignKey(nameof(QualificationName))]
        public Qualification Qualification { get; set; }
    }

    [Table("MUPs")]
    public class MUP
    {
        [Key, MaxLength(127), ForeignKey("Module")]
        public string ModuleId { get; set; }

        public virtual Module Module { get; set; }

        public int ModuleTechId { get; set; }

        [DisplayName("Форма освоения модуля")]
        public virtual ModuleTech Tech { get; set; }

        [DisplayName("Отображать в личном кабинете студента")]
        public bool ShowInLC { get; set; }

        public bool Removed { get; set; }

        public virtual List<MUPPeriod> Periods { get; set; }

        public virtual ICollection<MUPDiscipline> Disciplines { get; set; }
    }

    public class MUPPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MUP")]
        public string MUPId { get; set; }

        [DisplayName("Год")]
        public int Year { get; set; }

        [ForeignKey("Semester")]
        public int SemesterId { get; set; }

        public DateTime? SelectionDeadline { get; set; }

        public virtual MUP MUP { get; set; }

        [DisplayName("Семестр")]
        public virtual Semester Semester { get; set; }

        [DisplayName("Курс")]
        public int? Course { get; set; }

        public bool Removed { get; set; }

        public virtual ICollection<MUPDisciplineTmerPeriod> MUPDisciplineTmerPeriods { get; set; }
    }

    public class MUPDiscipline
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MUP")]
        public string MUPId { get; set; }

        public virtual MUP MUP { get; set; }

        [ForeignKey("Discipline")]
        public string DisciplineUid { get; set; }

        public virtual Discipline Discipline { get; set; }

        public virtual IList<MUPDisciplineTmer> Tmers { get; set; }
    }

    public class MUPDisciplineTmer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Discipline")]
        public int MUPDisciplineId { get; set; }

        public virtual MUPDiscipline Discipline { get; set; }

        [ForeignKey("Tmer")]
        public string TmerId { get; set; }

        [DisplayName("Нагрузка")]
        public virtual Tmer Tmer { get; set; }

        [DisplayName("Периоды")]
        public virtual IList<MUPDisciplineTmerPeriod> Periods { get; set; }

        public bool Removed { get; set; }
    }

    public class MUPDisciplineTmerPeriod : IDisciplineTmerPeriod
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tmer")]
        public int MUPDisciplineTmerId { get; set; }

        public virtual MUPDisciplineTmer Tmer { get; set; }

        public bool Removed { get; set; }

        [ForeignKey("Period")]
        public int MUPPeriodId { get; set; }

        public virtual MUPPeriod Period { get; set; }

        public virtual ICollection<Division> Divisions { get; set; }
        public virtual ICollection<MUPSubgroupCount> MUPSubgroupCounts { get; set; }

        public string GetDivisionsStr()
        {
            return string.Join(", ", Divisions.Select(d => $"{d.ParentShortName()}{d.typeTitle} {d.title}"));
        }
        
        [DisplayName("Распределение")]
        public string Distribution { get; set; }

        public void CleanDistribution()
        {
            if (string.IsNullOrWhiteSpace(Distribution) || (Tmer.Tmer.kmer.ToLower() != "tlekc" && Tmer.Tmer.kmer.ToLower() != "tprak" && Tmer.Tmer.kmer.ToLower() != "tlab"))
            {
                Distribution = null;
                return;
            }
            var sb = new StringBuilder();
            bool commaIsPossible = false;
            foreach (var c in Distribution)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    commaIsPossible = true;
                }
                if (c == ',' && commaIsPossible)
                {
                    sb.Append(c);
                    commaIsPossible = false;
                }
            }
            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
            Distribution = sb.ToString();

            if (string.IsNullOrWhiteSpace(Distribution))
            {
                Distribution = null;
            }
        }

        public int[] ExtractDistribution()
        {
            CleanDistribution();
            if (string.IsNullOrWhiteSpace(Distribution))
                return new[] { 1 };
            var array = Distribution.Split(',').Select(CleanDistributionToken).ToArray();
            if (array.Length == 0)
                return new[] { 1 };
            return array;
        }

        private static int CleanDistributionToken(string arg)
        {
            int res;
            int.TryParse(arg, out res);
            if (res < 1)
                res = 1;
            return res;
        }
        public List<MUPDisciplineTmerPeriodDivision> MUPDisciplineTmerPeriodDivision { get; set; }
    }
    public class MUPDisciplineTmerPeriodDivision
    {
        public int MUPDisciplineTmerPeriodId { get; set; }
        public MUPDisciplineTmerPeriod MUPDisciplineTmerPeriods { get; set; }
        public string DivisionId { get; set; }
        public Division Divisions { get; set; }
    }


    [Table("MUPSubgroupCounts")]
    public class MUPSubgroupCount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MUPDisciplineTmerPeriodId { get; set; }
        [ForeignKey("MUPDisciplineTmerPeriodId")]
        public MUPDisciplineTmerPeriod MUPDisciplineTmerPeriod { get; set; }

        public int CompetitionGroupId { get; set; }
        [ForeignKey("CompetitionGroupId")]
        public MUPCompetitionGroup CompetitionGroup { get; set; }
        [DisplayName("Колличество подгрупп")]
        public int GroupCount { get; set; }

        public virtual ICollection<MUPSubgroup> Subgroups { get; set; }
    }

    public class MUPSubgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Индекс")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Лимит")]
        public int Limit { get; set; }

        public int InnerNumber { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        public virtual MUPSubgroup Parent { get; set; }

        [ForeignKey("Meta")]
        public int SubgroupCountId { get; set; }

        public virtual MUPSubgroupCount Meta { get; set; }

        public double? ExpectedChildCount { get; set; }

        public virtual ICollection<MUPSubgroupMembership> Students { get; set; }

        public virtual ICollection<MUPSubgroup> Subgroups { get; set; }

        [ForeignKey("Teacher")]
        public string TeacherId { get; set; }

        [DisplayName("Преподаватель")]
        public virtual Teacher Teacher { get; set; }
        [DisplayName("Комментарий")]
        public String Description { get; set; }

        public virtual ICollection<MUPSubgroupTeacher> Teachers { get; set; }

        public string MUPModeusTeamId { get; set; }

        [ForeignKey(nameof(MUPModeusTeamId))]
        public virtual MUPModeusTeam MUPModeusTeam { get; set; }

        public bool Removed { get; set; }
    }

    public class MUPSubgroupMembership
    {
        [Key, Column(Order = 1)]
        public string studentId { get; set; }

        [ForeignKey("studentId")]
        public Student Student { get; set; }

        [Key, Column(Order = 0)]
        public int SubgroupId { get; set; }

        [ForeignKey("SubgroupId")]
        public virtual MUPSubgroup Subgroup { get; set; }
    }

    public class MUPCompetitionGroup : ICompetitionGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        [Range(0, 6)]
        public int StudentCourse { get; set; }
        [Required]
        [Range(2012, Int32.MaxValue)]
        public int Year { get; set; }
        [Required]
        public int SemesterId { get; set; }
        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; }
        public virtual ICollection<MUPProperty> MUPProperties { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<MUPAdmission> MUPAdmissions { get; set; }
        public override string ToString()
        {
            return $"{Name}, {Semester.Name.ToLower()} семестр, {Year}";
        }
        public List<MUPCompetitionGroupContents> MUPCompetitionGroupContents { get; set; }
    }
    public class MUPCompetitionGroupContents
    {
        public string GroupId { get; set; }
        public Group Groups { get; set; }
        public int MUPCompetitionGroupId { get; set; }
        public MUPCompetitionGroup MUPCompetitionGroups { get; set; }
    }


    public class MUPProperty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MUPCompetitionGroupId { get; set; }
        [ForeignKey("MUPCompetitionGroupId")]
        public virtual MUPCompetitionGroup MUPCompetitionGroup { get; set; }
        public String MUPId { get; set; }
        [ForeignKey("MUPId")]
        public virtual MUP MUP { get; set; }
        public int Limit { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
        public List<MUPTeachers> MUPTeachers { get; set; }
    }
    public class MUPTeachers
    {
        public int MUPPropertyId { get; set; }
        public MUPProperty MUPProperties { get; set; }
        public string TeacherId { get; set; }
        public Teacher Teachers { get; set; }
    }


    public class MUPAdmission
    {
        [Key, MaxLength(127), Column(Order = 0)]
        public string studentId { get; set; }
        [Key, Column(Order = 1)]
        [Index("IX_MUPAdmission_Count", 1)]
        [Index("IX_MUPAdmission_MUPCompetitionGroupId", 0)]
        public int MUPCompetitionGroupId { get; set; }

        [Key, Column(Order = 2)]
        [Index("IX_MUPAdmission_Count", 0)]
        public string MUPId { get; set; }

        [ForeignKey("MUPId")]
        public virtual MUP MUP { get; set; }

        [ForeignKey("studentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("MUPCompetitionGroupId")]
        public virtual MUPCompetitionGroup MUPCompetitionGroup { get; set; }

        public bool Published { get; set; }

        [DisplayName("Статус")]
        [Index("IX_MUPAdmission_Count", 2)]
        public AdmissionStatus Status { get; set; }
    }

    public class MUPModeus
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int? Course { get; set; }
        public int? SemesterId { get; set; }

        [ForeignKey(nameof(SemesterId))]
        public virtual Semester Semester { get; set; }

        public int? Units { get; set; }

        public string Directions { get; set; }

        public bool Removed { get; set; }

        public virtual ICollection<MUPModeusTeam> Teams { get; set; }

        public virtual ICollection<MUPModeusDirections> ModeusDirections { get; set; }

        public virtual ICollection<MUPModeusRealization> Realizations { get; set; }

        public virtual ICollection<MUPDisciplineConnection> Connections { get; set; }
    }

    public class MUPModeusDirections
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string MUPModeusId { get; set; }
        public string DirectionId { get; set; }

        [ForeignKey(nameof(MUPModeusId))]
        public virtual MUPModeus MUPModeus { get; set; }

        [ForeignKey(nameof(DirectionId))]
        public virtual Direction Direction { get; set; }
    }

    [Table("MUPModeusRealizations")]
    public class MUPModeusRealization
    {
        [Key]
        public string Id { get; set; }

        public string CourseUnitId { get; set; }

        [ForeignKey(nameof(CourseUnitId))]
        public virtual MUPModeus MUPModeus { get; set; }

        public bool Deleted { get; set; }

        public bool Removed { get; set; }

        public string State { get; set; }
        public int? Year { get; set; }
        public int? SemesterId { get; set; }

        [ForeignKey(nameof(SemesterId))]
        public virtual Semester Semester { get; set; }
    }

    [Table("MUPModeusTeams")]
    public class MUPModeusTeam
    {
        [Key]
        public string Id { get; set; }
        public string MUPModeusRealizationId { get; set; }

        [ForeignKey(nameof(MUPModeusRealizationId))]
        public virtual MUPModeusRealization MUPRealization { get; set; }

        public string MUPModeusId { get; set; }

        [ForeignKey(nameof(MUPModeusId))]
        public virtual MUPModeus MUPModeus { get; set; }

        public int? Year { get; set; }
        public int? SemesterId { get; set; }

        [ForeignKey(nameof(SemesterId))]
        public virtual Semester Semester { get; set; }

        public string Name { get; set; }

        public int? Limit { get; set; }
        public int? StudentsCount { get; set; }

        public bool Deleted { get; set; }

        public bool Removed { get; set; }

        public string EventTypes { get; set; }
        public string Kmers { get; set; }

        public virtual ICollection<MUPModeusTeacher> Teachers { get; set; }
        public virtual ICollection<MUPModeusTeamStudent> Students { get; set; }

        public virtual ICollection<MUPSubgroup> MUPSubgroups { get; set; }
    }

    [Table("MUPModeusTeachers")]
    public class MUPModeusTeacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string MUPModeusTeamId { get; set; }

        [ForeignKey(nameof(MUPModeusTeamId))]
        public virtual MUPModeusTeam MUPModeusTeam { get; set; }

        public string TmerId { get; set; }

        [ForeignKey(nameof(TmerId))]
        public virtual Tmer Tmer { get; set; }

        public bool Deleted { get; set; }

        public bool Removed { get; set; }

        public string TeacherId { get; set; }
        public string PersonId { get; set; }

        public string EventTypes { get; set; } 
    }

    [Table("MUPModeusTeamStudents")]
    public class MUPModeusTeamStudent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string MUPModeusTeamId { get; set; }

        [ForeignKey(nameof(MUPModeusTeamId))]
        public virtual MUPModeusTeam MUPModeusTeam { get; set; }

        public string StudentId { get; set; }
        public string PersonId { get; set; }
        public bool Deleted { get; set; }

        public bool Removed { get; set; }
    }

    [Table("MUPDisciplineConnections")]
    public class MUPDisciplineConnection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string MUPModeusId { get; set; }

        public string ModuleId { get; set; }

        public string DisciplineId { get; set; }

        public string ModuleMUPId { get; set; }

        public string DisciplineMUPId { get; set; }

        [ForeignKey(nameof(MUPModeusId))]
        public virtual MUPModeus MUPModeus { get; set; }

        [ForeignKey(nameof(ModuleId))]
        public virtual Module Module { get; set; }

        [ForeignKey(nameof(ModuleMUPId))]
        public virtual Module ModuleMUP { get; set; }

        [ForeignKey(nameof(DisciplineId))]
        public virtual Discipline Discipline { get; set; }

        [ForeignKey(nameof(DisciplineMUPId))]
        public virtual Discipline DisciplineMUP { get; set; }
    }

    [Table("MUPSubgroupTeachers")]
    public class MUPSubgroupTeacher
    {
        public int MUPSubgroupId { get; set; }

        [ForeignKey(nameof(MUPSubgroupId))]
        public virtual MUPSubgroup MUPSubgroup { get; set; } 

        public string TeacherId { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; }
    }

    [Table("CompetenceGroups")]
    public class CompetenceGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
    }

    [Table("BasicCharacteristicOPInfo")]
    public class BasicCharacteristicOPInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ProfileId { get; set; }

        [ForeignKey(nameof(ProfileId))]
        public virtual Profile Profile { get; set; }

        public int Year { get; set; }

        public virtual ICollection<BasicCharacteristicOP> BasicCharacteristicOPs { get; set; }
    }

    [Table("FileStorage")]
    public class FileStorage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Ip { get; set; }
        public string HttpUser { get; set; }
        
        /// <summary>
        /// Имя, под которым пользователь загрузил файл
        /// </summary>
        public string FileNameForUser { get; set; }

        /// <summary>
        /// Имя, под которым файл хранится на диске
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Относительная ссылка. Недостающая часть хранится в конфиге (поле FileFolder)
        /// </summary>
        public string Path { get; set; }
        public string Comment { get; set; }
    }

    [Table("ProfActivityArea")]
    public class ProfActivityArea
    {
        [Key]
        [MaxLength(10)]
        [Required]
        public string Code { get; set; }

        public string Title { get; set; }
    }

    [Table("ProfActivityKind")]
    public class ProfActivityKind
    {
        [Key]
        [MaxLength(10)]
        [Required]
        public string Code { get; set; }

        public string Title { get; set; }
    }

    [Table("ProfStandards")]
    public class ProfStandard
    {
        [Key]
        [MaxLength(20)]
        public string Code { get; set; }

        public string Title { get; set; }

        public string ProfActivityAreaCode { get; set; }

        [ForeignKey(nameof(ProfActivityAreaCode))]
        public virtual ProfActivityArea ProfActivityArea { get; set; }
        
        public string ProfActivityKindCode { get; set; }

        [ForeignKey(nameof(ProfActivityKindCode))]
        public virtual ProfActivityKind ProfActivityKind { get; set; }

        public virtual ICollection<ProfOrder> ProfOrders { get; set; }
    }

    [Table("ProfOrders")]
    public class ProfOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ProfStandardCode { get; set; }

        [ForeignKey(nameof(ProfStandardCode))]
        public virtual ProfStandard ProfStandard { get; set; }

        public string NumberOfMintrud { get; set; }

        public DateTime? DateOfMintrud { get; set; }

        public string RegNumberOfMinust { get; set; }

        public DateTime? RegNumberDateOfMinust { get; set; }

        public string Status { get; set; }

        public virtual ICollection<ProfOrderConnection> OrderChanges { get; set; }
    }

    [Table("ProfOrderChanges")]
    public class ProfOrderChange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string NumberOfMintrud { get; set; }

        public DateTime? DateOfMintrud { get; set; }

        public string RegNumberOfMinust { get; set; }

        public DateTime? RegNumberDateOfMinust { get; set; }

        public virtual ICollection<ProfOrderConnection> BaseOrders { get; set; }
    }

    [Table("ProfOrderConnections")]
    public class ProfOrderConnection
    {
        [Key]
        public int ProfOrderId { get; set; }

        [Key]
        public int ProfOrderChangeId { get; set; }

        public string Status { get; set; }

        [ForeignKey(nameof(ProfOrderId))]
        public virtual ProfOrder ProfOrder { get; set; }

        [ForeignKey(nameof(ProfOrderChangeId))]
        public virtual ProfOrderChange ProfOrderChange { get; set; }
    }

    public class VariantsUni
    {
        [Key]
        public string TrajectoryUuid { get; set; }

        public int? VariantId { get; set; }

        public string ProfileId { get; set; }

        public string DocumentName { get; set; }

        [ForeignKey(nameof(VariantId))]
        public virtual Variant Variant { get; set; }

        [ForeignKey(nameof(ProfileId))]
        public virtual Profile Profile { get; set; }
    }

    [Table("DirectionOrders")]
    public class DirectionOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DirectionId { get; set; }

        [ForeignKey(nameof(DirectionId))]
        public virtual Direction Direction { get; set; }

        public string Number { get; set; }
        public DateTime? Date { get; set; }
    }

    [Table("EduResultTypes")]
    public class EduResultType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }
    }

    [Table("EduResultKinds")]
    public class EduResultKind
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }
    }

    [Table("EduResults2")]
    public class EduResult2
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// ShortName из типа + "-" + SerialNumber. 
        /// Например, У-9
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Порядковый номер внутри компетенции и типа. 
        /// </summary>
        public int SerialNumber { get; set; }

        public string Description { get; set; }

        public int EduResultTypeId { get; set; }

        [ForeignKey(nameof(EduResultTypeId))]
        public virtual EduResultType EduResultType { get; set; }

        public int EduResultKindId { get; set; }

        [ForeignKey(nameof(EduResultKindId))]
        public virtual EduResultKind EduResultKind { get; set; }

        public int CompetenceId { get; set; }

        [ForeignKey(nameof(CompetenceId))]
        public virtual Competence Competence { get; set; }

    }

    [Table("CompetencePassports")]
    public class CompetencePassport
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VersionedDocumentId { get; set; }

        [ForeignKey(nameof(VersionedDocumentId))]
        public virtual VersionedDocument VersionedDocument { get; set; }

        public int BasicCharacteristicOPId { get; set; }

        [ForeignKey(nameof(BasicCharacteristicOPId))]
        public virtual BasicCharacteristicOP BasicCharacteristicOP { get; set; }

        public int? UpopStatusId { get; set; }

        [ForeignKey("UpopStatusId")]
        public virtual UPOPStatus Status { get; set; }

        public DateTime StatusChangeTime { get; set; }

        public string Comment { get; set; }

        public int Year { get; set; }

        [Required]
        public int Version { get; set; }

        public int? BasedOnId { get; set; }

        public int? FileStorageDocxId { get; set; }

        [ForeignKey(nameof(FileStorageDocxId))]
        public virtual FileStorage FileStorageDocx { get; set; }

        [ForeignKey(nameof(BasedOnId))]
        public virtual CompetencePassport BasedOn { get; set; }
    }

    [Table("RatingCoefficients")]
    public class RatingCoefficient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ModuleType { get; set; }
        
        public string ModuleId { get; set; }        

        public string Level { get; set; }

        public decimal Coefficient { get; set; }

        public int Year { get; set; }
    }

    [Table("ModuleAnnotations")]
    public class ModuleAnnotation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VersionedDocumentId { get; set; }

        [ForeignKey(nameof(VersionedDocumentId))]
        public virtual VersionedDocument VersionedDocument { get; set; }

        public int BasicCharacteristicOPId { get; set; }

        [ForeignKey(nameof(BasicCharacteristicOPId))]
        public virtual BasicCharacteristicOP BasicCharacteristicOP { get; set; }

        public int PlanNumber { get; set; }
        public int PlanVersionNumber { get; set; }

        public int? UpopStatusId { get; set; }

        [ForeignKey("UpopStatusId")]
        public virtual UPOPStatus Status { get; set; }

        public DateTime StatusChangeTime { get; set; }

        public string Comment { get; set; }
        

        public int? FileStorageDocxId { get; set; }

        [ForeignKey(nameof(FileStorageDocxId))]
        public virtual FileStorage FileStorageDocx { get; set; }

    }


    public class PracticeReview
    {
        [Key, ForeignKey(nameof(Practice))]
        public int PracticeId { get; set; }

        public Practice Practice { get; set; }

        /// <summary>
        /// Мероприятия
        /// </summary>
        public string Events { get; set; }

        /// <summary>
        /// Характеристика
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Был ли трудоустроен
        /// </summary>
        public bool Employment { get; set; }

        /// <summary>
        /// Предложили ли следующую практику
        /// </summary>
        public bool FuturePractice { get; set; }

        /// <summary>
        /// Предложили ли трудоустройство
        /// </summary>
        public bool FutureEmployment { get; set; }

        /// <summary>
        /// Предложения/замечания
        /// </summary>
        public string Suggestions { get; set; }

        /// <summary>
        /// Оценка
        /// </summary>
        public int Score { get; set; }
    }

    public class BasicCharacteristicOPRatifyData
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int Year { get; set; }
        public string RatifyingPersonPost { get; set; }
        public string RatifyingPersonName { get; set; }
        public string RatifyingSubdivision { get; set; }
    }
}


