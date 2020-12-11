using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public abstract class BasePeriodEditViewModel
    {
        public int id { get; set; }
        public int year { get; set; }
        public int semesterId { get; set; }
        public string semesterName { get; set; }
        [DisplayName("Дата окончания выбора")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? selectionDeadline { get; set; }
        public bool isDeleted { get; set; }

        [DisplayName("Минимальное количество обучающихся")]
        public int min { get; set; }

        [DisplayName("Максимальное количество обучающихся")]
        public int max { get; set; }

        public SelectList Selector { get; set; }
    }

    public abstract class AdditionalModuleEditViewModel <T> where T : BasePeriodEditViewModel//periodType
    {
        public Module Module { get; set; }

        public string moduleUUId { get; set; }

        [DisplayName("Форма освоения майнора")]
        public string techid { get; set; }

        public string tech { get; set; }

        [DisplayName("Отображать в личном кабинете студента")]
        public bool showInLc { get; set; }

        [DisplayName("Без приоритета")]
        public bool withoutPriorities { get; set; }

        public List<T> periods { get; set; }

        public SelectList SemesterSelector { get; set; }
        public SelectList TechSelector { get; set; }
    }
}