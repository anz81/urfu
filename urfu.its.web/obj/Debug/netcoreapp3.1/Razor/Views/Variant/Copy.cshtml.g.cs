#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fd67d09626d360ad5f74b54a5199135a1184805c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Variant_Copy), @"mvc.1.0.view", @"/Views/Variant/Copy.cshtml")]
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
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
using PagedList;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
using PagedList.Mvc;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fd67d09626d360ad5f74b54a5199135a1184805c", @"/Views/Variant/Copy.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Variant_Copy : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<Urfu.Its.Web.DataContext.EduProgram>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 5 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
  
    ViewBag.Title = "Копирование траектории "+ViewBag.Variant.Name;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
 using (Html.BeginForm("Copy", "Variant", FormMethod.Get))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
Write(Html.Hidden("variantId", @ViewBag.Variant.Id as string));

#line default
#line hidden
#nullable disable
            WriteLiteral("    <table class=\"table\">\r\n        <thead>\r\n            <tr>\r\n                <th>\r\n                    ");
#nullable restore
#line 18 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayNameFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </th>\r\n                <th>\r\n                    ");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayNameFor(model => model.Profile.NAME));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </th>\r\n                <th>\r\n                    ");
#nullable restore
#line 24 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayNameFor(model => model.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n        <tbody>\r\n            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.TextBox("name", (object)ViewBag.name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 34 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.TextBox("profileName", (object)ViewBag.profileName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 37 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.TextBox("year", (object)ViewBag.year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td colspan=\"11\"><input type=\"submit\" value=\"Применить\" /> | ");
#nullable restore
#line 41 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
                                                                        Write(Html.ActionLink("Очистить фильтры", "Copy", "Variant", new { variantId = @ViewBag.Variant.Id }, null));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n");
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n<table class=\"table\" id=\"tablePrograms\">\r\n    <thead>\r\n\r\n        <tr>\r\n            <th>\r\n                ");
#nullable restore
#line 53 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.Direction.okso));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.Direction.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 59 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 62 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.HeadFullName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.qualification));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                Подразделение\r\n            </th>\r\n            <th>\r\n                Кафедра\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 74 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.Profile.NAME));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 77 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.familirizationType));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 80 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.familirizationCondition));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 83 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
           Write(Html.DisplayNameFor(model => model.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th></th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n");
#nullable restore
#line 89 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
         foreach (var item in Model)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 93 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.Direction.okso));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 96 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.Direction.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 99 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 102 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.HeadFullName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 105 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.qualification));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 108 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.Division.shortTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 111 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.Chair.shortTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 114 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.Profile.NAME));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 117 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.familirizationType));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 120 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.familirizationCondition));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 123 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.DisplayFor(modelItem => item.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 126 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
               Write(Html.ActionLink("Добавить в программу", "CopyExecute", new {ViewBag.id, programId = item.Id }));

#line default
#line hidden
#nullable disable
            WriteLiteral(" \r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 129 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n\r\n");
#nullable restore
#line 133 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Variant\Copy.cshtml"
Write(Html.PagedListPager((IPagedList)Model, page => Url.Action("Copy",
    new
    {
        page,
        id = ViewBag.id,
        name = ViewBag.name,
        qualification = ViewBag.qualification,
        divisionShortTitle = ViewBag.divisionShortTitle,
        chairShortTitle = ViewBag.chairShortTitle,
        profileName = ViewBag.profileName,
        familirizationType = ViewBag.familirizationType,
        familirizationCondition = ViewBag.familirizationCondition,
        year = ViewBag.year
    })));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<Urfu.Its.Web.DataContext.EduProgram>> Html { get; private set; }
    }
}
#pragma warning restore 1591