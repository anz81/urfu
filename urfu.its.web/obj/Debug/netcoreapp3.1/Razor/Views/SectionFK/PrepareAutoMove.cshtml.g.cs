#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "482d008d11853747a2dd53ac1743a676aabcd370"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_SectionFK_PrepareAutoMove), @"mvc.1.0.view", @"/Views/SectionFK/PrepareAutoMove.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"482d008d11853747a2dd53ac1743a676aabcd370", @"/Views/SectionFK/PrepareAutoMove.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_SectionFK_PrepareAutoMove : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Controllers.AutoMoveData>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
  
    ViewBag.Title = "Автоматический перевод студентов";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 6 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n");
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
 foreach (var cg in Model.CompetitionGroups)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <h3>        \r\n        ");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
   Write(cg.Description);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </h3>\r\n");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
    foreach (var s in cg.Sections.Where(s=>s.Display))
     {

#line default
#line hidden
#nullable disable
            WriteLiteral("         <h4>Секция: ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                Write(s.Prop.SectionFk.Module.shortTitle);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</h4>
         <table  class=""table table-condensed"">
             <thead><tr>
                 <td></td>
                 <td>Фамилия</td>
                 <td>Имя</td>
                 <td>Отчество</td>
                 <td>Группа</td>
                 <td></td>
             </tr>
             </thead>
");
#nullable restore
#line 26 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
              foreach (var student in s.Removed)
             {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td>Отчиcлить</td>\r\n                    <td>");
#nullable restore
#line 30 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                   Write(student.Person.Surname);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td>");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                   Write(student.Person.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td>");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                   Write(student.Person.PatronymicName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td>");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                   Write(student.Group.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td>в ");
#nullable restore
#line 34 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                     Write(cg.LookupTo(student));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                </tr>\r\n");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
             }

#line default
#line hidden
#nullable disable
#nullable restore
#line 37 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
              foreach (var student in s.Appended)
             {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td>Зачислить</td>\r\n                    <td>");
#nullable restore
#line 41 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                   Write(student.Person.Surname);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td>");
#nullable restore
#line 42 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                   Write(student.Person.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td>");
#nullable restore
#line 43 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                   Write(student.Person.PatronymicName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td>");
#nullable restore
#line 44 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                   Write(student.Group.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    <td>из ");
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                      Write(cg.LookupFrom(student));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                </tr>\r\n");
#nullable restore
#line 47 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
             }

#line default
#line hidden
#nullable disable
            WriteLiteral("         </table>\r\n");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
     }

#line default
#line hidden
#nullable disable
            WriteLiteral("    <h4>\r\n        Не переведённые\r\n    </h4>\r\n");
            WriteLiteral(@"    <table class=""table table-condensed"">
        <thead>
            <tr>
                <td>Фамилия</td>
                <td>Имя</td>
                <td>Отчество</td>
                <td>Группа</td>
                <td>Секция</td>
                <td>Пожелание</td>
            </tr>
        </thead>
");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
         foreach (var s in cg.Ungranted)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>");
#nullable restore
#line 68 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
               Write(s.Student.Person.Surname);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 69 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
               Write(s.Student.Person.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 70 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
               Write(s.Student.Person.PatronymicName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 71 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
               Write(s.Student.Group.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>из ");
#nullable restore
#line 72 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
                  Write(cg.LookupFrom(s.Student));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 73 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
               Write(s.Section.Module.shortTitle);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            </tr>\r\n");
#nullable restore
#line 75 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </table>\r\n");
#nullable restore
#line 77 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div>\r\n    <hr />\r\n    ");
#nullable restore
#line 81 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
Write(Html.ActionLink("Посмотреть в Excel", "PrepareAutoMoveDownload", new { year = (int)ViewBag.year, semester = (int)ViewBag.semester,course = (int?)ViewBag.course, modules = (string)ViewBag.modules }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n\r\n<div>\r\n    <hr />\r\n");
#nullable restore
#line 86 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
     using (Html.BeginForm("ExecuteAutoMove", "SectionFK", new { year = (int)ViewBag.year, semester = (int)ViewBag.semester,course = (int?)ViewBag.course, modules = (string)ViewBag.modules }, FormMethod.Post, new { id = "execform" }))
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 88 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
   Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"form-group\">\r\n\r\n            <div class=\"col-md-offset-2 col-md-10\">\r\n                <input type=\"submit\" value=\"Выполнить переводы\" class=\"btn btn-default\" />\r\n            </div>\r\n        </div>\r\n");
#nullable restore
#line 96 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFK\PrepareAutoMove.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>

<script>
    $(""#execform"").submit(function (event) {


        if (confirm(""Сейчас будет выполнен автоматический перевод.""))
            return true;
        else {
            event.preventDefault();
            return false;
        }
    });
</script>
");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Controllers.AutoMoveData> Html { get; private set; }
    }
}
#pragma warning restore 1591
