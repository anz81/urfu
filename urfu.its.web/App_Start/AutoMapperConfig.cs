using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using Newtonsoft.Json;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Models;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.App_Start;
using Urfu.Its.Web.Controllers;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web
{
    public class AutoMapperConfig
    {
        public static void Register()
        {
            ConfigureMappingFromServicesToDb();
            ConfigureMappingFromDbToApi();
            
            MapperConfiguration mc = new MapperConfiguration(new AutoMapper.Configuration.MapperConfigurationExpression());
            mc.AssertConfigurationIsValid();
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            //services.AddSingleton(mapper);

            //services.AddMvc();
        }

        private static void ConfigureMappingFromDbToApi()
        {
            var mce = new MapperConfigurationExpression();
            var mc = new MapperConfiguration(mce);
            var mapper = new Mapper(mc);
            Mapper.Map<Subgroup, SubgroupApiDto>(Subgroup, SubgroupApiDto)
                .ForMember(s=>s.groupId, opt=>opt.MapFrom(s=>s.Meta.groupId))
                .ForMember(s=>s.moduleId, opt=>opt.MapFrom(s=>s.Meta.moduleId))
                .ForMember(s=>s.selectable, opt=>opt.MapFrom(s=>s.Meta.Selectable))
                .ForMember(s=>s.programId, opt=>opt.MapFrom(s=>s.Meta.programId))
                .ForMember(s=>s.term, opt=>opt.MapFrom(s=>s.Meta.Term))
                .ForMember(s=>s.kmer, opt=>opt.MapFrom(s=>s.Meta.kmer))
                .ForMember(s => s.removed, opt => opt.MapFrom(s => s.Removed ? 1 : 0))
                .ForMember(s=>s.catalogDisciplineUuid, opt=>opt.MapFrom(s=>s.Meta.catalogDisciplineUuid))
                .ForMember(s=>s.disciplineUUID, opt=>opt.MapFrom(s=>s.Meta.disciplineUUID))
                .ForMember(s=>s.additionalUUID, opt=>opt.MapFrom(s=>s.Meta.additionalUUID))
                .ForMember(s=>s.year,opt=>opt.Ignore())
                .ForMember(s=>s.dckey,opt=>opt.Ignore())
                .ForMember(s=>s.detailDiscipline,opt=>opt.Ignore())
                .ForMember(s=>s.studentCount,opt=>opt.Ignore())
                .ForMember(s=>s.moduleName,opt=>opt.Ignore())
                .ForMember(s=>s.moduleNumber,opt=>opt.Ignore())
                .ForMember(s=>s.combinedKey,opt=>opt.Ignore())
                .ForMember(s => s.combinedKey2, opt => opt.Ignore());
            Mapper.CreateMap<Subgroup, SubgroupWithMemebersApiDto>()
                .ForMember(s=>s.groupId, opt=>opt.MapFrom(s=>s.Meta.groupId))
                .ForMember(s=>s.moduleId, opt=>opt.MapFrom(s=>s.Meta.moduleId))
                .ForMember(s=>s.selectable, opt=>opt.MapFrom(s=>s.Meta.Selectable))
                .ForMember(s=>s.programId, opt=>opt.MapFrom(s=>s.Meta.programId))
                .ForMember(s=>s.term, opt=>opt.MapFrom(s=>s.Meta.Term))
                .ForMember(s=>s.kmer, opt=>opt.MapFrom(s=>s.Meta.kmer))
                .ForMember(s=>s.catalogDisciplineUuid, opt=>opt.MapFrom(s=>s.Meta.catalogDisciplineUuid))
                .ForMember(s => s.disciplineUUID, opt => opt.MapFrom(s => s.Meta.disciplineUUID))
                .ForMember(s => s.additionalUUID, opt => opt.MapFrom(s => s.Meta.additionalUUID))
                .ForMember(s=>s.year,opt=>opt.Ignore())
                .ForMember(s=>s.dckey,opt=>opt.Ignore())
                .ForMember(s=>s.detailDiscipline,opt=>opt.Ignore())
                .ForMember(s=>s.studentCount,opt=>opt.Ignore())
                .ForMember(s=>s.moduleName,opt=>opt.Ignore())
                .ForMember(s=>s.students,opt=>opt.Ignore())
                .ForMember(s=>s.combinedKey,opt=>opt.Ignore())
                .ForMember(s => s.combinedKey2, opt => opt.Ignore());
            Mapper.CreateMap<Variant, VariantApiDto>()
                .ForMember(dst => dst.direction, option => option.MapFrom(variant => variant.Program.Direction))
                .ForMember(dst => dst.groups, option => option.MapFrom(variant => variant.Groups))
                .ForMember(dst => dst.variantName, option => option.MapFrom(variant => variant.Name))
                .ForMember(dst => dst.selectionGroups, option => option.MapFrom(variant => variant.SelectionGroups))
                .ForMember(dst => dst.id, option => option.MapFrom(variant => variant.Id))
                .ForMember(dst => dst.createDate, option => option.MapFrom(variant => variant.CreateDate))
                .ForMember(dst => dst.year, option => option.MapFrom(variant => variant.Program.Year))
                .ForMember(dst => dst.qualification, option => option.MapFrom(variant => variant.Program.qualification))
                .ForMember(dst => dst.familirizationTech, option => option.Ignore())
                .ForMember(dst => dst.familirizationType, option => option.MapFrom(variant => variant.Program.familirizationType))
                .ForMember(dst => dst.familirizationCondition, option => option.MapFrom(variant => variant.Program.familirizationCondition))
                .ForMember(dst => dst.eduProgramName, option => option.MapFrom(variant => variant.Program.Name))
                .ForMember(dst => dst.modules, option => option.MapFrom(variant => variant.Groups.SelectMany(g=>g.Contents).Where(c=>c.Selected)));

            Mapper.CreateMap<Direction, DirectionApiDto>();

            Mapper.CreateMap<VariantSelectionGroup, VariantSelectionGroupApiDto>()
                .ForMember(dst => dst.id, option => option.MapFrom(variantGroup => variantGroup.Id))
                .ForMember(dst => dst.groupName, option => option.MapFrom(variantGroup => variantGroup.Name))
                .ForMember(dst => dst.testUnits, option => option.MapFrom(variantGroup => variantGroup.TestUnits));

            Mapper.CreateMap<VariantGroup, VariantGroupApiDto>()
                .ForMember(dst => dst.id, option => option.MapFrom(variantGroup => variantGroup.Id))
                .ForMember(dst => dst.testUnits, option => option.MapFrom(variantGroup => variantGroup.TestUnits))
                .ForMember(dst => dst.groupName,option => option.MapFrom(variantGroup => variantGroup.GroupType.GetName()));

            Mapper.CreateMap<VariantContent, VariantContentApiDto>()
                .ForMember(dst => dst.id, option => option.MapFrom(src => src.Id))
                .ForMember(dst => dst.selectable, option => option.MapFrom(src => src.Selectable))
                .ForMember(dst => dst.limits, option => option.MapFrom(src =>

                     src.Group.Variant.ProgramLimits.Where(l => l.ModuleId == src.moduleId).Select(l => l.StudentsCount).FirstOrDefault()
                ))
                .ForMember(dst => dst.moduleUuid, option => option.MapFrom(src => src.moduleId))
                .ForMember(dst => dst.requiredVariantContentIds,
                    option => option.MapFrom(src => src.Requirments.Select(r => r.Id)))
                .ForMember(dst => dst.requirements,
                    option => option.MapFrom(src => src.Requirments.Select(r => new ModueRequirementApiDto
                    {
                        variantContentId = r.Id,
                        number = r.Module.number,
                        moduleTitle = r.Module.title,
                        moduleUuid = r.Module.uuid
                    })))
                .ForMember(dst => dst.title, option => option.MapFrom(src => src.Module.title))
                .ForMember(dst => dst.shortTitle, option => option.MapFrom(src => src.Module.shortTitle))
                .ForMember(dst => dst.number, option => option.MapFrom(src => src.Module.number))
                .ForMember(dst => dst.coordinator, option => option.MapFrom(src => src.Module.coordinator))
                .ForMember(dst => dst.type, option => option.MapFrom(src => src.Module.type))
                .ForMember(dst => dst.testUnits, option => option.MapFrom(src => src.Module.testUnits))
                .ForMember(dst => dst.priority, option => option.MapFrom(src => src.Module.priority))
                .ForMember(dst => dst.state, option => option.MapFrom(src => src.Module.state))
                .ForMember(dst => dst.approvedDate, option => option.MapFrom(src => src.Module.approvedDate))
                .ForMember(dst => dst.comment, option => option.MapFrom(src => src.Module.comment))
                .ForMember(dst => dst.file, option => option.MapFrom(src => src.Module.file))
                .ForMember(dst => dst.specialities, option => option.MapFrom(src => src.Module.specialities))
                .ForMember(dst => dst.disciplines, option => option.MapFrom(src => src.Module.disciplines))
                .ForMember(dst => dst.competence, option => option.MapFrom(src => src.Module.competence))
                .ForMember(dst => dst.plans, option => option.MapFrom(src =>
                    src.Module.Plans.Where(p => p.directionId == src.Group.Variant.Program.directionId)
                    .Where(p => p.qualification == src.Group.Variant.Program.qualification &&
                                p.familirizationType == src.Group.Variant.Program.familirizationType &&
                                p.familirizationCondition == src.Group.Variant.Program.familirizationCondition &&
                                p.versionNumber == src.Group.Variant.Program.PlanVersionNumber &&
                                p.eduplanNumber == src.Group.Variant.Program.PlanNumber &&
                                (p.faculty == src.Group.Variant.Program.divisionId || p.faculty == src.Group.Variant.Program.departmentId || p.faculty == src.Group.Variant.Program.chairId) 
                                && p.active && !p.remove
                    )))
                .ForMember(dst => dst.groupId, option => option.MapFrom(src => src.VariantGroupId))
                .ForMember(dst => dst.selectionGroupId, option => option.MapFrom(src => src.VariantSelectionGroupId))
                .ForMember(dst => dst.variantContentType, option => option.MapFrom(src => src.ModuleType.Name));

            Mapper.CreateMap<Discipline, DisciplineApiDto>();

            Mapper.CreateMap<Plan, PlanApiDto>()
                .ForMember(dst=>dst.versionNumber,opt=>opt.MapFrom(src=>src.versionTitle))
                .ForMember(dst => dst.allTerms,
                    option => option.MapFrom(src => JsonConvert.DeserializeObject<int[]>(src.allTermsExtracted)))
                .ForMember(dst => dst.controls,
                    option =>
                        option.MapFrom(src => JsonConvert.DeserializeObject<List<Dictionary<string, List<int>>>>(src.controls)))
                        .ForMember(src=>src.teachers,opt=>opt.Ignore());

            Mapper.CreateMap<PlanAdditionalDto, PlanAdditional>()
                .ForMember(dst => dst.disciplineUUID, opt => opt.MapFrom(src => src.id))
                .ForMember(dst => dst.versionUUID, opt => opt.MapFrom(src => src.version))
                .ForMember(dst => dst.discipline, opt => opt.MapFrom(src => src.title))
                .ForMember(dst => dst.allload, opt => opt.MapFrom(src => src.allload != "null" ? (int?)int.Parse(src.allload) : null))
                .ForMember(dst => dst.allaudit, opt => opt.MapFrom(src => src.allaudit != "null" ? (int?)int.Parse(src.allaudit) : null))
                .ForMember(dst => dst.self, opt => opt.MapFrom(src => src.self != "null" ? (int?)int.Parse(src.self) : null))
                .ForMember(dst => dst.lecture, opt => opt.MapFrom(src => src.lecture != "null" ? (int?)int.Parse(src.lecture) : null))
                .ForMember(dst => dst.practice, opt => opt.MapFrom(src => src.practice != "null" ? (int?)int.Parse(src.practice) : null))
                .ForMember(dst => dst.labs, opt => opt.MapFrom(src => src.labs != "null" ? (int?)int.Parse(src.labs) : null))
                .ForMember(dst => dst.contactTotal, opt => opt.MapFrom(src => src.contactTotal != "null" ? (Decimal?)Decimal.Parse(src.contactTotal,CultureInfo.InvariantCulture) : null))
                .ForMember(dst => dst.contactSelf, opt => opt.MapFrom(src => src.contactSelf != "null" ? (Decimal?)Decimal.Parse(src.contactSelf, CultureInfo.InvariantCulture) : null))
                .ForMember(dst => dst.contactControl, opt => opt.MapFrom(src => src.contactControl != "null" ? (Decimal?)Decimal.Parse(src.contactControl,CultureInfo.InvariantCulture) : null))
                .ForMember(dst => dst.contactLecture, opt => opt.MapFrom(src => src.contactLecture != "null" ? (Decimal?)Decimal.Parse(src.contactLecture, CultureInfo.InvariantCulture) : null))
                .ForMember(dst => dst.contactPractice, opt => opt.MapFrom(src => src.contactPractice != "null" ? (Decimal?)Decimal.Parse(src.contactPractice, CultureInfo.InvariantCulture) : null))
                .ForMember(dst => dst.contactLabs, opt => opt.MapFrom(src => src.contactLabs != "null" ? (Decimal?)Decimal.Parse(src.contactLabs, CultureInfo.InvariantCulture) : null))
                .ForMember(dst => dst.gosLoadInTestUnits, opt => opt.MapFrom(src => src.gosLoadInTestUnits != "null" && src.gosLoadInTestUnits != "" ? int.Parse(src.gosLoadInTestUnits) : 0 ))
                .ForMember(dst => dst.ttu1, opt => opt.MapFrom(src => src.ttu1 != "" ? int.Parse(src.ttu1) : 0 ))
                .ForMember(dst => dst.ttu2, opt => opt.MapFrom(src => src.ttu2 != "" ? int.Parse(src.ttu2) : 0 ))
                .ForMember(dst => dst.ttu3, opt => opt.MapFrom(src => src.ttu3 != "" ? int.Parse(src.ttu3) : 0 ))
                .ForMember(dst => dst.ttu4, opt => opt.MapFrom(src => src.ttu4 != "" ? int.Parse(src.ttu4) : 0 ))
                .ForMember(dst => dst.ttu5, opt => opt.MapFrom(src => src.ttu5 != "" ? int.Parse(src.ttu5) : 0 ))
                .ForMember(dst => dst.ttu6, opt => opt.MapFrom(src => src.ttu6 != "" ? int.Parse(src.ttu6) : 0 ))
                .ForMember(dst => dst.ttu7, opt => opt.MapFrom(src => src.ttu7 != "" ? int.Parse(src.ttu7) : 0 ))
                .ForMember(dst => dst.ttu8, opt => opt.MapFrom(src => src.ttu8 != "" ? int.Parse(src.ttu8) : 0 ))
                .ForMember(dst => dst.ttu9, opt => opt.MapFrom(src => src.ttu9 != "" ? int.Parse(src.ttu9) : 0 ))
                .ForMember(dst => dst.ttu10, opt => opt.MapFrom(src => src.ttu10 != "" ? int.Parse(src.ttu10) : 0 ))
                .ForMember(dst => dst.ttu11, opt => opt.MapFrom(src => src.ttu11 != "" ? int.Parse(src.ttu11) : 0 ))
                .ForMember(dst => dst.ttu12, opt => opt.MapFrom(src => src.ttu12 != "" ? int.Parse(src.ttu12) : 0 ))
                ;



            Mapper.CreateMap<VariantAdmission, VariantAdmissionDto>()
                .ForMember(dst => dst.variantId,
                option => option.MapFrom(src => src.variantId));
            Mapper.CreateMap<ModuleAdmission, ModuleAdmissionDto>();
            Mapper.CreateMap<AdmissionStatus, AdmissionStatusDto>();
            Mapper.CreateMap<Division, DivisionApiDto>();
            Mapper.CreateMap<EduProgram, ProgramApiDto>();

            Mapper.CreateMap<VariantsUni, TrajectoryDto>()
                .ForMember(dst => dst.trajectory_uuid, opt => opt.MapFrom(src => src.TrajectoryUuid))
                .ForMember(dst => dst.externalid, opt => opt.MapFrom(src => src.VariantId))
                .ForMember(dst => dst.specialization_uuid, opt => opt.MapFrom(src => src.ProfileId))
                .ForMember(dst => dst.documentname, opt => opt.MapFrom(src => src.DocumentName));

            Mapper.CreateMap<BasicCharacteristicOPRatifyData, RatifyingInfo>();
        }

        private static void ConfigureMappingFromServicesToDb()
        {
            Mapper.CreateMap<ProfileXmlDto, DataContext.Profile>()
                .ForMember(dst=>dst.Direction,opt=>opt.Ignore())
                .ForMember(dst=>dst.Division,opt=>opt.Ignore())
                .ForMember(dst=>dst.remove,opt=>opt.Ignore());

            var standards = new Dictionary<string, string>() {
                { "ФГОС ВО", "ФГОС ВО" },
                { "ФГОС 3++", "ФГОС ВО 3++" },
                { "СУОС", "СУОС" }
            };
            
            Mapper.CreateMap<DirectionDto, Direction>()
                .ForMember(dst => dst.qualifications, option => option.MapFrom(src => string.Join(", ", src.qualifications)))
                .ForMember(dst => dst.Modules, option => option.Ignore())
                .ForMember(dst => dst.Programs, option => option.Ignore())
                .ForMember(dst => dst.Orders, option => option.Ignore())
                .ForMember(dst => dst.Users, option => option.Ignore())
                .ForMember(dst => dst.Profiles, option => option.Ignore())
                .ForMember(dst => dst.AreaEducationId, option => option.MapFrom(src => src.areaEducation.id))
                .ForMember(dst => dst.AreaEducation, option => option.MapFrom(src => src.areaEducation))
                .ForMember(dst => dst.standard, option => option.MapFrom(src => standards.ContainsKey(src.standard) ? standards[src.standard] : null));
                

            Mapper.CreateMap<DisciplineDto, Discipline>()
                .ForMember(dst => dst.Modules, option => option.Ignore())
                .ForMember(dst => dst.WorkingProgramResponsiblePersons, option => option.Ignore())
                .ForMember(dst => dst.pkey, option => option.MapFrom(src => src.discipline));

            Mapper.CreateMap<GroupXmlDto, Group>()
                .ForMember(dst=>dst.Profile,opt=>opt.Ignore())
                .ForMember(dst=>dst.SectionFkCompetitionGroups,opt=>opt.Ignore())
                .ForMember(dst=>dst.ForeignLanguageCompetitionGroups,opt=>opt.Ignore())
                .ForMember(dst => dst.ProjectCompetitionGroups, opt => opt.Ignore())
                .ForMember(dst => dst.MUPCompetitionGroups, opt => opt.Ignore());

            Mapper.CreateMap<StudentXmlDto, Student>()
                .ForMember(dst=>dst.Group,opt=>opt.Ignore())
                .ForMember(dst=>dst.Sportsman,opt=>opt.Ignore())
                .ForMember(dst=>dst.Selections,opt=>opt.Ignore())
                .ForMember(dst=>dst.Rating,opt=>opt.Ignore())
                .ForMember(dst=>dst.ForeignLanguageLevel,opt=>opt.Ignore())
                .ForMember(dst=>dst.ForeignLanguageRating,opt=>opt.Ignore())
                .ForMember(dst=>dst.ForeignLanguageTargetLevel,opt=>opt.Ignore())
                .ForMember(dst=>dst.RatingType,opt=>opt.Ignore())
                .ForMember(dst=>dst.planVerion,opt=>opt.Ignore())
                .ForMember(dst=>dst.sectionFKDebtTerms, opt=>opt.Ignore())
                .ForMember(dst=>dst.versionNumber,opt=>opt.Ignore())
                .ForMember(dst=>dst.Person,opt=>opt.Ignore())
                .ForMember(dst=>dst.VariantAdmissions,opt=>opt.Ignore())
                .ForMember(dst=>dst.ModuleAdmissions,opt=>opt.Ignore())
                .ForMember(dst=>dst.MinorAdmissions, opt=>opt.Ignore())
                .ForMember(dst=>dst.SectionFKAdmissions, opt=>opt.Ignore())
                .ForMember(dst=>dst.ForeignLanguageAdmissions, opt=>opt.Ignore())
                .ForMember(dst=>dst.ProjectAdmissions, opt => opt.Ignore())
                .ForMember(dst=>dst.MUPAdmissions, opt => opt.Ignore())
                .ForMember(dst=>dst.Practices, opt=>opt.Ignore())
                .ForMember(dst=>dst.MinorSelections, opt=>opt.Ignore())
                .ForMember(dst=>dst.SectionFKSelections, opt=>opt.Ignore())
                .ForMember(dst=>dst.ForeignLanguageSelections, opt=>opt.Ignore())
                .ForMember(dst=>dst.ProjectSelections, opt => opt.Ignore())
                .ForMember(dst=>dst.Male, opt=>opt.MapFrom(x=>x.Sex!=0))
                .ForMember(dst=>dst.SelectionJson,opt=>opt.Ignore());
            
            Mapper.CreateMap<PersonXmlDto, Person>();

            Mapper.CreateMap<ApploadDto, Appload>()
                .ForMember(dst=>dst.grp,opt=>opt.MapFrom(src=>src.group))
                .ForMember(dst=>dst.duModule,opt=>opt.MapFrom(src=>src.eduModule))
                .ForMember(dst=>dst.year,opt=>opt.Ignore())
                .ForMember(dst => dst.DisciplineType, opt => opt.Ignore())
                .ForMember(dst=>dst.term,opt=>opt.Ignore())
                .ForMember(dst => dst.Level, opt => opt.MapFrom(src => src.TypeProject));

            Mapper.CreateMap<GroupHistoryDto, GroupsHistory>()
                .ForMember(dst => dst.GroupId, opt => opt.MapFrom(src => src.group))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.code))
                .ForMember(dst => dst.Course, opt => opt.MapFrom(src => src.course))
                .ForMember(dst => dst.Qual, opt => opt.MapFrom(src => src.qualification.ToUpperFirstLetter()))
                .ForMember(dst => dst.FamTech,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.familirizationTech == "обычная"
                                    ? "Традиционная"
                                    : src.familirizationTech.ToUpperFirstLetter()))
                .ForMember(dst => dst.FamType, opt => opt.MapFrom(src => src.familirizationForm.ToUpperFirstLetter()))
                .ForMember(dst => dst.ManagingDivisionId, opt => opt.MapFrom(src => src.formativeDivision))
                .ForMember(dst => dst.ChairId, opt => opt.MapFrom(src => src.parent))
                .ForMember(dst => dst.ProfileId, opt => opt.MapFrom(src => src.specialization))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.title))
                .ForMember(dst => dst.YearHistory, opt => opt.MapFrom(src => src.year))
                .ForMember(dst => dst.Profile, opt => opt.Ignore())
                .ForMember(dst => dst.Group, opt => opt.Ignore());
                



            Mapper.CreateMap<DivisionDto, Division>()
                .ForMember(dst => dst.MinorDisciplineTmerPeriod, opt => opt.Ignore())
                .ForMember(dst => dst.SectionFKDisciplineTmerPeriod, opt => opt.Ignore())
                .ForMember(dst => dst.ForeignLanguageDisciplineTmerPeriods, opt => opt.Ignore())
                .ForMember(dst => dst.ProjectDisciplineTmerPeriods, opt => opt.Ignore())
                .ForMember(dst => dst.MUPDisciplineTmerPeriods, opt => opt.Ignore())
                .ForMember(dst => dst.ParentDivision, option => option.Ignore())
                .ForMember(dst => dst.Users,opt=>opt.Ignore());

            Mapper.CreateMap<ModuleDto, Module>()
                .ForMember(dst => dst.disciplines, option => option.MapFrom(src => src.disciplines))
                .ForMember(dst => dst.specialities, option => option.MapFrom(src => string.Join(", ", src.specialities.Distinct().OrderBy(s => s))))
                .ForMember(dst => dst.Directions, option => option.Ignore())
                .ForMember(dst => dst.UsedInVariantContents, option => option.Ignore())
                .ForMember(dst => dst.Minor, option => option.Ignore())
                .ForMember(dst => dst.SectionFk, option => option.Ignore())
                .ForMember(dst => dst.ForeignLanguage, option => option.Ignore())
                .ForMember(dst => dst.Users, option => option.Ignore())
                .ForMember(dst => dst.RequiredFor, option => option.Ignore())
                .ForMember(dst => dst.Plans, option => option.Ignore())
                .ForMember(dst => dst.Source, option => option.Ignore())
                .ForMember(dst => dst.Project, option => option.Ignore())
                .ForMember(dst => dst.Level, option => option.MapFrom(src => src.typeProject))
                .ForMember(dst => dst.MUP, option => option.Ignore());

            Mapper.CreateMap<PlanDto, Plan>()
                .ForMember(dst => dst.controls, option => option.MapFrom(src => JsonConvert.SerializeObject(src.controls)))
                .ForMember(dst => dst.testUnitsByTerm, option => option.MapFrom(src => JsonConvert.SerializeObject(src.testUnitsByTerm)))
                .ForMember(dst => dst.terms, option => option.MapFrom(src => JsonConvert.SerializeObject(src.terms)))
                .ForMember(dst => dst.loads, option => option.MapFrom(src => JsonConvert.SerializeObject(src.loads)))
                .ForMember(dst => dst.allTermsExtracted,
                    option =>
                        option.MapFrom(
                            src =>
                                JsonConvert.SerializeObject(
                                    src.terms.Union(src.controls.SelectMany(c => c.Values).SelectMany(l => l)))))
                .ForMember(dst => dst.directionId, option => option.Ignore())
                .ForMember(dst => dst.Module, option => option.Ignore())
                .ForMember(dst => dst.FacultyDivision, option => option.Ignore())
                .ForMember(dst => dst.Teachers, option => option.Ignore())
                .ForMember(dst => dst.active, option => option.MapFrom(src => src.versionStatus == "Утверждено"))
                .ForMember(dst => dst.remove, option => option.Ignore())
                .ForMember(dst => dst.additionalWeeks, option => option.MapFrom(src => src.additionalWeeks));
            
            //Mapper.CreateMap<IEnumerable<PlanDto>, List<PlanTerm>>()
            //    .ConvertUsing<MyTypeConverter>();

            Mapper.CreateMap<IEnumerable<PlanTermsDto>, List<PlanTerm>>()
                .ConvertUsing<PlanDtoTypeConverter>();

            Mapper.CreateMap<TeacherDto, Teacher>()
                .ForMember(dst => dst.AccountancyGuid, option => option.MapFrom(src => src.accountancyGuid))
                .ForMember(dst => dst.SectionFKProperties, option => option.Ignore())
                .ForMember(dst => dst.ForeignLanguageProperties, option => option.Ignore())
                .ForMember(dst => dst.MUPProperties, option => option.Ignore())
                .ForMember(dst => dst.academicDegree, option => option.MapFrom(src => src.academicDegree))
                .ForMember(dst => dst.academicTitle, option => option.MapFrom(src => src.academicTitle))
                .ForMember(dst => dst.UserId, option => option.Ignore())
                .ForMember(dst => dst.User, option => option.Ignore())
                .ForMember(dst => dst.Practices, option => option.Ignore());

            Mapper.CreateMap<MinorPeriod, PeriodApiDto>()
                .ForMember(dst => dst.year, option => option.MapFrom(src => src.Year))
                .ForMember(dst => dst.semester, option => option.MapFrom(src => src.Semester.Name))
                .ForMember(dst => dst.selectionDeadline, option => option.MapFrom(src => src.SelectionDeadline))
                .ForMember(dst => dst.minStudentCount, option => option.MapFrom(src => src.MinStudentsCount))
                .ForMember(dst => dst.maxStudentCount, option => option.MapFrom(src => src.MaxStudentsCount));


            Mapper.CreateMap<MinorDisciplineTmerPeriod, MinorDisciplineTmerPeriodApiDto>()
                .ForMember(dst => dst.year, option => option.MapFrom(src => src.Period.Year))
                .ForMember(dst => dst.semester, option => option.MapFrom(src => src.Period.Semester.Name))
                .ForMember(dst => dst.chairs, option => option.MapFrom(src => src.Divisions.Select(d=> $"{d.ParentName()}/{d.typeTitle} {d.title}")));

            Mapper.CreateMap<MinorDisciplineTmer, MinorDisciplineTmerApiDto>()
                .ForMember(dst => dst.rmer, option => option.MapFrom(src => src.Tmer.rmer))
                .ForMember(dst => dst.periods, option => option.MapFrom(src => src.Periods));

            Mapper.CreateMap<MinorDiscipline, MinorDisciplineApiDto>()
                .ForMember(dst => dst.uid, opt => opt.MapFrom(src => src.Discipline.uid))
                .ForMember(dst => dst.title, opt => opt.MapFrom(src => src.Discipline.title))
                .ForMember(dst => dst.section, opt => opt.MapFrom(src => src.Discipline.section))
                .ForMember(dst => dst.testUnits, opt => opt.MapFrom(src => src.Discipline.testUnits))
                .ForMember(dst => dst.file, opt => opt.MapFrom(src => src.Discipline.file))
                .ForMember(dst => dst.number, opt => opt.MapFrom(src => src.Discipline.number))
                .ForMember(dst => dst.tmers, opt => opt.MapFrom(src => src.Tmers));

            Mapper.CreateMap<Module, ModuleApiDto>()
                .ForMember(dst => dst.disciplines, option => option.MapFrom(src => src.GetMinorDisciplines()))
                .ForMember(dst => dst.tech, option => option.MapFrom(src => src.GetTechName()))
                .ForMember(dst => dst.showInLC, option => option.MapFrom(src => src.GetShowInLC()))
                .ForMember(dst => dst.requirmentId, option => option.MapFrom(src => src.Minor == null ? null : src.Minor.GetRequirmentId()))
                .ForMember(dst => dst.requirmentTitle, option => option.MapFrom(src => src.Minor == null ? null : src.Minor.GetRequirmentTitle()))
                .ForMember(dst => dst.period, option => option.Ignore());

            Mapper.CreateMap<Module, MinorApiDto>()
                .ForMember(dst => dst.disciplines, option => option.MapFrom(src => src.GetMinorDisciplines()))
                .ForMember(dst => dst.tech, option => option.MapFrom(src => src.GetTechName()))
                .ForMember(dst => dst.showInLC, option => option.MapFrom(src => src.GetShowInLC()))
                .ForMember(dst => dst.requirmentId, option => option.MapFrom(src => src.Minor == null ? null : src.Minor.GetRequirmentId()))
                .ForMember(dst => dst.requirmentTitle, option => option.MapFrom(src => src.Minor == null ? null : src.Minor.GetRequirmentTitle()))
                .ForMember(dst => dst.period, option => option.Ignore())
                .ForMember(dst => dst.agreement, option => option.Ignore());
            
            Mapper.CreateMap<ForeignLanguagePeriod, PeriodApiDto>()
                .ForMember(dst => dst.year, option => option.MapFrom(src => src.Year))
                .ForMember(dst => dst.semester, option => option.MapFrom(src => src.Semester.Name))
                .ForMember(dst => dst.selectionDeadline, option => option.MapFrom(src => src.SelectionDeadline))
                .ForMember(dst => dst.minStudentCount, option => option.Ignore())
                .ForMember(dst => dst.maxStudentCount, option => option.Ignore());


            Mapper.CreateMap<DirectorDto, Director>()
                .ForMember(dst => dst.DivisionUuid, option => option.MapFrom(src => src.divisionUUID))
                .ForMember(dst => dst.Division, option => option.Ignore())
                .ForMember(dst => dst.Surname, option => option.MapFrom(src => src.lastName))
                .ForMember(dst => dst.Name, option => option.MapFrom(src => src.firstName))
                .ForMember(dst => dst.PatronymicName, option => option.MapFrom(src => src.middleName));

            Mapper.CreateMap<ModuleAgreementDto, ModuleAgreement>()
                .ForMember(dst => dst.UniId, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.SemesterId, opt => opt.Ignore())
                .ForMember(dst => dst.Semester, opt => opt.Ignore())
                .ForMember(dst => dst.Module, opt => opt.Ignore())
                .ForSourceMember(st => st.Terms, opt => opt.Ignore());

            Mapper.CreateMap<ModuleAgreement, ModuleAgreementApiDto>();

            Mapper.CreateMap<ProjectRoleApiDto, ProjectRole>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.Project, opt => opt.Ignore())
                .ForMember(dst => dst.ProjectId, opt => opt.Ignore())
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.title))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dst => dst.EmployersId, opt => opt.MapFrom(src => src.id));

            Mapper.CreateMap<EduProgram, EduProgramVM>()
                .ForMember(dst => dst.RequiredPlanNumber, opt => opt.MapFrom(src => src.PlanNumber))
                .ForMember(dst => dst.RequiredPlanVersionNumber, opt => opt.MapFrom(src => src.PlanVersionNumber));

            Mapper.CreateMap<TmerDto, Tmer>();

            Mapper.CreateMap<AreaEducationDto, AreaEducation>()
                .ForMember(dst => dst.Code, opt => opt.MapFrom(src => src.code))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.title))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dst => dst.Orders, opt => opt.Ignore())
                .ForMember(dst => dst.Competences, opt => opt.Ignore());

            Mapper.CreateMap<TrajectoryDto, VariantsUni>()
                .ForMember(dst => dst.TrajectoryUuid, opt => opt.MapFrom(src => src.trajectory_uuid))
                .ForMember(dst => dst.VariantId, opt => opt.MapFrom(src => src.externalid))
                .ForMember(dst => dst.ProfileId, opt => opt.MapFrom(src => src.specialization_uuid))
                .ForMember(dst => dst.DocumentName, opt => opt.MapFrom(src => src.documentname))
                .ForMember(dst => dst.Profile, opt => opt.Ignore())
                .ForMember(dst => dst.Variant, opt => opt.Ignore());
        }
    }

    public class MyTypeConverter : ITypeConverter<IEnumerable<PlanDto>, List<PlanTerm>>
    {
        public List<PlanTerm> Convert(IEnumerable<PlanDto> pd, List<PlanTerm> pt, ResolutionContext context)
        {
            if (context == null || pd == null)
                return null;

            //var source = context.SourceValue as IEnumerable<PlanDto>;

            return pd
                .SelectMany(s => s.termsCount
                    .Select(o => new PlanTerm()
                    {
                        TermsCount = o.Value,
                        Year = Int32.Parse(o.Key),
                        eduplanUUID = s.eduplanUUID
                    }))
                .ToList();
        }
    }

    public class PlanDtoTypeConverter : ITypeConverter<IEnumerable<PlanTermsDto>, List<PlanTerm>>
    {
        public List<PlanTerm> Convert(IEnumerable<PlanTermsDto> ptd, List<PlanTerm> pt, ResolutionContext context)
        {
            if (context == null || ptd == null)
                return null;

            return ptd
                .SelectMany(p => p.TermsCount
                    .Select(t => new PlanTerm()
                    {
                        TermsCount = t.TermsCount,
                        Year = t.Year,
                        eduplanUUID = p.eduplanUUID
                    }))
                .ToList();
        }

        List<PlanTerm> ITypeConverter<IEnumerable<PlanTermsDto>, List<PlanTerm>>.Convert(IEnumerable<PlanTermsDto> source, List<PlanTerm> destination, ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    class String2NullableIntResolver : IValueResolver<string, int?, int?>
    {
        public int? Resolve(string source, int? dest, int? destm, ResolutionContext context)
        {
            return ResolveCore(source);
        }

        protected int? ResolveCore(string source)
        {
            // do something
            return (!String.IsNullOrEmpty(source) && source != "null") ? 
                            (int?)int.Parse(source) : null ;
        }
    }
    class String2NullableDecimalResolver : IValueResolver<string, Decimal?, Decimal?>
    {
        public Decimal? Resolve(string source, Decimal? dest, Decimal? r, ResolutionContext context)
        {
            return ResolveCore(source);
        }

        protected Decimal? ResolveCore(string source)
        {
            // do something
            return (!String.IsNullOrEmpty(source) && source != "null") ?
                            (Decimal?)Decimal.Parse(source) : null;
        }
    }
}