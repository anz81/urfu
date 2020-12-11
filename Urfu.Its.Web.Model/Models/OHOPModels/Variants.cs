using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Urfu.Its.Web.Model.Models;

namespace Urfu.Its.Web.Models
{
    public class Variants
    {
        /// <summary>
        /// для Таблицы 1. true - по образовательной программе, false - по траекториям
        /// </summary>
        public bool IsProfile { get; set; }
        public ICollection<VariantInfo> VariantInfos { get; set; }
    }

    /// <summary>
    /// Источник данных - таблицы из БД
    /// </summary>
    public enum IdSource
    {
        Profile,
        Variants,
        VariantUni
    }

    public class VariantSourceInfo
    {
        public IdSource IdSource { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class VariantInfo : VariantSourceInfo
    {
        public ICollection<ProfActivityInfo> ProfActivityRows { get; set; }
    }

    public class ProfActivityInfo
    {
        public string ProfObjects { get; set; }
        public string ProfTaskTypes { get; set; }

        public string AreaCode { get; set; }
        public string AreaTitle { get; set; }

        public bool NoProfStandard { get; set; }

        public string KindCode { get; set; }
        public string KindTitle { get; set; }

        public string StandardCode { get; set; }
        public string StandardTitle { get; set; }

        public string Functions { get; set; }

        /// <summary>
        /// для Таблицы 4. Профессиональные компетенции
        /// </summary>
        public ICollection<CompetenceInfoVM> Competences { get; set; } = new List<CompetenceInfoVM>();
    }

    /// <summary>
    /// Используется для формирования списка траекторий (ОХОП Раздел 2. Таблица 1) из двух таблиц БД - Variants и VariantUni.
    /// </summary>
    public class VariantInfoModel
    {
        public IdSource IdSource { get; set; }
        public string Id { get; set; }

        public string Name { get; set; }
    }
}