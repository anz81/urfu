using System.Linq;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared
{
    public static class FdpHelper
    {
        public static Plan GetDisciplinePlan(this ApplicationDbContext db, string linkedPlanId, string disciplineName)
        {
            var linkedPlan = db.Plans.First(p => p.disciplineUUID == linkedPlanId);
            var directionId = linkedPlan.directionId;
            var versionUuid = linkedPlan.versionUUID;
            var moduleUuid = linkedPlan.moduleUUID;
            var eduPlanUuid = linkedPlan.eduplanUUID;
            var familirizationType = linkedPlan.familirizationType;
            var actualPlan = db.Plans.FirstOrDefault(p =>
                p.directionId == directionId
                && p.versionUUID == versionUuid
                && p.moduleUUID == moduleUuid
                && p.eduplanUUID == eduPlanUuid
                && p.familirizationType == familirizationType
                && p.disciplineTitle == disciplineName);
            return actualPlan;
        }
    }
}