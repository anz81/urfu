using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public static class ModuleStructureAdditionalInfo
    {
        public static Dictionary<string, string> ModuleAdditionals { get; } = new Dictionary<string, string>()
            {
            // ключи используются в ModuleStructure.js !!!
            { "requiredPart", "Модули обязательной части" },
                {"formedPart", "Модули части, формируемые участниками образовательных отношений"}
            };

        public static Dictionary<string, string> PracticeAdditionals { get; } = new Dictionary<string, string>()
            {
                {"studyPractice", "Учебная практика"},
               {"internship", "Производственная практика"}
            };

        public static Dictionary<string, string> GiaAdditionals { get; } = new Dictionary<string, string>()
            {
                {"gia", "Подготовка к сдаче и сдача государственного экзамена"},
                {"diploma", "Подготовка к процедуре защиты и защита выпускной квалификационной работы"}
            };
    }

    public class ModuleStructure
    {
        public ICollection<ModuleInfoSelected> Modules { get; set; } = new List<ModuleInfoSelected>();
        public ICollection<ModuleInfoSelected> Practices { get; set; } = new List<ModuleInfoSelected>();
        public ICollection<ModuleInfoSelected> Gia { get; set; } = new List<ModuleInfoSelected>();
        public ICollection<ModuleInfoSelected> Facultative { get; set; } = new List<ModuleInfoSelected>();

        public int Sum { get { return ModulesSum + PracticesSum + GiaSum; } }

        public int RequiredSum { get; set; }

        // Используются только в шаблоне

        public int ModulesSum { get { return Modules.Where(m => ModuleStructureAdditionalInfo.ModuleAdditionals.ContainsKey(m.Id)).Select(m => m.Capacity).Sum(); } }

        public int PracticesSum { get { return Practices.Where(m => m.Selected).Select(m => m.Capacity).Sum(); } }

        public int GiaSum { get { return Gia.Where(m => m.Selected).Select(m => m.Capacity).Sum(); } }

        public int FacultativeSum { get { return Facultative.Where(m => m.Selected).Select(m => m.Capacity).Sum(); } }

        public ICollection<ModuleInfoSelected> SelectedModules { get { return Modules.Where(m => m.Selected).ToList(); } }
        public ICollection<ModuleInfoSelected> SelectedPractices { get { return Practices.Where(m => m.Selected && m.Capacity > 0).ToList(); } }
        public ICollection<ModuleInfoSelected> SelectedGia { get { return Gia.Where(m => m.Selected && m.Capacity > 0).ToList(); } }
        public ICollection<ModuleInfoSelected> SelectedFacultative { get { return Facultative.Where(m => m.Selected && m.Capacity > 0).ToList(); } }
    }
}
