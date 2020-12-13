using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Common;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Urfu.Its.Web.DataContext
{
    public class ProjectSync
    {
        const string tlekc = "tlekc"; // нагрузка лекции
        const string prex = "prex"; // нагрузка экзамен

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private Division chair;

        public string Message { get; set; }

        /// <summary>
        /// Добавить новый проект
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool AddProject(ProjectApiDto project)
        {
            Message = null;
            using (var dbtran = db.Database.BeginTransaction())
            {
                try
                {
                    if (db.Projects.Any(p => p.EmployersId == project.id))
                    {
                        Message = "Проект с таким id уже существует";
                        return false;
                    }

                    if (!IsLevelValid(project.level))
                    {
                        Message = "Некорректный уровень проекта";
                        return false;
                    }

                    // проверка на наличие проектных групп для каждого лимита
                    List<ProjectCompetitionGroup> projectGroups = GetProjectGroups(project);
                    if (projectGroups == null)
                    {
                        Message = "Не создана проектная группа";
                        return false;
                    }

                    // проектные группы есть
                    // создаем предприятие и договор
                    var contract = CreateCompanyAndContract(project);

                    // создаем модуль и проект, добавляем роли
                    var projectDB = CreateModuleAndProject(project, contract.Id);

                    // создаем периоды и лимиты по договору, и периоды на проект
                    var projectPeriods = CreatePeriodsAndLimits(project, contract, projectDB);

                    // создаем дисциплину
                    var projectDiscipline = CreateProjectDiscipline(project, projectDB);

                    // создаем свойства проекта - связь проектной группы и проекта
                    var properties = CreateProjectProperties(projectGroups, projectDB);

                    // добавление кураторов и РОПа
                    CreateUsers(project, properties, projectDB);

                    // добавление нагрузки. для проектов это лекции и экзамен. 
                    var projectTmerPeriods = CreateProjectTmersPeriods(projectDiscipline, projectPeriods);

                    // добавление количества подгрупп
                    if (project.maxSubgroups.HasValue)
                    {
                        CreateProjectSubgroupCount(project.maxSubgroups.Value, projectGroups, projectTmerPeriods);
                    }

                    db.SaveChanges();
                    dbtran.Commit();
                }
                catch (Exception ex)
                {
                    dbtran.Rollback();
                    Message = $"{ex?.Message} {ex?.InnerException?.InnerException?.Message}";
                }
            }

            return string.IsNullOrWhiteSpace(Message);
        }

        /// <summary>
        /// Обновить информацию о существующем проекте
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool UpdateProject(ProjectApiDto project)
        {
            Message = null;

            using (var dbtran = db.Database.BeginTransaction())
            {
                try
                {
                    var projectDB = db.Projects.FirstOrDefault(p => p.EmployersId == project.id);

                    if (projectDB == null)
                    {
                        Message = "Проект с таким id не найден";
                        return false;
                    }

                    if (!IsLevelValid(project.level))
                    {
                        Message = "Некорректный уровень проекта";
                        return false;
                    }

                    // проверка на наличие проектных групп для каждого лимита
                    List<ProjectCompetitionGroup> projectGroups = GetProjectGroups(project);
                    if (projectGroups == null)
                    {
                        Message = "Не создана проектная группа";
                        return false;
                    }

                    // проектные группы есть
                    // обновляем предприятие и договор
                    UpdateCompanyAndContract(project, projectDB);

                    // обновляем модуль и проект, и роли
                    UpdateModuleAndProject(project, projectDB);

                    // обновляем периоды и лимиты по договору, и периоды на проект
                    UpdatePeriodsAndLimits(project, projectDB);

                    // обновляем дисциплину
                    UpdateProjectDiscipline(project, projectDB);

                    // создаем свойства проекта - связь проектной группы и проекта
                    UpdateProjectProperties(projectGroups, projectDB);

                    // обновление кураторов и РОПа
                    UpdateUsers(project, projectDB);

                    // обновление нагрузки
                    UpdateProjectTmersPeriods(projectDB);

                    // обновление количества подгрупп
                    if (project.maxSubgroups.HasValue)
                    {
                        UpdateProjectSubgroupCount(project.maxSubgroups.Value, projectGroups, projectDB);
                    }

                    db.SaveChanges();
                    dbtran.Commit();
                }
                catch (Exception ex)
                {
                    dbtran.Rollback();
                    Message = $"{ex?.Message} {ex?.InnerException?.InnerException?.Message}";
                    Logger.Error(ex);
                }
            }

            return string.IsNullOrWhiteSpace(Message);
        }

        private bool IsLevelValid(string level)
        {
            return Regex.IsMatch(level, "^[А-Яа-я]+$");
        }

        private List<ProjectCompetitionGroup> GetProjectGroups(ProjectApiDto project)
        {
            List<ProjectCompetitionGroup> projectGroups = new List<ProjectCompetitionGroup>();
            bool projectGroupsNotFound = false;
            foreach (var l in project.limits)
            {
                Func<ProjectCompetitionGroup, bool> predicate
                    = g => g.Year == l.year && g.SemesterId == l.semesterId && (g.StudentCourse == 0 || g.StudentCourse == l.course || l.course == 0)
                            && g.Groups.Select(gr => gr.ProfileId).Contains(l.profileId);

                if (!projectGroups.Any(predicate))
                {
                    var groups = db.ProjectCompetitionGroups.Include(g => g.Groups).Where(predicate).ToList();

                    if (groups.Count() == 0)
                        projectGroupsNotFound = true;

                    projectGroups.AddRange(groups);
                }
            }
            return projectGroupsNotFound ? null : projectGroups;
        }

        private Contract CreateCompanyAndContract(ProjectApiDto project)
        {
            Company company = null;
            if (project.organization.urfu)
            {
                company = db.Companies.FirstOrDefault(c => c.Name.Contains("УрФУ") && c.Source == Source.Project);
            }
            if (company == null)
            {
                company = new Company();
                company.Email = project.customer.email;
                company.PhoneNumber = project.customer.phone;
                company.Name = project.organization.name;
                company.ShortName = project.organization.name;
                company.Source = Source.Project;
                company.PostOfDirector = project.customer.position;
                company.Director = project.customer.name;

                db.Companies.Add(company);
                db.SaveChanges();
            }

            Contract contract = new Contract();
            contract.Director = project.customer.name;
            contract.PostOfDirector = project.customer.position;
            contract.Email = project.customer.email;
            contract.PhoneNumber = project.customer.phone;
            contract.CompanyId = company.Id;
            contract.Division = project.customer.division;

            db.Contracts.Add(contract);
            db.SaveChanges();

            return contract;
        }

        private Project CreateModuleAndProject(ProjectApiDto project, int contractId)
        {
            Module module = new Module();

            module.uuid = GuidHelper.GetGuid();
            module.priority = 1;
            module.Source = Source.Project;
            module.Level = project.level;
            module.title = project.title;
            module.shortTitle = !string.IsNullOrWhiteSpace(project.shortTitle)? project.shortTitle: project.title;
            module.type = "Проектное обучение";
            module.file = project.file?.url?.ToString();
            module.testUnits = GetTestUnits(project.level);

            db.Modules.Add(module);
            db.SaveChanges();

            // создаем проект
            Project projectDB = new Project();

            projectDB.ContractId = contractId;
            projectDB.ModuleId = module.uuid;
            projectDB.EmployersId = project.id;
            projectDB.ModuleTechId = 1;
            projectDB.ShowInLC = false;
            projectDB.WithoutPriorities = false;
            projectDB.Summary = project.summary;
            projectDB.Description = project.description;
            projectDB.Target = project.target;

            db.Projects.Add(projectDB);
            db.SaveChanges();
            
            project.roles = project.roles ?? new List<ProjectRoleApiDto>();
            foreach (var roleDto in project.roles)
            {
                var mc = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<AutoMapperConfig>();
                });
                var mapper = new Mapper(mc);
                var projectRole = mapper.Map<ProjectRole>(roleDto);
                projectRole.ProjectId = projectDB.ModuleId;
                db.ProjectRoles.Add(projectRole);
            }
            db.SaveChanges();

            return projectDB;
        }

        private List<ProjectPeriod> CreatePeriodsAndLimits(ProjectApiDto project, Contract contract, Project projectDB)
        {
            var groupLimits = project.limits.GroupBy(l => new { l.semesterId, l.year });
            var profileIds = project.limits.Select(l => l.profileId).Distinct();

            foreach (var groupLimit in groupLimits)
            {
                var period = new ContractPeriod()
                {
                    ContractId = contract.Id,
                    SemesterId = groupLimit.Key.semesterId,
                    Year = groupLimit.Key.year
                };
                db.ContractPeriods.Add(period);
                db.SaveChanges();

                db.ProjectPeriods.Add(new ProjectPeriod()
                {
                    ProjectId = projectDB.ModuleId,
                    SemesterId = period.SemesterId,
                    Year = period.Year
                });

                foreach (var l in groupLimit)
                {
                    var profile = db.Profiles.FirstOrDefault(p => p.ID == l.profileId);
                    var limit = new ContractLimit
                    {
                        ContractPeriodId = period.Id,
                        Course = l.course,
                        Profile = profile,
                        DirectionId = profile.DIRECTION_ID,
                        Limit = l.limit
                    };
                    db.ContractLimits.Add(limit);
                    db.SaveChanges();
                }
            }
            db.SaveChanges();

            var oksoListStr = string.Join(", ", db.Profiles.Where(p => profileIds.Contains(p.ID)).Select(p => p.Direction.okso).Distinct());
            projectDB.Module.specialities = oksoListStr;
            db.SaveChanges();

            var projectPeriods = db.ProjectPeriods.Where(p => p.ProjectId == projectDB.ModuleId).ToList();
            return projectPeriods;
        }

        private ProjectDiscipline CreateProjectDiscipline(ProjectApiDto project, Project projectDB)
        {
            // создаем дисциплину
            Discipline discipline = new Discipline()
            {
                uid = "prjits" + GuidHelper.GetGuid(),
                title = project.title,
                pkey = GuidHelper.GetGuid(),
                testUnits = GetTestUnits(project.level)
            };
            db.Disciplines.Add(discipline);
            db.SaveChanges();

            // связка дисциплины и проекта
            ProjectDiscipline projectDiscipline = new ProjectDiscipline()
            {
                DisciplineUid = discipline.uid,
                ProjectId = projectDB.ModuleId
            };
            db.ProjectDisciplines.Add(projectDiscipline);
            db.SaveChanges();

            return projectDiscipline;
        }

        private List<ProjectProperty> CreateProjectProperties(List<ProjectCompetitionGroup> projectGroups, Project projectDB)
        {
            // свойства проекта - связь проектной группы и проекта
            var properties = new List<ProjectProperty>();
            foreach (var group in projectGroups)
            {
                properties.Add(new ProjectProperty
                {
                    ProjectCompetitionGroupId = group.Id,
                    ProjectId = projectDB.ModuleId
                });
            }
            db.ProjectProperties.AddRange(properties);
            db.SaveChanges();

            return properties;
        }

        private void CreateUsers(ProjectApiDto project, List<ProjectProperty> properties, Project projectDB)
        {
            // создаем кураторов
            List<ProjectUser> curators = new List<ProjectUser>();
            foreach (var property in properties)
            {
                foreach (var t in project.teachers)
                {
                    ProjectUser curator = new ProjectUser()
                    {
                        ProjectPropertyId = property.Id,
                        TeacherId = t,
                        Type = ProjectUserType.Curator,
                        IsChief = false
                    };
                    curators.Add(curator);
                }
            }
            db.ProjectUsers.AddRange(curators);
            db.SaveChanges();

            // создаем РОПа

            if (project.programManager == null)
                throw new Exception("Не найден пользователь с правами РОПа");

            // проверка на то, что у пользователя есть права ропа
            var ropRoles = db.RoleSets.FirstOrDefault(rs => rs.Id == 1).Contents.Select(c => c.RoleId);
            var teacher = db.Teachers.FirstOrDefault(t => t.pkey == project.programManager.runpId && ropRoles.All(r => t.User.Roles.Select(ur => ur.RoleId).Contains(r)));

            if (teacher == null) // если пользователь не имеет прав ропа, считается, что у проекта ропа нет 
                throw new Exception("Не найден пользователь с правами РОПа");

            if (teacher.UserId == null)
            {
                teacher.User = db.Users.FirstOrDefault(u => u.AdName == project.programManager.userPrincipalName);
            }
            ProjectUser rop = new ProjectUser()
            {
                IsChief = true,
                ProjectId = projectDB.ModuleId,
                TeacherId = teacher.pkey,
                Type = ProjectUserType.ROP
            };
            db.ProjectUsers.Add(rop);
            db.SaveChanges();

            // добавление профилей РОПа
            // добавляем только те профили, которые есть у нас в базе
            var profiles = db.Profiles.Include(p => p.Division).Where(p => project.programManagerProfileIds.Contains(p.ID)).ToList();
            var ropProfiles = new List<ProjectROPProfile>();

            // вытаскиваем кафедру для ProjectDisciplineTmerPeriod
            chair = profiles.FirstOrDefault(p => p.QUALIFICATION.ToLower().Contains("бакалавр"))?.Division ?? profiles.FirstOrDefault()?.Division;

            foreach (var profile in profiles)
            {
                ropProfiles.Add(new ProjectROPProfile()
                {
                    ProfileId = profile.ID,
                    ProjectUserId = rop.Id
                });
            }
            db.ProjectROPProfiles.AddRange(ropProfiles);
            db.SaveChanges();
        }

        private List<ProjectDisciplineTmerPeriod> CreateProjectTmersPeriods(ProjectDiscipline projectDiscipline, List<ProjectPeriod> projectPeriods)
        {
            // добавление нагрузки
            // Лекции
            var tmerLeks = db.ProjectTmers.FirstOrDefault(t => t.ProjectDisciplineId == projectDiscipline.Id && t.TmerId == tlekc);
            if (tmerLeks == null)
            {
                tmerLeks = new ProjectDisciplineTmer()
                {
                    ProjectDisciplineId = projectDiscipline.Id,
                    TmerId = tlekc
                };
                db.ProjectTmers.Add(tmerLeks);
            }
            // Экзамен
            var tmerExam = db.ProjectTmers.FirstOrDefault(t => t.ProjectDisciplineId == projectDiscipline.Id && t.TmerId == prex);
            if (tmerExam == null)
            {
                tmerExam = new ProjectDisciplineTmer()
                {
                    ProjectDisciplineId = projectDiscipline.Id,
                    TmerId = prex
                };
                db.ProjectTmers.Add(tmerExam);
            }
            db.SaveChanges();

            // распределение нагрузок по периодам
            List<ProjectDisciplineTmerPeriod> tmerPeriods = new List<ProjectDisciplineTmerPeriod>();
            foreach (var period in projectPeriods)
            {
                var tmerPeriodLeks = CreateProjectDisciplineTmerPeriod(tmerLeks, period);
                tmerPeriods.Add(tmerPeriodLeks);

                var tmerPeriodExam = CreateProjectDisciplineTmerPeriod(tmerExam, period);
                tmerPeriods.Add(tmerPeriodExam);
            }
            db.ProjectTmerPeriods.AddRange(tmerPeriods);
            db.SaveChanges();

            return tmerPeriods;
        }

        private ProjectDisciplineTmerPeriod CreateProjectDisciplineTmerPeriod(ProjectDisciplineTmer tmer, ProjectPeriod period)
        {
            var tmerPeriod = new ProjectDisciplineTmerPeriod()
            {
                ProjectDisciplineTmerId = tmer.Id,
                ProjectPeriodId = period.Id
            };
            tmerPeriod.Divisions = new List<Division>() { chair };
            return tmerPeriod;
        }

        private void CreateProjectSubgroupCount(int subgroupCount, List<ProjectCompetitionGroup> projectGroups, List<ProjectDisciplineTmerPeriod> projectTmerPeriods)
        {
            List<ProjectSubgroupCount> projectSubgroupCount = new List<ProjectSubgroupCount>();

            foreach (var pg in projectGroups)
            {
                foreach (var period in projectTmerPeriods)
                {
                    projectSubgroupCount.Add(new ProjectSubgroupCount()
                    {
                        CompetitionGroupId = pg.Id,
                        ProjectDisciplineTmerPeriodId = period.Id,
                        GroupCount = subgroupCount
                    });
                }
            }

            db.ProjectSubgroupCounts.AddRange(projectSubgroupCount);
            db.SaveChanges();
        }

        private void UpdateCompanyAndContract(ProjectApiDto project, Project projectDB)
        {
            Company company = projectDB.Contract?.Company;
            if (!project.organization.urfu && company != null) // если это предприятие УрФУ, то информацию о нем НЕ меняем
            {
                company.Email = project.customer.email;
                company.PhoneNumber = project.customer.phone;
                company.Name = project.organization.name;
                company.ShortName = project.organization.name;
                company.PostOfDirector = project.customer.position;
                company.Director = project.customer.name;
            }

            Contract contract = projectDB.Contract;
            if (contract != null)
            {
                contract.Director = project.customer.name;
                contract.PostOfDirector = project.customer.position;
                contract.Email = project.customer.email;
                contract.PhoneNumber = project.customer.phone;
                contract.Division = project.customer.division;
            }

            db.SaveChanges();
        }

        private void UpdateModuleAndProject(ProjectApiDto project, Project projectDB)
        {
            var module = projectDB.Module;
            if (module != null)
            {
                module.Level = project.level;
                module.title = project.title;
                module.shortTitle = !string.IsNullOrWhiteSpace(project.shortTitle) ? project.shortTitle : project.title;
                module.file = project.file?.url?.ToString();
                module.testUnits = GetTestUnits(project.level);
            }

            projectDB.Summary = project.summary;
            projectDB.Description = project.description;
            projectDB.Target = project.target;

            project.roles = project.roles ?? new List<ProjectRoleApiDto>();
            var existRolesIds = projectDB.Roles.Select(r => r.Id).ToList();
            foreach (var roleDto in project.roles)
            {
                var role = projectDB.Roles.FirstOrDefault(r => r.EmployersId == roleDto.id);
                if (role == null)
                {
                    // добавляем новую роль
                    var mc = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<AutoMapperConfig>();
                    });
                    var mapper = new Mapper(mc);
                    var projectRole = mapper.Map<ProjectRole>(roleDto);
                    projectRole.ProjectId = projectDB.ModuleId;
                    db.ProjectRoles.Add(projectRole);
                }
                else
                {
                    // обновляем существующую
                    role.Description = roleDto.description;
                    role.Title = roleDto.title;
                    existRolesIds.Remove(role.Id);
                }
                db.SaveChanges();
            }
            db.ProjectRoles.RemoveRange(db.ProjectRoles.Where(r => existRolesIds.Contains(r.Id)));
            db.SaveChanges();
        }

        private void UpdatePeriodsAndLimits(ProjectApiDto project, Project projectDB)
        {
            var groupLimits = project.limits.GroupBy(l => new { l.semesterId, l.year });

            var profileIds = project.limits.Select(l => l.profileId).Distinct();
            var oksoListStr = string.Join(", ", db.Profiles.Where(p => profileIds.Contains(p.ID)).Select(p => p.Direction.okso).Distinct());
            projectDB.Module.specialities = oksoListStr;
            db.SaveChanges();


            var existPeriods = db.ContractPeriods.Include("Limits").Where(p => p.ContractId == projectDB.ContractId).ToList();
            var existProjectPeriods = db.ProjectPeriods.Where(p => p.ProjectId == projectDB.ModuleId).ToList();
            var existLimits = db.ContractLimits.Where(l => l.Period.ContractId == projectDB.ContractId).ToList();

            foreach (var groupLimit in groupLimits)
            {
                var period = existPeriods.FirstOrDefault(p => p.SemesterId == groupLimit.Key.semesterId && p.Year == groupLimit.Key.year);
                if (period == null)
                {
                    period = new ContractPeriod()
                    {
                        ContractId = projectDB.ContractId.Value,
                        SemesterId = groupLimit.Key.semesterId,
                        Year = groupLimit.Key.year
                    };
                    db.ContractPeriods.Add(period);
                    db.SaveChanges();
                }
                else
                {
                    existPeriods.Remove(period);
                }

                var projectPeriod = existProjectPeriods.FirstOrDefault(p => p.SemesterId == period.SemesterId && p.Year == period.Year);
                if (projectPeriod == null)
                {
                    db.ProjectPeriods.Add(new ProjectPeriod()
                    {
                        ProjectId = projectDB.ModuleId,
                        SemesterId = period.SemesterId,
                        Year = period.Year
                    });
                }
                else
                {
                    existProjectPeriods.Remove(projectPeriod);
                }

                foreach (var l in groupLimit)
                {
                    var limit = existLimits.FirstOrDefault(lim => lim.ContractPeriodId == period.Id && lim.ProfileId == l.profileId && lim.Course == l.course);

                    if (limit == null)
                    {
                        var profile = db.Profiles.FirstOrDefault(p => p.ID == l.profileId);
                        limit = new ContractLimit
                        {
                            ContractPeriodId = period.Id,
                            Course = l.course,
                            Profile = profile,
                            DirectionId = profile.DIRECTION_ID
                        };
                        db.ContractLimits.Add(limit);
                    }
                    else
                    {
                        existLimits.Remove(limit);
                    }

                    limit.Limit = l.limit;
                    db.SaveChanges();
                }
            }

            // удалить оставшиеся периоды и лимиты
            db.ContractPeriods.RemoveRange(existPeriods);
            db.ContractLimits.RemoveRange(existLimits);
            db.ProjectPeriods.RemoveRange(existProjectPeriods);

            db.SaveChanges();
        }

        private void UpdateProjectDiscipline(ProjectApiDto project, Project projectDB)
        {
            try
            {
                var projectDiscipline = db.ProjectDisciplines.FirstOrDefault(p => p.ProjectId == projectDB.ModuleId)?.Discipline;
                projectDiscipline.title = project.title;
                projectDiscipline.testUnits = GetTestUnits(project.level);
                db.SaveChanges();
            }
            catch { }
        }

        private void UpdateProjectProperties(List<ProjectCompetitionGroup> projectGroups, Project projectDB)
        {
            var properties = new List<ProjectProperty>();
            var existProperties = db.ProjectProperties.Where(p => p.ProjectId == projectDB.ModuleId).ToList();
            foreach (var group in projectGroups)
            {
                var prop = existProperties.FirstOrDefault(p => p.ProjectCompetitionGroupId == group.Id);
                if (prop == null)
                {
                    properties.Add(new ProjectProperty
                    {
                        ProjectCompetitionGroupId = group.Id,
                        ProjectId = projectDB.ModuleId
                    });
                }
                else
                    existProperties.Remove(prop);
            }
            db.ProjectProperties.AddRange(properties);
            db.ProjectProperties.RemoveRange(existProperties);
            db.SaveChanges();
        }

        private void UpdateUsers(ProjectApiDto project, Project projectDB)
        {
            var properties = db.ProjectProperties.Where(p => p.ProjectId == projectDB.ModuleId).ToList();

            // обновляем кураторов
            List<ProjectUser> users = new List<ProjectUser>();
            var exist_curators = db.ProjectUsers.Where(u => u.Type == ProjectUserType.Curator && u.ProjectProperty.ProjectId == projectDB.ModuleId).ToList();
            foreach (var property in properties)
            {
                foreach (var t in project.teachers)
                {
                    var curator = exist_curators.FirstOrDefault(c => c.ProjectPropertyId == property.Id && c.TeacherId == t);
                    if (curator == null)
                    {
                        curator = new ProjectUser()
                        {
                            ProjectPropertyId = property.Id,
                            TeacherId = t,
                            Type = ProjectUserType.Curator,
                            IsChief = false
                        };
                        users.Add(curator);
                    }
                    else
                    {
                        exist_curators.Remove(curator);
                    }
                }
            }
            db.ProjectUsers.AddRange(users);
            db.ProjectUsers.RemoveRange(exist_curators);
            db.SaveChanges();

            // обновляем РОПа
            var exist_rops = db.ProjectUsers.Where(u => u.Type == ProjectUserType.ROP && u.ProjectId == projectDB.ModuleId).ToList();
            var exist_chiefRop = exist_rops.FirstOrDefault(r => r.IsChief);

            if (project.programManager == null && exist_rops.Count == 0)
                return;

            var ropRoles = db.RoleSets.FirstOrDefault(rs => rs.Id == 1).Contents.Select(c => c.RoleId);
            var teacher = db.Teachers.FirstOrDefault(t => t.pkey == project.programManager.runpId && ropRoles.All(r => t.User.Roles.Select(ur => ur.RoleId).Contains(r)));

            var rop = exist_rops.FirstOrDefault(r => r.TeacherId == teacher.pkey);
            if (rop != null)
                exist_rops.Remove(rop);
            if (rop == null && teacher != null)
            {
                if (teacher.UserId == null)
                {
                    teacher.User = db.Users.FirstOrDefault(u => u.Id == project.programManager.userPrincipalName);
                }
                rop = new ProjectUser()
                {
                    IsChief = true,
                    ProjectId = projectDB.ModuleId,
                    TeacherId = teacher.pkey,
                    Type = ProjectUserType.ROP
                };
                db.ProjectUsers.Add(rop);
            }
            db.ProjectUsers.RemoveRange(exist_rops);
            db.SaveChanges();

            if (rop == null)
                return;

            // обновление профилей РОПа
            // добавляем только те профили, которые есть у нас в базе
            var profilesIDs = db.Profiles.Where(p => project.programManagerProfileIds.Contains(p.ID)).Select(p => p.ID).ToList();
            var ropProfiles = new List<ProjectROPProfile>();
            var exist_ropProfiles = db.ProjectROPProfiles.Where(p => p.ProjectUserId == rop.Id).ToList();
            foreach (var profile in profilesIDs)
            {
                var e_profile = exist_ropProfiles.FirstOrDefault(r => r.ProfileId == profile);
                if (e_profile == null)
                {
                    ropProfiles.Add(new ProjectROPProfile()
                    {
                        ProfileId = profile,
                        ProjectUserId = rop.Id
                    });
                }
                else
                {
                    exist_ropProfiles.Remove(e_profile);
                }
            }
            db.ProjectROPProfiles.AddRange(ropProfiles);
            db.ProjectROPProfiles.RemoveRange(exist_ropProfiles);
            db.SaveChanges();
        }

        private void UpdateProjectTmersPeriods(Project projectDB)
        {
            // распределение нагрузок по периодам

            var projectDiscipline = db.ProjectDisciplines.FirstOrDefault(d => d.ProjectId == projectDB.ModuleId);
            var projectPeriods = db.ProjectPeriods.Where(p => p.ProjectId == projectDB.ModuleId).ToList();

            var tmerLeks = db.ProjectTmers.FirstOrDefault(t => t.ProjectDisciplineId == projectDiscipline.Id && t.TmerId == tlekc);
            var tmerExam = db.ProjectTmers.FirstOrDefault(t => t.ProjectDisciplineId == projectDiscipline.Id && t.TmerId == prex);

            List<ProjectDisciplineTmerPeriod> tmerPeriods = new List<ProjectDisciplineTmerPeriod>();
            var exist_tmerPeriods = db.ProjectTmerPeriods.Where(p => p.Period.ProjectId == projectDB.ModuleId).ToList();
            foreach (var period in projectPeriods)
            {
                var periodLeks = exist_tmerPeriods.FirstOrDefault(p => p.ProjectPeriodId == period.Id && p.Tmer.TmerId == tlekc);
                if (periodLeks == null)
                {
                    tmerPeriods.Add(new ProjectDisciplineTmerPeriod()
                    {
                        ProjectDisciplineTmerId = tmerLeks.Id,
                        ProjectPeriodId = period.Id
                    });
                }
                else
                {
                    exist_tmerPeriods.Remove(periodLeks);
                }

                var periodExam = exist_tmerPeriods.FirstOrDefault(p => p.ProjectPeriodId == period.Id && p.Tmer.TmerId == prex);
                if (periodExam == null)
                {
                    tmerPeriods.Add(new ProjectDisciplineTmerPeriod()
                    {
                        ProjectDisciplineTmerId = tmerExam.Id,
                        ProjectPeriodId = period.Id
                    });
                }
                else
                {
                    exist_tmerPeriods.Remove(periodExam);
                }
            }
            db.ProjectTmerPeriods.AddRange(tmerPeriods);
            db.ProjectTmerPeriods.RemoveRange(exist_tmerPeriods);
            db.SaveChanges();
        }

        private void UpdateProjectSubgroupCount(int subgroupCount, List<ProjectCompetitionGroup> projectGroups, Project projectDB)
        {
            List<ProjectSubgroupCount> projectSubgroupCount = new List<ProjectSubgroupCount>();
            var projectTmerPeriods = db.ProjectTmerPeriods.Where(p => p.Period.ProjectId == projectDB.ModuleId).ToList();
            var projectTmerPeriodIds = projectTmerPeriods.Select(t => t.Id);

            var exist_subgroupCount = db.ProjectSubgroupCounts.Where(s => projectTmerPeriodIds.Contains(s.ProjectDisciplineTmerPeriodId)).ToList();
            foreach (var pg in projectGroups)
            {
                foreach (var period in projectTmerPeriods)
                {
                    var e_subgroupCount = exist_subgroupCount.FirstOrDefault(s => s.ProjectDisciplineTmerPeriodId == period.Id && s.CompetitionGroupId == pg.Id);
                    if (e_subgroupCount == null)
                    {
                        projectSubgroupCount.Add(new ProjectSubgroupCount()
                        {
                            CompetitionGroupId = pg.Id,
                            ProjectDisciplineTmerPeriodId = period.Id,
                            GroupCount = subgroupCount
                        });
                    }
                    else
                    {
                        exist_subgroupCount.Remove(e_subgroupCount);
                        e_subgroupCount.GroupCount = subgroupCount;
                    }
                }
            }

            db.ProjectSubgroupCounts.AddRange(projectSubgroupCount);
            db.ProjectSubgroupCounts.RemoveRange(exist_subgroupCount);
            db.SaveChanges();
        }

        private int GetTestUnits(string level)
        {
            return level == "A" || level == "А" ? 3 : 6; // первая А - англ., вторая - рус.
        }
    }
}