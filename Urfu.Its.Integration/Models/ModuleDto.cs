using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Urfu.Its.Integration.Models
{
    public class ModuleDto
    {
        public string uuid { get; set; }
        public string title { get; set; }
        public int? number { get; set; }
        public string shortTitle { get; set; }
        public string coordinator { get; set; }
        public string type { get; set; }
        public string competence { get; set; }
        public decimal testUnits { get; set; }
        public decimal priority { get; set; }
        public string state { get; set; }
        [JsonConverter(typeof(RuDateConverter))]
        public DateTime? approvedDate { get; set; }
        public string comment { get; set; }
        public string file { get; set; }
        public string[] specialities { get; set; }
        public DisciplineDto[] disciplines { get; set; }
        public bool includeInCore { get; set; }
        public string typeProject { get; set; }
        public string annotation { get; set; }
    }

    public class DateConverter : IsoDateTimeConverter
    {
        public DateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

    public class RuDateConverter:IsoDateTimeConverter
    {
        public RuDateConverter()
        {
            DateTimeFormat = "dd.MM.yyyy";
        }
    }

    public class DisciplineDto
    {
        // UUID epmsde{дисциплина};{раздел};{группа дисциплин}
        public string uid { get; set; }
        // UUID дисциплины
        public string discipline { get; set; }
        // Название дисциплины
        public string title { get; set; }
        // Название раздела
        public string section { get; set; }
        // Кол-во зачетных единиц
        public decimal testUnits { get; set; }
        // Файл
        public string file { get; set; }
        // Номер
        public int? number { get; set; }
    }

    public class PlanDto
    {
        // UUID учебного плана
        public string eduplanUUID { get; set; }
        // Подразделение
        public string faculty { get; set; }
        // Номер учебного плана
        public int? eduplanNumber { get; set; }
        // Номер версии учебного плана
        public int versionNumber { get; set; }
        // Название версии
        public string versionTitle { get; set; }
        // Версия в архиве
        public bool versionActive { get; set; }
        // UUID версии плана
        public string versionUUID { get; set; }
        // UUID плана на дисциплину
        public string disciplineUUID { get; set; }
        // UUID плана на дополнительное мероприятие
        public string additionalUUID { get; set; }
        // UUID справочной дисциплины
        public string catalogDisciplineUUID { get; set; }
        // Название дисциплины
        public string disciplineTitle { get; set; }
        // UUID учебного модуля
        public string moduleUUID { get; set; }
        // Состояние версии плана
        public string versionStatus { get; set; }
        // Контольные мероприятия -> семестры проведения
        public List<Dictionary<string,List<int>>> controls { get; set; }
        // Виды учебных нагрузок (лекции, практики и тд)
        public List<string> loads { get; set; }
        // Семестры для проведения учебной нагрузки
        public List<int> terms { get; set; }
        
        // Форма освоения
        public string familirizationType { get; set; }
        // Технология освоения
        public string familirizationTech { get; set; }
        // Условие освоения
        public string familirizationCondition { get; set; }
        // Уровень подготовки
        public string qualification { get; set; }
        // распределение зачетных единиц по семестрам
        public string additionalType { get; set; }
        public Dictionary<string, int> testUnitsByTerm { get; set; }
        public Dictionary<string, int> termsCount { get; set; }

        /// <summary>
        /// Образовательная программа
        /// </summary>
        public string learnProgramUUID { get; set; }
        public string learnProgramTitle { get; set; }
        public string learnProgramCode { get; set; }
        public int? additionalWeeks { get; set; }

        public string moduleGroupType { get; set; }

        public string moduleSubgroupType { get; set; }
    }

    public class PlanAdditionalDto
    {
        // UUID дисциплины
        public string id { get; set; }
        // UUID плана
        public string plan { get; set; }
        // название плана
        public string planTitle { get; set; }
        // UUID версии учебного плана
        public string version { get; set; }
        // Название версии учебного плана
        public string versionTitle { get; set; }
        public string title { get; set; }
        // UUID родителя
        public string parent { get; set; }
        // зачетные единицы 
        public int testUnits { get; set; }
        public string controls { get; set; }
        public string terms { get; set; }
        public string okso { get; set; }
        public string speciality { get; set; }
        public string profile { get; set; }
        public string profileCode { get; set; }
        public string level { get; set; }
        public string groupID { get; set; }
        public string groupTitle { get; set; }
        public string allload { get; set; }
        public string allaudit { get; set; }
        public string self { get; set; }
        public string lecture { get; set; }
        public string practice { get; set; }
        public string labs { get; set; }
        public string contactTotal { get; set; }
        public string contactSelf { get; set; }
        public string contactControl { get; set; }
        public string contactLecture { get; set; }
        public string contactPractice { get; set; }
        public string contactLabs { get; set; }
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
        public string gosLoadInTestUnits { get; set; }
        public string ttu1 { get; set; }
        public string ttu2 { get; set; }
        public string ttu3 { get; set; }
        public string ttu4 { get; set; }
        public string ttu5 { get; set; }
        public string ttu6 { get; set; }
        public string ttu7 { get; set; }
        public string ttu8 { get; set; }
        public string ttu9 { get; set; }
        public string ttu10 { get; set; }
        public string ttu11 { get; set; }
        public string ttu12 { get; set; }


    }

    public class PlanTermsDto
    {
        public string eduplanUUID { get; set; }
        public List<PlanTermsCountDto> TermsCount { get; set; }
    }

    public class PlanTermsCountDto
    {
        public int Year { get; set; }
        public int TermsCount { get; set; }
    }

    public class PlanTermsWeeksDto
    {
        public string eduplanUUID { get; set; }
        public List<PlanTermWeekDto> WeeksCount { get; set; }
    }

    public class PlanTermWeekDto
    {
        public int Term { get; set; }
        public int WeeksCount { get; set; }
    }
}
