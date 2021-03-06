#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "62f74bfb6d66ec668117ae9a606681ba200f2f16"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_BasicCharacteristicOP_Versions), @"mvc.1.0.view", @"/Views/BasicCharacteristicOP/Versions.cshtml")]
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
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
using Urfu.Its.Common;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
using Urfu.Its.Web.DataContext;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
using Urfu.Its.Web.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"62f74bfb6d66ec668117ae9a606681ba200f2f16", @"/Views/BasicCharacteristicOP/Versions.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_BasicCharacteristicOP_Versions : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 4 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
  
    ViewBag.Title = "Список версий ОХОП";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

    var canViewCompetencePassport = User.IsInRole(ItsRoles.Admin);

#line default
#line hidden
#nullable disable
            WriteLiteral("<style>\r\n\r\n    .its-medium {\r\n        width: 24px;\r\n        height: 24px;\r\n        margin-left: 5px;\r\n    }\r\n</style>\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n<script type=\"text/javascript\">\r\n    var localStorageName = \"BasicCharacteristicOPVersionsListFilter\";\r\n    Ext.onReady(function() {\r\n        Ext.tip.QuickTipManager.init();\r\n        \r\n        var dataDivisions = Urfu.parseJson(\'");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                                       Write(ViewBag.Divisions);

#line default
#line hidden
#nullable disable
                WriteLiteral("\');\r\n        var divisionStore = Ext.create(\"Ext.data.Store\",\r\n            {\r\n                data: dataDivisions\r\n            });\r\n        var directionStore = Ext.create(\"Ext.data.Store\", {});\r\n        var dataDirections = Urfu.parseJson(\'");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                                        Write(ViewBag.Directions);

#line default
#line hidden
#nullable disable
                WriteLiteral("\');\r\n\r\n        var profileStore = Ext.create(\"Ext.data.Store\", {});\r\n\r\n        var dataStatuses = Urfu.parseJson(\'");
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                                      Write(ViewBag.Statuses);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"');
        var statusStore = Ext.create(""Ext.data.Store"",
            {
                data: dataStatuses
            });

        var store = Ext.create(""Ext.data.Store"",
            {
                autoLoad: false,
                //pageSize: 300,
                remoteSort: false,
                remoteFilter: true,
                proxy: {
                    type: 'ajax',
                    url: '");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                     Write(Url.Action("Versions"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                    reader: {
                        type: 'json',
                        rootProperty: 'data',
                        totalProperty: 'total'
                    }
                }
            });

        var prevSettings = {};
        try {
            var prevSettingString = JSON.parse(localStorage.getItem(localStorageName) || ""[]"");

            for (var i = 0; i < prevSettingString.length; i++) {
                prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
            }
        } catch (err) {
        }
        var filtersWnd = null;
        function loadStore() {
            store.proxy.setUrl(window.location.pathname +
                '?filter=' +
                encodeURIComponent(localStorage.getItem(localStorageName)));
            store.load();
        }

        var setFilters = function(id) {
            var settings = [
                { property: 'divisionId', value: filtersWnd.getComponent(""division"").getValu");
                WriteLiteral(@"e() },
                { property: 'directionId', value: filtersWnd.getComponent(""direction"").getValue() },
                { property: 'profileId', value: filtersWnd.getComponent(""profile"").getValue() },
                { property: 'statusId', value: filtersWnd.getComponent(""status"").getValue() },
                { property: 'id', value: id }
            ];
            localStorage.setItem(localStorageName, JSON.stringify(settings));
            loadStore();
        };

        var changeDivisionEvent = function (newValue) {
            filtersWnd.items.items.find(p => p.itemId == ""direction"").setValue('');
            directionStore.setData([]);

            filtersWnd.items.items.find(p => p.itemId == ""profile"").setValue('');
            profileStore.setData([]);

            if (newValue != '' && newValue != null) {
                try {
                    var currentDivisionData = divisionStore.data.items.filter(d => d.data.DivisionId == newValue)[0].data;
                    directi");
                WriteLiteral(@"onStore.setData(currentDivisionData.Directions);
                }
                catch{ }
            }
            else {
                directionStore.setData(dataDirections);
            }
        };

        var changeDirectionEvent = function (newValue) {
            filtersWnd.items.items.find(p => p.itemId == ""profile"").setValue('');
            try {
                var currentDirectionData = directionStore.data.items.filter(d => d.data.DirectionId == newValue)[0].data;
                profileStore.setData(currentDirectionData.Profiles);
            }
            catch{ }
        };

        filtersWnd = Ext.create('Ext.window.Window',
            {
                title: ""Фильтры"",
                closeAction: 'hide',
                resizable: false,
                autoHeight: false,
                bodyPadding: 6,
                defaults: {
                    xtype: 'textfield',
                    width: 500,
                    labelWidth: 130
                },");
                WriteLiteral(@"
                items: [
                    {
                        fieldLabel: ""Институт"",
                        itemId: ""division"",
                        value: prevSettings['divisionId'],
                        xtype: ""combobox"",
                        store: divisionStore,
                        valueField: 'DivisionId',
                        displayField: 'DivisionName',
                        queryMode: 'local',
                        anyMatch: true,
                        width: 500,
                        listeners: {
                            change: function (t, newValue, oldValue, eOpts) {
                                changeDivisionEvent(newValue);
                            }
                        }
                    },
                    {
                        fieldLabel: ""Направление"",
                        itemId: ""direction"",
                        value: prevSettings['directionId'],
                        xtype: ""combobox"",
          ");
                WriteLiteral(@"              store: directionStore,
                        valueField: 'DirectionId',
                        displayField: 'DirectionName',
                        queryMode: 'local',
                        anyMatch: true,
                        width: 500,
                        listeners: {
                            change: function (t, newValue, oldValue, eOpts) {
                                changeDirectionEvent(newValue);
                            }
                        }
                    },
                    {
                        fieldLabel: ""Образовательная программа"",
                        itemId: ""profile"",
                        value: prevSettings['profileId'],
                        xtype: ""combobox"",
                        store: profileStore,
                        valueField: 'ProfileId',
                        displayField: 'ProfileName',
                        queryMode: 'local',
                        anyMatch: true,
                   ");
                WriteLiteral(@"     width: 500
                    },
                    {
                        fieldLabel: ""Статус"",
                        itemId: ""status"",
                        value: prevSettings['statusId'],
                        xtype: ""combobox"",
                        store: statusStore,
                        valueField: 'Id',
                        displayField: 'Name',
                        queryMode: 'local',
                        anyMatch: true,
                        width: 300
                    },
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
            });");
                WriteLiteral(@"

        changeDivisionEvent(prevSettings['divisionId']);
        filtersWnd.getComponent(""division"").setValue(prevSettings['divisionId']);

        changeDirectionEvent(prevSettings['directionId']);
        filtersWnd.getComponent(""direction"").setValue(prevSettings['directionId']);

        filtersWnd.getComponent(""profile"").setValue(prevSettings['profileId']);

        setFilters(prevSettings['id']);


        var updateStatusWnd = function (record) {
            return Ext.create('Ext.window.Window',
                {
                    title: ""Редактирование статуса ОХОП"",
                    closeAction: 'hide',
                    overflowY: 'auto',
                    resizable: true,
                    maxHeight: 500,
                    autoHeight: true,
                    maxWidth: 400,
                    modal: true,
                    bodyPadding: 6,
                    layout: { type: 'vbox', align: 'stretch' },
                    items: [
                        ");
                WriteLiteral(@"{
                            xtype: 'hidden',
                            hidden: true,
                            value: record.data.versionedDocumentId,
                            itemId: ""Id""
                        },
                        {
                            xtype: 'label',
                            text: `${record.data.profile} ${record.data.profileTitle}`,
                            style: 'font-weight: normal;'
                        },
                        {
                            xtype: 'label',
                            text: `Версия ${record.data.version}`,
                            style: 'font-weight: normal;'
                            //style: 'font-weight: bold;'
                        },
                        {
                            fieldLabel: ""Статус"",
                            itemId: ""StatusId"",
                            //margin: '10 0 0 0',
                            value: record.data.statusId,
                       ");
                WriteLiteral(@"     xtype: ""combobox"",
                            store: statusStore,
                            valueField: 'Id',
                            displayField: 'Name',
                            queryMode: 'local',
                            anyMatch: true,
                            width: 300,
                            labelWidth: 50
                        },
                        {
                            xtype: 'label',
                            text: `Комментарий`,
                            style: 'font-weight: normal;'
                            //style: 'font-weight: bold;'
                        },
                        {
                            xtype: 'textarea',
                            fieldLabel: '',
                            itemId: 'Comment',
                            value: record.data.comment,
                            width: 400,
                            height: 100
                        }
                    ],
                   ");
                WriteLiteral(@" buttons: [
                        {
                            text: ""Сохранить"",
                            handler: function (btn) {

                                var statusId = btn.up().up().items.items.find(c => c.itemId == ""StatusId"").getValue();
                                var id = btn.up().up().items.items.find(c => c.itemId == ""Id"").getValue();
                                var comment = btn.up().up().items.items.find(c => c.itemId == ""Comment"").getValue();

                                Ext.getBody().mask('Сохранение статуса...');
                                Ext.Ajax.request({
                                    url: '/BasicCharacteristicOP/UpdateVersionStatus',
                                    params: {
                                        id: id,
                                        status: statusId,
                                        comment: comment
                                    },
                                    timeout: 120000,
      ");
                WriteLiteral(@"                              success: function (response) {
                                        var data = Ext.decode(response.responseText);
                                        if (!data.success) {
                                            Ext.MessageBox.show({
                                                title: 'Информационное сообщение',
                                                msg: data.message,
                                                buttons: Ext.MessageBox.OK,
                                                fn: function (btn) {
                                                }
                                            });
                                        }
                                        else
                                            store.reload();
                                        Ext.getBody().unmask();
                                    },
                                    failure: function (d) {
                               ");
                WriteLiteral(@"         Ext.MessageBox.alert('Ошибка', ""Неизвестная ошибка"");
                                        Ext.getBody().unmask();
                                    }
                                });

                                btn.up('window').close();
                            }
                        }
                    ]
                });
        }

        var id = 'versionsOHOPGrid';

        var gridPanel = Ext.create('Ext.grid.Panel', {
            region: 'center',
            store: store,
            id: id,
            tbar: [
                {
                    xtype: 'button',
                    text: 'Фильтры...',
                    handler: function() { filtersWnd.show(); }
                },
                {
                    xtype: 'button',
                    text: ""Отменить фильтры"",
                    handler: function() {
                        localStorage.setItem(localStorageName, ""[]"");
                        loadStore();
         ");
                WriteLiteral("           }\r\n                },\r\n                \'-\',\r\n                {\r\n                    xtype: \'button\',\r\n                    text: \'Отчет в Excel\',\r\n                    handler: function() {\r\n                        var fileUrl = \'");
#nullable restore
#line 344 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                                  Write(Url.Action("DownloadReport"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' +
                            ""?filter="" +
                            encodeURIComponent(localStorage.getItem(""BasicCharacteristicOPVersionsListFilter"") || ""[]"");
                        window.location.href = fileUrl;
                    }
                }
            ],
            loadMask: true,
            columnLines: true,
            viewConfig: {
                markDirty: false
            },
            columns: [
                { xtype: 'rownumberer', width: 50 },
                {
                    header: 'Версия',
                    align: 'center',
                    dataIndex: 'version',
                    width: 110,
                    renderer: Ext.util.Format.htmlEncode
                },
                {
                    header: 'Направление',
                    dataIndex: 'directionOkso',
                    width: 200,
                    renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                        valu");
                WriteLiteral(@"e = `${record.data.directionOkso} - ${record.data.directionTitle}`;
                        metaData.tdAttr = 'data-qtip=""' + Ext.String.htmlEncode(value) + '""';
                        return value;
                    }
                },
                {
                    header: 'Стандарт',
                    dataIndex: 'standard',
                    renderer: Ext.util.Format.htmlEncode,
                    width: 130
                },
                {
                    header: 'Образовательная программа',
                    dataIndex: 'profile',
                    width: 250,
                    renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                        value = `${record.data.profile} - ${record.data.profileTitle}`;
                        metaData.tdAttr = 'data-qtip=""' + Ext.String.htmlEncode(value) + '""';
                        return value;
                    }
                },
                {
                    head");
                WriteLiteral(@"er: 'Уровень обучения',
                    align: 'center',
                    dataIndex: 'qualification',
                    width: 150
                },
                {
                    header: 'Подразделение',
                    dataIndex: 'chairTitle',
                    cellWrap: true,
                    renderer: Ext.util.Format.htmlEncode,
                    width: 180
                },
                {
                    header: 'Год',
                    dataIndex: 'year',
                    renderer: Ext.util.Format.htmlEncode,
                    width: 80
                },
                {
                    header: 'Статус',
                    align: 'center',
                    dataIndex: 'status',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
                },
                {
                    header: 'Дата изменения статуса',
                    align: 'center',
                    dataIndex: 'sta");
                WriteLiteral(@"tusChangeTime',
                    width: 200,
                    //renderer: Ext.util.Format.dateRenderer('d.m.Y H:i')
                },
                {
                    xtype: 'actioncolumn',
                    region: 'center',
                    width: 80,
                    items: [
                        {
                            icon: '");
#nullable restore
#line 430 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                              Write(Url.Content("~/Content/Images/comment2.png"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                            iconCls: 'icon-toppadding',
                            tooltip: 'Комменатрий',
                            handler: function (grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                Ext.Msg.show({
                                    title: 'Комментарий',
                                    message: rec.data.comment,
                                    width: 500,
                                    height: 300
                                })
                                grid.getSelectionModel().select(rec);
                            }
                        },
                        {
                            icon: '");
#nullable restore
#line 445 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                              Write(Url.Content("~/Content/Images/send.png?30"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                            iconCls: 'icon-padding-mediumicon',
                            tooltip: 'Отправить на согласование',
                            handler: function (grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                if (rec.data.statusId === 10 || rec.data.statusId === 11) {
                                    Ext.MessageBox.show({
                                        title: 'Информационное сообщение',
                                        msg: ""Документ уже находится в обработке или подписан"",
                                        buttons: Ext.MessageBox.OK
                                    });

                                    return;
                                }

                                Ext.getBody().mask('Отправка...');
                                Ext.Ajax.request({
                                    url: '/BasicCharacteristicOP/SendVersion',
                  ");
                WriteLiteral(@"                  params: {
                                        id: rec.get('versionedDocumentId')
                                    },
                                    timeout: 120000,
                                    success: function (response) {
                                        var d = Ext.JSON.decode(response.responseText);

                                        if (!d.success) {
                                            Ext.MessageBox.show({
                                                title: 'Ошибка',
                                                msg: d.message,
                                                buttons: Ext.MessageBox.OK
                                            });
                                        } else {
                                            try {
                                                grid.getStore().getAt(rowIndex).data.status = d.status;
                                                grid.getStore().getAt(rowIndex");
                WriteLiteral(@").data.statusId = d.statusId;
                                                grid.getStore().getAt(rowIndex).data.statusChangeTime = d.statusDate;

                                                Ext.getCmp(id).getView().scrollRowIntoView(rowIndex)

                                                Ext.getCmp(id).getView().focusRow(grid.getStore().getAt(rowIndex));
                                                Ext.getCmp(id).getSelectionModel().select(grid.getStore().getAt(rowIndex));

                                                Ext.getCmp(id).getView().refresh();


                                                Ext.MessageBox.show({
                                                    title: 'Уведомление',
                                                    msg: 'Документ отправлен на согласование',
                                                    buttons: Ext.MessageBox.OK
                                                });
                                            }
             ");
                WriteLiteral(@"                               catch{

                                            }
                                        }

                                        Ext.getBody().unmask();
                                    },
                                    failure: function (d) {
                                        Ext.MessageBox.show({
                                            title: 'Ошибка',
                                            msg: 'Неизвестная ошибка',
                                            buttons: Ext.MessageBox.OK
                                        });
                                        Ext.getBody().unmask();
                                    }
                                });
                            }
                        },
                    ]
                },
                {
                    xtype: 'templatecolumn',
                    sortable: false,
                    tpl: new Ext.XTemplate(""<a href='/Document/{v");
                WriteLiteral(@"ersionedDocumentId}'>ОХОП</a>""),
                    width: 80
                },
                //{
                //    sortable: false,
                //    hideable: false,
                //    menuDisabled: true,
                //    width: 230,
                //    renderer: function (v, m, r) {
                //        var id = Ext.id();
                //        var sedOp = r.get(""SedOp"");
                //        var text = sedOp ? 'Отправить в УПОП' : 'Получить статус СЭД';

                //        Ext.defer(function () {
                //            Ext.widget('button', {
                //                renderTo: id,
                //                text: text,
                //                width: 210,
                //                handler: function () {
                //                    var wpId = r.get(""VersionedDocumentId"");
                //                    if (sedOp)
                //                        sendToSed(wpId, r);
              ");
                WriteLiteral(@"  //                    else
                //                        getStatus(wpId, r);
                //                }
                //            });
                //        }, 150);

                //        return Ext.String.format('<div id=""{0}""></div>', id);
                //    }
                //},
                {
                    xtype: 'actioncolumn',

                    region: 'center',
                    width: 170,
                    defaults: {
                        metadata: {
                            attr: 'style=""padding-left:10px;width:auto;""'
                        }
                    },
                    items: [
                        {
                            icon: '");
#nullable restore
#line 562 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                              Write(Url.Content("~/Content/Images/edit.png"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                            iconCls: \'");
#nullable restore
#line 563 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                                 Write(ViewBag.CanApprove);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' == 'True' ? 'icon-padding' : '{ visibility: hidden }',
                            tooltip: 'Изменить статус',
                            handler: function(grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                var wnd = updateStatusWnd(rec);
                                wnd.show();
                            }
                        },
                        {
                            icon: '");
#nullable restore
#line 572 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                              Write(Url.Content("~/Content/Images/doc1.png?22"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                            iconCls: 'icon-padding',
                            text: 'Скачать docx',
                            tooltip: 'Скачать docx',
                            handler: function(grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                window.location = '/Document/' + rec.get('versionedDocumentId') + '/Print?format=docx';
                            }
                        },
                        {
                            icon: '");
#nullable restore
#line 582 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                              Write(Url.Content("~/Content/Images/zip.png"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                            iconCls: 'icon-padding-bigicon',
                            text: 'Скачать zip',
                            tooltip: 'Скачать zip',
                            handler: function(grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                window.location = '/Document/' + rec.get('versionedDocumentId') + '/Archive';
                            }
                        },
                        {
                            icon: '");
#nullable restore
#line 592 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                              Write(Url.Content("~/Content/Images/remove2.png"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                            iconCls: \'");
#nullable restore
#line 593 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                                 Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' == 'True' ? 'icon-padding-squareicon' : '{ visibility: hidden }',
                            text: 'Удалить',
                            tooltip: 'Удалить',
                            handler: function(grid, rowIndex, colIndex) {

                                var rec = grid.getStore().getAt(rowIndex);
                                var maskEl = grid.getEl();
                                function request(confirmed) {
                                    maskEl.mask('Выполнение операции...');

                                    Ext.Ajax.request({
                                        url: '/BasicCharacteristicOP/RemoveModuleWorkingProgram',
                                        params: {
                                            id: rec.get('versionedDocumentId')
                                        },
                                        success: function (response) {
                                            store.reload();
                                           ");
                WriteLiteral(@" maskEl.unmask();
                                        },
                                        failure: function (d) {
                                            maskEl.unmask();

                                            console.error(d.responseText);
                                            alert(d.responseText);
                                        }
                                    });
                                }

                                Ext.MessageBox.show({
                                    title: 'Информационное сообщение',
                                    msg: ""Вы действительно хотите удалить версию ОХОП?"",
                                    buttons: Ext.MessageBox.YESNO,
                                    fn: function (btn) {
                                        if (btn === 'yes') {
                                            request();
                                        }
                                    }
                        ");
                WriteLiteral("        });\r\n                            }\r\n                        },\r\n                        {\r\n                            icon: \'");
#nullable restore
#line 634 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                              Write(Url.Content("~/Content/Images/arrow.png"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                            iconCls: \'");
#nullable restore
#line 635 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                                 Write(canViewCompetencePassport);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' == 'True' ? 'icon-padding-mediumicon' : '{ visibility: hidden }',
                            tooltip: 'Паспорт компетенций',
                            handler: function(grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                var settings = [
                                    { property: 'divisionId', value: '' },
                                    { property: 'directionId', value: '' },
                                    { property: 'profileId', value: '' },
                                    { property: 'statusId', value: '' },
                                    { property: 'ohopId', value: rec.get('versionedDocumentId')}
                                ];
                                localStorage.setItem(""CompetencePassportVersionsListFilter"", JSON.stringify(settings));
                                var link = """);
#nullable restore
#line 647 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\BasicCharacteristicOP\Versions.cshtml"
                                       Write(Url.Action("Index", "CompetencePassport"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""";
                                window.open(link, '_blank');
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
