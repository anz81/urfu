#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b1674ddb509a3ffa9e8ce3fd9156c7ad6e1c13d6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Project_Disciplines), @"mvc.1.0.view", @"/Views/Project/Disciplines.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b1674ddb509a3ffa9e8ce3fd9156c7ad6e1c13d6", @"/Views/Project/Disciplines.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Project_Disciplines : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<Urfu.Its.Web.Models.ProjectDisciplineViewModel>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n<table class=\"table\">\r\n    <tr>\r\n        <th>\r\n            ");
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
       Write(Html.DisplayNameFor(model => model.Discipline.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n");
            WriteLiteral("        <th>\r\n            ");
#nullable restore
#line 14 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
       Write(Html.DisplayNameFor(model => model.Discipline.testUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th></th>\r\n    </tr>\r\n");
#nullable restore
#line 18 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
     foreach (var item in Model)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <tr>\r\n            <td>\r\n                ");
#nullable restore
#line 22 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
           Write(Html.DisplayFor(modelItem => item.Discipline.title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n");
            WriteLiteral("            <td>\r\n                ");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
           Write(Html.DisplayFor(modelItem => item.Discipline.testUnits));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
                 if (item.ProjectDiscipline == null )
                {
                    if (ViewBag.CanEdit)
                    {
                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
                   Write(Html.ActionLink("Назначить аудиторную нагрузку, контрольные мероприятия и формы контроля", "EditTmers", new { projectId = ViewBag.ProjectId, disciplineId = item.Discipline.uid }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
                                                                                                                                                                                                           
                    }
                }
                else
                {
                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
               Write(Html.ActionLink("Редактировать аудиторную нагрузку, контрольные мероприятия и формы контроля", "Tmers", new { id = item.ProjectDiscipline.Id }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
                                                                                                                                                                    
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("               \r\n            </td>\r\n        </tr>\r\n");
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</table>\r\n\r\n<div>\r\n    ");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Project\Disciplines.cshtml"
Write(Html.ActionLink("Вернуться к списку", "Index"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<Urfu.Its.Web.Models.ProjectDisciplineViewModel>> Html { get; private set; }
    }
}
#pragma warning restore 1591
