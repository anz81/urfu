#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5ef115d9a1051abef633b45263ffdd4f05d98e0f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Variant_BasicContentEditold), @"mvc.1.0.view", @"/Views/Variant/BasicContentEditold.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\_ViewImports.cshtml"
using Urfu.Its.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\_ViewImports.cshtml"
using Urfu.Its.Web.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
using Urfu.Its.Web.DataContext;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"5ef115d9a1051abef633b45263ffdd4f05d98e0f", @"/Views/Variant/BasicContentEditold.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Variant_BasicContentEditold : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Model.Models.EditVariantContentViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<h3>");
#nullable restore
#line 4 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h3>\r\n\r\n<nav class=\"navbar navbar-default\">\r\n    <ul class=\"nav navbar-nav\">\r\n        <li>");
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.ActionLink("Лимиты", "Index", "EduProgramLimits", new {variantId = Model.VariantId}, null));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\r\n        <li>");
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.ActionLink("Управление группами выбора", "Index", "VariantSelectionGroup", new {variantId = Model.VariantId}, null));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\r\n        <li>");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.ActionLink("Редактирование модулей", "Index", "VariantContents", new {variantId = Model.VariantId}, null));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\r\n        <li>");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.ActionLink("Открыть в Excel", "Download", new {variantId = Model.VariantId}));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\r\n    </ul>\r\n</nav>\r\n\r\n");
            WriteLiteral("\r\n<p id=\"state\">\r\n    <b>Состояние:</b>\r\n    <button");
            BeginWriteAttribute("class", " class=\"", 903, "\"", 978, 2);
            WriteAttributeValue("", 911, "btn", 911, 3, true);
#nullable restore
#line 23 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
WriteAttributeValue("", 914, Model.State == VariantState.Development ? " btn-primary" : "", 914, 64, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" data-val=\"");
#nullable restore
#line 23 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                                                                                              Write((int) VariantState.Development);

#line default
#line hidden
#nullable disable
            WriteLiteral("\">Формируется</button>\r\n    <button");
            BeginWriteAttribute("class", " class=\"", 1058, "\"", 1128, 2);
            WriteAttributeValue("", 1066, "btn", 1066, 3, true);
#nullable restore
#line 24 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
WriteAttributeValue("", 1069, Model.State == VariantState.Review ? " btn-primary" : "", 1069, 59, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" data-val=\"");
#nullable restore
#line 24 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                                                                                         Write((int) VariantState.Review);

#line default
#line hidden
#nullable disable
            WriteLiteral("\">На согласовании</button>\r\n    <button");
            BeginWriteAttribute("class", " class=\"", 1207, "\"", 1279, 2);
            WriteAttributeValue("", 1215, "btn", 1215, 3, true);
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
WriteAttributeValue("", 1218, Model.State == VariantState.Approved ? " btn-primary" : "", 1218, 61, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" data-val=\"");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                                                                                           Write((int) VariantState.Approved);

#line default
#line hidden
#nullable disable
            WriteLiteral("\">Утверждена</button>\r\n</p>\r\n\r\n<table class=\"table table-bordered\">\r\n    <thead>\r\n    <tr>\r\n        <th>\r\n            ");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.DisplayNameFor(model => model.EduProgram));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.DisplayNameFor(model => model.qualification));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 38 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.DisplayNameFor(model => model.familirizationType));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 41 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.DisplayNameFor(model => model.familirizationCondition));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n    </tr>\r\n    </thead>\r\n    <tbody>\r\n    <tr>\r\n        <td>\r\n            ");
#nullable restore
#line 48 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.DisplayFor(model => model.EduProgram));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n        <td>\r\n            ");
#nullable restore
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.DisplayFor(model => model.qualification));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n        <td>\r\n            ");
#nullable restore
#line 54 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.DisplayFor(model => model.familirizationType));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n        <td>\r\n            ");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
       Write(Html.DisplayFor(model => model.familirizationCondition));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    </tbody>\r\n</table>\r\n");
#nullable restore
#line 94 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
 using (Html.BeginForm("BasicContentEditold", "Variant", new { variantId = Model.VariantId }, FormMethod.Post, new { encType = "multipart/form-data", name = "myform" }))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 96 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        <table>\r\n            <tr>\r\n                <td style=\"width: 600px;\">\r\n                    ");
#nullable restore
#line 101 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
               Write(Html.ValidationSummary(true));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    <div class=\"form-group\">\r\n                        <div class=\"col-md-10\">\r\n                            ");
#nullable restore
#line 104 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.HiddenFor(model => model.VariantId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 105 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.DisplayFor(modelItem => modelItem.Message));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n                    <div class=\"form-group\">\r\n                        ");
#nullable restore
#line 111 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                   Write(Html.LabelFor(model => model.Name, new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        <div class=\"col-md-10\">\r\n                            ");
#nullable restore
#line 113 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.EditorFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 114 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.ValidationMessageFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n\r\n                    <div class=\"form-group\">\r\n                        ");
#nullable restore
#line 119 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                   Write(Html.LabelFor(model => model.StudentsLimit, new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        <div class=\"col-md-10\">\r\n                            ");
#nullable restore
#line 121 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.EditorFor(model => model.StudentsLimit));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 122 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.ValidationMessageFor(model => model.StudentsLimit));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n\r\n                    <div class=\"form-group\">\r\n                        ");
#nullable restore
#line 127 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                   Write(Html.LabelFor(model => model.Year, new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        <div class=\"col-md-10\">\r\n                            ");
#nullable restore
#line 129 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.EditorFor(model => model.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 130 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.ValidationMessageFor(model => model.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </div>\r\n\r\n                    </div>\r\n\r\n                    <div class=\"form-group\">\r\n                        ");
#nullable restore
#line 136 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                   Write(Html.LabelFor(model => model.SelectionDeadline, new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        <div class=\"col-md-10\">\r\n                            ");
#nullable restore
#line 138 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.EditorFor(model => model.SelectionDeadline, new {@class = "form-control datecontrol"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 139 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.ValidationMessageFor(model => model.SelectionDeadline));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                        </div>
                    </div>
                </td>
                <td>
                    <input type=""submit"" value=""Сохранить"" class=""btn btn-default"" />
                </td>
            </tr>
        </table>
        <p>
            Группы
        </p>

        <p>
            <table class=""table table-striped table-hover table-bordered table-condensed"">
                <tr>
                    <th>");
#nullable restore
#line 155 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                           int rowNum = 1;

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n                    <th style=\"width:10%;\">\r\n                        Группа\r\n                    </th>\r\n                    <th>\r\n                        Зачётные единицы в группе\r\n                    </th>\r\n");
#nullable restore
#line 162 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                         for (int term = 1; term <= 8; term++)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <th>\r\n                                Семестр ");
#nullable restore
#line 165 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                                   Write(term);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </th>\r\n");
#nullable restore
#line 167 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <th>\r\n                        Сумма зачётных единиц по модулям\r\n                    </th>\r\n                </tr>\r\n");
#nullable restore
#line 172 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                 for (int i = 0; i < Model.Groups.Count; i++)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        ");
#nullable restore
#line 175 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                   Write(Html.HiddenFor(model => model.Groups[i].Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        ");
#nullable restore
#line 176 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                   Write(Html.HiddenFor(model => model.Groups[i].GroupType));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        <td>");
#nullable restore
#line 177 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.Raw(rowNum++));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>\r\n                            ");
#nullable restore
#line 179 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.DisplayFor(model => model.Groups[i].GroupType));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td>\r\n                            ");
#nullable restore
#line 182 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                       Write(Html.EditorFor(model => model.Groups[i].TestUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n");
#nullable restore
#line 184 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                         for (int term = 1; term <= 8; term++)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <td>\r\n                             ");
#nullable restore
#line 187 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                         Write(Model.Rows.Where(r => r.GroupType == Model.Groups[i].GroupType && (r.Selected || r.Base) && !r.SelectionGroupId.HasValue).SelectMany(r=>r.Plans.Where(p=>p.GetTermTestUnits(term)>0)/*.Take(1)*/).Sum(p => p.GetTermTestUnits(term)));

#line default
#line hidden
#nullable disable
            WriteLiteral(" <br />\r\n                        </td>\r\n");
#nullable restore
#line 189 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"

                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <td>\r\n                            Без групп выбора: ");
#nullable restore
#line 192 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                                          Write(Model.Rows.Where(r => r.GroupType == Model.Groups[i].GroupType && (r.Selected || r.Base) && !r.SelectionGroupId.HasValue).Sum(r => r.TestUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral(" <br/>\r\n                            По группам выбора: ");
#nullable restore
#line 193 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                                           Write(Model.AllSelectionGroups.Where(sg => Model.Rows.Any(r => r.SelectionGroupId == sg.Id && r.GroupType == Model.Groups[i].GroupType && (r.Selected || r.Base))).Sum(r => r.TestUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 196 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td></td>\r\n                    <td>\r\n                        <b>Итого</b>\r\n                    </td>\r\n                    <td>\r\n");
            WriteLiteral("                    </td>\r\n");
#nullable restore
#line 205 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                     for (int term = 1; term <= 8; term++)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <td>\r\n                            ");
#nullable restore
#line 208 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                        Write(Model.Rows.Where(r => (r.Selected || r.Base) && !r.SelectionGroupId.HasValue).SelectMany(r => r.Plans.Where(p => p.GetTermTestUnits(term) > 0)/*.Take(1)*/).Sum(p => p.GetTermTestUnits(term)));

#line default
#line hidden
#nullable disable
            WriteLiteral(" <br />\r\n                        </td>\r\n");
#nullable restore
#line 210 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"

                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <td>\r\n                        Без групп выбора: ");
#nullable restore
#line 213 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                                      Write(Model.Rows.Where(r => (r.Selected || r.Base) && !r.SelectionGroupId.HasValue).Sum(r => r.TestUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral(" <br />\r\n                        По группам выбора: ");
#nullable restore
#line 214 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                                       Write(Model.AllSelectionGroups.Where(sg => Model.Rows.Any(r => r.SelectionGroupId == sg.Id && (r.Selected || r.Base))).Sum(r => r.TestUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                    </td>
                </tr>

            </table>

        </p>

        <p>
            Модули
        </p>

        <table class=""table table-striped table-hover table-bordered table-condensed"" data-datatable=""true"" id=""mainVariantTable"">
            <thead>
                <tr>
                    <th style=""width: 2%;""></th>>
                    <th style=""width: 4%;"">
                        ");
#nullable restore
#line 231 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
                    Write(Model.IsBase?"Включить модуль в основную траекторию":"Включить модуль в траекторию");

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                    </th>
                    <th style=""width: 10%;"">
                        Название модуля
                    </th>
                    <th style=""width: 5%;"">
                        Зачётные единицы
                    </th>
                    <th style=""width: 7%;"">
                        Группа модуля
                    </th>
                    <th style=""width: 28%;"">
                        Тип модуля
                    </th>
                    <th style=""width: 4%;"">
                        Признак 'По выбору'
                    </th>
                    <th style=""width: 6%;"">
                        Лимит
                    </th>
                    <th style=""width: 6%;"">
                        Семестры
                    </th>
                    <th style=""width: 20%;"">
                        Группа выбора
                    </th>
                    <th style=""width: 10%;"">
                        Учебные планы
                    </th");
            WriteLiteral(">\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n                ");
#nullable restore
#line 263 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
           Write(Html.EditorFor(model => model.Rows));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
            </tbody>
        </table>
        <br />
        <hr />

        <div class=""form-group"">
            <div class=""col-md-offset-2 col-md-10"">
                <input type=""submit"" value=""Сохранить"" class=""btn btn-default"" />
            </div>
        </div>
    </div>
");
#nullable restore
#line 275 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\BasicContentEditold.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script>
        if (!Modernizr.inputtypes.date) {
            $(function() {
                $(""input[type='date']"")
                    .datepicker({ dateFormat: 'yy-mm-dd' })//, $.datepicker.regional['ru'])
                    .get(0)
                    .setAttribute(""type"", ""text"");
            });
        }

        $(""#state button"").not("".btn-primary"").click(function () {
            var button = $(this);
            $.post(""/Variant/ChangeState"", { ""variantId"" : $.url(""?variantId"") || $.url(""?VariantId""), ""state"": button.data(""val"") }, function (data, textStatus) {
                if (data.result) {
                    window.location.reload();
                } else {
                    alert(""Не удалось сменить состояние документа, недостаточно прав"" );
                }
            });
            
        });
    </script>
");
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Model.Models.EditVariantContentViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
