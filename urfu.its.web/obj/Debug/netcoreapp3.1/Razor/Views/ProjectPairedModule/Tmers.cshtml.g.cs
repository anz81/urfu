#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0ab49c398621c3a2828543ab9f87bfbb0021b19a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ProjectPairedModule_Tmers), @"mvc.1.0.view", @"/Views/ProjectPairedModule/Tmers.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0ab49c398621c3a2828543ab9f87bfbb0021b19a", @"/Views/ProjectPairedModule/Tmers.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ProjectPairedModule_Tmers : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.ProjectDiscipline>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
  
    ViewBag.Title = "Нагрузка";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h3>Проект: ");
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
       Write(Model.Project.Module.title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h3>\r\n<h3>Дисциплина: ");
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
           Write(Model.Discipline.title);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </h3>\r\n\r\n");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
 if (ViewBag.Message != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <h2 class=\"alert-danger\">");
#nullable restore
#line 12 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
                        Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
 using (Html.BeginForm())
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        <hr />\r\n        ");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
   Write(Html.ValidationSummary(true, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n        <h4>Aудиторная нагрузка</h4>\r\n        ");
#nullable restore
#line 24 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
   Write(Html.DisplayFor(m => ViewData["Tmers1"], "ProjectDisciplineTmer"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n        <h4>Контрольные мероприятия</h4>\r\n        ");
#nullable restore
#line 27 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
   Write(Html.DisplayFor(m => ViewData["Tmers2"], "ProjectDisciplineTmer"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n        <h4>Формы контроля</h4>\r\n        ");
#nullable restore
#line 30 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
   Write(Html.DisplayFor(m => ViewData["Tmers3"], "ProjectDisciplineTmer"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n    </div>\r\n");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
 if (ViewBag.CanEdit)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <p>\r\n        ");
#nullable restore
#line 38 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
   Write(Html.ActionLink("Редактировать нагрузку", "EditTmers", new { ProjectId = Model.ProjectId, disciplineId = Model.Discipline.uid }));

#line default
#line hidden
#nullable disable
            WriteLiteral(" |\r\n        ");
#nullable restore
#line 39 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
   Write(Html.ActionLink("Редактировать периоды", "EditPeriods", new { ProjectId = Model.ProjectId, disciplineId = Model.Discipline.uid }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </p>\r\n");
#nullable restore
#line 41 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("<hr />\r\n\r\n<div>\r\n    ");
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectPairedModule\Tmers.cshtml"
Write(Html.ActionLink("Вернуться к списку", "Disciplines", new { moduleId = Model.ProjectId }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.ProjectDiscipline> Html { get; private set; }
    }
}
#pragma warning restore 1591
