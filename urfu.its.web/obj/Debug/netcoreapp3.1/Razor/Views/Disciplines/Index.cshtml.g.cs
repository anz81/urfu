#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Disciplines\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "899dcf374447699aa7bf3401a00e7eeb95e3df40"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Disciplines_Index), @"mvc.1.0.view", @"/Views/Disciplines/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"899dcf374447699aa7bf3401a00e7eeb95e3df40", @"/Views/Disciplines/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Disciplines_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Disciplines\Index.cshtml"
  
    ViewBag.Title = "Дисциплины";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">
        Ext.onReady(function () {
            Ext.tip.QuickTipManager.init();
            var disciplinesStore = Ext.create(""Ext.data.BufferedStore"",
                {
                    fields: [""id"", ""title"", ""section"", ""testUnits"", ""file""],
                    autoLoad: true,
                    pageSize: 300,
                    remoteSort: true,
                    remoteFilter: true,
                    proxy: {
                        type: 'ajax',
                        url: '/Disciplines/Index',
                        reader: {
                            type: 'json',
                            rootProperty: 'data',
                            totalProperty: 'total'
                        }
                    }
                });

            window.disciplinesStore = disciplinesStore;

            var tpl = '<a href=""{file}"">Файл</a> | <a href=""/Disciplines/Edit?id={id}"">Изменить</a> \
                | <a href=""/Disciplines/Delet");
                WriteLiteral(@"e?id={id}"">Удалить</a>';

            var gridPanel = Ext.create('Ext.grid.Panel',
                {
                    region: 'center',
                    store: disciplinesStore,
                    loadMask: true,
                    tbar: [
                        {
                            xtype: 'box',
                            autoEl: { tag: 'a', href: '/Disciplines/Create', html: 'Создать новую дисциплину' }
                        },
                        '-',
                        {
                            xtype: 'label',
                            text: 'Название дисциплины'
                        },
                        {
                            id: 'txtTitle',
                            xtype: 'textfield',
                        },
                        {
                            xtype: 'label',
                            text: 'Тип'
                        },
                        {
                            id: 'txtSection',
      ");
                WriteLiteral(@"                      xtype: 'textfield',
                        },
                        {
                            xtype: 'button',
                            text: 'Применить',
                            handler: function () {
                                disciplinesStore.filter([
                                    { property: 'title', value: Ext.getCmp('txtTitle').getValue() },
                                    { property: 'section', value: Ext.getCmp('txtSection').getValue() },
                                    ]);
                            }
                        },
                        {
                            xtype: 'button',
                            text: 'Отменить',
                            handler: function () { disciplinesStore.clearFilter(); }
                        }
                    ],
                    columns: [
                        { xtype: 'rownumberer', width: 50 },
                        {
                            header:");
                WriteLiteral(@" 'Название дисциплины',
                            dataIndex: 'title',
                            width: 500,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        },
                        {
                            header: 'Тип',
                            dataIndex: 'section',
                            width: 210,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Зачётные единицы',
                            align: 'center',
                            dataIndex: 'testUnits',
                            width: 180,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            xtype: 'templatecolumn',
                            tpl: tpl,
                            width: 220
                        }
                    ]
                });
");
                WriteLiteral("\n            var items = [\r\n                gridPanel\r\n            ];\r\n\r\n            Urfu.createViewport(\'border\', items);\r\n        });\r\n    </script>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
