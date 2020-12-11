using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;
using Urfu.Its.Web.Models;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class ModuleAnnotationStructure
    {
        public ICollection<ModuleAnnotationRow> RequiredModules { get; set; } = new List<ModuleAnnotationRow>();
        public ICollection<ModuleAnnotationRow> Modules { get; set; } = new List<ModuleAnnotationRow>();
        public ICollection<ModuleAnnotationRow> Practices { get; set; } = new List<ModuleAnnotationRow>();
        public ICollection<ModuleAnnotationRow> Gia { get; set; } = new List<ModuleAnnotationRow>();
        public ICollection<VariantSourceInfo> PossibleVariants { get; set; } = new List<VariantSourceInfo>();

        public ICollection<ModuleAnnotationRow> Rows
        {
            get
            {
                var rows = new List<ModuleAnnotationRow>();
                rows.Add(new ModuleAnnotationRow() { Name = "Модули" });
                rows.Add(new ModuleAnnotationRow() { Name = "Обязательная часть" });
                rows.AddRange(RequiredModules);
                rows.Add(new ModuleAnnotationRow() { Name = "Формируемая участниками образовательных отношений" });
                rows.AddRange(Modules);
                rows.Add(new ModuleAnnotationRow() { Name = "Практика" });
                rows.AddRange(Practices);
                rows.Add(new ModuleAnnotationRow() { Name = "Государственная итоговая аттестация" });
                rows.AddRange(Gia);

                return rows;
            }
        }
    }

    public class ModuleAnnotationRow : ModuleExtraInfo
    {
        public string Annotation { get; set; }
        public ICollection<VariantSourceInfo> Variants { get; set; } = new List<VariantSourceInfo>();

        public ModuleAnnotationRow()
        {

        }

        public ModuleAnnotationRow(Web.DataContext.Module module)
        {
            Id = module.uuid;
            Name = module.title;
            Coordinator = module.coordinator;
            Number = module.number;
            Capacity = module.testUnits;
            Annotation = module.annotation;
        }
    }
}
