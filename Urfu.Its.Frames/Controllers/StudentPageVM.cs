using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Protocols;
using Dapper;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Frames.Controllers
{
    public class StudentPageVM
    {
        public string StudentId { get; set; }
        readonly ApplicationDbContext _db;

        public bool IsEmpty { get; set; }
        public bool GroupHistoryNotFound { get; set; }

        public List<SemesterSelectionVM> Semesters = new List<SemesterSelectionVM>();

        public string BeginDateStr { get; set; }
        public string EndDateStr { get; set; }

        public bool IsAdmittedOFK { get; set; }

        public StudentPageVM(ApplicationDbContext db, string studentId)
        {
            _db = db;
            StudentId = studentId;
            IsEmpty = true;
            if (studentId == null)
                return;

            var info = _db.Students.Where(s => s.Id == studentId).Select(s => new { s.Person.Name, s.Person.Surname, s.Person.PatronymicName, GroupName = s.Group.Name, s.Sportsman, s.Male }).FirstOrDefault();
            if (info == null)
            {
                IsEmpty = true;
                return;
            }

            Name = info.Name;
            Surname = info.Surname;
            PatronymicName = info.PatronymicName;
            GroupName = info.GroupName;

            if (info.Sportsman)
            {
                IsSportsmen = info.Sportsman;
            }
            var studentGroupId = _db.Students.FirstOrDefault(s => s.Id == studentId).GroupId;
            var studentHistoryGroup = db.GroupsHistories.Where(g => g.GroupId == studentGroupId).OrderByDescending(g => g.YearHistory).FirstOrDefault();
            if (studentHistoryGroup == null)
            {
                GroupHistoryNotFound = true;
                return;
            }

            var _allGroups = _db.SectionFKCompetitionGroups
                .Where(kg => kg.Year == studentHistoryGroup.YearHistory && kg.Groups.Any(g => g.Id == studentGroupId))
                .SelectMany(kg =>
                        _db.SectionFKs
                            .Where(
                                s =>
                                        s.ShowInLC
                            )
                            .Select(s => new {
                                sectionFK = s,
                                periods = _db.SectionFKPeriods.Where(p => p.Year == kg.Year && p.SemesterId == kg.SemesterId
                                    && (p.Course == null || p.Course == studentHistoryGroup.Course)
                                    && p.SectionFKId == s.ModuleId && (!p.Male.HasValue || p.Male == info.Male)).ToList()
                            })
                            .Where(s => s.periods.Count > 0)
                            .Select(s => new {
                                kg.Id,
                                kg.StudentCourse,
                                s.sectionFK.ModuleId,
                                s.sectionFK.WithoutPriorities,
                                s.sectionFK.Module.shortTitle,
                                isOFK = s.sectionFK.Module.title == "Оздоровительная физическая культура",
                                kg.Year,
                                kg.SemesterId,
                                SelectionBeginMin = s.periods.Where(p => p.SelectionBegin.HasValue).Min(p => p.SelectionBegin),
                                SelectionDeadline = s.periods.Where(p => p.SelectionDeadline.HasValue).Min(p => p.SelectionDeadline),
                                SelectionDeadlineMax = s.periods.Where(p => p.SelectionDeadline.HasValue).Max(p => p.SelectionDeadline),
                                SemesterName = kg.Semester.Name,
                                Admission = _db.SectionFKAdmissions.FirstOrDefault(a => a.studentId == studentId && a.SectionFKCompetitionGroupId == kg.Id
                                    && a.SectionFKId == s.sectionFK.ModuleId),
                                Limit = (int?)_db.SectionFKProperties.FirstOrDefault(p => p.SectionFKId == s.sectionFK.ModuleId && p.SectionFKCompetitionGroupId == kg.Id).Limit,
                                Admitted = _db.SectionFKAdmissions.Count(a => a.SectionFKId == s.sectionFK.ModuleId && a.Status == AdmissionStatus.Admitted
                                    && a.SectionFKCompetitionGroupId == kg.Id && !a.Student.Sportsman && (a.Student.Status == "Активный" || a.Student.Status == "Отп.с.посещ."))
                            })

                ).ToList();

            var isAdmitted = _allGroups.Where(g => g.Admission != null).LastOrDefault(g => g.Admission.SectionFKCompetitionGroup.Groups.Select(gr => gr.Id).Contains(studentGroupId)
                                        && g.Admission?.Status == AdmissionStatus.Admitted);

            var allGroups = _allGroups.Select(g => new
            {
                g,
                _firstChoice = g.StudentCourse == 1 && g.SemesterId == 1,
                _priority =
                    _db.SectionFKStudentSelectionPriorities.FirstOrDefault(
                        p =>
                            p.studentId == studentId && p.competitionGroupId == g.Id &&
                            p.sectionId == g.ModuleId)
            }).Select(g => new {
                g,
                // первый приоритет (priority) показывать если:
                // 1) студент еще не зачислен
                // 2) студент зачислен, последняя дата выбора секции прошла, секция не перевыбрана
                FirstChoice = g._firstChoice && isAdmitted == null ||
                    g._firstChoice && isAdmitted != null && DateTime.Now > g.g.SelectionDeadlineMax && g._priority?.changePriority == null,
            })
            .Select(g => new
            {
                g,
                priority = g.FirstChoice
                    ?
                    (int?)g.g._priority?.priority
                    : (int?)g.g._priority?.changePriority,

                Editable = isAdmitted?.Admission?.Published == false ? false : true,
                Status = g.g.g.Admission?.Published == true ? g.g.g.Admission?.Status : null
            }).ToList();

            // студенты, зачисленные на ОФК не могут перевыбирать секцию
            IsAdmittedOFK = isAdmitted != null ? (bool)isAdmitted?.isOFK : false;

            var OFKPriority = allGroups.FirstOrDefault(g => g.g.g.g.isOFK == true)?.priority != null;

            BeginDateStr = allGroups.Select(s => s.g.g.g.SelectionBeginMin).Min()?.ToShortDateString() ?? "";
            EndDateStr = allGroups.Select(s => s.g.g.g.SelectionDeadlineMax).Max()?.ToShortDateString() ?? "";

            var groups = allGroups.Where(r =>
                (r.g.g.g.SelectionBeginMin == null || r.g.g.g.SelectionBeginMin <= DateTime.Now)
                && (IsAdmittedOFK && r.g.g.g.isOFK || !IsAdmittedOFK) // студенты, зачисленные на ОФК видят только ОФК
                )
                .Select(g => new
                {
                    g.g.g.g.Id,
                    g.g.g.g.StudentCourse,
                    g.g.g.g.shortTitle,
                    g.g.g.g.SemesterName,
                    g.g.g.g.SemesterId,
                    g.g.g.g.Year,
                    g.g.g.g.SelectionDeadline,
                    g.g.g.g.SelectionDeadlineMax,
                    g.g.g.g.ModuleId,
                    g.priority,
                    g.Status,
                    g.g.FirstChoice,
                    g.g.g.g.Limit,
                    g.g.g.g.Admitted,
                    g.g.g.g.WithoutPriorities,
                    g.g.g.g.isOFK
                })
                .OrderBy(s => s.Year).ThenBy(s => s.SemesterId).ThenBy(s => s.Id).ThenBy(s => s.shortTitle)
                .ToList();



            int? id = null;
            SemesterSelectionVM vm = null;

            foreach (var row in groups)
            {
                if (id != row.Id)
                {
                    var deadlineMax = groups.Where(g => g.Id == row.Id).Select(g => g.SelectionDeadlineMax).Max();
                    Semesters.Add(vm = new SemesterSelectionVM(row.Id, row.SemesterName, row.Year, deadlineMax,
                        firstChoice: row.StudentCourse == 1 && row.SemesterId == 1 && isAdmitted == null));
                    id = row.Id;
                }
                vm.Rows.Add(new SemesterPriorityVM(row.ModuleId, row.shortTitle, row.priority, row.Status, row.Limit, row.Admitted,
                    row.WithoutPriorities, row.SelectionDeadline,
                    editable: (isAdmitted != null && !IsAdmittedOFK) // если студент зачислен НЕ на ОФК, то перевыбор возможен
                        ? true
                        : (row.isOFK ? true : !OFKPriority))); // если приоритет стоит на ОФК, то остальные секции не доступны для перевыбора
            }

            IsEmpty = false;
        }

        public bool IsSportsmen { get; set; }

        public string GroupName { get; set; }

        public string PatronymicName { get; set; }

        public string Surname { get; set; }

        public string Name { get; set; }
    }

    public class SemesterSelectionVM
    {
        private readonly DateTime? _selectionDeadline;
        private readonly bool _firstChoice;

        public DateTime? SelectionDeadline
        {
            get { return _selectionDeadline; }
        }

        public SemesterSelectionVM(int id, string semesterName, int year, DateTime? selectionDeadline, bool firstChoice)
        {
            _selectionDeadline = selectionDeadline;
            _firstChoice = firstChoice;
            Id = id;
            SemesterName = semesterName;
            Year = year;
            Rows = new List<SemesterPriorityVM>();
        }

        public int Id { get; set; }
        public string SemesterName { get; set; }
        public int Year { get; set; }

        public List<SemesterPriorityVM> Rows { get; set; }

        public int MaxPrioroty
        {
            get
            {
                if (_firstChoice)
                {
                    return Rows.Count;
                }
                return 1;
            }
        }

        public int MinimumPriorities
        {
            get
            {
                if (!_firstChoice)
                    return 0;
                return Math.Min(Rows.Count, 10);
            }
        }

        public string PriorityWording
        {
            get
            {
                var i = MinimumPriorities % 10;
                if (i == 1)
                    return "приоритет";

                if (i < 5 && i > 1)
                    return "приоритета";

                return "приоритетов";
            }
        }
    }

    public class SemesterPriorityVM
    {
        private readonly int _admitted;
        public int? Priority { get; }

        public string ModuleId { get; }

        public string ShortTitle { get; }

        public AdmissionStatus? Status { get; set; }

        public bool Editable { get; set; }

        public SemesterPriorityVM(string moduleId, string shortTitle, int? priority, AdmissionStatus? status, int? limit, int admitted,
            bool withoutPriorities, DateTime? deadline, bool editable)
        {
            _admitted = admitted;
            Priority = priority;
            Limit = limit;
            WithoutPriorities = withoutPriorities;
            ModuleId = moduleId;
            ShortTitle = shortTitle;
            Status = status;
            Editable = deadline >= DateTime.Now && editable;
        }

        public int? Limit { get; }
        public bool WithoutPriorities { get; set; }

        public int Admitted
        {
            get { return _admitted; }
        }

        public int? PlacesAvailable => Limit - Admitted;
    }
}