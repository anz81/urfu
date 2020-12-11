using System;
using System.Text.RegularExpressions;

namespace TemplateEngine
{
    public class MarkupCommandPatternAttribute : Attribute
    {
        public string Pattern { get; }
        public RegexOptions RegexOptions { get; }

        public MarkupCommandPatternAttribute(string pattern, RegexOptions regexOptions = RegexOptions.None)
        {
            Pattern = pattern;
            RegexOptions = regexOptions;
        }
    }
}