using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportCompetencesScriptBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Data Source=(localdb)\mssqllocaldb;Initial Catalog=its;Trusted_Connection=True";
            var directions = LoadDirections(connectionString).ToList();

            var folderPath = @"C:\Program Files (x86)\MMIS Lab\GosInsp\Dat";

            var metrics = LoadMetrics(folderPath).ToList();

            CheckData(metrics, directions);

            var outputFolder = "../../OutputScripts";

            var script = GenerateScript(metrics, directions);
            File.WriteAllText(Path.Combine(outputFolder, "InsertCompetences.sql"), script, Encoding.UTF8);
        }

        private static string GenerateScript(List<Metric> metrics, List<DirectionInfo> directions)
        {
            var builder = new StringBuilder();
            //builder.AppendLine("SET NOEXEC ON");
            builder.AppendLine("BEGIN TRAN");
            builder.AppendLine("GO");
            builder.AppendLine();
            var codeComparer = new CompetenceCodeComparer();
            foreach (var metric in metrics)
            {
                builder.AppendLine("BEGIN TRY");
                builder.AppendLine("\tINSERT INTO Competences([Code], [Content], [Order], [Type], [DirectionId], [Okso], [Standard], [ExternalId]) VALUES");
                var direction = directions.FirstOrDefault(d => d.okso == metric.Okso);
                builder.AppendLine(string.Join(",\r\n",
                    metric.Competences.GroupBy(c => c.Type)
                        .SelectMany(group => group.OrderBy(c => c.Code, codeComparer)
                            .Select((competence, order) => $"\t\t({SqlFormat(competence.Code)}, {SqlFormat(competence.Content)}, {SqlFormat(order)}, {SqlFormat(group.Key)}, {SqlFormat(direction?.uid)}, {SqlFormat(competence.Okso)}, {SqlFormat(competence.Standard)}, {SqlFormat(competence.Id)})"))));
                builder.AppendLine("END TRY");
                builder.AppendLine("BEGIN CATCH");
                builder.AppendLine("\traiserror('Batch error', 20, -1) with log");
                builder.AppendLine("END CATCH");
                builder.AppendLine("GO");
            }
            builder.AppendLine();
            builder.AppendLine("COMMIT");
            //builder.AppendLine("SET NOEXEC OFF");
            return builder.ToString();
        }

        private static string SqlFormat(object value)
        {
            switch (value)
            {
                case string _:
                    return "'" + value + "'";
                case null:
                    return "NULL";
                default:
                    return value.ToString();
            }
        }

        private static IEnumerable<DirectionInfo> LoadDirections(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT uid, okso, title FROM Directions", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new DirectionInfo
                            {
                                uid = (string)reader[0],
                                okso = (string)reader[1],
                                title = (string)reader[2]
                            };
                        }
                    }
                }
            }
        }

        private static IEnumerable<Metric> LoadMetrics(string folderPath)
        {
            DateTime? ReadDate(object value)
            {
                if (value == DBNull.Value)
                    return null;
                return DateTime.Parse((string)value);
            }

            string ReadString(object value)
            {
                if (value == DBNull.Value)
                    return null;
                return (string)value;
            }

            Competence CompetenceFactory(DataRow row)
            {
                var code = ReadString(row[2])?.Trim().Replace("C", "С"); // Первый символ - английская буква, заменяем её на руссую                
                return new Competence
                {
                    Id = (int)row[0],
                    MetricId = (int)row[1],
                    Code = code,
                    Content = ReadString(row[3])?.Trim().ToLowerFirstLetter(),
                    NewCompetenceType = ReadString(row[9])
                };
            }

            Metric MetricFactory1(DataRow row) => new Metric
            {
                Id = (int)row[0],
                Okso = ReadString(row[2]),
                Name = ReadString(row[3]),
                Date = ReadDate(row[4])
            };

            var loader1 = new Loader(new SourceDescriptor
            {
                FilePath = Path.Combine(folderPath, "CompetenceTbl.mdb"),
                CompetenceFactory = CompetenceFactory,
                MetricFactory = MetricFactory1
            });

            var regex = new Regex(@"\d\d\.\d\d\.\d\d", RegexOptions.Compiled);

            var metrics = loader1.LoadMetrics()
                .Where(m => !m.Date.HasValue || m.Date.Value.Year > 2010)
                .Where(m => !string.IsNullOrWhiteSpace(m.Okso))
                .Where(m => regex.IsMatch(m.Okso))
                .Where(m => m.Competences.Any()).ToList();

            var globalOrder = 0;
            foreach (var metric in metrics)
                foreach (var competence in metric.Competences)
                {
                    ++globalOrder;
                    competence.Okso = metric.Okso;

                    if (competence.Type == "УК" || globalOrder >= 20978 || !string.IsNullOrWhiteSpace(competence.NewCompetenceType))
                        competence.Standard = "ФГОС ВО 3++";
                    else
                        competence.Standard = "ФГОС ВО";
                }

            // Исправление ошибок кодов типа:
            /*
                ОПК-1. (лишняя точка) 
                ОПК-1  (так будет исправлено) 

                ОПК -10 (лишние пробелы) 
                ОПК-10  (так будет исправлено) 

                ПСК-2-16 (кривой разделитель) 
                ПСК-2.16 (так будет исправлено) 
            */
            foreach (var metric in metrics)
                foreach (var competence in metric.Competences)
                {
                    var code = competence.Code;
                    competence.RawCode = competence.Code;
                    var m = CompetenceHelper.CodeRegex.Match(code);
                    if (!m.Success)
                        throw new InvalidOperationException();
                    var typeName = m.Groups[1].Value.ToUpper();
                    var n1 = m.Groups[2].Value;
                    code = $"{typeName}-{n1}";
                    if (m.Groups[3].Success)
                        code = $"{code}.{m.Groups[3].Value}";
                    competence.Code = code;
                }

            return metrics;
        }

        private static void CheckData(List<Metric> metrics, List<DirectionInfo> directions)
        {
            var duplicateStandartOksoAndCodes = metrics.SelectMany(m => m.Competences)
                .GroupBy(c => c.Okso + c.Code + c.Standard).Where(g=>g.Count() > 1).ToList();

            Debug.Assert(metrics.GroupBy(m => m.Id).All(g => g.Count() == 1));
            Debug.Assert(metrics.SelectMany(m => m.Competences).GroupBy(c => c.Id).All(g => g.Count() == 1));

            var invalidCompetences = metrics.SelectMany(m => m.Competences).Where(c => !c.IsValid).ToList();
            var invalidCompetencesString = string.Join("\r\n",
                invalidCompetences.Select(c =>
                {
                    var direction = directions.FirstOrDefault(d => d.okso == c.Metric.Okso);
                    return $"[{c.Id}]: {c.Code} - {c.Content ?? "null"}. Направление [{direction?.uid}] {direction?.okso} {direction?.title}";
                }));
            //Debug.Assert(metrics.SelectMany(m => m.Competences).All(g =>g.IsValid));
            Debug.Assert(metrics.All(g => g.IsValid));
            Debug.Assert(metrics.SelectMany(m => m.Competences).All(c => !string.IsNullOrWhiteSpace(c.Okso)));

            var comparer = new CompetenceCodeComparer();
            var codes = metrics.SelectMany(m => m.Competences).GroupBy(c => c.Code).Select(g => g.Key).OrderBy(k => k, comparer).ToList();

            var correctedCodes = metrics.SelectMany(m => m.Competences).Where(c => c.RawCode != c.Code).ToList();
            var correctionInfo = string.Join("\r\n", correctedCodes.Select(c => c.RawCode + "\t|\t" + c.Code));

            var emptyMetrics = metrics.Where(m => !m.Competences.Any()).ToList();
            Debug.Assert(!emptyMetrics.Any());

            var notExistsInOurDb = metrics.Where(m => !directions.Select(d => d.okso.ToLower()).Contains(m.Okso.ToLower())).ToList();
            var notExistsInTheirDb = directions.Where(d => !metrics.Select(m => m.Okso.ToLower()).Contains(d.okso.ToLower())).ToList();
            var existsInBothDbs = metrics.Where(m => directions.Select(d => d.okso.ToLower()).Contains(m.Okso.ToLower())).ToList();

            var groupedByOkso = existsInBothDbs.GroupBy(m => m.Okso).Where(c => c.Count() > 1).ToList();
            var groupedByOkso3 = existsInBothDbs.GroupBy(m => m.Okso).Where(c => c.Count() > 2).ToList();
            Debug.Assert(groupedByOkso3.Count == 0);

            var competences = metrics.SelectMany(m => m.Competences).ToList();
            var types = competences.GroupBy(c => c.Type).Select(t => t.Key).ToList();

            var expectedTypes = new[] { "ОК", "ОПК", "ПК", "УК", "ПСК" };
            Debug.Assert(expectedTypes.All(t => types.Contains(t)));
            Debug.Assert(types.All(t => expectedTypes.Contains(t)));
        }
    }

    public static class CompetenceHelper
    {
        public static readonly Regex CodeRegex = new Regex(@"([a-яА-Я]{1,3})\s*-?\s*(\d{1,3})(?:[\D](\d{1,2}))?", RegexOptions.Compiled);
    }

    internal class CompetenceCodeComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var m1 = CompetenceHelper.CodeRegex.Match(x);
            var m2 = CompetenceHelper.CodeRegex.Match(y);
            Debug.Assert(m1.Success);
            Debug.Assert(m2.Success);
            var type1 = m1.Groups[1].Value;
            var type2 = m2.Groups[1].Value;
            var c0 = string.Compare(type1, type2, StringComparison.InvariantCultureIgnoreCase);
            if (c0 == 0)
            {
                var x1 = int.Parse(m1.Groups[2].Value);
                var y1 = int.Parse(m2.Groups[2].Value);
                var c1 = x1.CompareTo(y1);
                if (c1 == 0)
                {
                    int.TryParse(m1.Groups[3].Value, out var x2);
                    int.TryParse(m2.Groups[3].Value, out var y2);
                    var c2 = x2.CompareTo(y2);
                    return c2;
                }
                return c1;
            }
            return c0;
        }
    }

    internal class DirectionInfo
    {
        public string uid { get; set; }
        public string okso { get; set; }
        public string title { get; set; }
    }

    public static class StringExtensions
    {
        public static string ToLowerFirstLetter(this string str)
        {
            if (str == null)
                return null;
            return Char.ToLower(str[0]) + str.Substring(1);
        }
    }

    public class Loader
    {
        private readonly SourceDescriptor _source;

        public Loader(SourceDescriptor source)
        {
            _source = source;
        }

        public IEnumerable<Metric> LoadMetrics()
        {
            var mt = LoadTable(_source.FilePath, "госМетрики");
            var competences = LoadCompetences().ToList();

            foreach (DataRow row in mt.Rows)
            {
                var metric = _source.MetricFactory(row);
                var metricCompetences = competences.Where(c => c.MetricId == metric.Id).ToList();
                foreach (var metricCompetence in metricCompetences)
                    metricCompetence.Metric = metric;
                metric.Competences = metricCompetences;
                yield return metric;
            }
        }

        public IEnumerable<Competence> LoadCompetences()
        {
            var ct = LoadTable(_source.FilePath, "госКомпетенции");
            foreach (DataRow row in ct.Rows)
            {
                yield return _source.CompetenceFactory(row);
            }
        }

        private DataTable LoadTable(string filePath, string tableName)
        {
            var table = new DataTable(tableName);

            using (var connection = CreateConnection(filePath))
            {
                var query = $"Select * From {tableName}";
                var adapter = new OleDbDataAdapter(query, connection);
                adapter.Fill(table);
            }

            return table;
        }

        private OleDbConnection CreateConnection(string source)
        {
            var connection = new OleDbConnection($"Provider=Microsoft.JET.OLEDB.4.0;data source={source}");
            connection.Open();
            return connection;
        }
    }

    public class SourceDescriptor
    {
        public string FilePath { get; set; }
        public Func<DataRow, Competence> CompetenceFactory { get; set; }
        public Func<DataRow, Metric> MetricFactory { get; set; }
    }

    public class Metric
    {
        public int Id { get; set; }
        public string Okso { get; set; }
        public string Name { get; set; }
        public ICollection<Competence> Competences { get; set; } = new List<Competence>();
        public DateTime? Date { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(Okso);
    }

    public class Competence
    {
        public string RawCode { get; set; }
        public string Code { get; set; }
        public int MetricId { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }

        public string Type => Code.Split(' ', '-', '.').First();

        public Metric Metric { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Content);
        public string Okso { get; set; }
        public string NewCompetenceType { get; set; }
        public string Standard { get; set; }
    }
}
