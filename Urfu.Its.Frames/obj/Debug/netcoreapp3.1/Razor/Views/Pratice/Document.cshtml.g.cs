#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "72c772dd36f783a6de4f84686076d769eb7a2dd7"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Pratice_Document), @"mvc.1.0.view", @"/Views/Pratice/Document.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"72c772dd36f783a6de4f84686076d769eb7a2dd7", @"/Views/Pratice/Document.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a29a642edbfb9ad6e9236f96e04f5da3c1de2bef", @"/Views/_ViewImports.cshtml")]
    public class Views_Pratice_Document : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Frames.Controllers.DocumentListVM>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/explanation.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("data-toggle", new global::Microsoft.AspNetCore.Html.HtmlString("tooltip"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
  
    Layout = null;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<!DOCTYPE html>\r\n\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "72c772dd36f783a6de4f84686076d769eb7a2dd74358", async() => {
                WriteLiteral(@"
    <meta name=""viewport"" content=""width=device-width"" />
    <title>Document</title>

    <link rel=""stylesheet""
          href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css""
          integrity=""sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u""
          crossorigin=""anonymous"">

    <style>
        input, select, textarea {
            max-width: initial;
        }

        .tooltip {
            opacity: 1 !important;
        }

        .tooltip-inner {
            background: #f2f2f2;
            color: #000000;
            box-shadow: 0 2px 7px 0 rgba(0,0,0,0.64);
        }

        .tooltip.top .tooltip-arrow {
            border-top-color: #f2f2f2;
        }
    </style>

    <script src=""https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js""></script>
");
                WriteLiteral("    <script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js\"></script>\r\n\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "72c772dd36f783a6de4f84686076d769eb7a2dd76340", async() => {
                WriteLiteral("    \r\n    <div class=\"container-fluid\">\r\n        <h4>");
#nullable restore
#line 47 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
       Write(Model.DisciplineTitle);

#line default
#line hidden
#nullable disable
                WriteLiteral("</h4>\r\n        <div class=\"row\">\r\n            <h5 class=\"col-sm-4\">");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                            Write(Model.YearInfo);

#line default
#line hidden
#nullable disable
                WriteLiteral("</h5>\r\n            <h5 class=\"col-sm-4\">");
#nullable restore
#line 50 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                            Write(Model.Semester);

#line default
#line hidden
#nullable disable
                WriteLiteral("</h5>\r\n            <h5 class=\"col-sm-4\">");
#nullable restore
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                            Write(Model.PeriodInfo());

#line default
#line hidden
#nullable disable
                WriteLiteral("</h5>\r\n        </div>\r\n\r\n        <div class=\"container row\">\r\n            ");
#nullable restore
#line 55 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
       Write(Html.ActionLink("Выбор предприятия, руководителя, темы", "Practice", null, new { ID = Model.PracticeID }, new { @class = "btn btn-info", role = "button" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
       Write(Html.ActionLink("Шаблоны документов", "Document", null, new { ID = Model.PracticeID }, new { @class = "btn btn-success", role = "button" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
       Write(Html.ActionLink("Сканы документов", "Scan", null, new { ID = Model.PracticeID }, new { @class = "btn btn-info", role = "button" }));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
        </div>

        <p>
        </p>
        <div class=""panel panel-default"">
            <div class=""panel-heading"">
                <div class=""row"">
                    <div class=""col-sm-12"">
                        До начала практики
                    </div>
                </div>
            </div>
            <div class=""panel-body"">
");
#nullable restore
#line 71 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                 foreach (var d in Model.BeforeDocuments)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <p>\r\n                        <div class=\"row\">\r\n                            <div class=\"col-sm-10\">\r\n                                <span class=\"glyphicon glyphicon-download-alt\"></span>\r\n                                ");
#nullable restore
#line 77 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                           Write(Html.ActionLink(" " + d.Name, "GetTemplate", new { practiceId = Model.PracticeID, type = d.DocumentType }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                                 if (!string.IsNullOrWhiteSpace(d.Title))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "72c772dd36f783a6de4f84686076d769eb7a2dd710487", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "title", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 80 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
AddHtmlAttributeValue("", 2996, d.Title, 2996, 8, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 81 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                            </div>\r\n                        </div>\r\n                    </p>\r\n");
#nullable restore
#line 86 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"            </div>
        </div>

        <div class=""panel panel-default"">
            <div class=""panel-heading"">
                <div class=""row"">
                    <div class=""col-sm-12"">
                        Для дистанционной формы прохождения практики
                    </div>
                </div>
            </div>
            <div class=""panel-body"">
");
#nullable restore
#line 99 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                 foreach (var d in Model.DistantDocuments)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <p>\r\n                        <div class=\"row\">\r\n                            <div class=\"col-sm-10\">\r\n                                <span class=\"glyphicon glyphicon-download-alt\"></span>\r\n                                ");
#nullable restore
#line 105 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                           Write(Html.ActionLink(" " + d.Name, "GetTemplate", new { practiceId = Model.PracticeID, type = d.DocumentType }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n");
#nullable restore
#line 106 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                                 if (!string.IsNullOrWhiteSpace(d.Title))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "72c772dd36f783a6de4f84686076d769eb7a2dd714237", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "title", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 108 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
AddHtmlAttributeValue("", 4175, d.Title, 4175, 8, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 109 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                            </div>\r\n                        </div>\r\n                    </p>\r\n");
#nullable restore
#line 114 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"            </div>
        </div>

        <div class=""panel panel-default"">
            <div class=""panel-heading"">
                <div class=""row"">
                    <div class=""col-sm-12"">
                        Отчет по практике
                    </div>
                </div>
            </div>
            <div class=""panel-body"">
");
#nullable restore
#line 127 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                 foreach (var d in Model.AfterDocuments)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <p>\r\n                        <div class=\"row\">\r\n                            <div class=\"col-sm-10\">\r\n                                <span class=\"glyphicon glyphicon-download-alt\"></span>\r\n                                ");
#nullable restore
#line 133 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                           Write(Html.ActionLink(" " + d.Name, "GetTemplate", new { practiceId = Model.PracticeID, type = d.DocumentType }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n");
#nullable restore
#line 134 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                                 if (!string.IsNullOrWhiteSpace(d.Title))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "72c772dd36f783a6de4f84686076d769eb7a2dd717962", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "title", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 136 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
AddHtmlAttributeValue("", 5325, d.Title, 5325, 8, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 137 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                        </div>\r\n                    </p>\r\n");
#nullable restore
#line 141 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Document.cshtml"
                }

#line default
#line hidden
#nullable disable
                WriteLiteral("            </div>\r\n        </div>\r\n    </div>\r\n\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"


</html>

<script>
    // после загрузки страницы
    $(function () {
        // инициализировать все элементы на страницы, имеющих атрибут data-toggle=""tooltip"", как компоненты tooltip
        $('[data-toggle=""tooltip""]').tooltip()
    })
</script>


");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Frames.Controllers.DocumentListVM> Html { get; private set; }
    }
}
#pragma warning restore 1591
