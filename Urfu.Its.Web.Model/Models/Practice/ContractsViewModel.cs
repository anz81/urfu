using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.Practice
{
    public class ContractStudents
    {
        public Contract Contract { get; set; }
        public List<Student> Students { get; set; }

        public ContractStudents()
        {
        }

        public ContractStudents(Contract contract, List<Student> students)
        {
            Contract = contract;
            Students = students;
        }
    }

    public class ContractViewModelRow
    {
        public int Id { get; set; }
        public string YearSemester { get; set; }
        public string LastSemester { get; set; }
        public string Number { get; set; }
        public string ContractDate { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public List<string> Okso { get; set; }
        public List<string> FullOkso { get; set; }
        public string Profile { get; set; }
        public string FullProfile { get; set; }
        public string Qualification { get; set; }
        public int CountPeople { get; set; }
        public string DirectorCntr { get; set; }
        public string DirectorInitialsCntr { get; set; }
        public string DirectorGenitiveCntr { get; set; }
        public string PostOfDirectorCntr { get; set; }
        public string PostOfDirectorGenitiveCntr { get; set; }
        public string PersonInChargeCntr { get; set; }
        public string PersonInChargeInitialsCntr { get; set; }
        public string PostOfPersonInChargeCntr { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsShortDated { get; set; }
        public bool IsEndless { get; set; }
        public string Comment { get; set; }
        public string PersonalComment { get; set; }
        public int? Limit { get; set; }
        public string FolderNumber { get; set; }
        public string ScanName { get; set; }
        public int? YearKs { get; set; }

        /// <summary>
        /// Индекс строки в таблице при отображении
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Для краткосрочных договоров нужна ссылка на страницу со студентами
        /// </summary>
        public string StudentsStr
        {
            get
            {
                return IsShortDated ? "Студенты" : "";
            }
        }

        // списки нужны для фильтрации
        public List<int> Semester { get; set; }
        public List<int> Year { get; set; }
        public List<string> OksoList { get; set; }
        public List<string> QualificationList { get; set; }
    }

    public class ContractsViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Director { get; set; }
        public string DirectorInitials { get; set; }
        public string DirectorGenitive { get; set; }
        public string PostOfDirector { get; set; }
        public string PostOfDirectorGenitive { get; set; }
        public string PersonInCharge { get; set; }
        public string PersonInChargeInitials { get; set; }
        public string PostOfPersonInCharge { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        
        public ContractsViewModel()
        {

        }

        public ContractsViewModel(int companyId, string companyName, string director, string directorInitials, string directorGenitive,
            string postOfDirector, string postOfDirectorGenitive,
            string personInCharge, string personInChargeInitials, string postOfPersonInCharge, string phoneNumber, string email)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            Director = director;
            DirectorInitials = directorInitials;
            DirectorGenitive = directorGenitive;
            PostOfDirector = postOfDirector;
            PostOfDirectorGenitive = postOfDirectorGenitive;
            PersonInCharge = personInCharge;
            PersonInChargeInitials = personInChargeInitials;
            PostOfPersonInCharge = postOfPersonInCharge;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public IQueryable<ContractViewModelRow> GetRows(List<ContractStudents> contracts)
        {
            var rows = new List<ContractViewModelRow>();

            foreach (var contract in contracts)
            {
                var row = GetRow(contract.Contract, contract.Students);
                rows.Add(row);
            }

            return rows.AsQueryable();
        }

        public IQueryable<ContractViewModelRow> GetRowOfOneContract (ContractStudents contract, int rowIndex)
        {
            var rows = new List<ContractViewModelRow>();

            var row = GetRow(contract.Contract, contract.Students);
            row.RowIndex = rowIndex;
            rows.Add(row);

            return rows.AsQueryable();
        }

        private ContractViewModelRow GetRow(Contract contract, List<Student> studentKs)
        {
            ContractViewModelRow row = new ContractViewModelRow();
            row.Id = contract.Id;

            row.Number = contract.Number ?? "";
            row.DirectorCntr = contract.Director ?? "";
            row.DirectorInitialsCntr = contract.DirectorInitials ?? "";
            row.DirectorGenitiveCntr = contract.DirectorGenitive ?? "";
            row.PostOfDirectorCntr = contract.PostOfDirector ?? "";
            row.PostOfDirectorGenitiveCntr = contract.PostOfDirectorGenitive ?? "";

            row.PersonInChargeCntr = contract.PersonInCharge ?? "";
            row.PersonInChargeInitialsCntr = contract.PersonInChargeInitials ?? "";
            row.PostOfPersonInChargeCntr = contract.PostOfPersonInCharge ?? "";
            row.PhoneNumber = contract.PhoneNumber;
            row.Email = contract.Email;
            row.FolderNumber = contract.FolderNumber.ToString() ?? "";

            row.ContractDate = (contract.ContractDate != null) ? contract.ContractDate.Value.ToShortDateString() : "";
            row.StartDate = (contract.StartDate != null) ? contract.StartDate.Value.ToShortDateString() : "";
            row.FinishDate = (contract.FinishDate != null) ? contract.FinishDate.Value.ToShortDateString() : "";

            row.YearKs = contract.Year;

            if (contract.Periods != null)
            {
                var semestersList = GetSemestersList(contract.Periods);
                row.YearSemester = string.Join(", ", semestersList);
                row.LastSemester = (semestersList.Count() != 0) ? semestersList.Last() : "";
                if (!contract.IsShortDated)
                {
                    var directionList = contract.Periods.Where(p => p.Limits != null).SelectMany(p => p.Limits)
                        .Where(l => l.Direction != null)
                        .GroupBy(l => new {l.Direction.okso, l.Direction.standard})
                        .Select(group => group.First().Direction);
                    row.Okso= directionList.Select(d => d.okso + " (" + d.standard + ")").ToList();
                    row.FullOkso = directionList.Select(d=>d.OksoAndTitle + " (" + d.standard + ")").ToList();
                    row.Profile = GetProfile(contract.Periods);
                    row.FullProfile = GetFullProfile(contract.Periods);
                }
                else
                {
                    var directionList = studentKs.Select(s => s.Group.Profile.Direction)
                        .GroupBy(d => new {d.okso, d.standard}).Select(group=> group.First());
                    row.Okso = directionList.Select(d => d.okso + " (" + d.standard + ")").ToList();
                    row.FullOkso = directionList.Select(d => d.OksoAndTitle + " (" + d.standard + ")").ToList();
                    row.Profile = string.Join(", ", studentKs.Select(s => s.Group.Profile.CODE).Distinct()); 
                    row.FullProfile = string.Join(", ", studentKs.Select(s => s.Group.Profile.OksoAndTitle).Distinct());
                }
                row.Qualification = GetQualifications(contract.Periods);
                row.CountPeople = GetCountPeople(contract.Periods);
            }
            else
            {
                row.YearSemester = "";
                row.LastSemester = "";
                row.Okso = new List<string>();
                row.Qualification = "";
                row.CountPeople = 0;
            }


            row.IsShortDated = contract.IsShortDated;
            row.IsEndless = contract.IsEndless;

            row.Comment = contract.Comment ?? "";
            row.PersonalComment = contract.PersonalComment ?? "";
            row.Limit = contract.Limit;
            row.ScanName = contract.FileStorageId != null ? contract.FileStorage.FileNameForUser : "";

            CreateFilterLists(row, contract.Periods);

            return row;
        }


        private int GetCountPeople(ICollection<ContractPeriod> periods)
        {
            int count = 0;
            foreach (var p in periods)
            {
                foreach (var l in p.Limits)
                {
                    count += l.Limit;
                }
            }
            return count;
        }

        private string GetQualifications(ICollection<ContractPeriod> periods)
        {
            string result = "";
            Dictionary<string, string> qualifications = new Dictionary<string, string>();

            foreach (var p in periods)
            {
                foreach (var l in p.Limits)
                {
                    string course = (l.Course > 0) ? l.Course.ToString() : "";
                    string qualificationName = l.QualificationName ?? "";

                    if (!qualifications.Keys.Contains(qualificationName))
                        qualifications.Add(qualificationName, course != "" ? (course + ", ") : course);
                    else
                    {
                        if (!qualifications[qualificationName].Contains(course))
                            qualifications[qualificationName] += course != "" ? (course + ", ") : course;
                    }
                }
            }
            foreach (var pair in qualifications)
            {
                if (pair.Value != "" || pair.Key != "")
                {
                    string course = RemoveLastComma(pair.Value);

                    if (!string.IsNullOrEmpty(course))
                    {
                        result += pair.Key + " (" + RemoveLastComma(pair.Value) + " курс), ";
                    }
                    else
                    {
                        result += pair.Key + ", ";
                    }
                }
            }

            result = RemoveLastComma(result);

            return result;
        }

        private string GetProfile(ICollection<ContractPeriod> periods)
        {
            string profile = "";

            foreach (var p in periods)
            {
                foreach (var l in p.Limits)
                {
                    if (l.Profile != null)
                    {
                        profile += (!profile.Contains(l.Profile.CODE)) ? l.Profile.CODE + ", " : "";
                    }
                }
            }

            profile = RemoveLastComma(profile);

            return profile;
        }

        private string GetFullProfile(ICollection<ContractPeriod> periods)
        {
            string profile = "";

            foreach (var p in periods)
            {
                foreach (var l in p.Limits)
                {
                    if (l.Profile != null)
                    {
                        profile += (!profile.Contains(l.Profile.CODE)) ? l.Profile.OksoAndTitle + ", " : "";
                    }
                }
            }

            profile = RemoveLastComma(profile);

            return profile;
        }

        private List<string> GetSemestersList(ICollection<ContractPeriod> periods)
        {
            List<string> semestersList = new List<string>();
            if (periods.Count() != 0)
            {
                var years = periods.OrderBy(p => p.Year).Select(p => p.Year).Distinct();
                foreach (var year in years)
                {
                    var semesters = periods.Where(p => p.Year == year).Select(p => p.Semester.Name);
                    if (semesters.Contains("Осенний"))
                        semestersList.Add(year.ToString() + " (Осенний)");
                    if (semesters.Contains("Весенний"))
                        semestersList.Add(year.ToString() + " (Весенний)");
                    if (semesters.Contains("Прочий"))
                        semestersList.Add(year.ToString() + " (Прочий)");
                }
            }

            return semestersList;
        }

        public void CreateFilterLists(ContractViewModelRow row, ICollection<ContractPeriod> periods)
        {
            if (periods != null)
            {
                row.Semester = periods.Select(p => p.SemesterId).Distinct().ToList();
                row.Year = periods.Select(p => p.Year).Distinct().ToList();

                List<string> okso = new List<string>();
                List<string> qualifications = new List<string>();

                foreach (var p in periods)
                {
                    foreach (var l in p.Limits)
                    {
                        if (l.Direction != null)
                            okso.Add(l.Direction.uid);
                        if (!string.IsNullOrEmpty(l.QualificationName))
                            qualifications.Add(l.QualificationName);
                    }
                }
                okso = okso.Distinct().ToList();
                qualifications = qualifications.Distinct().ToList();

                row.OksoList = okso;
                row.QualificationList = qualifications;
            }
            else
            {
                row.Semester = new List<int>();
                row.Year = new List<int>();
                row.OksoList = new List<string>();
                row.QualificationList = new List<string>();
            }
        }
        
        private string RemoveLastComma(string str)
        {
            try
            {
                str = str.Remove(str.Length - 2);
            }
            catch { }

            return str;
        }
    }
}
