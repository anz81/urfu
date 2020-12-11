using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration.ApiModel
{
    public class PeriodApiDto
    {
        public int year { get; set; }
        public string semester { get; set; }
        public int semesterId { get; set; }

        [JsonConverter(typeof(DateConverter))]
        public DateTime selectionDeadline { get; set; }
        
        //+2 лимита верхний и нижний
        public int? minStudentCount { get; set; }
        public int? maxStudentCount { get; set; }
    }

    public class MinorDisciplineTmerPeriodApiDto
    {
        public int year { get; set; }
        public string semester { get; set; }

        public string[] chairs { get; set; }
    }

    public class MinorDisciplineTmerApiDto
    {
        public string rmer { get; set; }
        public MinorDisciplineTmerPeriodApiDto[] periods;
    }

    public class MinorDisciplineApiDto
    {
        public string uid { get; set; } //Код дисциплины из UNI
        public string title { get; set; } //Название
        public string section { get; set; } //Группа дисциплин из UNI
        public decimal testUnits { get; set; } //Количество зачётных единиц
        public string file { get; set; } //Ссылка на файл
        public int? number { get; set; } //Номер

        public MinorDisciplineTmerApiDto[] tmers { get; set; }
    }

    public class ModuleApiDto
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
        public string specialities { get; set; }

        //по форме освоения(значение справочника-традиционная/смешанная/онлайн курс)
        public string tech { get; set; }
        public bool showInLC { get; set; }
        public string requirmentId { get; set; }
        public string requirmentTitle { get; set; }

        public PeriodApiDto period { get; set; }
        public MinorDisciplineApiDto[] disciplines { get; set; }
    }

    public class MinorApiDto : ModuleApiDto
    {
        public ModuleAgreementApiDto agreement { get; set; }
    }

    public class MinorSubgroupApiDto
    {
        //id модуля 
        public string moduleId { get; set; }

        //название модуля
        public string moduleName { get; set; }

        public string moduleType { get; set; }

        // Номер модуля
        public int? moduleNumber { get; set; }

        //id дисциплины 
        public string disciplineId { get; set; }

        //название дисциплины
        public string disciplineName { get; set; }

        //вид учебной нагрузки
        public string loadTypeId { get; set; }

        // вид учебной нагрузки (название)
        public string loadTypeName { get; set; }

        // учебный год 
        public int eduyear { get; set; }

        //семестр
        public int term { get; set; }

        //чит.кафедра (может быть несколько чит.кафедр)
        public string[] divisions { get; set; }

        //id подгруппы
        public int id { get; set; }

        // id родительской подгруппы
        public int? parentId { get; set; }

        //название подгруппы
        public string name { get; set; }

        //кол-во студентов
        public int studentCount { get; set; }

        //кол-во подгрупп
        public int groupCount { get; set; }
        
        // Ключ минор-подгруппы для БРС
        public string combinedKey { get; set; }

        // Ключ минор-подгруппы для Расписания и ЛК
        public string combinedKey2 { get; set; }

        // лимит
        public int limit { get; set; }

        public string teacherId { get; set; }
        public string competitionGroupName { get; set; }
        public string competitionGroupShortName { get; set; }
        public string chairId { get; set; }
        public int? course { get; set; }
        public string description { get; set; }
        public int removed { get; set; }
    }

    public class MinorSubgroupWithMemebersApiDto
    {
        //id модуля 
        public string moduleId { get; set; }

        //название модуля
        public string moduleName { get; set; }

        public string moduleType { get; set; }

        //id дисциплины 
        public string disciplineId { get; set; }

        //название дисциплины
        public string disciplineName { get; set; }

        //вид учебной нагрузки
        public string loadTypeId { get; set; }

        // вид учебной нагрузки (название)
        public string loadTypeName { get; set; }

        // учебный год 
        public int eduyear { get; set; }

        //семестр
        public int term { get; set; }

        //чит.кафедра (может быть несколько чит.кафедр)
        public DivisionApiDto[] divisions { get; set; }

        //id подгруппы
        public int id { get; set; }

        //название подгруппы
        public string name { get; set; }

        //кол-во студентов
        public int studentCount { get; set; }

        //студенты с учетом подгрупп
        public string[] students { get; set; }

        //кол-во подгрупп
        public int groupCount { get; set; }

        // Ключ минор-подгруппы для БРС
        public string combinedKey { get; set; }

        // Ключ минор-подгруппы для Расписания и ЛК
        public string combinedKey2 { get; set; }

        public string teacherId { get; set; }
        public int? studentCourse { get; set; }
        public string competitionGroupName { get; set; }
        public string chairId { get; set; }

        public override bool Equals(object obj)
        {
            var dto = obj as MinorSubgroupWithMemebersApiDto;
            if (dto == null)
                return false;

            var equals = true;
            foreach (var property in typeof(MinorSubgroupWithMemebersApiDto).GetProperties())
            {
                var dtoValue = property.GetValue(dto);
                var thisValue = property.GetValue(this);
                if (dtoValue is DivisionApiDto[])
                    equals = equals && (dtoValue as DivisionApiDto[]).SequenceEqual(thisValue as DivisionApiDto[]);
                else if (dtoValue is string[])
                    equals = equals && (dtoValue as string[]).Except(thisValue as string[]).Count() == 0 
                                    && (thisValue as string[]).Except(dtoValue as string[]).Count() == 0;
                else
                    equals = equals && dtoValue?.GetHashCode() == thisValue?.GetHashCode();
                
            }

            return equals;
        }
    }

    public class ModuleSubgroupContentApiDto
    {
        public int subgroupId { get; set; }
        public string combinedKey2 { get; set; }
        public IEnumerable<string> groups { get; set; }
    }
}
