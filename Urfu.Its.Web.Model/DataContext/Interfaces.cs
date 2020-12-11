using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Urfu.Its.Web.DataContext
{
    public interface IDisciplineTmerPeriod
    {
        int Id { get; set; }
        ICollection<Division> Divisions { get; set; }
    }

    public interface ICompetitionGroup
    {
        ICollection<Group> Groups { get; set; }
    }
}