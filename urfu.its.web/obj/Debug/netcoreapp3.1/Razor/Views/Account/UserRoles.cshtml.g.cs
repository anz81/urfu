#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6fa685705c6bfc1cf867dc90021f5686c3810ddb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_UserRoles), @"mvc.1.0.view", @"/Views/Account/UserRoles.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6fa685705c6bfc1cf867dc90021f5686c3810ddb", @"/Views/Account/UserRoles.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_UserRoles : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Models.SelectUserRolesViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
  
    ViewBag.Title = "Роли пользователя";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n<h2>Роли пользователя ");
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
                 Write(Html.DisplayFor(model => model.UserName));

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n<hr />\r\n<p id=\"roleSets\"></p>\r\n<p id=\"appendRoleSets\"></p>\r\n\r\n");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
 using (Html.BeginForm("UserRoles", "Account", FormMethod.Post, new { encType = "multipart/form-data", name = "myform" }))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        ");
#nullable restore
#line 18 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
   Write(Html.ValidationSummary(true));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        <div class=\"form-group\">\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
           Write(Html.HiddenFor(model => model.UserName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 26 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
           Write(Html.HiddenFor(model => model.FirstName));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
            </div>
        </div>

        <br />
        <hr />

        <table class=""table table-striped table-hover table-bordered table-condensed"">
            <tr>
                <th>

                </th>
                <th>
                    Роль
                </th>
            </tr>
            ");
#nullable restore
#line 42 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
       Write(Html.EditorFor(model => model.Roles));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
        </table>
        <br />
        <hr/>

            <div class=""form-group"">
                <div class=""col-md-offset-2 col-md-10"">
                    <input type=""submit"" value=""Сохранить"" class=""btn btn-default"" />
                </div>
            </div>
</div>
");
#nullable restore
#line 53 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n\r\n");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
 if (@Model != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script>\r\n        $(document).ready(function() {\r\n            $.ajax(\"/RoleSets/ForUser?id=");
#nullable restore
#line 61 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
                                    Write(Model.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral(@""").done(function (data) {
                if (data.length > 0) {
                    $(""#roleSets"").append(""<h4>Выбор одной роли</h4>"");
                    $(""#appendRoleSets"").append(""<h4>Множественный выбор ролей</h4>"");
                }
                for (var i = 0; i < data.length; i++) {
                    var rs = data[i];
                    if (rs.set) {
                        $(""#roleSets"").append(""<button class=\""btn-primary\"" style=\""margin:2px;\"" data-val=\"""" + rs.Id + ""\"">"" + rs.Name + ""</buttton>"");
                    } else {
                        $(""#roleSets"").append(""<button class=\""btn\"" style=\""margin:2px;\"" data-val=\"""" + rs.Id + ""\"">"" + rs.Name + ""</buttton>"");
                        $(""#appendRoleSets"").append(""<button class=\""btn\"" style=\""margin:2px;\"" data-val=\"""" + rs.Id + ""\"">"" + rs.Name + ""</buttton>"");
                    }
                }
                $(""#roleSets button"").click(function () {
                    var button = $(this);
              ");
            WriteLiteral("      var rs = button.data(\"val\");\r\n                    window.location = \"/Account/ApplyRoleSet/");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
                                                        Write(Model.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?set="" + rs;
                });

                $(""#appendRoleSets button"").click(function () {
                    var button = $(this);
                    var rs = button.data(""val"");
                    window.location = ""/Account/AppendRoleSet/");
#nullable restore
#line 84 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
                                                         Write(Model.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral("?set=\" + rs;\r\n                });\r\n            });\r\n        });\r\n    </script>\r\n");
#nullable restore
#line 89 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserRoles.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("    \r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Models.SelectUserRolesViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
