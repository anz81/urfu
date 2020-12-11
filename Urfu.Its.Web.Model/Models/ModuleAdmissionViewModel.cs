using System.ComponentModel;
using System.Linq;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Models
{
    public class ModuleAdmissionViewModel
    {
        [DisplayName("Статус")]
        public string StudentStatus { get; set; }

        [DisplayName("Личный номер студента")]
        public string PersonalNumber { get; set; }
        private readonly Student _student;
        private readonly Group _group;
        private readonly ModuleAdmission _admission;
        private readonly Person _person;
        private readonly Module _module;
        private readonly Direction _direction;
        private readonly int _admitted;
        private readonly int? _priority;

        public Module Module
        {
            get { return _module; }
        }

        [DisplayName("Зачислено")]
        public int Admitted
        {
            get { return _admitted; }
        }

        public ModuleAdmissionViewModel(Student student, Group @group, ModuleAdmission admission, Person person, Module module, Direction direction, int admitted, int? priority, IQueryable<string> otherAdmissions, string studentStatus, string personalNumber)
        {
            StudentStatus = studentStatus;
            PersonalNumber = personalNumber;
            _student = student;
            _group = @group;
            _admission = admission;
            _person = person;
            _module = module;
            _direction = direction;
            _admitted = admitted;
            _priority = priority;
            OtherAdmissions = string.Join(", ", otherAdmissions);
            if (_admission == null)
            {
                _admission = new ModuleAdmission
                {
                    Student = student,
                    studentId = student.Id,
                    Status = AdmissionStatus.Indeterminate,
                    moduleId = module.uuid
                };
            }
        }

        [DisplayName("Зачисления в другие модули группы")]
        public string OtherAdmissions { get; set; }

        [DisplayName("Приоритет в ЛК")]
        public int? Priority
        {
            get { return _priority; }
        }

        public ModuleAdmission Admission
        {
            get { return _admission; }
        }

        public Direction Direction
        {
            get { return _direction; }
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
    }
}