using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class SectionFKPeriodEditViewModel : BasePeriodEditViewModel
    {
        // amir: если понадобится дополнительные поля
        public SelectList MaleSelector { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SelectionBegin { get; set; }
        public int? Course { get; set; }
        public SelectList CourseSelector { get; set; }
        public Boolean? Male { get; set; }
    }

    public class SectionFKEditViewModel : AdditionalModuleEditViewModel<SectionFKPeriodEditViewModel>
    {
        
    }

    public class ForeignLanguagePeriodEditViewModel : BasePeriodEditViewModel
    {
        // amir: если понадобится дополнительные поля
        public int? Course { get; set; }
        public SelectList CourseSelector { get; set; }

    }

    public class ForeignLanguageEditViewModel : AdditionalModuleEditViewModel<ForeignLanguagePeriodEditViewModel>
    {
        
    }

    public class ProjectPeriodEditViewModel : BasePeriodEditViewModel
    {
        // amir: если понадобится дополнительные поля

    }

    public class ProjectEditViewModel : AdditionalModuleEditViewModel<ProjectPeriodEditViewModel>
    {
        public string Competences { get; set; }
    }

    public class MUPPeriodEditViewModel : BasePeriodEditViewModel
    {
        public int? Course { get; set; }
        public SelectList CourseSelector { get; set; }

    }

    public class MUPEditViewModel : AdditionalModuleEditViewModel<MUPPeriodEditViewModel>
    {
    }
}