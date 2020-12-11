using System;
using System.Collections.Generic;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{

    public class VariantDeleteVM
    {
        public Variant Variant { get; }
        public List<VariantGroup> VariantGroups { get; }
        public List<VariantSelectionGroup> VariantSelectionGroups { get; }
        public List<EduProgramLimit> EduProgramLimits { get; }
        public List<PlanTeacherVM> PlanTeachers { get; }
        public Int32 VariantAdmissionsCount { get; }
        public int StudentVariantSelectionsCount { get; }
        public int StudentSelectionPriorityCount { get; }
        public VariantDeleteVM(Variant variant, List<VariantGroup> variantGroups, List<VariantSelectionGroup> variantSelectionGroups, List<EduProgramLimit> eduProgramLimits, List<PlanTeacherVM> planTeachers,int variantAdmissionsCount, int studentVariantSelectionsCount, int studentSelectionPriorityCount)
        {
            Variant = variant;
            VariantGroups = variantGroups;
            VariantSelectionGroups = variantSelectionGroups;
            EduProgramLimits = eduProgramLimits;
            PlanTeachers = planTeachers;
            VariantAdmissionsCount = variantAdmissionsCount;
            StudentVariantSelectionsCount = studentVariantSelectionsCount;
            StudentSelectionPriorityCount = studentSelectionPriorityCount;
        }

        public bool CanDelete()
        {
            return /*VariantGroups.Count == 0 && VariantSelectionGroups.Count == 0
                   && EduProgramLimits.Count == 0 && PlanTeachers.Count == 0
                   && */VariantAdmissionsCount == 0 /*&& StudentVariantSelectionsCount == 0
                   && StudentSelectionPriorityCount == 0*/;
        }
    }
    public class PlanTeacherVM
    {
        public Module Module { get; set; }
        public Teacher Teacher { get; set; }
        public string catalogDisciplineUuid { get; set; }
        public Discipline discipline { get; set; }

        public PlanTeacherVM()
        {
            
        }
        public PlanTeacherVM(Module module, Teacher teacher, string catalogDisciplineUuid, Discipline discipline)
        {
            Module = module;
            Teacher = teacher;
            this.catalogDisciplineUuid = catalogDisciplineUuid;
            this.discipline = discipline;
        }
    }




}
