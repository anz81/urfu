#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0611575d68c22a43d931cd4c0dc41b3a852f502a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Variant_Plans), @"mvc.1.0.view", @"/Views/Variant/Plans.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0611575d68c22a43d931cd4c0dc41b3a852f502a", @"/Views/Variant/Plans.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Variant_Plans : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<Urfu.Its.Web.DataContext.Plan>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
  
    if (ViewBag.Title == null)
    {
        ViewBag.Title = "Дисциплины модуля";
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n<p><b>Направление:</b> ");
#nullable restore
#line 12 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                  Write(ViewBag.Direction.okso);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n\r\n<p>\r\n");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
     if (ViewBag.hideEditLinks == null || !ViewBag.hideEditLinks)
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
   Write(Html.ActionLink((string) ViewBag.BackButtonText, "BasicContentEdit", "Variant", new {variantId = ViewBag.VariantId}, null));

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                                                                                                                                   
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n\r\n<table class=\"table table-striped table-hover table-bordered table-condensed\">\r\n    <tr>\r\n        <th>\r\n            ");
#nullable restore
#line 24 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
       Write(Html.DisplayNameFor(model => model.eduplanNumber));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 27 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
       Write(Html.DisplayNameFor(model => model.versionNumber));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 30 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
       Write(Html.DisplayNameFor(model => model.disciplineTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
       Write(Html.DisplayNameFor(model => model.controls));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
       Write(Html.DisplayNameFor(model => model.loads));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 39 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
       Write(Html.DisplayNameFor(model => model.testUnitsByTerm));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th>\r\n            ");
#nullable restore
#line 42 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
       Write(Html.DisplayNameFor(model => model.terms));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th></th>\r\n    </tr>\r\n\r\n");
#nullable restore
#line 47 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
     foreach (var item in Model)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <tr>\r\n            <td>\r\n                ");
#nullable restore
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
           Write(Html.DisplayFor(modelItem => item.eduplanNumber));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 54 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
           Write(Html.DisplayFor(modelItem => item.versionNumber));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
           Write(Html.DisplayFor(modelItem => item.disciplineTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 60 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
           Write(Html.DisplayFor(modelItem => item.controls));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 63 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
           Write(Html.DisplayFor(modelItem => item.loads));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 66 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
           Write(Html.DisplayFor(modelItem => item.testUnitsByTerm));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 69 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
           Write(Html.DisplayFor(modelItem => item.terms));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n");
#nullable restore
#line 72 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                 if (ViewBag.hideEditLinks == null || !ViewBag.hideEditLinks)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <p>");
#nullable restore
#line 74 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                  Write(Html.ActionLink("Редактирование преподавателей ", "Teachers", new {moduleId = item.Module.uuid, item.eduplanUUID, ViewBag.VariantId, item.catalogDisciplineUUID}));

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n");
#nullable restore
#line 75 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </td>\r\n        </tr>\r\n");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
        if (item.Teachers.Any())
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    Преподаватели:\r\n                </td>\r\n                <td colspan=\"7\">\r\n                    <table class=\"table\">\r\n");
#nullable restore
#line 86 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                         foreach (var pt in item.Teachers)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <tr>\r\n                                <td>");
#nullable restore
#line 89 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                               Write(pt.load);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                                <td>\r\n                                    <p>");
#nullable restore
#line 91 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                                  Write(pt.Teacher.initials);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </p>\r\n                                </td>\r\n                                <td>");
#nullable restore
#line 93 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                               Write(pt.Teacher.workPlace);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            </tr>\r\n");
#nullable restore
#line 95 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </table>\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 99 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr></tr>\r\n");
#nullable restore
#line 103 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Plans.cshtml"
        }
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</table>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<Urfu.Its.Web.DataContext.Plan>> Html { get; private set; }
    }
}
#pragma warning restore 1591
