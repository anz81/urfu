#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fc79e076c45f4d4dc5166e0d8e9d3c00b637004f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_RoleSets_Index), @"mvc.1.0.view", @"/Views/RoleSets/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fc79e076c45f4d4dc5166e0d8e9d3c00b637004f", @"/Views/RoleSets/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_RoleSets_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<Urfu.Its.Web.DataContext.RoleSet>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
  
    ViewBag.Title = "Index";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>Index</h2>\r\n\r\n<p>\r\n    ");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
Write(Html.ActionLink("Create New", "Create"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</p>\r\n<table class=\"table\">\r\n    <tr>\r\n        <th>\r\n            ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
       Write(Html.DisplayNameFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </th>\r\n        <th></th>\r\n    </tr>\r\n\r\n");
#nullable restore
#line 20 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
 foreach (var item in Model) {

#line default
#line hidden
#nullable disable
            WriteLiteral("    <tr>\r\n        <td>\r\n            ");
#nullable restore
#line 23 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
       Write(Html.DisplayFor(modelItem => item.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n        <td>\r\n            ");
#nullable restore
#line 26 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
       Write(Html.ActionLink("Edit", "Edit", new { id=item.Id }));

#line default
#line hidden
#nullable disable
            WriteLiteral(" |\r\n            ");
#nullable restore
#line 27 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
       Write(Html.ActionLink("Details", "Details", new { id=item.Id }));

#line default
#line hidden
#nullable disable
            WriteLiteral(" |\r\n            ");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
       Write(Html.ActionLink("Delete", "Delete", new { id=item.Id }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\RoleSets\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</table>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<Urfu.Its.Web.DataContext.RoleSet>> Html { get; private set; }
    }
}
#pragma warning restore 1591
