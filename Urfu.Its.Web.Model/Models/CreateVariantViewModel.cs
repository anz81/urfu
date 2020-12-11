using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class CreateVariantViewModel
    {
        public CreateVariantViewModel()
        {
        }

        public int Id { get; set; }
        [Required]
        [DisplayName("Название траектории")]
        [Display(Name = "Название траектории")]
        public string Name { get; set; }
        
        [DisplayName("Направление")]
        [Display(Name="Направление")]
        public string directionId { get; set; }
        [DisplayName("Направление")]
        public virtual Direction Direction { get; set; }

        [DisplayName("Состояние")]
        public VariantState State { get; set; }

        [DisplayName("Дата создания")]
        public DateTime? CreateDate { get; set; }

        [DisplayName("Скопировать из траектории")]
        [Display(Name="Скопировать из траектории")]
        public int? CopyFromVariantId { get; set; }
        
        [DisplayName("Образовательная программа")]
        [Required]
        public int EduProgramId { get; set; }
    }
}