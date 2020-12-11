using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using PagedList.Core;
using Ext.Utilities.Linq;
using Urfu.Its.Web.Model.Models.Practice;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using Urfu.Its.Web.Excel;
using Urfu.Its.Common;
//using Microsoft.Ajax.Utilities;
using System.Linq.Expressions;
using Urfu.Its.Web.Model;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.PracticeView)]
    public class PracticeCompaniesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var companies = SortAndFilterCompanies(sort, filter);
                var contracts = db.Contracts.Where(c => companies.Select(com => com.Id).Contains(c.CompanyId));

                var locations = db.CompanyLocations;

                var paginated = companies.ToPagedList(page ?? 1, limit ?? 25);
                var cList = paginated.ToList();
                CompaniesViewModel model = new CompaniesViewModel(cList, contracts, locations);

                return Json(
                new
                {
                    data = model.Rows,
                    total = companies.Count()
                },
                new JsonSerializerSettings()
            );
            }
            else
            {
                ViewBag.Focus = focus;
                ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
                return View();
            }
        }

        private IQueryable<Company> SortAndFilterCompanies(string sort, string filter)
        {
            SortRules sortRules = SortRules.Deserialize(sort);
            IQueryable<Company> companies = db.PracticeCompanies().OrderByThenBy(sortRules.FirstOrDefault(), v => v.Name);

            ObjectableFilterRules rules = ObjectableFilterRules.Deserialize(filter);
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    switch (rule.Property)
                    {
                        case "Name":
                            string name = rule.Value.ToString();
                            companies = companies.Where(c => c.Name.Contains(name) || c.ShortName.Contains(name));
                            break;
                        case "ContractNumbers":
                            if (rule.Value.ToString() != "")
                            {
                                var companyIds = db.Contracts.Where(c => c.Number.Contains(rule.Value.ToString())).Select(c => c.CompanyId);
                                companies = companies.Where(c => companyIds.Contains(c.Id));
                            }
                            break;
                        case "IsShortDated":
                            bool isShortDated;
                            if (bool.TryParse(rule.Value.ToString(), out isShortDated))
                            {
                                if (isShortDated)
                                {
                                    var _contracts = db.Contracts.Where(c => c.IsShortDated).Select(c => c.CompanyId);
                                    companies = companies.Where(c => _contracts.Contains(c.Id));
                                }
                                else
                                {
                                    var _contracts = db.Contracts.Where(c => !c.IsShortDated).Select(c => c.CompanyId);
                                    companies = companies.Where(c => _contracts.Contains(c.Id));
                                }
                            }
                            break;
                        case "IsEndless":
                            bool isEndless;
                            if (bool.TryParse(rule.Value.ToString(), out isEndless))
                            {
                                if (isEndless)
                                {
                                    var _contracts = db.Contracts.Where(c => c.IsEndless).Select(c => c.CompanyId);
                                    companies = companies.Where(c => _contracts.Contains(c.Id));
                                }
                            }
                            break;
                        case "WithoutContract":
                            bool withoutC;
                            if (bool.TryParse(rule.Value.ToString(), out withoutC))
                            {
                                if (withoutC)
                                {
                                    var _allContracts = db.Contracts.Select(c => c.CompanyId);
                                    companies = companies.Where(c => !_allContracts.Contains(c.Id));
                                }
                            }
                            break;
                        case "IsConfirmed":
                            bool isConfirmed;
                            if (bool.TryParse(rule.Value.ToString(), out isConfirmed))
                            {
                                companies = companies.Where(c => c.IsConfirmed == isConfirmed);
                            }
                            break;
                        //case "OwnershipType":
                        //    if (rule.Value != null)
                        //    {
                        //        int typeId;
                        //        if (int.TryParse(rule.Value.ToString(), out typeId))
                        //        {
                        //            companies = companies.Where(c => c.OwnershipTypeId == typeId);
                        //        }
                        //    }
                        //    break;
                        case "FolderNumber":
                            int folderNumber;
                            if (int.TryParse(rule.Value.ToString(), out folderNumber))
                            {
                                var _contracts = db.Contracts.Where(c => c.FolderNumber == folderNumber).Select(c => c.CompanyId);
                                companies = companies.Where(c => _contracts.Contains(c.Id));
                            }
                            break;
                    }
                }
            }
            return companies;
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult SaveCompanyInfos(CompaniesViewModelRow companyVM, bool removeInfoDoc, bool uploadInfoDoc)
        {
            try
            {
                var company = new Company();
                var isNewCompany = true;

                if (companyVM.Id != 0)
                {
                    company = db.PracticeCompanies().FirstOrDefault(c => c.Id == companyVM.Id);
                    isNewCompany = false;
                    if (company == null)
                        return Json(new { success = false, message = "Редактируемое предприятие не найдено" });
                            //"text/html", Encoding.Unicode);
                }
                company.Name = companyVM.Name;
                company.ShortName = (!string.IsNullOrEmpty(companyVM.ShortName)) ? companyVM.ShortName : companyVM.Name;
                company.INN = companyVM.INN;
                company.Director = companyVM.Director;
                company.DirectorInitials = companyVM.DirectorInitials;
                company.DirectorGenitive = companyVM.DirectorGenitive;
                company.PostOfDirector = companyVM.PostOfDirector;
                company.PostOfDirectorGenitive = companyVM.PostOfDirectorGenitive;
                company.PersonInCharge = companyVM.PersonInCharge;
                company.PersonInChargeInitials = companyVM.PersonInChargeInitials;
                company.PhoneNumber = companyVM.Phone;
                company.PostOfPersonInCharge = companyVM.PostOfPersonInCharge;
                company.Address = companyVM.Address;
                company.CompanyPhoneNumber = companyVM.CompanyPhoneNumber;
                company.Email = companyVM.Email;
                company.Site = companyVM.Site;
                company.IsConfirmed = companyVM.IsConfirmed;

                if (removeInfoDoc && company.FileStorageId != null)
                {
                    Urfu.Its.Web.DataContext.FileStorageHelper.RemoveFile((int)company.FileStorageId);
                }

                if (uploadInfoDoc)
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        var doc = Request.Form.Files[0];
                        int? fileid= Urfu.Its.Web.DataContext.FileStorageHelper.SaveFile(doc, Urfu.Its.Web.DataContext.FileCategory.PracticeCompanies, "CompanyInfo", $"{(company.Id != 0 ? "companyId" + company.Id : "companyName"+company.Name)}", company.FileStorageId);
                        company.FileStorageId = fileid;

                    }
                }

                SetLocation(company, companyVM.CountryId, companyVM.RegionId, companyVM.CityId);

                if (isNewCompany) 
                    db.Companies.Add(company);

                db.SaveChanges();
                if (!isNewCompany)
                {
                    Logger.Info($"Отредактировано предприятие id:{company.Id} {company.Name}");
                }
                else
                {
                    Logger.Info($"Добавлено предприятие id:{company.Id} {company.Name}");
                }
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }
            catch
            {
                return Json(new { success = false, message = "Неизвестная ошибка" });//, "text/html", Encoding.Unicode);
            }

        }

        private Company SetLocation(Company company, int? countryId, int? regionId, int? cityId)
        {
            company.CompanyLocationId = null;
            var locationIds = new List<int?>() { cityId, regionId, countryId };
            foreach (var locationId in locationIds)
            {
                var location = db.CompanyLocations.FirstOrDefault(l => l.Id == locationId);
                if (location != null)
                {
                    company.CompanyLocationId = location.Id;
                    break;
                }
            }
            return company;
        }

        public ActionResult DownloadInfoDocument(int id)
        {
            var company = db.PracticeCompanies().FirstOrDefault(c => c.Id == id);

            if (company.FileStorageId != null)               
            return File(Urfu.Its.Web.DataContext.FileStorageHelper.GetBytes((int)company.FileStorageId), System.Net.Mime.MediaTypeNames.Application.Octet, company.FileStorage.FileNameForUser);

                return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult CheckStudentsAdmissions(int companyId)
        {
            var admissions = db.PracticeAdmissionCompanys.Where(a => a.Contract != null)
                .Where(a => a.Contract.CompanyId == companyId && a.Status == AdmissionStatus.Admitted).ToList();

            if (admissions.Count() > 0)
            {
                string students = StudentsInfo("Существуют студенты, зачисленные на предприятие<br></br>", admissions);
                students += "<br></br>";

                return Json(new { success = false, message = students });//, "text/html", Encoding.Unicode);
            }
            else
            {
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult CheckStudentsAdmissions2(List<int> companyIds)
        {
            var admissions = db.PracticeAdmissionCompanys.Where(a => a.Contract != null)
                .Where(a => companyIds.Contains(a.Contract.CompanyId) && a.Status == AdmissionStatus.Admitted)
                .GroupBy(a => a.Contract.Company.Name)
                .ToList();

            if (admissions.Count() > 0)
            {
                string studentsInfo = "";
                foreach (var company in admissions)
                {
                    studentsInfo += "<br></br><b>" + company.Key + "</b><br></br>";
                    studentsInfo = StudentsInfo(studentsInfo, company.AsEnumerable().ToList());
                }

                string students = "Существуют студенты, зачисленные на выбранные предприятия";
                students += studentsInfo;
                students += "<br></br>";
                return Json(new { success = false, message = students });//, "text/html", Encoding.Unicode);
            }
            else
            {
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteCompany(int companyId)
        {
            try
            {
                var company = db.PracticeCompanies().FirstOrDefault(c => c.Id == companyId);
                var contractInfo = db.Contracts.Where(cont => cont.CompanyId == company.Id).ToList();
                var admissions = db.PracticeAdmissionCompanys.Where(a => a.Contract != null).Where(a => a.Contract.CompanyId == company.Id && a.Status == AdmissionStatus.Admitted).ToList();

                if (company.FileStorageId != null)
                {
                    Urfu.Its.Web.DataContext.FileStorageHelper.RemoveFile((int)company.FileStorageId);
                }

                contractInfo.ForEach(c =>
                {
                    if (c.FileStorageId != null)
                        Urfu.Its.Web.DataContext.FileStorageHelper.RemoveFile((int)c.FileStorageId);
                });

                db.Companies.Remove(company);
                db.SaveChanges();

                if (contractInfo.Count() == 0)
                    Logger.Info($"Удалено предприятие id: {company.Id} {company.Name}");
                else
                    foreach (var c in contractInfo)
                    {
                        var studentsId = "";
                        foreach (var a in admissions.Where(a => a.ContractId == c.Id).Distinct()) studentsId += a.Practice.StudentId + "  ";
                        Logger.Info($"Удалено предприятие id:{company.Id} {company.Name} Договор id: {c.Id} Номер договора:{c.Number} {(studentsId.Length > 0 ? "UID студентов:" + studentsId : "")}");
                    }               
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch
            {
                return Json(new { success = false, message = "Неизвестная ошибка" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteCompanies(List<int> companyIds)
        {
            try
            {
                var companies = db.PracticeCompanies().Where(c => companyIds.Contains(c.Id)).ToList();
                var admissions = db.PracticeAdmissionCompanys.Where(a => a.Contract != null && companyIds.Contains(a.Contract.CompanyId) && a.Status == AdmissionStatus.Admitted).ToList();

                companies.ForEach(c =>
                {
                    if (c.FileStorageId != null)
                        Urfu.Its.Web.DataContext.FileStorageHelper.RemoveFile((int)c.FileStorageId);
                });

               var contractInfo =db.Contracts.Where(con => companyIds.Contains(con.CompanyId)).ToList();

                contractInfo.ForEach(con =>{
                                if (con.FileStorageId != null)
                        Urfu.Its.Web.DataContext.FileStorageHelper.RemoveFile((int)con.FileStorageId);
                });

                db.Companies.RemoveRange(companies);
                db.SaveChanges();

                companies.ForEach(c =>
                {
                   var contracts = contractInfo.Where(con => con.CompanyId == c.Id);
                      if (contracts.Count()!= 0)
                    {
                        foreach(var contr in contracts)                        
                        {
                            var studentsId = "";
                            foreach(var p in admissions.Where(a => a.ContractId == contr.Id)) studentsId += p.Practice.StudentId + "  ";
                            Logger.Info($"Удалено предприятие id:{c.Id} {c.Name} Договор id: {contr.Id} Номер договора:{contr.Number} {(studentsId.Length > 0 ? "UID студентов:" + studentsId : "")}");
                        }
                    }
                    else
                      Logger.Info($"Удалено предприятие id: {c.Id} {c.Name}");
                });

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch
            {
                return Json(new { success = false, message = "Неизвестная ошибка" });//, "text/html", Encoding.Unicode);
            }
        }

        public ActionResult Countries()
        {
            var countries = db.CompanyLocations.Where(l => l.Level == 1 && l.ParentId == null)
                .Select(l => new { CountryId = l.Id, Country = l.Name }).OrderBy(c => c.Country);
            return Json(
                new
                {
                    data = countries
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Regions(int countryId)
        {
            var regions = db.CompanyLocations.Where(l => l.Level == 2 && l.ParentId == countryId)
                .Select(l => new { RegionId = l.Id, Region = l.Name }).OrderBy(c => c.Region);
            return Json(
                new
                {
                    data = regions
                },
                new JsonSerializerSettings()
            ); 
        }

        public ActionResult Cities(int regionId)
        {
            var cities = db.CompanyLocations.Where(l => l.Level == 3 && l.ParentId == regionId)
                .Select(l => new { CityId = l.Id, City = l.Name }).OrderBy(c => c.City);
            return Json(
                new
                {
                    data = cities
                },
                new JsonSerializerSettings()
            );
        }

        //public ActionResult OwnershipTypes()
        //{
        //    var types = db.OwnershipTypes.Select(t => new
        //    {
        //        OwnershipTypeId = t.Id,
        //        OwnershipType = t.ShortName
        //    }).OrderBy(t => t.OwnershipType);
        //    return Json(
        //        new
        //        {
        //            data = types
        //        },
        //        JsonRequestBehavior.AllowGet
        //    );
        //}

        public ActionResult DownloadCompaniesReportExcel(string filter)
        {
            var companies = SortAndFilterCompanies(null, filter);
            var contracts = db.Contracts.Where(c => companies.Select(com => com.Id).Contains(c.CompanyId));
            var locations = db.CompanyLocations;

            CompaniesViewModel model = new CompaniesViewModel(companies.ToList(), contracts, locations);

            var stream = new VariantExport().Export(new
            {
                Rows = model.Rows
            }, "companiesReportTemplate.xlsx");
            return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, "Предприятия.xlsx");
        }

        public ActionResult SimilarCompanies(int id, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var companies = db.PracticeCompanies().Where(c => c.Id != id && (c.Name.Contains(name) || c.ShortName.Contains(name)))
                    .Select(c => new { c.Id, c.Name, c.Address, Status = c.IsConfirmed ? "Подтверждено" : "На проверку" }).OrderBy(c => c.Name).ToList();

                return Json(
                    new
                    {
                        data = companies,
                        total = companies.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                return Json(
                    new
                    {
                        data = new List<object>(),
                        total = 0
                    },
                    new JsonSerializerSettings()
                );
            }
        }

        public ActionResult IntegrateCompanies(List<int> idsToRemove, int newId)
        {
            var company = db.PracticeCompanies().FirstOrDefault(c => c.Id == newId);
            if (idsToRemove == null && company == null)
                return Json(new { success = false, message = "Предприятия не найдены" });//, "text/html", Encoding.Unicode);

            var companiesToRemove = db.PracticeCompanies().Where(c => idsToRemove.Contains(c.Id)).ToList();

            // перевести все договоры удаляемых предприятий на новое предприятие
            var contracts = db.Contracts.Where(c => idsToRemove.Contains(c.CompanyId));
            foreach (var contract in contracts)
            {
                contract.CompanyId = newId;
            }

            // удалить выбранные предприятия
            db.Companies.RemoveRange(companiesToRemove);
            db.SaveChanges();

            return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
        }


        public ActionResult SetConfirmedStatus(int id)
        {
            try
            {
                var company = db.PracticeCompanies().FirstOrDefault(c => c.Id == id);

                if (company != null)
                {
                    if (company.Location == null)
                        return Json(new { success = false, message = "Предприятие не подтверждено. Не указаны Страна, Регион, Город" });
                            //"text/html", Encoding.Unicode);
                    if(company.Location.Level != 3)
                    {
                         string message = "Предприятие не подтверждено. " + (ParentCount(company.Location) == 0 ? "Не указаны Регион и Город" : "Не указан Город");
                        return Json(new { success = false, message });
                            //"text/html", Encoding.Unicode); 
                    }
                }
                company.IsConfirmed = true;
                db.SaveChanges();
                return Json(new { success = true, message = "Предприятие подтверждено" });
                    //"text/html", Encoding.Unicode);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        int ParentCount(CompanyLocation location)
        {

            var count = location.Parent != null ? 1 : 0;
            return count + (location.Parent == null ? 0 : ParentCount(location.Parent));
        }


        #region Contracts page

        public ActionResult Contracts(int id, int? contractId, int? page, int? limit, string sort, string filter, string focus)
        {
            var company = db.Companies.FirstOrDefault(c => c.Id == id);
            var project = db.Projects.FirstOrDefault(p => p.ContractId == contractId && contractId.HasValue);
            var companyName = contractId.HasValue ? $"\"{company.Name} (проект \"{project?.Module?.title}\")" : $"\"{company.Name}\"";
            ContractsViewModel model = new ContractsViewModel(id, companyName, company.Director, company.DirectorInitials, company.DirectorGenitive,
                company.PostOfDirector, company.PostOfDirectorGenitive,
                company.PersonInCharge, company.PersonInChargeInitials,company.PostOfPersonInCharge, company.PhoneNumber, company.Email);

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                SortRules sortRules = SortRules.Deserialize(sort);

                var contracts = db.Contracts.Where(c => c.CompanyId == id && (contractId == null || c.Id == contractId))
                    .Select(c => new ContractStudents
                    {
                        Contract = c,
                        Students = db.PracticeAdmissionCompanys.Where(a => a.ContractId == c.Id && !a.remove).Select(a => a.Practice.Student).ToList()
                    })
                    .ToList();

                var rows = model.GetRows(contracts);
                rows = rows.Where(ObjectableFilterRules.Deserialize(filter)).OrderByThenBy(sortRules.FirstOrDefault(), v => v.Number);

                return Json(
                    new
                    {
                        data = rows,
                        total = rows.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                var okso = db.Directions.Select(d => new
                {
                    Id = d.uid,
                    Name = d.okso + " ("+ d.standard+ ")",
                }).OrderBy(d => d.Name).ToList();
                ViewBag.Okso = JsonConvert.SerializeObject(okso);

                var qualifications = db.Qualifications.ToList();
                ViewBag.Qualifications = JsonConvert.SerializeObject(qualifications);

                var semesters = db.Semesters.ToList();
                ViewBag.Semesters = JsonConvert.SerializeObject(semesters);

                ViewBag.Focus = focus;
                ViewBag.ContractId = contractId;
                ViewBag.IsProjectPage = contractId.HasValue;
                
                ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
                ViewBag.ProjectEdit = User.IsInRole(ItsRoles.ProjectManager) || User.IsInRole(ItsRoles.ProjectROP);

                return View("Contracts", model);
            }
        }

        public ActionResult Contract(int id, int rowIndex)
        {
            try
            {
                var contract = db.Contracts.FirstOrDefault(c => c.Id == id);
                ContractsViewModel model = new ContractsViewModel(
                    contract.CompanyId,
                    contract.Company.Name,
                    contract.Company.Director,
                    contract.Company.DirectorInitials,
                    contract.Company.DirectorGenitive,
                    contract.Company.PostOfDirector,
                    contract.Company.PostOfDirectorGenitive,
                    contract.Company.PersonInCharge,
                    contract.Company.PersonInChargeInitials,
                    contract.Company.PostOfPersonInCharge,
                    contract.Company.PhoneNumber,
                    contract.Company.Email);
                var students = db.PracticeAdmissionCompanys.Where(a => a.ContractId == contract.Id && !a.remove).Select(a => a.Practice.Student).ToList();

                var contractRow = model.GetRowOfOneContract(new ContractStudents(contract, students), rowIndex);

                return Json(
                    new
                    {
                        data = contractRow,
                    },
                    new JsonSerializerSettings()
                );
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }
       

        [HttpPost]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult AddOrUpdateContract(Contract contract, bool removeScan, bool uploadScan)
        {

            if (contract != null)
            {
                try
                {
                    if (contract.IsShortDated)
                    {
                        var existContract = db.Contracts.FirstOrDefault(c => c.Id != contract.Id && c.IsShortDated == true && c.Number == contract.Number);
                        if (existContract != null)
                            return Json(new { success = false, message = "Договор с таким номером уже существует" });//, "text/html", Encoding.Unicode);
                    }

                    bool isNewContract = contract.Id == 0;

                    if (!isNewContract && !db.Contracts.Any(c => c.Id == contract.Id))
                        return Json(new { success = false, message = "Редактируемый договор не найден" });

                    db.Entry(contract).State = isNewContract ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    
                    if (contract.IsShortDated && isNewContract)
                        contract.SerialNumber = db.GetNextSerialNumberKsContract(contract.Year.Value);

                    var filestorageId = isNewContract ? null:db.Entry(contract).GetDatabaseValues()["FileStorageId"] as int?;
                    if ( removeScan && filestorageId != null)
                        Urfu.Its.Web.DataContext.FileStorageHelper.RemoveFile((int)filestorageId);

                       else if (uploadScan && Request.Form.Files.Count > 0)
                        {
                            var doc = Request.Form.Files[0];
                            int? fileId = Urfu.Its.Web.DataContext.FileStorageHelper.SaveFile(doc, Urfu.Its.Web.DataContext.FileCategory.PracticeCompanies, "Contracts", $"{(contract.Id != 0 ? "contractId " + contract.Id : "contractNumber" + contract.Number)}", filestorageId);
                            contract.FileStorageId = fileId;
                        }

                    db.SaveChanges();

                    if (!isNewContract)
                    {
                        Logger.Info($"Отредактирован договор id:{contract.Id} Номер {contract.Number} Предприятие id:{contract.CompanyId} {db.Companies.FirstOrDefault(comp => comp.Id == contract.CompanyId)?.Name}");
                    }
                    else
                    {
                        Logger.Info($"Добавлен договор id:{contract.Id} Номер {contract.Number} Предприятие id:{contract.CompanyId} {db.Companies.FirstOrDefault(comp => comp.Id == contract.CompanyId)?.Name}");
                    }
                    return Json(
                        new
                        {
                            success = true,
                            id = contract.Id,
                        });//, "text/html", Encoding.Unicode);


                }
                catch
                {
                    return Json(new { success = false, message = "Неизвестная ошибка" }/*, "text/html", Encoding.Unicode*/);
                }
            }
            
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

        }


        [HttpGet]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult CreateContractKsNumber(int year)
        {
            string number = db.CreateContractKsNumber(year);
            return Json(new { success = true, contractNumber = number }, new JsonSerializerSettings());
        }

        public ActionResult YearsForFilter()
        {
            var years = db.ContractPeriods.Select(p => new { Year = p.Year }).Distinct().OrderBy(y => y.Year);
            return Json(
                new
                {
                    data = years
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult DownloadScan(int id)
        {
            var contract = db.Contracts.FirstOrDefault(c => c.Id == id);

            if (contract?.FileStorageId != null)
                return File(Urfu.Its.Web.DataContext.FileStorageHelper.GetBytes((int)contract.FileStorageId), System.Net.Mime.MediaTypeNames.Application.Octet, contract.FileStorage.FileNameForUser);

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult CheckStudentsAdmissionsOnContract(int contractId)
        {
            var admissions = db.PracticeAdmissionCompanys.Where(a => a.ContractId == contractId && a.Status == AdmissionStatus.Admitted).ToList();

            if (admissions.Count() > 0)
            {
                string students = StudentsInfo("Существуют студенты, зачисленные на предприятие по данному договору<br></br>", admissions);
                students += "<br></br>";
                return Json(new { success = false, message = students });//, "text/html", Encoding.Unicode);
            }
            else
            {
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult DeleteContract(int contractId)
        {
            try
            {
                var contract = db.Contracts.FirstOrDefault(c => c.Id == contractId);
                //Получение информации о предприятии и зачисленных студентах для Лога
                int companyID = contract.Company.Id;
                string companyName = contract.Company.Name;
                var admissions = db.PracticeAdmissionCompanys.Where(a => a.ContractId == contractId && a.Status == AdmissionStatus.Admitted).ToList();
                string studentsId = "";
                if (admissions.Count() > 0)
                {
                    admissions = admissions.Distinct().ToList();

                    foreach (var admission in admissions)
                    {
                        studentsId += admission.Practice.StudentId + "  ";
                    }

                }

                if (contract.FileStorageId != null)
                    Urfu.Its.Web.DataContext.FileStorageHelper.RemoveFile((int)contract.FileStorageId);

                db.Contracts.Remove(contract);
                db.SaveChanges();
                if (studentsId.Length > 0)
                {
                    Logger.Info($"Удален договор id:{contractId} Номер договора:{contract.Number} id Предприятия:{companyID} {companyName}  UID студентов:{studentsId}");
                }
                else
                    Logger.Info($"Удален договор id:{contractId} Номер договора:{contract.Number} id Предприятия:{companyID} {companyName}");

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch
            {
                return Json(new { success = false, message = "Неизвестная ошибка" });//, "text/html", Encoding.Unicode);
            }
        }

        #endregion

        #region Periods window

        public ActionResult Semesters()
        {
            var semesters = db.Semesters.ToList();
            return Json(
                new
                {
                    data = semesters
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Years()
        {
            var currentYear = DateTime.Today.Year;
            var years = new List<object>() { new { Year = currentYear - 1 } };
            for (int i = 0; i < 8; i++)
                years.Add(new { Year = currentYear + i });

            return Json(
                new
                {
                    data = years
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Periods(int contractId, int? page, int? limit, string sort, string filter, string focus)
        {
            SortRules sortRules = SortRules.Deserialize(sort);
            var periods = db.ContractPeriods.Where(p => p.ContractId == contractId)
                .OrderByThenBy(sortRules.FirstOrDefault(), v => v.Year).ToList();
            //var model = new PeriodsViewModel(periods);
            var model = periods.Select(p => new
            {
                p.Id,
                p.Year,
                p.SemesterId,
                Semester=p.Semester.Name,
                p.RequestNumber,
                p.AdditionalTerms,
                p.DivisionDescription,
                FileId= p.FileStorage?.Id,
                FileName = p.FileStorage?.FileNameForUser
            }).ToList();

            return Json(
                new
                {
                    data = model,
                    total = model.Count()
                },
                new JsonSerializerSettings()
            );
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult AutogenerationPeriods(int contractId, DateTime? finishDate)
        {
            var contract = db.Contracts.FirstOrDefault(c => c.Id == contractId);

            if (contract.IsEndless)
            {
                if (finishDate != null)
                {
                    contract.FinishDate = finishDate;
                    db.SaveChanges();
                }
                else
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            if (contract.StartDate != null && contract.FinishDate != null)
            {
                List<ContractPeriod> newPeriods = new List<ContractPeriod>();
                int startYear = contract.StartDate.Value.Year;
                int finishYear = contract.FinishDate.Value.Year;

                if (contract.StartDate.Value.Month < 9)
                {
                    newPeriods.Add(new ContractPeriod
                    {
                        ContractId = contractId,
                        Year = contract.StartDate.Value.Year - 1,
                        SemesterId = db.Semesters.FirstOrDefault(s => s.Name == "Весенний").Id
                    });
                }
                for (int year = startYear; year < finishYear; year++)
                {
                    newPeriods.Add(new ContractPeriod
                    {
                        ContractId = contractId,
                        Year = year,
                        SemesterId = db.Semesters.FirstOrDefault(s => s.Name == "Осенний").Id
                    });
                    newPeriods.Add(new ContractPeriod
                    {
                        ContractId = contractId,
                        Year = year,
                        SemesterId = db.Semesters.FirstOrDefault(s => s.Name == "Весенний").Id
                    });
                }
                if (contract.FinishDate.Value.Month >= 9)
                {
                    newPeriods.Add(new ContractPeriod
                    {
                        ContractId = contractId,
                        Year = finishYear,
                        SemesterId = db.Semesters.FirstOrDefault(s => s.Name == "Осенний").Id
                    });
                }
                var addedPeriods = AddNewPeriods(contractId, newPeriods);
                AddLimits(contractId, addedPeriods);
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        private List<ContractPeriod> AddNewPeriods(int contractId, List<ContractPeriod> newPeriods)
        {
            var addedPeriods = new List<ContractPeriod>();
            var periods = db.ContractPeriods.Where(p => p.ContractId == contractId).Select(p => new { p.Year, p.SemesterId }).ToList();
            foreach (var period in newPeriods)
            {
                var existingPeriods = periods.Where(p => p.Year == period.Year && p.SemesterId == period.SemesterId);
                if (existingPeriods.Count() == 0)
                {
                    db.ContractPeriods.Add(period);
                    addedPeriods.Add(period);
                }
            }
            db.SaveChanges();
            return addedPeriods;
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult UpdatePeriod(int id, int contractId, int year, int semesterId)
        {
            var existingPeriod = db.ContractPeriods.FirstOrDefault(p => p.ContractId == contractId && p.Year == year && p.SemesterId == semesterId);

            if (existingPeriod == null)
            {
                var period = db.ContractPeriods.FirstOrDefault(p => p.Id == id);
                if (period != null)
                {
                    period.Year = year;
                    period.SemesterId = semesterId;
                    db.SaveChanges();
                }
                else
                {
                    // новый период
                    period = new ContractPeriod()
                    {
                        Year = year,
                        SemesterId = semesterId,
                        ContractId = contractId
                    };
                    db.ContractPeriods.Add(period);
                    db.SaveChanges();

                    AddLimits(contractId, period);
                }
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            else
                return Json(new { msg = "Такой период уже существует" });
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemovePeriod(int id)
        {
            var period = db.ContractPeriods.FirstOrDefault(p => p.Id == id);
            if (period != null)
            {
                var isLimits = db.ContractLimits.Where(l => l.ContractPeriodId == id && l.Limit > 0).Count() > 0;
                if (isLimits)
                {
                    return Json(new { msg = "На данный период существуют лимиты" });
                }
                else
                {
                    db.ContractPeriods.Remove(period);
                    db.SaveChanges();
                    return new StatusCodeResult(StatusCodes.Status200OK);
                }
            }
            else
                return Json(new { success = false, message = "Неизвестная ошибка" });//, "text/html", Encoding.Unicode);
        }

        #endregion

        #region Directions window

        public ActionResult Okso(int? id, string filter)
        {
            if (id != null)
            {
                var directions = db.Directions.ToList().AsQueryable().Where(FilterRules.Deserialize(filter)).ToList();
                var profiles = db.Profiles.Where(p => !p.remove).ToList().AsQueryable().Where(FilterRules.Deserialize(filter)).ToList();
                var contract = db.Contracts.FirstOrDefault(c => c.Id == id);
                var limits = contract.Periods.Where(p => p.Limits != null).Select(p => p.Limits);
                foreach (var p in profiles)
                {
                    if (!directions.Contains(p.Direction))
                        directions.Add(p.Direction);
                }
                OksoViewModel model = new OksoViewModel(directions, profiles, limits);
                return JsonNet(model.Roots.OrderBy(m=>m.text));
            }
            else
                return JsonNet(new { });
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveAndAddOkso(int contractId, List<string> checkedOksoIds, List<string> uncheckedOksoIds,
            List<string> checkedProfileIds, List<string> uncheckedProfileIds)
        {
            uncheckedOksoIds = uncheckedOksoIds ?? new List<string>();
            checkedOksoIds = checkedOksoIds ?? new List<string>();
            checkedProfileIds = checkedProfileIds ?? new List<string>();
            uncheckedProfileIds = uncheckedProfileIds ?? new List<string>();

            var contractLimits = db.ContractLimits.Where(c => c.Period.ContractId == contractId).ToList();
            var notEmpty = contractLimits.Where(c => (uncheckedOksoIds.Contains(c.DirectionId) || uncheckedProfileIds.Contains(c.ProfileId)) && c.Limit > 0);
            if (notEmpty.Count() > 0)
            {
                var periods = db.ContractPeriods.Where(p => p.ContractId == contractId).ToList();
                if (periods.Count != 0)
                {
                    AddLimits(contractId, checkedOksoIds, checkedProfileIds);
                }
                else
                {
                    return Json(new { success = false, isLimits = false, msg = "Не определен ни один учебный период" });
                }

                // сообщение об удалении вместе с лимитами
                return Json(new { success = false, isLimits = true, msg = "Невозможно удалить направления, так как существуют ненулевые лимиты" });
            }
            else
            {
                var periods = db.ContractPeriods.Where(p => p.ContractId == contractId).ToList();
                if (periods.Count != 0)
                {
                    RemoveLimits(contractId, uncheckedOksoIds, uncheckedProfileIds);
                    AddLimits(contractId, checkedOksoIds, checkedProfileIds);
                    RemoveEmptyLimits(contractId);

                    UpdateModuleSpecialities(contractId);

                    var _contractLimits = db.ContractLimits.Where(c => c.Period.ContractId == contractId).ToList();
                    List<string> okso = new List<string>();
                    List<string> fullOkso = new List<string>();
                    List<string> profile = new List<string>();
                    List<string> fullProfile = new List<string>();
                    foreach (var l in _contractLimits)
                    {
                        if (l.Direction != null)
                        {
                            okso.Add(l.Direction.okso);
                            fullOkso.Add(l.Direction.OksoAndTitle);
                        }
                        if (l.Profile != null)
                        {
                            profile.Add(l.Profile.NAME);
                            profile.Add(l.Profile.OksoAndTitle);
                        }
                    }

                    return Json(
                        new
                        {
                            success = true,
                            okso = string.Join(", ", okso.Distinct()),
                            fullOkso = string.Join(", ", fullOkso.Distinct()),
                            profile = string.Join(", ", profile.Distinct()),
                            fullProfile = string.Join(", ", fullProfile.Distinct()),
                        });//, "text/html", Encoding.Unicode);
                }
                else
                {
                    return Json(new { success = false, isLimits = false, msg = "Не определен ни один учебный период" });
                }
            }
        }

        private void RemoveLimits(int contractId, List<string> directionIds, List<string> profileIds)
        {
            var periods = db.ContractPeriods.Where(p => p.ContractId == contractId).ToList();
            var limits = db.ContractLimits.Where(l => l.Period.ContractId == contractId && (directionIds.Contains(l.DirectionId) || profileIds.Contains(l.ProfileId)));
            db.ContractLimits.RemoveRange(limits);
            db.SaveChanges();
        }

        private void AddLimits(int contractId, List<string> directionIds, List<string> profileIds)
        {
            var contractLimits = db.ContractLimits.Where(l => l.Period.ContractId == contractId);
            var periods = db.ContractPeriods.Where(p => p.ContractId == contractId).ToList();
            foreach (var profileId in profileIds)
            {
                foreach (var p in periods)
                {
                    if (contractLimits.FirstOrDefault(l => l.ProfileId == profileId && l.ContractPeriodId == p.Id) == null)
                    {
                        var profile = db.Profiles.FirstOrDefault(pr => pr.ID == profileId);
                        directionIds.Remove(profile.DIRECTION_ID);

                        AddEmptyLimit(p.Id, directionId: profile.DIRECTION_ID, profileId: profile.ID);
                    }
                }
            }
            foreach (var directionId in directionIds)
            {
                foreach (var p in periods)
                {
                    if (contractLimits.FirstOrDefault(l => l.DirectionId == directionId && l.ContractPeriodId == p.Id) == null)
                    {
                        AddEmptyLimit(p.Id, directionId: directionId, profileId: null);
                    }
                }
            }
            db.SaveChanges();
        }

        private void RemoveEmptyLimits(int contractId)
        {
            var emptyLimits = db.ContractLimits.Where(l => l.Period.ContractId == contractId
                && l.QualificationName == null
                && l.Course == 0
                && l.Limit == 0
                && l.DirectionId == null
                && l.ProfileId == null);
            db.ContractLimits.RemoveRange(emptyLimits);
            db.SaveChanges();
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveOkso(int contractId, List<string> oksoToDelete)
        {
            oksoToDelete = oksoToDelete ?? new List<string>();
            var contractLimits = db.ContractLimits.Where(c => c.Period.ContractId == contractId).ToList();

            foreach (var okso in oksoToDelete)
            {
                var delete = contractLimits.Where(c => c.DirectionId == okso);
                db.ContractLimits.RemoveRange(delete);
            }
            db.SaveChanges();

            UpdateModuleSpecialities(contractId);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        #endregion

        #region Limits window

        public ActionResult Limits(int? id, int? page, int? limit, string sort, string filter, string focus)
        {
            if (id != null)
            {
                var contract = db.Contracts.FirstOrDefault(c => c.Id == id);
                var admissions = db.PracticeAdmissionCompanys.Where(a => a.ContractId == id).ToList();
                var model = new LimitsViewModel(contract, admissions);
                var rows = model.Rows.Where(ObjectableFilterRules.Deserialize(filter));

                return Json(
                        new
                        {
                            data = rows,
                            total = rows.Count()
                        },
                        new JsonSerializerSettings()
                    );
            }
            else
                return Json(
                        new
                        {
                            data = new List<LimitsViewModelRow>(),
                            total = 0
                        },
                        new JsonSerializerSettings()
                    );
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult SetContractLimits(int id)
        {
            var limits = db.ContractLimits.Where(l => l.Period.ContractId == id).ToList();
            var contractLimit = db.Contracts.FirstOrDefault(c => c.Id == id).Limit;
            var admissions = db.PracticeAdmissionCompanys.Where(a => a.ContractId == id && a.Status ==AdmissionStatus.Admitted).ToList();
            if (contractLimit != null)
            {
                
                foreach (var l in limits)
                {
                    var limitAdmissions = admissions.Where(a =>
                        a.Practice.Year == l.Period.Year
                        && a.Practice.SemesterId == l.Period.SemesterId
                        && (l.Profile == null || a.Practice.Group.ProfileId == l.ProfileId)
                        && (l.Direction == null || a.Practice.Group.Profile.DIRECTION_ID == l.DirectionId)
                        && (l.Course == 0 || l.Course == a.Practice.Group.Course)
                        && (l.Qualification == null || l.QualificationName == a.Practice.Group.Qual)).GroupBy(a => a.Practice.StudentId).Count();
                    if (limitAdmissions > contractLimit)
                        continue;
                    l.Limit = (int)contractLimit;
                }
                db.SaveChanges();
                return Json(new { success = true });//, "text/html", Encoding.Unicode);
            }

            return Json(new { success = false, message = "Лимит по договору не указан" });//, "text/html", Encoding.Unicode);
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult UpdateLimit(int id, string oksoId, string profileId, string qualification, int course, int periodId, int limitCount)
        {
            Expression<Func<ContractLimit, bool>> equalExpression = l =>
                (l.DirectionId == oksoId || oksoId == "-1" && l.DirectionId == null)
                && (l.ProfileId == profileId || profileId == "-1" && l.ProfileId == null)
                && (l.QualificationName == qualification || qualification == "Не указано" && l.QualificationName == null)
                && l.Course == course
                && l.Period.Id == periodId;
            Func<ContractLimit, bool> isEqual = equalExpression.Compile();

            var limitcheck = db.ContractLimits.Where(equalExpression);

            if (limitcheck.Any() && id == -1)
            {
                return Json(new { success = false, message = "На выбранный период по указанному направлению/ОП лимит уже сохранен", deleted = true });//, "text/html", Encoding.Unicode);
            }
            else if (limitcheck.Any(l => l.Id != id))
            {
                return Json(new { success = false, message = "На выбранный период по указанному направлению/ОП лимит уже сохранен", deleted = false });//, "text/html", Encoding.Unicode);
            }

            var limit = db.ContractLimits.FirstOrDefault(l => l.Id == id);
            var admissions = GetAdmissionsOnLimit(limit);
            if (admissions != null && admissions.Count() > 0 && (limit.Limit > limitCount || !isEqual(limit)))
            {
                string students = StudentsInfo("Изменения не будут применены. Существуют студенты, зачисленные на предприятие по данному лимиту<br></br>", admissions);
                students += "<br></br>";
                return Json(new { success = false, message = students });//, "text/html", Encoding.Unicode);
            }

            var okso = db.Directions.FirstOrDefault(d => d.uid == oksoId);
                var profile = db.Profiles.FirstOrDefault(p => p.ID == profileId);
                var qualificationFromDb = db.Qualifications.FirstOrDefault(q => q.Name == qualification);
                
                bool isNewLimit = false;
                if (limit == null)
                {
                    isNewLimit = true;
                    limit = new ContractLimit();
                }

                bool isOK = true;
                // проверка на соответствие okso и profile
                if (profile != null)
                {
                    isOK = profile.DIRECTION_ID == oksoId;
                }

            if (isOK)
            {
                limit.DirectionId = okso?.uid;
                limit.ProfileId = profile?.ID;
                limit.QualificationName = qualificationFromDb?.Name;
                limit.Course = course;
                limit.Period = db.ContractPeriods.FirstOrDefault(p => p.Id == periodId);
                limit.Limit = limitCount;

                if (isNewLimit)
                    db.ContractLimits.Add(limit);

                db.SaveChanges();

                UpdateModuleSpecialities(limit.Period.ContractId);

                return Json(new {
                    success = true,
                    message = "",
                    Id = limit.Id,
                    CurrentLimit = admissions != null ? limitCount - admissions.GroupBy(s => s.Practice.StudentId).Count() : limitCount
                });//, "text/html", Encoding.Unicode);
            }
            else
                return Json(new { success = false, message = "Образовательная программа не соответствует выбранному направлению" });//, "text/html", Encoding.Unicode);
            
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult CheckStudentsAdmissionsOnLimit(int limitId)
        {
            var limit = db.ContractLimits.FirstOrDefault(l => l.Id == limitId);

            var admissions = GetAdmissionsOnLimit(limit);

            if (admissions!=null && admissions.Count() > 0)
            {
                string students = StudentsInfo("Существуют студенты, зачисленные на предприятие по данному лимиту<br></br>", admissions);
                students += "<br></br>";
                return Json(new { success = false, message = students });//, "text/html", Encoding.Unicode);
            }
            else
            {
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult CheckStudentsAdmissionsOnLimits(List<int> limitIds)
        {
            if (limitIds == null)
                return Json(new { success = false, message = "Лимиты не выбраны" });//, "text/html", Encoding.Unicode);

            var limits = db.ContractLimits.Where(l => limitIds.Contains(l.Id)).ToList();

            var admissions = new List<PracticeAdmissionCompany>();
            foreach (var l in limits)
            {
                admissions.AddRange(GetAdmissionsOnLimit(l));
            }

            if (admissions.Count() > 0)
            {
                string students = StudentsInfo("Существуют студенты, зачисленные на предприятия по указанным лимитам<br></br>", admissions);
                students += "<br></br>";
                return Json(new { success = false, message = students });//, "text/html", Encoding.Unicode);
            }
            else
            {
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveLimit(int id)
        {
            var limit = db.ContractLimits.FirstOrDefault(l => l.Id == id);
            if (limit != null)
            {
                var period = limit.Period;

                var admissions = GetAdmissionsOnLimit(limit);
                if (admissions != null)
                {
                    foreach (var admission in admissions)
                    {
                        admission.Status = AdmissionStatus.Denied;
                        admission.ReasonOfDeny = "Лимит по Вашему направлению был удален";
                    }
                }

                db.ContractLimits.Remove(limit);
                db.SaveChanges();

                UpdateModuleSpecialities(period.ContractId);

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            else
                return Json(new { success = false, message = "Лимит не найден" }); //, "text/html", Encoding.Unicode);
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveLimits(List<int> ids)
        {
            if (ids == null)
                return Json(new { success = false, message = "Лимиты не выбраны" });//, "text/html", Encoding.Unicode);

            var limits = db.ContractLimits.Where(l => ids.Contains(l.Id)).ToList();

            foreach (var l in limits)
            {
                var admissions = GetAdmissionsOnLimit(l);

                foreach (var admission in admissions)
                {
                    admission.Status = AdmissionStatus.Denied;
                    admission.ReasonOfDeny = "Лимит по Вашему направлению был удален";
                }
            }

            db.ContractLimits.RemoveRange(limits);
            db.SaveChanges();

            UpdateModuleSpecialities(limits.FirstOrDefault()?.Period?.ContractId);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult PeriodList(int id)
        {
            var periods = db.ContractPeriods.Where(p => p.ContractId == id).Select(p => new
            {
                Id = p.Id,
                Name = p.Year + " (" + p.Semester.Name + ")"
            });

            return Json(
                new
                {
                    data = periods
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult LimitYearsList()
        {
            var years = db.ContractPeriods.Select(p => new { Year = p.Year }).Distinct().OrderBy(a => a.Year);

            return Json(
                new
                {
                    data = years
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult OksoList(int id)
        {
            var periods = db.ContractPeriods.Where(p => p.ContractId == id);
            var okso = db.ContractLimits.Where(l => periods.Contains(l.Period)).ToList();
            var result = new List<object>() {
                new {
                    Id = -1,
                    Name = "Не указано"
                    }
            };
            foreach (var o in okso)
            {
                if (o.Direction != null)
                {
                    var oksoElem = new
                    {
                        Id = o.DirectionId,
                        Name = (o.Direction.okso ?? "") + ' ' + (o.Direction.title ?? "") + " ("+ o.Direction.standard + ")"
                    };

                    if (!result.Contains(oksoElem))
                        result.Add(oksoElem);
                }
            }
            return Json(
                new
                {
                    data = result
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult ProfileList(int id)
        {
            var limits = db.ContractPeriods.Where(p => p.ContractId == id).Select(p => p.Limits).ToList();
            var profiles = new List<object>()
            {
                new {
                    Id = -1,
                    Name = "Не указано"
                    }
            };
            foreach (var l in limits)
            {
                var _limits = l.Where(j => j.ProfileId != null).Select(j => new
                {
                    Id = j.ProfileId,
                    Name = j.Profile.CODE + " " + j.Profile.NAME,
                }).ToList();
                foreach (var _l in _limits)
                {
                    if (!profiles.Contains(_l))
                        profiles.Add(_l);
                }

            }

            return Json(
                new
                {
                    data = profiles
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Qualifications()
        {
            var qualifications = new List<Qualification>() {
                new Qualification() { Name = "Не указано" }
            };
            qualifications.AddRange(db.Qualifications.ToList());

            return Json(
                new
                {
                    data = qualifications
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult CourseList()
        {
            var list = new List<object>();
            for (int i = 0; i <= 6; i++)
                list.Add(new { Course = i });

            return Json(
                new
                {
                    data = list
                },
                new JsonSerializerSettings()
            );
        }

        #endregion

        public ActionResult ContractStudents(int? limitId, int? contractKsId, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                SortRules sortRules = SortRules.Deserialize(sort);

                var admissions = new List<PracticeAdmissionCompany>();

                if (limitId != null && contractKsId == null) // если студенты по д/с договору по определенному лимиту
                {
                    ContractLimit limit = db.ContractLimits.FirstOrDefault(l => l.Id == limitId);

                    if (limit == null)
                        return new StatusCodeResult(StatusCodes.Status400BadRequest);

                    admissions = GetAdmissionsOnLimit(limit, null);

                }
                else if (limitId == null && contractKsId != null) // если студенты по к/с договору
                {
                    admissions = db.PracticeAdmissionCompanys.Where(a => a.ContractId == contractKsId).ToList();
                }

                var rows = admissions.Select(s =>
                    {
                        var plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == s.Practice.DisciplineUUID);
                        if (plan != null && !plan.remove)
                        {
                            return new
                            {
                                Student = s.Practice.Student.Person.FullName(),
                                Group = s.Practice.Group.Name,
                                Direction = s.Practice.Group.Profile.Direction.OksoAndTitle,
                                Profile = s.Practice.Group.Profile.OksoAndTitle,
                                PracticeName = plan.disciplineTitle,
                                PracticeType = plan.additionalType,
                                Dates = $"{s.Practice.BeginDate?.ToShortDateString()} - {s.Practice.EndDate?.ToShortDateString()}",
                                StatusName = s.StatusName,
                                id = s.PracticeId,
                                StudentId = s.Practice.StudentId,
                                groupId = s.Practice.Group.GroupId,
                                year = s.Practice.Year,
                                semestrId = s.Practice.SemesterId,
                                disciplineUID = s.Practice.DisciplineUUID
                            };
                        }
                        return null;
                    }
                ).Where(s=>s!= null).AsQueryable().Where(FilterRules.Deserialize(filter)).OrderBy(sortRules.FirstOrDefault());

                return Json(
                    new
                    {
                        data = rows,
                        total = rows.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                if (limitId != null && contractKsId == null) // если студенты по д/с договору по определенному лимиту 
                {
                    ContractLimit limit = db.ContractLimits.FirstOrDefault(l => l.Id == limitId);

                    ViewBag.LimitId = limitId;
                    ViewBag.ContractKsId = contractKsId;

                    ViewBag.CompanyId = limit.Period.Contract.CompanyId;
                    ViewBag.ContractId = limit.Period.ContractId;

                    ViewBag.Title = $"{limit.Period.Contract.Company.Name}, договор {limit.Period.Contract.Number}";
                }

                if (limitId == null && contractKsId != null) // если студенты по к/с договору
                {
                    Contract contract = db.Contracts.FirstOrDefault(c => c.Id == contractKsId);

                    ViewBag.LimitId = limitId;
                    ViewBag.ContractKsId = contractKsId;

                    ViewBag.CompanyId = contract.CompanyId;
                    ViewBag.ContractId = contractKsId;

                    ViewBag.Title = $"{contract.Company.Name}, договор {contract.Number}";
                }

                ViewBag.Status = JsonConvert.SerializeObject(new List<object>(){
                    new { StatusName = "Все", StatusValue = "" },
                    new { StatusName = "Согласовано", StatusValue = "Согласовано" },
                    new { StatusName = "Отклонено", StatusValue = "Отклонено" },
                    new { StatusName = "Формируется", StatusValue = "Формируется" }
                });

                return View(viewName: "LimitStudents");
            }
        }

       private List<PracticeAdmissionCompany> GetAdmissionsOnLimit(ContractLimit limit, AdmissionStatus? status = AdmissionStatus.Admitted)
        {
            if (limit == null)
                return null;
            var admissions = db.PracticeAdmissionCompanys.Where(a => a.Contract != null)
              .Where(a =>
                  a.Contract.Id == limit.Period.ContractId
               && a.Practice.SemesterId == limit.Period.SemesterId
               && a.Practice.Year == limit.Period.Year
               && (status == null || a.Status == status)
               && (limit.ProfileId == null || limit.ProfileId == a.Practice.Group.ProfileId)
               && ((limit.DirectionId == null && !a.Contract.Periods.Any(p => p.Limits.Select(l => l.DirectionId).Contains(a.Practice.Group.Profile.DIRECTION_ID))) || limit.DirectionId == a.Practice.Group.Profile.DIRECTION_ID)
               && (limit.Course == 0 || limit.Course == a.Practice.Group.Course)
               && (limit.QualificationName == null || limit.QualificationName == a.Practice.Group.Qual)
               ).ToList();

            return admissions;
        }

        private void AddLimits(int contractId, List<ContractPeriod> newPeriods)
        {
            var periodIds = db.ContractPeriods.Where(p => p.ContractId == contractId).Select(p => p.Id).ToList();
            var directions = db.ContractLimits.Where(l => periodIds.Contains(l.ContractPeriodId)).Select(l => new { l.ProfileId, l.DirectionId }).Distinct().ToList();
            if (directions.Count() > 0)
            {
                foreach (var d in directions)
                {
                    foreach (var period in newPeriods)
                    {
                        AddEmptyLimit(period.Id, directionId: d.DirectionId, profileId: d.ProfileId);
                    }
                }
            }
            else
            {
                foreach (var period in newPeriods)
                {
                    AddEmptyLimit(period.Id, directionId: null, profileId: null);
                }
            }
        }

        private void AddLimits(int contractId, ContractPeriod newPeriod)
        {
            var periodIds = db.ContractPeriods.Where(p => p.ContractId == contractId).Select(p => p.Id).ToList();
            var directions = db.ContractLimits.Where(l => periodIds.Contains(l.ContractPeriodId)).Select(l => new { l.ProfileId, l.DirectionId }).Distinct().ToList();
            if (directions.Count() > 0)
            {
                foreach (var d in directions)
                {
                    AddEmptyLimit(newPeriod.Id, directionId: d.DirectionId, profileId: d.ProfileId);
                }
            }
            else
            {
                AddEmptyLimit(newPeriod.Id, directionId: null, profileId: null);
            }
        }

        private void AddEmptyLimit(int periodId, string directionId, string profileId)
        {
            db.ContractLimits.Add(new ContractLimit()
            {
                Limit = 0,
                ContractPeriodId = periodId,
                Course = 0,
                DirectionId = directionId,
                ProfileId = profileId,
                QualificationName = null
            });
            db.SaveChanges();
            
            UpdateModuleSpecialities(db.ContractPeriods.FirstOrDefault(p => p.Id == periodId).ContractId);
        }

        private string StudentsInfo(string studentsStr, IEnumerable<PracticeAdmissionCompany> admissions)
        {
            admissions = admissions.Distinct();
            foreach (var admission in admissions)
            {
                var studentName = admission.Practice.Student.Person.ShortName();
                var groupName = admission.Practice.Group.Name;
                studentsStr += studentName + ", " + groupName + "<br>";
            }
            return studentsStr;
        }

        /// <summary>
        /// Обновить направления (specialities) в Modules, если это договор по проектному обучению
        /// </summary>
        /// <param name="contractId"></param>
        private void UpdateModuleSpecialities(int? contractId)
        {
            if (!db.Contracts.Any(c => c.Id == contractId && c.Company.Source == Source.Project))
                return;

            var module = db.Projects.FirstOrDefault(p => p.ContractId == contractId.Value).Module;
            if (module != null)
            {
                var directions = db.ContractLimits.Where(l => l.Period.ContractId == contractId && l.Direction != null)
                    .Select(l => l.Direction.okso)
                    .Distinct()
                    .ToList();

                module.specialities = string.Join(", ", directions);
                db.SaveChanges();
            }
        }

        public ActionResult DownloadContractPeriodFile(int fileId)
        {
            var file = db.FileStorage.FirstOrDefault(f=>f.Id== fileId);
            if (file != null)
                return File(Urfu.Its.Web.DataContext.FileStorageHelper.GetBytes(file.Id), System.Net.Mime.MediaTypeNames.Application.Octet, file.FileNameForUser);
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
    }
}