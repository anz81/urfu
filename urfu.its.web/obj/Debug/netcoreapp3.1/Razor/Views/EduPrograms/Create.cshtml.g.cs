#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d7f09bf280225c9b0cc0b7037f962cf2c37ac368"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_EduPrograms_Create), @"mvc.1.0.view", @"/Views/EduPrograms/Create.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d7f09bf280225c9b0cc0b7037f962cf2c37ac368", @"/Views/EduPrograms/Create.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_EduPrograms_Create : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.EduProgram>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
  
    ViewBag.Title = "Создание образовательной программы";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n");
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
 using (Html.BeginForm()) 
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        <h4>Программа</h4>\r\n        <hr />\r\n        ");
#nullable restore
#line 16 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
   Write(Html.ValidationSummary(true));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 19 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
       Write(Html.LabelFor(model => model.Name, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.EditorFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 22 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.ValidationMessageFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 27 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
       Write(Html.LabelFor(model => model.directionId, "Направление", new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.DropDownList("directionId", String.Empty));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 30 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.ValidationMessageFor(model => model.directionId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
       Write(Html.LabelFor(model => model.Year, new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 37 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.EditorFor(model => model.Year, new {htmlAttributes = new {@Value = @DateTime.Now.Year.ToString()}}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 38 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.ValidationMessageFor(model => model.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 43 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
       Write(Html.LabelFor(model => model.qualification, "Квалификация", new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.DropDownList("qualification", String.Empty));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 46 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.ValidationMessageFor(model => model.qualification));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
       Write(Html.LabelFor(model => model.divisionId, "Подразделение", new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 53 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.DropDownList("divisionId", String.Empty));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 54 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.ValidationMessageFor(model => model.divisionId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 59 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
       Write(Html.LabelFor(model => model.profileId, "Профиль", new {@class = "control-label col-md-2"}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 61 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.DropDownList("profileId", String.Empty));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 62 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.ValidationMessageFor(model => model.profileId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 68 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
       Write(Html.LabelFor(model => model.familirizationType, "Форма освоения", new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 70 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.DropDownList("familirizationType", String.Empty));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 71 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.ValidationMessageFor(model => model.familirizationType));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        \r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 76 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
       Write(Html.LabelFor(model => model.familirizationCondition, "Условие освоения", new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.DropDownList("familirizationCondition", String.Empty));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 79 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
           Write(Html.ValidationMessageFor(model => model.familirizationCondition));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
            </div>
        </div>


        <div class=""form-group"">
            <div class=""col-md-offset-2 col-md-10"">
                <input type=""submit"" value=""Создать"" class=""btn btn-default"" />
            </div>
        </div>
    </div>
");
#nullable restore
#line 90 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div>\r\n    ");
#nullable restore
#line 93 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Create.cshtml"
Write(Html.ActionLink("К списку", "Index"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral(@"
    <script>
    $(document).ready(function () {
        $(""#directionId"").change(function () {
            var divisionQuery = {};
            divisionQuery.url = ""/api/divisionnames?okso=&directionId="" + $(""#directionId"").val();
            divisionQuery.type = ""GET"";
            divisionQuery.datatype = ""json"";
            divisionQuery.contentType = ""application/json"";
            divisionQuery.success = function (programsList) {
                $(""#divisionId"").empty();
                for (var i = 0; i < programsList.length; i++) {
                    $(""#divisionId"").append(""<option value="" + programsList[i].id + "">"" + programsList[i].divisionName + ""</option>"");
                }
            };
            divisionQuery.error = function () { alert(""Ошибка получения списка подразделений""); };
            $.ajax(divisionQuery);


            var profilesQuery = {};
            profilesQuery.url = ""/api/profilenames?okso=&directionId="" + $(""#directionId"").val();
            profiles");
                WriteLiteral(@"Query.type = ""GET"";
            profilesQuery.datatype = ""json"";
            profilesQuery.contentType = ""application/json"";
            profilesQuery.success = function (programsList) {
                $(""#profileId"").empty();
                for (var i = 0; i < programsList.length; i++) {
                    $(""#profileId"").append(""<option value="" + programsList[i].id + "">"" + programsList[i].name + ""</option>"");
                }
            };
            profilesQuery.error = function () { alert(""Ошибка получения списка подразделений""); };
            $.ajax(profilesQuery);
        });
    });
    </script>
");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.EduProgram> Html { get; private set; }
    }
}
#pragma warning restore 1591
