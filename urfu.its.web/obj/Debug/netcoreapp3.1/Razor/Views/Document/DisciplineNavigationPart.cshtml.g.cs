#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "946d33226f67833cb6e4036e3920a1e317d3faba"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Document_DisciplineNavigationPart), @"mvc.1.0.view", @"/Views/Document/DisciplineNavigationPart.cshtml")]
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
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
using Urfu.Its.Common;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"946d33226f67833cb6e4036e3920a1e317d3faba", @"/Views/Document/DisciplineNavigationPart.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Document_DisciplineNavigationPart : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.VersionedDocs.ViewModels.DisciplineWorkingProgramViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 4 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
  
    ViewBag.Title = Model.DisplayName;
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<a");
            BeginWriteAttribute("href", " href=\"", 200, "\"", 270, 1);
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
WriteAttributeValue("", 207, Url.Action("Index", new { id = Model.ModuleWorkingProgramId }), 207, 63, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">РПМ</a>\r\n\r\n<div class=\"page-header\">\r\n    <h1>");
#nullable restore
#line 12 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
   Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\r\n</div>\r\n\r\n<a class=\"btn btn-default\"");
            BeginWriteAttribute("href", " href=\"", 375, "\"", 450, 1);
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
WriteAttributeValue("", 382, Url.Action("Print", new { id = Model.DocumentId, format = "docx" }), 382, 68, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" download>Печатная форма</a>\r\n\r\n");
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
 if (Model.IsSchemaActual)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <ul class=\"list-group\" style=\"margin-top: 30px\">\r\n");
#nullable restore
#line 20 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
         foreach (var s in Model.Sections)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <li class=\"list-group-item\"><a");
            BeginWriteAttribute("href", " href=\"", 665, "\"", 819, 1);
#nullable restore
#line 22 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
WriteAttributeValue("", 672, Url.Action("Section", new RouteValueDictionary(Request.QueryStringToRouteValueDictionary()) {{"id", Model.DocumentId}, {"section", s.SystemName}}), 672, 147, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 22 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
                                                                                                                                                                                                 Write(s.DisplayName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a></li>\r\n");
#nullable restore
#line 23 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </ul>\r\n");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
}
else
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"alert alert-warning\" style=\"margin-top: 30px\">Схема документа устарела. Доступна только его <a class=\"alert-link\"");
            BeginWriteAttribute("href", " href=\"", 1009, "\"", 1067, 1);
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
WriteAttributeValue("", 1016, Url.Action("Print", new { id = Model.DocumentId }), 1016, 51, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" download>печатная форма</a>.</div>\r\n");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Document\DisciplineNavigationPart.cshtml"
}

#line default
#line hidden
#nullable disable
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.VersionedDocs.ViewModels.DisciplineWorkingProgramViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591