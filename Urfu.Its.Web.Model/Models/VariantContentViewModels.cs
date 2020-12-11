using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Model.Models
{
    public class VariantHelper
    {
        public static List<EditVariantContentRowViewModel> VariantContentRows(Variant variant, Variant programVariant)
        {
            var res = variant.Program.Direction.Modules
                .Where(
                    m =>
                        m.UsedInVariantContents.Any(vc => vc.Group.VariantId == variant.Id && vc.Selected) ||
                        m.Plans.Any(
                            p => p.qualification == variant.Program.qualification &&
                                 p.familirizationType == variant.Program.familirizationType &&
                                 p.familirizationCondition == variant.Program.familirizationCondition &&
                                 p.versionNumber == variant.Program.PlanVersionNumber &&
                                 p.eduplanNumber == variant.Program.PlanNumber &&
                                 (p.faculty == programVariant.Program.divisionId ||
                                  p.faculty == programVariant.Program.departmentId ||
                                  p.faculty == programVariant.Program.chairId) &&
                                 p.active && !p.remove
                        )).Select(m => new EditVariantContentRowViewModel(m, variant, programVariant)).OrderBy(m => m.Priority).ThenBy(m => m.GroupType).ThenBy(m => m.ModuleName).ToList();
            return res;
        }
    }

    public class EditVariantContentViewModel : EditVariantVM
    {

        public EditVariantContentViewModel()
        {
        }

        public EditVariantContentViewModel(Variant variant, Variant programVariant) : base(variant)
        {
            SelectionDeadline = variant.SelectionDeadline;
            Rows = VariantHelper.VariantContentRows(variant, programVariant);
     
            int rowNum = 1;
            foreach (var row in Rows)
            {

                row.RowNum = rowNum++;
                List<SelectListItem> groups = new List<SelectListItem>();
                groups.Add(new SelectListItem() { Selected = null == row.SelectionGroupId });
                if (row.Base)
                {
                    groups =
                        groups.Concat(
                            variant.Program.Variant.SelectionGroups.Select(
                                g =>
                                    new SelectListItem()
                                    {
                                        Text = g.Name,
                                        Value = g.Id.ToString(),
                                        Selected = g.Id == row.SelectionGroupId
                                    })).ToList();
                    row.SelectionGroups = groups;
                }
                else
                {
                    groups =
                        groups.Concat(
                            variant.SelectionGroups.Select(
                                g =>
                                    new SelectListItem()
                                    {
                                        Text = g.Name,
                                        Value = g.Id.ToString(),
                                        Selected = g.Id == row.SelectionGroupId
                                    })).ToList();
                    row.SelectionGroups = groups;
                }
            }

            directionName = variant.Program.Direction.OksoAndTitle;
            Groups = variant.Groups.OrderBy(g => g.GroupType).ToList();
            SelectionGroups = variant.SelectionGroups.ToList();

            AllSelectionGroups = variant.SelectionGroups.Concat(variant.Program.Variant.SelectionGroups).ToList();
        }

        public List<EditVariantContentRowViewModel> Rows { get; set; }

        public List<VariantGroup> Groups { get; set; }
        public List<VariantSelectionGroup> SelectionGroups { get; set; }
        public List<VariantSelectionGroup> AllSelectionGroups { get; set; }

        [DisplayName("Направление")]
        public string directionName { get; set; }

        public string Message { get; set; }

        [DisplayName("Технология освоения")]
        public string familirizationTech { get; set; }

    }

    public class EditVariantContentRowViewModel
    {
        public EditVariantContentRowViewModel()
        {
        }

        readonly VariantContent _content;

        readonly List<Plan> _plans;
        [JsonIgnore]
        public List<Plan> Plans
        {
            get { return _plans; }
        }

        public EditVariantContentRowViewModel(Module module, Variant variant, Variant programVariant)
        {
            ModuleId = module.uuid;
            ModuleName = module.title;

            _plans = module.Plans.Where(p => p.directionId == variant.Program.directionId)
                .Where(p => p.qualification == variant.Program.qualification &&
                            p.familirizationType == variant.Program.familirizationType &&
                            p.familirizationCondition == variant.Program.familirizationCondition &&
                            p.versionNumber == variant.Program.PlanVersionNumber &&
                            p.eduplanNumber == variant.Program.PlanNumber &&
                                                 p.active && !p.remove
                )
                .Where(p => (p.faculty == programVariant.Program.divisionId || p.faculty == programVariant.Program.departmentId || p.faculty == programVariant.Program.chairId))
                .ToList();

            TestUnits = _plans
                .Where(p => p.terms.Length > 2)
                .ToList()
                .Select(p => new
                {
                    terms = JsonConvert.DeserializeObject<List<int>>(p.terms),
                    testUnitsByTerm = JObject.Parse(p.testUnitsByTerm)
                })
                .SelectMany(p => p.terms.Where(t =>
                    {
                        var value = p.testUnitsByTerm.GetValue(t.ToString());
                        return value != null;
                    })
                .Select(t => new { name = t, testUnits = (int)p.testUnitsByTerm.GetValue(t.ToString()) }))
                .Sum(t => t.testUnits);
            Priority = module.priority;
            
            PlanInfo = string.Join("," + Environment.NewLine,
                _plans
                    .Select(p => p.eduplanNumber + " " + p.versionTitle)
                    .Distinct()
                    .OrderBy(p => p));

            Terms = string.Join(",",
                _plans.SelectMany(p => JsonConvert.DeserializeObject<int[]>(p.allTermsExtracted)).Distinct().OrderBy(i => i));

            VariantId = variant.Id;
            Base = !variant.IsBase && programVariant.Groups.SelectMany(g => g.Contents).Any(c => c.moduleId == module.uuid && c.Selected);//адский запрос
            EduProgramLimit limit;
            if (Base)
            {
                _content =
                    programVariant.Groups.SelectMany(g => g.Contents).FirstOrDefault(vc => vc.moduleId == module.uuid);
                limit = programVariant.ProgramLimits.FirstOrDefault(l => l.ModuleId == ModuleId);
            }
            else
            {
                _content = variant.Groups.SelectMany(g => g.Contents).FirstOrDefault(vc => vc.moduleId == module.uuid);
                limit = variant.ProgramLimits.FirstOrDefault(l => l.ModuleId == ModuleId);
            }

            if (limit != null) Limits = limit.StudentsCount;


            IsFgosVo = programVariant.IsFgosVo(); //programVariant.Program.Direction.standard == "ФГОС ВО";
            var plan = _plans.FirstOrDefault(p => !string.IsNullOrEmpty(p.moduleGroupType));
            SubgroupType = VariantGroupTypeHelpers.TryParse(plan?.moduleSubgroupType);

            var planGrouopModuleType = plan?.moduleGroupType;

            RealGroupType = IsFgosVo
                ? VariantGroupTypeHelpers.TryParse(module.type)
                        ?? VariantGroupTypeHelpers.TryParse(module.disciplines.FirstOrDefault(d => !d.section.StartsWith("Контроль"))?.section)
                        ?? VariantGroupType.Base
                : VariantGroupTypeHelpers.TryParse(planGrouopModuleType) 
                        ?? VariantGroupType.Base;

            var possibleGroupType = IsFgosVo
                ? VariantGroupTypeHelpers.TryParse(module.disciplines.FirstOrDefault(d => !d.section.StartsWith("Контроль"))?.section)
                : VariantGroupTypeHelpers.TryParse(planGrouopModuleType);

            if (_content == null)
            {
                GroupType = possibleGroupType ?? VariantGroupType.Base;
                if (GroupType == VariantGroupType.Base)
                    ModuleTypeId = 1;
                if (GroupType == VariantGroupType.Variative) 
                    ModuleTypeId = (int)VariantContentType.Professional + 1;
                if (GroupType == VariantGroupType.Required)
                    ModuleTypeId = (int)VariantContentType.Shared + 1;
                if (GroupType == VariantGroupType.Selectable || GroupType == VariantGroupType.Formed)
                    ModuleTypeId = (int)VariantContentType.SelectableProfessional + 1;

                return;
            }

            VariantContentId = _content.Id;
            Selected = _content.Selected;
            if (Base)
                Selected = false;

            GroupType = possibleGroupType ?? _content.Group.GroupType;

            ModuleTypeId = _content.ModuleTypeId == 0 ? 1 : _content.ModuleTypeId;
            if (_content.ModuleType != null)
                ModuleType = _content.ModuleType.Name;
            Selectable = _content.Selectable;

            SelectionGroupId = _content.VariantSelectionGroupId;
        }

        [Display(Name = "Тип модуля")]
        public int ModuleTypeId { get; set; }

        public string ModuleType { get; set; }

        public decimal Priority { get; set; }

        public int RowNum { get; set; }

        [Required]
        public string ModuleId { get; set; }
        [Display(Name = "Название модуля")]
        public string ModuleName { get; set; }

        [Display(Name = "Есть в траектории")]
        public bool Selected { get; set; }

        [Display(Name = "Включить в основную траекторию")]
        public bool Base { get; set; }

        /// <summary>
        /// Используется для отображения в окне Модули поле Группа модуля. 
        /// </summary>
        [Display(Name = "Тип модуля")]
        public VariantGroupType GroupType { get; set; }

        /// <summary>
        /// ModuleSubgroupType из Plans. На данный момент (01.11.19) может быть null или "По выбору студента"
        /// </summary>
        public VariantGroupType? SubgroupType { get; set; }

        public bool IsFgosVo { get; set; }

        /// <summary>
        /// Используется для подсчета зачетных единиц и для добавления модуля в правильную группу УП. 
        /// Нужен, потому что практики и аттестация должны считаться отдельно, 
        ///     но в интерфейсе поля Группа модуля (свойство GroupType) и Тип модуля (moduleType) должны выводиться в соответствии с section в дисциплине
        /// </summary>
        public VariantGroupType RealGroupType { get; set; }

        [DisplayName("Признак 'по выбору'")]
        public bool Selectable { get; set; }
        [DisplayName("Лимиты")]
        public int? Limits { get; set; }

        public int? SelectionGroupId { get; set; }

        public List<SelectListItem> SelectionGroups { get; set; }

        [DisplayName("Зачётные единицы")]
        public int TestUnits { get; set; }

        public string PlanInfo { get; set; }

        public string Terms { get; set; }

        public string Prerequisites
        {
            get
            {
                if (_content != null)
                    return string.Join(",", _content.Requirments.Select(r => r.Module.number + " " + r.Module.shortTitle));
                return string.Empty;
            }
        }

        public string RequiredFor
        {
            get
            {
                if (_content != null)
                    return string.Join(",", _content.Group.Variant.Groups.SelectMany(g => g.Contents).Where(vc => vc.Requirments.Any(r => r.Id == _content.Id)).Select(r => r.Module.number + " " + r.Module.shortTitle));
                return string.Empty;
            }
        }

        public int VariantId { get; private set; }
        public int? VariantContentId { get; set; }
    }
}