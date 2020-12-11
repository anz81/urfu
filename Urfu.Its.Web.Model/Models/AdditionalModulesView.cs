using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Urfu.Its.Web.Models
{
    public class AdditionalModulesView
    {
        public AdditionalModulesView(string controllerName, string reportActionName, string localStorageName, string moduleType, bool useLevelFilter = false)
        {
            ControllerName = controllerName;
            ReportActionName = reportActionName;
            LocalStorageName = localStorageName;
            ModuleType = moduleType;
            UseLevelFilter = useLevelFilter;
        }

        public string ControllerName { get; set; }
        public string ReportActionName { get; set; }
        public string LocalStorageName { get; set; }
        public string ModuleType { get; set; }

        /// <summary>
        /// Использовать фильтр по уровню. Используется на форме Project
        /// </summary>
        public bool UseLevelFilter { get; set; }
    }
}