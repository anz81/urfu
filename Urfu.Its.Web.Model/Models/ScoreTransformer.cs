using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Urfu.Its.Web.Models
{
    public class ScoreTransformer
    {
        public static string ScoreToMark(decimal? r) => r.HasValue ? r >= 40 ? "Зачет" : "Незачет" : "";

        public static bool? IsPassScore(decimal? score)
        {
            if (score.HasValue)
                return score >= 40;
            return null;
        }
    }
}