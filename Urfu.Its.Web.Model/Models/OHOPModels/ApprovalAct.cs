using System.Collections.Generic;

namespace Urfu.Its.Web.Models
{
    public class ApprovalAct
    {
        public string ChairDirector { get; set; }
        public string DateChair { get; set; }
        public string Company { get; set; }
        public string CompanyDirector { get; set; }
        public string DateCompany { get; set; }
        public string NumberOfApprovalAct { get; set; }
        public string DateOfApprovalAct { get; set; }
        public ICollection<ExpertInfo> Experts { get; set; }

        /// <summary>
        /// Приложение к Акту согласования. Пункт 3.
        /// Изначально заполняется данными из Таблицы 1 и 4.
        /// </summary>
        public ICollection<VariantInfo> VariantInfos { get; set; } = null;

        public string FileName { get; set; } = string.Empty;

        public int? FileId { get; set; }
    }

    public class ExpertInfo
    {
        public string FullName { get; set; }
        public string Post { get; set; }
    }
}