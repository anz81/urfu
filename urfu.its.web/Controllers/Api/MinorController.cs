using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Urfu.Its.Common;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [IdentityBasicAuthentication]
    public class MinorController : BaseController
    {
        public object Get(string okso, int year, int semester)
        {
            using (var db = new ApplicationDbContext())
            {
                List<Module> minors = db.UniModules()
                    .Include(m=>m.Minor.Periods)
                    .Include(m=>m.Minor.Disciplines)
                    //.Include(m=>m.Minor.Requirment)
                    .Where(m =>
                               m.type.Contains("Майнор")
                            && m.specialities.Contains(okso)
                            && m.Minor.ShowInLC == true
                            && m.Minor.Periods.Any(p => p.Year == year && p.SemesterId == semester)
                    )
                    .OrderBy(m=>m.title)
                    .ToList();
                var me = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<AutoMapperConfig>();
                });
                var mapper = me.CreateMapper();
                var dtos = minors.Select(mapper.Map<MinorApiDto>).ToList();
                foreach (var m in dtos)
                {
                    m.period = mapper.Map<PeriodApiDto>(minors.First(f => f.uuid == m.uuid).Minor.Periods.First(p => p.Year == year && p.SemesterId == semester));
                    m.file = ChangeExtension(m.file);
                    
                    if (m.disciplines?.Count() > 0)
                    {
                        foreach (var d in m.disciplines)
                        {
                            d.file = ChangeExtension(d.file);
                        }
                        var disciplineUUID = m.disciplines[0].uid;
                        var agreement = mapper.Map<ModuleAgreementApiDto>(db.ModuleAgreements.FirstOrDefault(a => a.EduYear == year && a.SemesterId == semester
                            && a.ModuleUUID == m.uuid && disciplineUUID.Contains(a.DisciplineUUID)));

                        m.agreement = agreement;
                    }
                }
                return dtos;
            }
        }

        private string ChangeExtension(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file)) return file;

                return Path.ChangeExtension(file, "docx");
            }
            catch
            {
                return file;
            }
        }
        
        public object Post(StudentSelectionMinorDto selections)
        {
            Logger.Info("Запрос api выбора майноров студентов");
            return WriteStudentMinorSelections(selections);
        }

        public object GetSelection(string student)
        {
            using (var db = new ApplicationDbContext())
            {
                var selections = db.StudentSelectionMinorPriority
                    .Where(s=>s.studentId==student).ToList();

                var admissions = db.MinorAdmissions
                    .Where(a => a.studentId == student && a.Published).ToList();

                var request = new List<StudentSelectionMinorPeriodDto>();

                AddAdmissions(request, admissions);
                AddSelections(request, selections);

                var dto = new StudentSelectionMinorDto
                {
                    student = student,
                    request = request
                };

                return dto;
            }
        }

        private object WriteStudentMinorSelections(StudentSelectionMinorDto selections)
        {
            try
            {
                var affected = SyncEngine.WriteStudentMinorSelectionsToDatabase(selections);
                return new { success = true };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new { success = false, error = ex.ToString() };
            }

        }

        private void AddAdmissions(List<StudentSelectionMinorPeriodDto> request, List<MinorAdmission> admissions)
        {
            foreach (var a in admissions)
            {
                var year = a.MinorPeriod.Year;
                var semester = a.MinorPeriod.SemesterId;

                var p = GetMinorPeriodDto(request, a.MinorPeriod);


                p.minors.Add(new StudentSelectionMinorPriorityDto
                {
                    id = a.MinorPeriod.ModuleId,
                    prio = 0, //может быть зачислен без выбора студента
                    title = a.MinorPeriod.Minor.Module.title,
                    requirment = a.MinorPeriod.Minor.Requirments.FirstOrDefault()?.title,
                    deadline = a.MinorPeriod.SelectionDeadline,
                    admission = EnumHelper<AdmissionStatus>.GetDisplayValue(a.Status),
                    admissionid = (int)a.Status
                });
            }
        }

        private void AddSelections(List<StudentSelectionMinorPeriodDto> request, List<StudentSelectionMinorPriority> selections)
        {
            foreach (var s in selections)
            {
                var p = GetMinorPeriodDto(request, s.MinorPeriod);

                var a = p.minors.FirstOrDefault(m => m.id == s.MinorPeriod.ModuleId);
                if (a == null)
                {
                    p.minors.Add(new StudentSelectionMinorPriorityDto
                    {
                        id = s.MinorPeriod.ModuleId,
                        prio = s.priority,
                        title = s.MinorPeriod.Minor.Module.title,
                        requirment = s.MinorPeriod.Minor.Requirments.FirstOrDefault()?.title,
                        deadline = s.MinorPeriod.SelectionDeadline,
                        admission = EnumHelper<AdmissionStatus>.GetDisplayValue(AdmissionStatus.Indeterminate),
                        admissionid = (int)AdmissionStatus.Indeterminate
                    });
                }
                else
                {
                    a.prio = s.priority;
                }
            }
        }

        private StudentSelectionMinorPeriodDto GetMinorPeriodDto(List<StudentSelectionMinorPeriodDto> request, MinorPeriod period)
        {
            var p = request.FirstOrDefault(r => r.year == period.Year && r.semester == period.SemesterId);
            if (p == null)
            {
                p = new StudentSelectionMinorPeriodDto { year = period.Year, semester = period.SemesterId, minors = new List<StudentSelectionMinorPriorityDto>() };
                request.Add(p);
            }

            return p; 
        }

    }
}