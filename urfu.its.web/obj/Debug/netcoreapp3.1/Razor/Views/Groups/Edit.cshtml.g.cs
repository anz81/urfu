#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "034591f79cedb7ffb71ce93caeabae5db3587050"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Groups_Edit), @"mvc.1.0.view", @"/Views/Groups/Edit.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"034591f79cedb7ffb71ce93caeabae5db3587050", @"/Views/Groups/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Groups_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.Group>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
  
    ViewBag.Title = "Edit";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>Edit</h2>\r\n\r\n\r\n");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
 using (Html.BeginForm())
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        <h4>Group</h4>\r\n        <hr />\r\n        ");
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
   Write(Html.ValidationSummary(true));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        ");
#nullable restore
#line 18 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
   Write(Html.HiddenFor(model => model.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
       Write(Html.LabelFor(model => model.Name, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 23 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.EditorFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 24 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
       Write(Html.LabelFor(model => model.ProfileId, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.EditorFor(model => model.ProfileId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.ProfileId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 37 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
       Write(Html.LabelFor(model => model.Year, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 39 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.EditorFor(model => model.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.Year));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
       Write(Html.LabelFor(model => model.ChairId, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 47 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.EditorFor(model => model.ChairId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 48 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.ChairId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 53 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
       Write(Html.LabelFor(model => model.FormativeDivisionId, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 55 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.EditorFor(model => model.FormativeDivisionId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.FormativeDivisionId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 61 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
       Write(Html.LabelFor(model => model.FormativeDivisionParentId, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 63 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.EditorFor(model => model.FormativeDivisionParentId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 64 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.FormativeDivisionParentId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 69 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
       Write(Html.LabelFor(model => model.ManagingDivisionId, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 71 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.EditorFor(model => model.ManagingDivisionId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 72 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.ManagingDivisionId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 77 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
       Write(Html.LabelFor(model => model.ManagingDivisionParentId, new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 79 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.EditorFor(model => model.ManagingDivisionParentId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 80 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
           Write(Html.ValidationMessageFor(model => model.ManagingDivisionParentId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            <div class=\"col-md-offset-2 col-md-10\">\r\n                <input type=\"submit\" value=\"Save\" class=\"btn btn-default\" />\r\n            </div>\r\n        </div>\r\n    </div>\r\n");
#nullable restore
#line 90 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div>\r\n    ");
#nullable restore
#line 93 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
Write(Html.ActionLink("Back to List", "Index"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
#nullable restore
#line 97 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Groups\Edit.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.Group> Html { get; private set; }
    }
}
#pragma warning restore 1591
