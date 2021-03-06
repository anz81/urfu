#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4d62e01acb52cfb75a4e0fd6ce56260148509d85"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account__ExternalLoginsListPartial), @"mvc.1.0.view", @"/Views/Account/_ExternalLoginsListPartial.cshtml")]
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
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
using Urfu.Its.Web.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
using Urfu.Its.Web.Controllers;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4d62e01acb52cfb75a4e0fd6ce56260148509d85", @"/Views/Account/_ExternalLoginsListPartial.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Account__ExternalLoginsListPartial : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ExternalLoginListViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<h4>Use another service to log in.</h4>\r\n<hr />\r\n");
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
  
    var ac = new AccountController();
    var loginProviders = (await ac.SignInM.GetExternalAuthenticationSchemesAsync()).ToList();
    if (loginProviders.Count() == 0)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <div>
            <p>
                There are no external authentication services configured. See <a href=""http://go.microsoft.com/fwlink/?LinkId=313242"">this article</a>
                for details on setting up this ASP.NET application to support logging in via external services.
            </p>
        </div>
");
#nullable restore
#line 19 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
    }
    else
    {
        using (Html.BeginForm(Model.Action, "Account", new { ReturnUrl = Model.ReturnUrl }))
        {
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 24 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
       Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div id=\"socialLoginList\">\r\n                <p>\r\n");
#nullable restore
#line 27 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
                     foreach (var p in loginProviders)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <button type=\"submit\" class=\"btn btn-default\"");
            BeginWriteAttribute("id", " id=\"", 1075, "\"", 1094, 1);
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
WriteAttributeValue("", 1080, p.HandlerType, 1080, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" name=\"provider\"");
            BeginWriteAttribute("value", " value=\"", 1111, "\"", 1133, 1);
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
WriteAttributeValue("", 1119, p.HandlerType, 1119, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            BeginWriteAttribute("title", " title=\"", 1134, "\"", 1182, 6);
            WriteAttributeValue("", 1142, "Log", 1142, 3, true);
            WriteAttributeValue(" ", 1145, "in", 1146, 3, true);
            WriteAttributeValue(" ", 1148, "using", 1149, 6, true);
            WriteAttributeValue(" ", 1154, "your", 1155, 5, true);
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
WriteAttributeValue(" ", 1159, p.DisplayName, 1160, 14, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue(" ", 1174, "account", 1175, 8, true);
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
                                                                                                                                                                             Write(p.HandlerType);

#line default
#line hidden
#nullable disable
            WriteLiteral("</button>\r\n");
#nullable restore
#line 30 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </p>\r\n            </div>\r\n");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\_ExternalLoginsListPartial.cshtml"
        }
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ExternalLoginListViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
