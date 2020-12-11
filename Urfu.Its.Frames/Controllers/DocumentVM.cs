using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.Web.DataContext;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Frames.Controllers
{
    public class DocumentListVM
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


            public List<DocumentVM> BeforeDocuments { get; set; }
            public List<DocumentVM> AfterDocuments { get; set; }
            public List<DocumentVM> DistantDocuments { get; set; }

            public DocumentListVM()
            {
            }

            public DocumentListVM(ApplicationDbContext db, int practiceID)
            {
                _db = db;

                _practice = _db.Practices.FirstOrDefault(p => p.Id == practiceID);

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

                //var prefix = @"~/Content/practice/";

                BeforeDocuments = PracticFileDescriptor.Before()
                    .Where(d => d.Type != PracticeDocumentType.Rejection)
                    .Select(d => new DocumentVM
                    {
                        Name = d.TypeName,
                        DocumentType = (int)d.Type,
                        Title = d.Title
                    })
                    .ToList();

                AfterDocuments = PracticFileDescriptor.After()
                    .Where(d => d.Type != PracticeDocumentType.Rejection)
                    .Select(d => new DocumentVM
                    {
                        Name = d.TypeName,
                        DocumentType = (int)d.Type,
                        Title = d.Title
                    })
                    .ToList();

                DistantDocuments = PracticFileDescriptor.Distant()
                    .Where(d => d.Type != PracticeDocumentType.Rejection)
                    .Select(d => new DocumentVM
                    {
                        Name = d.TypeName,
                        DocumentType = (int)d.Type,
                        Title = d.Title
                    })
                    .ToList();

                //BeforeDocuments = new List<DocumentVM>
                //{
                //    new DocumentVM { Name="Договор с предприятием", Link=prefix+"Dogovor_o_provedenii_uchebnoi_proizvodstvennoi_preddiplomnoi_praktiki_studentov.doc" },
                //    new DocumentVM { Name="Резюме", Link=prefix+"Резюме.docx" },
                //    new DocumentVM { Name="Командировочное удостоверение (если практика с выездом)", Link=prefix+"Komandirovochnoe_udostoverenie_studenta.doc" },
                //    new DocumentVM { Name="Письмо-направление на предприятие", Link=prefix+"Письмо-направление на предприятие.docx" },
                //    new DocumentVM { Name="Заявление студента о переносе практики", Link=prefix+"Заявление о переносе практики.docx" },
                //};

                //AfterDocuments = new List<DocumentVM>
                //{
                //    new DocumentVM { Name="Извещение о прохождении практики" ,  },
                //    new DocumentVM { Name="Задание на практику" , Link=prefix+"Индивидуальное задание на учебную производственную(преддипломную) практику студента.docx" },
                //    new DocumentVM { Name="Отчет по практике" , Link=prefix+"Отчет по практике.docx" },
                //    new DocumentVM { Name="Отзыв руководителя" , Link=prefix+"Отзыв руководителя.docx" },
                //};
            }
        }

        public class DocumentVM
        {
            public string Name { get; set; }
            public int DocumentType { get; set; }

            /// <summary>
            /// Всплывающая подсказка при наведении
            /// </summary>
            public string Title { get; set; }
        }
}
