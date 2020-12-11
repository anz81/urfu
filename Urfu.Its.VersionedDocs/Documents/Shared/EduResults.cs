using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Model.Models.CompetencePassportModels;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public class EduResults
    {
        /// <summary>
        /// Результаты обучения Универсальных компетенций (УК)
        /// </summary>
        public ICollection<CompetenceEduResult> UniversalCompetences { get; set; } = new List<CompetenceEduResult>();

        /// <summary>
        /// Результаты обучения Общепрофессиональных компетенций (ОПК)
        /// </summary>
        public ICollection<CompetenceEduResult> GeneralCompetences { get; set; } = new List<CompetenceEduResult>();

        /// <summary>
        /// Результаты обучения Профессиональных компетенций (ПК)
        /// </summary>
        public ICollection<VariantProfCompetences> VariantProfCompetences { get; set; } = new List<VariantProfCompetences>();
    }

    public class CompetenceEduResult
    {
        public CompetenceInfoVM Competence { get; set; }
        public ICollection<ModuleInfoWithDisciplines> Modules { get; set; } = new List<ModuleInfoWithDisciplines>();

        public ICollection<EduResultRow> Rows { get
            {
                var baseRow = new EduResultRow();
                baseRow.KnowledgeEduResults = FilterEduResults(kind: 1, type: 1);
                baseRow.SkillEduResults = FilterEduResults(kind: 2, type: 1);
                baseRow.PracticalExperienceEduResults = FilterEduResults(kind: 3, type: 1);
                baseRow.OtherEduResults = FilterEduResults(kind: 4, type: 1);
                baseRow.Modules = Modules.Where(m => m.Disciplines.Any(d => d.EduResults.Any(r => r.TypeId == 1))).ToList();

                var additionalRow = new EduResultRow();
                additionalRow.KnowledgeEduResults = FilterEduResults(kind: 1, type: 2);
                additionalRow.SkillEduResults = FilterEduResults(kind: 2, type: 2);
                additionalRow.PracticalExperienceEduResults = FilterEduResults(kind: 3, type: 2);
                additionalRow.OtherEduResults = FilterEduResults(kind: 4, type: 2);
                additionalRow.Modules = Modules.Where(m => m.Disciplines.Any(d => d.EduResults.Any(r => r.TypeId == 2))).ToList();

                return new List<EduResultRow>() { baseRow, additionalRow };
            }
        }

        public ICollection<EduResultsInfo> FilterEduResults(int kind, int type)
        {
            return Modules.SelectMany(m => m.Disciplines).SelectMany(d => d.EduResults)
                    .Where(r => r.KindId == kind && r.TypeId == type).OrderBy(r => r.SerialNumber).ToList();
        }
    }

    public class VariantProfCompetences : VariantSourceInfo
    {
        public ICollection<CompetenceEduResult> ProfCompetences { get; set; } = new List<CompetenceEduResult>();
    }

    public class EduResultRow
    {
        public ICollection<EduResultsInfo> KnowledgeEduResults { get; set; }

        public ICollection<EduResultsInfo> SkillEduResults { get; set; }

        public ICollection<EduResultsInfo> PracticalExperienceEduResults { get; set; }

        public ICollection<EduResultsInfo> OtherEduResults { get; set; }

        public ICollection<ModuleInfoWithDisciplines> Modules { get; set; }
    }
}
