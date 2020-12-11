using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Integration;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.Practices;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
    /// <summary>
    /// Контроллер для получения данных из РПП
    /// </summary>
    public class PracticesWorkingProgramController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;
        private readonly IObjectLogger<DisciplineWorkingProgramController> _logger;

        public PracticesWorkingProgramController(ApplicationDbContext db, IVersionedDocumentService documentService, IObjectLogger<DisciplineWorkingProgramController> logger)
        {
            _db = db;
            _documentService = documentService;
            _logger = logger;
        }

        public ActionResult GetPracticeWays()
        {
            var ways = _db.PracticeWays.Select(_=>new
            {
                _.Id,
                Name = _.Description
            }).ToList();
            return Json(ways, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPracticeMethods()
        {
            var list = _db.PracticeTimes.Select(_ => new
            {
                _.Id,
                Name = _.Description
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGroups(int documentId)
        {
            var doc = _db.VersionedDocuments.Find(documentId);
            var docModel = (PracticesWorkingProgramFgosVoSchemaModel)_documentService.CreateModel(doc,
                nameof(PracticesWorkingProgramFgosVoSchemaModel.Fdps),
                nameof(PracticesWorkingProgramFgosVoSchemaModel.Profiles));
            var famTypes = docModel.Fdps.Select(f => f.FamType).ToList();
            var profileIds = docModel.Profiles.Select(p => p.Id).ToList();

            var groups = _db.GroupsHistories.Where(g => famTypes.Contains(g.FamType) && profileIds.Contains(g.ProfileId))
                .Select(g => new
                {
                    Id = g.GroupId,
                    g.Name
                }).OrderBy(g => g.Name);
            //  .ToListAsync();
            return Json(groups, JsonRequestBehavior.AllowGet);
        }

        [ErrorFilter]
        public async Task<ActionResult> GetTechCard(int documentId, string year, string semester, string disciplineName, string groupId)
        {
            var techCardService = new TechCardService();
            var techCardDto = await Task.Run(() => techCardService.GetTechCards(year, semester, disciplineName, groupId).FirstOrDefault());
            if (techCardDto == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var loads = techCardDto.loads.FirstOrDefault();
            
            var courseWorkCard = techCardDto.loads.FirstOrDefault(l =>
                new[] { "betweenCourseProject", "courseProject", "courseWork" }.Contains(l.technologyCardType));

            var practiceCertification = new TechCardPracticeCertificationInfo
            {
                Year = year,
                Semester = semester,
                DisciplineName = disciplineName,
                GroupId = groupId,
                EduLoad = PrepareCertificationItem(loads)
            };
            var courseWorksCertification = PrepareCertificationItem(courseWorkCard);

            return Json(new
            {
                TechCardDisciplineCertification = practiceCertification,
                TechCardSemesterSignificanceCoefficients = new List<TechCardSemesterSignificanceCoefficient>
                {
                    new TechCardSemesterSignificanceCoefficient
                    {
                        SemesterNumber = techCardDto.term,
                        Coefficient = techCardDto.termRatio
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        private static TechCardCertificationItemInfo PrepareCertificationItem(TechCardLoadDto labCard)
        {
            return new TechCardCertificationItemInfo
            {
                TotalCoefficient = labCard?.totalFactor?.ToString() ?? TechCardCertificationItemInfo.CoefficientNotProvidedText,
                CurrentCoefficient = labCard?.currentFactor?.ToString() ?? TechCardCertificationItemInfo.CoefficientNotProvidedText,
                IntermediateCoefficient = labCard?.intermediateFactor?.ToString() ?? TechCardCertificationItemInfo.CoefficientNotProvidedText,
                IntermediateCertification =
                    labCard?.controls?.FirstOrDefault(c => c.intermediate == 1)?.controlAction ??
                    TechCardCertificationItemInfo.IntermediateCertificationNotProvidedText,
                Controls = labCard?.controls?.Where(c => c.intermediate == 0).Select(c => new TechCardControlItemInfo
                {
                    Name = c.controlAction,
                    MaxPoints = c.maxValue,
                }).ToList() ?? new List<TechCardControlItemInfo>()
            };
        }
    }
}