#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MinorSubgroup\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "48c05ca6215737bc323e677c1508f3d113d54f5e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_MinorSubgroup_Index), @"mvc.1.0.view", @"/Views/MinorSubgroup/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"48c05ca6215737bc323e677c1508f3d113d54f5e", @"/Views/MinorSubgroup/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_MinorSubgroup_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MinorSubgroup\Index.cshtml"
  
    ViewBag.Title = "Подгруппы для майноров";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">
        Ext.onReady(function() {
            Ext.tip.QuickTipManager.init();
            Ext.Ajax.setTimeout(1200000);
            Ext.define('ComboBoxModel',
            {
                extend: 'Ext.data.Model',
                fields:
                [
                    { type: 'string', name: 'Id' },
                    { type: 'string', name: 'Name' }
                ]
            });
            var all = Ext.create('ComboBoxModel',
            {
                Name: 'Все'
            });
            var semestersStore = Ext.create('Ext.data.Store',
            {
                model: 'ComboBoxModel',
                proxy:
                {
                    type: 'ajax',
                    url: '/Minors/Semesters',
                    reader: { type: 'json', root: 'data' }
                },
                listeners: {
                    load: function() {
                        this.add(all);
                        this.co");
                WriteLiteral(@"mmitChanges();
                    }
                },
                remoteSort: false, //true for server sorting
                sorters: [
                    {
                        property: 'Id',
                        direction: 'ASC'
                    }
                ]
            });

            var yearStore = Ext.create('Ext.data.Store',
            {
                model: 'ComboBoxModel',
                proxy:
                {
                    type: 'ajax',
                    url: '/MinorSubgroup/MinorPeriodYears',
                    reader: { type: 'json', root: 'data' }
                },
                listeners: {
                    load: function() {
                        this.add(all);
                        this.commitChanges();
                    }
                },
                remoteSort: false, //true for server sorting
                sorters: [
                    {
                        property: 'Id',
                     ");
                WriteLiteral(@"   direction: 'ASC'
                    }
                ]
            });

            var minorStore = Ext.create('Ext.data.Store',
            {
                idProperty: 'Id',
                proxy:
                {
                    type: 'ajax',
                    url: '/MinorSubgroup/Modules',
                    reader: { type: 'json', root: 'data' }
                },
                listeners: {
                    load: function() {
                        this.add(all);
                        this.commitChanges();
                    }
                },
                remoteFilter: false,
                remoteSort: false, //true for server sorting
                sorters: [
                    {
                        property: 'Id',
                        direction: 'ASC'
                    }
                ]
            });
            var printWnd = null;

            function setMinorFilter() {
                minorStore.clearFilter();
          ");
                WriteLiteral(@"      var year = printWnd.getComponent(""year"").getValue();
                var semesterId = printWnd.getComponent(""semester"").getValue();

                if (year && year.length > 0)
                    minorStore.filter(""Year"", year);
                if (semesterId && semesterId.length > 0)
                    minorStore.filter(""SemesterId"", semesterId);
            }
            
            function updateTitle() {
");
                WriteLiteral("            }\r\n\r\n            var currentYear = ");
#nullable restore
#line 136 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MinorSubgroup\Index.cshtml"
                          Write(DateTime.Now.Month < 7? DateTime.Now.Year -1 : DateTime.Now.Year);

#line default
#line hidden
#nullable disable
                WriteLiteral(@";

            var prevSettings = {};
            try {
                var prevSettingString = JSON.parse(localStorage.getItem(""MinorSubgroupFilters"") || ""[]"");

                for (var i = 0; i < prevSettingString.length; i++) {
                    prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
                }

                if (prevSettings[""Year""] == null || prevSettings[""Year""] == """") {
                        prevSettings[""Year""] = currentYear;
                    }

            } catch (err) {

            }

            filtersWnd = null;
            var setFilters = function() {
                var settings = [
                    { property: 'Year', value: filtersWnd.getComponent(""Year"").getValue() },
                    { property: 'Semester', value: filtersWnd.getComponent(""Semester"").getValue() },
                    { property: 'ModuleTitle', value: filtersWnd.getComponent(""ModuleTitle"").getValue() },
                    { property: '");
                WriteLiteral(@"Name', value: filtersWnd.getComponent(""Name"").getValue() },
                    { property: 'subgroupType', value: filtersWnd.getComponent(""subgroupType"").getValue() },
                    { property: 'Limit', value: filtersWnd.getComponent(""Limit"").getValue() },
                    { property: 'count', value: filtersWnd.getComponent(""count"").getValue() }
                ];
                store.setFilters(settings);
                localStorage.setItem(""MinorSubgroupFilters"", JSON.stringify(settings));
                updateTitle();
            };

            filtersWnd = Ext.create('Ext.window.Window',
            {
                title: ""Фильтры"",
                closeAction: 'hide',
                resizable: false,
                autoHeight: true,
                bodyPadding: 6,
                defaults: {
                    xtype: 'textfield',
                    width: 500
                },
                items: [
                    { fieldLabel: ""Год"", itemId: ""Year"", valu");
                WriteLiteral(@"e: prevSettings[""Year""] },
                    { fieldLabel: ""Семестр"", itemId: ""Semester"", value: prevSettings[""Semester""] },
                    { fieldLabel: ""Модуль"", itemId: ""ModuleTitle"", value: prevSettings[""ModuleTitle""] },
                    { fieldLabel: ""Индекс"", itemId: ""Name"", value: prevSettings[""Name""] },
                    { fieldLabel: ""Тип подгруппы"", itemId: ""subgroupType"", value: prevSettings[""subgroupId""] },
                    { fieldLabel: ""Лимит"", itemId: ""Limit"", value: prevSettings[""Limit""] },
                    { fieldLabel: ""Студенты"", itemId: ""count"", value: prevSettings[""count""] }
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
                        handler: fu");
                WriteLiteral(@"nction() { filtersWnd.hide(); }
                    }
                ]
            });

            var store = Ext.create(""Ext.data.Store"",
                {
                    idProperty: 'Id',
                    fields: [""Id"", ""Year"", ""Semester"", ""ModuleTitle"", ""Name"", ""subgroupType"", ""Limit"", ""count""],
                    autoLoad: true,
                    remoteSort: true,
                    groupField: 'ModuleTitle',
                    remoteFilter: true,
                    filters: [
                        { property: 'Year', value: filtersWnd.getComponent(""Year"").getValue() },
                        { property: 'Semester', value: filtersWnd.getComponent(""Semester"").getValue() },
                        { property: 'ModuleTitle', value: filtersWnd.getComponent(""ModuleTitle"").getValue() },
                        { property: 'Name', value: filtersWnd.getComponent(""Name"").getValue() },
                        { property: 'subgroupType', value: filtersWnd.getComponent(""subgroupT");
                WriteLiteral(@"ype"").getValue() },
                        { property: 'Limit', value: filtersWnd.getComponent(""Limit"").getValue() },
                        { property: 'count', value: filtersWnd.getComponent(""count"").getValue() }
                    ],
                    proxy: {
                        type: 'ajax',
                        url: '/MinorSubgroup/Index',
                        reader: {
                            type: 'json',
                            rootProperty: 'data',
                            totalProperty: 'total',
                        },
                        timeout: 1200000
                    },
                });

            printWnd = Ext.create('Ext.window.Window',
            {
                title: ""Печать"",
                closeAction: 'hide',
                resizable: false,
                autoHeight: true,
                bodyPadding: 6,
                defaults: {
                    xtype: 'textfield',
                    width: 700
          ");
                WriteLiteral(@"      },
                items: [
                    {
                        fieldLabel: ""Год"",
                        itemId: ""year"",
                        editable: false,
                        xtype: ""combobox"",
                        store: yearStore,
                        valueField: 'Id',
                        displayField: 'Name',
                        queryMode: 'remote',
                        listeners: {
                            change: function (field, newValue, oldValue) {
                                all.data.Year = newValue;
                                setMinorFilter();
                            },
                            scope: this
                        }
                    },
                    {
                        fieldLabel: ""Семестр"",
                        itemId: ""semester"",
                        editable: false,
                        xtype: ""combobox"",
                        store: semestersStore,
                 ");
                WriteLiteral(@"       valueField: 'Id',
                        displayField: 'Name',
                        queryMode: 'remote',
                        listeners: {
                            change: function (field, newValue, oldValue) {
                                all.data.SemesterId = newValue;
                                setMinorFilter();
                            },
                            scope: this
                        }
                    },
                    {
                        fieldLabel: ""Майнор"",
                        itemId: ""minor"",
                        editable: false,
                        xtype: ""combobox"",
                        store: minorStore,
                        valueField: 'Id',
                        displayField: 'Name',
                        queryMode: 'remote',
                        listeners: {
                            change: function(field, newValue, oldValue) {
                            },
                            ");
                WriteLiteral(@"scope: this
                        },
                        tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=""."">',
                                '<div class=""x-boundlist-item"" style=""border-bottom:1px solid #f0f0f0;"">',
                                '<div>{Name}</div></div>',
                            '</tpl>'

                        ),

                        displayTpl: Ext.create('Ext.XTemplate',
                            '<tpl for=""."">',
                                '{Name}',
                            '</tpl>'
                          )

                    }
                ],
                buttons: [
                    {
                        text: ""Распечатать"",
                        handler: function () {

                            var filter = [
                                { property: ""Year"", value: printWnd.getComponent(""year"").getValue() },
                                { property: ""semesterId"", value: printWnd.getCom");
                WriteLiteral("ponent(\"semester\").getValue() },\r\n                                { property: \"minorId\", value: printWnd.getComponent(\"minor\").getValue() },\r\n                            ];\r\n                            var url = \'");
#nullable restore
#line 320 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MinorSubgroup\Index.cshtml"
                                  Write(Url.Action("MassPrint"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' +
                                ""?filter="" + JSON.stringify(filter);
                            window.location = url;
                            printWnd.hide();
                        }
                    },
                    {
                        text: ""Отмена"",
                        handler: function() { printWnd.hide(); }
                    }
                ]
                });


            var copyUrl = """";
            var copyMembershipsWnd = Ext.create('Ext.window.Window',
                {
                    title: ""Выбор параметра копирования"",
                    closeAction: 'hide',
                    resizable: false,
                    autoHeight: true,
                    bodyStyle: ""margin: 10px;"",
                    bodyPadding: 6,
                    defaults: {
                        width: 400,

                    },
                    items: [
                        {
                            labelwidth: 150,
                   ");
                WriteLiteral(@"         itemId: ""withTeacher"",
                            xtype: ""checkboxfield"",
                            boxLabel: ""С преподавателем""

                        },
                    ],
                    buttons: [
                        {
                            text: ""Копировать"",
                            handler: function () {
                                copyMembershipsWnd.mask('Копирование..');
                                var url = copyUrl +
                                    ""&withTeacher="" +
                                    (copyMembershipsWnd.getComponent(""withTeacher"").getValue() ? ""true"" : ""false"");
                                
                                Ext.Ajax.request({
                                    method: ""POST"",
                                    url: url,
                                    success: function (response) {
                                        copyMembershipsWnd.unmask();
                                        co");
                WriteLiteral(@"pyMembershipsWnd.hide();
                                        var data = Ext.decode(response.responseText);
                                        Ext.MessageBox.show({
                                            title: 'Информационное сообщение',
                                            msg: data.message,
                                            buttons: Ext.MessageBox.OK,
                                            fn: function (btn) {
                                                if (data.success) {
                                                    window.location = '/MinorSubgroup/Index?focus=' + data.focus;
                                                }
                                            }
                                        });                                        
                                    },
                                    failure: function (response) {
                                        copyMembershipsWnd.unmask();
                    ");
                WriteLiteral(@"                    copyMembershipsWnd.hide();
                                        Ext.MessageBox.alert('Ошибка', ""Неизвестная ошибка"");
                                    }
                                });
                            }
                        },
                        {
                            text: ""Отмена"",
                            handler: function () { copyMembershipsWnd.hide(); }
                        }
                    ]
                });

            //setFilters();
            updateTitle();
            var tpl = '<a href=""/MinorSubgroup/Students?id={Id}"">Студенты</a>\
                | <a href=""/MinorSubgroup/Edit?id={Id}"">Редактировать</a>\
                <tpl if=""data.MarksFrozen"">| <a href=""/MinorSubgroup/Unfreeze?id={Id}"">Открыть ведомость</a></tpl>\
                <tpl if=""!MarksFrozen"">| <a href=""/MinorSubgroup/Freeze?id={Id}"">Закрыть ведомость</a></tpl>';
            var grouping = Ext.create('Ext.grid.feature.Grouping',
          ");
                WriteLiteral(@"      { ftype: 'grouping', collapsible: true, startCollapsed: true });
            var gridPanel = Ext.create('Ext.grid.Panel',
            {
                region: 'center',
                store: store,
                loadMask: true,
                features: [grouping],
                tbar: [
                    {
                        xtype: 'button',
                        text: 'Фильтры...',
                        handler: function() { filtersWnd.show(); }
                    },
                    {
                        xtype: 'button',
                        text: ""Отменить фильтры"",
                        handler: function () {

                            filtersWnd.items.items.forEach(function (element, index, array) {
                                if (element.itemId == 'Year') {
                                    element.setValue(currentYear);
                                }
                                else {
                                    element.s");
                WriteLiteral(@"etValue("""");
                                }
                            });
                            
                            setFilters();
                            //localStorage.setItem(""MinorSubgroupFilters"", []);
                            //store.clearFilter();
                            //updateTitle();
                        }
                    },
                    {
                        xtype: 'button',
                        text: ""Количество подгрупп"",
                        handler: function() { window.location = ""/MinorSubgroupMeta/Index""; }
                    },
                    {
                        xtype: 'button',
                        text: ""Создать подгруппы"",
                        handler: function() { window.location = ""/MinorSubgroup/Create""; }
                    },
                    {
                        xtype: 'button',
                        text: ""Развернуть\\Свернуть группировку"",
                        handler: fun");
                WriteLiteral(@"ction() {
                            window.groupsCollapsed = !window.groupsCollapsed;
                            if (window.groupsCollapsed)
                                grouping.expandAll();
                            else
                                grouping.collapseAll();
                        }
                    },
                    {
                        xtype: 'button',
                        text: ""Распределить студентов"",
                        handler: function() { window.location = ""/MinorSubgroup/FillGroupsWithStudents""; }
                    },
                    {
                        xtype: 'button',
                        text: ""Удалить"",
                        handler: function() {
                            var objects = gridPanel.getSelectionModel().getSelection();
                            if (objects.length === 0)
                                return;
                            Ext.MessageBox.confirm('Удаление подгрупп',
              ");
                WriteLiteral(@"                  'Удалить подгруппы и зачисления студентов в них (' + objects.length + ' шт.) ?',
                                function(btn) {
                                    if (btn === 'yes') {
                                        var refresh = function() {
                                            window.location = ""/MinorSubgroup/Index"";
                                        };
                                        Ext.Ajax.request({
                                            url: '/MinorSubgroup/Delete',
                                            success: refresh,
                                            failure: refresh,
                                            params: { ids: objects.map(function(obj) { return obj.data.Id }) }
                                        });
                                    }
                                });

                        }
                    },
                    {
                        xtype: 'button',
      ");
                WriteLiteral(@"                  text: ""Копировать"",
                        handler: function() {
                            var objects = gridPanel.getSelectionModel().getSelection();
                            if (objects.length !== 2) {
                                Ext.MessageBox.alert(""Копирование зачислений студентов"", 'Выберите две подгруппы');
                                return;
                            }

                            var o0 = objects[0].data;
                            var o1 = objects[1].data;

                            if (o0.minorId !== o1.minorId) {
                                Ext.MessageBox.alert(""Копирование зачислений студентов"",
                                    'Копирование зачисление разрешено только внутри одного модуля');
                                return;
                            }

                            if ((!!o0.count) === (!!o1.count)) {
                                Ext.MessageBox.alert(""Копирование зачислений студентов"",
     ");
                WriteLiteral(@"                               'Одна из подгрупп должна быть пустой, в другую должны быть зачислены студенты');
                                return;
                            }


                            var src = null;
                            var dst = null;
                            if (o0.count === 0)
                                dst = o0;
                            if (o1.count === 0)
                                dst = o1;
                            if (o0.count !== 0)
                                src = o0;
                            if (o1.count !== 0)
                                src = o1;
                            if (src.subgroupType === dst.subgroupType && src.Semester === dst.Semester) {
                                Ext.MessageBox.alert(""Копирование зачислений студентов"",
                                    'Выберите другой вид нагрузки!');
                            } else {
                                copyUrl = ""/MinorSubgroup/SimpleCopyMem");
                WriteLiteral(@"bership?srcId="" + src.Id + ""&dstId="" + dst.Id;
                                copyMembershipsWnd.show();
                            }
                        }
                    },
                    '-',
                    {
                        xtype: 'button',
                        text: 'Открыть в Excel',
                        handler: function() {
                            var fileUrl = '");
#nullable restore
#line 539 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MinorSubgroup\Index.cshtml"
                                      Write(Url.Action("DownloadMinorSubGroups"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' +
                                ""?filter="" +
                                encodeURIComponent(localStorage.getItem(""MinorSubgroupFilters"") || ""[]"");
                            window.location.href = fileUrl;
                        }
                    },
                    '-',
                    {
                        xtype: 'button',
                        text: 'Печать',
                        handler: function() {
                            printWnd.show();
                        }
                    },
                ],
                columns: [
                    { xtype: 'rownumberer', width: 50 },
                    {
                        header: 'Год',
                        dataIndex: 'Year',
                        width: 100, /*
                        layout: 'anchor',
                        items: [
                        {
                            xtype: 'textfield',
                            anchor: '100%',
                           ");
                WriteLiteral(@" margin: '3px'
                        }],*/
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Семестр',
                        dataIndex: 'Semester',
                        width: 100,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Модуль',
                        dataIndex: 'ModuleTitle',
                        width: 300,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Индекс',
                        dataIndex: 'Name',
                        width: 300,
                        renderer: function(value, metaData, record, rowIdx, colIdx, store) {
                            metaData.tdAttr = 'data-qtip=""' + value + '""';
                            return value;
                        }
          ");
                WriteLiteral(@"          },
                    {
                        header: 'Нагрузка',
                        align: 'center',
                        dataIndex: 'subgroupType',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 180
                    },
                    {
                        header: 'Лимит',
                        align: 'center',
                        dataIndex: 'Limit',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 120
                    },
                    {
                        header: 'Студенты',
                        align: 'center',
                        dataIndex: 'count',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 120
                    },
                    {
                        header: 'Преподаватель',
                        align: 'center',
                        dataIndex: 'teacher',
          ");
                WriteLiteral(@"              renderer: Ext.util.Format.htmlEncode,
                        width: 150
                    },
                    {
                        //xtype: 'templatecolumn',
                        //tpl: tpl,
                        sortable: false,
                        width: 470,
                        dataIndex: 'MarksFrozen',
                        renderer: function (value, metaData, record) {
                            return '<a href=""/MinorSubgroup/Students?id=' + record.get(""Id"") +'"">Студенты</a>\
                                  | <a href=""/MinorSubgroup/Edit?id=' + record.get(""Id"") + '"">Редактировать</a>' +
                            (record.get(""HasScores"")
                                ? (record.get(""MarksFrozen"")
                                        ? ' | <a href=""/MinorSubgroup/Unfreeze?id=' +
                                        record.get(""Id"") +
                                        '"">Открыть ведомость</a>'
                                       ");
                WriteLiteral(@" : ' | <a href=""/MinorSubgroup/Freeze?id=' +
                                        record.get(""Id"") +
                                        '"">Закрыть ведомость</a>'
                                )
                                : '');
                        }
                    }
                ],
                selModel: {
                    selType: 'checkboxmodel' /*,
                    showHeaderCheckbox: true,
                    mode: 'SIMPLE'*/
                }
            });

            var items = [
                gridPanel
            ];

            Urfu.createViewport('border', items);

            gridPanel.getStore()
                .on('load',
                    function(store, records, options) {
                        var focus = '");
#nullable restore
#line 656 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MinorSubgroup\Index.cshtml"
                                Write(ViewBag.Focus);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"';
                        if (focus && focus.length > 0) {

                            var focusRow = store.findExact('Id', parseInt(focus));
                            if (focusRow >= 0) {
                                var rowData = store.getAt(focusRow);
                                var gv = gridPanel.getView();

                                var groupingFeature = gridPanel.getView().features[0];

                                groupingFeature.expand(rowData.data['ModuleTitle'], true);

                                gv.focusRow(rowData);
                                gridPanel.getSelectionModel().select(rowData);
                            }
                            return false;
                        }
                    });

            var alertString = '");
#nullable restore
#line 675 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MinorSubgroup\Index.cshtml"
                          Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
                WriteLiteral("\';\r\n            if (alertString.length > 0)\r\n                Ext.MessageBox.alert(\'Копирование зачислений студентов\', alertString);\r\n\r\n        });\r\n    </script>\r\n");
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