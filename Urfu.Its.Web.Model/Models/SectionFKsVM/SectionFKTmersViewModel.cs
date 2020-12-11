using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class SectionFKTmersRowViewModel
    {
        public SectionFKTmersRowViewModel()
        {
        }

        [Key, Required]
        public string TmerId { get; set; }

        public bool Checked { get; set; }

        public string Title { get; set; }
     
    }

    public class SectionFKTmersViewModel
    {
        public SectionFKTmersViewModel()
        {
        }

        public SectionFKTmersViewModel(SectionFK sectionFK, Discipline discipline, List<Tmer> tmers)
        {
            SectionFK = sectionFK;
            Discipline = discipline;
            var md = sectionFK.Disciplines.Where(d => d.DisciplineUid == discipline.uid).FirstOrDefault();
            var selectedIds = new HashSet<string>(md?.Tmers?.Select(t => t.TmerId) ?? Enumerable.Empty<string>());

            SectionFKDiscipline = md;

            Tmers1 = SetTmers(1, tmers, selectedIds);
            Tmers2 = SetTmers(2, tmers, selectedIds);
            Tmers3 = SetTmers(3, tmers, selectedIds);

            var max = (new []{ Tmers1.Count, Tmers2.Count, Tmers3.Count }).Max();
            AddToMax(Tmers1, max);
            AddToMax(Tmers2, max);
            AddToMax(Tmers3, max);
        }

        private void AddToMax(List<SectionFKTmersRowViewModel> tmers, int max)
        {
            while (tmers.Count < max)
            {
                tmers.Add(new SectionFKTmersRowViewModel());
            }
        }

        private List<SectionFKTmersRowViewModel> SetTmers(int kgmer, List<Tmer> tmers, HashSet<string> selectedIds)
        {
            var tmers1 = new List<SectionFKTmersRowViewModel>();
            foreach (var t in tmers.Where(m => m.kgmer == kgmer))
            {
                tmers1.Add(new SectionFKTmersRowViewModel
                {
                    Checked = selectedIds.Contains(t.kmer),
                    TmerId = t.kmer,
                    Title = t.rmer
                });
            }

            return tmers1;
        }

        public SectionFK SectionFK { get; set; }

        public Discipline Discipline { get; set; }

        public SectionFKDiscipline SectionFKDiscipline { get; set; }

        [DisplayName("Aудиторная нагрузка")]
        public List<SectionFKTmersRowViewModel> Tmers1 { get; set; }

        [DisplayName("Контрольные мероприятия")]
        public List<SectionFKTmersRowViewModel> Tmers2 { get; set; }

        [DisplayName("Формы контроля")]
        public List<SectionFKTmersRowViewModel> Tmers3 { get; set; }
    }

    public class ForeignLanguageTmersRowViewModel
    {
        public ForeignLanguageTmersRowViewModel()
        {
        }

        [Key, Required]
        public string TmerId { get; set; }

        public bool Checked { get; set; }

        public string Title { get; set; }
     
    }

    public class ForeignLanguageTmersViewModel
    {
        public ForeignLanguageTmersViewModel()
        {
        }

        public ForeignLanguageTmersViewModel(ForeignLanguage sectionFK, Discipline discipline, List<Tmer> tmers)
        {
            ForeignLanguage = sectionFK;
            Discipline = discipline;
            var md = sectionFK.Disciplines.Where(d => d.DisciplineUid == discipline.uid).FirstOrDefault();
            var selectedIds = new HashSet<string>(md?.Tmers?.Select(t => t.TmerId) ?? Enumerable.Empty<string>());

            ForeignLanguageDiscipline = md;

            Tmers1 = SetTmers(1, tmers, selectedIds);
            Tmers2 = SetTmers(2, tmers, selectedIds);
            Tmers3 = SetTmers(3, tmers, selectedIds);

            var max = (new []{ Tmers1.Count, Tmers2.Count, Tmers3.Count }).Max();
            AddToMax(Tmers1, max);
            AddToMax(Tmers2, max);
            AddToMax(Tmers3, max);
        }

        private void AddToMax(List<ForeignLanguageTmersRowViewModel> tmers, int max)
        {
            while (tmers.Count < max)
            {
                tmers.Add(new ForeignLanguageTmersRowViewModel());
            }
        }

        private List<ForeignLanguageTmersRowViewModel> SetTmers(int kgmer, List<Tmer> tmers, HashSet<string> selectedIds)
        {
            var tmers1 = new List<ForeignLanguageTmersRowViewModel>();
            foreach (var t in tmers.Where(m => m.kgmer == kgmer))
            {
                tmers1.Add(new ForeignLanguageTmersRowViewModel
                {
                    Checked = selectedIds.Contains(t.kmer),
                    TmerId = t.kmer,
                    Title = t.rmer
                });
            }

            return tmers1;
        }

        public ForeignLanguage ForeignLanguage { get; set; }

        public Discipline Discipline { get; set; }

        public ForeignLanguageDiscipline ForeignLanguageDiscipline { get; set; }

        [DisplayName("Aудиторная нагрузка")]
        public List<ForeignLanguageTmersRowViewModel> Tmers1 { get; set; }

        [DisplayName("Контрольные мероприятия")]
        public List<ForeignLanguageTmersRowViewModel> Tmers2 { get; set; }

        [DisplayName("Формы контроля")]
        public List<ForeignLanguageTmersRowViewModel> Tmers3 { get; set; }
    }

    public class ProjectTmersRowViewModel
    {
        public ProjectTmersRowViewModel()
        {
        }

        [Key, Required]
        public string TmerId { get; set; }

        public bool Checked { get; set; }

        public string Title { get; set; }

    }

    public class ProjectTmersViewModel
    {
        public ProjectTmersViewModel()
        {
        }

        public ProjectTmersViewModel(Project project, Discipline discipline, List<Tmer> tmers)
        {
            Project = project;
            Discipline = discipline;
            var md = project.Disciplines.Where(d => d.DisciplineUid == discipline.uid).FirstOrDefault();
            var selectedIds = new HashSet<string>(md?.Tmers?.Select(t => t.TmerId) ?? Enumerable.Empty<string>());

            ProjectDiscipline = md;

            Tmers1 = SetTmers(1, tmers, selectedIds);
            Tmers2 = SetTmers(2, tmers, selectedIds);
            Tmers3 = SetTmers(3, tmers, selectedIds);

            var max = (new[] { Tmers1.Count, Tmers2.Count, Tmers3.Count }).Max();
            AddToMax(Tmers1, max);
            AddToMax(Tmers2, max);
            AddToMax(Tmers3, max);
        }

        private void AddToMax(List<ProjectTmersRowViewModel> tmers, int max)
        {
            while (tmers.Count < max)
            {
                tmers.Add(new ProjectTmersRowViewModel());
            }
        }

        private List<ProjectTmersRowViewModel> SetTmers(int kgmer, List<Tmer> tmers, HashSet<string> selectedIds)
        {
            var tmers1 = new List<ProjectTmersRowViewModel>();
            foreach (var t in tmers.Where(m => m.kgmer == kgmer))
            {
                tmers1.Add(new ProjectTmersRowViewModel
                {
                    Checked = selectedIds.Contains(t.kmer),
                    TmerId = t.kmer,
                    Title = t.rmer
                });
            }

            return tmers1;
        }

        public Project Project { get; set; }

        public Discipline Discipline { get; set; }

        public ProjectDiscipline ProjectDiscipline { get; set; }

        [DisplayName("Aудиторная нагрузка")]
        public List<ProjectTmersRowViewModel> Tmers1 { get; set; }

        [DisplayName("Контрольные мероприятия")]
        public List<ProjectTmersRowViewModel> Tmers2 { get; set; }

        [DisplayName("Формы контроля")]
        public List<ProjectTmersRowViewModel> Tmers3 { get; set; }
    }

    public class MUPTmersRowViewModel
    {
        public MUPTmersRowViewModel()
        {
        }

        [Key, Required]
        public string TmerId { get; set; }

        public bool Checked { get; set; }

        public string Title { get; set; }

    }

    public class MUPTmersViewModel
    {
        public MUPTmersViewModel()
        {
        }

        public MUPTmersViewModel(MUP mup, Discipline discipline, List<Tmer> tmers)
        {
            MUP = mup;
            Discipline = discipline;
            var md = mup.Disciplines.Where(d => d.DisciplineUid == discipline.uid).FirstOrDefault();
            var selectedIds = new HashSet<string>(md?.Tmers?.Select(t => t.TmerId) ?? Enumerable.Empty<string>());

            MUPDiscipline = md;

            Tmers1 = SetTmers(1, tmers, selectedIds);
            Tmers2 = SetTmers(2, tmers, selectedIds);
            Tmers3 = SetTmers(3, tmers, selectedIds);

            var max = (new[] { Tmers1.Count, Tmers2.Count, Tmers3.Count }).Max();
            AddToMax(Tmers1, max);
            AddToMax(Tmers2, max);
            AddToMax(Tmers3, max);
        }

        private void AddToMax(List<MUPTmersRowViewModel> tmers, int max)
        {
            while (tmers.Count < max)
            {
                tmers.Add(new MUPTmersRowViewModel());
            }
        }

        private List<MUPTmersRowViewModel> SetTmers(int kgmer, List<Tmer> tmers, HashSet<string> selectedIds)
        {
            var tmers1 = new List<MUPTmersRowViewModel>();
            foreach (var t in tmers.Where(m => m.kgmer == kgmer))
            {
                tmers1.Add(new MUPTmersRowViewModel
                {
                    Checked = selectedIds.Contains(t.kmer),
                    TmerId = t.kmer,
                    Title = t.rmer
                });
            }

            return tmers1;
        }

        public MUP MUP { get; set; }

        public Discipline Discipline { get; set; }

        public MUPDiscipline MUPDiscipline { get; set; }

        [DisplayName("Aудиторная нагрузка")]
        public List<MUPTmersRowViewModel> Tmers1 { get; set; }

        [DisplayName("Контрольные мероприятия")]
        public List<MUPTmersRowViewModel> Tmers2 { get; set; }

        [DisplayName("Формы контроля")]
        public List<MUPTmersRowViewModel> Tmers3 { get; set; }
    }

}