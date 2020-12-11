using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using System.Web.SessionState;
using AutoMapper;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
    //[SessionState(SessionStateBehavior.Disabled)]
    public class ProgramsController : BaseController
    {
        public List<ProgramApiDto> Get()
        {
            using (var db = new ApplicationDbContext())
            {
                db.Database.SetCommandTimeout(300);
                //((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 300;
                var variants = VarinatApiHelper.ProgramsQueryForApi(db).ToList();
                var dtos = variants.Select(Mapper.Map<ProgramApiDto>).ToList();
                EduProgram.FillTeachers(db, dtos.SelectMany(d=>d.variants).ToList());
                return dtos;
            }
        }

        public List<ProgramApiDto> Get(string okso)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Database.SetCommandTimeout(300);
                //((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 300;
                List<EduProgram> variants = VarinatApiHelper.ProgramsQueryForApi(db).Where(v => v.Direction.okso == okso).ToList();
                var dtos = variants.Select(Mapper.Map<ProgramApiDto>).ToList();
                EduProgram.FillTeachers(db, dtos.SelectMany(d => d.variants).ToList());
                return dtos;
            }
        }

        public List<ProgramApiDto> Get(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Database.SetCommandTimeout(300);
                //((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 300;
                List<EduProgram> variants = VarinatApiHelper.ProgramsQueryForApi(db).Where(v => v.Id == id).ToList();
                var dtos = variants.Select(Mapper.Map<ProgramApiDto>).ToList();
                EduProgram.FillTeachers(db, dtos.SelectMany(d => d.variants).ToList());
                return dtos;
            }
        }
    }
}