#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e23eed56bbedef66d26b9f7058c0138ed7fe0565"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Pratice_Scan), @"mvc.1.0.view", @"/Views/Pratice/Scan.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e23eed56bbedef66d26b9f7058c0138ed7fe0565", @"/Views/Pratice/Scan.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a29a642edbfb9ad6e9236f96e04f5da3c1de2bef", @"/Views/_ViewImports.cshtml")]
    public class Views_Pratice_Scan : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Frames.Controllers.ScanListVM>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("loadForm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.SingleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("modelAttribute", new global::Microsoft.AspNetCore.Html.HtmlString("data"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("enctype", new global::Microsoft.AspNetCore.Html.HtmlString("multipart/form-data"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.SingleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
  
    Layout = null;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<!DOCTYPE html>\r\n\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e23eed56bbedef66d26b9f7058c0138ed7fe05655100", async() => {
                WriteLiteral("\r\n    <meta name=\"viewport\" content=\"width=device-width\" />\r\n    <title>Scan</title>\r\n\r\n    ");
#nullable restore
#line 14 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
Write(Styles.Render("~/Content/css"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"

    <style>
        input, select, textarea {
            max-width: initial;
        }
        .btn-danger {
            color: #ce1a16;
            background-color: #f5f5f5;
            border-color: #f5f5f5;
        }
        .btn-sm, .btn-xs {
            border-radius: 10px;
        }
        p.dline {
            line-height: .8;
            font-weight: bold;
        }
        .message {
            padding-left: 15px;            
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e23eed56bbedef66d26b9f7058c0138ed7fe05657123", async() => {
                WriteLiteral("\r\n\r\n    <div class=\"container-fluid\">\r\n        <h4>");
#nullable restore
#line 46 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
       Write(Model.DisciplineTitle);

#line default
#line hidden
#nullable disable
                WriteLiteral("</h4>\r\n        <div class=\"row\">\r\n            <h5 class=\"col-sm-4\">");
#nullable restore
#line 48 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                            Write(Model.YearInfo);

#line default
#line hidden
#nullable disable
                WriteLiteral("</h5>\r\n            <h5 class=\"col-sm-4\">");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                            Write(Model.Semester);

#line default
#line hidden
#nullable disable
                WriteLiteral("</h5>\r\n            <h5 class=\"col-sm-4\">");
#nullable restore
#line 50 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                            Write(Model.PeriodInfo());

#line default
#line hidden
#nullable disable
                WriteLiteral("</h5>\r\n        </div>\r\n\r\n\r\n        <div class=\"container row\">\r\n            ");
#nullable restore
#line 55 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
       Write(Html.ActionLink("Выбор предприятия, руководителя, темы", "Practice", null, new { ID = Model.PracticeID }, new { @class = "btn btn-info", role = "button" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
       Write(Html.ActionLink("Шаблоны документов", "Document", null, new { ID = Model.PracticeID }, new { @class = "btn btn-info", role = "button" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 57 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
       Write(Html.ActionLink("Сканы документов", "Scan", null, new { ID = Model.PracticeID }, new { @class = "btn btn-success", role = "button" }));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
        </div>
        <p>
        </p>
        <div class=""message"">
            <p class=""dline"">Объем файла не более 10 Мб.</p>
            <p class=""dline"">Перед загрузкой документов сократите объем изображений.</p>
            <p class=""dline"">Не загружайте документы с изображениями высокого качества.</p>
            <p class=""dline"">По статистике средний объем загружаемого документа не превышает 3 Мб.</p>
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
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                 foreach (var d in Model.BeforeDocuments)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <p>\r\n                        <div class=\"container row\">\r\n                            <div class=\"col-sm-3\">");
#nullable restore
#line 82 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                             Write(d.TypeName);

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n\r\n                            <div class=\"col-sm-2\">\r\n");
#nullable restore
#line 85 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && (d.Status == "Согласовано"))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <button type=\"button\" disabled class=\"btn btn-primary\"");
                BeginWriteAttribute("onclick", " onclick=\"", 3124, "\"", 3156, 3);
                WriteAttributeValue("", 3134, "loadClicked(", 3134, 12, true);
#nullable restore
#line 87 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 3146, d.TypeId, 3146, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 3155, ")", 3155, 1, true);
                EndWriteAttribute();
                WriteLiteral(">\r\n                                        обзор... <span class=\"glyphicon glyphicon-search\"></span>\r\n                                    </button>\r\n");
#nullable restore
#line 90 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                }
                                else
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral(" <button type=\"button\" class=\"btn btn-primary\"");
                BeginWriteAttribute("onclick", " onclick=\"", 3458, "\"", 3490, 3);
                WriteAttributeValue("", 3468, "loadClicked(", 3468, 12, true);
#nullable restore
#line 92 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 3480, d.TypeId, 3480, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 3489, ")", 3489, 1, true);
                EndWriteAttribute();
                WriteLiteral(">\r\n                                        обзор... <span class=\"glyphicon glyphicon-search\"></span>\r\n                                    </button>\r\n");
#nullable restore
#line 95 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-3\">\r\n");
#nullable restore
#line 98 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && !string.IsNullOrWhiteSpace(d.DocumentName))
                                {
                                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 100 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                               Write(Html.ActionLink(@d.DocumentName, "GetScan", new { documentId = d.DocumentId }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 100 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                                                                                                   ;

                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-1\">");
#nullable restore
#line 104 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                             Write(d.Date);

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n                            <div class=\"col-sm-1\">\r\n");
#nullable restore
#line 106 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && (d.Status == "" || d.Status == "Отклонено") && !string.IsNullOrWhiteSpace(d.DocumentName))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <button type=\"button\" class=\"btn btn-danger btn-xs\"");
                BeginWriteAttribute("onclick", " onclick=\"", 4487, "\"", 4540, 6);
                WriteAttributeValue("", 4497, "removeClicked(", 4497, 14, true);
#nullable restore
#line 108 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 4511, d.TypeId, 4511, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 4520, ",", 4520, 1, true);
                WriteAttributeValue(" ", 4521, "\'", 4522, 2, true);
#nullable restore
#line 108 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 4523, d.DocumentName, 4523, 15, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 4538, "\')", 4538, 2, true);
                EndWriteAttribute();
                WriteLiteral(">\r\n                                        <span class=\"glyphicon glyphicon-remove\"></span>\r\n\r\n                                    </button>\r\n");
#nullable restore
#line 112 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-2 col-sm-push-1\">");
#nullable restore
#line 114 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                                           Write(d.StatusComment());

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n                        </div>\r\n                    </p>\r\n");
#nullable restore
#line 117 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
            </div>
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
#line 131 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                 foreach (var d in Model.DistantDocuments)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <p>\r\n                        <div class=\"container row\">\r\n                            <div class=\"col-sm-3\">");
#nullable restore
#line 135 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                             Write(d.TypeName);

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n\r\n                            <div class=\"col-sm-2\">\r\n");
#nullable restore
#line 138 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && (d.Status == "Согласовано"))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <button type=\"button\" disabled class=\"btn btn-primary\"");
                BeginWriteAttribute("onclick", " onclick=\"", 5801, "\"", 5833, 3);
                WriteAttributeValue("", 5811, "loadClicked(", 5811, 12, true);
#nullable restore
#line 140 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 5823, d.TypeId, 5823, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 5832, ")", 5832, 1, true);
                EndWriteAttribute();
                WriteLiteral(">\r\n                                        обзор... <span class=\"glyphicon glyphicon-search\"></span>\r\n                                    </button>\r\n");
#nullable restore
#line 143 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                }
                                else
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral(" <button type=\"button\" class=\"btn btn-primary\"");
                BeginWriteAttribute("onclick", " onclick=\"", 6135, "\"", 6167, 3);
                WriteAttributeValue("", 6145, "loadClicked(", 6145, 12, true);
#nullable restore
#line 145 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 6157, d.TypeId, 6157, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 6166, ")", 6166, 1, true);
                EndWriteAttribute();
                WriteLiteral(">\r\n                                        обзор... <span class=\"glyphicon glyphicon-search\"></span>\r\n                                    </button>\r\n");
#nullable restore
#line 148 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-3\">\r\n");
#nullable restore
#line 151 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && !string.IsNullOrWhiteSpace(d.DocumentName))
                                {
                                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 153 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                               Write(Html.ActionLink(@d.DocumentName, "GetScan", new { documentId = d.DocumentId }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 153 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                                                                                                   ;

                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-1\">");
#nullable restore
#line 157 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                             Write(d.Date);

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n                            <div class=\"col-sm-1\">\r\n");
#nullable restore
#line 159 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && (d.Status == "" || d.Status == "Отклонено") && !string.IsNullOrWhiteSpace(d.DocumentName))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <button type=\"button\" class=\"btn btn-danger btn-xs\"");
                BeginWriteAttribute("onclick", " onclick=\"", 7164, "\"", 7217, 6);
                WriteAttributeValue("", 7174, "removeClicked(", 7174, 14, true);
#nullable restore
#line 161 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 7188, d.TypeId, 7188, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 7197, ",", 7197, 1, true);
                WriteAttributeValue(" ", 7198, "\'", 7199, 2, true);
#nullable restore
#line 161 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 7200, d.DocumentName, 7200, 15, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 7215, "\')", 7215, 2, true);
                EndWriteAttribute();
                WriteLiteral(">\r\n                                        <span class=\"glyphicon glyphicon-remove\"></span>\r\n\r\n                                    </button>\r\n");
#nullable restore
#line 165 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-2 col-sm-push-1\">");
#nullable restore
#line 167 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                                           Write(d.StatusComment());

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n                        </div>\r\n                    </p>\r\n");
#nullable restore
#line 170 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
            </div>
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
#line 184 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                 foreach (var d in Model.AfterDocuments)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <p>\r\n                        <div class=\"container row\">\r\n                            <div class=\"col-sm-4\">");
#nullable restore
#line 188 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                             Write(d.TypeName);

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n                            <div class=\"col-sm-2\">\r\n");
#nullable restore
#line 190 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && (d.Status == "Согласовано"))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <button type=\"button\" disabled class=\"btn btn-primary\"");
                BeginWriteAttribute("onclick", " onclick=\"", 8447, "\"", 8479, 3);
                WriteAttributeValue("", 8457, "loadClicked(", 8457, 12, true);
#nullable restore
#line 192 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 8469, d.TypeId, 8469, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 8478, ")", 8478, 1, true);
                EndWriteAttribute();
                WriteLiteral(">обзор... <span class=\"glyphicon glyphicon-search\"></span></button>\r\n");
#nullable restore
#line 193 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                }
                                else
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <button type=\"button\" class=\"btn btn-primary\"");
                BeginWriteAttribute("onclick", " onclick=\"", 8738, "\"", 8770, 3);
                WriteAttributeValue("", 8748, "loadClicked(", 8748, 12, true);
#nullable restore
#line 196 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 8760, d.TypeId, 8760, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 8769, ")", 8769, 1, true);
                EndWriteAttribute();
                WriteLiteral(">обзор... <span class=\"glyphicon glyphicon-search\"></span></button>\r\n");
#nullable restore
#line 197 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"

                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-3\">\r\n");
#nullable restore
#line 201 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && !string.IsNullOrWhiteSpace(d.DocumentName))
                                {
                                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 203 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                               Write(Html.ActionLink(@d.DocumentName, "GetScan", new { documentId = d.DocumentId }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 203 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                                                                                                   ;
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-1\">");
#nullable restore
#line 206 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                             Write(d.Date);

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n                            <div class=\"col-sm-1\">\r\n");
#nullable restore
#line 208 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                 if (d.DocumentId != null && (d.Status == "" || d.Status == "Отклонено") && !string.IsNullOrWhiteSpace(d.DocumentName))
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <button type=\"button\" class=\"btn btn-danger btn-xs\"");
                BeginWriteAttribute("onclick", " onclick=\"", 9687, "\"", 9740, 6);
                WriteAttributeValue("", 9697, "removeClicked(", 9697, 14, true);
#nullable restore
#line 210 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 9711, d.TypeId, 9711, 9, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 9720, ",", 9720, 1, true);
                WriteAttributeValue(" ", 9721, "\'", 9722, 2, true);
#nullable restore
#line 210 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 9723, d.DocumentName, 9723, 15, false);

#line default
#line hidden
#nullable disable
                WriteAttributeValue("", 9738, "\')", 9738, 2, true);
                EndWriteAttribute();
                WriteLiteral(">\r\n                                        <span class=\"glyphicon glyphicon-remove\"></span>\r\n                                    </button>\r\n");
#nullable restore
#line 213 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </div>\r\n                            <div class=\"col-sm-2\">");
#nullable restore
#line 215 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                             Write(d.StatusComment());

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n                        </div>\r\n                    </p>\r\n");
#nullable restore
#line 218 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                }

#line default
#line hidden
#nullable disable
                WriteLiteral("            </div>\r\n        </div>\r\n    </div>\r\n\r\n");
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e23eed56bbedef66d26b9f7058c0138ed7fe056531637", async() => {
                    WriteLiteral("\r\n        <div style=\"overflow: hidden;height : 0px;\">\r\n");
                    WriteLiteral("            <input type=\"hidden\" name=\"practiceId\"");
                    BeginWriteAttribute("value", " value=\"", 11698, "\"", 11723, 1);
#nullable restore
#line 249 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
WriteAttributeValue("", 11706, Model.PracticeID, 11706, 17, false);

#line default
#line hidden
#nullable disable
                    EndWriteAttribute();
                    WriteLiteral(" />\r\n            <input type=\"hidden\" id=\"documentType\" name=\"type\"");
                    BeginWriteAttribute("value", " value=\"", 11791, "\"", 11799, 0);
                    EndWriteAttribute();
                    WriteLiteral(" />\r\n");
                    WriteLiteral("            <input type=\"file\" id=\"file\" name=\"file\" onchange=\"submitform(event)\" />\r\n        </div>\r\n        ");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
                __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_1.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "action", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.SingleQuotes);
#nullable restore
#line 246 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
AddHtmlAttributeValue("", 11440, Url.Action("UploadDocument"), 11440, 29, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
                WriteLiteral(@"
            <script>

        //var documentType = 0;
                function submitform(event) {
                    var file = document.getElementById('file').files[0];
                    if (file && file.size < 10485760) {
                        $('#loadForm').submit();
                    }
                    else {
                        alert(""Ваш документ не загружен. Превышен допустимый размер файла"");
                        event.preventDefault();                       
                    }
                }


        function loadClicked(type) {
            //documentType = type
            var documentType = document.getElementById(""documentType"");
            documentType.value = type;

            performClick(""file"");
        }


        // Формирование события клика на элементе.
        function performClick(elemId) {
            var elem = document.getElementById(elemId);
            if (elem && document.createEvent) {
                elem.value = """";

  ");
                WriteLiteral(@"              var evt = document.createEvent(""MouseEvents"");
                evt.initEvent(""click"", true, false);
                elem.dispatchEvent(evt);
            }
        }

        function removeClicked(type, name) {
            if (confirm('Вы действительно хотите удалить приложенный документ ' + name + '?'))
        {
                //documentType = type
                var form_data = new FormData();
            form_data.append(""practiceId"", ");
#nullable restore
#line 303 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                                      Write(Model.PracticeID);

#line default
#line hidden
#nullable disable
                WriteLiteral(");\r\n            form_data.append(\"type\", type);\r\n            $.ajax({\r\n                url: \'");
#nullable restore
#line 306 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\Scan.cshtml"
                 Write(Url.Action("DeleteDocument"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                type: ""POST"",
                dataType: 'script',
                cache: false,
                contentType: false,
                processData: false,
                data: form_data,
                success: function (html) {
                    location.reload();
                }
            });
        }
        };


");
                WriteLiteral("\r\n            </script>\r\n\r\n");
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
            WriteLiteral("\r\n\r\n\r\n\r\n</html>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Frames.Controllers.ScanListVM> Html { get; private set; }
    }
}
#pragma warning restore 1591