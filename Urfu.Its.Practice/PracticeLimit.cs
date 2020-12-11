using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Practices
{

    public class PracticeLimit
    {
        public ContractLimit Limit { get; set; }
        public List<PracticeAdmissionCompany> Admissions { get; } = new List<PracticeAdmissionCompany>();

        public int Fill()
        {
            return Admissions.Count();
        }

        public void SetAdmissions(ApplicationDbContext db, List<PracticeAdmissionCompany> admissions)
        {
            var test = admissions.ToList();

            //забиваем лимит до упора
            foreach (var a in test)
            {
                bool isNewStudent = !Admissions.Select(_a => _a.Practice.StudentId).Contains(a.Practice.StudentId);

                if (Admissions.Count >= Limit.Limit && isNewStudent)
                    break;

                if (a.ContractId != Limit.Period.ContractId)
                    continue;

                if (Limit.Profile != null && Limit.ProfileId != a.Practice.Group.ProfileId)
                    continue;
                if (Limit.QualificationName != null && Limit.QualificationName != a.Practice.Group.Qual)
                    continue;
                if (Limit.Course != 0 && Limit.Course != a.Practice.Group.Course)
                    continue;

                var directionID = db.Plans.Where(p => p.disciplineUUID == a.Practice.DisciplineUUID).Select(p => p.directionId).FirstOrDefault();
                if (Limit.DirectionId != null && Limit.DirectionId != directionID)
                    continue;

                if (isNewStudent)
                {
                    Admissions.Add(a);
                }
                admissions.Remove(a);
            }
        }
    }
}
