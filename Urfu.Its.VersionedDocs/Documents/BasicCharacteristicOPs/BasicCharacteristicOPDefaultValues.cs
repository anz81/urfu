using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Model.Models.OHOPModels;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs
{
    public class BasicCharacteristicOPDefaultValues
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private BasicCharacteristicOPInfo info;

        private static readonly string universalCompetenceType = "УК";
        private static readonly string generalCompetenceType = "ОПК";
        
        public BasicCharacteristicOPDefaultValues(BasicCharacteristicOPInfo info)
        {
            this.info = info;
        }

        public RatifyingInfo RatifyingInfo()
        {
            var ratifyData = db.BasicCharacteristicOPRatifyData.FirstOrDefault(r => r.Year == info.Year)
                ?? db.BasicCharacteristicOPRatifyData.OrderByDescending(r => r.Year).First();
            var mce = new AutoMapper.Configuration.MapperConfigurationExpression();
            var config = new MapperConfiguration(mce);
            var mapper = new Mapper(config);

            return mapper.Map<RatifyingInfo>(ratifyData);
        }

        public EduProgramInfo EduProgramInfo()
        {
            return new EduProgramInfo
            {
                Name = info.Profile.NAME,
                Id = info.Id,
                Year = info.Year,
                ProfileId = info.ProfileId,
                Qualification = info.Profile.Direction.diplomaQualification,
                EducationLevelGenitive = info.Profile.GetEducationLevelGenitive(),
                EducationLevel = info.Profile.GetEducationLevel()
            };
        }

        public InstituteInfo Institute()
        {
            Division division = db.GetInstituteForChair(info.Profile.CHAIR_ID);

            var institute = new InstituteInfo
            {
                Name = division?.title,
                Id = division?.uuid
            };
            return institute;
        }

        public DirectionInfo2 Direction()
        {
            var direction = new DirectionInfo2
            {
                Id = info.Profile.Direction.uid,
                Code = info.Profile.Direction.okso,
                Title = info.Profile.Direction.title,
                Qualifications = info.Profile.Direction.qualifications,
                Standard = info.Profile.Direction.standard,
                AreaEducationCode = info.Profile.Direction.AreaEducation?.Code,
                AreaEducationTitle = info.Profile.Direction.AreaEducation?.Title
            };
            return direction;
        }

        public ProfileTrajectoriesInfo Profile()
        {
            var profile = info.Profile;

            var profileVM = new ProfileTrajectoriesInfo
            {
                Id = profile.ID,
                Name = profile.NAME,
                Code = profile.CODE,
                DirectionId = profile.DIRECTION_ID,
                Trajectories = new List<string>()
            };
            return profileVM;
        }

        public InstituteInfo Chair()
        {
            var division = db.Divisions.FirstOrDefault(d => d.uuid == info.Profile.CHAIR_ID);
            var chair = new InstituteInfo
            {
                Name = division?.title,
                Id = division?.uuid
            };
            return chair;
        }

        public string Language()
        {
            var str = "на государственном языке Российской Федерации";
            var foreignContent = info.Profile.FOREIGN_CONTENT;

            switch (foreignContent)
            {
                case "COMPLETE":
                    str = "полностью на иностранном языке";
                    break;
                case "NONE":
                    break;
                case "PARTIAL":
                    str += ", частично на иностранном языке";
                    break;
                default:
                    break;
            }

            return str;
        }

        public FormAndDuration FormAndDuration()
        {
            var divisionUuid = db.GetInstituteOrDepartmentForChair(info.Profile.CHAIR_ID)?.uuid;
            var durations = db.TrainingDurations.Where(t => t.DirectionUid == info.Profile.DIRECTION_ID
                    && t.Qualification == info.Profile.QUALIFICATION
                    && t.DivisionUuid == divisionUuid
                    ).ToList();
            var types = durations.Select(d => d.FamilirizationType).Distinct();

            var formAndDuration = new FormAndDuration();
            formAndDuration.Rows = new List<FormAndDurationRow>();

            foreach (var d in durations)
            {
                var form = d.FamilirizationType.ToLower();
                formAndDuration.Rows.Add(new FormAndDurationRow()
                {
                    Duration = d.Duration.ToYearMonthFormat(),
                    Form = form
                });

                var numbers = new List<decimal>();
                if (d.DurationSPO.HasValue)
                    numbers.Add(d.DurationSPO.Value);
                if (d.DurationSPOUnsuitableProfile.HasValue)
                    numbers.Add(d.DurationSPOUnsuitableProfile.Value);
                if (d.DurationVPO.HasValue)
                    numbers.Add(d.DurationVPO.Value);
                if (d.DurationVPOUnsuitableProfile.HasValue)
                    numbers.Add(d.DurationVPOUnsuitableProfile.Value);

                if (numbers.Count > 0)
                {
                    numbers.Sort();
                    numbers = numbers.Distinct().ToList();

                    formAndDuration.Rows.Add(new FormAndDurationRow()
                    {
                        Duration = string.Join(", ", numbers.Select(n => n.ToYearMonthFormat())),
                        Form = form,
                        Comment = "(ускоренное обучение по индивидуальному учебному плану)"
                    });
                }
            }

            var rowsList = string.Join(";\n", formAndDuration.Rows.OrderByDescending(r => r.Form).Select(r => r.ToString()));

            formAndDuration.Text = $"Обучение по программе {info.Profile.GetEducationLevelGenitive()} может осуществляться в {formAndDuration.Forms}.\n" +

                $"Срок получения образования по программе (вне зависимости от применяемых образовательных технологий) включая каникулы, " +
                $"предоставляемые после прохождения государственной итоговой аттестации, составляет:\n" +
                $"{rowsList};\n" +
                $"- при обучении по индивидуальному учебному плану инвалидов и лиц с ограниченными возможностями здоровья(далее – инвалиды и лица с ОВЗ) " +
                $"может быть увеличен по их заявлению не более чем на 1 год по сравнению со сроком получения образования, установленным для соответствующей формы обучения.";

            return formAndDuration;
        }

        public ICollection<RequisitesOrderFgosInfo> RequisitesOrders()
        {
            var direction = info.Profile.Direction;
            
            var orders = direction.standard == "СУОС" ?
                direction.AreaEducation.Orders.Where(o => o.QualificationName == null || o.QualificationName == info.Profile.QUALIFICATION)
                    .Select(order => new RequisitesOrderFgosInfo()
                    {
                        Id = order.Id,
                        Date = order.Date.HasValue ? order.Date.Value.ToShortDateString() : "",
                        Number = order.Number,
                        DirectionId = direction.uid,
                        DirectionCode = direction.okso
                    })
                :
                direction.Orders.Select(order => new RequisitesOrderFgosInfo()
                {
                    Id = order.Id,
                    Date = order.Date.HasValue ? order.Date.Value.ToShortDateString() : "",
                    Number = order.Number,
                    DirectionId = direction.uid,
                    DirectionCode = direction.okso
                });

            return orders.ToList();
        }

        public string PurposeAndFeature()
        {
            return $"[примерный текст, на который разработчики ОП могут ориентироваться при заполнении данного раздела]\n" +

                    $"[Основная профессиональная образовательная программа \"{info.Profile.CODE} - {info.Profile.NAME}\" направлена на подготовку инженерно " +
                    $"- технических работников уровня среднего звена управления(мастер, инженер - технолог), " +
                    $"способных организовать деятельность производственных подразделений металлургических предприятий.\n" +

                    $"Программа ориентирует выпускников на активное участие и инициативу в прорывном развитии классических металлургических производств, " +
                    $"на освоение новой техники, внедрение новых технологий, изменение культуры производства, " +
                    $"следование основным направлениям развития четвертой промышленной революции.\n" +

                    $"Особенностью программы является выраженная практико - ориентированность процесса обучения. " +
                    $"Увеличенный объем производственных практик, перенос части образовательного процесса на территорию предприятий " +
                    $"- партнеров дает возможность обучающимся последовательно овладеть необходимым уровнем квалификации, " +
                    $"начиная с рабочих профессий, обеспечивает включение выпускников в производственный процесс без дополнительного переобучения.\n" +

                    $"Вместе с тем, программа предполагает фундаментальную подготовку по естественнонаучным и общеинженерным дисциплинам " +
                    $"достаточную для продолжения обучения по программам инженерной магистратуры.\n" +

                    $"Приоритет активных методов обучения и включение в программу междисциплинарных проектов обеспечивает формирование у обучающихся, " +
                    $"наряду с профессиональными компетенциями, осознанного умения работать в команде и необходимых лидерских качеств. " +
                    $"Полученные профессиональные знания и умения, компетенции в области организации производства и технологического " +
                    $"предпринимательства дают возможность выпускникам программы работать в сфере малого бизнеса, самостоятельно организовать " +
                    $"инновационное производство новой востребованной на рынке продукции.\n" +

                    $"При проектировании образовательной программы и реализации обучения использованы лучшие мировые практики подготовки специалистов " +
                    $"в области техники и технологий, передовой отечественный опыт и собственные разработки УрФУ. ]";

        }

        public ModuleStructure ModuleStructure(ModuleStructure structure, bool addModulesFromDb = true)
        {
            if (addModulesFromDb)
            {
                // заполняем данными из БД
                var modules = info.Profile.Direction.Modules;

                // ПРАКТИКА
                structure.Practices = structure.Practices.Concat(modules.Where(m => m.type == "Учебная и производственная практики" && structure.Practices.All(p => p.Id != m.uuid))
                                            .Select(m => new ModuleInfoSelected()
                                            {
                                                Capacity = m.testUnits,
                                                Id = m.uuid,
                                                Name = m.title,
                                                IncludeInCore = m.IncludeInCore
                                            }))
                    .OrderByDescending(m => m.IncludeInCore).ThenBy(m => m.Name).ToList();
                foreach (var pair in ModuleStructureAdditionalInfo.PracticeAdditionals)
                {
                    if (!structure.Practices.Any(p => p.Id == pair.Key))
                    {
                        structure.Practices.Add(new ModuleInfoSelected()
                        {
                            Capacity = 0,
                            Id = pair.Key,
                            Name = pair.Value,
                            IncludeInCore = false
                        });
                    }
                }


                //ФАКУЛЬТАТИВЫ
                structure.Facultative = structure.Facultative.Concat(modules.Where(m => m.type == "Факультативные дисциплины" && structure.Facultative.All(p => p.Id != m.uuid))
                                            .Select(m => new ModuleInfoSelected()
                                            {
                                                Capacity = m.testUnits,
                                                Id = m.uuid,
                                                Name = m.title,
                                                IncludeInCore = m.IncludeInCore
                                            }))
                    .OrderByDescending(m => m.IncludeInCore).ThenBy(m => m.Name).ToList();

                //ГИА
                structure.Gia = structure.Gia.Concat(modules.Where(m => m.type == "Итоговая государственная аттестация" && structure.Gia.All(p => p.Id != m.uuid))
                                            .Select(m => new ModuleInfoSelected()
                                            {
                                                Capacity = m.testUnits,
                                                Id = m.uuid,
                                                Name = m.title,
                                                IncludeInCore = m.IncludeInCore
                                            }))
                    .OrderByDescending(m => m.IncludeInCore).ThenBy(m => m.Name).ToList();
                foreach (var pair in ModuleStructureAdditionalInfo.GiaAdditionals)
                {
                    if (!structure.Gia.Any(p => p.Id == pair.Key))
                    {
                        structure.Gia.Add(new ModuleInfoSelected()
                        {
                            Capacity = 0,
                            Id = pair.Key,
                            Name = pair.Value,
                            IncludeInCore = false
                        });
                    }
                }


                //МОДУЛИ
                structure.Modules = structure.Modules.Concat(modules.Where(m => !structure.Practices.Any(p => p.Id == m.uuid)
                                                    && !structure.Facultative.Any(p => p.Id == m.uuid)
                                                    && !structure.Gia.Any(p => p.Id == m.uuid)
                                                    && !structure.Modules.Any(p => p.Id == m.uuid)
                                                    && m.IncludeInCore)
                                            .Select(m => new ModuleInfoSelected()
                                            {
                                                Capacity = m.testUnits,
                                                Id = m.uuid,
                                                Name = m.title,
                                                IncludeInCore = m.IncludeInCore
                                            }))
                    .OrderByDescending(m => m.IncludeInCore).ThenBy(m => m.Name).ToList();

                if (!structure.Modules.Any(m => m.Id == ModuleStructureAdditionalInfo.ModuleAdditionals.First().Key))
                    structure.Modules = new List<ModuleInfoSelected>() {
                            new ModuleInfoSelected()
                                {
                                    Capacity = 0,
                                    Id = ModuleStructureAdditionalInfo.ModuleAdditionals.First().Key,
                                    Name = ModuleStructureAdditionalInfo.ModuleAdditionals.First().Value,
                                    IncludeInCore = false
                                }
                }
                    .Concat(structure.Modules)
                    .ToList();

                if (!structure.Modules.Any(m => m.Id == ModuleStructureAdditionalInfo.ModuleAdditionals.Last().Key))
                    structure.Modules.Add(new ModuleInfoSelected()
                    {
                        Capacity = 0,
                        Id = ModuleStructureAdditionalInfo.ModuleAdditionals.Last().Key,
                        Name = ModuleStructureAdditionalInfo.ModuleAdditionals.Last().Value,
                        IncludeInCore = false
                    });

                structure.Facultative = new List<ModuleInfoSelected>() { new ModuleInfoSelected()
                    {
                        Capacity = 0,
                        Id = "facultativeDefault",
                        IncludeInCore = true,
                        Name = "Факультативы",
                        Selected = true
                    } };

            }

            var divisionUuid = db.GetInstituteForChair(info.Profile.CHAIR_ID)?.uuid;
            var duration = db.TrainingDurations.FirstOrDefault(t => t.DirectionUid == info.Profile.DIRECTION_ID
                    && t.Qualification == info.Profile.QUALIFICATION
                    && t.DivisionUuid == divisionUuid && t.FamilirizationType == "Очная");

            if (duration != null)
            {
                // Нормативный срок обучения по очной форме обучения разделить на 0,5 и умножить на 30
                // Или бакалавриат 4 года - 240 з.е.
                // Магистратура 2 года - 120 з.е.

                structure.RequiredSum = (int)duration.Duration * 30 * 2;
            }

            return structure;
        }

        public ICollection<ProfStandardInfo> ProfStandardsList(string[] codes)
        {
            var standards = new List<ProfStandardInfo>();
            standards = db.ProfOrders.Include(o => o.ProfStandard).Where(s => codes.Contains(s.ProfStandardCode))
                .ToList()
                .OrderBy(c => c.ProfStandardCode)
                .Select(s => new ProfStandardInfo()
                {
                    ProfStandardCode = s.ProfStandardCode,
                    ProfStandardTitle = s.ProfStandard.Title,
                    Status = s.Status,
                    ProfOrderInfo = new ProfOrderInfo()
                    {
                        DateOfMintrud = s.DateOfMintrud.HasValue ? s.DateOfMintrud.Value.ToShortDateString() : null,
                        NumberOfMintrud = s.NumberOfMintrud,
                        RegNumberDateOfMinust = s.RegNumberDateOfMinust.HasValue ? s.RegNumberDateOfMinust.Value.ToShortDateString() : null,
                        RegNumberOfMinust = s.RegNumberOfMinust
                    },
                    ProfOrderChanges = s.OrderChanges.Select(o => new ProfOrderInfo()
                    {
                        DateOfMintrud = o.ProfOrderChange.DateOfMintrud.HasValue ? o.ProfOrderChange.DateOfMintrud.Value.ToShortDateString() : null,
                        NumberOfMintrud = o.ProfOrderChange.NumberOfMintrud,
                        RegNumberOfMinust = o.ProfOrderChange.RegNumberOfMinust,
                        RegNumberDateOfMinust = o.ProfOrderChange.RegNumberDateOfMinust.HasValue ? o.ProfOrderChange.RegNumberDateOfMinust.Value.ToShortDateString() : null
                    }).ToList()
                })
                .ToList();

            return standards;
        }

        public ICollection<CompetenceInfoVM> UniversalCompetences()
        {
            return Competences(universalCompetenceType);
        }

        public ICollection<CompetenceInfoVM> GeneralCompetences()
        {
            return Competences(generalCompetenceType);
        }


        private ICollection<CompetenceInfoVM> Competences(string type)
        {
            var competences = db.Competences.Where(c => !c.IsDeleted && c.Standard == info.Profile.Direction.standard
                    && c.AreaEducationId == info.Profile.Direction.AreaEducationId
                    && c.QualificationName.Contains(info.Profile.QUALIFICATION)
                    && (c.Standard == info.Profile.Direction.standard && c.Standard == "ФГОС ВО 3++" && c.DirectionId == info.Profile.DIRECTION_ID
                                    || info.Profile.Direction.standard == c.Standard && c.Standard == "СУОС")
                    && c.Type == type)
                .OrderBy(c => c.Order)
                    .Select(c => new CompetenceInfoVM()
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Content = c.Content,
                        Type = c.Type,
                        CompetenceGroupId = c.CompetenceGroupId.HasValue ? c.CompetenceGroupId.Value : 0,
                        CompetenceGroupName = c.CompetenceGroup.Name
                    }).ToList();
            return competences;
        }
    }
}
