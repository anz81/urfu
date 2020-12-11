using System.Text;

namespace Urfu.Its.Common
{
    public static class PersonHelper
    {
        public static string PrepareFullName(string surname, string name, string patronymic)
        {
            string fullName;

            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(surname))
                builder.Append(surname).Append(" ");
            if (!string.IsNullOrEmpty(name))
                builder.Append(name).Append(" ");
            if (!string.IsNullOrEmpty(patronymic))
                builder.Append(patronymic).Append(" ");

            if (builder.Length == 0)
            {
                fullName = string.Empty;
                return fullName;
            }

            builder.Length--;
            fullName = builder.ToString();

            return fullName;
        }

        public static string PrepareShortName(string surname, string name, string patronymic)
        {
            string shortName;

            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(name))
            {
                builder.Append(name[0]).Append(".");
                if (!string.IsNullOrEmpty(patronymic))
                    builder.Append(patronymic[0]).Append(".");
                builder.Append(" ");
            }
            
            if (!string.IsNullOrEmpty(surname))
                builder.Append(surname).Append(" ");

            if (builder.Length == 0)
            {
                shortName = string.Empty;
                return shortName;
            }

            builder.Length--;
            shortName = builder.ToString();

            return shortName;
        }
    }
}