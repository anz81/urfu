using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class EditVariantSelectionGroupViewModel
    {
        [Key]
        public int VariantId { get; set; }
        public string VaraintName { get; set; }
        public List<EditVariantSelectionGroupRowViewModel> Rows { get; set; }
    }
    public class EditVariantSelectionGroupRowViewModel
    {
        public EditVariantSelectionGroupRowViewModel()
        {
        }

        public EditVariantSelectionGroupRowViewModel(VariantSelectionGroup vsg)
        {
            Id = vsg.Id;
            Name = vsg.Name;
            TestUnits = vsg.TestUnits;
            SelectionDeadline = vsg.SelectionDeadline;
        }

        [Required]
        public int Id { get; set; }

        [Display(Name = "Название группы выбора")]
        public string Name { get; set; }

        [Display(Name = "Зачётные единицы")]
        public int TestUnits { get; set; }

        [DisplayName("Дата окончания выбора")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SelectionDeadline { get; set; }
    }
}