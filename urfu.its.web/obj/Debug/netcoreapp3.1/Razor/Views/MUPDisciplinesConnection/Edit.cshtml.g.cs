#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "06561ded5bbd99b58215c92ea089ee8cfde72e51"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_MUPDisciplinesConnection_Edit), @"mvc.1.0.view", @"/Views/MUPDisciplinesConnection/Edit.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"06561ded5bbd99b58215c92ea089ee8cfde72e51", @"/Views/MUPDisciplinesConnection/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_MUPDisciplinesConnection_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Models.MUPEditViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/bundles/jqueryval"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
  
    ViewBag.Title = "Редактирование модуля МУП";

#line default
#line hidden
#nullable disable
            WriteLiteral("<h2>");
#nullable restore
#line 5 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n");
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
 using (Html.BeginForm())
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        <hr />\r\n        ");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
   Write(Html.ValidationSummary(true, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        ");
#nullable restore
#line 14 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
   Write(Html.HiddenFor(model => model.Module.uuid));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
   Write(Html.HiddenFor(model => model.moduleUUId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        <div>\r\n            <dl class=\"dl-horizontal\">\r\n                <dt>\r\n                    ");
#nullable restore
#line 19 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 22 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.shortTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.shortTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.coordinator));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 34 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.coordinator));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 37 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.type));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.type));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 43 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.competence));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 46 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.competence));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.testUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 52 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.testUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 55 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.priority));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 58 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.priority));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 61 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.state));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 64 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.state));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 67 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.approvedDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 70 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.approvedDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 73 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.comment));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 76 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.comment));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 79 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.file));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 82 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.file));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n                <dt>\r\n                    ");
#nullable restore
#line 85 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayNameFor(model => model.Module.specialities));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dt>\r\n                <dd>\r\n                    ");
#nullable restore
#line 88 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.Module.specialities));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </dd>\r\n            </dl>\r\n        </div>\r\n\r\n        <hr />\r\n\r\n");
#nullable restore
#line 95 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
         if (ViewBag.CanEdit)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"form-group\">\r\n                ");
#nullable restore
#line 98 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
           Write(Html.LabelFor(model => model.Module.MUP.Tech, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                <div class=\"col-md-10\">\r\n                    ");
#nullable restore
#line 100 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DropDownListFor(model => model.techid, Model.TechSelector, htmlAttributes: new { @class = "form-control" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </div>\r\n            </div>\r\n            <div class=\"form-group\">\r\n                ");
#nullable restore
#line 104 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
           Write(Html.LabelFor(model => model.showInLc, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                <div class=\"col-md-10\">\r\n                    ");
#nullable restore
#line 106 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.EditorFor(model => model.showInLc, new { htmlAttributes = new { @class = "checkbox" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    ");
#nullable restore
#line 107 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.ValidationMessageFor(model => model.showInLc, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </div>\r\n            </div>\r\n");
#nullable restore
#line 110 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"form-group\">\r\n                ");
#nullable restore
#line 114 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
           Write(Html.LabelFor(model => model.Module.MUP.Tech, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                <div class=\"col-md-10\">\r\n                    ");
#nullable restore
#line 116 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.tech, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </div>\r\n            </div>\r\n            <div class=\"form-group\">\r\n                ");
#nullable restore
#line 120 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
           Write(Html.LabelFor(model => model.showInLc, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                <div class=\"col-md-10\">\r\n                    ");
#nullable restore
#line 122 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
               Write(Html.DisplayFor(model => model.showInLc, new { htmlAttributes = new { @class = "checkbox" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </div>\r\n            </div>\r\n");
#nullable restore
#line 125 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("        <h4>Периоды</h4>\r\n");
#nullable restore
#line 127 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
         if (ViewBag.CanEdit)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            <table class=""table table-nonfluid"">
                <thead>
                    <tr>
                        <th width=""150px"">Год</th>
                        <th width=""150px"">Семестр</th>
                        <th width=""150px"">Курс</th>
                        <th width=""200px"">Выбрать до</th>
                        <th width=""100px""></th>
                    </tr>
                </thead>
                <tbody id=""periodsBlock"">
");
#nullable restore
#line 140 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                     for (var i = 0; i < Model.periods.Count; i++)
                    {
                        var p = Model.periods[i];

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <tr>\r\n                            ");
#nullable restore
#line 144 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                       Write(Html.HiddenFor(m => m.periods[i].id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 145 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                       Write(Html.HiddenFor(m => m.periods[i].isDeleted));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            <td>\r\n                                ");
#nullable restore
#line 147 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.EditorFor(m => m.periods[i].year, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                ");
#nullable restore
#line 148 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.ValidationMessageFor(m => m.periods[i].year, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td>");
#nullable restore
#line 150 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.DropDownListFor(m => m.periods[i].semesterId, p.Selector, htmlAttributes: new { @class = "form-control" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 151 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.DropDownListFor(m => m.periods[i].Course, p.CourseSelector, htmlAttributes: new { @class = "form-control" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 152 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.EditorFor(m => m.periods[i].selectionDeadline, new { htmlAttributes = new { @class = "form-control datecontrol", type = "date" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n\r\n\r\n\r\n                            <td><input type=\"button\" value=\"Удалить\"");
            BeginWriteAttribute("onclick", " onclick=\"", 6716, "\"", 6750, 3);
            WriteAttributeValue("", 6726, "delPeriodClick(this,", 6726, 20, true);
#nullable restore
#line 156 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
WriteAttributeValue(" ", 6746, i, 6747, 2, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6749, ")", 6749, 1, true);
            EndWriteAttribute();
            WriteLiteral(" /></td>\r\n                        </tr>\r\n");
#nullable restore
#line 158 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </tbody>\r\n            </table>\r\n");
            WriteLiteral("            <a class=\"addPeriod\">Добавить период</a>\r\n");
#nullable restore
#line 163 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            <table class=""table table-nonfluid"">
                <thead>
                    <tr>
                        <th width=""150px"">Год</th>
                        <th width=""150px"">Семестр</th>
                        <th width=""150px"">Курс</th>
                        <th width=""200px"">Выбрать до</th>

                    </tr>
                </thead>
                <tbody id=""periodsBlock"">
");
#nullable restore
#line 177 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                     for (var i = 0; i < Model.periods.Count; i++)
                    {
                        var p = Model.periods[i];

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <tr>\r\n                            ");
#nullable restore
#line 181 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                       Write(Html.HiddenFor(m => m.periods[i].id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 182 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                       Write(Html.HiddenFor(m => m.periods[i].isDeleted));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            <td>\r\n                                ");
#nullable restore
#line 184 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.DisplayFor(m => m.periods[i].year, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td>\r\n                                ");
#nullable restore
#line 187 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.DisplayFor(m => m.periods[i].semesterName, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td>");
#nullable restore
#line 189 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.DisplayFor(m => m.periods[i].Course, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>\r\n                                ");
#nullable restore
#line 191 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                           Write(Html.DisplayFor(m => m.periods[i].selectionDeadline, new { htmlAttributes = new { @class = "form-control datecontrol", type = "date" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n\r\n                        </tr>\r\n");
#nullable restore
#line 195 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </tbody>\r\n            </table>\r\n");
#nullable restore
#line 198 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("        <hr />\r\n\r\n");
#nullable restore
#line 201 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
         if (ViewBag.CanEdit)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"form-group\">\r\n                <div class=\"col-md-offset-2 col-md-10\">\r\n                    <input type=\"submit\" value=\"Сохранить\" class=\"btn btn-default\" />\r\n                </div>\r\n            </div>\r\n");
#nullable restore
#line 208 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\r\n");
#nullable restore
#line 210 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("<div>\r\n    ");
#nullable restore
#line 212 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
Write(Html.ActionLink("Вернуться к списку", "Index", new { focus = Model.moduleUUId }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "06561ded5bbd99b58215c92ea089ee8cfde72e5127722", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n\r\n    <script>\r\n    if (!Modernizr.inputtypes.date) {\r\n    $(function() {\r\n        $(\"input[type=\'date\']\")\r\n            .datepicker({ dateFormat: \'yy-mm-dd\' });\r\n\r\n        });\r\n    }\r\n\r\n    $(function () {\r\n        var i = ");
#nullable restore
#line 229 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPDisciplinesConnection\Edit.cshtml"
           Write(Model.periods.Count);

#line default
#line hidden
#nullable disable
                WriteLiteral(@";
       $('.addPeriod').click(function ()
       {
            var html2Add =
 ""<tr>"" +
   ""<input data-val='true' data-val-number='The field id must be a number.' data-val-required='The id field is required.' id='periods_"" + i + ""__id' name='periods["" + i + ""].id' type='hidden' value='"" + (-i) + ""'>""+
 ""<input data-val='true' id='periods_"" + i + ""__isDeleted' name='periods["" + i + ""].isDeleted' type='hidden' value='False' />"" +
 ""<td><input class='form-control text-box single-line' data-val='true' data-val-number='The field year must be a number.' data-val-required='The year field is required.' id='periods_"" + i + ""__year' name='periods["" + i + ""].year' size='50' type='number' /></td>"" +
 ""<td><select id='periods_"" + i + ""__semesterId' name='periods["" + i + ""].semesterId' class='form-control'>"" +
 ""<option value='0'>Прочий</option>""+
 ""<option value='1'>Осенний</option>""+
 ""<option value='2'>Весенний</option>""+
""</select></td>"" +
""<td><select id='periods_"" + i + ""__Course' name='periods["" + i +");
                WriteLiteral(@" ""].Course' class='form-control'>"" +
""<option value=''>Все\</option>"" +
""<option value='1'>1</option>"" +
""<option value='2'>2</option>"" +
""<option value='3'>3</option>"" +
""<option value='4'>4</option>"" +
""<option value='5'>5</option>"" +
""</select></td>"" +
 ""<td><input class='form-control datecontrol text-box single-line' data-val='true' data-val-date='The field SelectionDeadline must be a date.' id='periods_"" + i + ""__selectionDeadline' name='periods["" + i + ""].selectionDeadline' type='date' value='' /></td>"" +
 ""<td><input type='button' value='Удалить' onclick='delPeriodClick(this, "" + i + "")'/></td>"" +
 ""</tr>"";
           $('#periodsBlock').append(html2Add);
            i++;

           $(""input[type='date']"")
                .datepicker({ dateFormat: 'yy-mm-dd' })
                .get(0)
                .setAttribute(""type"", ""text"");

        })

        delPeriodClick = function (button, i)
        {
            $(button.parentNode.parentNode).hide();
            $('#periods_' + i");
                WriteLiteral(" + \'__isDeleted\').attr(\"value\", true);\r\n            $(\'#periods_\' + i + \'__year\').attr(\"value\", 0);\r\n        }\r\n\r\n    })\r\n    </script>\r\n\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Models.MUPEditViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
