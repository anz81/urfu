#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "03a6a03fe27632bd946368a8acb968684f8320b6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ProfActivityArea_Index), @"mvc.1.0.view", @"/Views/ProfActivityArea/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"03a6a03fe27632bd946368a8acb968684f8320b6", @"/Views/ProfActivityArea/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ProfActivityArea_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml"
  
    ViewBag.Title = "Области профессиональной деятельности";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"" >

    Ext.onReady(function() {
            Ext.tip.QuickTipManager.init();

            var store = Ext.create(""Ext.data.Store"",
                {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: window.location.pathname,
                        reader: {
                            type: 'json',
                            rootProperty: 'data'
                        }
                    }
                });

            var gridPanel = Ext.create('Ext.grid.Panel',
                {
                    region: 'center',
                    store: store,
                    columnLines: true,
                    plugins: 'gridfilters',
                    tbar: [
                        {
                            xtype: 'button',
                            text: 'Добавить',
                            hidden: '");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml"
                                Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' == 'False',
                            handler: function() {
                                createRecordWindow().show();
                            }
                        }
                    ],
                    columns: [
                        { xtype: 'rownumberer', header: '№', width: 60 },
                        {
                            header: 'Код',
                            dataIndex: 'Code',
                            width: 200,
                            align: 'center',
                            filter: {
                                type: 'string',
                                itemDefaults: {
                                    emptyText: 'Искать...'
                                }
                            }
                        },
                        {
                            header: 'Область профессиональной деятельности',
                            dataIndex: 'Title',
                            width: 590,
               ");
                WriteLiteral(@"             filter: {
                                type: 'string',
                                itemDefaults: {
                                    emptyText: 'Искать...'
                                }
                            }
                        },
                        {
                            xtype: 'actioncolumn',
                            resizable: false,
                            sortable: false,
                            width: 70,
                            hidden: '");
#nullable restore
#line 72 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml"
                                Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral("\' == \'False\',\r\n                            items: [\r\n                                {\r\n                                    icon: \'");
#nullable restore
#line 75 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml"
                                      Write(Url.Content("/Content/Images/edit.png"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                                    iconCls: 'icon-padding',
                                    tooltip: 'Редактировать',
                                    handler: function(grid, rowIndex, colIndex, item, e, record) {
                                        createRecordWindow(record).show();
                                    }
                                },
                                {
                                    icon: '");
#nullable restore
#line 83 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml"
                                      Write(Url.Content("/Content/Images/remove.png"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                                    tooltip: 'Удалить',
                                    iconCls: 'icon-padding',
                                    handler: function(grid, rowIndex, colIndex, item, e, record) {
                                        Ext.MessageBox.show({
                                            title: 'Удаление',
                                            msg: 'Вы действительно хотите удалить запись?',
                                            buttons: Ext.MessageBox.YESNO,
                                            fn: function(button) {
                                                if ('yes' == button) {
                                                    Ext.Ajax.request({
                                                        method: 'GET',
                                                        url: '");
#nullable restore
#line 95 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml"
                                                         Write(Url.Action("Delete"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                                                        params: { code: record.get('Code') },
                                                        success: function(response) {
                                                            var r = Ext.decode(response.responseText);
                                                            if (!r.success)
                                                                Ext.MessageBox.alert('', r.message);
                                                            else {
                                                                store.reload();
                                                                Ext.MessageBox.alert('', 'Запись удалена');
                                                            }
                                                        },
                                                        failure: function(xhr) {
                                                            Ext.MessageBox.show({
         ");
                WriteLiteral(@"                                                       msg: xhr.responseText
                                                            })

                                                        }
                                                    });
                                                }
                                            },
                                            icon: Ext.MessageBox.QUESTION
                                        })
                                    }
                                }
                            ]
                        }
                    ]
                });

            function createRecordWindow(record) {
                return Ext.create('Ext.window.Window',
                    {
                        title: record ? 'Редактирование записи' : 'Добавление записи',
                        closeAction: 'hide',
                        closeToolText: 'Закрыть окно',
                        resizable: false,
     ");
                WriteLiteral(@"                   bodyPadding: 6,
                        viewModel: {
                            data: Ext.apply({}, record ? record.data : {}),
                        },
                        items: {
                            xtype: 'form',
                            layout: { type: 'vbox', align: 'stretch' },
                            defaults: {
                                xtype: 'textfield',
                                labelWidth: 100,
                                width: 450
                            },
                            items: [
                                {
                                    fieldLabel: 'Код',
                                    bind: '{Code}',
                                    name: 'Code',
                                    readOnly: record,
                                    focusable:!record,
                                    allowBlank: false,
                                    validator: function (v) {
           ");
                WriteLiteral(@"                             return /^[0-9]*$/.test(v) ? true : 'Могут использоваться только цифры';
                                    },
                                    listeners: {
                                        blur: function (field) {
                                            if (store.getDataSource().find('Code',field.value)) {
                                                Ext.MessageBox.alert('Ошибка', 'Существует область профессиональной деятельности с кодом ' + field.value);
                                                record ? field.setValue(record.get('Code')) : field.setValue('');
                                            }
                                        }
                                    }

                                },
                                {
                                    fieldLabel: 'Название',
                                    bind: '{Title}',
                                    name: 'Title',
                           ");
                WriteLiteral(@"         allowBlank: false
                                }
                            ],
                            buttons: [
                                {

                                    text: ""Сохранить"",
                                    formBind: true,
                                    handler: function() {

                                        var window = this.up('window');
                                        var form = window.down('form');

                                        if (!form.isValid()) {
                                            Ext.Msg.alert('Ошибка', 'Заполнены не все поля');
                                            return;
                                        }

                                        form.submit({
                                            url: record ? '");
#nullable restore
#line 187 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml"
                                                      Write(Url.Action("Edit"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\' : \'");
#nullable restore
#line 187 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfActivityArea\Index.cshtml"
                                                                              Write(Url.Action("Create"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                                            success: function(form, action) {
                                                var r = action.result;
                                                if (!r.success) {
                                                    Ext.MessageBox.alert('', r.message);
                                                } else {
                                                    Ext.MessageBox.alert('', 'Информация сохранена.');
                                                    store.reload();
                                                    window.close();
                                                }
                                            },
                                            failure: function(form, action) {
                                                Ext.MessageBox.alert('',
                                                    'Информация не сохранена. ' + action.result.message);
                                            }
      ");
                WriteLiteral(@"                                  });
                                    }
                                },
                                {
                                    text: ""Отмена"",
                                    handler: function() { this.up('window').close(); }
                                }
                            ]
                        }
                    })
            }


            var items = [
                gridPanel
            ];

            Urfu.createViewport('border', items);
        });

    </script >
");
            }
            );
            WriteLiteral("\r\n\r\n");
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
