using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.Practice;

namespace Urfu.Its.Frames.Controllers
{
    public static class DocumentType
    {
        public static string Dogovor = "Dogovor";
        public static string Rezume = "Rezume";
        public static string Letter = "Letter";
        public static string Comand = "Comand";
        public static string Transfer = "Transfer";
        public static string Notice = "Notice";
        public static string Task = "Task";
        public static string Report = "Report";
        public static string Review = "Review";
    }

    public class ScanListVM
    {
        private ApplicationDbContext _db;

        private Practice _practice;
        private Student _student;

        public int PracticeID { get; set; }

        public string DisciplineTitle { get; set; }

        public int Year { get; set; }
        public string Semester { get; set; }

        public string YearInfo => $"{Year}/{Year % 100 + 1} уч.год";

        public string PeriodInfo()
        {
            if (_practice.BeginDate != null || _practice.EndDate != null)
                return $"c {_practice.BeginDate:dd.MM.yyyy г.} по {_practice.EndDate:dd.MM.yyyy г.} ";
            else
                return "";
        }


        public List<ScanVM> BeforeDocuments { get; set; }
        public List<ScanVM> AfterDocuments { get; set; }
        public List<ScanVM> DistantDocuments { get; set; }

        public ScanListVM()
        {
        }

        public ScanListVM(ApplicationDbContext db, int practiceID)
        {
            _db = db;

            _practice = _db.Practices.Include(p => p.Documents).FirstOrDefault(p => p.Id == practiceID);

            if (_practice == null)
                throw new Exception("Не найдена практика");

            _student = _db.Students.FirstOrDefault(s => s.Id == _practice.StudentId);

            Fill();
        }

        private void Fill()
        {
            PracticeID = _practice.Id;

            var plan = _db.Plans.FirstOrDefault(p => p.disciplineUUID == _practice.DisciplineUUID);

            DisciplineTitle = plan.disciplineTitle;

            Year = _practice.Year;
            Semester = _practice.Semester.Name;

            BeforeDocuments = new List<ScanVM>
            {
                GetScanVM(PracticeDocumentType.Contract, _practice),
                GetScanVM(PracticeDocumentType.Resume, _practice),
                GetScanVM(PracticeDocumentType.Referral, _practice),
                GetScanVM(PracticeDocumentType.Trip, _practice),
                GetScanVM(PracticeDocumentType.Postponement, _practice),
                GetScanVM(PracticeDocumentType.Task, _practice),
                GetScanVM(PracticeDocumentType.Rejection, _practice)
            };

            AfterDocuments = new List<ScanVM>
            {
                GetScanVM(PracticeDocumentType.Notice, _practice),
                GetScanVM(PracticeDocumentType.Report, _practice),
                GetScanVM(PracticeDocumentType.Review, _practice),
            };

            DistantDocuments = new List<ScanVM>
            {
                GetScanVM(PracticeDocumentType.DistantContract, _practice),
                GetScanVM(PracticeDocumentType.DistantReferral, _practice),
                GetScanVM(PracticeDocumentType.DistantTask, _practice),
            };
        }

        private ScanVM GetScanVM(PracticeDocumentType documentType, Practice practice)
        {
            var descriptor = PracticFileDescriptor.Get(documentType);
            var document = practice.Documents.FirstOrDefault(d => d.DocumentType == documentType);

            if (document?.FileStorageId != null)
                _db.Entry(document).Reference(d => d.FileStorage).Load();

            return new ScanVM
            {
                TypeId = (int)documentType,
                TypeName = descriptor.TypeName + (documentType == PracticeDocumentType.Rejection ? " (в свободной форме)" : ""),
                DocumentId = document?.Id,
                DocumentName = document?.FileStorageId != null ? document.FileStorage.FileNameForUser : "",
                Status = PracticeDocumentViewModel.GetStatus(document?.Status ?? AdmissionStatus.Indeterminate),
                Comment = document?.Comment,
                Date = document?.FileStorageId != null ? document.FileStorage.Date.ToShortDateString() : null
            };
        }

    }

    public class ScanVM
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public int? DocumentId { get; set; }
        public string DocumentName { get; set; }

        public string Status { get; set; }
        public string Comment { get; set; }

        public string StatusComment()
        {
            return string.IsNullOrWhiteSpace(Comment)
                ? Status
                : $"{Status} ({Comment})";
        }
        public string Date { get; set; }
    }
}