#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e0349b06c389a9794f2bf1a8e03e8d906b8948f0"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_PracticeGroup_Group), @"mvc.1.0.view", @"/Views/PracticeGroup/Group.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e0349b06c389a9794f2bf1a8e03e8d906b8948f0", @"/Views/PracticeGroup/Group.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_PracticeGroup_Group : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Model.Models.Practice.GroupViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
  
    ViewBag.Title = "Группа " + Model.GroupName + ", " + Model.Title.Title;
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<style>
    .x-progress-default .x-progress-text-back {
        line-height: 17px
    }

    .x-progress-default .x-progress-text {
        line-height: 17px
    }
      .letter-admission-doc .x-grid-cell-inner {
        background-color: #B6DFDC;
        border-top-color: #b3e798;
        border-top-style: solid;
        border-bottom-color: #b3e798;
        border-bottom-style: solid;
    }
    .admission-doc .x-grid-cell-inner {
        border-top-color: #b3e798;
        border-top-style: solid;
        border-bottom-color: #b3e798;
        border-bottom-style: solid;
        background-color: #b3e798
    }
</style>

");
            WriteLiteral("    <div class=\"container-fluid\">\r\n        <script type=\"text/javascript\">\r\n\r\n            var disciplineUID = \'");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                            Write(Model.DisciplineUid);

#line default
#line hidden
#nullable disable
            WriteLiteral("\';\r\n            var year = ");
#nullable restore
#line 37 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                  Write(Model.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n            var semesterID = \'");
#nullable restore
#line 38 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                         Write(Model.SemesterID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\';\r\n            Ext.Ajax.setTimeout(60000);\r\n            var practiceref = \'&groupID=");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                   Write(Model.GroupHistoryId);

#line default
#line hidden
#nullable disable
            WriteLiteral("&year=");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                                              Write(Model.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral("&semesterID=");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                                                                     Write(Model.SemesterID);

#line default
#line hidden
#nullable disable
            WriteLiteral("&disciplineUID=");
#nullable restore
#line 40 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                                                                                                     Write(Model.DisciplineUid);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"';

            var localStorageName = ""PracticeGroupFilters"";

            var columnsToShowName = 'columnsToShowPracticeGroupStudents';
            var columnsToShow = [];

            Ext.onReady(function () {
                Ext.tip.QuickTipManager.init();
                Ext.state.Manager.setProvider(Ext.create('Ext.state.CookieProvider'));

                var store = Ext.create(""Ext.data.Store"", {
                    idProperty: 'StudentID',
                    autoLoad: false,
                    proxy: {
                        type: 'ajax',
                        url: '");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                         Write(Url.Action("GroupAjax"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\',\r\n                        extraParams: {\r\n                            groupId: \'");
#nullable restore
#line 58 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                 Write(Model.GroupId);

#line default
#line hidden
#nullable disable
            WriteLiteral("\',\r\n                            disciplineId: \'");
#nullable restore
#line 59 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                      Write(Model.DisciplineUid);

#line default
#line hidden
#nullable disable
            WriteLiteral("\',\r\n                            year: ");
#nullable restore
#line 60 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                             Write(Model.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral(",\r\n                            semesterId: ");
#nullable restore
#line 61 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                   Write(Model.SemesterID);

#line default
#line hidden
#nullable disable
            WriteLiteral(@",
                            hideStudents: sessionStorage.getItem('hideStudents') || false
                        },
                        reader: {
                            type: 'json',
                            rootProperty: 'data',
                            totalProperty: 'total'
                        },
                        timeout: 60000
                    }
                });


                Ext.define('PracticeProgressBar', {
                    extend: 'Ext.ProgressBar',
                    alias: 'widget.practiceProgressBar',
                    //baseCls: null,
                    defaultListenerScope: true,
                    updateText: function (text) {
                        var me = this;
                        if (this.rendered) {
                            var widgetColumn = this.getWidgetColumn();
                            var record = this.getWidgetRecord();
                            var text = widgetColumn && widgetColumn.textDataIndex;
");
            WriteLiteral(@"                            if (record && record.get(text)) {
                                text = record.get(text);
                            }
                            else {
                                text = Math.round(this.getValue() * 100) + '%';
                            }
                        }
                        return this.callParent(arguments);
                    }
                });

                Ext.define('PrcaticeProgressColumn', {
                    extend: 'Ext.grid.column.Widget',
                    alias: 'widget.practiceProgressColumn',
                    defaultListenerScope: true,
                    onAfterRenderCustom: function (progressBar, eOpts) {
                        progressBar.updateText(progressBar.text);
                    },

                    widget: {
                        xtype: 'practiceProgressBar',
                        height: '17px',
                        defaultListenerScope: true,
                       ");
            WriteLiteral(@" listeners: {
                            afterrender: 'onAfterRenderCustom'
                        }
                    }
                });

                var prevSettings = {};
                try {
                    var prevSettingString = JSON.parse(localStorage.getItem(localStorageName) || ""[]"");

                    for (var i = 0; i < prevSettingString.length; i++) {
                        prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
                    }

                    columnsToShow = JSON.parse(localStorage.getItem(columnsToShowName)) || [];

                } catch (err) {

                }

                var filtersWnd = null;
                function loadStore() {
                    store.proxy.setUrl('");
#nullable restore
#line 130 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                   Write(Url.Action("GroupAjax"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' +
                        '?filter=' +
                        encodeURIComponent(localStorage.getItem(localStorageName)));
                    store.load();
                }
                var setFilters = function () {                    
                    var settings = [
                        { property: 'Name', value: filtersWnd.getComponent(""Name"").getValue() },
                        { property: 'Variant', value: filtersWnd.getComponent(""Variant"").getValue()}
                    ];
                    localStorage.setItem(localStorageName, JSON.stringify(settings));
                    loadStore();
                };
                filtersWnd = Ext.create('Ext.window.Window',
                    {
                        title: ""Фильтры"",
                        closeAction: 'hide',
                        resizable: false,
                        autoHeight: true,
                        bodyPadding: 6,
                        defaults: {
                            xtyp");
            WriteLiteral(@"e: 'textfield',
                            width: 500,
                            labelWidth: 130
                        },
                        items: [
                            { fieldLabel: ""ФИО студента"", itemId: ""Name"", value: prevSettings[""Name""] },
                            { fieldLabel: ""Траектория образовательной программы"", itemId: ""Variant"", value: prevSettings[""Variant""] },
                        ],
                        buttons: [
                            {
                                text: ""OK"",
                                handler: function () {
                                    setFilters();
                                    filtersWnd.hide();
                                }
                            },
                            {
                                text: ""Отмена"",
                                handler: function () { filtersWnd.hide(); }
                            }
                        ]
                    });
       ");
            WriteLiteral(@"         setFilters();
                var gridPanel = Ext.create('Ext.grid.Panel',
                    {
                        region: 'center',
                        store: store,
                        loadMask: true,
                        columnLines: true,
                        enableLocking: true,
                        id: 'GroupPracticeGrid',
                        stateful: true,
                        stateId: 'statefulgrid',
                        viewConfig: {
                            markDirty: false
                        },
                        tbar: [
                            {
                                xtype: 'button',
                                text: ""Руководители, темы"",
                                handler: function () {
                                    window.location = ""/Practice/Index?focus=");
#nullable restore
#line 192 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                                                        Write(Model.DisciplineUid);

#line default
#line hidden
#nullable disable
#nullable restore
#line 192 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                                                                            Write(Model.GroupHistoryId);

#line default
#line hidden
#nullable disable
#nullable restore
#line 192 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                                                                                                 Write(Model.SemesterID);

#line default
#line hidden
#nullable disable
            WriteLiteral(@""";
                                }
                            }, '-',
                            {
                                xtype: 'button',
                                text: ""Фильтры"",
                                handler: function () {
                                    filtersWnd.show();
                                }
                            },
                            {
                                xtype: 'button',
                                text: ""Отменить фильтры"",
                                handler: function () {
                                    settings = [];
                                    filtersWnd.getComponent(""Name"").setValue('');
                                    filtersWnd.getComponent(""Variant"").setValue('');
                                    setFilters();
                                }
                            }, '-',
                            {
                                xtype: 'checkbox',
               ");
            WriteLiteral(@"                 boxLabel: 'Скрыть неактивных студентов',
                                itemId: 'HideNotactiveStudents',
                                value: sessionStorage.getItem('hideStudents') || false,
                                listeners: {
                                    change: function (t, newValue, oldValue, eOpts) {
                                        store.getProxy().setExtraParam('hideStudents', newValue);
                                        sessionStorage.setItem('hideStudents', newValue);
                                        console.log('alert', newValue);
                                        setFilters();
                                    }
                                }
                            },
                        ],

                        viewConfig: {
                            getRowClass: function (record) {                           
                                if (record.get('RejectionLetter')) {
                         ");
            WriteLiteral(@"           if (record.get('DocHasNoStatus'))
                                        return 'letter-admission-doc'
                                    else
                                        return 'highlight'
                                }
                                else if (record.get('DocHasNoStatus'))
                                    return 'admission-doc'
                                else
                                    '';
                            },
                            markDirty: true
                        },
                        columns: [{
                            xtype: 'rownumberer',
                            width: 50,
                            locked: true,
                            scrollable: false
                        }, {
                            header: 'Студент',
                            dataIndex: 'Name',
                            width: 250,
                            locked: true,
                          ");
            WriteLiteral(@"  scrollable: false,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            header: 'Статус',
                            dataIndex: 'StudentStatus',
                            width: 110,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                       }, {
                            header: 'Вид возмещения затрат',
                            dataIndex: 'StudentCompensation',
                            width: 140,
                            renderer: Urfu.renders.htmlEncodeWithToolTip

                        },{
                            header: 'Номер',
                            dataIndex: 'PersonalNumber',
                            width: 110,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            header: 'Сроки практики',
                            align: 'left',
                            dat");
            WriteLiteral(@"aIndex: 'PracticeDateInfo',
                            width: 190,
                            renderer: function (value, metaData) {
                                var tooltippvalue = Ext.String.htmlEncode(value +' '+ 'Совпадает со сроком группы: ' + metaData.record.data.EqualsGroupDates);
                                metaData.tdAttr = 'data-qtip=""' + Ext.String.htmlEncode(tooltippvalue) + '""';
                                return value;
                            }
                            },{
                                header: 'Срок сдачи отчета',
                                align: 'left',
                                dataIndex: 'ReportDatesInfo',
                                width: 190,
                                renderer: function (value, metaData) {
                                   var tooltippvalue = Ext.String.htmlEncode(value +' '+ 'Совпадает со сроком группы: ' + metaData.record.data.EqualsGroupReportDates);
                                    metaData.");
            WriteLiteral(@"tdAttr = 'data-qtip=""' + Ext.String.htmlEncode(tooltippvalue) + '""';
                                    return value;
                                }
                            },
                            {
                            header: 'Руководитель практики от УрФУ',
                            dataIndex: 'Teacher',
                            width: 200,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            xtype: 'checkcolumn',
                            header: 'С выездом',
                            dataIndex: 'IsExternal',
                            width: 90,
                            disabled: true,
                            disabledCls: 'x-item-enabled'
                        }, {
                            header: 'Сроки выезда руководителя от УрФУ',
                            dataIndex: 'ExternalDateInfo',
                            width: 200,
                            rendere");
            WriteLiteral(@"r: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            header: 'Соруководитель УрФУ',
                            dataIndex: 'Teacher2',
                            width: 160,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            header: 'Подразделение УрФУ',
                            dataIndex: 'Subdivision',
                            width: 150,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            header: 'Статус',
                            dataIndex: 'AdmissionStatus',
                            width: 90,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            header: 'Предприятие',
                            dataIndex: 'CompanyName',
                            width: 200,
                            renderer: Urf");
            WriteLiteral(@"u.renders.htmlEncodeWithToolTip
                        }, {
                            header: 'Статус',
                            dataIndex: 'AdmissionCompanyStatus',
                            width: 90,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            xtype: 'practiceProgressColumn',
                            header: 'Документы, % заполняемости',
                            dataIndex: 'PercentComplete',
                            textDataIndex: 'PercentCompleteText',
                            width: 150,
                            
                            //widget: {
                            //    xtype: 'progressbarwidget',
                            //    textTpl: '{value:percent}',
                            //    style: { 'margin': '1px' }
                            //}
                            }, {
                                xtype: 'practiceProgressColumn',
                ");
            WriteLiteral(@"                header: 'Документы (согласованные), % заполняемости',
                                dataIndex: 'PercentCompleteAdmDoc',
                                textDataIndex: 'PercentCompleteeAdmDocText',
                                width: 164,
                            }, {
                            xtype: 'actioncolumn',
                            region: 'center',
                            sortable: false,
                            width: 50,
                            items: [{
                                icon: '");
#nullable restore
#line 358 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                  Write(Url.Content("~/Content/Images/edit.png"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                iconCls: 'icon-padding',
                                text: 'Редактировать',
                                tooltip: 'Редактировать',
                                handler: function (grid, rowIndex, colIndex) {
                                    var rec = grid.getStore().getAt(rowIndex);
                                    var link =""");
#nullable restore
#line 364 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                          Write(Url.Action("Practice"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?Id="" + rec.get('PracticeID') + ""&studentID="" + rec.get('StudentID') + practiceref;
                                    window.open(link, '_blank');

                                }
                            }]
                        }, {
                            xtype: 'checkcolumn',
                            text: 'Целевой прием',
                            dataIndex: 'IsTarget',
                            disabled: true,
                            hidden: '");
#nullable restore
#line 374 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                Write(Model.UserIsInRole);

#line default
#line hidden
#nullable disable
            WriteLiteral("\' != \'True\'\r\n                        }, {\r\n                            header: \'Номер договора\',\r\n                            dataIndex: \'ContractNumber\',\r\n                            width: 120,\r\n                            hidden: \'");
#nullable restore
#line 379 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                Write(Model.UserIsInRole);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' != 'True',
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            xtype: 'checkcolumn',
                            text: 'Есть договор',
                            dataIndex: 'ExistContract',
                            listeners: {
                                checkchange: function (grid, rowIndex, checked, eOpts) {
                                    var rec = gridPanel.getStore().getAt(rowIndex);

                                    var removeChecked = function () {
                                        gridPanel.getStore().getAt(rowIndex).set({ ExistContract: !checked });
                                        Ext.getCmp('GroupPracticeGrid').getView().refresh();
                                    };

                                    var request = function () {
                                        Ext.Ajax.request({
                                            url: '");
#nullable restore
#line 396 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                             Write(Url.Action("SetExistContract"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                            params: {
                                                practiceId: rec.get('PracticeID'),
                                                isChecked: checked
                                            },
                                            success: function (response) {
                                                if (response.responseText != """") {
                                                    var data = Ext.decode(response.responseText);
                                                    if (data.success) {
                                                    }
                                                    else {
                                                        removeChecked();
                                                        Ext.MessageBox.show({
                                                            title: 'Ошибка',
                                                            msg: data.message,
       ");
            WriteLiteral(@"                                                     buttons: Ext.MessageBox.OK
                                                        });
                                                    }
                                                }
                                            },
                                            failure: function (response) {
                                                Ext.MessageBox.show({
                                                    title: 'Ошибка',
                                                    msg: 'Неизвестная ошибка',
                                                    buttons: Ext.MessageBox.OK
                                                });
                                            }
                                        });
                                    };

                                    // проверка на наличие роли у пользователя
                                    // несколько по-тупому 
                       ");
            WriteLiteral("             Ext.Ajax.request({\r\n                                        url: \'");
#nullable restore
#line 429 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeGroup\Group.cshtml"
                                         Write(Url.Action("CheckConfirmRole"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                        success: function (response) {
                                            if (response.responseText == """") {
                                                request();
                                            }
                                            else {
                                                removeChecked();
                                                Ext.MessageBox.alert('Ошибка', ""У вас нет прав редактировать это поле"");
                                            }
                                        },
                                        failure: function (response) {
                                        }
                                    });
                                }
                            }
                        }, {
                            header: 'Траектория образовательной программы',
                            dataIndex: 'Variant',
                            width: 400,
         ");
            WriteLiteral(@"                   renderer: Urfu.renders.htmlEncodeWithToolTip
                        }]
                    });

                var items = [
                    gridPanel
                ];

                Urfu.createViewport('border', items);

                hideColumns(gridPanel, columnsToShow);

                gridPanel.on('columnschanged', function (ct, e) {
                    if (ct.gridVisibleColumns === null || ct.gridVisibleColumns === undefined)
                        return;
                    columnsToShow = ct.gridVisibleColumns.map(function (c) { return c.dataIndex; });
                    localStorage.setItem(columnsToShowName, JSON.stringify(columnsToShow));
                });

            });

            
            function hideColumns(gridPanel, columnsToShow) {
                if (columnsToShow.length > 0) {
                    gridPanel.columns.filter(column => !column.locked && !columnsToShow.includes(column.dataIndex)).forEach(column => column.setHid");
            WriteLiteral("den(true));\r\n                }\r\n            };\r\n        </script>\r\n    </div>\r\n");
            WriteLiteral("\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Model.Models.Practice.GroupViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
