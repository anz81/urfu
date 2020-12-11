using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.CompetencePassports.Loaders
{
    public class EduResultsLoader : ObjectBlockContentLoader<EduResults>
    {
        private readonly ApplicationDbContext _db;
        private readonly CompetencePassport _passport;
        private readonly BasicCharacteristicOPSchemaModel _ohopData;

        public EduResultsLoader(ApplicationDbContext db, CompetencePassport passport, BasicCharacteristicOPSchemaModel ohopData)
        {
            _db = db;
            _passport = passport;
            _ohopData = ohopData;
        }

        protected override EduResults LoadAnyContent(JToken blockContent)
        {
            var eduResults = new EduResults();

            var item = blockContent as JObject;

            if (item[nameof(EduResults.GeneralCompetences)].Type != JTokenType.Null)
            {
                var competences = item[nameof(EduResults.GeneralCompetences)].Value<object>();
                eduResults.GeneralCompetences = JsonConvert.DeserializeObject<ICollection<CompetenceEduResult>>(competences.ToString());
            }
            if (eduResults.GeneralCompetences.Count == 0)
            {
                eduResults.GeneralCompetences = _ohopData.GeneralCompetences.Select(c => new CompetenceEduResult() { Competence = c }).OrderBy(c => c.Competence.Order).ToList();
            }

            if (item[nameof(EduResults.UniversalCompetences)].Type != JTokenType.Null)
            {
                var competences = item[nameof(EduResults.UniversalCompetences)].Value<object>();
                eduResults.UniversalCompetences = JsonConvert.DeserializeObject<ICollection<CompetenceEduResult>>(competences.ToString());
            }
            if (eduResults.UniversalCompetences.Count == 0)
            {
                eduResults.UniversalCompetences = _ohopData.UniversalCompetences.Select(c => new CompetenceEduResult() { Competence = c }).OrderBy(c => c.Competence.Order).ToList();
            }

            if (item[nameof(EduResults.VariantProfCompetences)].Type != JTokenType.Null)
            {
                var competences = item[nameof(EduResults.VariantProfCompetences)].Value<object>();
                eduResults.VariantProfCompetences = JsonConvert.DeserializeObject<ICollection<VariantProfCompetences>>(competences.ToString());
            }
            if (eduResults.VariantProfCompetences.Count == 0)
            {
                eduResults.VariantProfCompetences = _ohopData.Variants.VariantInfos.Select(v => new VariantProfCompetences()
                {
                    Id = v.Id,
                    IdSource = v.IdSource,
                    Name = v.Name,
                    ProfCompetences = v.ProfActivityRows.SelectMany(p => p.Competences).GroupBy(c => c.Id).Select(c => new CompetenceEduResult() { Competence = c.First() }).OrderBy(c => c.Competence.Order).ToList()
                }).ToList();
            }

            return eduResults;
        }
    }
}