#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "21d851255d3d6833b2d52c4009623141a232cd09"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_UserDirections), @"mvc.1.0.view", @"/Views/Account/UserDirections.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"21d851255d3d6833b2d52c4009623141a232cd09", @"/Views/Account/UserDirections.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_UserDirections : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Models.EditUserDirectionsViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
  
    ViewBag.Title = "Направления для пользователя "+Model.UserFIO;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>Направления для пользователя ");
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
                            Write(Model.UserFIO);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n");
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
 using (Html.BeginForm("UserDirections", "Account", FormMethod.Post, new { encType = "multipart/form-data", name = "myform" }))
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-horizontal\">\r\n        ");
#nullable restore
#line 14 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
   Write(Html.ValidationSummary(true));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        <div class=\"form-group\">\r\n            <div class=\"col-md-10\">\r\n                ");
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
           Write(Html.HiddenFor(model => model.UserName));

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
                    Оксо
                </th>
                <th>
                    Название
                </th>
                <th>
                    Стандарт
                </th>
            </tr>

");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
             for (var i = 0; i < Model.Rows.Count; i++)
            {
                

#line default
#line hidden
#nullable disable
#nullable restore
#line 42 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
           Write(Html.HiddenFor(model => model.Rows[i].directionId));

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td style=\"text-align: center\">\r\n                        ");
#nullable restore
#line 45 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
                   Write(Html.CheckBoxFor(model => model.Rows[i].Checked));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 48 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
                   Write(Html.DisplayFor(model => model.Rows[i].Okso));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
                   Write(Html.DisplayFor(model => model.Rows[i].Title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 54 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
                   Write(Html.DisplayFor(model => model.Rows[i].Standard));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"

        </table>
        <br />
        <hr />

        <div class=""form-group"">
            <div class=""col-md-offset-2 col-md-10"">
                <input type=""submit"" value=""Сохранить"" class=""btn btn-default"" />
            </div>
        </div>
    </div>
");
#nullable restore
#line 70 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\UserDirections.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Models.EditUserDirectionsViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
