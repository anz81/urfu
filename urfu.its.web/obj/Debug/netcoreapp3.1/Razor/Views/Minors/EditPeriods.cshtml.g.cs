#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4f1e82eb29c8b205186b35f76ce6883c5a3f67b3"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Minors_EditPeriods), @"mvc.1.0.view", @"/Views/Minors/EditPeriods.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4f1e82eb29c8b205186b35f76ce6883c5a3f67b3", @"/Views/Minors/EditPeriods.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Minors_EditPeriods : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Models.MinorTmersPeriodViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
  
    ViewBag.Title = "EditPeriod";

#line default
#line hidden
#nullable disable
            WriteLiteral("<h3>Майнор: ");
#nullable restore
#line 6 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
       Write(Model.Minor.Module.title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h3>\r\n<h3>Дисциплина: ");
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
           Write(Model.Discipline.Discipline.title);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </h3>\r\n\r\n<h2>Редактирование периодов</h2>\r\n\r\n");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
 using (Html.BeginForm())
{


#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
Write(Html.ValidationSummary(true, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
Write(Html.HiddenFor(model => model.Minor.ModuleId));

#line default
#line hidden
#nullable disable
#nullable restore
#line 19 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
Write(Html.HiddenFor(model => model.Discipline.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"<table class=""table table-striped table-hover table-bordered table-condensed"">
    <tr>
        <th width=""200px"">
            Нагрузка
        </th>
        <th width=""50px"">
            
        </th>
        <th  width=""150px"">
            Год
        </th>
        <th  width=""auto"">
            Семестр
        </th>
");
            WriteLiteral("    </tr>\r\n\r\n");
#nullable restore
#line 38 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
   for (var i = 0; i < Model.Rows.Count; i++)
  {

#line default
#line hidden
#nullable disable
            WriteLiteral("    <tr>\r\n        ");
#nullable restore
#line 41 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
   Write(Html.HiddenFor(model => model.Rows[i].Tmer.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        ");
#nullable restore
#line 42 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
   Write(Html.HiddenFor(model => model.Rows[i].Period.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 43 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
             if (i == 0 || Model.Rows[i].Tmer.TmerId != Model.Rows[i - 1].Tmer.TmerId)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <td");
            BeginWriteAttribute("rowspan", " rowspan=\"", 1136, "\"", 1169, 1);
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
WriteAttributeValue("", 1146, Model.GetPeriodCount(), 1146, 23, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n                    ");
#nullable restore
#line 46 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
               Write(Html.DisplayFor(model => model.Rows[i].Tmer.Tmer.rmer));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n");
#nullable restore
#line 48 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        <td>\r\n            ");
#nullable restore
#line 50 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
       Write(Html.CheckBoxFor(model => model.Rows[i].Checked));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n        <td>\r\n            ");
#nullable restore
#line 53 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
       Write(Html.DisplayFor(model => model.Rows[i].Period.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n\r\n        <td>\r\n            ");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
       Write(Html.DisplayFor(model => model.Rows[i].Period.Semester.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n");
            WriteLiteral("    </tr>\r\n");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
  }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n  </table>\r\n");
            WriteLiteral("  <div class=\"form-group\">\r\n    <div class=\"col-md-offset-2 col-md-10\">\r\n        <input type=\"submit\" value=\"Сохранить\" class=\"btn btn-default\" />\r\n    </div>\r\n  </div>\r\n");
#nullable restore
#line 75 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div>\r\n    ");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Minors\EditPeriods.cshtml"
Write(Html.ActionLink("Вернуться к списку", "Index"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Models.MinorTmersPeriodViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
