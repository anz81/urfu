#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "074036d1c3355cccd08e11ec4852d6f5c8dfb0a9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_ForgotPassword), @"mvc.1.0.view", @"/Views/Account/ForgotPassword.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"074036d1c3355cccd08e11ec4852d6f5c8dfb0a9", @"/Views/Account/ForgotPassword.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_ForgotPassword : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Models.ForgotPasswordViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
  
    ViewBag.Title = "Forgot your password?";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 6 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral(".</h2>\r\n\r\n");
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
 using (Html.BeginForm("ForgotPassword", "Account", FormMethod.Post, new { @class = "form-horizontal", role = "form" }))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <h4>Enter your email.</h4>\r\n    <hr />\r\n");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
Write(Html.ValidationSummary("", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-group\">\r\n        ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
   Write(Html.LabelFor(m => m.Email, new { @class = "col-md-2 control-label" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        <div class=\"col-md-10\">\r\n            ");
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
       Write(Html.TextBoxFor(m => m.Email, new { @class = "form-control" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div class=\"form-group\">\r\n        <div class=\"col-md-offset-2 col-md-10\">\r\n            <input type=\"submit\" class=\"btn btn-default\" value=\"Email Link\" />\r\n        </div>\r\n    </div>\r\n");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\ForgotPassword.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Models.ForgotPasswordViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
