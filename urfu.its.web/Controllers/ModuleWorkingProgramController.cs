using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.WorkingProgramView)]
    public class ModuleWorkingProgramController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;
        private readonly IObjectLogger<ModuleWorkingProgramController> _logger;

        public ModuleWorkingProgramController(ApplicationDbContext db, IVersionedDocumentService documentService, IObjectLogger<ModuleWorkingProgramController> logger)
        {
            _db = db;
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<ActionResult> GetUniversalCompetences(int documentId, string profileId)
        {
            return await Task.FromResult(new EmptyResult());
        }

        public async Task<ActionResult> GetEduResults(int documentId, string profileId)
        {
            var eduResults = await _db.EduResults.Where(r => r.ProfileId == profileId && !r.IsDeleted)
                .Select(r=>new EduResultCompetencesInfo
                {
                    Id = r.Id,
                    Name = "РО-" + r.CodeNumber,
                    CodeNumber = r.CodeNumber,
                    Description = r.Description,
                    ProfileId = r.ProfileId                    
                }).ToListAsync();
            return Json(eduResults, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetCompetences(int documentId, string profileId)
        {
            var document = _db.VersionedDocuments.Find(documentId);

            dynamic modelDirections = _documentService.CreateProxyModel(document, nameof(ModuleWorkingProgramFgosVoSchemaModel.Directions));
            var directionIds = new List<string>();
            foreach (var d in modelDirections.Directions)
                directionIds.Add(d.Id);

            var competences = await _db.Competences.Where(c => !c.IsDeleted && directionIds.Contains(c.DirectionId) && (c.ProfileId == null || c.ProfileId == profileId))
                .Select(c => new CompetenceInfo
                {
                    Code = c.Code,
                    Order = c.Order,
                    Id = c.Id,
                    Type = c.Type,
                    DirectionId = c.DirectionId,
                    Content = c.Content
                }).ToListAsync();
            return Json(competences, JsonRequestBehavior.AllowGet);
        }        
    }
}