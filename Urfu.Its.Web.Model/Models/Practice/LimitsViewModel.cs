using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class LimitsViewModelRow
    {
        public int Id { get; set; }
        public string Okso { get; set; }
        public string Profile { get; set; }
        public string Qualification { get; set; }
        public int Course { get; set; }
        public int Limit { get; set; }
        public string Period { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
        
        public string RequestNumber { get; set; }

        /// <summary>
        /// Количество заявок в статусе "Формируется"
        /// </summary>
        public int AdmissionsIndeterminate { get; set; }

        /// <summary>
        /// Количество заявок в статусе "Согласовано"
        /// </summary>
        public int AdmissionsAdmitted { get; set; }

        /// <summary>
        /// Остаток по лимиту (количество свободных мест)
        /// </summary>
        public int CurrentLimit { get { return Limit - AdmissionsAdmitted; } }
    }

    public class LimitsViewModel
    {
        public IQueryable<LimitsViewModelRow> Rows { get; set; }

        public LimitsViewModel(Contract contract, List<PracticeAdmissionCompany> admissions)
        {
            var rows = new List<LimitsViewModelRow>();
            
            if (contract.Periods != null)
            {
                var periods = contract.Periods.OrderBy(p => p.Year).ThenBy(d => d.SemesterId);
                foreach (var p in periods)
                {
                    if (p.Limits != null)
                    {
                        foreach (var l in p.Limits)
                        {
                            var row = new LimitsViewModelRow();                           
                            row.Id = l.Id;
                            row.Okso = (l.Direction != null) ? ((l.Direction.okso ?? "") + ' ' + (l.Direction.title ?? "")) + 
                                ' ' + "("+ l.Direction.standard + ")": "Не указано";
                            row.Profile = (l.Profile != null) ? (l.Profile.CODE + " " + l.Profile.NAME) : "Не указано";
                            row.Qualification = l.QualificationName ?? "Не указано";
                            row.Period = l.Period.Year + " (" + l.Period.Semester.Name + ")";
                            row.Course = l.Course;
                            row.Limit = l.Limit;
                            row.Semester = l.Period.SemesterId;
                            row.Year = l.Period.Year;
                            row.RequestNumber = l.Period.RequestNumber;

                            var limitAdmissions = admissions.Where(a =>
                                a.Practice.Year == l.Period.Year
                                && a.Practice.SemesterId == l.Period.SemesterId
                                && (l.Profile == null || a.Practice.Group.ProfileId == l.ProfileId)
                                && (l.Direction == null || a.Practice.Group.Profile.DIRECTION_ID == l.DirectionId)
                                && (l.Course == 0 || l.Course == a.Practice.Group.Course)
                                && (l.Qualification == null || l.QualificationName == a.Practice.Group.Qual));
                            
                            row.AdmissionsAdmitted = limitAdmissions.Where(a => a.Status == AdmissionStatus.Admitted).GroupBy(a => a.Practice.StudentId).Count();
                            row.AdmissionsIndeterminate = limitAdmissions.Where(a => a.Status == AdmissionStatus.Indeterminate).GroupBy(a => a.Practice.StudentId).Count();

                            rows.Add(row);
                        }
                    }
                }
            }

            Rows = rows.AsQueryable();
        }
    }
}
