#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2cc357276c5bf24235b9c7f6165d785a2412be26"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Sync_Index), @"mvc.1.0.view", @"/Views/Sync/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2cc357276c5bf24235b9c7f6165d785a2412be26", @"/Views/Sync/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Sync_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Models.SyncModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
  
    ViewBag.Title = "Синхронизация данных";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<h2>Синхронизация данных</h2>\n\n<li>");
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация направлений", "Directions", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация модулей", "Modules", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация персонала (групп, студентов и персон)", "People", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n");
            WriteLiteral("<li>\r\n    Синхронизация рейтинга\r\n");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
     using (Html.BeginForm("Rating", "Sync", FormMethod.Post, new { id = "rating form" }))
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div>\r\n            Год\r\n        </div>\r\n        <div>\r\n            ");
#nullable restore
#line 22 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
       Write(Html.TextBox("Year", 2014));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </div>\r\n        <div>\r\n            Курс\r\n        </div>\r\n        <div>\r\n            ");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
       Write(Html.TextBox("Class", 2));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </div>\r\n        <div>\r\n            Семестр\r\n        </div>\r\n        <div>\r\n            ");
#nullable restore
#line 34 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
       Write(Html.TextBox("Term", 1));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </div>\r\n        <div>\r\n            <div class=\"checkbox\">\r\n                <label>");
#nullable restore
#line 38 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
                  Write(Html.CheckBox("WithCoefficients", true));

#line default
#line hidden
#nullable disable
            WriteLiteral("&nbsp;Учет уровней</label>\r\n            </div>\r\n        </div>\r\n        <div>\r\n            <input type=\"submit\" value=\"Запуск\" class=\"btn btn-default\" />\r\n        </div>\r\n");
#nullable restore
#line 44 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 46 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация средного балла", "RatingAvg", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 47 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация учебных планов студентов", "StudentPlan", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 48 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация всех учебных планов студентов", "SyncStudentPlans", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация выбора студента", "Selection", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 50 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация преподавателей", "SyncTeachers", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация РОПов", "SyncROPs", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 52 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация рейтинга абитуриентов", "SyncEntrants", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 53 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация рейтинга ИЯ", "SyncForeignLanguageRating", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 54 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация соглашений", "SyncModuleAgreements", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 55 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация траекторий", "SyncTrajectories", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Синхронизация контрольных мероприятий", "SyncTmers", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n<li>");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Создание ОП (образовательных программ) текущего года", "CreatePrograms", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\nСоздание версий ОП (образовательных программ)\n");
#nullable restore
#line 59 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
 using (Html.BeginForm("CreatePrograms", "Sync", FormMethod.Get, new { id = "CreatePrograms form" }))
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div>\n        Год\n    </div>\n    <div>\n        ");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.TextBox("year", DateTime.Now.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    </div>\n    <div>\n        <input type=\"submit\" value=\"Запуск\" class=\"btn btn-default\" />\n    </div>\n");
#nullable restore
#line 70 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("<br>\n\n\nСинхронизация нагрузок\n");
#nullable restore
#line 75 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
 using (Html.BeginForm("SyncApploads", "Sync", FormMethod.Get, new { id = "apploads form" }))
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div>\n        Год\n    </div>\n    <div>\n        ");
#nullable restore
#line 81 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.TextBox("Year", 2015));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    </div>\n    <div>\n        Семестр\n    </div>\n    <div>\n        ");
#nullable restore
#line 87 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.TextBox("Term", 1));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    </div>\n    <div>\n        <input type=\"submit\" value=\"Запуск\" class=\"btn btn-default\" />\n    </div>\n");
#nullable restore
#line 92 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("<br>\nСинхронизация должников по ФК\n");
#nullable restore
#line 95 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
 using (Html.BeginForm("SyncDebtors", "Sync", FormMethod.Get, new { id = "debtors form" }))
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div>\n        Модуль\n    </div>\r\n    <div>\n        ");
#nullable restore
#line 101 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.DropDownList("moduleTitle", new List<SelectListItem>()
        {
            new SelectListItem() {Text = "Физическая культура и спорт",Value = "Физическая культура и спорт"},
        }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    </div>\r\n    <div>\n        Год\n    </div>\r\n    <div>\n        ");
#nullable restore
#line 110 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.TextBox("Year", ""));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    </div>\r\n    <div>\n        Семестр\n    </div>\r\n    <div>\n        ");
#nullable restore
#line 116 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.DropDownList("moduleTitle", new List<SelectListItem>()
        {
            new SelectListItem() {Text = "Все",Value = ""},
            new SelectListItem() {Text = "Прочий",Value = "Прочий"},
            new SelectListItem() {Text = "Осенний",Value = "Осенний"},
            new SelectListItem() {Text = "Весенний",Value = "Весенний"},
        }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    </div>\r\n    <div>\n        <input type=\"submit\" value=\"Запуск\" class=\"btn btn-default\" />\n    </div>\r\n");
#nullable restore
#line 127 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("<br>\n<li>");
#nullable restore
#line 129 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.ActionLink("Отправка всех зачислений в РУНП", "SendAdmissionsToRunp", "Sync"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\nСинхронизация исторических групп\r\n");
#nullable restore
#line 131 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
 using (Html.BeginForm("GroupHistories", "Sync", FormMethod.Post, new { id = "grouphistories form" }))
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div>\r\n        Год\r\n    </div>\r\n    <div>\r\n        ");
#nullable restore
#line 137 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.TextBox("Year", DateTime.Now.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n    <div>\r\n        <input type=\"submit\" value=\"Запуск\" class=\"btn btn-default\" />\r\n    </div>\r\n");
#nullable restore
#line 142 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
#nullable restore
#line 147 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
Write(Html.DisplayTextFor(m=>m.Message));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
#nullable restore
#line 148 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
 if (Model.ModuleSynInProgress)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <p>Синхронизация модулей запущена ");
#nullable restore
#line 150 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
                                 Write(Model.ModulesSyncStarted);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n");
#nullable restore
#line 151 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("<div>\n    Синхронизация подразделения\n</div>\n");
#nullable restore
#line 155 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
 using (Html.BeginForm("SyncDivision", "Sync", FormMethod.Get, new { id = "SyncDivision form" }))
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div>\n        uuid\n    </div>\n    <div>\n        ");
#nullable restore
#line 161 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
   Write(Html.TextBox("id", "undifa18ggl5g0000jn134fnnnm25pgk"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    </div>\n    <div>\n        <input type=\"submit\" value=\"Запуск\" class=\"btn btn-default\" />\n    </div>\n");
#nullable restore
#line 166 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Sync\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\n\n\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script>
        $(document).ready(function() {
            $(""#testSelectionController"").click(function() {
                $.ajax({
                    url: ""/Sync/GenerateTestSelections"",
                    type: ""POST"",
                    success: function () { alert(""success""); },
                    failure: function (errMsg) {
                        alert(errMsg);
                    },
                });
            });
        });

        $(document).ready(function () {
            $(""#testPostSelectionController"").click(function () {
                $.ajax({
                    url: ""/api/StudentSelectionChanged"",
                    type: ""POST"",
                    data: JSON.stringify([{ ""studentPersonId"": ""studen18ggl5g0000kgm63e6gr2n5uog"", ""selectedVariantId"": 7, ""priorities"": [{ ""variantContentId"": 237, ""proprity"": 1 }] }, { ""studentPersonId"": ""studen18ggl5g0000k1r485obsanip04"", ""selectedVariantId"": 3, ""priorities"": [{ ""variantContentId"": 236, ""proprity"": 1 }, { ""variantContentId"": 2");
                WriteLiteral(@"37, ""proprity"": 2 }] }, { ""studentPersonId"": ""studen18ggl5g0000k1jdoen9kmhc5vo"", ""selectedVariantId"": 2, ""priorities"": [{ ""variantContentId"": 234, ""proprity"": 2 }, { ""variantContentId"": 235, ""proprity"": 4 }, { ""variantContentId"": 236, ""proprity"": 3 }, { ""variantContentId"": 237, ""proprity"": 3 }] }]),
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function (data) { alert(data); },
                    failure: function (errMsg) {
                        alert(errMsg);
                    },
                    beforeSend: function (xhr) { xhr.setRequestHeader(""Authorization"", ""Basic "" + btoa(""lks"" + "":"" + ""lks123"")); }
                });
            });
        });
    </script>
");
            }
            );
            WriteLiteral("\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Models.SyncModel> Html { get; private set; }
    }
}
#pragma warning restore 1591