#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9af79407db0460a1330fcb508443df9d132829b1"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Pratice_GetCompanyInfo), @"mvc.1.0.view", @"/Views/Pratice/GetCompanyInfo.cshtml")]
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
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\_ViewImports.cshtml"
using Urfu.Its.Frames;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\_ViewImports.cshtml"
using Urfu.Its.Frames.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9af79407db0460a1330fcb508443df9d132829b1", @"/Views/Pratice/GetCompanyInfo.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a29a642edbfb9ad6e9236f96e04f5da3c1de2bef", @"/Views/_ViewImports.cshtml")]
    public class Views_Pratice_GetCompanyInfo : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Frames.Controllers.PersonalContractVM>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/Scripts/jquery.validate.unobtrusive.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9af79407db0460a1330fcb508443df9d132829b13538", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n<div id=\"companyinfo\">\r\n");
#nullable restore
#line 5 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
     using (Html.BeginForm("AddContratToCompany", "Practice", FormMethod.Post))
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
   Write(Html.HiddenFor(m => m.CompanyId));

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
   Write(Html.HiddenFor(m => m.PracticeID));

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
   Write(Html.HiddenFor(m => m.InstituteTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"form-group div.content\">\r\n            ");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.ShortName, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.ShortName, new { @class = "form-control col-sm-10", @readonly = "readonly" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 17 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.INN, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 19 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.INN, new { @class = "form-control col-sm-10", @readonly = "readonly" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 23 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.Director, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.Director, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 26 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.ValidationMessageFor(m => m.Director, null, new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 30 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.DirectorInitials, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.DirectorInitials, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.ValidationMessageFor(m => m.DirectorInitials, null, new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 37 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.DirectorGenetive, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 39 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.DirectorGenetive, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.ValidationMessageFor(m => m.DirectorGenetive, null, new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 44 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.PostOfDirector, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 46 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.PostOfDirector, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 50 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.PostOfDirectorGenetive, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 52 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.PostOfDirectorGenetive, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.PersonInCharge, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 58 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.PersonInCharge, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 59 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.ValidationMessageFor(m => m.PersonInCharge, null, new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 63 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.PersonInChargeInitials, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.PersonInChargeInitials, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 66 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.ValidationMessageFor(m => m.PersonInChargeInitials, null, new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 70 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.PostOfPersonInCharge, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 72 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.PostOfPersonInCharge, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 76 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.Phone, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.Phone, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 82 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.PersonInChargeEmail, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 84 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.PersonInChargeEmail, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 85 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.ValidationMessageFor(m => m.PersonInChargeEmail, null, new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 89 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.Location, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 91 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.Location, new { @class = "form-control col-sm-10", @readonly = "readonly" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 95 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.Address, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 97 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.Address, new { @class = "form-control col-sm-10", @readonly = "readonly" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 101 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.CompanyPhone, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 103 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.CompanyPhone, new { @class = "form-control col-sm-10", @readonly = "readonly" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 107 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.Email, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 109 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.Email, new { @class = "form-control col-sm-10", @readonly = "readonly" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 113 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.Site, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 115 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextBoxFor(m => m.Site, new { @class = "form-control col-sm-10", @readonly = "readonly" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div class=\"form-group\">\r\n            ");
#nullable restore
#line 119 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
       Write(Html.LabelFor(m => m.Comment, new { @class = "control-label col-sm-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <div class=\"col-sm-10\">\r\n                ");
#nullable restore
#line 121 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
           Write(Html.TextAreaFor(m => m.Comment, new { @class = "form-control col-sm-10" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n");
            WriteLiteral("        <div class=\"button-panel pull-right\">\r\n            <div class=\"col-sm-12\">\r\n                <button class=\"btn btn-default\" type=\"submit\" name=\"admissionCompany\" value=\"AddContract\" id=\"btnSubmit\"");
            BeginWriteAttribute("href", " href=\"", 6734, "\"", 6741, 0);
            EndWriteAttribute();
            WriteLiteral(">Отправить</button>\r\n                <button class=\"btn \" type=\"button\" id=\"cancelСhoiceOrganizationButton\">Отмена</button>\r\n            </div>\r\n        </div>\r\n");
#nullable restore
#line 131 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetCompanyInfo.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>

<script type=""text/javascript"">
    $(""#cancelСhoiceOrganizationButton"").click(function () {
        $(""#PersonalContract_CompanyName"").prop('readonly', false);
        $(""#PersonalContract_CompanyName"").val("""");
        $(""#companyinfo"").remove();
    });

</script>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Frames.Controllers.PersonalContractVM> Html { get; private set; }
    }
}
#pragma warning restore 1591
