#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6f578f5a942993fe176ed9ba7ee98c0a44416115"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_SectionFK_CopyMembershipPrepare), @"mvc.1.0.view", @"/Views/SectionFK/CopyMembershipPrepare.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6f578f5a942993fe176ed9ba7ee98c0a44416115", @"/Views/SectionFK/CopyMembershipPrepare.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_SectionFK_CopyMembershipPrepare : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Controllers.SectionFKCompetitionGroupMembershipsCopyVm>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
  
    ViewBag.Title = "Копирование зачислений конкурсных групп";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 6 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n<div style=\"border: 1px;\">\r\n    Копировать из \r\n    \r\n    <div class=\"alert-info\">\r\n        ");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
   Write(Model.A.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(" (");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                  Write(Model.A.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                                Write(Model.A.Semester.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(" зачислено студентов ");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                                                                           Write(Model.ACount);

#line default
#line hidden
#nullable disable
            WriteLiteral(")\r\n    </div>\r\n    в \r\n    <div class=\"alert-info\">\r\n        ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
   Write(Model.B.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(" (");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                  Write(Model.B.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                                Write(Model.B.Semester.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(" зачислено студентов ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                                                                           Write(Model.BCount);

#line default
#line hidden
#nullable disable
            WriteLiteral(")\r\n    </div>\r\n</div>\r\n\r\n<br />\r\n<div>\r\n    Общие секции\r\n    <table class=\"table\">\r\n        <thead>\r\n            <tr>\r\n                <th>\r\n                    Секция\r\n                </th>\r\n                <th>\r\n                    ");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(Model.A.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(" (");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                              Write(Model.A.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                                            Write(Model.A.Semester.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(")\r\n                </th>\r\n                <th>\r\n                    ");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(Model.B.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(" (");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                              Write(Model.B.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                                            Write(Model.B.Semester.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(")\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
         foreach (var s in Model.CommonSections)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(s.Module.title);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 43 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(Model.AAdmissions[s]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 46 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(Model.BAdmissions[s]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("        \r\n\r\n");
#nullable restore
#line 52 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
         foreach (var s in Model.ExceptSections)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(s.Module.title);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 59 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(Model.AAdmissions[s]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td class=\"alert-warning\">\r\n                    Нет в конкурсной группе\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("        \r\n\r\n");
#nullable restore
#line 68 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
         foreach (var s in Model.NewSections)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 72 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(s.Module.title);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td class=\"alert-warning\">\r\n                    Нет в конкурсной группе\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(Model.BAdmissions[s]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 81 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </table>\r\n</div>\r\n<br />\r\n<div>\r\n    Общие группы\r\n\r\n    <table class=\"table\">\r\n");
#nullable restore
#line 89 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
         foreach (var s in Model.CommonGroups)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 93 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
               Write(s.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 96 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </table>\r\n    \r\n");
#nullable restore
#line 99 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
     if (!Model.CommonGroups.Any())
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"alert-warning\">\r\n            Нет общих групп! Копирование бессмысленно.\r\n        </div>\r\n");
#nullable restore
#line 104 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\r\n\r\n");
#nullable restore
#line 107 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
 if (Model.ExceptGroups.Any())
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <br />\r\n    <div>\r\n        Группы, которые есть только в источнике зачислений\r\n\r\n        <table class=\"table\">\r\n");
#nullable restore
#line 114 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
             foreach (var s in Model.ExceptGroups)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 118 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                   Write(s.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n");
#nullable restore
#line 121 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"

            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </table>\r\n\r\n\r\n        <div class=\"alert-warning\">\r\n            Не все учебные группы есть в целевой конкурсной группе\r\n        </div>\r\n    </div>\r\n    <br />\r\n");
#nullable restore
#line 131 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 134 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
 if (Model.NewGroups.Any())
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <br />\r\n    <div>\r\n        Группы, которые есть только в новой конкурсной группе\r\n\r\n        <table class=\"table\">\r\n");
#nullable restore
#line 141 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
             foreach (var s in Model.NewGroups)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 145 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
                   Write(s.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n");
#nullable restore
#line 148 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </table>\r\n\r\n\r\n        <div class=\"alert-info\">\r\n            В конкурсной группе есть новые группы, в которые не будут скопированы зачисления\r\n        </div>\r\n    </div>\r\n    <br />\r\n");
#nullable restore
#line 157 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 159 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
 using (Html.BeginForm("CopyMembership", "SectionFK", new { src = Model.A.Id, dst = Model.B.Id }, FormMethod.Post, null, new { id = "execform" }))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 161 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-group\">\r\n\r\n        <div class=\"col-md-offset-2 col-md-10\">\r\n            <input type=\"submit\" value=\"Выполнить копирование\" class=\"btn btn-default\" />\r\n        </div>\r\n    </div>\r\n");
#nullable restore
#line 169 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\CopyMembershipPrepare.cshtml"
}

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Controllers.SectionFKCompetitionGroupMembershipsCopyVm> Html { get; private set; }
    }
}
#pragma warning restore 1591
