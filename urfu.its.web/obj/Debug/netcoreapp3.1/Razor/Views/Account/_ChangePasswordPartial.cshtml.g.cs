#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "efade34c3a6209c7be60123c57c057a8b92ad14c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account__ChangePasswordPartial), @"mvc.1.0.view", @"/Views/Account/_ChangePasswordPartial.cshtml")]
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
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"efade34c3a6209c7be60123c57c057a8b92ad14c", @"/Views/Account/_ChangePasswordPartial.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Account__ChangePasswordPartial : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Models.ManageUserViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<p>You\'re logged in as <strong>");
#nullable restore
#line 4 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
                          Write(User.Identity.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>.</p>\r\n\r\n");
#nullable restore
#line 6 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
 using (Html.BeginForm("Manage", "Account", FormMethod.Post, new { @class = "form-horizontal", role = "form" }))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <h4>Change Password Form</h4>\r\n    <hr />\r\n");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
Write(Html.ValidationSummary("", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-group\">\r\n        ");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
   Write(Html.LabelFor(m => m.OldPassword, new { @class = "col-md-2 control-label" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        <div class=\"col-md-10\">\r\n            ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
       Write(Html.PasswordFor(m => m.OldPassword, new { @class = "form-control" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div class=\"form-group\">\r\n        ");
#nullable restore
#line 19 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
   Write(Html.LabelFor(m => m.NewPassword, new { @class = "col-md-2 control-label" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        <div class=\"col-md-10\">\r\n            ");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
       Write(Html.PasswordFor(m => m.NewPassword, new { @class = "form-control" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div class=\"form-group\">\r\n        ");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
   Write(Html.LabelFor(m => m.ConfirmPassword, new { @class = "col-md-2 control-label" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        <div class=\"col-md-10\">\r\n            ");
#nullable restore
#line 27 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
       Write(Html.PasswordFor(m => m.ConfirmPassword, new { @class = "form-control" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n");
            WriteLiteral("    <div class=\"form-group\">\r\n        <div class=\"col-md-offset-2 col-md-10\">\r\n            <input type=\"submit\" value=\"Change password\" class=\"btn btn-default\" />\r\n        </div>\r\n    </div>\r\n");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ChangePasswordPartial.cshtml"
}

#line default
#line hidden
#nullable disable
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Models.ManageUserViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
