using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class MinorTmersRowViewModel
    {
        public MinorTmersRowViewModel()
        {
        }

        [Key, Required]
        public string TmerId { get; set; }

        public bool Checked { get; set; }

        public string Title { get; set; }
     
    }

    public class MinorTmersViewModel
    {
        public MinorTmersViewModel()
        {
        }

        public MinorTmersViewModel(Minor minor, Discipline discipline, List<Tmer> tmers)
        {
            Minor = minor;
            Discipline = discipline;
            var md = minor.Disciplines.Where(d => d.DisciplineUid == discipline.uid).FirstOrDefault();
            var selectedIds = new HashSet<string>(md?.Tmers?.Select(t => t.TmerId) ?? Enumerable.Empty<string>());

            MinorDiscipline = md;

            Tmers1 = SetTmers(1, tmers, selectedIds);
            Tmers2 = SetTmers(2, tmers, selectedIds);
            Tmers3 = SetTmers(3, tmers, selectedIds);

            var max = (new []{ Tmers1.Count, Tmers2.Count, Tmers3.Count }).Max();
            AddToMax(Tmers1, max);
            AddToMax(Tmers2, max);
            AddToMax(Tmers3, max);
        }

        private void AddToMax(List<MinorTmersRowViewModel> tmers, int max)
        {
            while (tmers.Count < max)
            {
                tmers.Add(new MinorTmersRowViewModel());
            }
        }

        private List<MinorTmersRowViewModel> SetTmers(int kgmer, List<Tmer> tmers, HashSet<string> selectedIds)
        {
            var tmers1 = new List<MinorTmersRowViewModel>();
            foreach (var t in tmers.Where(m => m.kgmer == kgmer))
            {
                tmers1.Add(new MinorTmersRowViewModel
                {
                    Checked = selectedIds.Contains(t.kmer),
                    TmerId = t.kmer,
                    Title = t.rmer
                });
            }

            return tmers1;
        }

        public Minor Minor { get; set; }

        public Discipline Discipline { get; set; }

        public MinorDiscipline MinorDiscipline { get; set; }

        [DisplayName("Aудиторная нагрузка")]
        public List<MinorTmersRowViewModel> Tmers1 { get; set; }

        [DisplayName("Контрольные мероприятия")]
        public List<MinorTmersRowViewModel> Tmers2 { get; set; }

        [DisplayName("Формы контроля")]
        public List<MinorTmersRowViewModel> Tmers3 { get; set; }
    }
}