using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class VariantAdmissionViewModel
    {
        private readonly Student _student;
        private readonly Group _group;
        private readonly VariantAdmission _admission;
        private readonly Person _person;
        private readonly Variant _variant;
        private readonly string _anotherAdmission;
        private readonly int? _priority;

        public Variant Variant
        {
            get { return _variant; }
        }

        public VariantAdmissionViewModel(Student student, Group @group, VariantAdmission admission, Person person, Variant variant, string anotherAdmission, int? priority)
        {
            _student = student;
            _group = @group;
            _admission = admission;
            _person = person;
            _variant = variant;
            _anotherAdmission = anotherAdmission;
            _priority = priority;
            if (_admission == null)
            {
                _admission = new VariantAdmission
                {
                    Student = student,
                    studentId = student.Id,
                    Status = AdmissionStatus.Indeterminate,
                    variantId = variant.Id
                };
            }
        }

        [DisplayName("Приоритет в ЛК")]
        public int? Priority
        {
            get { return _priority; }
        }

        public string AnotherAdmission
        {
            get { return _anotherAdmission; }
        }

        public Person Person
        {
            get { return _person; }
        }

        public Student Student
        {
            get { return _student; }
        }

        public Group Group
        {
            get { return _group; }
        }

        public VariantAdmission Admission
        {
            get { return _admission; }
        }
    }
}