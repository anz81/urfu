using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
//using Urfu.Its.Practices;
using System.Collections.Generic;
using Urfu.Its.Web.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestSharp;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Urfu.Its.Practices;

namespace Urfu.Its.Frames.Controllers
{
    [Authorize]
    public class PracticeController : Controller
    {
        private bool isTestAuthorize = false;

        //private string StudentID()
        //{
        //return "studen18hc2jg0000ldha1e9eqc6og0g"; //Терентьева
        //return "studen18ggl5g0000kg4acroq4vjod6c"; //Амир
        //return "studen18hc2jg0000ls61ncq3j2geju8"; //Арина
        //}

        //public void SignIn()
        //{
        //    if (!Request.IsAuthenticated)
        //    {
        //        HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" },
        //            WsFederationAuthenticationDefaults.AuthenticationType);
        //    }
        //}

        //public void SignOut()
        //{
        //    string callbackUrl = Url.Action("SignOutCallback", "Home", routeValues: null, protocol: Request.Url.Scheme);

        //    HttpContext.GetOwinContext().Authentication.SignOut(
        //        new AuthenticationProperties { RedirectUri = callbackUrl },
        //        WsFederationAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        //}

        //public ActionResult SignOutCallback()
        //{
        //    if (Request.IsAuthenticated)
        //    {
        //        // Redirect to home page if the user is authenticated.
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View();
        //}

        private void Log(ApplicationDbContext db, int practiceId, string message)
        {
            var practice = db.Practices.FirstOrDefault(p => p.Id == practiceId);
            Log(db, practice, message);
        }

        private void Log(ApplicationDbContext db, Practice practice, string message)
        {

            var student = practice.Student.Person;
            var group = practice.Group;
            var discipline = db.Plans.FirstOrDefault(p => p.disciplineUUID == practice.DisciplineUUID);

            Logger.Info($"ЛК Практика \"{discipline.disciplineTitle}\"(Id={practice.Id}): {message}, {student.FullName()}, {group.Name}");
        }

        [Authorize]
        public ActionResult LK()
        {
            using (var db = new ApplicationDbContext())
            {
                var studentIds = UserSecurity.StudentIDs(User, db);

                Response.Cookies.Append("quest", "");

                var students = db.Students.Where(s => studentIds.Contains(s.Id)).ToList();
                if (students.Count == 0)
                    return View("Message", new Message("Модуль 'Учебная и производственная практика' для вашей группы недоступен. Отсутствует модульный учебный план"));

                return View("ListPractices", new PracticeMainList(db, studentIds));
            }
        }

        //Вход из ЛК
        public ActionResult Index()
        {
            Response.Cookies.Append(".AspNetCore.Cookies", "");
            Response.Cookies.Append("quest", "practice");

            return RedirectToAction("LK");
        }

        //Вход из ITS
        [Authorize]
        public ActionResult Student(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var studentIds = UserSecurity.StudentIDs(User, db);
                var isAdmin = UserSecurity.IsAdmin(User.GetADName(), db);

                Logger.Info($"Practice Student studentId={string.Join(", ", studentIds)} isAdmin={isAdmin}");

                var student = db.Students.FirstOrDefault(s => s.Id == id);
                if (student == null)
                    return View("Message", new Message("Модуль 'Учебная и производственная практика' для вашей группы не доступен. Отсутствует модульный учебный план"));

                if (isTestAuthorize || isAdmin || studentIds.Contains(id))
                {
                    return View("ListPractices", new PracticeMainList(db, id));
                }

                return new UnauthorizedResult();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create(string studentID, string groupID, int year, int semesterID, string disciplineUID)
        {
            //var studentId = StudentID();

            using (var db = new ApplicationDbContext())
            {
                var studentIds = UserSecurity.StudentIDs(User, db);
                var isAdmin = UserSecurity.IsAdmin(User.GetADName(), db);

                if (isTestAuthorize || isAdmin || studentIds.Contains(studentID))
                {
                    return View("Practice", new PracticeVM(db, studentID, groupID, year, semesterID, disciplineUID));
                }

                return new UnauthorizedResult();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Practice(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var practice = db.Practices.FirstOrDefault(p => p.Id == id);
                if (practice == null)
                    return new NotFoundResult(); // ($"Не найдена практика с ID={id}");

                var studentIds = UserSecurity.StudentIDs(User, db);
                var isAdmin = UserSecurity.IsAdmin(User.GetADName(), db);

                Logger.Info($"Practice studentId={string.Join(", ", studentIds)} isAdmin={isAdmin}");

                if (isTestAuthorize || isAdmin || studentIds.Contains(practice?.StudentId))
                {
                    var model = new PracticeVM(db, id);
                    if (Request.Cookies["inform"] != null)
                        model.ShowMessage = true;
                    return View("Practice", model);
                }

                return new UnauthorizedResult();
            }
        }


        //[HttpPost]
        //public ActionResult Practice(PracticeVM practice)
        //{
        //    var studentId = StudentID();

        //    using (var db = new ApplicationDbContext())
        //    {
        //        SaveAdmission(db, practice);

        //        return View("Practice", new PracticeVM(db, practice.PracticeID));
        //    }
        //}

        [HttpPost]
        public ActionResult Practice(PracticeVM practice, string operation)
        {
            //var studentId = StudentID();

            using (var db = new ApplicationDbContext())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        switch (operation)
                        {
                            case "Admission":
                                SaveAdmission(db, practice);
                                break;
                            case "Save":
                                SavePractice(db, practice);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    //Response.Cookies["inform"]["x"] = "yes";
                    return RedirectToAction("Practice", new { id = practice.PracticeID });
                }

                return View("Practice", new PracticeVM(db, practice.PracticeID));
            }
        }


        [HttpPost]
        public ActionResult Contract(PracticeVM practice)
        {
            using (var db = new ApplicationDbContext())
            {
                if (string.IsNullOrEmpty(practice.PersonalContract.CompanyName))
                {
                    ViewBag.Message = "Название предпрития не может быть пустым!";
                    return View("Practice", new PracticeVM(db, practice.PracticeID));
                }

                try
                {
                    AddContract(db, practice);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                return RedirectToAction("Practice", new { id = practice.PracticeID });
            }
            //return View("Practice", new PracticeVM(db, practice.PracticeID));
        }


        [HttpPost]
        public ActionResult AddContratToCompany(PersonalContractVM ksContract)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    var practice = db.Practices.FirstOrDefault(p => p.Id == ksContract.PracticeID);
                    if (practice == null)
                        return new NotFoundResult();
                    var company = db.Companies.FirstOrDefault(c => c.Id == ksContract.CompanyId);
                    if (company != null)
                    {
                        var contract = new Contract();//db.Contracts.Create();
                        contract.Company = company;
                        contract.Comment = ksContract.Comment;
                        contract.Director = ksContract.Director;
                        contract.DirectorInitials = ksContract.DirectorInitials;
                        contract.DirectorGenitive = ksContract.DirectorGenetive;
                        contract.PostOfDirector = ksContract.PostOfDirector;
                        contract.PostOfDirectorGenitive = ksContract.PostOfDirectorGenetive;
                        contract.PersonInCharge = ksContract.PersonInCharge;
                        contract.PersonInChargeInitials = ksContract.PersonInChargeInitials;
                        contract.PostOfPersonInCharge = ksContract.PostOfPersonInCharge;
                        contract.PhoneNumber = ksContract.Phone;
                        contract.IsShortDated = true;
                        contract.Number = db.CreateContractKsNumber(practice.Year, ksContract.InstituteTitle);
                        contract.Year = practice.Year;
                        contract.SerialNumber = db.GetNextSerialNumberKsContract(practice.Year);
                        contract.Email = ksContract.PersonInChargeEmail;


                        var period = new ContractPeriod(); //db.ContractPeriods.Create();
                        period.Contract = contract;
                        period.Year = practice.Year;
                        period.SemesterId = practice.SemesterId;


                        var admission = practice.AdmissionCompanys.FirstOrDefault();
                        if (admission == null)
                        {
                            //admission = db.PracticeAdmissionCompanys.Create();
                            db.PracticeAdmissionCompanys.Add(admission);
                        }

                        admission.PracticeId = practice.Id;
                        admission.Contract = contract;
                        admission.CreateDate = DateTime.Now;
                        admission.Status = AdmissionStatus.Indeterminate;

                        db.SaveChanges();
                        Logger.Info($"Добавлено предприятие id {company.Id} {company.Name} Договор id {contract.Id} Номер {contract.Number} uuid {practice.StudentId}");
                        Log(db, practice, "Подана заявка на предприятие по персональному договору");

                    }

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            return RedirectToAction("Practice", new { id = ksContract.PracticeID });
        }

        [HttpPost]
        public ActionResult Company(PracticeVM practice, int contractPeriodID)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    if (practice.Agreement)
                        SaveAdmissionCompany(db, practice, contractPeriodID);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                return RedirectToAction("Practice", new { id = practice.PracticeID });

                //return View("Practice", new PracticeVM(db, practice.PracticeID));
            }
        }

        [HttpGet]
        public ActionResult GetLocations(int? parentId, int level, string id, string targetProperty)
        {
            using (var db = new ApplicationDbContext())
            {
                var locations = new List<SelectListItem>();
                if (parentId != null)
                {
                    locations = db.CompanyLocations.Where(l => l.ParentId == parentId && l.Level == level).Select(l => new SelectListItem()
                    {
                        Text = l.Name,
                        Value = l.Id.ToString()
                    }).OrderBy(r => r.Text).ToList();
                }
                locations.Insert(0, new SelectListItem { Value = null, Text = "" });
                return PartialView(new LocationVM() { Items = locations, listId = id, targetProperty = targetProperty });
            }
        }

        [HttpGet]
        public ActionResult Document(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var practice = db.Practices.FirstOrDefault(p => p.Id == id);
                if (practice == null)
                    return new NotFoundResult();

                var studentIds = UserSecurity.StudentIDs(User, db);
                var isAdmin = UserSecurity.IsAdmin(User.GetADName(), db);

                if (isTestAuthorize || isAdmin || studentIds.Contains(practice?.StudentId))
                {
                    return View("Document", new DocumentListVM(db, id));
                }

                return new UnauthorizedResult();

            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Scan(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    var practice = db.Practices.FirstOrDefault(p => p.Id == id);
                    if (practice == null)
                        return new NotFoundResult();

                    var studentIds = UserSecurity.StudentIDs(User, db);
                    var isAdmin = UserSecurity.IsAdmin(User.GetADName(), db);

                    if (isTestAuthorize || isAdmin || studentIds.Contains(practice?.StudentId))
                    {
                        return View("Scan", new ScanListVM(db, id));
                    }

                    return new UnauthorizedResult();
                }
                catch (Exception ex)
                {
                    Logger.Info($"ЛК Практика ошибка на странице Сканы");
                    Logger.Error(ex);

                    return new UnauthorizedResult();
                }
            }
        }


        private void SavePractice(ApplicationDbContext db, PracticeVM practiceVm)
        {
            var practice = db.Practices.First(p => p.Id == practiceVm.PracticeID);
            practice.FinishTheme = practiceVm.FinishTheme;
            db.SaveChanges();

            Log(db, practice, "Изменена тема");
        }

        private void SaveAdmission(ApplicationDbContext db, PracticeVM practiceVm)
        {
            var practice = db.Practices.Include(p => p.Admissions).First(p => p.Id == practiceVm.PracticeID);
            practice.FinishTheme = practiceVm.FinishTheme;

            //удалим не нужные, пишем только по 1
            var overs = practice.Admissions.Skip(1).ToList();
            foreach (var a in overs)
                practice.Admissions.Remove(a);

            var admission = practice.Admissions.FirstOrDefault();
            if (admission == null)
            {
                //admission = db.PracticeAdmissions.Create();
                db.PracticeAdmissions.Add(admission);
            }

            admission.PracticeId = practiceVm.PracticeID;
            admission.PracticeThemeId = practiceVm.ThemaID ?? 0;
            admission.TeacherPKey = practiceVm.TeacherID;
            admission.TeacherPKey2 = practiceVm.Teacher2ID;
            admission.Subdivision = practiceVm.Subdivision;
            admission.CreateDate = DateTime.Now;
            admission.Status = AdmissionStatus.Indeterminate;
            admission.ReasonOfDeny = null;


            db.SaveChanges();

            Log(db, practice, "Подана заявка в УрФУ");
        }

        private void SaveAdmissionCompany(ApplicationDbContext db, PracticeVM practiceVm, int contractPeriodID)
        {
            var practice = db.Practices.Include(p => p.AdmissionCompanys).First(p => p.Id == practiceVm.PracticeID);

            var contract = db.ContractPeriods.First(c => c.Id == contractPeriodID);

            //удалим не нужные, пишем только по 1
            var overs = practice.AdmissionCompanys.Skip(1).ToList();
            foreach (var a in overs)
                db.PracticeAdmissionCompanys.Remove(a);


            var admission = practice.AdmissionCompanys.FirstOrDefault();
            if (admission == null)
            {
                //admission = db.PracticeAdmissionCompanys.Create();
                db.PracticeAdmissionCompanys.Add(admission);
            }

            admission.PracticeId = practiceVm.PracticeID;
            admission.ContractId = contract.ContractId;
            admission.CreateDate = DateTime.Now;
            admission.Status = AdmissionStatus.Indeterminate;
            admission.Agreement = practiceVm.Agreement;

            db.SaveChanges();
            Log(db, practice, "Подана заявка на предприятие");

            //Отправка заявки в очередь для ЛКП. Отправлять заявки с 2020 года — начало работы ЛКП.
            if (practice.Year >= 2020)
            {
                Task.Run(() => PracticeAdmissionPublication.PublishPracticeAdmission(practice.Id, contract.ContractId));
            }

        }

        public ActionResult AutoCompleteSearch(string term)
        {
            using (var db = new ApplicationDbContext())
            {
                var companies = db.Companies.Where(c => (c.Name.Contains(term) || c.ShortName.Contains(term)) && c.IsConfirmed)
                    .Select(c => new { Value = c.Id, Name = c.Name })
                    .Distinct().ToList();
                return Json(companies, new JsonSerializerSettings());
            }
        }

        [HttpGet]
        public ActionResult GetCompanyInfo(int Id, int practiceId, string instituteTitle)
        {
            using (var db = new ApplicationDbContext())
            {
                var company = db.Companies.FirstOrDefault(c => c.Id == Id);
                var practice = db.Practices.FirstOrDefault(p => p.Id == practiceId);
                if (company != null && practice != null)
                {
                    var companyInfo = new PersonalContractVM();
                    companyInfo.CompanyId = company.Id;
                    companyInfo.ShortName = company.ShortName;
                    companyInfo.Address = company.Address;
                    companyInfo.Director = company.Director;
                    companyInfo.DirectorInitials = company.DirectorInitials;
                    companyInfo.PostOfDirector = company.PostOfDirector;
                    companyInfo.PostOfDirectorGenetive = company.PostOfDirectorGenitive;
                    companyInfo.DirectorGenetive = company.DirectorGenitive;
                    companyInfo.CompanyPhone = company.CompanyPhoneNumber;
                    companyInfo.Email = company.Email;
                    companyInfo.Site = company.Site;
                    companyInfo.INN = company.INN;
                    companyInfo.PersonInCharge = company.PersonInCharge;
                    companyInfo.PersonInChargeInitials = company.PersonInChargeInitials;
                    companyInfo.PostOfPersonInCharge = company.PostOfPersonInCharge;
                    companyInfo.Phone = company.PhoneNumber;
                    companyInfo.Location = company.Location?.FullLocation();
                    companyInfo.Comment = company.Comment;
                    companyInfo.PracticeID = practiceId;
                    companyInfo.InstituteTitle = instituteTitle;


                    return PartialView(companyInfo);
                }
                return NotFound("Предприятие не найдено");
            }
        }

        private void AddContract(ApplicationDbContext db, PracticeVM practiceVm)
        {
            var practice = db.Practices.FirstOrDefault(p => p.Id == practiceVm.PracticeID);

            //удалим не нужные, пишем только по 1         
            var overs = practice.AdmissionCompanys.Skip(1).ToList();
            foreach (var a in overs)
                db.PracticeAdmissionCompanys.Remove(a);

            var company = new Company(); // db.Companies.Create();

            company.Name = practiceVm.PersonalContract.CompanyName;
            company.ShortName = practiceVm.PersonalContract.ShortName;
            company.Address = practiceVm.PersonalContract.Address;
            company.Comment = practiceVm.PersonalContract.Comment;
            company.Director = practiceVm.PersonalContract.Director;
            company.DirectorInitials = practiceVm.PersonalContract.DirectorInitials;
            company.DirectorGenitive = practiceVm.PersonalContract.DirectorGenetive;
            company.PostOfDirector = practiceVm.PersonalContract.PostOfDirector;
            company.PostOfDirectorGenitive = practiceVm.PersonalContract.PostOfDirectorGenetive;
            company.CompanyPhoneNumber = practiceVm.PersonalContract.CompanyPhone;
            company.Email = practiceVm.PersonalContract.Email;
            company.Site = practiceVm.PersonalContract.Site;
            company.INN = practiceVm.PersonalContract.INN;
            company.PersonInCharge = practiceVm.PersonalContract.PersonInCharge;
            company.PersonInChargeInitials = practiceVm.PersonalContract.PersonInChargeInitials;
            company.PostOfPersonInCharge = practiceVm.PersonalContract.PostOfPersonInCharge;
            company.PhoneNumber = practiceVm.PersonalContract.Phone;

            company.CompanyLocationId = practiceVm.PersonalContract.CityId ?? (practiceVm.PersonalContract.RegionId ?? practiceVm.PersonalContract.CountryId);

            var contract = new Contract(); // db.Contracts.Create();
            contract.Company = company;
            contract.Comment = practiceVm.PersonalContract.Comment;
            contract.Director = practiceVm.PersonalContract.Director;
            contract.DirectorInitials = practiceVm.PersonalContract.DirectorInitials;
            contract.DirectorGenitive = practiceVm.PersonalContract.DirectorGenetive;
            contract.PostOfDirector = practiceVm.PersonalContract.PostOfDirector;
            contract.PostOfDirectorGenitive = practiceVm.PersonalContract.PostOfDirectorGenetive;
            contract.PersonInCharge = practiceVm.PersonalContract.PersonInCharge;
            contract.PersonInChargeInitials = practiceVm.PersonalContract.PersonInChargeInitials;
            contract.PostOfPersonInCharge = practiceVm.PersonalContract.PostOfPersonInCharge;
            contract.PhoneNumber = practiceVm.PersonalContract.Phone;
            contract.IsShortDated = true;
            contract.Number = db.CreateContractKsNumber(practice.Year, practiceVm.InstituteTitle);
            contract.Year = practice.Year;
            contract.SerialNumber = db.GetNextSerialNumberKsContract(practice.Year);
            contract.Email = practiceVm.PersonalContract.PersonInChargeEmail;

            var period = new ContractPeriod();//db.ContractPeriods.Create();
            period.Contract = contract;
            period.Year = practice.Year;
            period.SemesterId = practice.SemesterId;

            var admission = practice.AdmissionCompanys.FirstOrDefault();
            if (admission == null)
            {
                //admission = db.PracticeAdmissionCompanys.Create();
                db.PracticeAdmissionCompanys.Add(admission);
            }

            admission.PracticeId = practiceVm.PracticeID;
            admission.Contract = contract;
            admission.CreateDate = DateTime.Now;
            admission.Status = AdmissionStatus.Indeterminate;

            db.Companies.Add(company);

            db.SaveChanges();
            Logger.Info($"Добавлено предприятие id {company.Id} {company.Name} Договор id {contract.Id} Номер {contract.Number} uuid {practice.StudentId}");
            Log(db, practice, "Подана заявка на предприятие по персональному договору");
        }


        //private void SaveAdmissionCompany(ApplicationDbContext db, PracticeVM practiceVm)
        //{
        //    var admission = db.PracticeAdmissions.Create();

        //    admission.PracticeId = practiceVm.PracticeID;
        //    admission.PracticeThemeId = practiceVm.ThemaID ?? 0;
        //    admission.TeacherPKey = practiceVm.TeacherID;
        //    admission.CreateDate = DateTime.Now;
        //    admission.Status = AdmissionStatus.Indeterminate;

        //    db.PracticeAdmissions.Add(admission);

        //    var practice = db.Practices.First(p => p.Id == practiceVm.PracticeID);
        //    practice.FinishTheme = practiceVm.FinishTheme;

        //    db.SaveChanges();
        //}

        public ActionResult GetTemplate(int practiceId, PracticeDocumentType type)
        {
            var file = PracticFileDescriptor.Get(type);

            var documents = new PracticeDocuments();

            using (var stream = documents.GetDocuments(type, practiceId))
            {
                var bytes = stream.ToArray();
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
            }
        }

        public ActionResult UploadDocument(int practiceId, PracticeDocumentType? type)
        {
            if (Request.Form.Files.Count > 0)
            {
                using (var db = new ApplicationDbContext())
                {
                    using (var tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var file = Request.Form.Files[0];

                            if (type == null)
                                Log(db, practiceId, $"Попытка загрузить документ {System.IO.Path.GetFileName(file.FileName)} с неопознанным типом");
                            else
                            {
                                var document = db.PracticeDocuments.Where(d => d.PracticeId == practiceId && d.DocumentType == type.Value).FirstOrDefault();
                                if (document == null)
                                {
                                    //document = db.PracticeDocuments.Create();
                                    document.PracticeId = practiceId;
                                    document.DocumentType = type.Value;

                                    db.PracticeDocuments.Add(document);
                                }

                                int? id = FileStorageHelper.SaveFile(file, FileCategory.Practice, folder: $"{document.Practice.Group.Profile.Direction.okso}_{document.Practice.Year}", comment: $"{("PracticeId: " + document.PracticeId + " DocumentType: " + document.DocumentType)}", id: document.FileStorageId);

                                document.FileStorageId = id;

                                db.SaveChanges();

                                tran.Commit();

                            }
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();

                            Logger.Error(ex);
                        }
                    }
                }
            }

            return RedirectToAction("Scan", new { id = practiceId });
        }

        [HttpPost]
        public ActionResult DeleteDocument(int practiceId, PracticeDocumentType type)
        {
            using (var db = new ApplicationDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        var document = db.PracticeDocuments.Where(d => d.PracticeId == practiceId && d.DocumentType == type).FirstOrDefault();
                        if (document == null || document.Status == AdmissionStatus.Admitted)
                        {
                            return NotFound("Документ не найден или согласован"); ;
                        }

                        if (document.FileStorageId != null)
                            FileStorageHelper.RemoveFile((int)document.FileStorageId);

                        db.PracticeDocuments.Remove(document);
                        db.SaveChanges();
                        tran.Commit();
                        var descr = PracticFileDescriptor.Get(document.DocumentType);
                        Log(db, document.PracticeId, $"Удален документ {descr.Type}");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Logger.Error(ex);
                    }


                }
                //return View("Scan", new ScanListVM(db, practiceId));
                return Scan(practiceId);
                // return RedirectToAction("Scan", new { id = practiceId });
            }

        }

        [Authorize]
        public ActionResult GetScan(int documentId)
        {
            using (var db = new ApplicationDbContext())
            {
                var studentIds = UserSecurity.StudentIDs(User, db);
                var isAdmin = UserSecurity.IsAdmin(User.GetADName(), db);

                var document = db.PracticeDocuments.Include(d => d.Practice).Include(d => d.FileStorage).FirstOrDefault(d => d.Id == documentId);

                Logger.Info($"GetScan practice.StudentId={document?.Practice.StudentId} studentId={string.Join(", ", studentIds)} isAdmin={isAdmin} AdName={User.GetADName()}");

                if (isTestAuthorize || isAdmin || studentIds.Contains(document?.Practice.StudentId))
                {
                    if (document == null || document.FileStorageId == null)
                    {
                        return NotFound("Document not found");
                    }

                    return File(FileStorageHelper.GetBytes((int)document.FileStorageId), System.Net.Mime.MediaTypeNames.Application.Octet, document.FileStorage.FileNameForUser.ToDownloadFileName());
                }

                return new UnauthorizedResult();
            }
        }

        public ActionResult GetContract(int contractId)
        {
            using (var db = new ApplicationDbContext())
            {
                var contract = db.Contracts.Include(c => c.FileStorage).FirstOrDefault(c => c.Id == contractId);

                if (contract == null || contract.FileStorageId == null)
                {
                    return NotFound("Document not found");
                }

                return File(FileStorageHelper.GetBytes((int)contract.FileStorageId), System.Net.Mime.MediaTypeNames.Application.Pdf, "Договор №" + contract.Number.Replace('/', '_') + ".pdf");

            }
        }

        public ActionResult GetCompanyDocument(int contractId)
        {
            using (var db = new ApplicationDbContext())
            {
                var contract = db.Contracts.Include(c => c.Company).Include(c => c.FileStorage).FirstOrDefault(c => c.Id == contractId);

                if (contract?.Company?.FileStorageId == null)
                {
                    return NotFound("Document not found");
                }

                return File(FileStorageHelper.GetBytes((int)contract.Company.FileStorageId), System.Net.Mime.MediaTypeNames.Application.Pdf, contract.FileStorage.FileNameForUser.ToDownloadFileName());

            }
        }

        public ActionResult GetDocument(int fileId)
        {
            using (var db = new ApplicationDbContext())
            {
                var file = db.FileStorage.FirstOrDefault(f => f.Id == fileId);
                if (file == null)
                {
                    return NotFound("Document not found");
                }
                return File(FileStorageHelper.GetBytes(file.Id), System.Net.Mime.MediaTypeNames.Application.Octet, file.FileNameForUser.ToDownloadFileName());
            }
        }

    }

}

