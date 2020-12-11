using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Web.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Frames.Controllers
{
    public class PracticeTypes
    {
        static public string T1 => "Учебная практика";
        static public string T2 => "Производственная практика";

        static public List<string> Names = new List<string>
        {
            T1,T2
        };
    }

    internal class TermVM
    {
        public int Term { get; set; }
        public int Course { get; set; }
        public int SemesterID { get; set; }
        public string Semester { get; set; }

        public override bool Equals(object obj)
        {
            TermVM term = obj as TermVM;
            if (term == null) return false;
            bool equal =
                Term == term.Term
                && Course == term.Course
                && SemesterID == term.SemesterID
                && Semester == term.Semester;
            return equal;
        }
    }

    public class PlanVM
    {
        internal List<Semester> Semesters { get; set; }

        internal Plan Plan { get; set; }
        internal List<PlanTerm> TermsCount { get; set; }

        internal List<TermVM> Terms { get; set; }

        internal string File { get; set; }
        internal string Level { get; set; }

        internal PlanVM(Plan plan, List<PlanTerm> termsCount, List<Semester> semesters, string file = null, string level = null)
        {
            Plan = plan;
            TermsCount = termsCount;
            Semesters = semesters;
            File = file;
            Level = level;

            CreateAllTerms();
        }

        private void CreateAllTerms()
        {
            if (TermsCount.Count == 0)
            {
                TermsCount = new List<PlanTerm>
                {
                    new PlanTerm{ Year= 1, TermsCount = 2 },
                    new PlanTerm{ Year= 2, TermsCount = 2 },
                    new PlanTerm{ Year= 3, TermsCount = 2 },
                    new PlanTerm{ Year= 4, TermsCount = 2 },
                    new PlanTerm{ Year= 5, TermsCount = 2 },
                };
            }

            var count = 0;
            Terms = new List<TermVM>();

            foreach (var tc in TermsCount.OrderBy(t => t.Year))
            {
                for (var i = 1; i <= tc.TermsCount; i++)
                {
                    count++;
                    var semester = SemesterByOrder(i);

                    Terms.Add(new TermVM
                    {
                        Term = count,
                        Course = tc.Year,
                        SemesterID = semester.Id,
                        Semester = semester.Name,
                    });
                }
            }
        }

        private Semester SemesterByOrder(int i)
        {
            switch (i)
            {
                case 1: return Semesters.FirstOrDefault(s => s.Id == 1);
                case 2: return Semesters.FirstOrDefault(s => s.Id == 2);
                default: return Semesters.FirstOrDefault(s => s.Id == 0);
            }
        }

        internal IEnumerable<TermVM> GetTerms()
        {
            if (Terms == null)
                CreateAllTerms();

            var list = Plan
               .allTermsExtracted
               .Substring(1, Plan.allTermsExtracted.Length - 2)
               .Split(',')
               .Select(GetTerm)
               .ToList();

            return list;
        }

        private TermVM GetTerm(string term)
        {
            int t;
            if (int.TryParse(term, out t))
            {
                if (t <= Terms.Count)
                {
                    return Terms[t - 1];
                }
            }
            return null;
        }
    }

    internal class PlanTerm2
    {
        internal PlanVM Plan { get; set; }
        internal TermVM Term { get; set; }
        internal Practice Practice { get; set; }

        string TermStr { get; set; }

        internal PlanTerm2(PlanVM plan, string term)
        {
            Plan = plan;
            int t;
            if (int.TryParse(term, out t))
            {
                if (t <= Plan.Terms.Count)
                {
                    Term = Plan.Terms[t - 1];
                    return;
                }
            }

            TermStr = term;
        }
    }

    public class PracticeInfoVM
    {
        public int SemesterID { get; set; }
        public string GroupID { get; set; }

        public string DiscilineUID { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; }
        public string PracticeType { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }

        public int PracticeID { get; set; }
        public string PracticePeriod { get; set; }

        public string YearInfo => $"{Year}/{Year + 1} уч.год";
        public string SemesterInfo => $"{Semester} семестр";
    }

    public class PracticeMainList
    {
        public string StudentFIO { get; set; }
        public List<PracticeList2> Practices { get; set; }

        public PracticeMainList(ApplicationDbContext db, List<string> studentIds)
        {
            studentIds = studentIds ?? new List<string>();
            Practices = new List<PracticeList2>();
            foreach (var studentId in studentIds)
            {
                Practices.Add(new PracticeList2(db, studentId));
            }
            Practices = Practices.OrderBy(p => p.StudentId).ToList();
            StudentFIO = db.Students.FirstOrDefault(s => studentIds.Contains(s.Id))?.Person.FullName();
        }

        public PracticeMainList(ApplicationDbContext db, string studentId)
        {
            Practices = new List<PracticeList2>();
            Practices.Add(new PracticeList2(db, studentId));
            Practices = Practices.OrderBy(p => p.StudentId).ToList();
            StudentFIO = db.Students.FirstOrDefault(s => studentId == s.Id)?.Person.FullName();
        }
    }

    public class PracticeList2
    {
        private ApplicationDbContext _db;
        public string StudentId { get; set; }

        public List<Group2VM> Groups { get; set; }
        public List<PlanVM> Plans { get; set; }

        public PracticeList2(ApplicationDbContext db, string studentId)
        {
            _db = db;
            StudentId = studentId;

            var student = _db.Students
                .Include(s => s.Person)
                .Where(s => s.Id == StudentId)
                .FirstOrDefault();

            var groups = _db.GroupsHistories
              .Where(h => h.GroupId == student.GroupId)
              .ToList();

            Groups = groups.Select(g => new Group2VM(g)).ToList();

            var semesters = _db.Semesters.ToList();

            var planTerms = _db.PlanTerms;

            var plans = _db.Plans
                .Where(p => p.eduplanNumber == student.planVerion
                    && p.versionNumber == student.versionNumber
                    && PracticeTypes.Names.Contains((p.additionalType))
                    && !p.remove)
                 .Select(p => new
                 {
                     Plan = p,
                     PlanTerms = planTerms.Where(pt => pt.eduplanUUID == p.eduplanUUID)
                 })
                .ToList();

            Plans = plans.Select(p => new PlanVM(p.Plan, p.PlanTerms.ToList(), semesters)).ToList();

            foreach (var p in Plans)
            {
                var terms = p.GetTerms();

                foreach (var t in terms)
                {
                    if (t != null)
                    {
                        var group = Groups.LastOrDefault(g => g.Course == t.Course);
                        if (group != null)
                            group.Add(p, t);
                    }

                }
            }

            var practics = _db.Practices
                .Where(p => p.StudentId == StudentId && !p.remove)
                .ToList();

            foreach (var p in practics)
            {
                var group = Groups.LastOrDefault(g => g.Year == p.Year);
                if (group == null)
                    continue;

                group.Add(p);
            }

        }
    }

    public class Group2VM
    {
        private GroupsHistory _group;

        public string ID => _group.Id;
        public string Name => _group.Name;
        public int Year => _group.YearHistory;
        public int Course => _group.Course;
        public string YearInfo => $"{Year}/{Year + 1} уч.год";

        public Dictionary<string, List<PracticeInfo2VM>> Practices;

        public Group2VM(GroupsHistory group)
        {
            _group = group;
            Practices = PracticeTypes.Names.ToDictionary(n => n, n => new List<PracticeInfo2VM>());
        }



        public bool HavePractice => Practices.Values.Any(l => l.Count > 0);

        internal void Add(PlanVM p, TermVM t)
        {
            var practice = new PracticeInfo2VM
            {
                Name = p.Plan.disciplineTitle,
                SemesterID = t.SemesterID,
                Semester = $"{t.Semester} семестр",
                DisciplineUID = p.Plan.disciplineUUID
            };

            List<PracticeInfo2VM> list = null;
            if (Practices.TryGetValue(p.Plan.additionalType, out list))
            {
                list.Add(practice);
            }
        }

        internal void Add(Practice p)
        {
            var pi = Practices
                .SelectMany(pt => pt.Value)
                .FirstOrDefault(i => i.DisciplineUID == p.DisciplineUUID && i.SemesterID == p.SemesterId);

            if (pi != null)
            {
                pi.SetPractice(p);
                return;
            }
        }
    }

    public class PracticeTypeVM
    {
        public string Name { get; set; }
        public List<PracticeInfo2VM> List { get; set; } = new List<PracticeInfo2VM>();
    }

    public class PracticeInfo2VM
    {
        public string DisciplineUID { get; set; }
        public string Name { get; set; }
        public int SemesterID { get; set; }
        public string Semester { get; set; }
        public int PracticeID { get; set; }
        public string Period { get; set; }

        internal void SetPractice(Practice p)
        {
            PracticeID = p.Id;
            if (p.BeginDate != null || p.EndDate != null)
                Period = $"с {p.BeginDate:dd.MM.yyyy г.} по {p.EndDate:dd.MM.yyyy г.}";
        }
    }

}