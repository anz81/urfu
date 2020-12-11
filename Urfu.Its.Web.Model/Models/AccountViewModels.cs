using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
//using System.Web.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Urfu.Its.Web.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Urfu.Its.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен состоять минимум из {2} символов.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Display(Name = "Имя пользователя")]
        [Remote("IsUserNameAvailable", "Account")]
        public string UserName { get; set; }

        [Required, MaxLength(127)]
        [Display(Name = "Логин в ActiveDirectory")]
        [Remote("IsAdNameAvailable","Account")]
        public string AdName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен состоять минимум из {2} символов.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    
    public class EditUserViewModel
    {
        public EditUserViewModel() { }
  
        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(ApplicationUser user)
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Email = user.Email;
            Patronymic = user.Patronymic;
            AdName = user.AdName;
        }
  
        [Required]
        [Display(Name = "Имя пользователя")]
        [Key]
        public string UserName { get; set; }
  
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
  
        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Required, MaxLength(127)]
        [Display(Name = "Логин в ActiveDirectory")]
        public string AdName { get; set; }
  
        [Required]
        public string Email { get; set; }

        [Display(Name = "Сбросить пароль")]
        public bool ResetPassword { get; set; }

        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Display(Name = "Подтверждение пароля")]
        public string NewPasswordConfirmation { get; set; }
    }
  
  
    public class SelectUserRolesViewModel
    {
        public SelectUserRolesViewModel() 
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }
  
  
        // Enable initialization with an instance of ApplicationUser:
        public SelectUserRolesViewModel(ApplicationUser user) : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            DbSet<IdentityRole> allRoles;
            using (var db = new ApplicationDbContext())
            {
                allRoles = db.Roles;

                foreach (var role in allRoles)
                {
                    // An EditorViewModel will be used by Editor Template:
                    var rvm = new SelectRoleEditorViewModel(role);
                    this.Roles.Add(rvm);
                }
                UserStore<ApplicationUser> us = new UserStore<ApplicationUser>(db);
                UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(us,null,null,null,null,null,null,null,null);
                var Roles = um.GetRolesAsync(user).Result;
                // Set the Selected property to true for those roles for 
                // which the current user is a member:
                foreach (var userRole in Roles)
                {
                    var checkUserRole =
                        this.Roles.Find(r => r.RoleName == userRole);

                    checkUserRole.Selected = true;
                }
            }
        }

        public SelectUserRolesViewModel(RoleSet set) : this()
        {
            FirstName = set.Id.ToString();
            UserName = set.Name;
            DbSet<IdentityRole> allRoles;
            using (var db = new ApplicationDbContext())
            {
                allRoles = db.Roles;

                foreach (var role in allRoles)
                {
                    // An EditorViewModel will be used by Editor Template:
                    var rvm = new SelectRoleEditorViewModel(role);
                    this.Roles.Add(rvm);
                }

                // Set the Selected property to true for those roles for 
                // which the current user is a member:
                foreach (var userRole in set.Contents)
                {
                    var checkUserRole =
                        this.Roles.Find(r => r.Id == userRole.RoleId);

                    checkUserRole.Selected = true;
                }
            }
        }
  
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }
  
    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() {}
        public SelectRoleEditorViewModel(IdentityRole role)
        {
            this.RoleName = role.Name;
            Id = role.Id;
        }
  
        public bool Selected { get; set; }
  
        [Required]
        public string RoleName { get; set;}

        public string RoleDescription { get { return ItsRoles.GetDescription(RoleName); }}

        public string Id { get; set; }
    }


    public static class ItsRoles
    {
        public const string Admin = "Admin";
        public const string AllDirections = "AllDirections";
        public const string ServiceLogin = "ServiceLogin";
        public const string NsiView = "Nsi.view";
        public const string NsiEdit = "Nsi.edit";
        public const string VariantsView = "Variants.view";
        public const string VariantsEdit = "Variants.edit";
        public const string VariantsChangeState = "Variants.changeState";
        public const string StudentAdmission = "StudentAdmission";
        public const string StudentAdmissionRead = "StudentAdmission.view";
        public const string MinorView = "Minor.view";
        public const string MinorEdit = "Minor.edit";
        public const string MinorEditMarks = "Minor.editMarks";
        public const string MinorFreezeMarks = "Minor.freezeMarks";
        public const string MinorAutoAdmission = "Minor.autoAdmission";
        public const string MinorViewAdmission = "Minor.viewAdmission";
        public const string MinorCreateGroup = "Minor.createGroup";
        public const string MinorLimitEdit = "Minor.editLimit";
        public const string MinorReport = "Minor.report";
        public const string AllMinors = "AllMinor";
        public const string MinorMassPublishAdmissions = "Minor.MassPublishAdmissions";
        public const string SectionFKMassPublishAdmissions = "SectionFK.MassPublishAdmissions";
        public const string SportsmanSetting = "SportsmanSetting";
        public const string SectionFKManager = "SectionFKManager";
        public const string ForeignLanguageManager = "ForeignLanguageManager";
        public const string ForeignLanguageMassPublishAdmissions = "ForeignLanguage.MassPublishAdmissions";
        public const string PracticeManager = "PracticeManager";
        public const string PracticeView = "PracticeView";
        //public const string PracticeEdit = "PracticeEdit";
        public const string WorkingProgramManager = "WorkingProgramManager";
        public const string WorkingProgramView = "WorkingProgramView";
        public const string ConfirmationOfContractPractice = "ConfirmationOfContractPractice";
        public const string ProjectManager = "ProjectManager";
        public const string ProjectCurator = "ProjectCurator";
        public const string ProjectROP = "ProjectROP";
        public const string ProjectView = "ProjectView";
        public const string MUPManager = "MUPManager";
        public const string ApproveOhopRpdRpm = "ApprovalOhopRpdRpm";
        public const string RatingCoefficientEdit = "CoefficientEdit";


        static ItsRoles()
        {
            RoleDescription[Admin] = "Создание пользователей и управление их правами";
            RoleDescription[AllDirections] = "Доступ ко всем направлениям";
            RoleDescription[NsiView] = "Чтение справочной информации";
            RoleDescription[NsiEdit] = "Изменение справочной информации";
            RoleDescription[VariantsView] = "Чтение вариантов";
            RoleDescription[VariantsEdit] = "Изменение вариантов";
            RoleDescription[VariantsChangeState] = "Изменение состояния варианта";
            RoleDescription[ServiceLogin] = "Соединение с API";
            RoleDescription[StudentAdmission] = "Зачисление студентов";
            RoleDescription[StudentAdmissionRead] = "Чтение зачислений студентов";
            RoleDescription[MinorView] = "Просмотр майноров";
            RoleDescription[MinorEdit] = "Редактирование информации по майнорам";
            RoleDescription[MinorEditMarks] = "Редактирование оценок по майнорам";
            RoleDescription[MinorFreezeMarks] = "Изменение статуса ведомостей на майноры";
            RoleDescription[AllMinors] = "Доступ ко всем майнорам";
            RoleDescription[MinorAutoAdmission] = "Расчёт автоматического зачисления на майноры";
            RoleDescription[MinorViewAdmission] = "Просмотр зачислений на майноры";
            RoleDescription[MinorCreateGroup] = "Создание подгрупп на майноры";
            RoleDescription[MinorLimitEdit] = "Редактирование лимитов майноров";
            RoleDescription[MinorReport] = "Просмотр отчета по майнорам";
            RoleDescription[MinorMassPublishAdmissions] = "Массовая отправка зачислений в ЛК Майноры";
            RoleDescription[SportsmanSetting] = "Установка признака спортсмена";
            RoleDescription[SectionFKManager] = "Работа с секциями ФК";
            RoleDescription[ForeignLanguageManager] = "Работа с модулями ИЯ";
            RoleDescription[SectionFKMassPublishAdmissions] = "Массовая отправка зачислений в ЛК Секции ФК";
            RoleDescription[ForeignLanguageMassPublishAdmissions] = "Массовая отправка зачислений в ЛК Модули ИЯ";
            RoleDescription[PracticeManager] = "Работа с практиками";
            RoleDescription[PracticeView] = "Просмотр практик";
            RoleDescription[WorkingProgramManager] = "Работа с рабочими программами";
            RoleDescription[WorkingProgramView] = "Просмотр рабочих программ";
            RoleDescription[ConfirmationOfContractPractice] = "Подтверждение договора на практику";
            RoleDescription[ProjectView] = "Просмотр Проектного обучения";
            RoleDescription[ProjectManager] = "Работа с модулем Проектное обучение";
            RoleDescription[ProjectCurator] = "Куратор Проектного обучения";
            RoleDescription[ProjectROP] = "РОП Проектного обучения";
            RoleDescription[MUPManager] = "Работа с МУПами";
            RoleDescription[ApproveOhopRpdRpm] = "Согласование ОХОП, РПД, РПМ";
            RoleDescription[RatingCoefficientEdit] = "Коэффициенты для расчета рейтинга";
        }

        public static IEnumerable<string> RoleNames { get { return RoleDescription.Keys;} }

        static readonly Dictionary<string,string> RoleDescription = new Dictionary<string, string>();

        public static string GetDescription(string roleName)
        {
            return RoleDescription[roleName];
        }
    }
}

