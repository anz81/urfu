#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "19120071cfbfa873e44be7fcf34b76194c4197bf"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Modules_Edit), @"mvc.1.0.view", @"/Views/Modules/Edit.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"19120071cfbfa873e44be7fcf34b76194c4197bf", @"/Views/Modules/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Modules_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.Module>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
  
    ViewBag.Title = "Редактирование модуля";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n\r\n");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
 using (Html.BeginForm())
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        <hr />\r\n        ");
#nullable restore
#line 16 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
   Write(Html.ValidationSummary(true, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        ");
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
   Write(Html.HiddenFor(model => model.uuid));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 20 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.title, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 22 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.title, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 23 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.title, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.shortTitle, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 30 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.shortTitle, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.shortTitle, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.coordinator, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 38 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.coordinator, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 39 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.coordinator, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 44 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.type, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 46 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.type, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 47 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.type, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 52 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.competence, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 54 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.competence, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 55 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.competence, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 60 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.testUnits, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 62 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.testUnits, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 63 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.testUnits, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 68 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.priority, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 70 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.priority, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 71 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.priority, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 76 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.state, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.state, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 79 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.state, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 84 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.approvedDate, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 86 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.approvedDate, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 87 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.approvedDate, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 92 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.comment, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 94 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.comment, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 95 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.comment, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 100 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.file, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 102 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.file, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 103 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.file, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 108 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
       Write(Html.LabelFor(model => model.specialities, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 110 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.EditorFor(model => model.specialities, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 111 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.specialities, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
            </div>
        </div>

        <div class=""form-group"">
            <div class=""col-md-offset-2 col-md-10"">
                <input type=""submit"" value=""Сохранить"" class=""btn btn-default"" />
            </div>
        </div>
    </div>
");
#nullable restore
#line 121 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div>\r\n    ");
#nullable restore
#line 124 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
Write(Html.ActionLink("Вернуться к списку", "Index"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
#nullable restore
#line 128 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Modules\Edit.cshtml"
Write(Scripts.Render("~/bundles/jqueryval"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.Module> Html { get; private set; }
    }
}
#pragma warning restore 1591