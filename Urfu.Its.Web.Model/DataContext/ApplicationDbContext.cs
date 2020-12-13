using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.Web.Models;
using static AutoMapper.QueryableExtensions.LetPropertyMaps;

// ReSharper disable InconsistentNaming

namespace Urfu.Its.Web.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
#if DEBUG
            //if (System.Diagnostics.Debugger.IsAttached)
            //Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

#endif
        }

        public ApplicationDbContext()
        {
        }

        /*public ApplicationDbContext(string connectionStringName)
  : base(connectionStringName, throwIfV1Schema: false)
{
#if DEBUG
   //if (System.Diagnostics.Debugger.IsAttached)
   //Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

#endif
}*/
public static ApplicationDbContext Create()
{

   return new ApplicationDbContext();
}/*
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
   base.OnModelCreating(modelBuilder);
   modelBuilder.Entity<ApplicationUser>(entity =>
   {
       entity.Property(e => e.UserName)
           .IsRequired()
           .HasMaxLength(256);
   });
}
*/
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanAdditional> PlanAdditionals { get; set; }
        public DbSet<PlanTerm> PlanTerms { get; set; }
        public DbSet<PlanDisciplineTerm> PlanDisciplineTerms { get; set; }

        public DbSet<Division> Divisions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupsHistory> GroupsHistories { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<EduProgram> EduPrograms { get; set; }
        public DbSet<VariantContent> VariantContents { get; set; }
        public DbSet<VariantModuleType> VariantModuleTypes { get; set; }
        public DbSet<VariantGroup> VariantGroups { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<VariantSelectionGroup> VariantSelectionGroups { get; set; }
        public DbSet<UserDirection> UserDirections { get; set; }
        public DbSet<UserDivision> UserDivisions { get; set; }
        public DbSet<UserMinor> UserMinors { get; set; }

        public DbSet<FamilirizationType> FamilirizationTypes { get; set; }
        public DbSet<FamilirizationTech> FamilirizationTechs { get; set; }
        public DbSet<FamilirizationCondition> FamilirizationConditions { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        //public DbSet<VariantLimit> VariantLimits { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<PlanTeacher> PlanTeachers { get; set; }
        public DbSet<VariantAdmission> VariantAdmissions { get; set; }
        public DbSet<ModuleAdmission> ModuleAdmissions { get; set; }

        public DbSet<StudentVariantSelection> StudentVariantSelections { get; set; }
        public DbSet<StudentSelectionTeacher> StudentSelectionTeachers { get; set; }
        public DbSet<StudentSelectionPriority> StudentSelectionPriority { get; set; }
        public DbSet<StudentSelectionMinorPriority> StudentSelectionMinorPriority { get; set; }

        public DbSet<EduProgramLimit> EduProgramLimits { get; set; }

        public DbSet<Subgroup> Subgroups { get; set; }
        public DbSet<MetaSubgroup> MetaSubgroups { get; set; }
        public DbSet<SubgroupMembership> SubgroupMemberships { get; set; }

        public DbSet<Appload> Apploads { get; set; }
        public DbSet<Tmer> Tmers { get; set; }

        public DbSet<RoleSet> RoleSets { get; set; }

        public DbSet<Semester> Semesters { get; set; }

        public DbSet<ModuleTech> ModuleTeches { get; set; }

        public DbSet<Minor> Minors { get; set; }
        public DbSet<MinorPeriod> MinorPeriods { get; set; }
        public DbSet<MinorDiscipline> MinorDisciplines { get; set; }
        public DbSet<MinorDisciplineTmer> MinorTmers { get; set; }
        public DbSet<MinorDisciplineTmerPeriod> MinorTmerPeriods { get; set; }

        public DbSet<MinorAdmission> MinorAdmissions { get; set; }
        public DbSet<MinorAutoAdmissionReport> MinorAutoAdmissionReports { get; set; }

        public DbSet<SectionFKAutoMoveReport> SectionFKAutoMoveReports { get; set; }

        public DbSet<MinorSubgroup> MinorSubgroups { get; set; }
        public DbSet<MinorSubgroupMembership> MinorSubgroupMemberships { get; set; }
        public DbSet<SectionFK> SectionFKs { get; set; }

        public DbSet<SectionFKPeriod> SectionFKPeriods { get; set; }
        public DbSet<SectionFKDiscipline> SectionFKDisciplines { get; set; }
        public DbSet<SectionFKDisciplineTmer> SectionFKTmers { get; set; }
        public DbSet<SectionFKDisciplineTmerPeriod> SectionFKTmerPeriods { get; set; }

        public DbSet<SectionFKAdmission> SectionFKAdmissions { get; set; }
        public DbSet<SectionFKSubgroup> SectionFKSubgroups { get; set; }
        public DbSet<SectionFKSubgroupMembership> SectionFKSubgroupMemberships { get; set; }
        public DbSet<SectionFKCompetitionGroup> SectionFKCompetitionGroups { get; set; }
        public DbSet<SectionFKProperty> SectionFKProperties { get; set; }
        public DbSet<SectionFKSubgroupCount> SectionFKSubgroupCounts { get; set; }

        public DbSet<SectionFKStudentSelectionPriority> SectionFKStudentSelectionPriorities { get; set; }


        public DbSet<FirstTrainingPlaceFK> FirstTrainingPlaceFKs { get; set; }

        public DbSet<ForeignLanguage> ForeignLanguages { get; set; }

        public DbSet<ForeignLanguagePeriod> ForeignLanguagePeriods { get; set; }
        public DbSet<ForeignLanguageDiscipline> ForeignLanguageDisciplines { get; set; }
        public DbSet<ForeignLanguageDisciplineTmer> ForeignLanguageTmers { get; set; }
        public DbSet<ForeignLanguageDisciplineTmerPeriod> ForeignLanguageTmerPeriods { get; set; }

        public DbSet<ForeignLanguageAdmission> ForeignLanguageAdmissions { get; set; }
        public DbSet<ForeignLanguageSubgroup> ForeignLanguageSubgroups { get; set; }
        public DbSet<ForeignLanguageSubgroupMembership> ForeignLanguageSubgroupMemberships { get; set; }
        public DbSet<ForeignLanguageCompetitionGroup> ForeignLanguageCompetitionGroups { get; set; }
        public DbSet<ForeignLanguageProperty> ForeignLanguageProperties { get; set; }
        public DbSet<ForeignLanguageSubgroupCount> ForeignLanguageSubgroupCounts { get; set; }
        public DbSet<ForeignLanguageStudentSelectionPriority> ForeignLanguageStudentSelectionPriorities { get; set; }
        public DbSet<StudentPlan> StudentPlans { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyLocation> CompanyLocations { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractLimit> ContractLimits { get; set; }
        public DbSet<ContractPeriod> ContractPeriods { get; set; }
        public DbSet<Competence> Competences { get; set; }
        public DbSet<CompetenceType> CompetenceTypes { get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<EduResult> EduResults { get; set; }
        public DbSet<PracticeTeacher> PracticeTeachers { get; set; }
        public DbSet<PracticeTheme> PracticeThemes { get; set; }
        public DbSet<Practice> Practices { get; set; }
        public DbSet<PracticeAdmission> PracticeAdmissions { get; set; }
        public DbSet<PracticeAdmissionCompany> PracticeAdmissionCompanys { get; set; }
        public DbSet<PracticeDocument> PracticeDocuments { get; set; }
        public DbSet<PracticeTime> PracticeTimes { get; set; }
        public DbSet<PracticeWay> PracticeWays { get; set; }
        public DbSet<PracticeInfo> PracticeInfo { get; set; }
        public DbSet<PracticeDecree> PracticeDecrees { get; set; }
        public DbSet<PracticeDecreeNumber> PracticeDecreeNumbers { get; set; }
        public DbSet<LettersOfAttorney> LettersOfAttorney { get; set; }
        public DbSet<PracticeAgreement> PracticeAgreements { get; set; }
         public DbSet<VersionedDocumentTemplate> VersionedDocumentTemplates { get; set; }
        public DbSet<VersionedDocument> VersionedDocuments { get; set; }
        public DbSet<VersionedDocumentBlockLink> VersionedDocumentBlockLinks { get; set; }
        public DbSet<VersionedDocumentBlock> VersionedDocumentBlocks { get; set; }
        public DbSet<ModuleWorkingProgram> ModuleWorkingPrograms { get; set; }
        public DbSet<DisciplineWorkingProgram> DisciplineWorkingPrograms { get; set; }
        public DbSet<WorkingProgramAuthor> WorkingProgramAuthors { get; set; }
        public DbSet<RequisiteOrderFGOS> RequisiteOrderFgoss { get; set; }
        public DbSet<PlanTermWeek> PlanTermWeeks { get; set; }
        public DbSet<KindAction> KindActions { get; set; }
        public DbSet<OwnershipTypes> OwnershipTypes { get; set; }
        public DbSet<UPOPStatus> UpopStatuses { get; set; }

        public DbSet<ModuleWorkingProgramChangeList> ModuleWorkingProgramChangeLists { get; set; }

        public DbSet<WorkingProgramResponsiblePerson> WorkingProgramResponsiblePersons { get; set; }

        public DbSet<Director> Directors { get; set; }

        public DbSet<PracticeChangedDecree> PracticeChangedDecrees { get; set; }
        public DbSet<PracticeChangedDecreeReason> PracticeChangedDecreeReasons { get; set; }
        public DbSet<PracticeChangedDecreeStudent> PracticeChangedDecreeStudents { get; set; }

        public DbSet<ProjectAutoMoveReport> ProjectAutoMoveReports { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectPeriod> ProjectPeriods { get; set; }
        public DbSet<ProjectDiscipline> ProjectDisciplines { get; set; }
        public DbSet<ProjectDisciplineTmer> ProjectTmers { get; set; }
        public DbSet<ProjectDisciplineTmerPeriod> ProjectTmerPeriods { get; set; }
        
        public DbSet<ProjectCompetitionGroup> ProjectCompetitionGroups { get; set; }
        public DbSet<ProjectProperty> ProjectProperties { get; set; }
        
        public DbSet<ProjectSubgroup> ProjectSubgroups { get; set; }
        public DbSet<ProjectSubgroupMembership> ProjectSubgroupMemberships { get; set; }
        public DbSet<ProjectSubgroupCount> ProjectSubgroupCounts { get; set; }

        public DbSet<ProjectAdmission> ProjectAdmissions { get; set; }
        public DbSet<ProjectStudentSelectionPriority> ProjectStudentSelectionPriorities { get; set; }
    
        public DbSet<ProjectUser> ProjectUsers { get; set; }

        public DbSet<ModuleRelation> ModuleRelations { get; set; }

        public DbSet<ProjectROPProfile> ProjectROPProfiles { get; set; }

        public DbSet<ProjectCompetence> ProjectCompetences { get; set; }
        public DbSet<ModuleAgreement> ModuleAgreements { get; set; }

        public DbSet<ProjectRole> ProjectRoles { get; set; }

        public DbSet<BasicCharacteristicOP> BasicCharacteristicOPs { get; set; }

        public DbSet<TrainingDuration> TrainingDurations { get; set; }

        public DbSet<AreaEducation> AreaEducations { get; set; }
        public DbSet<AreaEducationOrder> AreaEducationOrders { get; set; }

        public DbSet<MUP> MUPs { get; set; }
        public DbSet<MUPPeriod> MUPPeriods { get; set; }
        public DbSet<MUPDiscipline> MUPDisciplines { get; set; }
        public DbSet<MUPDisciplineTmer> MUPDisciplineTmers { get; set; }
        public DbSet<MUPDisciplineTmerPeriod> MUPDisciplineTmerPeriods { get; set; }
        public DbSet<MUPSubgroupCount> MUPSubgroupCounts { get; set; }
        public DbSet<MUPSubgroup> MUPSubgroups { get; set; }
        public DbSet<MUPSubgroupMembership> MUPSubgroupMemberships { get; set; }
        public DbSet<MUPCompetitionGroup> MUPCompetitionGroups { get; set; }
        public DbSet<MUPProperty> MUPProperties { get; set; }
        public DbSet<MUPAdmission> MUPAdmissions { get; set; }

        public DbSet<MUPModeus> MUPModeuses { get; set; }
        public DbSet<MUPModeusDirections> MUPModeusDirections { get; set; }
        public DbSet<MUPModeusRealization> MUPModeusRealizations { get; set; }
        public DbSet<MUPModeusTeam> MUPModeusTeams { get; set; }
        public DbSet<MUPModeusTeacher> MUPModeusTeachers { get; set; }
        public DbSet<MUPModeusTeamStudent> MUPModeusTeamStudents { get; set; }

        public DbSet<MUPDisciplineConnection> MUPDisciplineConnections { get; set; }

        public DbSet<MUPSubgroupTeacher> MUPSubgroupTeachers { get; set; }

        public DbSet<CompetenceGroup> CompetenceGroups { get; set; }

        public DbSet<BasicCharacteristicOPInfo> BasicCharacteristicOPInfos { get; set; }

        public DbSet<FileStorage> FileStorage { get; set; }

        public DbSet<ProfActivityArea> ProfActivityAreas { get; set; }
        public DbSet<ProfActivityKind> ProfActivityKinds { get; set; }
        public DbSet<ProfStandard> ProfStandards { get; set; }
        public DbSet<ProfOrder> ProfOrders { get; set; }
        public DbSet<ProfOrderChange> ProfOrderChanges { get; set; }
        public DbSet<ProfOrderConnection> ProfOrderConnections { get; set; }

        public DbSet<VariantsUni> VariantUni { get; set; }

        public DbSet<DirectionOrder> DirectionOrders { get; set; }

        public DbSet<EduResult2> EduResults2 { get; set; }
        public DbSet<EduResultType> EduResultTypes { get; set; }
        public DbSet<EduResultKind> EduResultKinds { get; set; }

        public DbSet<CompetencePassport> CompetencePassports { get; set; }
        public DbSet<RatingCoefficient> RatingCoefficients { get; set; }

        public DbSet<ModuleAnnotation> ModuleAnnotations { get; set; }
        public DbSet<BasicCharacteristicOPRatifyData> BasicCharacteristicOPRatifyData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VariantContent>() 
                .HasOne(vc => vc.ModuleType)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ModuleAdmission>()
                .HasOne(vc => vc.Student)
                .WithMany(s => s.ModuleAdmissions)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VariantAdmission>()
                .HasOne(vc => vc.Student)
                .WithMany(s => s.VariantAdmissions)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<ModuleDisciplineMapping>()
                .HasKey(t => new { t.MId, t.DId });

            modelBuilder.Entity<ModuleDisciplineMapping>()
                .HasOne(pt => pt.Discipline)
                .WithMany(p => p.ModuleDisciplineMapping)
                .HasForeignKey(pt => pt.DId);

            modelBuilder.Entity<ModuleDisciplineMapping>()
                .HasOne(pt => pt.Modules)
                .WithMany(p => p.ModuleDisciplineMapping)
                .HasForeignKey(pt => pt.MId);

            modelBuilder.Entity<VariantContentRequirements>()
                .HasKey(t => new { t.RequiredForId, t.RequirementId });

            modelBuilder.Entity<VariantContentRequirements>()
                .HasOne(pt => pt.VariantContent)
                .WithMany(p => p.VariantContentRequirements)
                .HasForeignKey(pt => pt.RequiredForId);

            modelBuilder.Entity<VariantContentRequirements>()
                .HasOne(pt => pt.VariantContent)
                .WithMany(p => p.VariantContentRequirements)
                .HasForeignKey(pt => pt.RequirementId);

            modelBuilder.Entity<VariantContent>()
                .HasOne(c => c.Group)
                .WithMany(variant => variant.Contents)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VariantSelectionGroup>()
                .HasOne(c => c.Variant)
                .WithMany(variant => variant.SelectionGroups)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VariantGroup>()
                .HasOne(c => c.Variant)
                .WithMany(variant => variant.Groups)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ModulesInDirections>()
                .HasKey(t => new { t.DirectionId, t.ModuleId });

            modelBuilder.Entity<ModulesInDirections>()
                .HasOne(pt => pt.Direction)
                .WithMany(p => p.ModulesInDirections)
                .HasForeignKey(pt => pt.DirectionId);

            modelBuilder.Entity<ModulesInDirections>()
                .HasOne(pt => pt.Modules)
                .WithMany(p => p.ModulesInDirections)
                .HasForeignKey(pt => pt.ModuleId);
                       
            modelBuilder.Entity<Variant>()
                .HasOne(c => c.Program)
                .WithMany(program => program.Variants)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<VariantContent>()
                .Ignore(vc => vc.RequirmentsIds);

            modelBuilder.Entity<VariantContent>()
                .HasOne(c => c.Module)
                .WithMany(module => module.UsedInVariantContents)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EduProgram>()
                .HasOne(c => c.Division)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EduProgram>()
                .HasOne(c => c.Chair)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EduProgram>()
                .HasOne(c => c.Direction)
                .WithMany(d => d.Programs)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Plan>()
                .HasKey(p => new { p.disciplineUUID, p.moduleUUID, p.eduplanUUID, p.versionUUID });

            modelBuilder.Entity<Plan>()
                .HasOne(p => p.Module)
                .WithMany(discipline => discipline.Plans)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserDivision>()
                .HasOne(p => p.User)
                .WithMany(u => u.UserDivisions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserDivision>()
                .HasOne(p => p.Division)
                .WithMany(d => d.Users)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<UserMinor>()
                .HasOne(p => p.User)
                .WithMany(u => u.Minors)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserMinor>()
                .HasOne(p => p.Module)
                .WithMany(d => d.Users)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserDirection>()
                .HasOne(p => p.User)
                .WithMany(u => u.Directions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserDirection>()
                .HasOne(p => p.Direction)
                .WithMany(d => d.Users)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<StudentVariantSelection>()
                .HasOne(c => c.Variant)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StudentVariantSelection>()
                .HasOne(c => c.Student)
                .WithMany(s => s.Selections)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<StudentSelectionTeacher>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StudentSelectionPriority>()
                .HasOne(c => c.VariantContent)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StudentSelectionPriority>()
                .HasOne(c => c.Variant)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SectionFKSubgroupCount>()
                .HasOne(c => c.SectionFKDisciplineTmerPeriod)
                .WithMany(c => c.SectionFKSubgroupCounts)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SectionFKSubgroup>()
                .HasOne(c => c.Meta)
                .WithMany(c => c.Subgroups)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<ForeignLanguageSubgroupCount>()
                .HasOne(c => c.ForeignLanguageDisciplineTmerPeriod)
                .WithMany(c => c.ForeignLanguageSubgroupCounts)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ForeignLanguageSubgroup>()
                .HasOne(c => c.Meta)
                .WithMany(c => c.Subgroups)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MinorDisciplineTmerPeriodDivision>()
                .HasKey(t => new { t.DisciplineTmerPeriodId, t.DivisionId });

            modelBuilder.Entity<MinorDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.Divisions)
                .WithMany(p => p.MinorDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.DivisionId);

            modelBuilder.Entity<MinorDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.MinorDisciplineTmerPeriod)
                .WithMany(p => p.MinorDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.DisciplineTmerPeriodId);

            modelBuilder.Entity<MinorRequirements>()
                .HasKey(t => new { t.RequiredForId, t.RequirementId });

            modelBuilder.Entity<MinorRequirements>()
                .HasOne(pt => pt.Minor)
                .WithMany(p => p.MinorRequirements)
                .HasForeignKey(pt => pt.RequiredForId);

            modelBuilder.Entity<MinorRequirements>()
                .HasOne(pt => pt.Minor)
                .WithMany(p => p.MinorRequirements)
                .HasForeignKey(pt => pt.RequirementId);
                        
            modelBuilder.Entity<StudentSelectionMinorPriority>()
              .HasOne(c => c.MinorPeriod)
              .WithMany()
              .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Minor>()
              .Ignore(vc => vc.RequirmentId);

            modelBuilder.Entity<SectionFKDisciplineTmerPeriodDivision>()
                .HasKey(t => new { t.SectionFKDisciplineTmerPeriodId, t.DivisionId });

            modelBuilder.Entity<SectionFKDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.Divisions)
                .WithMany(p => p.SectionFKDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.DivisionId);

            modelBuilder.Entity<SectionFKDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.SectionFKDisciplineTmerPeriod)
                .WithMany(p => p.SectionFKDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.SectionFKDisciplineTmerPeriodId);

            modelBuilder.Entity<ForeignLanguageDisciplineTmerPeriodDivision>()
                .HasKey(t => new { t.ForeignLanguageDisciplineTmerPeriodId, t.DivisionId });

            modelBuilder.Entity<ForeignLanguageDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.Divisions)
                .WithMany(p => p.ForeignLanguageDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.DivisionId);

            modelBuilder.Entity<ForeignLanguageDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.ForeignLanguageDisciplineTmerPeriods)
                .WithMany(p => p.ForeignLanguageDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.ForeignLanguageDisciplineTmerPeriodId);

            modelBuilder.Entity<SectionFKCompetitionGroupContents>()
                .HasKey(t => new { t.GroupId, t.SectionFKCompetitionGroupId });

            modelBuilder.Entity<SectionFKCompetitionGroupContents>()
                .HasOne(pt => pt.Groups)
                .WithMany(p => p.SectionFKCompetitionGroupContents)
                .HasForeignKey(pt => pt.GroupId);

            modelBuilder.Entity<SectionFKCompetitionGroupContents>()
                .HasOne(pt => pt.SectionFkCompetitionGroups)
                .WithMany(p => p.SectionFKCompetitionGroupContents)
                .HasForeignKey(pt => pt.SectionFKCompetitionGroupId);

            modelBuilder.Entity<SectionFKTeachers>()
                .HasKey(t => new { t.TeacherId, t.SectionFKPropertyId });

            modelBuilder.Entity<SectionFKTeachers>()
                .HasOne(pt => pt.Teachers)
                .WithMany(p => p.SectionFKTeachers)
                .HasForeignKey(pt => pt.TeacherId);

            modelBuilder.Entity<SectionFKTeachers>()
                .HasOne(pt => pt.SectionFKProperties)
                .WithMany(p => p.SectionFKTeachers)
                .HasForeignKey(pt => pt.SectionFKPropertyId);

            modelBuilder.Entity<SectionFKTrainingPlace>()
                .HasKey(t => new { t.TrainingPlaceId, t.SectionFKPropertyId });

            modelBuilder.Entity<SectionFKTrainingPlace>()
                .HasOne(pt => pt.TrainingPlaces)
                .WithMany(p => p.SectionFKTrainingPlace)
                .HasForeignKey(pt => pt.TrainingPlaceId);

            modelBuilder.Entity<SectionFKTrainingPlace>()
                .HasOne(pt => pt.SectionFKProperties)
                .WithMany(p => p.SectionFKTrainingPlace)
                .HasForeignKey(pt => pt.SectionFKPropertyId);

            modelBuilder.Entity<ForeignLanguageCompetitionGroupContents>()
                .HasKey(t => new { t.GroupId, t.ForeignLanguageCompetitionGroupId });

            modelBuilder.Entity<ForeignLanguageCompetitionGroupContents>()
                .HasOne(pt => pt.Groups)
                .WithMany(p => p.ForeignLanguageCompetitionGroupContents)
                .HasForeignKey(pt => pt.GroupId);

            modelBuilder.Entity<ForeignLanguageCompetitionGroupContents>()
                .HasOne(pt => pt.ForeignLanguageCompetitionGroups)
                .WithMany(p => p.ForeignLanguageCompetitionGroupContents)
                .HasForeignKey(pt => pt.ForeignLanguageCompetitionGroupId);

            modelBuilder.Entity<ForeignLanguageTeachers>()
                .HasKey(t => new { t.TeacherId, t.ForeignLanguagePropertyId });

            modelBuilder.Entity<ForeignLanguageTeachers>()
                .HasOne(pt => pt.Teachers)
                .WithMany(p => p.ForeignLanguageTeachers)
                .HasForeignKey(pt => pt.TeacherId);

            modelBuilder.Entity<ForeignLanguageTeachers>()
                .HasOne(pt => pt.ForeignLanguageProperties)
                .WithMany(p => p.ForeignLanguageTeachers)
                .HasForeignKey(pt => pt.ForeignLanguagePropertyId);
                        
            modelBuilder.Entity<MinorSubgroupMembership>().Property(x => x.Score).HasColumnType("DECIMAL(4, 1)");

            modelBuilder.Entity<PlanAdditional>().Property(x => x.contactTotal).HasColumnType("DECIMAL(20, 2)");
            modelBuilder.Entity<PlanAdditional>().Property(x => x.contactSelf).HasColumnType("DECIMAL(20, 2)");
            modelBuilder.Entity<PlanAdditional>().Property(x => x.contactControl).HasColumnType("DECIMAL(20, 2)");
            modelBuilder.Entity<PlanAdditional>().Property(x => x.contactLecture).HasColumnType("DECIMAL(20, 2)");
            modelBuilder.Entity<PlanAdditional>().Property(x => x.contactPractice).HasColumnType("DECIMAL(20, 2)");
            modelBuilder.Entity<PlanAdditional>().Property(x => x.contactLabs).HasColumnType("DECIMAL(20, 2)");

            modelBuilder.Entity<PlanAdditional>()
                .HasKey(p => new { p.versionUUID, disciplineUUID = p.disciplineUUID,});

            modelBuilder.Entity<PlanTermWeek>()
                .HasKey(p => new {p.eduplanUUID, p.Term});

            modelBuilder.Entity<ModuleWorkingProgram>()
                .HasMany(p => p.SourceChangeLists).WithOne(p => p.Source)
                .HasForeignKey(p => p.SourceId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<ModuleWorkingProgram>()
                .HasMany(p => p.TargetChangeLists).WithOne(p => p.Target)
                .HasForeignKey(p => p.TargetId).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProjectDisciplineTmerPeriodDivision>()
                .HasKey(t => new { t.ProjectDisciplineTmerPeriodId, t.DivisionId });

            modelBuilder.Entity<ProjectDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.Divisions)
                .WithMany(p => p.ProjectDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.DivisionId);

            modelBuilder.Entity<ProjectDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.ProjectDisciplineTmerPeriods)
                .WithMany(p => p.ProjectDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.ProjectDisciplineTmerPeriodId);
            
            modelBuilder.Entity<ProjectCompetitionGroupContents>()
                .HasKey(t => new { t.ProjectCompetitionGroupId, t.GroupId });

            modelBuilder.Entity<ProjectCompetitionGroupContents>()
                .HasOne(pt => pt.Groups)
                .WithMany(p => p.ProjectCompetitionGroupContents)
                .HasForeignKey(pt => pt.GroupId);

            modelBuilder.Entity<ProjectCompetitionGroupContents>()
                .HasOne(pt => pt.ProjectCompetitionGroups)
                .WithMany(p => p.ProjectCompetitionGroupContents)
                .HasForeignKey(pt => pt.ProjectCompetitionGroupId);
                        
            modelBuilder.Entity<ModuleAgreement>()
                .HasKey(p => new { p.ModuleUUID, p.DisciplineUUID, p.UniId, p.SemesterId, p.EduYear });

            modelBuilder.Entity<BasicCharacteristicOPMapping>()
                .HasKey(t => new { t.BasicCharacteristicOPId, t.ModuleWorkingProgramId });

            modelBuilder.Entity<BasicCharacteristicOPMapping>()
                .HasOne(pt => pt.ModuleWorkingPrograms)
                .WithMany(p => p.BasicCharacteristicOPMapping)
                .HasForeignKey(pt => pt.ModuleWorkingProgramId);

            modelBuilder.Entity<BasicCharacteristicOPMapping>()
                .HasOne(pt => pt.BasicCharacteristicOPs)
                .WithMany(p => p.BasicCharacteristicOPMapping)
                .HasForeignKey(pt => pt.BasicCharacteristicOPId);

            modelBuilder.Entity<MUPCompetitionGroupContents>()
                .HasKey(t => new { t.MUPCompetitionGroupId, t.GroupId });

            modelBuilder.Entity<MUPCompetitionGroupContents>()
                .HasOne(pt => pt.MUPCompetitionGroups)
                .WithMany(p => p.MUPCompetitionGroupContents)
                .HasForeignKey(pt => pt.MUPCompetitionGroupId);

            modelBuilder.Entity<MUPCompetitionGroupContents>()
                .HasOne(pt => pt.Groups)
                .WithMany(p => p.MUPCompetitionGroupContents)
                .HasForeignKey(pt => pt.GroupId);
            
            modelBuilder.Entity<MUPDisciplineTmerPeriodDivision>()
                .HasKey(t => new { t.MUPDisciplineTmerPeriodId, t.DivisionId });

            modelBuilder.Entity<MUPDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.MUPDisciplineTmerPeriods)
                .WithMany(p => p.MUPDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.MUPDisciplineTmerPeriodId);

            modelBuilder.Entity<MUPDisciplineTmerPeriodDivision>()
                .HasOne(pt => pt.Divisions)
                .WithMany(p => p.MUPDisciplineTmerPeriodDivision)
                .HasForeignKey(pt => pt.DivisionId);

            modelBuilder.Entity<MUPTeachers>()
                .HasKey(t => new { t.TeacherId, t.MUPPropertyId });

            modelBuilder.Entity<MUPTeachers>()
                .HasOne(pt => pt.Teachers)
                .WithMany(p => p.MUPTeachers)
                .HasForeignKey(pt => pt.TeacherId);

            modelBuilder.Entity<MUPTeachers>()
                .HasOne(pt => pt.MUPProperties)
                .WithMany(p => p.MUPTeachers)
                .HasForeignKey(pt => pt.MUPPropertyId);

            modelBuilder.Entity<MUPSubgroupTeacher>()
                .HasKey(p => new { p.MUPSubgroupId, p.TeacherId });

            modelBuilder.Entity<ProfOrderConnection>()
                .HasKey(p => new { p.ProfOrderId, p.ProfOrderChangeId });

            base.OnModelCreating(modelBuilder);
        }

        public Division GetInstituteForChair(string chairId)
        {
            var division = Divisions.FirstOrDefault(d => d.uuid == chairId);
            while(division?.typeTitle != "Институт" && division != null)
            {
                division = Divisions.FirstOrDefault(d => d.uuid == division.parent);
            }
            return division;
        }

        public Division GetInstituteOrDepartmentForChair(string chairId)
        {
            var division = Divisions.FirstOrDefault(d => d.uuid == chairId);
            while (division?.typeTitle != "Институт" && division?.typeTitle != "Департамент" && division != null)
            {
                division = Divisions.FirstOrDefault(d => d.uuid == division.parent);
            }
            return division;
        }

        public IQueryable<Division> ChairsForDivision(string divisionUuid)
        {
            var chairs = new List<Division>();
            var division = Divisions.FirstOrDefault(d => d.uuid == divisionUuid);
            if (division?.typeTitle != "Кафедра")
            {
                var childs = Divisions.Where(d => d.parent == divisionUuid).ToList();
                
                for(int i = 0; i < childs.Count; i++)// var d in childs)
                {
                    var currentDivision = childs[i];
                    if (currentDivision.typeTitle == "Кафедра" && !chairs.Contains(currentDivision))
                    {
                        chairs.Add(currentDivision);
                    }
                    else
                    {
                        var _childs = Divisions.Where(_d => _d.parent == currentDivision.uuid).ToList();
                        childs.AddRange(_childs);
                    }
                    childs.RemoveAt(i);
                    i = -1;
                }
            }
            else
            {
                chairs.Add(division);
            }
            return chairs.AsQueryable();
        }

        public IQueryable<Competence> CompetencesActive()
        {
            return Competences.Where(_ => !_.IsDeleted);
        }
        public IQueryable<Module> ModulesForUser(IPrincipal principal)
        {
            IQueryable<Module> modules = UniModules();
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;
                modules = modules.Where(m => m.Directions.SelectMany(d => d.Users).Any(u => u.UserName == userName));
            }
            return modules;
        }
        public IQueryable<ModuleWorkingProgram> ModuleWorkingProgramsForUser(IPrincipal principal)
        {
            IQueryable<ModuleWorkingProgram> modules = ModuleWorkingPrograms;
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;
                modules = modules.Where(m => m.Module.Directions.SelectMany(d => d.Users).Any(u => u.UserName == userName));
            }
            return modules;
        }

        public IQueryable<Module> MinorsForUser(IPrincipal principal)
        {
            IQueryable<Module> modules = UniModules().Where(m => m.type.Contains("Майнор"));

            if (!principal.IsInRole(ItsRoles.AllMinors))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;

                return UserMinors.Where(u => u.UserName == userName).Select(u => u.Module);
            }
            return modules;
        }



        public IQueryable<MinorSubgroup> MinorSubgroupsForUser(IPrincipal principal)
        {
            if (!principal.IsInRole(ItsRoles.AllMinors))
            {
                var minorsForUser = MinorsForUser(principal);

                return MinorSubgroups.Where(s => minorsForUser.Any(mx => mx.uuid == s.Meta.Period.Minor.ModuleId));
            }
            return MinorSubgroups;
        }

        public IQueryable<Direction> DirectionsForUser(IPrincipal principal, bool considerDivisions = false)
        {
            IQueryable<Direction> directions = Directions;
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;
                directions = directions.Where(m => m.Users.Any(u => u.UserName == userName));
            }

            if (considerDivisions && !principal.IsInRole(ItsRoles.AllDirections)) // учитываем указанные подразделения в правах пользователя
            {
                var directionsUids = directions.Select(d => d.uid).ToList();
                directions = DivisionsForUser(principal).Select(d => d.uuid).ToList()
                    .Select(d => DirectionsForDivision(d))
                    .SelectMany(d => d)
                    .Where(d => directionsUids.Contains(d.uid)).AsQueryable();
            }

            return directions;
        }

        public IQueryable<Direction> DirectionsForUser(string userName)
        {
            IQueryable<Direction> directions = Directions;

            directions = directions.Where(m => m.Users.Any(u => u.UserName == userName));

            return directions;
        }

        public IQueryable<Direction> DirectionsForDivision(string divisionUuid)
        {
            var divisionUuids = Divisions.Where(d => d.parent == divisionUuid || d.uuid == divisionUuid || 
                Divisions.Any(p => p.uuid == d.parent && p.parent == divisionUuid) /*d.ParentDivision.parent == divisionUuid*/).Select(d => d.uuid).ToList();
            return Profiles.Where(p => divisionUuids.Contains(p.CHAIR_ID)).Select(p => p.Direction).Distinct();
        }

        public IQueryable<Profile> ProfilesForDivision(string divisionUuid)
        {
            var divisionUuids = Divisions.Where(d => d.parent == divisionUuid || d.uuid == divisionUuid ||
                Divisions.Any(p => p.uuid == d.parent && p.parent == divisionUuid) /*d.ParentDivision.parent == divisionUuid*/).Select(d => d.uuid).ToList();
            return Profiles.Where(p => divisionUuids.Contains(p.CHAIR_ID));
        } 

        public IQueryable<Division> DivisionsForUser(string userName)
        {
            IQueryable<Division> divisions = Divisions;

            divisions = divisions.Where(m => m.Users.Any(u => u.UserName == userName));

            return divisions;
        }


        public IQueryable<Division> DivisionsForUser(IPrincipal principal)
        {
            IQueryable<Division> directions = Divisions;
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;
                directions = directions.Where(m => m.Users.Any(u => u.UserName == userName));
            }
            return directions;
        }

        public Division[] FlattenedHierarchicalDivisionsForUser(IPrincipal principal)
        {
            var divisions = DivisionsForUser(principal).ToList();

            IEnumerable<Division> GetParentDivisions(IEnumerable<Division> ds)
            {
                var parentIds = ds.Where(d => d.parent != null).Select(d => d.parent).ToList();
                var parents = Divisions.Where(d => parentIds.Contains(d.uuid)).ToList();
                
                if (!parents.Any())
                    return Enumerable.Empty<Division>();

                return parents.Concat(GetParentDivisions(parents));
            }

            var flattenedHierarchy = GetParentDivisions(divisions);
            var result = divisions.Concat(flattenedHierarchy).ToArray();
            return result;
        }

        public IQueryable<Division> InstitutesForUser(IPrincipal principal, bool includeDepartments = false)
        {
            var divisionsForUser = DivisionsForUser(principal).ToList();
            var divisions = new List<Division>();
            foreach (var d in divisionsForUser)
            {
                var currentDivision = d;
                while (currentDivision != null)
                {
                    if ((currentDivision.typeTitle == "Институт" || currentDivision.typeTitle == "Филиал" 
                                || currentDivision.typeTitle == "Департамент" && includeDepartments) && !divisions.Contains(currentDivision))
                    {
                        divisions.Add(currentDivision);
                        currentDivision = null;
                    }
                    else
                    {
                        currentDivision = Divisions.FirstOrDefault(_d => _d.uuid == currentDivision.parent);
                    }
                }
            }
            return divisions.AsQueryable();
        }

        public IQueryable<Division> PossibleDivisionsForUser(string userName)
        {
            var db = new ApplicationDbContext();
            UserStore<ApplicationUser> us = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(us, null, null, null, null, null, null, null, null);
            var user = um.FindByNameAsync(userName).Result;
            var Roles = um.GetRolesAsync(user).Result;
            IQueryable<Division> divisions = Divisions;
            divisions =
                divisions.Where(
                    m =>
                        m.Users.Any(u => u.UserName == userName) || 
                            Roles.Any(ur => ur == Roles.FirstOrDefault(r => r == ItsRoles.AllDirections))
                        || m.uuid == "undifa18ggl5g0000jn134fnnnm25pgk" // Институт Технологий открытого образования добавляется всем и всегда, потому что у него нет направлений
                            )
                    .Concat(DirectionsForUser(userName).SelectMany(d => d.Programs.Select(p => p.Division)))
                    .Distinct();
            return divisions;
        }

        public IQueryable<Variant> VariantsForUser(IPrincipal principal)
        {
            IQueryable<Variant> modules = Variants;
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;
                modules =
                    modules.Where(
                        m =>
                            m.Program.Direction.Users.Any(u => u.UserName == userName) &&
                            m.Program.Division.Users.Any(u => u.UserName == userName));
            }
            return modules;
        }

        public IQueryable<Subgroup> SubgroupsForUser(IPrincipal principal)
        {
            IQueryable<Subgroup> modules = Subgroups.Where(s => !s.Removed);
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;
                modules =
                    modules.Where(
                        m =>
                            m.Meta.Program.Direction.Users.Any(u => u.UserName == userName) &&
                            m.Meta.Program.Division.Users.Any(u => u.UserName == userName));
            }
            return modules;
        }
        public IQueryable<MetaSubgroup> MetaSubgroupsForUser(IPrincipal principal)
        {
            IQueryable<MetaSubgroup> modules = MetaSubgroups;
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;
                modules =
                    modules.Where(
                        m =>
                            m.Program.Direction.Users.Any(u => u.UserName == userName) &&
                            m.Program.Division.Users.Any(u => u.UserName == userName));
            }
            return modules;
        }

        public IQueryable<EduProgram> EduProgramsForUser(IPrincipal principal)
        {
            IQueryable<EduProgram> modules = EduPrograms.Include(p => p.Direction);
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userName = claim.Value;
                modules =
                    modules.Where(
                        m =>
                            m.Direction.Users.Any(u => u.UserName == userName) &&
                            m.Division.Users.Any(u => u.UserName == userName));
            }
            return modules;
        }


        public IQueryable<Group> GroupsForUser(IPrincipal principal)
        {
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                return Groups.Where(s => UserDivisions.Any(d => d.UserName == principal.Identity.Name && (d.DivisionId == s.ManagingDivisionParentId || d.DivisionId == s.FormativeDivisionParentId || d.DivisionId == s.ManagingDivisionId || d.DivisionId == s.FormativeDivisionId))).Distinct();
            }
            return Groups;
        }

        public IQueryable<Student> StudentsForUser(IPrincipal principal)
        {
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                return
                    Students.Where(s => UserDivisions.Any(d => d.UserName == principal.Identity.Name && (d.DivisionId == s.Group.ManagingDivisionParentId || d.DivisionId == s.Group.FormativeDivisionParentId || d.DivisionId == s.Group.ManagingDivisionId || d.DivisionId == s.Group.FormativeDivisionId))).Distinct();
            }
            return Students;
        }

        public IQueryable<Discipline> DisciplinesForUser(IPrincipal principal)
        {
            if (!principal.IsInRole(ItsRoles.AllDirections))
            {
                return ModulesForUser(principal).SelectMany(m => m.disciplines).Distinct();
            }
            return Disciplines;
        }

        public IQueryable<Plan> PlansForUser(IPrincipal principal)
        {
            return ModulesForUser(principal).SelectMany(m => m.Plans).Distinct();
        }

        public List<SemesterTestUnits> GetSemesterTestUnitsesForStudent(string id)
        {

            var student = Students.FirstOrDefault(_ => _.Id == id);

            var currentProgramId = VariantAdmissions.Where(va => va.studentId == id && va.Status == AdmissionStatus.Admitted).OrderByDescending(v => v.Variant.Program.Year).Select(v => v.Variant.EduProgramId).FirstOrDefault();

            var query = Variants.Include("EduPrograms").Where(
                    v =>
                        VariantAdmissions.Any(
                            va =>
                                va.Variant.EduProgramId == v.EduProgramId && va.studentId == id &&
                                va.Status == AdmissionStatus.Admitted) && (!v.IsBase || v.EduProgramId == currentProgramId) ) 

                .Select(variant => new
                {
                    variantId = variant.Id,
                    variant.IsBase,
                    variant.Program,
                    variant.EduProgramId,
                    modules = variant.Groups.SelectMany(g => g.Contents).Where(vc => vc.Selected)
                        .Select(vc => new
                        {

                            admission =
                            ModuleAdmissions.Where(ma => ma.moduleId == vc.moduleId && ma.studentId == id)
                                .Select(va => (AdmissionStatus?)va.Status)
                                .FirstOrDefault(),
                            shouldHaveStatus = vc.Selectable || vc.SelectionGroup != null,
                            vc.Module.Plans,
                            vc.Module.type,
                        }),
                    admission =
                    VariantAdmissions.OrderByDescending(va => va.Variant.Program.Year)
                        .Where(va => va.variantId == variant.Id && va.studentId == id)
                        .Select(va => (AdmissionStatus?)va.Status)
                        .FirstOrDefault(),
                });


            var semTestUnits =
                query.Where(m => m.IsBase || (m.admission.HasValue && m.admission == AdmissionStatus.Admitted))
                .SelectMany(
                        _ =>
                            _.modules.Where(m => !m.type.Equals("Факультативные дисциплины") && ((!m.shouldHaveStatus && _.EduProgramId == currentProgramId) || (m.shouldHaveStatus && (m.admission.HasValue && m.admission == AdmissionStatus.Admitted))))
                                .Select(m => new { module = m, variant = _ })
                                .SelectMany(m => m.module.Plans
                                    .Where(p => p.qualification == m.variant.Program.qualification &&
                                                p.familirizationType == m.variant.Program.familirizationType &&
                                                p.familirizationCondition == m.variant.Program.familirizationCondition &&
                                                p.versionNumber == m.variant.Program.PlanVersionNumber &&
                                                p.eduplanNumber == m.variant.Program.PlanNumber &&
                                                p.directionId == m.variant.Program.directionId &&
                                                p.active &&
                                                p.versionStatus == "Утверждено" && 
                                                
                                                m.variant.Program.State == VariantState.Approved

                                    )
                                    .Where(
                                        p =>
                                            p.faculty == m.variant.Program.divisionId ||
                                            p.faculty == m.variant.Program.departmentId ||
                                            p.faculty == m.variant.Program.chairId))
                                .Where(p => p.terms.Length > 2)
                                .Select(p => new
                                {
                                    p.terms, p.testUnitsByTerm,
                                    p.disciplineTitle,
                                    p.eduplanNumber,
                                    p.Module.title,
                                    p.versionNumber,
                                    



                                    
                                })
                    ).ToList()
                    .Select(p => new
                    {
                        terms = JsonConvert.DeserializeObject<List<int>>(p.terms),
                        testUnitsByTerm = JObject.Parse(p.testUnitsByTerm),
                        p.eduplanNumber,
                        p.disciplineTitle,
                        p.title,
                        p.versionNumber



                    });


            var semesterTestUnitses = semTestUnits.SelectMany(p => p.terms.Where(t =>
                       {
                           var value = p.testUnitsByTerm.GetValue(t.ToString());

                           return value != null;
                       })
                     .Select(t => new
                    {
                        name = t, testUnits = (int)p.testUnitsByTerm.GetValue(t.ToString()),
                        p.disciplineTitle,
                        p.eduplanNumber,
                        p.title,
                        p.versionNumber

                     }))
                     .GroupBy(t => t.name).Select(t =>
                  {
                      Debug.WriteLine(t.Key.ToString());
                      foreach (var tu in t)
                      {
                          Debug.WriteLine($"{tu.title} -- {tu.disciplineTitle} -- {tu.eduplanNumber} -- версия {tu.versionNumber} -- {tu.testUnits} ");
                      }
                      return new SemesterTestUnits()
                    {
                        

                        Semester = t.Key,
                        TestUnits = t.Sum(_ => _.testUnits)

                    };
                }).OrderBy(_ => _.Semester).ToList();


            var minorAdmission = MinorAdmissions.Include("MinorPeriod.Minor.Module").Where(a => a.studentId == id).ToList();

            foreach (var admission in minorAdmission.Where(_ => _.Status == AdmissionStatus.Admitted))
            {
                var semester = (admission.MinorPeriod.Year - Int32.Parse(student.Group.Year)) * 2 +
                               admission.MinorPeriod.SemesterId;

                var semesterTestUnits = semesterTestUnitses.FirstOrDefault(_ => _.Semester == semester);
                if (semesterTestUnits != null)
                    semesterTestUnits.TestUnits +=
                        admission.MinorPeriod.Minor.Module.testUnits;
                else
                {
                    semesterTestUnitses.Add(new SemesterTestUnits()
                    {
                        Semester = semester,
                        TestUnits = admission.MinorPeriod.Minor.Module.testUnits
                    });
                }

            }
            return semesterTestUnitses;
        }

        public Variant FillVariantWithDefaults(Variant copyFromVariant, CreateVariantViewModel variantVm)
        {
            Variant variant = new Variant
            {
                Name = variantVm.Name,
                State = VariantState.Development,
                EduProgramId = variantVm.EduProgramId
            };

            var program = EduPrograms.Find(variantVm.EduProgramId);

            FillVariantWithDefaults(copyFromVariant, variant, program);

            return variant;
        }

        public void FillVariantWithDefaults(Variant copyFromVariant, Variant variant, EduProgram program)
        {
            string id = program.directionId;
            variant.CreateDate = DateTime.Now;

            if (copyFromVariant == null)
            {
                var isFgosVo = program.Direction.standard == "ФГОС ВО";

                var modulesForVariant =
                    UniModules()
                        .Where(m => m.Directions.Any(d => d.uid == id))
                        .Include(m => m.disciplines)
                        .Select(m => new
                        {
                            plan = m.Plans.FirstOrDefault(
                                        p => p.qualification == program.qualification &&
                                             p.familirizationType == program.familirizationType &&
                                             p.familirizationCondition == program.familirizationCondition &&
                                             p.versionNumber == program.PlanVersionNumber &&
                                             p.eduplanNumber == program.PlanNumber &&
                                             p.active && !p.remove
                                        ),
                            module = m
                        })
                        .Where(m => m.plan != null)
                        .Where(m => isFgosVo || !isFgosVo && m.plan.moduleGroupType != null)   
                        .ToList()
                        .Select(m => new
                        {
                            m.module,
                            m.module
                                .disciplines.FirstOrDefault(d => !d.section.StartsWith("Контроль"))?
                                .section,
                            type = VariantGroupTypeHelpers.TryParse(m.module.type),
                            m.plan.moduleGroupType,
                            m.plan.moduleSubgroupType
                        });

                var excludeModules =
                    new HashSet<string>(
                        EduPrograms.Where(p => p.Id == program.Id)
                            .SelectMany(p => p.Variant.Groups)
                            .SelectMany(g => g.Contents)
                            .Select(m => m.moduleId));

                variant.Groups = new List<VariantGroup>();

                foreach (var moduleForVariant in modulesForVariant)
                {
                    if (string.IsNullOrWhiteSpace(moduleForVariant.section))
                        continue;
                    
                    bool groupBySection = !moduleForVariant.type.HasValue;

                    var groupType = isFgosVo
                        ? moduleForVariant.type.HasValue ? moduleForVariant.type.Value : VariantGroupTypeHelpers.Parse(moduleForVariant.section)
                        : VariantGroupTypeHelpers.Parse(moduleForVariant.moduleGroupType);

                    var subgroupType = isFgosVo ? null : VariantGroupTypeHelpers.TryParse(moduleForVariant.moduleSubgroupType);

                    if (variant.Groups.Any(g => g.GroupType == groupType && g.SubgroupType == subgroupType))
                        continue;

                    var modules = new[] { moduleForVariant }.AsEnumerable();
                    if (groupBySection) {
                        modules = isFgosVo
                            ? modulesForVariant.Where(m => m.section == moduleForVariant.section && !m.type.HasValue)
                            : modulesForVariant.Where(m => m.moduleGroupType == moduleForVariant.moduleGroupType 
                                    && (!subgroupType.HasValue || subgroupType.HasValue && m.moduleSubgroupType == moduleForVariant.moduleSubgroupType));
                    }

                    var group = new VariantGroup()
                    {
                        GroupType = groupType,
                        TestUnits = modules.Sum(i => i.module.testUnits),
                        SubgroupType = subgroupType
                    };

                    variant.Groups.Add(group);

                    group.Contents = new List<VariantContent>();
                    foreach (var module in modules)
                    {
                        if (excludeModules.Contains(module.module.uuid))
                            continue;

                        var content = new VariantContent
                        {
                            Group = group,
                            Module = module.module,
                        };
                        if (group.GroupType == VariantGroupType.Variative)
                            content.ModuleTypeId = (int)VariantContentType.Professional + 1;
                        if (group.GroupType == VariantGroupType.Required)
                            content.ModuleTypeId = (int)VariantContentType.Shared + 1;
                        if (group.GroupType == VariantGroupType.Selectable || group.GroupType == VariantGroupType.Formed)
                            content.ModuleTypeId = (int)VariantContentType.SelectableProfessional + 1;
                        group.Contents.Add(content);
                    }
                }
            }
            else
            {
                CopyVariantData(copyFromVariant, variant);
            }
        }

        public void CopyVariantData(Variant copyFromVariant, Variant variant)
        {
            variant.Name = copyFromVariant.Name;
            variant.IsBase = copyFromVariant.IsBase;
            variant.SelectionDeadline = copyFromVariant.SelectionDeadline;
            variant.CreateDate = DateTime.Now;


            variant.Groups = new List<VariantGroup>();
            variant.SelectionGroups = new List<VariantSelectionGroup>();

            Dictionary<VariantSelectionGroup, VariantSelectionGroup> selectionTranslation =
                new Dictionary<VariantSelectionGroup, VariantSelectionGroup>();

            foreach (var srcGroup in copyFromVariant.SelectionGroups)
            {
                var g = new VariantSelectionGroup
                {
                    Name = srcGroup.Name,
                    Variant = variant,
                    TestUnits = srcGroup.TestUnits,
                    SelectionDeadline = srcGroup.SelectionDeadline
                };
                variant.SelectionGroups.Add(g);

                selectionTranslation[srcGroup] = g;
            }

            Dictionary<VariantContent, VariantContent> contentTranslation =
                new Dictionary<VariantContent, VariantContent>();

            foreach (var srcGroup in copyFromVariant.Groups)
            {
                var g = new VariantGroup
                {
                    GroupType = srcGroup.GroupType,
                    TestUnits = srcGroup.TestUnits,
                    SubgroupType = srcGroup.SubgroupType
                };

                variant.Groups.Add(g);

                g.Contents = new List<VariantContent>();
                foreach (var srcModule in srcGroup.Contents)
                {
                    var content = new VariantContent
                    {
                        Group = g,
                        Module = srcModule.Module,
                        ModuleTypeId = srcModule.ModuleTypeId,
                        Selectable = srcModule.Selectable,
                        Selected = srcModule.Selected,
                        ContentType = srcModule.ContentType,
                    };
                    if (srcModule.SelectionGroup != null)
                        content.SelectionGroup = selectionTranslation[srcModule.SelectionGroup];

                    contentTranslation[srcModule] = content;
                    g.Contents.Add(content);
                }
            }

            foreach (var srcModule in copyFromVariant.Groups.SelectMany(g => g.Contents))
            {
                var dstModule = contentTranslation[srcModule];
                dstModule.Requirments = new List<VariantContent>();
                foreach (var r in srcModule.Requirments)
                {
                    dstModule.Requirments.Add(contentTranslation[r]);
                }
            }

            foreach (var pt in PlanTeachers.Where(pt => pt.variantId == copyFromVariant.Id))
            {
                var teacher = new PlanTeacher
                {
                    Selectable = pt.Selectable,
                    Variant = variant,
                    TeacherPkey = pt.TeacherPkey,
                    catalogDisciplineUuid = pt.catalogDisciplineUuid,
                    eduplanUuid = pt.eduplanUuid,
                    load = pt.load,
                    moduleId = pt.moduleId,
                };

                PlanTeachers.Add(teacher);
            }
        }

        public bool DropVariant(Variant variant)
        {
            using (var transaction = Database.BeginTransaction())
            {
                try
                {
                    VariantGroups.RemoveRange(variant.Groups.AsEnumerable());
                    StudentSelectionPriority.RemoveRange(StudentSelectionPriority.Where(p => p.variantId == variant.Id));
                    VariantAdmissions.RemoveRange(VariantAdmissions.Where(v => v.variantId == variant.Id));
                    StudentVariantSelections.RemoveRange(StudentVariantSelections.Where(v => v.selectedVariantId == variant.Id));
                    EduProgramLimits.RemoveRange(EduProgramLimits.Where(l => l.VariantId == variant.Id));
                    PlanTeachers.RemoveRange(PlanTeachers.Where(t => t.variantId == variant.Id));

                    SaveChanges();

                    foreach (
                        var vc in
                            VariantContents.Include(vc => vc.Requirments).Where(vc => vc.Group.VariantId == variant.Id))
                    {
                        vc.Requirments.Clear();
                    }

                    SaveChanges();

                    Variants.Remove(variant);

                    SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public void AddRandomTeacher(string load, string moduleId, string eduplanUuid, int variantId,
            string catalogDisciplineUuid)
        {
            var teacher = Teachers.OrderBy(r => Guid.NewGuid()).First();
            var pt = new PlanTeacher();
            pt.TeacherPkey = teacher.pkey;
            pt.Selectable = true;
            pt.catalogDisciplineUuid = catalogDisciplineUuid;
            pt.variantId = variantId;
            pt.eduplanUuid = eduplanUuid;
            pt.moduleId = moduleId;
            pt.load = load;
            PlanTeachers.Add(pt);
            SaveChanges();
        }

        public List<PlanTeacher> GetTachers(int variantId, Plan plan)
        {
            return PlanTeachers.Include(pt => pt.Teacher).Where(
                pt =>
                    pt.moduleId == plan.moduleUUID &&
                    pt.eduplanUuid == plan.eduplanUUID &&
                    pt.variantId == variantId &&
                    pt.catalogDisciplineUuid == plan.catalogDisciplineUUID
                ).OrderBy(pt => pt.load).ThenBy(pt => pt.Teacher.initials).ToList();
        }

        public IList<LimitViewModel> GetLimitViewModels(IPrincipal principal, int variantId, out bool error, out List<string> wrongModules)
        {
            var variant = Variants.Find(variantId);

            var limits = EduProgramLimits.Where(m => m.VariantId == variantId)
                .ToDictionary(l => l.ModuleId, l => l.StudentsCount);

            List<string> moduleUuids =
                variant.Groups.SelectMany(g => g.Contents)
                    .Where(c => c.Selected || variant.IsBase)
                    .Select(vc => vc.moduleId)
                    .ToList();

            var modules = UniModules().Where(m => moduleUuids.Contains(m.uuid)).Include(m => m.disciplines).ToList();
            //var StudentsCountAll =
            //    Variants.Where(v => v.EduProgramId == variant.EduProgramId && v.IsBase == false)
            //        .SelectMany(v => v.Groups)
            //        .SelectMany(vg => vg.Contents)
            //        .Where(vc => vc.Selected == true).SelectMany(vc => EduProgramLimits.Where(l => l.ModuleId == vc.moduleId)).Sum(l => (int?)l.StudentsCount) ?? 0;
            var isFgosVo = variant.IsFgosVo();
            var result = new List<LimitViewModel>();

            wrongModules = new List<string>();
            error = false;

            foreach (var m in modules)
            {
                var plan = m.Plans.Where(p => p.directionId == variant.Program.directionId)
                    .Where(p => p.qualification == variant.Program.qualification &&
                                p.familirizationType == variant.Program.familirizationType &&
                                p.familirizationCondition == variant.Program.familirizationCondition &&
                                p.versionNumber == variant.Program.PlanVersionNumber &&
                                p.eduplanNumber == variant.Program.PlanNumber &&
                                                     p.active
                    )
                    .Where(p => p.faculty == variant.Program.divisionId || p.faculty == variant.Program.departmentId || p.faculty == variant.Program.chairId)
                    .FirstOrDefault(p => !string.IsNullOrEmpty(p.moduleGroupType));
                var planGroupModuleType = plan?.moduleGroupType;

                var groupType = isFgosVo
                    ? VariantGroupTypeHelpers.TryParse(m.disciplines.FirstOrDefault(d => !d.section.StartsWith("Контроль"))?.section)
                    : VariantGroupTypeHelpers.TryParse(planGroupModuleType);

                if (!groupType.HasValue)
                {
                    error = true;
                    wrongModules.Add(m.numberAndTitle);
                    continue;
                }

                var limitViewModel = new LimitViewModel
                {
                    ModuleId = m.uuid,
                    ModuleNumberAndTitle = m.numberAndTitle,
                    StudentsCount = limits.ContainsKey(m.uuid) ? limits[m.uuid] : (int?)null,
                    VariantLimits = GetLimitsByModuleAndProgram(variant.EduProgramId, m.uuid),
                    ModuleTitle = m.title,
                    GroupType = groupType.Value,
                    Comment = GetStudentsCountLimit(variant.EduProgramId, m.uuid)
                };

                EduProgramLimit eduProgramLimit =
                    variant.Program.Variant.ProgramLimits.FirstOrDefault(l => l.ModuleId == m.uuid);
                if (eduProgramLimit != null) limitViewModel.ProgramStudentsCount = eduProgramLimit.StudentsCount;

                result.Add(limitViewModel);
            }
            
            return error ? new List<LimitViewModel>() : result.OrderBy(vm => vm.GroupType).ThenBy(vm => vm.ModuleTitle).ToList();
        }

        public IList<EduProgramLimit> GetLimitsByModuleAndProgram(int EduProgramId, string ModuleId)
        {
            return Variants.Where(v => v.EduProgramId == EduProgramId && !v.IsBase)
                .SelectMany(v => v.Groups)
                .SelectMany(vg => vg.Contents)
                .Where(vc => vc.Selected)
                .SelectMany(
                    vc => EduProgramLimits.Where(l => l.ModuleId == vc.moduleId && l.VariantId == vc.Group.VariantId))
                .Where(m => m.ModuleId == ModuleId).ToList();
        }

        public EduProgramLimit GetProgramLimit(int EduProgramId, string ModuleId)
        {
            return EduPrograms.Find(EduProgramId).Variant.ProgramLimits.FirstOrDefault(m => m.ModuleId == ModuleId);
        }

        public string GetStudentsCountLimit(int EduProgramId, string ModuleId)
        {
            var programLimit = GetProgramLimit(EduProgramId, ModuleId);
            var variantLimits = GetLimitsByModuleAndProgram(EduProgramId, ModuleId);

            var programLimitCount = (programLimit == null ? 0 : programLimit.StudentsCount);
            var variantLimitsCount = variantLimits.Sum(m => (int?)m.StudentsCount) ?? 0;

            var limitCount = programLimitCount - variantLimitsCount;

            return String.Format((limitCount < 0 ? "Превышение:" : "Свободно") + " {0}", Math.Abs(limitCount));
        }

        public string[] GetFamilirizationTechs()
        {
            return FamilirizationTechs.Select(ft => ft.Name).ToArray();
        }

        public EduProgram CopyProgramAndSave(EduProgram src, int year)
        {
            var dst = new EduProgram
            {
                qualification = src.qualification,
                familirizationType = src.familirizationType,
                familirizationCondition = src.familirizationCondition,
                profileId = src.profileId,
                directionId = src.directionId,
                divisionId = src.divisionId,
                departmentId = src.departmentId,
                chairId = src.chairId,
                HeadFullName = src.HeadFullName,
                IsNetwork = src.IsNetwork,
                Year = year,
                Name = src.Name,
                State = VariantState.Development,
            };
            EduPrograms.Add(dst);

            dst.Variants = new List<Variant>();

            var variantTranslation = new Dictionary<int, Variant>();

            foreach (var srcVariant in src.Variants)
            {
                var dstVariant = new Variant();
                CopyVariantData(srcVariant, dstVariant);
                variantTranslation[srcVariant.Id] = dstVariant;
                dst.Variants.Add(dstVariant);
                /*if (srcVariant.IsBase)
                    dst.Variant = dstVariant;*/ //NOTE: AB: Эта операция создаёт циклическую ссылки изза чего сохранение невозможно.
                Variants.Add(dstVariant);
            }
            SaveChanges();

            dst.Variant = dst.Variants.First(v => v.IsBase);
            SaveChanges();


            /*  Приоритеты пока не получается перенести без реакторинга БД
                        foreach (var srcSvs in StudentVariantSelections.Where(svs => svs.Variant.EduProgramId == src.Id).ToList())
                        {
                            var dstSvs = new StudentVariantSelection
                            {
                                studentId = srcSvs.studentId,
                                selectedVariantId = variantTranslation[srcSvs.selectedVariantId].Id,
                                selectedVariantPriority = srcSvs.selectedVariantPriority
                            };

                            StudentVariantSelections.Add(dstSvs);
                        }

                        foreach (var srcSsp in StudentSelectionPriority.Where(ssp => ssp.Variant.EduProgramId == src.Id).Include(ssp=>ssp.VariantContent.moduleId))
                        {
                            var variant = variantTranslation[srcSsp.variantId];
                            var vc = variant.Groups.SelectMany(g=>g.Contents).FirstOrDefault(c=>c.moduleId==srcSsp.VariantContent.moduleId);
                            if(vc==null)
                                continue;
                            var dstSsp = new StudentSelectionPriority
                            {
                                studentId = srcSsp.studentId,
                                proprity = srcSsp.proprity,
                                variantId = variant.Id,
                                variantContentId = vc.Id
                            };

                            StudentSelectionPriority.Add(dstSsp);
                        }*/

            foreach (var sva in VariantAdmissions.Where(va => va.Variant.EduProgramId == src.Id).ToList())
            {
                var dva = new VariantAdmission
                {
                    studentId = sva.studentId,
                    Status = sva.Status,
                    variantId = variantTranslation[sva.variantId].Id,
                    Published = sva.Published,
                };

                VariantAdmissions.Add(dva);
            }
            /*  КЕопирвоание зачисления в модули не требуется
                        foreach (var sva in ModuleAdmissions.Where(va => va.Student.VariantAdmissions.Any(va=>va.Variant.EduProgramId==src.Id)).ToList())
                        {
                            var dva = new ModuleAdmission
                            {
                                studentId = sva.studentId,
                                Status = sva.Status,
                                moduleId = sva.moduleId,
                                Published = sva.Published,
                            };

                            ModuleAdmissions.Add(dva);
                        }*/

            //SaveChanges();

            return dst;
        }

        public bool IsMinorAccessible(IPrincipal principal, string minorID)
        {
            if (principal.IsInRole(ItsRoles.AllMinors)) return true;

            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userName = claim.Value;

            var access = Users.Single(u => u.UserName == userName).Minors.Any(m => m.ModuleId == minorID);

            return access;
        }

        public bool IsSectionFKAccessible(IPrincipal principal, string sectionFKId)
        {
            return true;
        }
        public bool IsForeignLanguageAccessible(IPrincipal principal, string foreignLanguageId)
        {
            return true;
        }

        public string CreateContractKsNumber(int year, string instituteTitle = null)
        {
            int number = 0;
            //int year = DateTime.Now.Month < 7 ? DateTime.Now.Year - 1 : DateTime.Now.Year;

            var lastContract = Contracts.Where(c => c.IsShortDated && c.SerialNumber != null && c.Year == year).OrderByDescending(c => c.SerialNumber.Value).FirstOrDefault();
            if (lastContract != null)
                number = lastContract.SerialNumber.Value;

            instituteTitle = instituteTitle == null ? "" : instituteTitle + "/";

            string contractNumber = "";
            var exist = false;
            do
            {
                number++;
                contractNumber = $"{year}/{instituteTitle}{number}к";
                exist = Contracts.FirstOrDefault(c => c.Number == contractNumber) != null;
            }
            while (exist);
            
            return contractNumber;
        }

        public int GetNextSerialNumberKsContract(int year)
        {
            var lastContract = Contracts.Where(c => c.IsShortDated && c.SerialNumber != null && c.Year == year).OrderByDescending(c => c.SerialNumber.Value).FirstOrDefault();
            return lastContract != null ? lastContract.SerialNumber.Value + 1 : 1;
        }

        public IQueryable<Company> PracticeCompanies()
        {
            return Companies.Where(c => c.Source == Source.Practice);
        }

        public IQueryable<Company> ProjectCompanies()
        {
            return Companies.Where(c => c.Source == Source.Project);
        }

        public IQueryable<Module> UniModules()
        {
            return Modules.Where(c => c.Source == Source.Uni);
        }

        public IQueryable<Module> ProjectModules()
        {
            return Modules.Where(c => c.Source == Source.Project);
        }


        /// <summary>
        /// Модули по проектному обучению из uni.
        /// type "Проектное обучение", "Парный модуль"
        /// </summary>
        /// <returns></returns>
        public IQueryable<Module> UniProjectModules()
        {
            return Modules.Where(c => c.type == "Парный модуль" || c.type == "Проектное обучение");
        }

        /// <summary>
        /// Модули по проектному обучению из uni для добавления связей.
        /// type "Проектное обучение" уровня А, "Парный модуль"
        /// </summary>
        /// <returns></returns>
        public IQueryable<Module> UniProjectModulesForConnection()
        {
            return Modules.Where(c => c.type == "Парный модуль" 
                        || c.type == "Проектное обучение" && (c.Level == "A" || c.Level == "А")); // первая А англ., вторая - рус.
        }

        public IQueryable<Project> ProjectsForUser(IPrincipal principal, bool includePairedModule = false)
        {
            var projects = Enumerable.Empty<Project>().ToList();
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            string userId = claim.Value;

            var userDirections = DirectionsForUser(principal).Select(d => d.uid).ToList();
            Expression<Func<Project, bool>> conditionExpression = p => p.Module.Source == Source.Project
                                                                || includePairedModule && p.Module.type == "Парный модуль";

            if (principal.IsInRole(ItsRoles.ProjectManager) ||
                (principal.IsInRole(ItsRoles.ProjectView) && !principal.IsInRole(ItsRoles.ProjectROP) && !principal.IsInRole(ItsRoles.ProjectCurator)))
            {
                return Projects.Where(conditionExpression);
            }

            if (principal.IsInRole(ItsRoles.ProjectROP))
            {
                projects.AddRange(ProjectUsers.Where(p => p.Type == ProjectUserType.ROP && p.Teacher.UserId == userId).Select(p => p.Project).ToList());
                if (includePairedModule)
                    projects.AddRange(Projects.Where(p => p.Module.type == "Парный модуль" && p.Module.Directions.Any(d => userDirections.Contains(d.uid))));
            }
            if (principal.IsInRole(ItsRoles.ProjectCurator))
            {
                projects.AddRange(ProjectUsers.Where(p => p.Type == ProjectUserType.Curator && p.Teacher.UserId == userId).Select(p => p.ProjectProperty.Project).ToList());
            }

            return projects.Distinct().Where(p => p != null).AsQueryable().Where(conditionExpression);
        }

        public IQueryable<ProjectProperty> ProjectPropertiesForUser(IPrincipal principal, bool includePairedModule = false)
        {
            var projectsForUser = ProjectsForUser(principal, includePairedModule).Select(p => p.ModuleId).ToList();
            var properties = ProjectProperties.Where(p => projectsForUser.Contains(p.ProjectId));

            return properties;
        }

        public IQueryable<Profile> ProjectProfilesForUser(IPrincipal principal, string projectId = null, bool includeCurators = false)
        {
            var profiles = Enumerable.Empty<Profile>().AsQueryable();
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            if (includeCurators && principal.IsInRole(ItsRoles.ProjectCurator)
                && ProjectUsers.Any(p => p.ProjectProperty.ProjectId == projectId && p.Teacher.UserId == userId))
            {
                var directionsForUser = DirectionsForUser(principal).Select(d => d.uid).ToList();
                profiles = Profiles.Where(p => directionsForUser.Contains(p.DIRECTION_ID));
            }

            var projectUserROP = ProjectUsers.FirstOrDefault(p => p.ProjectId == projectId && p.Teacher.UserId == userId && p.Type == ProjectUserType.ROP);
            if (principal.IsInRole(ItsRoles.ProjectROP) && projectUserROP != null)
            {
                profiles = projectUserROP.IsChief ? Profiles
                    : ProjectROPProfiles.Where(p => p.ProjectUser.Teacher.UserId == userId).Select(p => p.Profile);
            }
           
            if (principal.IsInRole(ItsRoles.ProjectManager) ||
                (principal.IsInRole(ItsRoles.ProjectView) && !principal.IsInRole(ItsRoles.ProjectROP)))
            {
                profiles = Profiles;
            }

            return profiles;
        }

        public IQueryable<ProjectSubgroup> ProjectSubgroupsForUser(IPrincipal principal)
        {
            var subgroups = Enumerable.Empty<ProjectSubgroup>().AsQueryable();
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            if (principal.IsInRole(ItsRoles.ProjectCurator))
            {
                subgroups = ProjectSubgroups.Where(p => p.Teacher.UserId == userId);
            }

            if (principal.IsInRole(ItsRoles.ProjectROP))
            {
                var projects = ProjectsForUser(principal, includePairedModule: true).Select(p => p.ModuleId).ToList();
                subgroups = ProjectSubgroups.Where(p => projects.Contains(p.Meta.ProjectDisciplineTmerPeriod.Period.ProjectId));
            }

            if (principal.IsInRole(ItsRoles.ProjectManager) ||
                (principal.IsInRole(ItsRoles.ProjectView) && !principal.IsInRole(ItsRoles.ProjectROP) && !principal.IsInRole(ItsRoles.ProjectCurator)))
            {
                subgroups = ProjectSubgroups;
            }

            return subgroups;
        }

        public IQueryable<ProjectCompetitionGroup> ProjectCompetitionGroupsForUser(IPrincipal principal)
        {
            var directionsForUser = DirectionsForUser(principal).Select(d => d.uid).ToList();
            var groupsForUser = GroupsForUser(principal).Where(g => directionsForUser.Contains(g.Profile.DIRECTION_ID)).Select(g => g.Id).ToList();

            IEnumerable<ProjectCompetitionGroup> projectGroups = ProjectPropertiesForUser(principal, includePairedModule: true).Select(p => p.ProjectCompetitionGroup).Distinct().ToList();

            if (principal.IsInRole(ItsRoles.ProjectROP) || principal.IsInRole(ItsRoles.ProjectManager))
                projectGroups = projectGroups.Concat(
                    ProjectCompetitionGroups.Where(g => g.Groups.Count == 0 || g.Groups.Any(gr => groupsForUser.Contains(gr.Id))).ToList())
                    .Distinct();

            return projectGroups.AsQueryable();
        }

        public bool CanEditProject(IPrincipal user, string projectId)
        {
            var module = Modules.FirstOrDefault(m => m.uuid == projectId);
            var userDirections = DirectionsForUser(user).Select(d => d.uid).ToList();
            var claimsIdentity = (ClaimsIdentity) user.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            string userId = claim.Value;

            return user.IsInRole(ItsRoles.ProjectManager)

                // если пользователь имеет права РОПа и назначен РОПом на проект
                // если пользователь имеет права РОПа, тип модуля Парный модуль и направления модуля и пользователя имеют хотя бы одно общее направление
                || user.IsInRole(ItsRoles.ProjectROP) 
                        && (
                                ProjectUsers.Include(u => u.Teacher).Any(u =>
                                                        u.ProjectId == projectId
                                                        && u.Teacher.UserId == userId
                                                        && u.Type == ProjectUserType.ROP)
                                || module.type == "Парный модуль" && module.Directions.Any(d => userDirections.Contains(d.uid)));
        }

        public bool CanEditProjectCompetitionGroup(IPrincipal user, int projectGroupId)
        {
            var projectGroup = ProjectCompetitionGroups.FirstOrDefault(g => g.Id == projectGroupId);

            // если пользователь РОП хотя бы на одном из проектов данной ПГ, он может ее редактировать
            // если пользователь РОП и на проектную группу не назначена ни одна академ группа, он может ее редактировать
            return user.IsInRole(ItsRoles.ProjectManager)
                    || ProjectCompetitionGroupsForUser(user).FirstOrDefault(p => p.Id == projectGroupId).ProjectProperties.Any(p => CanEditProject(user, p.ProjectId))
                    || user.IsInRole(ItsRoles.ProjectROP) && projectGroup?.Groups?.Count == 0; 
        }

        public bool CanEditProjectGroup(IPrincipal user, int projectGroupId)
        {
            return user.IsInRole(ItsRoles.ProjectManager) || ProjectCompetitionGroupsForUser(user).Select(g => g.Id).ToList().Contains(projectGroupId);
        }


    }


    public class SemesterTestUnits
    {
        public Int32 Semester { get; set; }
        public Int32 TestUnits { get; set; }
    }
    public static class ContextExtensions
    {
        public static void AddOrUpdate(this ApplicationDbContext ctx, object entity)
        {
            var entry = ctx.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case EntityState.Added:
                    ctx.Add(entity);
                    break;
                case EntityState.Unchanged:
                    //item already in db no need to do anything  
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }    
}
