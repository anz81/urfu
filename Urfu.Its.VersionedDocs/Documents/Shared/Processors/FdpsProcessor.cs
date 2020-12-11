using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Module.Processors
{
    public class FdpsProcessor : IBlockContentProcessor
    {
        private readonly IVersionedDocumentInspector _inspector;
        private readonly ApplicationDbContext _db;

        public FdpsProcessor(IVersionedDocumentInspector inspector, ApplicationDbContext db)
        {
            _inspector = inspector;
            _db = db;
        }

        public JToken ProcessContent(JToken data)
        {
            var fdps = (JArray) data;
            foreach (var fdp in fdps)
            {
                var disciplineIdToken = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DisciplineId)];
                if (disciplineIdToken.Type == JTokenType.Null)
                {
                    _inspector.Error("Не указана версия УП.");
                    _inspector.StopProcessing();
                }

                if (fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)]?.Value<string>() == null)
                    fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)] = Guid.NewGuid().ToString();

                var disciplineId = disciplineIdToken.Value<string>();
                var plan = _db.Plans.FirstOrDefault(p=>p.disciplineUUID == disciplineId);
                if (plan == null)
                {
                    _inspector.Error($"УП не найден по идентификатору дисциплины {disciplineId}");
                    _inspector.StopProcessing();
                }

                if (plan.versionStatus != "Утверждено")
                {
                    _inspector.Error($"УП не утвержден");
                    _inspector.StopProcessing();
                }

                fdp[nameof(FamilirizationTypeDirectionPlanInfo.PlanVersionId)] = plan.versionUUID;
                fdp[nameof(FamilirizationTypeDirectionPlanInfo.PlanNumber)] = plan.eduplanNumber;
                fdp[nameof(FamilirizationTypeDirectionPlanInfo.PlanVersionTitle)] = plan.versionTitle;
            }

            return fdps;
        }
    }
}