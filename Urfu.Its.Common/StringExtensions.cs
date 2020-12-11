using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Urfu.Its.Common
{
    public static class StringExtensions
    {
        public static string TrimFrontIfLongerThan(this string value, int minLimit, int maxLimit)
        {
            if (value.Length > minLimit)
            {
                var expectedLimit = value.IndexOf(" ", minLimit);
                if (expectedLimit > maxLimit || expectedLimit < 0)
                    return value.Substring(value.Length - (minLimit - 3)) + "...";
                return value.Substring(value.Length - (expectedLimit)) + "...";
            }

            return value;
        }
        public static string CleanFileName(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        public static string NoLonger(this string fileName, int length)
        {
            if (fileName.Length > length)
                return fileName.Substring(0, length);
            return fileName;

        }
        public static string ToDownloadFileName(this string value)
        {
            return value.Replace(' ', (char)0160);
        }

        public static string ToLowerFirstLetter(this string str)
        {
            if (str == null)
                return null;
            return Char.ToLower(str[0]) + str.Substring(1);
        }
        public static string ToUpperFirstLetter(this string str)
        {
            if (str == null)
                return null;
            return Char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
