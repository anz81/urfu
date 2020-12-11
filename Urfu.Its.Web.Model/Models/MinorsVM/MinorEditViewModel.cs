using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class MinorPeriodEditViewModel : BasePeriodEditViewModel
    {
        
    }

    public class MinorEditViewModel :AdditionalModuleEditViewModel<MinorPeriodEditViewModel>
    {
        
    }
}