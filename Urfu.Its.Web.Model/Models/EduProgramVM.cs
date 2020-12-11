using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    [NotMapped]
    public class EduProgramVM : EduProgram
    {
        [Required(ErrorMessage = "Введите версию плана")]
        [DisplayName("Версия плана")]
        public int RequiredPlanVersionNumber { get; set; }

        [Required(ErrorMessage = "Введите номер плана")]
        [DisplayName("Номер плана")]
        public int? RequiredPlanNumber { get; set; }
    }
}