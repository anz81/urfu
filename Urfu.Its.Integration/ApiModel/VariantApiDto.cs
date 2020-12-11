using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable InconsistentNaming

namespace Urfu.Its.Integration.ApiModel
{

    public class ApiDtoFunctions
    {
        /// <summary>
        /// Ключ, определяющий подгруппу.
        /// 
        /// Должен указывать на отдельную подгруппу (ключевой атрибут подгруппы)
        /// для учебной нагрузки (дисциплина, семестр, группа)
        /// </summary>
        public static string ToSubgroupKey(int innerNumber, string groupId, string catalogDisciplineUuid, string kmer, int term, int year)
        {
            return innerNumber + "-" + groupId + "-" + catalogDisciplineUuid + "-" + kmer + "-" + term + "-" + year;
        }

        /// <summary>
        /// Ключ, определяющий подгруппу.
        /// 
        /// Должен указывать на отдельную подгруппу (ключевой атрибут подгруппы)
        /// для учебной нагрузки (дисциплина, семестр, группа)
        /// </summary>
        public static string ToSubgroupKey(int innerNumber, string groupId, string catalogDisciplineUuid, string kmer, int term, int year, int? course, string competitionGroupShortName)
        {
            return innerNumber + "-" + groupId + "-" + catalogDisciplineUuid + "-" + kmer + "-" + term + "-" + year + "-" + course + "-" + competitionGroupShortName;
        }
    }

    public class ProgramApiDto
    {
        public int id { get; set; }//Код программы в ИТС
        public string name { get; set; } // Название образовательной программы
        public string headfullname { get; set; }
        public List<VariantApiDto> variants { get; set; } //варианты
        public DirectionApiDto direction { get; set; } //Направление
        public int year { get; set; } // год
        public string familirizationType { get; set; } // Тип осноения
        public string familirizationTech { get; set; } // Технология освоения
        public string familirizationCondition { get; set; } //Условия освоения
        public string qualification { get; set; } // Квалификация
        public DivisionApiDto division { get; set; } // подразделение (институт)
        public DivisionApiDto department { get; set; } // депратамент
        public DivisionApiDto chair { get; set; } // подразделение (институт)
        public string state { get; set; } //статус
    }

    public class DivisionApiDto
    {
        public string uuid { get; set; } //идентификатор подразделения в ЮНИ
        public string title { get; set; } //Навазние подразделения
        public string typeTitle { get; set; } //Тип подразделения
        public string shortTitle { get; set; } //Сокращённое название подразделения

        public override bool Equals(object obj)
        {
            var dto = obj as DivisionApiDto;
            if (dto == null)
                return false;

            return dto.uuid == uuid && dto.title == title && dto.typeTitle == typeTitle && dto.shortTitle == shortTitle;
        }
    }

    public class VariantApiDto
    {
        public int id { get; set; } //Код варианта в ИТС
        public bool isBase { get; set; }//признак основного варианта
        public string variantName { get; set; } //Название варианта в ИТС
        public DirectionApiDto direction { get; set; } //Направление
        public List<VariantSelectionGroupApiDto> selectionGroups { get; set; } //Группы модулей в варианте
        public List<VariantGroupApiDto> groups { get; set; } //Группы модулей в варианте
        public List<VariantContentApiDto> modules { get; set; } // Модули
        public DateTime? createDate { get; set; } //дата создания
        public int year { get; set; } // год
        public string familirizationType { get; set; } // Тип осноения
        public string familirizationTech { get; set; } // Технология освоения
        public string familirizationCondition { get; set; } //Условия освоения
        public string qualification { get; set; } // Квалификация
        public string eduProgramName { get; set; } // Название образовательной программы
        public DateTime? selectionDeadline { get; set; } //Дата окончания выбора варианта
        public string state { get; set; } //статус
    }

    public class DirectionApiDto
    {
        public string uid { get; set; } //Идентификатор направления в UNI
        public string okso { get; set; } // Код ОКСО
        public string title { get; set; } // Название направления
    }

    public class VariantGroupApiDto
    {
        public int id { get; set; } //Код группы модулей в ИТС
        public string groupName { get; set; } //Название группы модулей
        public int testUnits { get; set; } // Количество зачётных единиц
    }

    public class VariantSelectionGroupApiDto
    {
        public int id { get; set; } //Код группы выбора модулей в ИТС
        public string groupName { get; set; } //Название группы выбора модулей
        public int testUnits { get; set; } // Количество зачётных единиц
        public DateTime? selectionDeadline { get; set; } //Дата окончания выбора группы
    }

    public class VariantContentApiDto
    {
        public string moduleUuid { get; set; } //Код модуля в UNI
        public string title { get; set; } //Название из УНИ
        public int? number { get; set; } //Номер
        public string shortTitle { get; set; } //Короткое название
        public string coordinator { get; set; } //Координатор
        public string type { get; set; } //Тип
        public string competence { get; set; } //Компетенции
        public decimal testUnits { get; set; } //Количество зачётных единиц
        public decimal priority { get; set; } //Приоритет
        public string state { get; set; } //Состояние
        public DateTime? approvedDate { get; set; } //дата утверждения
        public string comment { get; set; } //Комментарий из UNI
        public string file { get; set; } //Ссылка на файл
        public string specialities { get; set; } //Специальности
        public DisciplineApiDto[] disciplines { get; set; } //Дисциплины
        public string variantContentType { get; set; } // (Общий\Траекторный\Выборный)

        public int groupId { get; set; } // идентификатор группы
        public int? selectionGroupId { get; set; } // идентификатор группы выбора

        public int id { get; set; } // Идентификатор использования модуля в варианте в ИТС
        public bool selectable { get; set; } //Признак выборности
        public int? limits { get; set; } //Лимиты
        public List<int> requiredVariantContentIds { get; set; } //Идентификаторы ИТС пререквизитов
        public List<ModueRequirementApiDto> requirements { get; set; } //Пререквизиты
        public List<PlanApiDto> plans { get; set; } //Учебные планы
    }

    public class ModueRequirementApiDto
    {
        public int variantContentId { get; set; } // Идентификатор использования модуля в варианте в ИТС
        public string moduleUuid { get; set; } //Код модуля в UNI
        public string moduleTitle { get; set; } //Название из УНИ
        public int? number { get; set; } //Номер модуля
    }


    public class DisciplineApiDto
    {
        public string uid { get; set; } //Код дисциплины из UNI
        public string title { get; set; } //Название
        public string section { get; set; } //Группа дисциплин из UNI
        public decimal testUnits { get; set; } //Количество зачётных единиц
        public string file { get; set; } //Ссылка на файл
        public int? number { get; set; } //Номер
    }

    public class PlanApiDto
    {
        public string eduplanUUID { get; set; } //Код учебного плана из UNI
        public int? eduplanNumber { get; set; } //Номер
        public string versionUUID { get; set; } // идентификатор плана
        public string versionNumber { get; set; } //номер версии
        public bool versionActive { get; set; }
        public string disciplineUUID { get; set; } //Код дисциплины из UNI
        public string disciplineTitle { get; set; } //название дисциплины 
        public string moduleUUID { get; set; } // Код модуля из UNI
        public string versionStatus { get; set; }
        public int[] allTerms { get; set; } // номера семестров
        public string catalogDisciplineUUID { get; set; } 
        public List<Dictionary<string, List<int>>> controls { get; set; } //контрольные мероприятия

        public Dictionary<string,List<TeacherApiDto>> teachers { get; set; } // Преподаватели сгруппированные по нагрузке
    }


    public class TeacherApiDto
    {
        public TeacherApiDto()
        {
            selectable = true;
        }

        public string pkey { get; set; } //Идентификатор
        public string firstName { get; set; } //Имя
        public string middleName { get; set; } //Отчество
        public string lastName { get; set; } //Фамилия
        public string post { get; set; } // Должность
        public string initials { get; set; } //Сокращённое имя
        public string division { get; set; } //Идентификатор подразделения
        public string workPlace { get; set; } //Место работы
        public bool selectable { get; set; } //Признак выборности
    }
    
    public class StudentAdmissionDto
    {
        public string studentId { get; set; } //ИД студента
        public List<VariantAdmissionDto> variants { get; set; } //Зачисление на варианты
        public List<ModuleAdmissionDto> modules { get; set; } //Зачисление на модули
    }

    [JsonConverter(typeof(StringEnumConverter))] 
    public enum AdmissionStatusDto
    {
        Indeterminate = 0, //Статуса нет
        Admitted, //Зачислен
        Denied, // 
        //ResetSelection // После публикации, сейчас не используется
    }

    public class VariantAdmissionDto
    {
        public int variantId { get; set; } // ИД варианта

        public AdmissionStatusDto status { get; set; } //Статус зачисления
    }

    public class ModuleAdmissionDto
    {
        public string moduleId { get; set; } //Ид модуля

        public AdmissionStatusDto status { get; set; } //Статус зачисления
    }


    public class SectionFKAdmissionDto
    {
        public string studentId { get; set; } //Идентификатор студента
        public string studentName { get; set; } //Конкатенация ФИО студента

        public string groupId { get; set; } //идентификатор группы студента
        public string groupName { get; set; }//название группы студента

        public SectionFKModuleAdmissionDto[] modules { get; set; } //модули студента
    }
    public class SectionFKModuleAdmissionDto
    {
        public string moduleId { get; set; } //Идентификатор модуля
        public string moduleName { get; set; } //title модуля
        public string moduleType { get; set; } //title модуля

        public RunpDisciplineDto[] disciplines { get; set; } // дисциплины модуля
        public int? year { get; set; }
        public int? term { get; set; }
    }

    public class ModulePeriodDto
    {
        public string moduleId { get; set; } //Идентификатор модуля
        public string moduleName { get; set; } //title модуля
        public string moduleType { get; set; } //title модуля
        public int year { get; set; }  // учебный год
        public int term { get; set; }  // номер семестра
        public int course { get; set; } // курс
        public ModuleDisciplineDto[] disciplines { get; set; } // дисциплины модуля

    }

    public class ModuleDisciplineDto
    {
        public string disciplineId { get; set; } //идентификатор дисциплины
        public string disciplineName { get; set; } //название дисциплины
        public List<String> tmers { get; set; } // виды нагрузки
    }

    public class RunpAdmissionDto
    {
        public string studentId { get; set; } //Идентификатор студента
        public string studentName { get; set; } //Конкатенация ФИО студента

        public string groupId { get; set; } //идентификатор группы студента
        public string groupName { get; set; }//название группы студента
        public int eduProgYear { get; set; } //год образовательной программы студента

        public RunpModuleAdmissionDto[] modules { get; set; } //модули студента
    }

    public class RunpModuleAdmissionDto
    {
        public string moduleId { get; set; } //Идентификатор модуля
        public string moduleName { get; set; } //title модуля
        public int? moduleLimit { get; set; } //лимит на модуль в программе

        public RunpDisciplineDto[] disciplines { get; set; } // дисциплины модуля
        public bool isBase { get; set; } //признак модуля из ОП
        public bool selectable { get; set; } //признак выборности модуля
        public string selectionGroupName { get; set; } //название группы выбора модуля
    }

    public class RunpDisciplineDto
    {
        public string disciplineId { get; set; } //идентификатор дисциплины
        public string disciplineName { get; set; } //название дисциплины
        public Dictionary<string, string> teachers { get; set; } //выбранные стдуентом преподы дисциплины
        public List<string> detailDisciplines { get; set; }
        public string terms { get; set; }
    }

    public class RunpProgramLimitDto
    {
        public int programId { get; set; } // оно тут чтобы не забыть что программа есть
        public string programName { get; set; }
        public string programQualification { get; set; }
        public string programFamCondition { get; set; }
        public string programFamType { get; set; }
        public string programFamTech { get; set; }
        public string programDivision { get; set; }
        public string programDirection { get; set; }
        public int programLimit { get; set; }
        public int programYear { get; set; }

        public List<RunpModuleLimitDto> modules { get; set; }
    }

    public class RunpModuleLimitDto
    {
        public string moduleId { get; set; }
        public int limit { get; set; }
        public List<String> disciplines { get; set; }
    }

    public class RunpDisciplineLimitDto
    {
        public string catalogDisciplineUUID { get; set; }
        public string disciplineUUID { get; set; }
        //public List<string> detailDisciplines { get; set; }
    }

    public class SubgroupApiDto
    {
        public int id { get; set; }

        public int innerNumber { get; set; }
        
        public string name { get; set; }
        
        public int limit { get; set; }

        public int studentCount { get; set; }

        public int? parentId { get; set; }

        public string groupId { get; set; }
        
        public string moduleId { get; set; }

        public string moduleName { get; set; }

        public int? moduleNumber { get; set; }

        public int term { get; set; }

        public int programId { get; set; }

        public string kmer { get; set; }

        public int year;
        public string combinedKey { get; set; }
        public string combinedKey2 { get; set; }

        public string catalogDisciplineUuid { get; set; }
        
        public string disciplineUUID { get; set; }
        
        public string additionalUUID { get; set; }

        public string dckey { get; set; }

        public string detailDiscipline { get; set; }

        public bool selectable { get; set; }
        public int removed { get; set; }
    }

    public class SubgroupWithMemebersApiDto
    {
        public int id { get; set; }

        public int innerNumber { get; set; }

        public string name { get; set; }
        
        public int limit { get; set; }

        public int studentCount { get; set; }

        public int? parentId { get; set; }

        public string groupId { get; set; }
        
        public string moduleId { get; set; }

        public string moduleName { get; set; }

        public int term { get; set; }

        public int programId { get; set; }

        public string kmer { get; set; }

        public int year;
        public string combinedKey { get; set; }
        public string combinedKey2 { get; set; }

        public string catalogDisciplineUuid { get; set; }

        public string disciplineUUID { get; set; }

        public string additionalUUID { get; set; }


        public string dckey { get; set; }

        public string detailDiscipline { get; set; }

        public bool selectable { get; set; }

        public List<string> students { get; set; }
    }
}
