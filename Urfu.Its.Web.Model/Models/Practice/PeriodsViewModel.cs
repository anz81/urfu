using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.Practice
{
    public class PeriodsViewModelRow
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; }
        public int rowId { get; set; }
        public string RequestNumber { get; set; }
        public PeriodsViewModelRow(int id, int year, string semester, string requestNumber)
        {
            Id = id;
            Year = year;
            Semester = semester;
            rowId = 0;
            RequestNumber = requestNumber;
        }
    }

    public class PeriodsViewModel
    {
        public List<PeriodsViewModelRow> Rows { get; set; }

        public PeriodsViewModel(List<ContractPeriod> periods)
        {
            Rows = new List<PeriodsViewModelRow>();

            foreach (var period in periods)
            {
                Rows.Add(new PeriodsViewModelRow(period.Id, period.Year, period.Semester.Name, period.RequestNumber));
            }

            //Rows.Add(new PeriodsViewModelRow() { Id = 0, Year = 2017, Semester = "Осенний" });
        }
    }
}
