using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using EFExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.Core.ExcelPackage;
using PagedList.Core;
using Urfu.Its.Common;
using Urfu.Its.Integration;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
//using WebGrease.Css.Extensions;
using Group = Urfu.Its.Web.DataContext.Group;
using Profile = Urfu.Its.Web.DataContext.Profile;
using Microsoft.Extensions.Hosting.Internal;

namespace Urfu.Its.Web.Models
{
    public class SyncModel
    {
        public string Message { get; set; }

        public bool ModuleSynInProgress
        {
            get { return SyncEngine.ModulesSyncInProgress; }
        }

        public DateTime? ModulesSyncStarted
        {
            get { return SyncEngine.ModulesSyncStarted; }
        }
    }


    public class SyncEngine
    {
        //TODO: Use HostingEnvironmtn.QueueBackgroundWorkItem or other reliable way to run a task under IIS
        private static Task DirectionsSyncTask;
        private static bool PeopleSyncInProgress;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DoDirectionsSync(out bool alreadyInProgress)
        {
            Logger.Info("Запрос синхронизации направлений");
            if (DirectionsSyncTask != null && DirectionsSyncTask.Status == TaskStatus.Running)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            DirectionsSyncTask = Task.Factory.StartNew(SyncDirections);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DoPeopleSync(out bool alreadyInProgress)
        {
            Logger.Info("Запрос синхронизации персонала");
            if (PeopleSyncInProgress)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem((Action<CancellationToken>)SyncPeople);
        }

        internal static void SyncPeople(CancellationToken cancellationToken)
        {

            PeopleSyncInProgress = true;
            Logger.Info("Синхронизация групп");
            var sw = Stopwatch.StartNew();
            HashSet<string> groupIds = new HashSet<string>();
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var profileIds = new HashSet<string>(db.Profiles.Select(p => p.ID));
                    var chairIds = new HashSet<string>(db.Divisions.Select(d => d.uuid));
                    var restService = new UniRestService();
                    var groupsXml =
                        restService.GetGroupsXml().Where(g => profileIds.Contains(g.ProfileId)).ToList();

                    foreach (var dto in groupsXml)
                    {
                        groupIds.Add(dto.Id);
                    }
                    var me = new MapperConfiguration(cfg => {
                        cfg.AddProfile<AutoMapperConfig>();
                    });
                    var mapper = me.CreateMapper();
                    var op = db.Upsert(groupsXml.Select(mapper.Map<Group>));
                    op.Key(student => student.Id);
                    op.ExcludeField(s => s.Profile);
                    op.ExcludeField(s => s.ForeignLanguageCompetitionGroups);
                    op.ExcludeField(s => s.SectionFkCompetitionGroups);
                    op.ExcludeField(s => s.ProjectCompetitionGroups);
                    op.ExcludeField(s => s.MUPCompetitionGroups);

                    op.Execute();

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация группы " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Info("Синхронизация персон");

            if (cancellationToken.IsCancellationRequested)
            {
                PeopleSyncInProgress = false;
                return;
            }
            sw = Stopwatch.StartNew();
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    db.ChangeTracker.AutoDetectChangesEnabled = false;
                    //db.Configuration.ValidateOnSaveEnabled = false;
                    var restService = new UniRestService();
                    var personsXml =
                        restService.GetPersonsXml();
                    var me = new MapperConfiguration(cfg => {
                        cfg.AddProfile<AutoMapperConfig>();
                    });
                    var mapper = me.CreateMapper();
                    var op = db.Upsert(personsXml.Select(mapper.Map<Person>));
                    op.Key(person => person.Id);
                    op.Execute();

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация персон " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            if (cancellationToken.IsCancellationRequested)
            {
                PeopleSyncInProgress = false;
                return;
            }

            Logger.Info("Синхронизация студентов");
            sw = Stopwatch.StartNew();
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var restService = new UniRestService();
                    var studentsXml =
                        restService.GetStudentsXml().Where(s => groupIds.Contains(s.GroupId));
                    var me = new MapperConfiguration(cfg => {
                        cfg.AddProfile<AutoMapperConfig>();
                    });
                    var mapper = me.CreateMapper();
                    var op = db.Upsert(studentsXml.Select(mapper.Map<Student>));
                    op.Key(student => student.Id);
                    op.ExcludeField(student => student.Rating);
                    op.ExcludeField(student => student.Sportsman);
                    op.ExcludeField(student => student.ForeignLanguageRating);
                    op.ExcludeField(student => student.ForeignLanguageLevel);
                    op.ExcludeField(student => student.ForeignLanguageTargetLevel);
                    op.ExcludeField(student => student.RatingType);
                    op.ExcludeField(student => student.planVerion);
                    op.ExcludeField(student => student.sectionFKDebtTerms);
                    op.ExcludeField(student => student.versionNumber);
                    op.ExcludeField(student => student.SelectionJson);
                    op.ExcludeField(student => student.Group);
                    op.ExcludeField(student => student.Person);
                    op.ExcludeField(student => student.Selections);
                    op.ExcludeField(student => student.ModuleAdmissions);
                    op.ExcludeField(student => student.VariantAdmissions);
                    op.ExcludeField(student => student.MinorAdmissions);
                    op.ExcludeField(student => student.MinorSelections);
                    op.ExcludeField(student => student.SectionFKSelections);
                    op.ExcludeField(student => student.ForeignLanguageSelections);
                    op.ExcludeField(student => student.SectionFKAdmissions);
                    op.ExcludeField(student => student.ForeignLanguageAdmissions);
                    op.ExcludeField(student => student.Practices);
                    op.ExcludeField(student => student.ProjectAdmissions);
                    op.ExcludeField(student => student.ProjectSelections);
                    op.ExcludeField(student => student.MUPAdmissions);
                    op.Execute();

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация студентов " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Info("Синхронизация директоров институтов");

            if (cancellationToken.IsCancellationRequested)
            {
                PeopleSyncInProgress = false;
                return;
            }
            sw = Stopwatch.StartNew();
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    db.ChangeTracker.AutoDetectChangesEnabled = false;
                    //db.Configuration.ValidateOnSaveEnabled = false;
                    var restService = new UniRestService();
                    var directorsDto =
                        restService.GetDirectorsDto();

                    directorsDto = directorsDto.Where(d => db.Divisions.Select(_ => _.uuid).Contains(d.divisionUUID)).ToList();
                    var me = new MapperConfiguration(cfg => {
                        cfg.AddProfile<AutoMapperConfig>();
                    });
                    var mapper = me.CreateMapper();
                    var op = db.Upsert(directorsDto.Select(mapper.Map<Director>));
                    op.Key(director => director.DivisionUuid);
                    op.ExcludeField(director => director.Division);
                    op.Execute();

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация директоров институтов " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            PeopleSyncInProgress = false;
        }

        public static void SyncApploads(int year, int term)
        {
            var sw = Stopwatch.StartNew();
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            Logger.Info("Синхронизация нагрузок года " + year + " семестра " + term);
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {

                    var restService = new ApploadService();
                    var groups =
                        restService.GetApploads(year, term, true).Select(mapper.Map<Appload>).ToList();
                    SetYearTermFApploads(year, term, DisciplineType.Other, groups);

                    var notItsApploads = restService.GetApploads(year, term, false);

                    var sectionFKs = notItsApploads
                            .Where(
                                a =>
                                    a.disciplineTitle == "Физическая культура" ||
                                    a.disciplineTitle == "Прикладная физическая культура").Select(mapper.Map<Appload>).ToList();
                    SetYearTermFApploads(year, term, DisciplineType.SectionFK, sectionFKs);

                    var foreignLanguages = notItsApploads
                            .Where(
                                a =>
                                    a.disciplineTitle == "Иностранный язык" ||
                                    a.disciplineTitle == "иностранный язык").Select(mapper.Map<Appload>).ToList();
                    SetYearTermFApploads(year, term, DisciplineType.ForeignLanguage, foreignLanguages);

                    var minors = notItsApploads
                           .Where(
                               a =>
                                   a.disciplineTitle.StartsWith("Майнор ")).Select(mapper.Map<Appload>).ToList();
                    SetYearTermFApploads(year, term, DisciplineType.Minor, minors);

                    var projects = notItsApploads
                        .Where(a => a.modtypeUNI == "Проектное обучение").Select(mapper.Map<Appload>).ToList();
                    SetYearTermFApploads(year, term, DisciplineType.Project, projects);

                    var pairedModule = notItsApploads
                        .Where(a => a.modtypeUNI == "Парный модуль").Select(mapper.Map<Appload>).ToList();
                    SetYearTermFApploads(year, term, DisciplineType.PairedModule, pairedModule);


                    // TODO переписать условия, чтобы не повторялись! 
                    var chairs = new List<string>(){
                        "undich18ggl5g0000kaimq8hqbcvlj94", // кафедра школа бакалавриата (школа), институт ИРИТ-РТФ
                        "undich18hc2jg0000llta22objdfk90k" // кафедра школы бакалавриата (школа), институт ИЕНиМ
                        };

                    var groupsrtf = db.Groups.Where(g => chairs.Contains(g.ChairId)).Select(d => d.Id);
                    var others = notItsApploads.Where(a => groupsrtf.Contains(a.group)
                                                                    && a.disciplineTitle != "Физическая культура"
                                                                    && a.disciplineTitle != "Прикладная физическая культура"
                                                                    && a.disciplineTitle != "Иностранный язык"
                                                                    && a.disciplineTitle != "иностранный язык"
                                                                    && !a.disciplineTitle.StartsWith("Майнор ")
                                                                    && a.modtypeUNI != "Проектное обучение"
                                                                    && a.modtypeUNI != "Парный модуль"
                                            ).Select(mapper.Map<Appload>).ToList();

                    SetYearTermFApploads(year, term, DisciplineType.MUP, others);

                    var objects = groups.Union(sectionFKs).Union(foreignLanguages).Union(minors).Union(projects).Union(pairedModule).Union(others).ToList();

                    var op = db.Upsert(objects);
                    op.Key(student => student.uuid);
                    op.Execute();

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация нагрузок " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void SyncGroupHistory(int year)
        {
            var sw = Stopwatch.StartNew();
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            Logger.Info("Синхронизация исторические группы года " + year);
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {

                    var restService = new GroupHistoryService();
                    var groupsHistories =
                        restService.GetGroupHistories(year).Select(mapper.Map<GroupsHistory>).ToList();
                    
                    var isCurrentEduYear = DateTime.Now.Month < 7 
                        ? year == DateTime.Now.Year - 1 
                        : year == DateTime.Now.Year;
                    
                    foreach (var groupsHistory in groupsHistories)
                    {
                        if (!db.Groups.Any(_ => _.Id == groupsHistory.GroupId))
                            continue;
                        
                        if (!db.GroupsHistories.Any(_ => _.Id == groupsHistory.Id))
                            db.GroupsHistories.Add(groupsHistory);
                        else
                        {
                            if (isCurrentEduYear) // обновляем всю информацию по группе текущего года
                                db.Entry(groupsHistory).State = EntityState.Modified;
                        }
                    }

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация исторические группы " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private static void SetYearTermFApploads(int year, int term, DisciplineType disciplineType, List<Appload> objects)
        {
            foreach (var appload in objects)
            {
                appload.year = year;
                appload.term = term;
                appload.DisciplineType = disciplineType;
            }
        }

        public static void SyncDebtors(string moduleTitle, int? year, string term)
        {
            var sw = Stopwatch.StartNew();
            Logger.Info("Синхронизация должников " + year + " семестра " + term);
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {

                    var restService = new DebtorsService();
                    var debtors =
                        restService.GetDebtors(moduleTitle, year, term);

                    var groupedDebtors = debtors.OrderBy(_ => _.year).ThenByDescending(_ => _.term).GroupBy(_ => _.studentCode);
                    string json;
                    foreach (var student in db.Students.Where(_ => !String.IsNullOrEmpty(_.sectionFKDebtTerms)).ToList())
                    {
                        var affected =
                            db.Database.ExecuteSqlCommand(
                                "Update students set " +
                                "sectionFKDebtTerms = @p0 where id = @p1 ", null, student.Id);
                    }
                    db.SaveChanges();
                    foreach (var r in groupedDebtors)
                    {
                        json = JsonConvert.SerializeObject(r.Select(_ => new { year = _.year, term = _.term }));
                        var affected =
                            db.Database.ExecuteSqlCommand(
                                "Update students set " +
                                "sectionFKDebtTerms = @p0 where id = @p1 ", json, r.Key);
                    }

                    db.SaveChanges();
                    dbcxtransaction.Commit();

                }
                Logger.Info("Закончена cинхронизация должников " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        public static void SyncTmers()
        {
            var sw = Stopwatch.StartNew();
            Logger.Info("Синхронизация контрольных мероприятий");
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var restService = new TmerService();
                    var objects = restService.GetTmers();
                    var tmers = objects.Select(mapper.Map<Tmer>).ToList();

                    Regex regex = new Regex(@"[uk]\d{3}");
                    for (int i = 0; i < tmers.Count(); i++)
                    {
                        if (regex.IsMatch(tmers[i].kmer))
                            tmers[i].kmer = tmers[i].kmer.ToUpper();
                    }

                    var op = db.Upsert(tmers);
                    op.Key(o => o.kmer);
                    op.Execute();

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация контрольных мероприятий " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        internal static void SyncDirections()
        {
            HashSet<string> directionsIds = new HashSet<string>();
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            Logger.Info("Синхронизация направлений");
            var sw = Stopwatch.StartNew();
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var restService = new UniRestService();
                    var standards = new List<string>() { "ФГОС ВО", "ФГОС 3++", "СУОС" };
                    var directionsFromService =
                        restService.GetDirections()
                            .Where(DirectionNumberIsValid)
                            .Where(d => standards.Contains(d.standard))
                            .Where(DirectionQualificationIsValid);

                    foreach (var directionDto in directionsFromService)
                    {
                        var qualifications = string.Join(", ", directionDto.qualifications);
                        var areaEdu = db.AreaEducations.FirstOrDefault(a => a.Code == directionDto.areaEducation.code
                                                                            && a.Title == directionDto.areaEducation.title);
                        if (areaEdu == null)
                        {
                            areaEdu = mapper.Map<AreaEducation>(directionDto.areaEducation);
                            db.AreaEducations.Add(areaEdu);
                            db.SaveChanges();
                        }
                        directionDto.areaEducation.id = areaEdu.Id;
                    }

                    directionsIds = new HashSet<string>(directionsFromService.Select(d => d.uid));
                    foreach (var p in directionsFromService.Select(mapper.Map<Direction>))
                        if (!db.Directions.Any(d => d.uid == p.uid))
                        {
                            var d = new Direction();
                            d.uid = p.uid;
                            db.Directions.Add(d);
                        }
                    //db.Directions.AddOrUpdate(d => d.uid, directionsFromService.Select(mapper.Map<Direction>).ToArray());
                    db.SaveChanges();
                }
                Logger.Info("Направления синхронизованы " + sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Info("Синхронизация профилей");
            sw = Stopwatch.StartNew();

            using (var db = new ApplicationDbContext())
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var restService = new UniRestService();
                        var directionsFromService =
                            restService.GetProfilesXml().Where(p => directionsIds.Contains(p.DIRECTION_ID));


                        Dictionary<string, ProfileXmlDto> removedDuplicats = new Dictionary<string, ProfileXmlDto>();
                        foreach (var dto in directionsFromService)
                        {
                            removedDuplicats[dto.ID] = dto;
                        }

                        // синхронизация отсутствующих подразделений
                        var divisions = db.Divisions.Select(d => d.uuid).ToList();
                        var profilesWithAbsentChairs = removedDuplicats.Where(r => !divisions.Contains(r.Value.CHAIR_ID));
                        var actualDivisions = profilesWithAbsentChairs.Count() > 0 ? new UniDivisionsService().GetDivisions() : new Dictionary<string, DivisionDto>();
                        var chairsToUpdate = new HashSet<DivisionDto>();
                        foreach (var profile in profilesWithAbsentChairs)
                        {
                            if (profile.Value.CHAIR_ID == null)
                                continue;

                            DivisionDto ld;
                            if (actualDivisions.TryGetValue(profile.Value.CHAIR_ID, out ld))
                            {
                                chairsToUpdate.Add(ld);
                            }
                            else
                                Logger.Info($"Не найдено подразделение {profile.Value.CHAIR_ID} при синхронизации профилей. Профиль {profile.Value.CODE} {profile.Value.ID} не будет синхронизирован");
                        }
                        if (chairsToUpdate.Count() > 0)
                            UpdateDivision(chairsToUpdate.ToArray(), actualDivisions);

                        // помечаем все профили удаленными
                        var existedProfiles = db.Profiles;
                        foreach (var profile in existedProfiles)
                        {
                            profile.remove = true;
                            db.Entry(profile).State = EntityState.Modified;
                        }
                        db.SaveChanges();

                        // сохранение профилей
                        foreach (var p in removedDuplicats.Values.Select(mapper.Map<DataContext.Profile>))
                            if (!db.Profiles.Any(d => d.ID == p.ID))
                            {
                                var d = new Profile();
                                d.ID = p.ID;
                                db.Profiles.Add(d);
                            }
                        /*db.Profiles.AddOrUpdate(d => d.ID,
                            removedDuplicats.Values.Select(mapper.Map<DataContext.Profile>).ToArray());*/
                        db.SaveChanges();
                        dbcxtransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbcxtransaction.Rollback();
                        Logger.Info("Ошибка при синхронизации профилей");
                        Logger.Error(ex);
                    }
                }
            }
             Logger.Info("Профили синхронизованы " + sw.Elapsed);
        }

        private static readonly string[] Qualifications =
        {
            "Магистр", "Прикладной бакалавриат", "Бакалавр", "Специалист", "Аспирант"
        };

        private static bool DirectionQualificationIsValid(DirectionDto arg)
        {
            return arg.qualifications.Intersect(Qualifications).Any();
        }

        private static bool DirectionNumberIsValid(DirectionDto arg)
        {
            var okso = arg.okso;
            if (string.IsNullOrWhiteSpace(okso))
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(okso, "\\d\\d\\.\\d\\d\\.\\d\\d",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DoModulesSync(out bool alreadyInProgress)
        {
            Logger.Info("Запрос синхронизации модулей");
            if (ModulesSyncInProgress)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem((Action<CancellationToken>)SyncModules);
        }

        public static bool ModulesSyncInProgress { get; set; }

        public static DateTime? ModulesSyncStarted { get; set; }

        internal static void SyncModules(CancellationToken cancellationToken)
        {
            ModulesSyncInProgress = true;
            Logger.Info("Синхронизация модулей и планов");
            var sw = Stopwatch.StartNew();
            ModulesSyncStarted = DateTime.Now;
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    db.ChangeTracker.AutoDetectChangesEnabled = false;
                    //db.Configuration.ValidateOnSaveEnabled = false;

                    var divisions = new UniDivisionsService().GetDivisions();

                    foreach (var direction in db.Directions.OrderBy(d => d.standard).ToList())
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;
                        try
                        {
                            SyncModulesForDirection(direction.uid, db, divisions);
                            Logger.Error($"Синхронизованы модули для {direction.OksoAndTitleStandard}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Ошибка при синхронизации модулей по направлению {direction.OksoAndTitleStandard} Id {direction.uid}");
                            Logger.Error(ex);
                        }
                    }
                    foreach (var direction in db.Directions.OrderBy(d => d.standard).ToList())
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;
                        try
                        {
                            SyncDataForDirection(direction.uid, db, divisions);
                            Logger.Error($"Синхронизованы планы для {direction.OksoAndTitleStandard}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Ошибка при синхронизации планов по направлению {direction.OksoAndTitleStandard} Id {direction.uid}");
                            Logger.Error(ex);
                        }
                    }
                }

                Logger.Info("Модули и планы синхронизованы " + sw.Elapsed);

                //и еще кое чего загрузим
                SyncPlanTerms(cancellationToken);

                SyncPlanWeeks(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                ModulesSyncInProgress = false;
            }
        }

        public static void SyncModulesForDirection(string direction, ApplicationDbContext db,
            Dictionary<string, DivisionDto> divisions)
        {
            var restService = UniModulesService.Create();
            var modulesForDirection = restService.GetModulesForDirection(direction);
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            var modules = modulesForDirection.Select(mapper.Map<Module>).ToArray();
            var disciplines = modules.SelectMany(m => m.disciplines)
                .Select(d => new { d.uid, d })
                .GroupBy(o => o.uid)
                .Select(g => g.First().d)
                .ToArray(); //получение уникальных дисциплин
            foreach (var p in disciplines)
                if (!db.Disciplines.Any(d => d.uid == p.uid))
                {
                    var d = new Discipline();
                    d.uid = p.uid;
                    db.Disciplines.Add(d);
                }
            //db.Disciplines.AddOrUpdate(d => d.uid, disciplines);
            db.SaveChanges();
            var disciplinesDictionary = db.Disciplines.Include(d => d.Modules).ToDictionary(d => d.uid);
            foreach (var module in modules)
            {
                module.disciplines = module.disciplines.Select(d => disciplinesDictionary[d.uid]).ToList();
            }
            foreach (var p in modules)
                if (!db.Modules.Any(d => d.uuid == p.uuid))
                {
                    var d = new Module();
                    d.uuid = p.uuid;
                    db.Modules.Add(d);
                }
            //db.Modules.AddOrUpdate(m => m.uuid, modules);
            db.SaveChanges();

            var tech = db.ModuleTeches.First();

            var needminors = db.UniModules().Where(m => m.type.Contains(ModuleTypes.Minor) && m.Minor == null).ToList();
            foreach (var m in needminors)
            {
                var minor = new Minor { Module = m, Tech = tech };
                m.Minor = minor;

                db.Minors.Add(minor);
            }

            var needSectionFKs = db.UniModules().Where(m => m.type.Contains(ModuleTypes.SectionFK) && m.SectionFk == null).ToList();
            foreach (var m in needSectionFKs)
            {
                var sectionFk = new SectionFK { Module = m, Tech = tech };
                m.SectionFk = sectionFk;

                db.SectionFKs.Add(sectionFk);
            }

            var needForeignLanguages = db.UniModules().Where(m => m.type.Contains(ModuleTypes.ForeignLanguage) && m.ForeignLanguage == null).ToList();
            foreach (var m in needForeignLanguages)
            {
                var foreignLanguage = new ForeignLanguage() { Module = m, Tech = tech };
                m.ForeignLanguage = foreignLanguage;

                db.ForeignLanguages.Add(foreignLanguage);
            }

            db.SaveChanges();


            var dbModules =
                db.UniModules().Where(m => m.Directions.Any(d => d.uid == direction)).Include(m => m.disciplines).ToList();
            var modulesDictionary = modules.ToDictionary(m => m.uuid);
            var modules111 = dbModules.Select(m => m.uuid).ToList();
            foreach (var dbModule in dbModules)
            {
                Module seviceModule;
                if (!modulesDictionary.TryGetValue(dbModule.uuid, out seviceModule))
                    continue;
                foreach (var discipline in dbModule.disciplines.ToList())
                {
                    if (seviceModule.disciplines.All(d => d.uid != discipline.uid))
                        dbModule.disciplines.Remove(discipline);
                }


                foreach (var discipline in seviceModule.disciplines.ToList())
                {
                    if (dbModule.disciplines.All(d => d.uid != discipline.uid))
                        dbModule.disciplines.Add(discipline);
                }
            }
            db.SaveChanges();
            var directionObject = db.Directions.Find(direction);

            foreach (var module in modules)
            {
                var loadedModule = db.UniModules().First(m => m.uuid == module.uuid);
                module.Directions = new List<Direction>();
                if (loadedModule.Directions.All(m => m.uid != direction))
                {
                    loadedModule.Directions.Add(directionObject);
                    module.Directions.Add(directionObject);
                    directionObject.Modules.Add(loadedModule);
                }
            }
            db.ChangeTracker.DetectChanges();
            db.SaveChanges();
            var modulesIds = modules.Select(_ => _.uuid).ToList();
            var modulesWBadDireciton = db.UniModules().Where(_ => _.Directions.Any(d => d.uid == direction)).ToList();
            foreach (var module in modulesWBadDireciton)
            {
                if (!modulesIds.Contains(module.uuid))
                {
                    module.Directions.Remove(directionObject);
                }

            }
            db.ChangeTracker.DetectChanges();
            db.SaveChanges();
        }

        public static void SyncDataForDirection(string direction, ApplicationDbContext db,
            Dictionary<string, DivisionDto> divisions)
        {
            var restService = UniModulesService.Create();
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            var plans = restService.GetPlansForDirection(direction);
            var planDivision =
                plans.Select(p => p.faculty)
                    .Distinct()
                    .Select(id => divisions[id])
                    .Select(mapper.Map<Division>)
                    .ToArray();
            foreach (var p in planDivision)
                if (!db.Divisions.Any(d => d.uuid == p.uuid))
                {
                    var d = new Division();
                    d.uuid = p.uuid;
                    db.Divisions.Add(d);
                }
            //db.Divisions.AddOrUpdate(d => d.uuid, planDivision);

            var existedPlans = db.Plans.Where(p => p.directionId == direction);
            foreach (var plan in existedPlans)
            {
                plan.remove = true;
                db.Entry(plan).State = EntityState.Modified;
            }
            db.SaveChanges();
            

            var correctPlans = plans.Where(p => db.Modules.Any(m => m.uuid == p.moduleUUID)).ToList();
            foreach (var p in correctPlans.Select(
                delegate (PlanDto dto)
                {
                    var plan = mapper.Map<Plan>(dto);
                    plan.directionId = direction;
                    plan.remove = false;
                    return plan;
                }))
                if (!db.Plans.Any(d => d.disciplineUUID == p.disciplineUUID & d.versionUUID == p.versionUUID &
                    d.moduleUUID == p.moduleUUID & d.eduplanUUID == p.eduplanUUID))
                {
                    var d = new Plan();
                    d.versionUUID = p.versionUUID;
                    d.disciplineUUID = p.disciplineUUID;
                    d.moduleUUID = p.moduleUUID;
                    d.eduplanUUID = p.eduplanUUID;
                    db.Plans.Add(d);
                }
           /* db.Plans.AddOrUpdate(p => new { p.moduleUUID, p.eduplanUUID, p.disciplineUUID, p.versionUUID }, correctPlans.Select(
                delegate (PlanDto dto)
                {
                    var plan = mapper.Map<Plan>(dto);
                    plan.directionId = direction;
                    plan.remove = false;
                    return plan;
                }).ToArray());*/
            db.SaveChanges();

            var plansAdd = new UniRestService().GetPlanAdditionalsForDirection(direction);


            foreach (var p in plansAdd.Select(
                    delegate (PlanAdditionalDto dto)
                    {
                        var plan = mapper.Map<PlanAdditional>(dto);
                        return plan;
                    }))
                if (!db.PlanAdditionals.Any(d => d.disciplineUUID == p.disciplineUUID & d.versionUUID == p.versionUUID))
                {
                    var d = new PlanAdditional();
                    d.versionUUID = p.versionUUID;
                    d.disciplineUUID = p.disciplineUUID;
                    db.PlanAdditionals.Add(d);
                }
            /*db.PlanAdditionals.AddOrUpdate(p => new { disciplineUUID = p.disciplineUUID, p.versionUUID }, plansAdd.Select(
                    delegate (PlanAdditionalDto dto)
                    {
                        var plan = mapper.Map<PlanAdditional>(dto);
                        return plan;
                    }).ToArray());*/
            db.SaveChanges();



            /* Это код, который оставляет только активный план, если есть активный план. Код выключен в связи с переходом на версию у образоватлеьной программы
                        foreach (var moduleId in db.Modules.Where(m=>m.Directions.Any(d=>d.uid== direction)).Select(m=>m.uuid).ToList())
                        {
                            var groups =
                                db.Plans.Where(p => p.directionId == direction && p.moduleUUID == moduleId)
                                    .GroupBy(
                                        g =>
                                            new
                                            {
                                                g.qualification,
                                                g.familirizationCondition,
                                                g.familirizationTech,
                                                g.familirizationType
                                            })
                                            .Where(g=> g.Any(p => p.versionActive) && g.Any(p => !p.versionActive))
                                    .ToList();
                            foreach (var g in groups)
                            {
                                db.Plans.RemoveRange(g.Where(gx => !gx.versionActive));
                            }
                        }

                        db.SaveChanges();*/
            foreach (var p in plans.Select(p => p.familirizationCondition)
                    .Distinct()
                    .Select(n => new FamilirizationCondition() { Name = n }))
                if (!db.FamilirizationConditions.Any(d => d.Name == p.Name))
                {
                    var d = new FamilirizationCondition();
                    d.Name = p.Name;
                    db.FamilirizationConditions.Add(d);
                }
           /* db.FamilirizationConditions.AddOrUpdate(c => c.Name,
                plans.Select(p => p.familirizationCondition)
                    .Distinct()
                    .Select(n => new FamilirizationCondition() { Name = n })
                    .ToArray());*/
            foreach (var p in plans.Select(p => p.familirizationTech)
                    .Distinct()
                    .Select(n => new FamilirizationTech() { Name = n }))
                if (!db.FamilirizationTechs.Any(d => d.Name == p.Name))
                {
                    var d = new FamilirizationTech();
                    d.Name = p.Name;
                    db.FamilirizationTechs.Add(d);
                }
           /* db.FamilirizationTechs.AddOrUpdate(c => c.Name,
                plans.Select(p => p.familirizationTech)
                    .Distinct()
                    .Select(n => new FamilirizationTech() { Name = n })
                    .ToArray());*/
            foreach (var p in plans.Select(p => p.qualification)
                    .Distinct()
                    .Select(n => new Qualification() { Name = n }))
                if (!db.Qualifications.Any(d => d.Name == p.Name))
                {
                    var d = new Qualification();
                    d.Name = p.Name;
                    db.Qualifications.Add(d);
                }
            /*db.Qualifications.AddOrUpdate(c => c.Name,
                plans.Select(p => p.qualification)
                    .Distinct()
                    .Select(n => new Qualification() { Name = n })
                    .ToArray());*/
            foreach (var p in plans.Select(p => p.familirizationType)
                    .Distinct()
                    .Select(n => new FamilirizationType() { Name = n }))
                if (!db.FamilirizationTypes.Any(d => d.Name == p.Name))
                {
                    var d = new FamilirizationType();
                    d.Name = p.Name;
                    db.FamilirizationTypes.Add(d);
                }
            /*db.FamilirizationTypes.AddOrUpdate(c => c.Name,
                plans.Select(p => p.familirizationType)
                    .Distinct()
                    .Select(n => new FamilirizationType() { Name = n })
                    .ToArray());
            */
            db.SaveChanges();
        }


        internal static void SyncPlanTerms(CancellationToken cancellationToken)
        {
            //ModulesSyncInProgress = true;
            Logger.Info("Синхронизация семестров");
            var sw = Stopwatch.StartNew();
            //ModulesSyncStarted = DateTime.Now;
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    db.ChangeTracker.AutoDetectChangesEnabled = false;
                    //db.Configuration.ValidateOnSaveEnabled = false;

                    try
                    {
                        SyncPlanTerms(db);
                        Logger.Info("Семестры плана синхронизованы " + sw.Elapsed);

                        SyncPlanDisciplineTerms(db);
                        Logger.Info("Семестры по практикам синхронизованы " + sw.Elapsed);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Ошибка при синхронизации семестров");
                        Logger.Error(ex);
                    }

                    Logger.Info("Семестры синхронизованы " + sw.Elapsed);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                //ModulesSyncInProgress = false;
            }
        }

        internal static void SyncPlanWeeks(CancellationToken cancellationToken)
        {
            //ModulesSyncInProgress = true;
            Logger.Info("Синхронизация данных по количеству недель в УП");
            var sw = Stopwatch.StartNew();
            //ModulesSyncStarted = DateTime.Now;
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    db.ChangeTracker.AutoDetectChangesEnabled = false;
                    //db.Configuration.ValidateOnSaveEnabled = false;

                    try
                    {
                        SyncPlanWeeks(db);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Ошибка при синхронизации данных по количеству недель в УП");
                        Logger.Error(ex);
                    }
                    Logger.Info("Данные по количеству недель в УП синхронизованы " + sw.Elapsed);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                //ModulesSyncInProgress = false;
            }
        }

        private static void SyncPlanTerms(ApplicationDbContext db)
        {
            var service = new UniPlanTermsService();
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            var plans = service.GetPlanTerms();

            var planTerms = mapper.Map<List<PlanTerm>>(plans);
            foreach (var pt in planTerms)
            {
                var p = new PlanTerm();
                p.eduplanUUID = pt.eduplanUUID;
                p.Year = pt.Year;
                // db.PlanTerms.AddOrUpdate(p);
                try
                {
                    db.PlanTerms.Update(p);
                }
                catch
                {
                    db.PlanTerms.Add(p);
                }
            }
            db.SaveChanges();
        }

        private static void SyncPlanDisciplineTerms(ApplicationDbContext db)
        {
            var types = new[] { "Производственная практика", "Учебная практика" };

            var practices = db.Plans.Where(p => types.Contains(p.additionalType) || p.Module.type == "Парный модуль" || p.Module.type == "Проектное обучение");


            var planTerms = db.PlanTerms.Where(t => practices.Any(p => p.eduplanUUID == t.eduplanUUID)).ToList();

            var discipline = practices.Select(p => new { p.eduplanUUID, p.disciplineUUID, p.allTermsExtracted }).ToList();

            var planDisciplieTerms = new List<PlanDisciplineTerm>();

            foreach (var d in discipline)
            {
                var terms = planTerms.Where(p => p.eduplanUUID == d.eduplanUUID);
                planDisciplieTerms.AddRange(CreatePlanDisciplieTerms(d.disciplineUUID, d.allTermsExtracted, terms));
            }
            foreach (var p in planDisciplieTerms)
                if (!db.PlanDisciplineTerms.Any(d => d.DisciplineUUID == p.DisciplineUUID & d.Term == p.Term))
                {
                    var d = new PlanDisciplineTerm();
                    d.DisciplineUUID = p.DisciplineUUID;
                    d.Term = p.Term;
                    db.PlanDisciplineTerms.Add(d);
                }
           // db.PlanDisciplineTerms.AddOrUpdate(p => new { p.DisciplineUUID, p.Term }, planDisciplieTerms.ToArray());

            db.SaveChanges();
        }

        private static List<PlanDisciplineTerm> CreatePlanDisciplieTerms(string disciplineUID, string terms, IEnumerable<PlanTerm> planTerms)
        {
            var list = new List<PlanDisciplineTerm>();

            var finds = JsonConvert.DeserializeObject<List<int>>(terms);

            var t = 0;
            foreach (var pt in planTerms.OrderBy(pt => pt.Year))
            {
                var s = 0;
                //считаем сквозные семестры и когда находим наш добавляем в список

                for (var i = 0; i < pt.TermsCount; i++)
                {
                    s++;
                    t++;
                    if (finds.Contains(t))
                    {
                        list.Add(new PlanDisciplineTerm
                        {
                            DisciplineUUID = disciplineUID,
                            Term = t,
                            Course = pt.Year,
                            SemesterID = s <= 2 ? s : 0
                        });
                    }
                }
            }

            return list;
        }

        private static void SyncPlanWeeks(ApplicationDbContext db)
        {
            var service = new UniPlanTermsService();

            var weeksData = service.GetPlanTermsWeeks();

            var planTermWeeks = new List<PlanTermWeek>();
            foreach (var planTermsWeeksDto in weeksData)
            {
                foreach (var w in planTermsWeeksDto.WeeksCount)
                {
                    planTermWeeks.Add(new PlanTermWeek { eduplanUUID = planTermsWeeksDto.eduplanUUID, Term = w.Term, WeeksCount = w.WeeksCount });
                }
            }
            foreach (var p in planTermWeeks)
                if (!db.PlanTermWeeks.Any(d => d.eduplanUUID == p.eduplanUUID & d.Term == p.Term))
                {
                    var d = new PlanTermWeek();
                    d.eduplanUUID = p.eduplanUUID;
                    d.Term = p.Term;
                    db.PlanTermWeeks.Add(d);
                }
            //db.PlanTermWeeks.AddOrUpdate(p => new { p.eduplanUUID, p.Term }, planTermWeeks.ToArray());

            db.SaveChanges();
        }

        public static void GenerateTestVariants()
        {
        }

        private static bool _ratingSyncInProgress;

        public static void DoRatingSync(int year, int @class, int term, bool withCoefficients, out bool alreadyInProgress)
        {
            if (_ratingSyncInProgress)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            _ratingSyncInProgress = true;
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(token =>
            {
                try
                {
                    SyncRating(year, @class, term, withCoefficients);
                }
                finally
                {
                    _ratingSyncInProgress = false;
                }
            });
        }
        private static bool _groupHistory;

        public static void DoGroupHistorySync(int year, out bool alreadyInProgress)
        {
            if (_groupHistory)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            _groupHistory = true;
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(token =>
            {
                try
                {
                    SyncGroupHistory(year);
                }
                finally
                {
                    _groupHistory = false;
                }
            });
        }

        private static void SyncRating(int year, int @class, int term, bool withCoefficients)
        {
            Logger.Info($"Синхронизация рейтинга, параметры {year} {@class} {term} {withCoefficients}");
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var ratings = new BrsService().GetRatings(year, @class, term, withCoefficients);
                    foreach (var r in ratings)
                    {
                        var affected =
                            db.Database.ExecuteSqlCommand(
                                "Update students set RatingType = " + (int)StudentRatingType.Regular +
                                ", rating = @p0 where id = @p1 ", r.rate, r.id);
                    }

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при синхронизации рейтинга " + year + " " + term + " " + withCoefficients + " " + @class);
                Logger.Error(ex);
            }
            Logger.Info("Синхронизация рейтинга закончена");
        }


        public static void SyncForeignLanguageRating()
        {
            Logger.Info($"Синхронизация рейтинга ИЯ");
            try
            {
                var ratings = new ForeignLanguageRatingService().GetRating().GroupBy(r => r.uni);
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    foreach (var r in ratings)
                    {
                        var lastMark = r.OrderByDescending(rx => rx.attempt_number).First();

                        var affected =
                            db.Database.ExecuteSqlCommand(
                                "Update students set ForeignLanguageRating = @p1, ForeignLanguageLevel = @p2 where id = @p0 ", r.Key, lastMark.grade, lastMark.grade_level);
                    }

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при синхронизации рейтинга ИЯ ");
                Logger.Error(ex);
            }
            Logger.Info("Синхронизация рейтинга ИЯ закончена");
        }

        private static bool _ratingAvgSyncInProgress;
        public static void DoRatingAvgSync(out bool alreadyInProgress)
        {
            if (_ratingAvgSyncInProgress)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            _ratingAvgSyncInProgress = true;
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(token =>
            {
                try
                {
                    SyncRatingAvg();
                }
                finally
                {
                    _ratingAvgSyncInProgress = false;
                }
            });
        }

        private static void SyncRatingAvg()
        {
            Logger.Info($"Синхронизация средного балла");
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var ratings = new UniRatingAvgService().GetRating();
                    foreach (var r in ratings)
                    {
                        var affected =
                            db.Database.ExecuteSqlCommand(
                                "Update students set RatingType = " + (int)StudentRatingType.UniAvg +
                                ", rating = @p0 where id = @p1 ", r.avgScore, r.student);
                    }

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при синхронизации средного балла");
                Logger.Error(ex);
            }
            Logger.Info("Синхронизация средного балла закончена");
        }



        private static bool _studentPlanSyncInProgress;
        public static void DoStudentPlanSync(out bool alreadyInProgress)
        {
            if (_studentPlanSyncInProgress)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            _studentPlanSyncInProgress = true;
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(token =>
            {
                try
                {
                    SyncStudentPlan();
                }
                finally
                {
                    _studentPlanSyncInProgress = false;
                }
            });
        }

        public static void SyncStudentPlan()
        {
            Logger.Info($"Синхронизация планов студентов");
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var ratings = new UniRestService().GetStudentPlans();
                    foreach (var r in ratings)
                    {
                        var affected =
                            db.Database.ExecuteSqlCommand(
                                "Update students set planVerion = @p0, versionNumber = @p1 where id = @p2 ", r.planNumber, r.versionNumber, r.StudentId);
                    }

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при синхронизации планов студентов");
                Logger.Error(ex);
            }
            Logger.Info("Закончена синхронизация планов студентов");
        }


        private static bool _studentAllPlansSyncInProgress;
        public static void DoStudentAllPlansSync(out bool alreadyInProgress)
        {
            if (_studentPlanSyncInProgress)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            _studentPlanSyncInProgress = true;
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(token =>
            {
                try
                {
                    SyncStudentAllPlans();
                }
                finally
                {
                    _studentPlanSyncInProgress = false;
                }
            });
        }

        public static void SyncStudentAllPlans()
        {
            Logger.Info($"Синхронизация всех планов студентов");
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var newStudentsPlans = new UniRestService().GetStudentAllPlans().SelectMany(_ => _.vs.Where(dto => dto.versionNumber != null && dto.planNumber != null).Select(dto => new StudentPlan()
                    {
                        StudentId = _.student,
                        PlanNumber = dto.planNumber.Value,
                        VersionNumber = dto.versionNumber.Value
                    })).ToArray();//.Where(_ => !db.StudentPlans.Any(plan => plan.PlanNumber == _.PlanNumber && plan.StudentId == _.StudentId && plan.VersionNumber == _.VersionNumber));
                    //db.StudentPlans.AddOrUpdate(_=> new {_.StudentId,_.PlanNumber,_.VersionNumber},newStudentsPlans);
                    //foreach (var sp in studentsPlans)
                    //{
                    //    db.StudentPlans.AddRange(sp);
                    //}
                    //db.StudentPlans.AddRange(newStudentsPlans);
                    db.Upsert(newStudentsPlans);
                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при синхронизации всех планов студентов");
                Logger.Error(ex);
            }
            Logger.Info("Закончена синхронизация всех планов студентов");
        }

        private static bool _selectionSyncInProgress;

        public static void SyncStudentSelection(out bool alreadyInProgress)
        {
            if (_selectionSyncInProgress)
            {
                alreadyInProgress = true;
                return;
            }
            alreadyInProgress = false;
            _selectionSyncInProgress = true;
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(token =>
            {
                try
                {
                    SyncSelection();
                }
                finally
                {
                    _selectionSyncInProgress = false;
                }
            });
        }

        internal static void SyncSelection()
        {
            try
            {
                Logger.Info("Синхронизация всех приоритетов студентов");
                var selections = new LksService().GetSelection();
                Logger.Info("Получены приоритетов студентов");
                int cnt = 0;
                foreach (var part in selections.Partition(100))
                {
                    var list = part.ToList();
                    cnt += list.Count;
                    Logger.Info("Запись " + cnt + " приоритетов");
                    WriteStudentSelectionsToDatabase(list, new List<string>());
                }
                Logger.Info("Сохранены приоритетов студентов");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void ResyncStudentSelections()
        {
            try
            {
                List<StudentSelectionDto> dtos;
                using (var db = new ApplicationDbContext())
                {
                    dtos =
                        db.Students.Where(s => s.SelectionJson != null)
                            .Select(s => s.SelectionJson)
                            .ToList()
                            .Select(s => JsonConvert.DeserializeObject<StudentSelectionDto>(s))
                            .ToList();
                }

                Logger.Info("Ресинхронизация выборов для {0} студентов", dtos.Count);

                foreach (var part in dtos.Partition(10).ToList())
                {
                    var list = part.ToList();
                    Logger.Info("Ресинхронизация выборов для студентов {0}",
                        string.Join(", ", list.Select(l => l.studentPersonId)));
                    WriteStudentSelectionsToDatabase(list, new List<string>());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static int WriteStudentSelectionsToDatabase(List<StudentSelectionDto> selections,
            List<string> orphanIdList)
        {
            if (selections == null)
                return 0;
            int totalAffected = 0;
            using (var db = new ApplicationDbContext())
            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                foreach (var selectionDto in selections)
                {
                    var affected =
                        db.Database.ExecuteSqlCommand("Update students set SelectionJson = @p0 where Id = @p1 ",
                            JsonConvert.SerializeObject(selectionDto), selectionDto.studentPersonId);
                    totalAffected += affected;
                    if (affected == 0)
                    {
                        //Logger.Error("Не найден студент {0} для синхронизации выбора", selectionDto.studentPersonId);
                        //orphanIdList.Add(selectionDto.studentPersonId);
                    }
                }

                db.SaveChanges();
                dbcxtransaction.Commit();
            }

            Logger.Info("Выбор следующих студентов был записан в таблицу Students поле SelectionJson: {0}",
                        string.Join(", ", selections.Select(s => s.studentPersonId)));

            using (var db = new ApplicationDbContext())
            using (var dbcxtransaction = db.Database.BeginTransaction())
            {

                foreach (var selectionDto in selections)
                {

                    if (selectionDto.variants == null)
                        continue;


                    var student = db.Students.Find(selectionDto.studentPersonId);
                    if (student == null)
                    {
                        Logger.Error("Не найден студент {0} для синхронизации выбора", selectionDto.studentPersonId);
                        orphanIdList.Add(selectionDto.studentPersonId);
                        continue;
                    }

                    db.StudentVariantSelections.RemoveRange(
                        db.StudentVariantSelections.Where(id => id.studentId == selectionDto.studentPersonId));
                    db.StudentSelectionTeachers.RemoveRange(
                        db.StudentSelectionTeachers.Where(id => id.studentId == selectionDto.studentPersonId));
                    db.StudentSelectionPriority.RemoveRange(
                        db.StudentSelectionPriority.Where(id => id.studentId == selectionDto.studentPersonId));

                    db.SaveChanges();

                    var selectedVariants = selectionDto.variants.Select(s => s.selectedVariantId);
                    var admission = db.VariantAdmissions.FirstOrDefault(a => a.studentId == selectionDto.studentPersonId && a.Status == AdmissionStatus.Admitted && selectedVariants.Contains(a.variantId));
                    var variants = new List<int>().AsEnumerable();
                    if(admission != null)
                    {
                        try
                        {
                            variants = admission.Variant.IsBase
                                   ? admission.Variant.Program.Variants.Select(v => v.Id) // если зачислен на базовую траекторию, то берем и базовую, и все производные от нее
                                                                                          // если зачислен на обычную, то берем только ее и базовую
                                   : admission.Variant.Program.VariantId.HasValue // проверяем, указана ли базовая траектория 
                                           ? new List<int>() { admission.Variant.Program.VariantId.Value, admission.variantId }
                                           : new List<int>() { admission.variantId };
                        }
                        catch (Exception ex)
                        {
                            Logger.Info("У студента {0} есть зачисление variantId = {1}, но произошла ошибка",
                                           selectionDto.studentPersonId, admission?.variantId);
                            Logger.Error(ex);
                        }
                    }

                    HashSet<int> variantIds = new HashSet<int>();
                    foreach (var v in selectionDto.variants)
                    {
                        var variant = db.Variants.FirstOrDefault(_v => _v.Id == v.selectedVariantId);
                        if (variant == null)
                        {
                            Logger.Error("Траектория {0} не найдена. Выбор студента {1}", v.selectedVariantId, selectionDto.studentPersonId);
                            continue;
                        }

                        if (!variant.IsBase && v.selectedVariantPriority == 0 && admission == null)
                            continue;
                        if (!variant.IsBase && v.selectedVariantPriority == 0 && admission != null && !variants.Contains(v.selectedVariantId))
                            continue;

                        if (!variantIds.Add(v.selectedVariantId))
                        {
                            Logger.Error("Для студента {0} пропущен вариант {1}", selectionDto.studentPersonId,
                                v.selectedVariantId);
                            continue;
                        }

                        if (!variant.IsBase) // на базовых траекториях приоритетов не существует
                        {
                            var selection = new StudentVariantSelection
                            {
                                studentId = selectionDto.studentPersonId,
                                selectedVariantId = v.selectedVariantId,
                                selectedVariantPriority = v.selectedVariantPriority
                            };
                            db.StudentVariantSelections.Add(selection);
                        }


                        HashSet<int> variantContentIds = new HashSet<int>();
                        foreach (var m in v.priorities)
                        {
                            if (m.proprity == 0)
                                continue;
                            if (!variantContentIds.Add(m.variantContentId))
                            {
                                Logger.Error("Для студента {0} пропущен модуль {1}", selectionDto.studentPersonId,
                                    m.variantContentId);
                                continue;
                            }
                            if (db.VariantContents.Any(c => c.Id == m.variantContentId))
                            {
                                var ssp = new StudentSelectionPriority
                                {
                                    studentId = selectionDto.studentPersonId,
                                    variantId = v.selectedVariantId,
                                    variantContentId = m.variantContentId,
                                    proprity = m.proprity
                                };
                                db.StudentSelectionPriority.Add(ssp);
                            }
                        }

                        foreach (var t in v.teachers)
                        {
                            var sst = new StudentSelectionTeacher
                            {
                                studentId = selectionDto.studentPersonId,
                                selectedVariantPriority = v.selectedVariantPriority,
                                pkey = t.pkey,
                                control = t.control,
                                disciplineUUID = t.disciplineUUID
                            };
                            db.StudentSelectionTeachers.Add(sst);
                        }
                    }
                    db.SaveChanges();
                }

                db.SaveChanges();
                dbcxtransaction.Commit();
            }

            return totalAffected;
        }

        public static int WriteStudentMinorSelectionsToDatabase(StudentSelectionMinorDto selections)
        {
            if (selections == null)
                return 0;

            if (selections.request.Count == 0)
                return 0;

            //пока договорились что присылают 1
            var request = selections.request[0];

            using (var db = new ApplicationDbContext())
            {
                using (var dbtran = db.Database.BeginTransaction())
                {
                    //удаляем предыдущий выбор
                    db.StudentSelectionMinorPriority.RemoveRange(
                        db.StudentSelectionMinorPriority.Where(s => s.studentId == selections.student
                                                                    && s.MinorPeriod.Year == request.year
                                                                    && s.MinorPeriod.SemesterId == request.semester));


                    //сохраняем новые приоритеты
                    foreach (var p in request.minors)
                    {
                        var minorPeriod = db.MinorPeriods.FirstOrDefault(m => m.Year == request.year
                                                                              && m.SemesterId == request.semester
                                                                              && m.ModuleId == p.id);

                        var priority = new StudentSelectionMinorPriority
                        {
                            studentId = selections.student,
                            priority = p.prio,
                            minorPeriodId = minorPeriod.Id
                        };

                        db.StudentSelectionMinorPriority.Add(priority);
                    }

                    db.SaveChanges();
                    dbtran.Commit();
                }
            }

            return 0;
        }

        public static int WriteStudentForeignLanguageSelectionsToDatabase(StudentSelectionForeignLanuguageDto selection)
        {
            if (selection == null)
                return 0;

            using (var db = new ApplicationDbContext())
            {
                using (var dbtran = db.Database.BeginTransaction())
                {
                    var student = db.Students.FirstOrDefault(_ => _.Id == selection.student);
                    if (student == null)
                    {
                        return 0;
                    }
                    //удаляем предыдущий выбор
                    db.ForeignLanguageStudentSelectionPriorities.RemoveRange(
                        db.ForeignLanguageStudentSelectionPriorities.Where(s => s.studentId == selection.student
                                                                    && s.CompetitionGroup.Year == selection.year
                                                                    && s.CompetitionGroup.SemesterId == selection.semester));


                    //сохраняем новый приоритеты

                    var cg = db.ForeignLanguageCompetitionGroups.FirstOrDefault(m => m.Year == selection.year
                                                                          && m.SemesterId == selection.semester
                                                                          && m.Groups.Any(g => g.Id == db.Students.FirstOrDefault(s => s.Id == selection.student).GroupId));
                    if (cg == null) return 1;
                    var priority = new ForeignLanguageStudentSelectionPriority
                    {
                        studentId = selection.student,
                        sectionId = selection.moduleId,
                        competitionGroupId = cg.Id
                    };

                    db.ForeignLanguageStudentSelectionPriorities.Add(priority);

                    student.ForeignLanguageTargetLevel = selection.targetLevel;


                    db.SaveChanges();
                    dbtran.Commit();
                }
            }

            return 0;
        }

        public static void SyncTeachers()
        {

            Logger.Info("Синхронизация преподавателей");


            var restService = new TeacherService();
            var teacherDtos =
                restService.GetTeachers();

            try
            {
                WriteTeachersToDb(teacherDtos);
                Logger.Info("Закончена запись преподавателей ");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void SyncEntrants()
        {
            Logger.Info("Синхронизация рейтинга абитуриентов");
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var ratings = new EntrantsService().GetEntrantsRating();
                    foreach (var r in ratings)
                    {
                        var rating = r.subjects > 0 ? r.finalMark / r.subjects : 100M;
                        var affected =
                            db.Database.ExecuteSqlCommand(
                                "Update students set RatingType = " + (int)StudentRatingType.School +
                                ", rating = @p0 where id = @p1 and (RatingType is null or RatingType<>" +
                                (int)StudentRatingType.Regular + ")", rating, r.student);
                    }

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена запись рейтинга абитуриентов ");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void SyncModuleAgreements()
        {
            Logger.Info("Синхронизация соглашений");
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var agreementsDto = new AgreementService().GetAgreements();
                    var agreements = new List<ModuleAgreement>();

                    foreach (var dto in agreementsDto)
                    {
                        dto.Terms = dto.Terms ?? new int[0];
                        foreach (var term in dto.Terms)
                        {
                            var agreement = mapper.Map<ModuleAgreement>(dto);
                            agreement.SemesterId = term;
                            agreements.Add(agreement);
                        }
                    }

                    db.ModuleAgreements.RemoveRange(db.ModuleAgreements);
                    db.ModuleAgreements.AddRange(agreements);

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация соглашений");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void SyncROPs()
        {
            Logger.Info("Синхронизация РОПов");
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var ropsDto = new ROPService().GetROPs();

                    foreach(var rop in ropsDto)
                    {
                        try
                        {
                            var user = db.Users
                                .Include(u => u.UserDivisions)
                                .Include(u => u.Directions)
                                .FirstOrDefault(u => u.AdName == rop.userPrincipalName || u.SamAccountName == rop.samAccountName);

                            if (user == null)
                            {
                                Logger.Info($"Пользователь {rop.userPrincipalName} не найден");
                                continue;
                            }

                            user.SamAccountName = rop.samAccountName;

                            // сохранение подразделений
                            rop.divisions = rop.divisions ?? new List<string>();
                            var divisionsToAdd = rop.divisions.Where(d => !user.UserDivisions.Any(ud => ud.DivisionId == d)).Distinct()
                                .Select(d => new UserDivision()
                                {
                                    DivisionId = d,
                                    User = user
                                });
                            user.UserDivisions = user.UserDivisions.ToList().Concat(divisionsToAdd).ToArray();

                            // сохранение направлений
                            rop.directions = rop.directions ?? new List<string>();
                            var directionsToAdd = rop.directions.Where(d => !user.Directions.Any(ud => ud.DirectionId == d)).Distinct()
                                .Select(d => new UserDirection()
                                {
                                    DirectionId = d,
                                    User = user
                                });
                            user.Directions = user.Directions.ToList().Concat(directionsToAdd).ToArray();
                            
                            // сохранение ролей для РОПа
                            var roles = db.RoleSets.FirstOrDefault(r => r.Name == "Руководитель ОП")?.Contents?.Select(r => r.Role) ?? new List<IdentityRole>();

                            var um = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                            var currentRoles = um.GetRolesAsync(user).Result;

                            var rolesToAdd = roles.Where(r => !currentRoles.Any(cr => cr == r.Name));
                            foreach (var role in rolesToAdd)
                            {
                                um.AddToRoleAsync(user, role.Name);
                            }

                            // связь пользователя и преподавателя
                            var teacher = db.Teachers.FirstOrDefault(t => t.pkey == rop.runpId);
                            if (teacher != null)
                                teacher.User = user;
                        }
                        catch (Exception ex)
                        {
                            Logger.Info($"Ошибка при синхронизации РОПа {rop.userPrincipalName}");
                            Logger.Error(ex);
                        }
                    }

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация РОПов");
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void SyncTrajectories()
        {
            Logger.Info("Синхронизация траекторий");
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            try
            {
                using (var db = new ApplicationDbContext())
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var trajectoryDtos = new TrajectoryService().GetTrajectories();
                    foreach (var p in trajectoryDtos.Select(mapper.Map<VariantsUni>))
                        if (!db.VariantUni.Any(d => d.TrajectoryUuid == p.TrajectoryUuid))
                        {
                            var d = new VariantsUni();
                            d.TrajectoryUuid = p.TrajectoryUuid;
                            db.VariantUni.Add(d);
                        }
                   // db.VariantUni.AddOrUpdate(v => v.TrajectoryUuid, trajectoryDtos.Select(mapper.Map<VariantsUni>).ToArray());

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                Logger.Info("Закончена синхронизация траекторий");
            }
            catch (Exception ex)
            {
                Logger.Info($"Ошибка при синхронизации траекторий");
                Logger.Error(ex);
            }
        }

        public static string GetProjectStudentInfo(string id)
        {
            var info = new ProjectStudentInfoService().GetStudentInfo(id);
            return info;
        }

        public static void WriteTeachersToDb(IEnumerable<TeacherDto> teacherDtos)
        {
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            using (var db = new ApplicationDbContext())
            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                var teachers = teacherDtos.Select(mapper.Map<Teacher>).Where(_ => !string.IsNullOrEmpty(_.pkey));
                var op = db.Upsert(teachers);
                op.ExcludeField(t => t.SectionFKProperties);
                op.ExcludeField(t => t.ForeignLanguageProperties);
                op.ExcludeField(t => t.BigName);
                op.ExcludeField(t => t.FullName);
                op.ExcludeField(t => t.UserId);
                op.ExcludeField(t => t.User);
                op.ExcludeField(t => t.MUPProperties);
                op.ExcludeField(t => t.Practices);
                op.Key(student => student.pkey);
                op.Execute();

                db.SaveChanges();
                dbcxtransaction.Commit();
            }
        }

        private static readonly string[] instTypeTitles = { "Факультет", "Институт", "Филиал" };

        private static readonly string[] departmentTypeTitles = { "Факультет", "Институт", "Филиал", "Департамент" };


        public static void CreateEduProgramms(int year)
        {
            var divisions = new UniDivisionsService().GetDivisions();
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            using (var db = new ApplicationDbContext())
            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                var profilKeys =
                    db.Profiles
                        //.Where(prof => db.EduPrograms.All(prog => prog.profileId != prof.ID && prog.Year == year))
                        .Select(
                            profile =>
                                new
                                {
                                    profile,
                                    plans =
                                        db.Plans.Where(
                                            pl =>
                                                pl.directionId == profile.DIRECTION_ID &&
                                                pl.qualification == profile.QUALIFICATION && pl.active)
                                            .Select(
                                                pl =>
                                                    new
                                                    {
                                                        pl.familirizationCondition,
                                                        pl.familirizationTech,
                                                        pl.familirizationType
                                                    }).Distinct()
                                })

                        .ToList();

                Dictionary<Profile, string> profileFacultyMapping = new Dictionary<Profile, string>();
                Dictionary<Profile, string> profileDeptMapping = new Dictionary<Profile, string>();

                var notFoundChiars =
                    profilKeys.Where(k => k.profile.CHAIR_ID == null || !divisions.ContainsKey(k.profile.CHAIR_ID))
                        .ToList();

                foreach (var pk in notFoundChiars)
                {
                    Logger.Error("Не найдена кафедра для создания версии ОП " + pk.profile.CODE + " код кафедры " +
                                 pk.profile.CHAIR_ID);
                }

                profilKeys =
                    profilKeys.Where(k => k.profile.CHAIR_ID != null && divisions.ContainsKey(k.profile.CHAIR_ID)
                        /* && k.profile.CODE.StartsWith("08.04.01")*/).ToList();

                foreach (var key in profilKeys)
                {
                    var inst = divisions[key.profile.CHAIR_ID];
                    while (!instTypeTitles.Contains(inst.typeTitle))
                    {
                        inst = divisions[inst.parent];
                    }
                    profileFacultyMapping[key.profile] = inst.uuid;
                }

                foreach (var key in profilKeys)
                {
                    var inst = divisions[key.profile.CHAIR_ID];
                    while (!departmentTypeTitles.Contains(inst.typeTitle))
                    {
                        inst = divisions[inst.parent];
                    }
                    profileDeptMapping[key.profile] = inst.uuid;
                }

                var chiarIds = profilKeys.Select(p => p.profile.CHAIR_ID);
                var facultyIds = profilKeys.Select(p => profileFacultyMapping[p.profile]);
                var deptIds = profilKeys.Select(p => profileDeptMapping[p.profile]);

                var planDivision =
                    chiarIds.Union(facultyIds)
                        .Union(deptIds)
                        .Distinct()
                        .Select(id => divisions[id])
                        .Select(mapper.Map<Division>)
                        .ToArray();
                foreach (var p in planDivision) if (!db.Divisions.Any(d => d.uuid == p.uuid))
                    {
                        var d = new Division();
                        d.uuid = p.uuid;
                        db.Divisions.Add(d);
                    }
            //    db.Divisions.AddOrUpdate(d => d.uuid, planDivision);
                db.SaveChanges();

                foreach (var key in profilKeys)
                {
                    foreach (var pl in key.plans)
                    {
                        if (db.EduPrograms
                            .Any(prog =>
                                prog.profileId == key.profile.ID &&
                                prog.Year == year &&
                                prog.familirizationType == pl.familirizationType &&
                                prog.familirizationCondition == pl.familirizationCondition))
                            continue;
                        var p = new EduProgram
                        {
                            directionId = key.profile.DIRECTION_ID,
                            profileId = key.profile.ID,
                            divisionId = profileFacultyMapping[key.profile],
                            departmentId = profileDeptMapping[key.profile],
                            chairId = key.profile.CHAIR_ID,
                            qualification = key.profile.QUALIFICATION,
                            Year = year,
                            Name = key.profile.NAME,
                            familirizationCondition = pl.familirizationCondition,
                            familirizationType = pl.familirizationType,
                            State = VariantState.Approved
                        };
                        db.EduPrograms.Add(p);
                    }
                }

                db.SaveChanges();
                dbcxtransaction.Commit();
            }
        }

        public static void GenerateTestSelections()
        {
            using (var db = new ApplicationDbContext())
            {
                db.ChangeTracker.AutoDetectChangesEnabled = false;
                var programs = db.EduPrograms.Where(p => p.Variant.State == VariantState.Approved);
                foreach (var p in programs.ToList())
                {
                    var students = db.Students
                        .Where(s => s.Group.Profile.DIRECTION_ID == p.directionId
                                    && s.Group.FamCond == p.familirizationCondition
                                    && s.Group.FamType == p.familirizationType
                                    && s.Group.Qual == p.qualification);
                    foreach (var s in students.ToList())
                    {
                        int priority = 1;
                        foreach (var v in p.Variants.Where(v => !v.IsBase).OrderBy(sx => Guid.NewGuid()).ToList())
                        {
                            var svs = new StudentVariantSelection
                            {
                                selectedVariantId = v.Id,
                                studentId = s.Id,
                                selectedVariantPriority = priority++
                            };
                            db.StudentVariantSelections.Add(svs);
                        }
                    }

                    db.SaveChanges();
                }
            }
        }

        public static void UpdateDivision(DivisionDto[] divisionDtos, Dictionary<string, DivisionDto> allDivisions)
        {
            HashSet<string> ids = new HashSet<string>();
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();
            foreach (var dto in divisionDtos)
            {
                var w = dto;
                while (w != null)
                {
                    if (!ids.Add(w.uuid))
                        break;
                    if (w.parent == null)
                        break;
                    if (!allDivisions.TryGetValue(w.parent, out w))
                        break;
                }
            }
            using (var db = new ApplicationDbContext())
            {
                db.ChangeTracker.AutoDetectChangesEnabled = false;

                var planDivision = ids.Select(id => allDivisions[id]).Select(mapper.Map<Division>).ToArray();
                foreach(var p in planDivision) if (!db.Divisions.Any(d => d.uuid == p.uuid))
                    {
                        var d = new Division();
                        d.uuid = p.uuid;
                        db.Divisions.Add(d);
                    }
                db.SaveChanges();
                Logger.Info($"Синхронизированы подразделения: {string.Join(", ", ids)}");
            }
        }
    }
}
