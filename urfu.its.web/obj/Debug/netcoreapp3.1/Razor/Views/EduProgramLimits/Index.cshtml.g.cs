#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "dc7117165ac624e41a89c444c17318d38581213d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_EduProgramLimits_Index), @"mvc.1.0.view", @"/Views/EduProgramLimits/Index.cshtml")]
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
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
using Urfu.Its.Web.DataContext;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"dc7117165ac624e41a89c444c17318d38581213d", @"/Views/EduProgramLimits/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_EduProgramLimits_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IList<Urfu.Its.Web.Models.LimitViewModel>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 4 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
 if (ViewBag.Variant.IsBase)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <h2>Лимиты версии ОП &quot;");
#nullable restore
#line 6 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                          Write(ViewBag.Variant.Program.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("&quot; направление ");
#nullable restore
#line 6 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                                                                          Write(ViewBag.Variant.Program.Direction.okso);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n");
#nullable restore
#line 7 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
}
else
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <h2>Лимиты траектории &quot;");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                           Write(ViewBag.Variant.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("&quot; направление ");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                                                                   Write(ViewBag.Variant.Program.Direction.okso);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n");
#nullable restore
#line 11 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<nav class=\"navbar navbar-default\">\r\n    <ul class=\"nav navbar-nav\">\r\n        <li>");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
       Write(Html.ActionLink("Вернутся к " + (ViewBag.Variant.IsBase ? "версии" : "траектории") + " ОП", "BasicContentEdit", "Variant", new { variantId = ViewBag.Variant.Id }, null));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\r\n    </ul>\r\n</nav>\r\n\r\n");
#nullable restore
#line 19 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
 if (ViewBag.Error)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"alert alert-danger\">\r\n        <b>Для модулей не определена часть УП:</b> ");
#nullable restore
#line 22 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                                              Write(ViewBag.WrongModules);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n");
#nullable restore
#line 24 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
}
else
{


#line default
#line hidden
#nullable disable
            WriteLiteral("<table class=\"table table-bordered table-nonfluid\">\r\n    <thead>\r\n        <tr>\r\n            <th>Название модуля</th>\r\n            <th>Группа модуля</th>\r\n            <th>Количество студентов</th>\r\n\r\n");
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
             if (!ViewBag.Variant.IsBase)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <th>Общий лимит</th>\r\n");
#nullable restore
#line 38 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
            }
            else
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <th>Распределение по траекториям</th>\r\n");
#nullable restore
#line 42 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("            <th>Комментарий</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n");
#nullable restore
#line 47 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
         foreach (var item in Model)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>");
#nullable restore
#line 50 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
               Write(Html.DisplayFor(m => item.ModuleNumberAndTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
               Write(Html.DisplayFor(m => item.GroupType));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td style=\"text-align: center\">\r\n");
#nullable restore
#line 53 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                     if (ViewBag.Variant.State != VariantState.Approved)
                    {
                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 55 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                   Write(Html.TextBoxFor(m => item.StudentsCount, new
                   {
                       @class = "autosave form-control",
                       @autocomplete = "off",
                       @data_path = Url.Action("SetStudentsCount", new { variantId = ViewBag.Variant.Id, moduleId = item.ModuleId }),
                       @data_message = "message_" + item.ModuleId
                   }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 61 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                     
                    }
                    else
                    {
                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                   Write(Html.DisplayFor(m => item.StudentsCount));

#line default
#line hidden
#nullable disable
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                                                                 
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </td>\r\n\r\n");
#nullable restore
#line 69 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                 if (!ViewBag.Variant.IsBase)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <td>");
#nullable restore
#line 71 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                   Write(item.ProgramStudentsCount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n");
#nullable restore
#line 72 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                }
                else
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <td>\r\n");
#nullable restore
#line 76 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                         foreach (var limit in item.VariantLimits)
                        {
                            

#line default
#line hidden
#nullable disable
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                             Write(limit.Variant.Program.Direction.okso);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                                                                   Write(limit.Variant.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(" - <strong>");
#nullable restore
#line 78 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                                                                                                 Write(limit.StudentsCount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>");
            WriteLiteral("<br />\r\n");
#nullable restore
#line 79 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </td>\r\n");
#nullable restore
#line 81 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                <td");
            BeginWriteAttribute("id", " id=\"", 2812, "\"", 2841, 2);
            WriteAttributeValue("", 2817, "message_", 2817, 8, true);
#nullable restore
#line 82 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
WriteAttributeValue("", 2825, item.ModuleId, 2825, 16, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 82 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
                                             Write(item.Comment);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            </tr>\r\n");
#nullable restore
#line 84 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n");
#nullable restore
#line 87 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduProgramLimits\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script>
        $(function () {
            function debounce() {
                var el = $(this);
                var value = $(this).val();

                $.post(el.data(""path""), { ""value"": value }, function (data) {
                    el.removeClass(""modified"");
                    if (data.status) {
                        el.removeClass(""modified"");
                        el.addClass(""saved"");
                        setTimeout(function () {
                            el.removeClass(""saved"");
                        }, 1000);
                    } else {
                        el.addClass(""error"");
                    }

                    var messageid = el.data(""message"");
                    if (data.message && messageid) {
                        $(""#"" + messageid).html(data.message);
                    } else if (data.message) {
                        el.attr(""title"", data.message);
                    }
                });
            }

            funct");
                WriteLiteral(@"ion oninput() {

                var that = $(this);
                that.removeClass(""error"");
                that.removeAttr(""title"");
                that.removeClass(""saved"");
                that.addClass(""modified"");

                var messageid = that.data(""message"");
                if (messageid) {
                    var message = $(""#"" + messageid);
                    if (message) {
                        message.html("""");
                    }
                } else {
                    that.attr(""title"", """");
                }

                var timerid = that.data(""timerid"");
                if (timerid) {
                    clearTimeout(timerid);
                    that.removeData(""timerid"");
                }
                timerid = setTimeout(function () {
                    debounce.call(that);
                }, 500);
                that.data(""timerid"", timerid);
            }

            $(""input.autosave"").on('input', oninput);
            $("".");
                WriteLiteral(@"delete"").on(""click"", function (event) {
                if (confirm(""Удалить запись?"")) {
                    var el = $(this);
                    $.post(el.attr(""href""), null, function () {
                        location.reload();
                    });
                }
                return false;
            });
        });
    </script>

");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IList<Urfu.Its.Web.Models.LimitViewModel>> Html { get; private set; }
    }
}
#pragma warning restore 1591
