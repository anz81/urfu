#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "52591b203a0d197fb409e32080665379722a75db"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ProjectSubgroupMeta_Edit), @"mvc.1.0.view", @"/Views/ProjectSubgroupMeta/Edit.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"52591b203a0d197fb409e32080665379722a75db", @"/Views/ProjectSubgroupMeta/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ProjectSubgroupMeta_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.ProjectDisciplineTmerPeriod>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 4 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
  
    ViewBag.Title = "Редактирование количества подгрупп";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
 using (Html.BeginForm())
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        <h4>Редактирование количества подгрупп</h4>\r\n        <hr />\r\n        ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
   Write(Html.ValidationSummary(true, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        ");
#nullable restore
#line 16 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
   Write(Html.HiddenFor(model => model.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        <input type=\"hidden\" name=\"competitionGroupId\"");
            BeginWriteAttribute("value", " value=\"", 472, "\"", 507, 1);
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
WriteAttributeValue("", 480, ViewBag.competitionGroupId, 480, 27, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 19 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
       Write(Html.LabelFor(model => model.Period.Project.Module.numberAndTitle, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
           Write(Html.DisplayFor(model => model.Period.Project.Module.numberAndTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 26 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
       Write(Html.LabelFor(model => model.Period.Year, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
           Write(Html.DisplayFor(model => model.Period.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
       Write(Html.LabelFor(model => model.Period.Semester, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
           Write(Html.DisplayFor(model => model.Period.Semester.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
       Write(Html.LabelFor(model => model.Tmer.Tmer.rmer, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 42 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
           Write(Html.DisplayFor(model => model.Tmer.Tmer.rmer));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            <label class=\"control-label col-md-2\" for=\"catalogDisciplineUuid\">Дисциплина</label>\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 48 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
           Write(Html.DisplayFor(model => model.Tmer.Discipline.Discipline.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 52 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
       Write(Html.Label("", htmlAttributes: new { @class = "control-label col-md-2" }, labelText: "Количество групп"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
            <div class=""col-md-10"">
                <input class=""form-control text-box single-line"" data-val=""true"" data-val-number=""The field Колличество подгрупп must be a number."" data-val-required=""Требуется поле Колличество подгрупп."" id=""GroupCount"" name=""GroupCount"" type=""number""");
            BeginWriteAttribute("value", " value=\"", 2454, "\"", 2489, 1);
#nullable restore
#line 54 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
WriteAttributeValue("", 2462, ViewBag.ExpectedChildCount, 2462, 27, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n                <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"GroupCount\" data-valmsg-replace=\"true\"></span>\r\n            </div>\r\n        </div>\r\n\r\n");
#nullable restore
#line 59 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
         if (@Model.Tmer.Tmer.kmer == "TLEKC" || @Model.Tmer.Tmer.kmer == "TPRAK")
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"form-group\">\r\n                ");
#nullable restore
#line 62 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
           Write(Html.Label(Model.Tmer.Tmer.kmer == "TLEKC" ? "Распределение практических занятий" : "Распределение лабораторных занятий", "", htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                <div class=\"col-md-10\">\r\n                    ");
#nullable restore
#line 64 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
               Write(Html.EditorFor(model => model.Distribution, new { htmlAttributes = new { @class = "form-control", type = "hidden" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    ");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
               Write(Html.ValidationMessageFor(model => model.Distribution, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    <p id=\"distrbox\"></p>\r\n                    <p id=\"distroWarn\" class=\"text-danger\"></p>\r\n                </div>\r\n            </div>\r\n");
#nullable restore
#line 70 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 72 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
         if (@ViewBag.CanEdit)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"form-group\">\r\n                <div class=\"col-md-offset-2 col-md-10\">\r\n                    <input type=\"submit\" value=\"Сохранить\" class=\"btn btn-default\" />\r\n                </div>\r\n            </div>\r\n");
#nullable restore
#line 79 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n");
#nullable restore
#line 82 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n\r\n<div>\r\n    ");
#nullable restore
#line 87 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
Write(Html.ActionLink("Вернуться к списку", "Index", new { focus = Model.Id, competitionGroupId = ViewBag.competitionGroupId }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n\r\n\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n");
#nullable restore
#line 93 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
     if (@Model.Tmer.Tmer.kmer == "TLEKC" || @Model.Tmer.Tmer.kmer == "TPRAK")
    {

#line default
#line hidden
#nullable disable
                WriteLiteral(@"        <script>
            function computeWarning() {
                var total = 0;
                $(""#distrbox"").children().each(function (i) {
                    total += parseInt($(this).val());
                });
                var ec = '");
#nullable restore
#line 101 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
                     Write(ViewBag.ExpectedChildCount);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"';
                if (ec && ec.length > 0) {
                    $(""#distroWarn"").empty();
                    if (ec != total)
                        $(""#distroWarn"").text(""Сумма распределений равна "" + total + "" , количество подгрупп равно "" + ec);
                }
            };

            function rebuildDistr() {
                var str = """";
                $(""#distrbox"").children().each(function(i) {
                    if (str !== """")
                        str += "","";
                    str += $(this).val();
                });
                $(""#Distribution"").val(str);
                computeWarning();
            };

            function buildBoxes() {
                var count = $(""#GroupCount"").val();
                var distr = $(""#Distribution"").val().split("","");

                $(""#distrbox"").empty();
                for (var i = 0; i < count; i++) {
                    $(""#distrbox"").append('<input class=""form-control text-box single-line"" type=""number"" onch");
                WriteLiteral(@"ange=""rebuildDistr()"" value=""' + (distr[i]||1) + '"">');
                }
            };

            $(document).ready(function () {
                $(""#GroupCount"").change(function () {
                    buildBoxes();
                    rebuildDistr();
                });
                buildBoxes();
                computeWarning();
            });
        </script>
");
#nullable restore
#line 139 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectSubgroupMeta\Edit.cshtml"
    }

#line default
#line hidden
#nullable disable
            }
            );
            WriteLiteral("\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.ProjectDisciplineTmerPeriod> Html { get; private set; }
    }
}
#pragma warning restore 1591
