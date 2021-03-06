#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2d5a60e52fb73d2e3d917b774b2a5c6fb78abd3e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ForeignLanguage_Properties), @"mvc.1.0.view", @"/Views/ForeignLanguage/Properties.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2d5a60e52fb73d2e3d917b774b2a5c6fb78abd3e", @"/Views/ForeignLanguage/Properties.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ForeignLanguage_Properties : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.ForeignLanguageCompetitionGroup>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
  
    ViewBag.Title = Model.ToString();

    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<script type=\"text/javascript\">\r\n    var trainingPlaceCounter = 0;\r\n    var teachersCounter = 0;\r\n    \r\n    Ext.onReady(function () {\r\n\r\n        Ext.tip.QuickTipManager.init();\r\n        function gettoken() {\r\n            var token = \'");
#nullable restore
#line 16 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                    Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral(@"';
            token = $(token).val();
            return token;
        }


        var store = Ext.create(""Ext.data.Store"",
        {
            idProperty: 'Id',
            autoLoad: true,
            //pageSize: 300,
            remoteSort: true,
            remoteFilter: true,
            proxy: {
                type: 'ajax',
                url: '");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                 Write(Url.Action("Properties",new {competitionGroupId = Model.Id}));

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
        teachersPanel = {}; trainingPlacePanel = {};
        var teacherStore = Ext.create(""Ext.data.BufferedStore"",
        {
            idProperty: 'Id',
            autoLoad: false,
            pageSize: 300,
            remoteSort: true,
            remoteFilter: true,
            proxy: {
                type: 'ajax',
                url: '");
#nullable restore
#line 48 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                 Write(Url.Action("PropertyTeachers"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                reader: {
                    type: 'json',
                    rootProperty: 'data',
                    totalProperty: 'total'
                }
            },
            listeners: {
                'load': function () {

                    if (teachersCounter >= 1 && teachersPanel.isMasked()) teachersPanel.unmask();
                    else {
                        teachersCounter++;
                    }

                }
            }
        });
        var selectedTeacherStore = Ext.create(""Ext.data.Store"",
        {
            idProperty: 'Id',
            autoLoad: false,
            pageSize: 300,
            remoteSort: true,
            remoteFilter: true,
            proxy: {
                type: 'ajax',
                url: '");
#nullable restore
#line 75 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                 Write(Url.Action("PropertyTeachers"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                reader: {
                    type: 'json',
                    rootProperty: 'data',
                    totalProperty: 'total'
                }
            },
            filters: { selected: true },
            listeners: {
                'load': function () {

                    if (teachersCounter >= 1 && teachersPanel.isMasked()) teachersPanel.unmask();
                    else {
                        teachersCounter++;
                    }

                }
            }
        });
     
        var gridPanel = Ext.create('Ext.grid.Panel',
        {
            region: 'center',
            store: store,
            loadMask: true,
            columnLines: true,
           /* tbar: [
                {}
            ]*/
            plugins: [
                    Ext.create('Ext.grid.plugin.CellEditing', {
                        clicksToEdit: 1,
                        listeners: {
                            'beforeedit': function (e, editor) {");
            WriteLiteral(@"

                            }
                        }
                    })],
            columns: [
                { xtype: 'rownumberer', width: 50 },
                {
                    header: 'Наименование',
                    dataIndex: 'ForeignLanguageName',
                    width: 300,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Лимит',
                    dataIndex: 'Limit',
                    width: 150,
                    renderer: Urfu.renders.htmlEncodeWithToolTip,
                    editor: {
                        xtype: 'numberfield',
                        hideTrigger: true,
                        allowBlank: false,
                        minValue: 0
                    }
                },
                {
                    header: 'Преподаватели',
                    dataIndex: 'Teachers',
                    width: 300,
                    renderer: Urfu");
            WriteLiteral(@".renders.htmlEncodeWithToolTip
                },
                {
                    xtype: 'actioncolumn',

                    region: 'center',
                    width: 200,
                    defaults: {
                        metadata: {
                            attr: 'style=""padding-left:10px;width:auto;""'
                        }
                    },
                    items: [
                    {
                        icon: '");
#nullable restore
#line 151 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                          Write(Url.Content("~/Content/Images/teacher.png"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',  // Use a URL in the icon config
                        iconCls: 'icon-padding',
                        text: 'Назначение преподавателей',
                        tooltip: 'Назначение преподавателей',
                        handler: function (grid, rowIndex, colIndex) {
                            var rec = grid.getStore().getAt(rowIndex);
                            teachersPanel.getComponent('Id').setValue(rec.get('Id'));
                            teacherStore.proxy.setUrl('");
#nullable restore
#line 158 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                                                  Write(Url.Action("PropertyTeachers"));

#line default
#line hidden
#nullable disable
            WriteLiteral("?propertyId=\' + rec.get(\'Id\'));\r\n                            teacherStore.load();\r\n\r\n                            selectedTeacherStore.proxy.setUrl(\'");
#nullable restore
#line 161 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                                                          Write(Url.Action("PropertyTeachers"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?propertyId=' + rec.get('Id'));
                            selectedTeacherStore.filter({ property: 'selected', value: true });
                            selectedTeacherStore.load();
                            teachersCounter = 0;
                            teachersPanel.show();
                            teachersPanel.mask('Загрузка');


                        }
                    }, 

                    ]
                }
            ]

        });
        gridPanel.on('edit',
                function (editor, eValue) {
                    var record = eValue.record;
                    Ext.Ajax.request({
                        url: '");
#nullable restore
#line 181 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                         Write(Url.Action("SetForeignLanguagePropertyLimit"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                        params: {
                            propertyId: record.data.Id,
                            limit: eValue.value,
                            __RequestVerificationToken: gettoken()
                        },
                        success: function (response) {
                            record.commit();
                            //gridPanel.getView().refresh();
                        },
                        error: function (response) {
                            record.reject();
                        },
                        failure: function (response) {
                            record.reject();
                        }
                    });

                    // commit the changes right after editing finished
                    //e.grid.store.save();

                });

        var keyUpEvent = function (textField) {
            var store = this.up('tablepanel').store;
       
            store.filter({
                property:");
            WriteLiteral(@" textField.datapropName,
                value: this.value,
            });
            store.reload();
            
        };

        teachersPanel = Ext.create('Ext.window.Window',
            {
                title: ""Преподаватели"",
                closeAction: 'hide',
                resizable: false,
                autoHeight: false,
                bodyPadding: 6,
                height: 600,
                defaults: {
                    xtype: 'textfield',
                    width: 800,

                },
                items: [
                      {
                          xtype: 'hidden',
                          itemId: 'Id',
                      },
                    {
                        xtype: 'grid',
                        id: 'teachersGrid',
                        store: teacherStore,
                        loadMask: true,
                        columnLines: true,
                        height: 300,
                        listeners: {
 ");
            WriteLiteral(@"                       },
                        columns: [
                            { xtype: 'checkcolumn', text: '', dataIndex: 'selected', width: 50, sortable: false },                            
                            { xtype: 'rownumberer', width: 60 },
                            {
                                header: 'Фамилия',
                                dataIndex: 'lastName',
                                width: 150,
                                renderer: Urfu.renders.htmlEncodeWithToolTip,
                                items: {
                                    xtype: 'textfield',
                                    flex: 1,
                                    datapropName: 'lastName',
                                    margin: 2,
                                    enableKeyEvents: true,
                                    listeners: {
                                        keyup: keyUpEvent,
                                        buffer: 500
         ");
            WriteLiteral(@"                           }
                                }
                            },
                            {
                                header: 'Имя',
                                dataIndex: 'firstName',
                                width: 150,
                                renderer: Urfu.renders.htmlEncodeWithToolTip,
                                items: {
                                    xtype: 'textfield',
                                    flex: 1,
                                    datapropName: 'firstName',
                                    margin: 2,
                                    enableKeyEvents: true,
                                    listeners: {
                                        keyup: keyUpEvent,
                                        buffer: 500
                                    }
                                }

                            },
                            {
                                header: 'Отчеств");
            WriteLiteral(@"о',
                                dataIndex: 'middleName',
                                width: 150,
                                renderer: Urfu.renders.htmlEncodeWithToolTip,
                                items: {
                                    xtype: 'textfield',
                                    flex: 1,
                                    datapropName: 'middleName',
                                    margin: 2,
                                    enableKeyEvents: true,
                                    listeners: {
                                        keyup: keyUpEvent,
                                        buffer: 500
                                    }
                                }
                            },
                            {
                                header: 'Место работы',
                                dataIndex: 'workPlace',
                                width: 200,
                                renderer: Urfu.renders.htmlE");
            WriteLiteral(@"ncodeWithToolTip,
                                items: {
                                    xtype: 'textfield',
                                    flex: 1,
                                    datapropName: 'workPlace',
                                    margin: 2,
                                    enableKeyEvents: true,
                                    listeners: {
                                        keyup: keyUpEvent,
                                        buffer: 500
                                    }
                                }
                            },
                        ]

                    },
                    {
                        xtype: 'grid',
                        id: 'selectedTeachersGrid',
                        store: selectedTeacherStore,
                        title: 'Выбранные преподаватели',
                        loadMask: true,
                        columnLines: true,
                        dataBuffered: true,
       ");
            WriteLiteral(@"                 height: 200,
                        listeners: {
                        },
                        columns: [
                            { xtype: 'checkcolumn', text: '', dataIndex: 'selected', width: 50, sortable: false },
                            { xtype: 'rownumberer', width: 60 },
                            {
                                header: 'Фамилия',
                                dataIndex: 'lastName',
                                width: 150,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                header: 'Имя',
                                dataIndex: 'firstName',
                                width: 150,
                                renderer: Urfu.renders.htmlEncodeWithToolTip

                            },
                            {
                                header: 'Отчество',
                                dataIn");
            WriteLiteral(@"dex: 'middleName',
                                width: 150,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                header: 'Место работы',
                                dataIndex: 'workPlace',
                                width: 200,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                        ]

                    }
                ],
                buttons: [
                    {
                        text: ""Сохранить"",
                        handler: function () {
                            teachersPanel.mask('Сохранение..');
                            var teachers = new Array();
                            selectedTeachersGrid.store.data.items.forEach(function (item, i, arr) {

                                teachers[i] = {
                                    id: item.data.teacherId,");
            WriteLiteral(@"
                                    selected: item.data.selected
                                };
                            });
                            Ext.Ajax.request({
                                method: 'POST',
                                dataType: 'json',
                                url: '");
#nullable restore
#line 376 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguage\Properties.cshtml"
                                 Write(Url.Action("UpdateForeignLanguagePropertyTeachers"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?propertyId=' + teachersPanel.getComponent(""Id"").getValue(),
                                params: { teacherRows: JSON.stringify(teachers) },
                                traditional: true,
                                success: function (response) {
                                    teachersPanel.unmask();
                                    teachersPanel.hide();
                                    store.reload();
                                },
                                failure: function (response) {
                                    teachersPanel.unmask();
                                    teachersPanel.hide();
                                }
                            });
                           
                        }
                    },
                    {
                        text: ""Отмена"",
                        handler: function () { teachersPanel.hide(); }
                    }],
                listeners: {
                    'shown': f");
            WriteLiteral(@"unction () {

                    }
                }
            });
        
        teachersGrid = Ext.getCmp('teachersGrid');
        selectedTeachersGrid = Ext.getCmp('selectedTeachersGrid');
        var selectableCellClick = function (grid, td, columnIndex, record, tr, rowIndex, e, eOpts,selectedGrid) {
            //remember to change - it must be column number (first has 0)
            if (columnIndex == 0) {

                if (record.data.selected) {
                    $(td).children('div').children('div').addClass('x-grid-checkcolumn-checked');
                    var clone = record.clone();
                    clone.commit();
                    if (grid.store.$className !== 'Ext.data.BufferedStore') {
                        record.commit();
                    }
                    
                    selectedGrid.store.add(clone); // добавляем в выбранных
                    //record.commit();

                }
                else {
                    if (grid.sto");
            WriteLiteral(@"re.$className !== 'Ext.data.BufferedStore') record.reject();

                }
            }
        };
        var selectedCellClick = function (grid, td, columnIndex, record, tr, rowIndex, e, eOpts) {
            //remember to change - it must be column number (first has 0)
            if (columnIndex == 0) {
                if (record.data.selected) {
                    $(td).children('div').children('div').addClass('x-grid-checkcolumn-checked');
                }
                else {
                    $(td).children('div').children('div').removeClass('x-grid-checkcolumn-checked');
                }
            }
        };

        teachersGrid.on('cellclick', function (grid, td, columnIndex, record, tr, rowIndex, e, eOpts) {
            selectableCellClick(grid, td, columnIndex, record, tr, rowIndex, e, eOpts, selectedTeachersGrid);
        });
        
        selectedTeachersGrid.on('cellclick', selectedCellClick);

        var items = [
            gridPanel
        ];
");
            WriteLiteral("\r\n        Urfu.createViewport(\'border\', items);\r\n\r\n    });\r\n</script>\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.ForeignLanguageCompetitionGroup> Html { get; private set; }
    }
}
#pragma warning restore 1591
