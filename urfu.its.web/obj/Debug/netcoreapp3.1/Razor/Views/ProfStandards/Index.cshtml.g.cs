#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f9e8c9a1c63f1efde55f999fb5124c7ec3f57054"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ProfStandards_Index), @"mvc.1.0.view", @"/Views/ProfStandards/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f9e8c9a1c63f1efde55f999fb5124c7ec3f57054", @"/Views/ProfStandards/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ProfStandards_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
  
    ViewBag.Title = "Профессиональные стандарты";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<style>
    .x-action-col-icon {
        height: 16px;
        width: 16px;
        margin-left: 7px !important;
    }

    .custom-combo .x-form-trigger-wrap-default {
        border: none;
    }

    .displayfield {
        margin-top: 11px;
        padding-left: 6px;
    }
    
</style>

");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">
    Ext.onReady(function() {
        Ext.tip.QuickTipManager.init();

        var filterName = ""ProfStandardsFilters"";

        var store = Ext.create(""Ext.data.Store"",
            {
                autoLoad: true,
                remoteSort: true,
                remoteFilter: true,
                proxy: {
                    type: 'ajax',
                    url: window.location.pathname,
                    reader: {
                        type: 'json',
                        rootProperty: 'data'
                    }
                }
            });

        var profActivityAreaStore = Ext.create(""Ext.data.Store"",
            {
                autoLoad: false,
                proxy: {
                    type: 'ajax',
                    url: '");
#nullable restore
#line 52 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
                     Write(Url.Action("GetProfActivityArea"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                    reader: { type: 'json' }
                }
            });

        var standardStore = Ext.create(""Ext.data.Store"",
            {
                autoLoad: false,
                proxy: {
                    type: 'ajax',
                    url: '");
#nullable restore
#line 62 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
                     Write(Url.Action("GetProfStandards"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                    reader: {
                        type: 'json',
                        rootProperty: 'data'
                    }
                }
            });


        var prevSettings = {};
        try {
            var prevSettingString = JSON.parse(localStorage.getItem(filterName) || ""[]"");

            for (var i = 0; i < prevSettingString.length; i++) {
                prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
            }
            store.setFilters(prevSettingString);
        } catch (err) {
            console.log(err);
        }

        var filtersWnd = null;
        var setFilters = function() {
            var settings = [
                { property: 'Code', value: filtersWnd.getComponent(""ProfStandardCodeField"").getValue() },
                { property: 'ProfArea', value: filtersWnd.getComponent(""ProfActivityAreaField"").getValue() }
            ];
            store.setFilters(settings);
            localStorage.setI");
                WriteLiteral(@"tem(filterName, JSON.stringify(settings));
        };

        filtersWnd = Ext.create('Ext.window.Window',
            {
                title: ""Фильтры"",
                closeAction: 'hide',
                closeToolText: 'Закрыть окно',
                resizable: false,
                autoHeight: true,
                bodyPadding: 6,
                defaults: {
                    xtype: 'textfield',
                    width: 500
                },
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: ""Область профессиональной деятельности"",
                        labelWidth: 200,
                        itemId: ""ProfActivityAreaField"",
                        store: profActivityAreaStore,
                        value: prevSettings[""ProfActivityAreaCode""],
                        queryMode: 'remote',
                        displayField: 'ProfAreaTitle',
                        valueField: 'ProfArea',
    ");
                WriteLiteral(@"                    listeners: {
                            change: function(t, newValue, oldValue) {
                                filtersWnd.getComponent('ProfStandardCodeField').clearValue();
                                standardStore.load({
                                    params: {
                                        areaCode: newValue
                                    }
                                })
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: ""Код профессионального стандарта"",
                        labelWidth: 200,
                        itemId: ""ProfStandardCodeField"",
                        store: standardStore,
                        value: prevSettings[""Code""],
                        queryMode: 'local',
                        displayField: 'Title',
                        valueField: 'Code',
                        any");
                WriteLiteral(@"Match: true
                    }
                ],
                buttons: [
                    {
                        text: ""OK"",
                        handler: function() {
                            setFilters();
                            filtersWnd.hide();
                        }
                    },
                    {
                        text: ""Отмена"",
                        handler: function() { filtersWnd.hide(); }
                    }
                ]

            });

        var gridPanel = Ext.create('Ext.grid.Panel',
            {
                region: 'center',
                store: store,
                columnLines: true,
                tbar: [
                    {
                        xtype: 'button',
                        text: 'Фильтры...',
                        handler: function() { filtersWnd.show(); }
                    },
                    {
                        xtype: 'button',
                        text: 'От");
                WriteLiteral(@"менить фильтры',
                        handler: function() {
                            store.clearFilter();
                            settings = [];
                            localStorage.setItem(filterName, JSON.stringify([]));
                        }
                    },
                    {
                        xtype: 'button',
                        text: 'Добавить стандарт',
                        hidden: '");
#nullable restore
#line 179 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
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
                        header: 'Код профессионального стандарта',
                        dataIndex: 'Code',
                        width: 200,
                        align: 'center',
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Наименование профессионального стандарта',
                        dataIndex: 'Title',
                        width: 590,
                        align: 'left',
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Область профессиональной деятельности',
                        dataIn");
                WriteLiteral(@"dex: 'ProfArea',
                        width: 250,
                        align: 'center',
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Вид профессиональной деятельности',
                        dataIndex: 'ProfKind',
                        width: 650,
                        align: 'left',
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        xtype: 'actioncolumn',
                        resizable: false,
                        sortable: false,
                        align: 'center',
                        width: 60,
                        hidden: '");
#nullable restore
#line 221 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
                            Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' == 'False',
                        items: [
                            {
                                icon: '/Content/Images/edit.png',
                                tooltip: 'Редактировать',
                                handler: function(grid, rowIndex, colIndex, item, e, record) {
                                    createRecordWindow(record).show();
                                }
                            },
                            {
                                icon: '");
#nullable restore
#line 231 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
                                  Write(Url.Content("/Content/Images/remove.png"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                                tooltip: 'Удалить',

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
#line 243 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
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
                WriteLiteral(@"   msg: xhr.responseText
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


        var proxy = new Ext.data.proxy.Ajax({
            url: '");
#nullable restore
#line 274 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
             Write(Url.Action("GetProfAktivityKinds"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
            reader: {
                type: 'json',
                rootProperty: 'data'
            }
        });


        function createRecordWindow(record) {
            return Ext.create('Ext.window.Window',
                {
                    title: record ? 'Редактирование записи' : 'Добавление записи',
                    closeAction: 'hide',
                    closeToolText: 'Закрыть окно',
                    resizable: false,
                    bodyPadding: 6,
                    viewModel: {
                        data: Ext.apply({}, record ? record.data : {}),
                        stores: {
                            areastore: {
                                autoLoad: true,
                                proxy: {
                                    type: 'ajax',
                                    url: '");
#nullable restore
#line 297 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
                                     Write(Url.Action("GetProfActivityArea"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                                    reader: {
                                        type: 'json',
                                        rootProperty: 'data'
                                    }
                                }
                            },
                            profactkindstore: {}
                        }
                    },
                    items: {
                        xtype: 'form',
                        layout: { type: 'vbox', align: 'stretch' },
                        defaults: {
                             labelWidth: 200,
                             width: 700
                        },
                        items: [
                            {
                                xtype: 'combobox',
                                fieldLabel: 'Область профессиональной деятельности',
                                name: 'ProfActivityAreaCode',
                                readOnly: !!record,
                                readOn");
                WriteLiteral(@"lyCls:'custom-combo',
                                bind: {
                                    store: '{areastore}',
                                    value: '{ProfAreaCode}'
                                },
                                allowBlank: false,
                                queryMode: 'local',
                                displayField: 'ProfAreaTitle',
                                valueField: 'ProfArea',
                                forceSelection: true,
                                listeners: {
                                    select: function (combo, value, eOpts) {
                                        var profstore = combo.up('form').up('window').viewModel
                                            .getStore('profactkindstore');
                                        if (profstore && proxy && !record) {
                                            proxy.extraParams = {
                                                areaCode: value.data.ProfArea,
 ");
                WriteLiteral(@"                                               newprofactivitykinds: true
                                            };
                                            profstore.setProxy(proxy).load({
                                                callback : function() {
                                                    if (this.getCount() == 0)
                                                        this.insert(0,
                                                            {
                                                                ""ProfActivityKindCode"":null,
                                                                ""Title"":
                                                                    ""Ничего не найдено""
                                                            });
                                                }
                                            });
                                        }
                                        var codefield = combo.up('form'");
                WriteLiteral(@").query('[name=Code]')[0];
                                        codefield.setValue(codefield.originalValue);
                                    },
                                    dirtychange: function (t, isDirty, eOpts) {
                                        try {
                                            var profstore = t.up('form').up('window').viewModel
                                                .getStore('profactkindstore');
                                            if (profstore && proxy && record) {
                                                proxy.extraParams = {
                                                        areaCode:t.value,
                                                        newprofactivitykinds:false
                                                    };
                                                profstore.setProxy(proxy).load();
                                            }
                                        }
                         ");
                WriteLiteral(@"               catch{
                                            t.up('form').up('window').viewModel.getStore('profactkindstore').setData([]);
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'combobox',
                                fieldLabel: 'Вид профессиональной деятельности',
                                name: 'ProfActivityKindCode',
                                displayField: 'Title',
                                valueField: 'ProfActivityKindCode',
                                queryMode: 'local',
                                forceSelection: true,
                                readOnly: !!record,
                                readOnlyCls: 'custom-combo',
                                allowBlank: false,
                                bind: {
                                        store: '{profactkind");
                WriteLiteral(@"store}',
                                        value: '{ProfActivityKindCode}'
                                    },
                                validator: function () {
                                    if (Ext.isEmpty(this.getRawValue()) || this.getValue() == null) {
                                        return false;
                                    }
                                    else
                                        return true;
                                },
                                listeners: {
                                    select: function (combo, value, eOpts) {
                                            var codefield = combo.up('form').query('[name=Code]')[0];
                                            codefield.setValue(value.data.ProfActivityKindCode);
                                    }
                                },

                            },
                            {
                                xtype: 'displayfi");
                WriteLiteral(@"eld',
                                fieldLabel: 'Код профессионального стандарта',
                                name: 'Code',
                                bind: '{Code}',
                                fieldCls:'displayfield',
                                value: '<i>Значение совпадает с кодом вида профессиональной деятельности</i>'
                            },
                            {
                                xtype: 'textarea',
                                fieldLabel: 'Наименование профессионального стандарта',
                                name: 'Title',
                                maxLength: 160,
                                minLength: 1,
                                bind: '{Title}'
                            }
                        ],
                          buttons: [
                                {
                                    text: ""Сохранить"",
                                    formBind: true,
                                  ");
                WriteLiteral(@"  handler: function() {
                                        var window = this.up('window');
                                        var form = window.down('form');

                                        if (!form.isValid()) {
                                            Ext.Msg.alert('Ошибка', 'Заполнены не все поля');
                                            return;
                                        }
                                        form.submit({
                                            url: record ? '");
#nullable restore
#line 432 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
                                                      Write(Url.Action("Edit"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\' : \'");
#nullable restore
#line 432 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProfStandards\Index.cshtml"
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

              });
        }

        var items = [
            gridPanel
        ];

        Urfu.createViewport('border', items);

  });

    </script>
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