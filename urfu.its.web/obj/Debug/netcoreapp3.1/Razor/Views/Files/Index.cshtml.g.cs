#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Files\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2b21a4f11ec77c176747d67bf7686373f516f01b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Files_Index), @"mvc.1.0.view", @"/Views/Files/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2b21a4f11ec77c176747d67bf7686373f516f01b", @"/Views/Files/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Files_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Files\Index.cshtml"
  
    ViewBag.Title = "Файлы";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            WriteLiteral(@"    <div class=""form-horizontal"">
        <script type=""text/javascript"">
            var lastFilter = [];
            Ext.onReady(function () {
                Ext.tip.QuickTipManager.init();
                
                var store = Ext.create(""Ext.data.Store"",
                    {
                        autoLoad: true,
                        pageSize: 25,
                        remoteSort: true,
                        remoteFilter: true,
                        proxy: {
                            type: 'ajax',
                            url: '/Files/Index',
                            reader: {
                                type: 'json',
                                rootProperty: 'data',
                                totalProperty: 'total'
                            }
                        }
                    });

                var tpl = '<a href=""");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Files\Index.cshtml"
                               Write(Url.Action("Download"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?path={fullPath}&fileName={fileName}"">{fileName}</a>';

                var filePanel = Ext.create('Ext.window.Window',
                    {
                        title: ""Документ"",
                        closeAction: 'hide',
                        scrollable: true,
                        resizable: true,
                        bodyPadding: 6,
                        fileUpload: true,
                        items: [
                            Ext.create('Ext.form.Panel',
                                {
                                    title: '',
                                    id: 'fileUploadFileTest',
                                    fileUpload: true,
                                    items: [
                                        {
                                            xtype: 'filefield',
                                            fieldLabel: 'Документ',
                                            emptyText: 'Выбрать документ',
                           ");
            WriteLiteral(@"                 name: 'document-path',
                                            buttonText: 'Загрузить',
                                            width: 600,
                                            multiple: false
                                        }
                                    ],
                                    buttons: [
                                        {

                                            text: ""Сохранить"",
                                            handler: function () {
                                                var form = Ext.getCmp('fileUploadFileTest');
                                                form.getForm().submit({
                                                    method: 'POST',
                                                    url: '");
#nullable restore
#line 66 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Files\Index.cshtml"
                                                     Write(Url.Action("Upload"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                                    success: function (response) {
                                                        store.reload();
                                                        filePanel.hide();
                                                    },
                                                    failure: function (response) {
                                                        Ext.MessageBox.alert('Ошибка');
                                                    }
                                                });
                                            }
                                        },
                                        {
                                            text: ""Отмена"",
                                            handler: function () {
                                                filePanel.hide();
                                            }
                                        }
                               ");
            WriteLiteral(@"     ]

                                })
                        ]
                    });

                var gridPanel = Ext.create('Ext.grid.Panel',
                    {
                        region: 'center',
                        store: store,
                        loadMask: true,
                        columnLines: true,
                        tbar: [
                            {
                                xtype: 'button',
                                text: ""Загрузить"",
                                handler: function () {
                                    filePanel.show();
                                }
                            }
                        ],
                        columns: [
                            { xtype: 'rownumberer', width: 50 },
                            {
                                header: 'Файл',
                                xtype: 'templatecolumn',
                                sortable: false,
           ");
            WriteLiteral(@"                     tpl: tpl,
                                width: 500
                            },
                            {
                                xtype: 'actioncolumn',
                                region: 'center',
                                sortable: false,
                                width: 70,
                                items: [
                                    {
                                        icon: '");
#nullable restore
#line 120 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Files\Index.cshtml"
                                          Write(Url.Content("~/Content/Images/remove.png"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                        iconCls: 'icon-padding',
                                        text: 'Удалить',
                                        tooltip: 'Удалить',
                                        handler: function (grid, rowIndex, colIndex) {

                                                var request = function () {
                                                    var rec = grid.getStore().getAt(rowIndex);
                                                    Ext.Ajax.request({
                                                            url: '");
#nullable restore
#line 129 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Files\Index.cshtml"
                                                             Write(Url.Action("Delete"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                                            params: {
                                                                path: rec.get(""fullPath"")
                                                            },
                                                            success: function (response) {
                                                                store.reload();
                                                            },
                                                            failure: function (response) {
                                                                Ext.MessageBox.alert('Ошибка');
                                                            }
                                                        });
                                                }
                                                Ext.MessageBox.show({
                                                    title: 'Информационное сообщение',
                                  ");
            WriteLiteral(@"                  msg: ""Удалить документ?"",
                                                    buttons: Ext.MessageBox.YESNO,
                                                    fn: function (btn) {
                                                        if (btn === 'yes') {
                                                            request();
                                                        }
                                                    }
                                                });
                                        }
                                    }
                                ]
                            }
                        ]

                    });

            var items = [
                gridPanel
            ];

            Urfu.createViewport('border', items);

            });
        </script>
    </div>
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591