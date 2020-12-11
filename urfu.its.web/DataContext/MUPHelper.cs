//using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
//using Microsoft.EntityFrameworkCore.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;
using Urfu.Its.Common;
//using Urfu.Its.Web.Migrations;
using Urfu.Its.Web.Model.Models;

namespace Urfu.Its.Web.DataContext
{
    public class MUPHelper
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private readonly Dictionary<string, string> tmers = new Dictionary<string, string>()
        {
            { "PRETEST", "prz" }, // зависимое поле pretest_tmer
            //{ "К", "U004" },
            { "LECT", "tlekc" },
            { "LAB", "tlab" },
            { "SEMI", "tprak" },
            { "EXAMINATION", "prex" }, // зависимое поле exam_tmer
            { "CONS", "U006"}
        };

        /// <summary>
        /// у преподавателей EventTypes
        /// </summary>
        private readonly string mid_check = "MID_CHECK";

        /// <summary>
        /// tmers["EXAMINATION"]
        /// </summary>
        private readonly string exam_tmer = "prex";

        /// <summary>
        /// tmers["PRETEST"]
        /// </summary>
        private readonly string pretest_tmer = "prz";

        private readonly string publishedState = "Published";

        /// <summary>
        /// Добавить связи Модуль-МУП
        /// </summary>
        /// <param name="moduleId">Модуль, к которому будут добавлены связи</param>
        /// <param name="connections">Список обновленных связей</param>
        /// <param name="message">Сообщение с причиной частично успешного обновления связей</param>
        /// <returns>true - успешно, false - частично успешно, см. сообщение message со списком неудаленных связей</returns>
        public bool ConnectDisciplineToMUP(string moduleId, List<MUPDisciplineConnectionVM> connections, out string message)
        {
            var existConnections = db.MUPDisciplineConnections.Where(m => m.ModuleId == moduleId).ToList();

            foreach (var connection in connections)
            {
                foreach (var mup in connection.mups)
                {
                    var mupDB = db.MUPModeuses.FirstOrDefault(m => m.Id == mup);

                    var existConnection = existConnections.FirstOrDefault(c => c.DisciplineId == connection.uid && c.MUPModeusId == mup);
                    if (existConnection == null)
                    {
                        var moduleMUP = CreateModule(mupDB);

                        db.MUPDisciplineConnections.Add(new MUPDisciplineConnection()
                        {
                            DisciplineId = connection.uid,
                            ModuleId = moduleId,
                            MUPModeusId = mup,
                            ModuleMUPId = moduleMUP.uuid
                        });
                        db.SaveChanges();
                    }
                    else
                    {
                        var connectionDB = db.MUPDisciplineConnections.FirstOrDefault(c => c.Id == existConnection.Id);
                        if (connectionDB.ModuleMUPId == null)
                        {
                            var module = CreateModule(mupDB);
                            connectionDB.ModuleMUPId = module.uuid;
                            db.SaveChanges();
                        }
                        existConnections.Remove(existConnection);
                    }
                }
            }

            var deleted = existConnections.Select(c => new
            {
                connection = c,
                subgroups = SubgroupsExist(c)
            });
            

            db.MUPDisciplineConnections.RemoveRange(deleted.Where(c => !c.subgroups).Select(m => m.connection));
            db.SaveChanges();

            var unremoved = deleted.Where(c => c.subgroups).Select(m => m.connection.ModuleMUP.title);

            message = $"Связь со следующими МУПами не может быть удалена: {string.Join("; ", unremoved)}. Существуют подгруппы.";
            return unremoved.Count() == 0;
        }

        private bool SubgroupsExist(MUPDisciplineConnection connection)
        {
            return connection.ModuleMUP?.MUP?.Periods?.SelectMany(p => p.MUPDisciplineTmerPeriods)
                .SelectMany(s => s.MUPSubgroupCounts).Any(s => s.Subgroups.Count() > 0) ?? false;
        }

        public void FillMUPTables()
        {
            var modules = db.MUPDisciplineConnections.Where(c => !c.MUPModeus.Removed).Select(c => c.ModuleId).Distinct().ToList();
            foreach (var module in modules)
            {
                FillMUPTables(module);
            }
        }

        public void FillMUPTables(string moduleId)
        {
            Logger.Info($"Начата конвертация связей для модуля uni {moduleId}");

            var moduleMUPIds = db.MUPDisciplineConnections.Where(c => c.ModuleId == moduleId).Select(c => c.ModuleMUPId).ToList();

            foreach (var moduleMUP in moduleMUPIds)
            {
                Logger.Info($"Начата конвертация связей для модуля its {moduleMUP}");

                var mup = db.MUPs.FirstOrDefault(m => !m.Removed && m.ModuleId == moduleMUP);
                if (mup == null)
                {
                    mup = new MUP()
                    {
                        ModuleId = moduleMUP,
                        ModuleTechId = 1
                    };
                    db.MUPs.Add(mup);
                    db.SaveChanges();
                }

                var teams = GetTeams(moduleMUP).ToList();

                CreateMUPPeriods(moduleMUP, teams);
                CreateMUPDisciplines(moduleMUP);
                CreateMUPDisciplineTmers(moduleMUP, teams);
                CreateMUPDisciplineTmerPeriods(moduleMUP, mup, teams);

                Logger.Info($"Закончена конвертация связей для модуля its {moduleMUP}");
            }

            Logger.Info($"Закончена конвертация связей для модуля uni {moduleId}");
        }

        public void FillMUPSubgroupTables(int competitionGroupId)
        {
            Logger.Info($"Начата конвертация связей для конкурсной группы МУП {competitionGroupId}");
            try
            {
                var competitionGroup = db.MUPCompetitionGroups.FirstOrDefault(g => g.Id == competitionGroupId);
                if (competitionGroup == null)
                {
                    Logger.Info($"Закончена конвертация связей для конкурсной группы МУП {competitionGroupId}. Конкурсная группа не найдена");
                    return;
                }

                CreateMUPProperties(competitionGroup);
                CreateMUPSubgroupCounts(competitionGroup);

                var teams = GetTeams(competitionGroup).ToList();

                CreateMUPSubgroups(competitionGroup, teams);
                CreateMUPTeachers(competitionGroup, teams);
                CreateMUPSubgroupTeachers(competitionGroup);
                CreateMUPStudentsMembership(competitionGroup);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Info($"Закончена конвертация связей для конкурсной группы МУП {competitionGroupId}");
        }

        private void CreateMUPPeriods(string moduleMUPId, List<MUPModeusTeam> teams)
        {
            var periods = teams.Select(t => new
            {
                year = t.MUPRealization.Year,
                semester = t.MUPRealization.SemesterId,
                course = t.MUPModeus.Course
            }).Distinct();

            var mupPeriods = db.MUPPeriods.Where(p => p.MUPId == moduleMUPId).ToList();
            foreach (var z in db.MUPPeriods.Where(p => p.MUPId == moduleMUPId).ToList().Where(p => !periods.Any(per => per.course == p.Course && per.semester == p.SemesterId && per.year == p.Year)))
                z.Removed = true;
            //db.MUPPeriods.Where(p => p.MUPId == moduleMUPId).ToList().Where(p => !periods.Any(per => per.course == p.Course && per.semester == p.SemesterId && per.year == p.Year))
            //    .ForEach(p => p.Removed = true);

            foreach (var period in periods)
            {
                var mupPeriod = mupPeriods.FirstOrDefault(p => p.Course == period.course && p.SemesterId == period.semester && p.Year == period.year);
                if (mupPeriod == null)
                {
                    db.MUPPeriods.Add(new MUPPeriod()
                    {
                        Course = period.course,
                        MUPId = moduleMUPId,
                        SemesterId = period.semester.Value,
                        Year = period.year.Value
                    });
                }
                else
                    mupPeriod.Removed = false;
                db.SaveChanges();
            }
        }

        private Module CreateModule(MUPModeus mup)
        {
            var moduleMUP = new Module();
            moduleMUP.uuid = GuidHelper.GetGuid();
            moduleMUP.type = "МУП";
            moduleMUP.title = mup?.Name;
            moduleMUP.shortTitle = mup?.ShortName;
            moduleMUP.Source = Urfu.Its.Web.Models.Source.Mup;

            db.Modules.Add(moduleMUP);
            db.SaveChanges();

            return moduleMUP;
        }

        private void CreateMUPDisciplines(string moduleMUPId)
        {
            var connections = db.MUPDisciplineConnections.Where(c => c.ModuleMUPId == moduleMUPId).ToList();
            foreach (var connect in connections)
            {
                Discipline discipline = connect.DisciplineMUP;
                if (discipline == null)
                {
                    discipline = new Discipline()
                    {
                        uid = "mupits" + GuidHelper.GetGuid(),
                        title = connect.ModuleMUP.title,
                        pkey = GuidHelper.GetGuid(),
                        testUnits = 0
                    };
                    db.Disciplines.Add(discipline);
                    connect.DisciplineMUP = discipline;
                    db.SaveChanges();
                }

                if (!db.MUPDisciplines.Any(d => d.DisciplineUid == discipline.uid && d.MUPId == moduleMUPId))
                {
                    db.MUPDisciplines.Add(new MUPDiscipline()
                    {
                        DisciplineUid = discipline.uid,
                        MUPId = moduleMUPId
                    });
                    db.SaveChanges();
                }
            }
        }

        private void CreateMUPDisciplineTmers(string moduleMUPId, List<MUPModeusTeam> teams)
        {
            var mupDisciplines = db.MUPDisciplines.Where(d => d.MUPId == moduleMUPId).ToList();

            foreach (var mupDiscipline in mupDisciplines)
            {
                var disciplineTeams = teams.Where(t => t.MUPModeus.Connections.FirstOrDefault(c => c.DisciplineMUPId == mupDiscipline.DisciplineUid) != null);

                var kmers = new List<string>();
                foreach (var team in disciplineTeams)
                {
                    kmers.AddRange(GetTmers(team.EventTypes).Concat(GetTmers(team.Kmers)));
                }
                var uniqueKmers = kmers.Distinct();

                foreach (var z in db.MUPDisciplineTmers.Where(t => t.MUPDisciplineId == mupDiscipline.Id && !uniqueKmers.Contains(t.TmerId)))
                    z.Removed = true;
//                db.MUPDisciplineTmers.Where(t => t.MUPDisciplineId == mupDiscipline.Id && !uniqueKmers.Contains(t.TmerId)).ForEach(t => t.Removed = true);

                foreach (var kmer in uniqueKmers)
                {
                    var tmer = db.MUPDisciplineTmers.FirstOrDefault(t => t.MUPDisciplineId == mupDiscipline.Id && t.TmerId == kmer);
                    if (tmer == null)
                    {
                        db.MUPDisciplineTmers.Add(new MUPDisciplineTmer()
                        {
                            MUPDisciplineId = mupDiscipline.Id,
                            TmerId = kmer
                        });
                    }
                    else
                        tmer.Removed = false;
                    db.SaveChanges();
                }
            }
        }

        private void CreateMUPDisciplineTmerPeriods(string moduleMUPId, MUP mup, List<MUPModeusTeam> teams)
        {
            var mupDisciplinesTmers = db.MUPDisciplineTmers.Where(t => t.Discipline.MUPId == moduleMUPId).ToList();

            foreach (var mupTmer in mupDisciplinesTmers)
            {
                var disciplineTeams = teams.Where(t => t.MUPModeus.Connections.FirstOrDefault(c => c.DisciplineMUPId == mupTmer.Discipline.DisciplineUid) != null);
                var periods = mup.Periods;
                foreach (var z in db.MUPDisciplineTmerPeriods.Where(t => t.MUPDisciplineTmerId == mupTmer.Id)) z.Removed = true;
                //db.MUPDisciplineTmerPeriods.Where(t => t.MUPDisciplineTmerId == mupTmer.Id).ForEach(t => t.Removed = true);

                foreach (var team in disciplineTeams)
                {
                    var period = periods.FirstOrDefault(p => p.Year == team.MUPRealization.Year.Value && p.SemesterId == team.MUPRealization.SemesterId.Value && p.Course == team.MUPModeus.Course);
                    if (period == null)
                        continue;
                    var tmerPeriod = db.MUPDisciplineTmerPeriods.FirstOrDefault(p => p.MUPDisciplineTmerId == mupTmer.Id && p.MUPPeriodId == period.Id);

                    if (period != null && tmerPeriod == null)
                    {
                        tmerPeriod = new MUPDisciplineTmerPeriod()
                        {
                            MUPDisciplineTmerId = mupTmer.Id,
                            MUPPeriodId = period.Id
                        };
                        db.MUPDisciplineTmerPeriods.Add(tmerPeriod);

                    }
                    tmerPeriod.Removed = false;

                    tmerPeriod.Divisions = tmerPeriod.Divisions ?? new List<Division>();
                    var chairUuid = "undich18ggl5g0000kaimq8hqbcvlj94"; // uuid кафедры школы бакалавриата
                    if (!tmerPeriod.Divisions.Any(d => d.uuid == chairUuid))
                    {
                        tmerPeriod.Divisions.Add(db.Divisions.FirstOrDefault(d => d.uuid == chairUuid));
                    }
                    db.SaveChanges();
                }
            }
        }

        private IQueryable<MUPModeusTeam> GetTeams(string moduleMUPId)
        {
            var teams = db.MUPDisciplineConnections.Where(c => c.ModuleMUPId == moduleMUPId)
                    .SelectMany(c => c.MUPModeus.Teams)
                        // команда не считается удаленной только если оба признака Removed и Deleted = 0
                        .Where(t => !t.Deleted && !t.Removed && (t.EventTypes != null || t.Kmers != null) && t.MUPRealization.SemesterId.HasValue && t.MUPRealization.Year.HasValue
                                    && t.MUPRealization.State == publishedState);
            return teams;
        }

        private IQueryable<MUPModeusTeam> GetTeams(MUPCompetitionGroup competitionGroup)
        {
            var moduleMUPIds = competitionGroup.MUPProperties.Select(p => p.MUPId);
            var teams = db.MUPDisciplineConnections.Where(c => moduleMUPIds.Contains(c.ModuleMUPId))
                    .SelectMany(c => c.MUPModeus.Teams)
                        .Where(t => !t.Deleted && !t.Removed && (t.EventTypes != null || t.Kmers != null)
                                && t.MUPRealization.SemesterId == competitionGroup.SemesterId && t.MUPRealization.Year == competitionGroup.Year
                                && t.MUPRealization.State == publishedState);
            return teams;
        }

        private IEnumerable<string> GetTmers(string tmerStr)
        {
            var tmersList = tmerStr?.Replace(" ", "").Split(',').Where(s => tmers.Keys.Contains(s)) ?? new List<string>();
            return tmersList.Select(t => tmers[t]);
        }

        private bool IsMidCheck(string tmerStr, string tmer)
        {
            tmerStr = tmerStr ?? "";
            return tmerStr.Replace(" ", "").Split(',').Any(s => s == mid_check) && (tmer == exam_tmer || tmer == pretest_tmer);
        }

        private IQueryable<MUP> GetMUPsQuery(MUPCompetitionGroup competitionGroup)
        {
            return db.MUPs.Where(m => !m.Removed && m.Periods.Any(p => (p.Year == competitionGroup.Year) && (p.SemesterId == competitionGroup.SemesterId)));
        }

        private void CreateMUPProperties(MUPCompetitionGroup competitionGroup)
        {
            var mups = GetMUPsQuery(competitionGroup).ToList();

            foreach (var mup in mups)
            {
                if (!competitionGroup.MUPProperties.Any(p => p.MUPId == mup.ModuleId && p.MUPCompetitionGroupId == competitionGroup.Id))
                {
                    competitionGroup.MUPProperties.Add(new MUPProperty
                    {
                        MUPId = mup.ModuleId,
                        MUPCompetitionGroupId = competitionGroup.Id,

                    });
                    db.SaveChanges();
                    //Logger.Info($"Добавлена запись в MUPProperties: MUPId {mup.ModuleId}, MUPCompetitionGroupId {competitionGroup.Id}");
                }
            }
        }

        private void CreateMUPSubgroupCounts(MUPCompetitionGroup competitionGroup)
        {
            var tmerPeriods = GetMUPsQuery(competitionGroup)
                .SelectMany(m => m.Disciplines).SelectMany(d => d.Tmers).SelectMany(t => t.Periods).ToList();

            foreach (var tmerPeriod in tmerPeriods)
            {
                if (!db.MUPSubgroupCounts.Any(c => c.CompetitionGroupId == competitionGroup.Id && c.MUPDisciplineTmerPeriodId == tmerPeriod.Id))
                {
                    db.MUPSubgroupCounts.Add(new MUPSubgroupCount()
                    {
                        CompetitionGroupId = competitionGroup.Id,
                        MUPDisciplineTmerPeriodId = tmerPeriod.Id,
                        GroupCount = 100
                    });
                    db.SaveChanges();
                    //Logger.Info($"Добавлена запись в MUPSubgroupCounts: MUPDisciplineTmerPeriodId {tmerPeriod.Id}, MUPCompetitionGroupId {competitionGroup.Id}");
                }
            }
        }

        private void CreateMUPSubgroups(MUPCompetitionGroup competitionGroup, List<MUPModeusTeam> teams)
        {
            Logger.Info($"Начато добавление подгрупп для конкурсной группы МУП {competitionGroup.Id}");
            var subgroupsCounts = competitionGroup.MUPProperties.Select(p => p.MUP).SelectMany(m => m.Disciplines)
                .SelectMany(d => d.Tmers).SelectMany(t => t.Periods).Where(p =>
                        p.Period.Year == competitionGroup.Year && p.Period.SemesterId == competitionGroup.SemesterId
                            && (p.Period.Course == null || competitionGroup.StudentCourse == 0 || p.Period.Course == competitionGroup.StudentCourse))
                            .SelectMany(s => s.MUPSubgroupCounts).Where(s => s.CompetitionGroupId == competitionGroup.Id);

            var groups = competitionGroup.Groups.Select(g => g.Id);
            foreach (var z in db.MUPSubgroups.Where(s => s.Meta.CompetitionGroupId == competitionGroup.Id)) z.Removed = true;
            //db.MUPSubgroups.Where(s => s.Meta.CompetitionGroupId == competitionGroup.Id).ForEach(s => s.Removed = true);



            foreach (var team in teams)
            {
                var teamTmers = GetTmers(team.EventTypes).Concat(GetTmers(team.Kmers));

                var suitableSubgroupsCounts = subgroupsCounts.Where(s => teamTmers.Contains(s.MUPDisciplineTmerPeriod.Tmer.TmerId)
                        && team.MUPModeus.Connections.Any(c =>
                            c.ModuleMUPId == s.MUPDisciplineTmerPeriod.Period.MUPId && c.DisciplineMUPId == s.MUPDisciplineTmerPeriod.Tmer.Discipline.DisciplineUid));

                //Logger.Info($"Найдено {suitableSubgroupsCounts.Count()} subgroupCount для mupmodeusteam {team.Name} {team.Id}");
                foreach (var c in suitableSubgroupsCounts) c.Subgroups = c.Subgroups ?? new List<MUPSubgroup>();
                //suitableSubgroupsCounts.ForEach(c => c.Subgroups = c.Subgroups ?? new List<MUPSubgroup>());
                //suitableSubgroupsCounts.ForEach(c => c.Subgroups.Where(s => s.MUPModeusTeamId == team.Id).ForEach(s => s.Removed = true));

                foreach (var subgroupCount in suitableSubgroupsCounts)
                {
                    subgroupCount.Subgroups = subgroupCount.Subgroups ?? new List<MUPSubgroup>();
                    var subgroup = subgroupCount.Subgroups.FirstOrDefault(s => s.MUPModeusTeamId == team.Id);

                    if (subgroup == null)
                    {
                        var innerNumber = subgroupCount.Subgroups.Select(s => s.InnerNumber).OrderByDescending(s => s).FirstOrDefault() + 1;
                        var subgroupName = subgroupCount.MUPDisciplineTmerPeriod.Tmer?.Tmer.kmer.ToLower() == "U006" ? $"{team.MUPModeus.Name}\\{GetKmerName(subgroupCount.MUPDisciplineTmerPeriod.Tmer?.Tmer.kmer.ToLower())}\\{innerNumber}" : $"{team.MUPModeus.Name}\\{GetKmerName(subgroupCount.MUPDisciplineTmerPeriod.Tmer?.Tmer.kmer.ToLower())}{innerNumber}";

                        subgroup = new MUPSubgroup()
                        {
                            Limit = 100,
                            MUPModeusTeamId = team.Id,
                            Name = subgroupName,
                            SubgroupCountId = subgroupCount.Id,
                            InnerNumber = innerNumber,
                            Description = team.Name
                        };
                        subgroupCount.Subgroups.Add(subgroup);
                        Logger.Info($"Добавлена подгруппа {subgroupName} для mupmodeusteam {team.Name} {team.Id}");
                    }
                    else
                        subgroup.Removed = false;
                }
            }
            db.SaveChanges();
            Logger.Info($"Закончено добавление подгрупп для конкурсной группы МУП {competitionGroup.Id}");
        }

        private void CreateMUPTeachers(MUPCompetitionGroup competitionGroup, List<MUPModeusTeam> teams)
        {
            Logger.Info($"Начато добавление преподавателей для конкурсной группы МУП {competitionGroup.Id}");
            var teachers = teams.Where(t => t.MUPSubgroups.Count > 0).Select(team => new
            {
                mupId = team.MUPSubgroups.FirstOrDefault().Meta.MUPDisciplineTmerPeriod.Period.MUPId,
                teachers = team.Teachers.Where(t => !t.Deleted && !t.Removed).Select(t => t.PersonId).Distinct()
            })
            .GroupBy(t => t.mupId)
            .Select(t => new
            {
                mupId = t.Key,
                teachers = t.SelectMany(tt => tt.teachers).Distinct()
            });

            var properties = competitionGroup.MUPProperties;
            foreach (var p in properties)
            {
                var teacherIds = teachers.FirstOrDefault(t => t.mupId == p.MUPId)?.teachers ?? new List<string>();

                p.Teachers = p.Teachers ?? new List<Teacher>();
                var teachersToRemove = p.Teachers.Where(t => !teacherIds.Contains(t.AccountancyGuid)).ToList();
                foreach (var t in teachersToRemove)
                {
                    p.Teachers.Remove(t);
                }

                var teachersToAdd = teacherIds.Where(t => !p.Teachers.Select(pt => pt.AccountancyGuid).Contains(t));

                p.Teachers = p.Teachers.Concat(db.Teachers.Where(t => teachersToAdd.Contains(t.AccountancyGuid))).ToList();
                db.SaveChanges();
            }
            Logger.Info($"Закончено добавление преподавателей для конкурсной группы МУП {competitionGroup.Id}");
        }

        private void CreateMUPSubgroupTeachers(MUPCompetitionGroup competitionGroup)
        {
            Logger.Info($"Начато добавление преподавателей на подгруппы для конкурсной группы МУП {competitionGroup.Id}");
            var subgroups = db.MUPSubgroups.Where(s => s.Meta.CompetitionGroupId == competitionGroup.Id && !s.Removed).ToList();
            foreach (var subgroup in subgroups)
            {
                var tmer = subgroup.Meta.MUPDisciplineTmerPeriod.Tmer.TmerId;

                var subgroupModeusTeacherIds = subgroup.MUPModeusTeam.Teachers.Where(t => !t.Deleted && !t.Removed).ToList()
                    .Where(t => GetTmers(t.EventTypes).Contains(tmer) || IsMidCheck(t.EventTypes, tmer))
                    .Select(t => t.PersonId).Distinct().ToList();
                db.MUPSubgroupTeachers.RemoveRange(db.MUPSubgroupTeachers.Where(s => s.MUPSubgroupId == subgroup.Id && !subgroupModeusTeacherIds.Contains(s.Teacher.AccountancyGuid)));

                foreach (var teacherId in subgroupModeusTeacherIds)
                {
                    var teacher = db.Teachers.FirstOrDefault(t => t.AccountancyGuid == teacherId);
                    if (!db.MUPSubgroupTeachers.Any(t => t.Teacher.AccountancyGuid == teacherId && t.MUPSubgroupId == subgroup.Id) && teacher != null)
                    {
                        db.MUPSubgroupTeachers.Add(new MUPSubgroupTeacher()
                        {
                            MUPSubgroupId = subgroup.Id,
                            TeacherId = teacher.pkey
                        });
                    }
                    db.SaveChanges();
                }
                subgroup.Teacher = db.MUPSubgroupTeachers.FirstOrDefault(t => t.MUPSubgroupId == subgroup.Id)?.Teacher;
            }
            db.SaveChanges();
            Logger.Info($"Закончено добавление преподавателей на подгруппы для конкурсной группы МУП {competitionGroup.Id}");
        }

        private void CreateMUPStudentsMembership(MUPCompetitionGroup competitionGroup)
        {
            Logger.Info($"Начато добавление зачислений студентов на подгруппы для конкурсной группы МУП {competitionGroup.Id}");
            var groups = competitionGroup.Groups.Select(g => g.Id);

            db.MUPSubgroupMemberships.RemoveRange(
                db.MUPSubgroupMemberships.Where(m => m.Subgroup.Meta.CompetitionGroupId == competitionGroup.Id && m.Subgroup.Removed));
            db.SaveChanges();

            var subgroups = db.MUPSubgroups.Include(s => s.Students).Where(s => s.Meta.CompetitionGroupId == competitionGroup.Id && !s.Removed).ToList();
            var subgroupMemberships = subgroups.SelectMany(s => s.MUPModeusTeam.Students
                            .Where(st => !st.Removed && !st.Deleted).GroupBy(st => st.StudentId)
                            .Select(st => db.Students.FirstOrDefault(stdb => groups.Contains(stdb.GroupId)))
                            .Where(st => st != null)
                            .Select(st => new MUPSubgroupMembership()
                            {
                                studentId = st.Id,
                                SubgroupId = s.Id
                            })
                ).ToList();

            db.MUPSubgroupMemberships.RemoveRange(subgroups.SelectMany(s => s.Students));
            db.MUPSubgroupMemberships.AddRange(subgroupMemberships);
            db.SaveChanges();

            Logger.Info($"Закончено добавление зачислений студентов на подгруппы для конкурсной группы МУП {competitionGroup.Id}");

            Logger.Info($"Начато добавление зачислений на конкурсную группу МУП {competitionGroup.Id}");

            var newAdmissions = subgroupMemberships
                .Select(s => new MUPAdmission()
                {
                    MUPCompetitionGroupId = competitionGroup.Id,
                    MUPId = s.Subgroup.Meta.MUPDisciplineTmerPeriod.Period.MUPId,
                    studentId = s.studentId,
                    Status = AdmissionStatus.Admitted,
                    Published = false
                }).GroupBy(a => new { a.studentId, a.MUPCompetitionGroupId, a.MUPId }).ToList();

            db.MUPAdmissions.RemoveRange(db.MUPAdmissions.Where(a => a.MUPCompetitionGroupId == competitionGroup.Id));
            db.MUPAdmissions.AddRange((IEnumerable<MUPAdmission>)newAdmissions);
            db.SaveChanges();

            Logger.Info($"Закончено добавление зачислений на конкурсную группу МУП {competitionGroup.Id}");
        }

        /// <summary>
        /// Извлекает последнее число из строки. 
        /// Предполагается, что строка будет вида л1, л1п1 и т.п.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private int GetLastNumber(string str)
        {
            str = str ?? ".";
            int index = str.Length - 1;
            var innerNumberStr = ""; // число в виде строки в перевернутом виде. Например, если строка "13", то число должно быть 31
            var character = str[index];

            // Идем с конца строки до первого нечислового символа
            while (char.IsDigit(character))
            {
                innerNumberStr += character;
                index--;
                character = index >= 0 ? str[index] : '.'; // если прошли всю строку, то выходим из цикла
            }

            int innerNumber;
            var innerNumberParsed = int.TryParse(new string(innerNumberStr.Reverse().ToArray()), out innerNumber);
            return innerNumber;
        }

        private string GetKmerName(string kmer)
        {
            switch (kmer)
            {
                case "prz":
                    return "зачет";
                case "tlekc":
                    return "л";
                case "tlab":
                    return "лаб";
                case "tprak":
                    return "п";
                case "prex":
                    return "экзамен";
                case "U006":
                    return "консультация перед экзаменом";
                default:
                    return "";
            }
        }
    }
}