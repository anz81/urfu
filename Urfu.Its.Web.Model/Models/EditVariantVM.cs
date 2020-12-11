using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class EditVariantVM
    {
        public EditVariantVM()
        {
        }
        public EditVariantVM(Variant variant)
        {
            VariantId = variant.Id;
            Name = variant.Name;
            Year = variant.Program.Year;
            StudentsLimit = variant.StudentsLimit;
            SelectionDeadline = variant.SelectionDeadline;
            IsBase = variant.IsBase;
            EduProgram = variant.Program.Name;
            qualification = variant.Program.qualification;
            familirizationType = variant.Program.familirizationType;
            familirizationCondition = variant.Program.familirizationCondition;
            State = variant.State;
        }
        [Key]
        public Int32 VariantId { get; set; }

        [DisplayName("Условие освоения")]
        public string familirizationCondition { get; set; }

        [DisplayName("Форма освоения")]
        public string familirizationType { get; set; }

        [DisplayName("Квалификация")]
        public string qualification { get; set; }

        [DisplayName("Образовательная программа")]
        public string EduProgram { get; set; }

        [DisplayName("Название траектории")]
        public string Name { get; set; }

        [DisplayName("Год")]
        public int Year { get; set; }

        [DisplayName("Лимит студентов")]
        public int StudentsLimit { get; set; }

        [DisplayName("Дата окончания выбора")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SelectionDeadline { get; set; }

        public bool IsBase { get; set; }

        [DisplayName("Состояние")]
        public VariantState State { get; set; }
    }
}