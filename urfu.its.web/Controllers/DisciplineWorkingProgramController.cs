using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;
using Urfu.Its.Integration;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize]
    [ControllerErrorLoggerAttribute]
    [Authorize(Roles = ItsRoles.WorkingProgramView)]
    public class DisciplineWorkingProgramController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;
        private readonly IObjectLogger<DisciplineWorkingProgramController> _logger;
        
        public DisciplineWorkingProgramController(ApplicationDbContext db, IVersionedDocumentService documentService, IObjectLogger<DisciplineWorkingProgramController> logger)
        {
            _db = db;
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<ActionResult> GetCompetences(int documentId)
        {
            //var document = await _db.VersionedDocuments.FindAsync(documentId);
            var dwp = await _db.DisciplineWorkingPrograms.FindAsync(documentId);
            var moduleDocument = _db.VersionedDocuments.Find(dwp.ModuleWorkingProgramId);

            var moduleModel = _documentService.CreateModel<ModuleWorkingProgramFgosVoSchemaModel>(moduleDocument, nameof(ModuleWorkingProgramFgosVoSchemaModel.DisciplineCompetences));
            var competenceIds = moduleModel.DisciplineCompetences.SelectMany(r => r.Items)
                .Where(d => d.DisciplineName == dwp.Discipline.title).SelectMany(d => d.CompetenceIds).Distinct()
                .ToList();

            var competences = _db.Competences.Where(c=>competenceIds.Contains(c.Id))
                .Select(c => new CompetenceInfo
                {
                    Code = c.Code,
                    Order = c.Order,
                    Id = c.Id,
                    Type = c.Type,
                    DirectionId = c.DirectionId,
                    Content = c.Content
                }).ToList();
            return Json(competences, new JsonSerializerSettings());
        }

        [ErrorFilter]
        public ActionResult PickUpTimeDistributionSection(int documentId, TimeDistributionSectionInfo section)
        {
            var doc = _db.VersionedDocuments.Find(documentId);
            var docModel = (DisciplineWorkingProgramFgosVoSchemaModel) _documentService.CreateModel(doc, "Sections");
            var sectionItem = docModel.Sections.FirstOrDefault(s => s.ItemId == section.SectionId);
            if (sectionItem == null)
                throw new InvalidOperationException($"В документе с идентификатором '{documentId}' не найден раздел с идентификатором '{section.SectionId}'");

            section.SectionCode = sectionItem.Code;
            section.SectionName = sectionItem.Name;

            section.TotalControlWorkTime = new[]
            {
                section.ControlWorkTime, section.ColloquiumTime
            }.Sum();
            section.TotalOutOfDoreTime = new[]
            {
                section.HomeworkTime, section.GraphicsWorkTime, section.ReferatsTime,
                section.ProjectWorkTime, section.CalcWorkTime, section.CalcGraphicsWorkTime,
                section.ForeignLanguageWorkTime, section.TranslationWorkTime, section.CourseWorkTime,
                section.CourseProjectTime
            }.Sum();
            section.TotalPreparationTime = new[]
            {
                section.PreparationLectionsTime, section.PreparationPracticesTime,
                section.PreparationLabsTime, section.PreparationSeminarsTime
            }.Sum();
            section.TotalHomeworkTime = new[]
            {
                section.TotalControlWorkTime, section.TotalOutOfDoreTime, section.TotalPreparationTime
            }.Sum();
            section.TotalAuditoryTime = new[]
            {
                section.LectionsTime, section.PracticesTime, section.LabsTime
            }.Sum();
            section.TotalTime = new[]
            {
                section.TotalAuditoryTime, section.TotalHomeworkTime
            }.Sum();
            return Json(section, new JsonSerializerSettings());
        }

        public async Task<ActionResult> GetGroups(int documentId)
        {
            var doc = _db.VersionedDocuments.Find(documentId);
            var docModel = (DisciplineWorkingProgramFgosVoSchemaModel)_documentService.CreateModel(doc, 
                nameof(DisciplineWorkingProgramFgosVoSchemaModel.Fdps), 
                nameof(DisciplineWorkingProgramFgosVoSchemaModel.Profiles));
            var famTypes = docModel.Fdps.Select(f => f.FamType).ToList();
            var profileIds = docModel.Profiles.Select(p => p.Id).ToList();

            var groups = await _db.Groups.Where(g => famTypes.Contains(g.FamType) && profileIds.Contains(g.ProfileId))
                .Select(g=>new
                {
                    g.Id,
                    g.Name
                })
                .ToListAsync();
            return Json(groups, new JsonSerializerSettings());
        }

        [ErrorFilter]
        public async Task<ActionResult> GetTechCard(int documentId, string year, string semester, string disciplineName,
            string groupId)
        {            
            var techCardService = new TechCardService();
            var techCardDto = await Task.Run(() => techCardService.GetTechCards(year, semester, disciplineName, groupId).FirstOrDefault());

            if (techCardDto == null)
                throw new Exception("Не найдена тех. карта.");

            var lectionCard = techCardDto.loads.FirstOrDefault(l => l.technologyCardType == "lecture");
            var practiceCard = techCardDto.loads.FirstOrDefault(l => l.technologyCardType == "practice");
            var labCard = techCardDto.loads.FirstOrDefault(l => l.technologyCardType == "laboratory");
            var courseWorkCard = techCardDto.loads.FirstOrDefault(l =>
                new[] {"betweenCourseProject", "courseProject", "courseWork"}.Contains(l.technologyCardType));
            
            var disciplineCertification = new TechCardDisciplineCertificationInfo
            {
                Year = year,
                Semester = semester,
                DisciplineName = disciplineName,
                GroupId = groupId,
                Lections = PrepareCertificationItem(lectionCard),
                Practices = PrepareCertificationItem(practiceCard),
                Labs = PrepareCertificationItem(labCard)
            };
            var courseWorksCertification = PrepareCertificationItem(courseWorkCard);

            return Json(new
            {
                TechCardDisciplineCertification = disciplineCertification,
                TechCardCourseWorksCertification = courseWorksCertification,
                TechCardSemesterSignificanceCoefficients = new List<TechCardSemesterSignificanceCoefficient>
                {
                    new TechCardSemesterSignificanceCoefficient
                    {
                        SemesterNumber = techCardDto.term,
                        Coefficient = techCardDto.termRatio
                    }
                }
            }, new JsonSerializerSettings());            
        }

        [ErrorFilter]
        public ActionResult PrepareTechCardCertificationItem(TechCardCertificationItemInfo item)
        {
            item.InitDefaults();
            return Json(item, new JsonSerializerSettings());
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
                Controls = labCard?.controls?.Where(c=>c.intermediate == 0).Select(c=>new TechCardControlItemInfo
                {
                    Name = c.controlAction,
                    MaxPoints = c.maxValue,                    
                }).ToList() ?? new List<TechCardControlItemInfo>()
            };
        }

        public async Task<ActionResult> GetSoftware(string search)
        {
            var serviceUrl = ConfigurationManager.AppSettings["SoftwareServiceUrl"];
            var login = ConfigurationManager.AppSettings["SoftwareServiceLogin"];
            var password = ConfigurationManager.AppSettings["SoftwareServicePassword"];

            /*var query = HttpUtility.ParseQueryString(string.Empty);
            query["search"] = search;
            var queryString = query.ToString();*/

            using (var client = new HttpClient(new WebRequestHandler
            {
                Credentials = new NetworkCredential(login, password)
            })
            {
                BaseAddress = new Uri(serviceUrl)
            })
            {
                var strData = await client.GetStringAsync("");
                var array = JArray.Parse(strData);
                var items = array.Select(a => new
                {
                    Id = Guid.NewGuid(),
                    Developer = a.Value<string>("Developer"),
                    Class = a.Value<string>("Class"),
                    NumberOfLicenses = a.Value<int>("NumberOfLicenses"),
                    Name = a.Value<string>("Name"),
                    DisplayName = a.Value<string>("Name") + " [" + a.Value<string>("Class") + "]"
                }).OrderBy(i=>i.Class).ToList();
                return Json(items, new JsonSerializerSettings()); 
            }
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task RemoveWorkingProgram(int id, bool confirmed)
        {
            var wp = await _db.DisciplineWorkingPrograms.FirstOrDefaultAsync(w => w.VersionedDocumentId == id);
            if (wp == null)
                throw new InvalidOperationException($"Не найдена версия РП с идентификатором {id}.");

            var disciplineId = wp.DisciplineId;
            var hasInheritedDocuments = _db.DisciplineWorkingPrograms.Where(w => w.DisciplineId == disciplineId).Any(w => w.BasedOnId == id);
            if (hasInheritedDocuments)
                throw new InvalidOperationException("На выбранной версии РП созданы на основе другие документы. Удаление невозможно.");

            if (confirmed)
            {
                var disciplineTitle = wp.Discipline.title;
                var wpVersion = wp.Version;

                _logger.Debug("Попытка удаления РПД с идентификатором {0}.", id);
                var doc = wp.VersionedDocument;
                _db.DisciplineWorkingPrograms.Remove(wp);
                var blocksToRemove = BlockDataHelper.GetIndependentBlocks(_db, doc);
                _db.VersionedDocuments.Remove(doc);
                _db.VersionedDocumentBlocks.RemoveRange(blocksToRemove);

                _db.SaveChanges();
                _logger.Debug("РПД с идентификатором {0} удален.", id);
                _logger.Info("Удаление версии РПД: '{0}', uid - '{1}', версия - '{2}'", disciplineTitle, disciplineId, wpVersion);
            }
        }

        public async Task<ActionResult> GetControls(string loadKind, string loadType)
        {
            List<Tmer> tmers;
            
            switch (loadType)
            {
                case "current":
                {
                    tmers = await _db.Tmers.Where(t => t.kgmer == 2)
                        .Where(t=>!new[] { "prkp", "prktr", "U002" }.Contains(t.kmer))
                        .ToListAsync();
                    break;
                }
                case "intermediate":
                {
                    tmers = await _db.Tmers.Where(t => t.kgmer == 3)
                        .Where(t => !new[] {"U039", "U032", "U033"}.Contains(t.kmer))
                        .ToListAsync();
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(loadType));
            }

            var result = tmers.Select(t => new
            {
                Name = t.rmer
            });               

            return Json(result, new JsonSerializerSettings());
        }
    }
}