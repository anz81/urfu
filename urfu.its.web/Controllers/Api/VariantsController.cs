
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using System.Web.SessionState;
using Microsoft.AspNetCore.Session;
using AutoMapper;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
// ReSharper disable InconsistentNaming

namespace Urfu.Its.Web.Controllers
{
    
  //  [SessionState(SessionStateBehavior.Disabled)]
    public class VariantsController : BaseController
    {
        public List<VariantApiDto> Get()
        {
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();

            using (var db = new ApplicationDbContext())
            {
                var variants = VarinatApiHelper.VariantsQueryForApi(db).ToList();
                var dtos = variants.Select(mapper.Map<VariantApiDto>).ToList();
                EduProgram.FillTeachers(db, dtos);
                return dtos; 
            }
        }

        public List<VariantApiDto> Get(string okso)
        {
            var me = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = me.CreateMapper();

            using (var db = new ApplicationDbContext())
            {
                List<Variant> variants = VarinatApiHelper.VariantsQueryForApi(db).Where(v => v.Program.Direction.okso == okso).ToList();
                var dtos = variants.Select(mapper.Map<VariantApiDto>).ToList();
                EduProgram.FillTeachers(db, dtos);
                return dtos;
            }
        }
    }

  //  [SessionState(SessionStateBehavior.Disabled)]
    public class VariantNamesController : BaseController
    {
        public object Get(string okso, string directionId)
        {
            return Enumerable.Empty<Variant>().ToList();
            //using (var db = new ApplicationDbContext())
            //{
            //    List<Variant> variants;
            //    if (okso != null)
            //        variants = db.Variants.Where(v => v.Program.Direction.okso == okso).ToList();
            //    else if (directionId != null)
            //        variants = db.Variants.Where(v => v.Program.directionId == directionId).ToList();
            //    else
            //        variants = db.Variants.ToList();

            //    return variants.Select(v => new { variantName = v.Name, id = v.Id }).ToList();
            //}
        }
    }

    public class VarinatApiHelper
    {
        /// <summary>
        /// Запрос, загружающий все необходимые пререквизиты для экспорта варианта в ЛК
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IQueryable<Variant> VariantsQueryForApi(ApplicationDbContext db)
        {
            return db.Variants
                .Include(v => v.Program.Direction.Modules.Select(m => m.Plans))
                .Include(v => v.Groups.Select(g => g.Contents.Select(c => c.Module.disciplines)))
                .Include(v => v.Groups.Select(g => g.Contents.Select(c => c.ModuleType)))
                .Include(v=>v.SelectionGroups)
                .Include(v=>v.Program.Variant);
        }

        /// <summary>
        /// Запрос, загружающий все необходимые пререквизиты для экспорта варианта в ЛК
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IQueryable<Variant> VariantsQueryForEdit(ApplicationDbContext db)
        {
            return db.Variants
                .Include(v => v.Program.Direction.Modules.Select(m => m.Plans))
                .Include(v => v.Groups.Select(g => g.Contents.Select(c=>c.ModuleType)))
                //.Include(v => v.Groups.Select(g => g.Contents.Select(c => c.Module.Plans)))
                .Include(v=>v.SelectionGroups)
                .Include(v=>v.Program.Variant);
        }

        /// <summary>
        /// Запрос, загружающий все необходимые пререквизиты для экспорта программы в ЛК
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IQueryable<EduProgram> ProgramsQueryForApi(ApplicationDbContext db)
        {
            return db.EduPrograms
                .Include(p => p.Variants.Select(v=>v.Program.Direction.Modules.Select(m => m.Plans)))
                .Include(p => p.Variants.Select(v=>v.Groups.Select(g => g.Contents.Select(c => c.Module.disciplines))))
                .Include(p => p.Variants.Select(v=>v.Groups.Select(g => g.Contents.Select(c => c.ModuleType))))
                .Include(p=>p.Variants.Select(v=>v.SelectionGroups))
                .Include(p=>p.Variants.Select(v=>v.Program.Variant));
        }
    }
}
