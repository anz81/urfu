
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.OHOPModels
{
    public class FormAndDuration
    {
        private readonly Dictionary<string, string> formsGenitive = new Dictionary<string, string>()
        {
            { "очная","очной" },
            { "очно-заочная","очно-заочной" },
            { "заочная","заочной" }
        };

        public string Text { get; set; }

        public List<FormAndDurationRow> Rows { get; set; }

        public string Forms
        {
            get
            {
                var forms = Rows.Select(r => r.Form.ToLower()).Distinct()
                    .Where(f => formsGenitive.Keys.Contains(f)).Select(f => formsGenitive[f]).OrderByDescending(f => f);
                var formWord = forms.Count() > 1 ? "формах" : "форме";
                return $"{string.Join(", ", forms)} {formWord}";
            }
        }
    }

    public class FormAndDurationRow
    {
        public string Form { get; set; }

        /// <summary>
        /// Примечание к форме обучения
        /// </summary>
        public string Comment { get; set; }

        public string Duration { get; set; }
        
        public override string ToString()
        {
            return $"- {Form} форма обучения {Comment} {Duration}";
        }
    }
}
