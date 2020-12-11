using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Urfu.Its.Common
{
    public static class DecimalExtensions
    {
        public static string ToYearMonthFormat(this decimal value)
        {
            var result = "";
            
            int year = 0, month = 0;

            var numbers = value.ToString().Split(',');
            if (numbers.Length == 1)
            {
                int.TryParse(numbers[0], out year);
            }
            if (numbers.Length == 2)
            {
                int.TryParse(numbers[0], out year);
                if (numbers[1].Length == 2 && numbers[1].Last() == '0')
                    numbers[1] = numbers[1].First().ToString();
                int.TryParse(numbers[1], out month);
            }

            if (year != 0)
            {
                if (year == 1)
                    result += $"{year} год";
                if (year >= 2 && year <= 4)
                    result += $"{year} года";
                if (year >= 5)
                    result += $"{year} лет";
            }

            if (!string.IsNullOrWhiteSpace(result) && month != 0)
                result += " ";

            if (month != 0)
            {
                result += $"{month} мес.";
            }

            return result;
        }
    }
}
