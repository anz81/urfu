using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Practices
{
    public class PracticeDocumentContract
    {
        /// <summary>
        /// В шаблонах - Вид практики
        /// </summary>
        public string PracticeType { get; set; }

        /// <summary>
        /// В шаблонах - Тип практики
        /// </summary>
        public string PracticeName { get; set; }

        /// <summary>
        /// Место прохождения практики
        /// </summary>
        public string PracticePlace { get; set; }

        /// <summary>
        /// Зачетные единицы
        /// </summary>
        public string TestUnits { get; set; }

        public string ContractNumber { get; set; }
        public string ContractDate { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string NumberLetterOfAttorney { get; set;}
         public string DateLetterOfAttorney { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyEmail { get; set; }
        public string INN { get; set; }

        public string Director { get; set; }
        public string DirectorInitials { get; set; }
        public string DirectorGenitive { get; set; }
        public string PostGenitive { get; set; }
        public string PersonInCharge { get; set; }
        public string PersonInChargeInitials { get; set; }
        public string PostOfPersonInCharge { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string OKSO { get; set; }
        public string Direction { get; set; }
        public string Profile { get; set; }
        public string Course { get; set; }
        public string Semester { get; set; }

        public string Institute { get; set; }
        public string Department { get; set; }
        public string DepartmentFullTitle { get; set; }
        public string Chair { get; set; }
        public string Group { get; set; }

        public string DirectorOfInstitute { get; set; }

        public string Student { get; set; }
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
        public string StudentPatronymicName { get; set; }
        public string StudentShort { get; set; }
        public string StudentPhone { get; set; }
        public string StudentEmail { get; set; }
        public string StudentDateOfBirth { get; set; }
        public string StudentAge { get; set; }

        //Бюджет
        public string Compensation { get; set; }
        //Магистр
        public string Qualification { get; set; }

        public string FamilirizationTech { get; set; }
        public string FamilirizationType { get; set; }
        public string FamilirizationCondition { get; set; }

        //Образовательная программа>
        public string LearnProgramCode { get; set; }
        public string LearnProgramTitle { get; set; }

        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public string StartDate2 { get; set; }
        public string FinishDate2 { get; set; }
        
        public string StudyYear { get; set; }

        public string Teacher { get; set; }
        public string TeacherFullName { get; set; }
        public string TeacherPost { get; set; }
        public string TeacherEmail { get; set; }

        public string Theme { get; set; }
        public string FinishTheme { get; set; }

        public string Theme2
        {
            get { return string.IsNullOrEmpty(FinishTheme)? Theme: FinishTheme; }
        }
        
        public string DecreeNumber { get; set; }
        public string DecreeDate { get; set; }

        public string ReportStartDate { get; set; }
        public string ReportFinishDate { get; set; }


        // поля для отзыва руководителя
        
        /// <summary>
        /// Мероприятия
        /// </summary>
        public string Events { get; set; }

        /// <summary>
        /// Характеристика
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Был ли трудоустроен
        /// </summary>
        public string Employment { get; set; }

        /// <summary>
        /// Предложили ли следующую практику
        /// </summary>
        public string FuturePractice { get; set; }

        /// <summary>
        /// Предложили ли трудоустройство
        /// </summary>
        public string FutureEmployment { get; set; }

        /// <summary>
        /// Предложения/замечания
        /// </summary>
        public string Suggestions { get; set; }

        /// <summary>
        /// Оценка
        /// </summary>
        public string Score { get; set; }

    }
}
