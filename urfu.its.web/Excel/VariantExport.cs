using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using Urfu.Its.Common;
using Urfu.Its.Web.Controllers;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Excel
{
    public class VariantExport
    {
        public Stream ExportVariant(EditVariantContentViewModel obj)
        {
            obj.Rows = obj.Rows.Where(r => r.Selected || r.Base).ToList();
            var templateFile = "variantTemplate.xlsx";
            return Export(obj, templateFile);
        }

        public Stream Export(object obj, string templateFile, List<ReportDynamicColumn> dynColumns = null)
        {
            var he = new HostingEnvironment();
            using (var fileStream = File.Open(he.ContentRootPath + templateFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var package = new ExcelPackage(fileStream))
            {
                FillWorkbookWithObject(package, obj, dynColumns);
                var ms = new MemoryStream();
                package.SaveAs(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }

        private void FillWorkbookWithObject(ExcelPackage package, object o, List<ReportDynamicColumn> dynColumns)
        {
            foreach (var ws in package.Workbook.Worksheets)
            {
                FillSheetWithObj(ws, o, dynColumns);
            }
        }

        private void FillSheetWithObj(ExcelWorksheet sheet, object o, List<ReportDynamicColumn> dynColumns)
        {
            var dimension = sheet.Dimension;
            var startRow = dimension.Start.Row;
            var endRow = dimension.End.Row;
            var startColumn = dimension.Start.Column;
            var endColumn = dimension.End.Column;

            List<RowSpecs> rows = new List<RowSpecs>();
            if (dynColumns != null)
                for (int i = 1; i <= dynColumns.Count; i++)
                {
                    var c = dynColumns[i - 1];
                    sheet.InsertColumn(endColumn + i, 1, endColumn - 1);
                    sheet.Cells[1, endColumn - 1].Copy(sheet.Cells[1, endColumn + i]);
                    sheet.Column(endColumn + i).Style.Border = sheet.Column(endColumn - 1).Style.Border;
                    sheet.Column(endColumn + i).Style.Fill = sheet.Column(endColumn - 1).Style.Fill;
                    sheet.Column(endColumn + i).Width = sheet.Column(endColumn - 1).Width + 10;


                    sheet.Cells[startRow, endColumn + i].Value = c.GetName();
                    //sheet.Column(endColumn + i).AutoFit();
                }

            for (int rIdx = startRow; rIdx <= endRow; rIdx++)
            {


                for (int cIdx = startColumn; cIdx <= endColumn; cIdx++)
                {
                    var value = sheet.Cells[rIdx, cIdx].Value;
                    if (value != null)
                    {
                        var vts = value.ToString();
                        if (vts.StartsWith("$") && vts.EndsWith("$"))
                        {
                            var propertyPath = vts.Trim('$');

                            rows.Add(new RowSpecs { ColIdx = cIdx, RowIdx = rIdx, SourceName = propertyPath });
                        }
                        else
                        if (vts.StartsWith("{*") && vts.EndsWith("*}"))
                        {
                            var propertyPath = vts.TrimStart('{').TrimEnd('}').Trim('*');
                            var replaceString = string.Empty;

                            try
                            {
                                var eval = Eval(o, propertyPath);
                                if (eval is Image)
                                {
                                    sheet.Cells[rIdx, cIdx].Value = "";
                                    var picture = sheet.Drawings.AddPicture(propertyPath, (Image)eval);
                                    picture.SetPosition(rIdx, 0, cIdx, 0);
                                }
                                else
                                {
                                    if (eval != null)
                                    {
                                        replaceString = eval.ToString();
                                    }
                                    sheet.Cells[rIdx, cIdx].Value = replaceString;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            int shift = 0;
            foreach (var spec in rows)
            {
                ProcessRowsSpec(sheet, spec, o, ref shift, dynColumns, endColumn);
            }
        }

        private void ProcessRowsSpec(ExcelWorksheet sheet, RowSpecs spec, object o, ref int shift, List<ReportDynamicColumn> dynColumns, int dynColStart)
        {
            var rows = (IEnumerable<object>)Eval(o, spec.SourceName);
            var count = rows.Count();
            int startRow = spec.RowIdx + shift + 1;
            int endRow = startRow + count - 1;

            sheet.InsertRow(startRow, count, spec.RowIdx + shift);

            var startColumn = sheet.Dimension.Start.Column;
            var endColumn = sheet.Dimension.End.Column;

            var iterator = rows.GetEnumerator();

            for (int rIdx = startRow; rIdx <= endRow; rIdx++)
            {
                iterator.MoveNext();
                for (int cIdx = startColumn; cIdx <= endColumn; cIdx++)
                {
                    var nValue = sheet.Cells[spec.RowIdx + shift, cIdx].Value;
                    if (nValue == null)
                        continue;
                    var strValue = nValue.ToString();
                    if (strValue.StartsWith("$") && strValue.EndsWith("$"))
                    {
                        continue;
                    }
                    else if (strValue == "{RowNumber}")
                    {
                        nValue = rIdx - startRow + 1;
                    }
                    else if (strValue.StartsWith("{") && strValue.EndsWith("}"))
                    {
                        nValue = Eval(iterator.Current, strValue.Substring(1, strValue.Length - 2));
                    }

                    sheet.Cells[rIdx, cIdx].Value = nValue;
                }

                if (dynColumns != null)
                    for (int i = 1; i <= dynColumns.Count; i++)
                    {
                        var c = dynColumns[i - 1];
                        sheet.Cells[rIdx, dynColStart - 1].Copy(sheet.Cells[rIdx, dynColStart + i]);
                        sheet.Cells[rIdx, dynColStart + i].Value = c.GetValue(Eval(iterator.Current, "dynColumn"));
                    }
            }

            sheet.DeleteRow(spec.RowIdx + shift);

            shift += count - 1;
        }

        Dictionary<Tuple<Type, string>, MethodInfo> getters = new Dictionary<Tuple<Type, string>, MethodInfo>();

        private object Eval(object o, string propertyPath)
        {
            var dotIndex = propertyPath.IndexOf('.');
            if (dotIndex < 0)
                return InvokeValue(o, propertyPath);
            return Eval(InvokeValue(o, propertyPath.Substring(0, dotIndex)), propertyPath.Substring(dotIndex + 1));
        }

        private object InvokeValue(object o, string field)
        {
            if (o == null)
                return null;
            var type = o.GetType();
            MethodInfo info;
            var tuple = Tuple.Create(type, field);
            if (!getters.TryGetValue(tuple, out info))
            {
                var propertyInfo = type.GetProperty(field);
                if (propertyInfo == null)
                    return "Не найдено свойство " + field + " у типа " + type.Name;
                info = getters[tuple] = propertyInfo.GetMethod; //не уверен что это что-то ускорит, но пусть будет
            }
            var result = info.Invoke(o, null);
            if (result is bool)
            {
                return ((bool)result) ? "Да" : "Нет";
            }
            if (result is Enum)
            {
                return ((Enum)result).ConvertToName();
            }
            if (result is IEnumerable<string>)
            {
                return string.Join(", ", (IEnumerable<string>)result);
            }
            return result;
        }
    }

    public class ReportDynamicColumn
    {
        public Func<string> GetName { get; set; }
        public Func<object, object> GetValue { get; set; }
    }

    internal class RowSpecs
    {
        public int RowIdx { get; set; }
        public int ColIdx { get; set; }
        public string SourceName { get; set; }
    }
}