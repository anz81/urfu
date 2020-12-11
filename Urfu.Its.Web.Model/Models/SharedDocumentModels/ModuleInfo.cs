using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.CompetencePassportModels;

namespace Urfu.Its.Web.Model.Models.SharedDocumentModels
{
    public class ModuleInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Code { get; set; }
        public int Capacity { get; set; }
    }

    public class ModuleInfoSelected : ModuleInfo
    {
        public bool Selected { get; set; } = true;
        public bool IncludeInCore { get; set; }
    }

    public class ModuleExtraInfo : ModuleInfo
    {
        public int? Number { get; set; }
        public string Coordinator { get; set; }

        public string FullName
        {
            get
            {
                if (Number.HasValue && !string.IsNullOrWhiteSpace(Coordinator))
                    return $"{Name} ({Number}, {Coordinator})";
                if (Number.HasValue && string.IsNullOrWhiteSpace(Coordinator))
                    return $"{Name} ({Number})";
                if (!Number.HasValue && !string.IsNullOrWhiteSpace(Coordinator))
                    return $"{Name} ({Coordinator})";
                return Name;
            }
        }
    }

    public class ModuleInfoWithDisciplines : ModuleExtraInfo
    {
        public ICollection<DisciplineInfo> Disciplines { get; set; } = new List<DisciplineInfo>();
        
        public ModuleInfoWithDisciplines()
        {

        }

        public ModuleInfoWithDisciplines(Module module)
        {
            Id = module.uuid;
            Name = module.title;
            Number = module.number;
            Coordinator = module.coordinator;
            Disciplines = module.disciplines.Select(d => new DisciplineInfo()
            {
                Id = d.uid,
                Name = d.title
            }).ToList();
        }
    }

    public class DisciplineInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<EduResultsInfo> EduResults { get; set; } = new List<EduResultsInfo>();
    }
}