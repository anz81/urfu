using System;
using System.Collections.Generic;
using System.Linq;

using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.Practice
{
    public class PlanTermViewModel
    {
        //сквозная нумерация
        public int Term { get; set; }
        public int Course { get; set; }
        public int SemesterID { get; set; }
        public string Semester { get; set; }
    }

    public class PlanViewModel
    {
        public string DisciplineUID { get; set; }
        public string EduplanUID { get; set; }
        public string Name { get; set; } 

        public List<PlanTermViewModel> AllTerms { get; set; }
        public List<PlanTermViewModel> PlanTerms { get; set; }

        public PlanViewModel(Plan plan, List<PlanTerm> termsCount, List<Semester> semesters)
        {
            DisciplineUID = plan.disciplineUUID;
            EduplanUID = plan.eduplanUUID;
            Name = plan.disciplineTitle;

            CreateAllTerms(termsCount, semesters);
            PlanTerms = plan
               .allTermsExtracted
               .Substring(1, plan.allTermsExtracted.Length - 2)
               .Split(',')
               .Select(GetTerm)
               .ToList();
        }

        public List<PlanTermViewModel> GetPracticeTerm()
        {
            List<PlanTermViewModel> result = new List<PlanTermViewModel>();
            foreach(var term in AllTerms)
            {
                if (PlanTerms.Contains(term))
                    result.Add(term);
            }
            return result;
        }

        private void CreateAllTerms(List<PlanTerm> termsCount, List<Semester> semesters)
        {
            if (termsCount.Count == 0)
            {
                termsCount.AddRange(new[]
                {
                    new PlanTerm{ Year= 1, TermsCount = 2 },
                    new PlanTerm{ Year= 2, TermsCount = 2 },
                    new PlanTerm{ Year= 3, TermsCount = 2 },
                    new PlanTerm{ Year= 4, TermsCount = 2 },
                    new PlanTerm{ Year= 5, TermsCount = 2 },
                });
            }

            var count = 0;
            AllTerms = new List<PlanTermViewModel>();

            foreach (var tc in termsCount.OrderBy(t => t.Year))
            {
                for (var i = 1; i <= tc.TermsCount; i++)
                {
                    count++;
                    var semester = SemesterByOrder(semesters, i);

                    AllTerms.Add(new PlanTermViewModel
                    {
                        Term = count,
                        Course = tc.Year,
                        SemesterID = semester.Id,
                        Semester = semester.Name,
                    });
                }
            }
        }

        private PlanTermViewModel GetTerm(string term)
        {
            int t;
            if (int.TryParse(term, out t))
            {
                if (t <= AllTerms.Count)
                {
                    return AllTerms[t - 1];
                }
            }
            return null;
        }

        private Semester SemesterByOrder(List<Semester> semesters, int i)
        {
            switch (i)
            {
                case 1: return semesters.FirstOrDefault(s => s.Id == 1);
                case 2: return semesters.FirstOrDefault(s => s.Id == 2);
                case 3: return semesters.FirstOrDefault(s => s.Id == 0);
                default: return semesters.FirstOrDefault(s => s.Id == 0);
            }


            throw new ArgumentOutOfRangeException($"Не известный порядковый номер ({i}) семестра");
        }

    }
}
