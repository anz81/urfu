#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c76f560009707119a3e3756138e95fb02f2b9d34"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Modules_Details), @"mvc.1.0.view", @"/Views/Modules/Details.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c76f560009707119a3e3756138e95fb02f2b9d34", @"/Views/Modules/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Modules_Details : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.Module>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
  
    ViewBag.Title = "Модуль \"" + @Model.title + '"';

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n<div>\r\n    <hr />\r\n    <dl class=\"dl-horizontal\">\r\n        <dt>\r\n            ");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.shortTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.shortTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.coordinator));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.coordinator));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 37 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.type));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 41 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.type));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.competence));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.competence));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 53 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.testUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.testUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 61 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.priority));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.priority));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 69 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.state));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 73 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.state));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 77 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.approvedDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 81 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.approvedDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 85 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.comment));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 89 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.comment));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 93 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.file));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 97 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.file));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n        <dt>\r\n            ");
#nullable restore
#line 101 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.specialities));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n\r\n        <dd>\r\n            ");
#nullable restore
#line 105 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
       Write(Html.DisplayFor(model => model.specialities));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n\r\n    </dl>\r\n</div>\r\n<p>\r\n    ");
#nullable restore
#line 111 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
Write(Html.ActionLink("Изменить", "Edit", new { id = Model.uuid }));

#line default
#line hidden
#nullable disable
            WriteLiteral(" |\r\n    ");
#nullable restore
#line 112 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Details.cshtml"
Write(Html.ActionLink("Вернуться к списку", "Index"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</p>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.Module> Html { get; private set; }
    }
}
#pragma warning restore 1591