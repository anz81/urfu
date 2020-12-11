using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class RatingCoefficientsViewModel
    {
       public int? Id { get; set; }
       public int ModuleType { get; set;}
       public int Year { get; set; }
       public decimal? Coefficient { get; set; }
       public string Level { get; set; }

       public string ModuleId { get; set;}
       public string ModuleTitle { get; set;}

       public RatingCoefficientsViewModel()
       {
       }

       public RatingCoefficientsViewModel(RatingCoefficient theCoefficient, string moduleTitle)
       {
           Id = theCoefficient.Id;
           ModuleType = theCoefficient.ModuleType;
           Year = theCoefficient.Year;
           Coefficient = theCoefficient.Coefficient;
           Level = theCoefficient.Level;
           ModuleId = theCoefficient.ModuleId;
           ModuleTitle = moduleTitle;
       }
        public string ModuleTypeName
        {
            get {
                switch(ModuleType)
            {
                case (int)ModuleTypeParam.ForeignLanguage:                     
                    return "Иностранный язык";

                case (int)ModuleTypeParam.Project:
                    return "Проектное обучение";

                case (int)ModuleTypeParam.MUP:
                    return "МУПы";
                }
                return "";
            }
        }

    }
}
