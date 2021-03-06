#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "47c5a6994183f913b1afe0c204f9ba874841fccb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_PracticeReport_Students), @"mvc.1.0.view", @"/Views/PracticeReport/Students.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"47c5a6994183f913b1afe0c204f9ba874841fccb", @"/Views/PracticeReport/Students.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_PracticeReport_Students : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
  
    ViewBag.Title = "Отчет по студентам";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            WriteLiteral(@"    <div class=""container-fluid"">
        <script type=""text/javascript"">
            Ext.onReady(function () {

                Ext.tip.QuickTipManager.init();

                var localStorageName = ""PracticeReportStudentFilters"";

                function parseJson(json) {
                    var data = JSON.parse(json.replace(/&quot;/g, '""'));
                    return data;
                }

                var yearData = parseJson('");
#nullable restore
#line 21 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                     Write(ViewBag.Years);

#line default
#line hidden
#nullable disable
            WriteLiteral("\');\r\n                var yearStore = Ext.create(\'Ext.data.Store\', {\r\n                    data: yearData,\r\n                });\r\n\r\n                var semesterData = parseJson(\'");
#nullable restore
#line 26 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                         Write(ViewBag.Semesters);

#line default
#line hidden
#nullable disable
            WriteLiteral("\');\r\n                var semesterStore = Ext.create(\'Ext.data.Store\', {\r\n                    data: semesterData\r\n                });\r\n\r\n                var divisionsData = parseJson(\'");
#nullable restore
#line 31 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                          Write(ViewBag.Divisions);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"');
                var divisionStore = Ext.create('Ext.data.Store', {
                   data: divisionsData
                });
                divisionStore.on(""load"", function (store) {
                    store.insert(0, { Id: '', Name: 'Не указано' /*'&nbsp'*/ });
                });
                divisionStore.load();

                var prevSettings = {};
                try {
                    var prevSettingString = JSON.parse(localStorage.getItem(localStorageName) || ""[]"");

                    for (var i = 0; i < prevSettingString.length; i++) {
                        prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
                    }

                    if (prevSettings[""Year""] == null) {
                        prevSettings[""Year""] = ");
#nullable restore
#line 49 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                           Write(DateTime.Now.Month < 7? DateTime.Now.Year -1 : DateTime.Now.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral(@";
                    }
                    if (prevSettings[""Division""] == null || prevSettings[""Division""] == ""[]"") {
                        prevSettings[""Division""] = divisionStore.getAt(1);
                    }
                    if (prevSettings[""Semester""] == null) {
                        prevSettings[""Semester""] = ");
#nullable restore
#line 55 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                               Write(DateTime.Now.Month < 7?2:1);

#line default
#line hidden
#nullable disable
            WriteLiteral(@";
                    }
                }
                catch (err) {
                };

                var setFilters = function () {

                    var settings = [
                        { property: 'Year', value: yearCmbx.getValue() },
                        { property: 'Semester', value: semesterCmbx.getValue() },
                        { property: 'Division', value: divisionCmbx.getValue() },
                        { property: 'Direction', value: directionCmbx.getValue() },
                        { property: 'Group', value: groupCmbx.getValue() },
                        //{ property: 'FamilirizationType', value: prevSettings['FamilirizationType'] },
                        //{ property: 'Qualification', value: prevSettings['Qualification'] }
                        { property: 'PracticeName', value: filtersWnd.getComponent(""PracticeName"").getValue() },
                        { property: 'StudentName', value: filtersWnd.getComponent(""StudentName"").getValue() }
         ");
            WriteLiteral(@"           ];
                    store.setFilters(settings);
                    localStorage.setItem(localStorageName, JSON.stringify(settings));
                };

                var directionStore = Ext.create(""Ext.data.Store"",
                    {
                        autoLoad: false,
                        proxy: {
                            type: 'ajax',
                            url: '");
#nullable restore
#line 83 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                             Write(Url.Action("DirectionList"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?institute=' + null,
                            reader: {
                                type: 'json',
                                rootProperty: 'data'
                            }
                        },
                        listeners: {
                            load: function (t, records, successful, eOpts) {
                                t.insert(0, { Id: '', Name: 'Не указано' /*'&nbsp'*/ });
                                directionCmbx.setValue(prevSettings[""Direction""]);
                                directionCmbx.setDisabled(false);
                            }
                        }
                    });

                var groupStore = Ext.create(""Ext.data.Store"",
                    {
                        autoLoad: false,
                        proxy: {
                            type: 'ajax',
                            url: '");
#nullable restore
#line 103 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                             Write(Url.Action("GroupList"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?year=' + 0 + '&institute=' + null,
                            reader: {
                                type: 'json',
                                rootProperty: 'data'
                            }
                        },
                        listeners: {
                            load: function (t, records, successful, eOpts) {
                                t.insert(0, { Id: '', Name: 'Не указано' /*'&nbsp'*/ });
                                groupCmbx.setValue(prevSettings[""Group""]);
                                groupCmbx.setDisabled(false);
                            }
                        }
                    });

");
            WriteLiteral(@"
                var store = Ext.create(""Ext.data.Store"", {
                    autoLoad: true,
                    remoteSort: false,
                    remoteFilter: true,
                    autoSync: false,
                    proxy: {
                        type: 'ajax',
                        url: '");
#nullable restore
#line 135 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                         Write(Url.Action("ReportStudents"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                        reader: {
                            type: 'json',
                            rootProperty: 'data',
                            totalProperty: 'total'
                        },
                        timeout: 1200000
                    }
                });

                function reloadOksoStore(institute) {
                    if (directionCmbx != null) directionCmbx.setDisabled(true);
                    directionStore.proxy.setUrl('");
#nullable restore
#line 147 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                            Write(Url.Action("DirectionList"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?institute=' + institute);
                    directionStore.reload();
                }

                function reloadGroupStore(year, institute, okso) {
                    if (groupCmbx != null) groupCmbx.setDisabled(true);
                    groupStore.proxy.setUrl('");
#nullable restore
#line 153 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                        Write(Url.Action("GroupList"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?year=' + year + '&institute=' + institute + '&okso=' + okso);
                    groupStore.reload();
                }

                function reloadOkso() {
                    var institute = divisionCmbx.getValue();
                    reloadOksoStore(institute);
                    prevSettings[""Direction""] = '';
                }

                function reloadGroups() {
                    var year = yearCmbx.getValue();
                    var institute = divisionCmbx.getValue();
                    var okso = directionCmbx.getValue();
                    reloadGroupStore(year, institute, okso);
                    prevSettings[""Group""] = '';
                }

                var filterLabelWidth = 110;
                var yearCmbx = Ext.create('Ext.form.ComboBox',
                    {
                        xtype: 'combobox',
                        fieldLabel: 'Учебный год',
                        store: yearStore,
                        queryMode: 'remote',
      ");
            WriteLiteral(@"                  valueField: 'Year',
                        displayField: 'Year',
                        labelWidth: filterLabelWidth,
                        width: 200,
                        value: prevSettings['Year'],
                        listeners: {
                            'select': function (combo, records, eOpts) {
                                reloadGroups();

                                semesterCmbx.setDisabled(false);
                                divisionCmbx.setDisabled(false);
                                directionCmbx.setDisabled(false);
                                groupCmbx.setValue('');
                                groupCmbx.setDisabled(false);
                            },
                            'blur': function (combo, event, eOpts) {
                                if (combo.getValue() == null) {
                                    // учебный год не выбран -> остальные фильтры неактивны
                                    semesterCmbx.s");
            WriteLiteral(@"etValue('');
                                    semesterCmbx.setDisabled(true);
                                    divisionCmbx.setValue('');
                                    divisionCmbx.setDisabled(true);
                                    directionCmbx.setValue('');
                                    directionCmbx.setDisabled(true);
                                    groupCmbx.setValue('');
                                    groupCmbx.setDisabled(true);
                                }
                                else {
                                    semesterCmbx.setDisabled(false);
                                    divisionCmbx.setDisabled(false);
                                    directionCmbx.setDisabled(false);
                                    groupCmbx.setValue('');
                                    groupCmbx.setDisabled(false);
                                }
                            }
                        }
                    });

            ");
            WriteLiteral(@"    yearStore.on(""load"", function () {
                    //yearCmbx.select(yearStore.getAt(yearStore.data.items.length - 1));
                });
                yearStore.load();

                var semesterCmbx = Ext.create('Ext.form.ComboBox', {
                        xtype: 'combobox',
                        fieldLabel: 'Семестр',
                        store: semesterStore,
                        valueField: 'Id',
                        displayField: 'Name',
                        queryMode: 'local',
                        labelWidth: filterLabelWidth,
                        width: 250,
                        value: prevSettings['Semester']
                });

                function selectEvent(combo, records, funcIfNotEmpty, funcIfEmpty) {
                    var ids = [];
                    var checkEmpty = records[records.length - 1].data.Id == '';

                    if (!checkEmpty) {
                        records.forEach(function (t) {
                     ");
            WriteLiteral(@"       if (t.data.Id != '')
                                ids.push(t.data.Id);
                        });
                        funcIfNotEmpty(ids);
                        combo.setValue(ids);
                    }
                    else {
                        // выбран вариант ""Не указано""
                        combo.setValue('');
                        ids.push('');
                        funcIfEmpty();
                    }

                    // отметить галки в checkbox
                    var node;
                    Ext.each(records, function (rec) {
                        var checked = ids.includes(rec.data.Id);
                        node = combo.getPicker().getNode(rec);
                        Ext.get(node).down('input').dom.checked = checked;
                    });
                }

                function expandEvent(combo) {
                    combo.value.forEach(function (item, value, index, values) {
                        var rec = combo.findRec");
            WriteLiteral(@"ord('Id', item);
                        var node = combo.getPicker().getNode(rec);
                        var extNode = Ext.get(node);
                        if (extNode != null)
                            extNode.down('input').dom.checked = true;
                    });
                }

                function blurEvent(combo) {
                    if (combo.value.length == 0)
                        combo.setValue('');
                }

                function beforedeselect(combo, rec) {
                    // снять галки в checkbox
                    var node = combo.getPicker().getNode(rec);
                    var extNode = Ext.get(node);
                    if (extNode != null)
                        extNode.down('input').dom.checked = false;
                }

                var divisionCmbx = Ext.create('Ext.form.ComboBox', {
                    xtype: 'combobox',
                    fieldLabel: 'Институт',
                    store: divisionStore,
               ");
            WriteLiteral(@"     valueField: 'Id',
                    displayField: 'Name',
                    queryMode: 'local',
                    multiSelect: true,
                    labelWidth: filterLabelWidth,
                    width: 600,
                    value: prevSettings['Division'],
                    tpl: new Ext.XTemplate('<tpl for=""."">', '<div class=""x-boundlist-item"">', '<input type=""checkbox"" />', '   ' + '{Name}', '</div>', '</tpl>'),
                    listeners: {
                        select: function (combo, records, eOpts) {
                            selectEvent(combo, records,
                                function (ids) { // выбран институт
                                    reloadOkso();
                                    directionCmbx.setDisabled(false);

                                    var year = yearCmbx.getValue();
                                    reloadGroupStore(year, ids, '');
                                    prevSettings[""Group""] = '';
                   ");
            WriteLiteral(@"                 groupCmbx.setDisabled(false);
                                },
                                function () { // выбрано ""Не указано""
                                    directionCmbx.getStore().removeAll();
                                    directionCmbx.getStore().insert(0, { Id: '', Name: 'Не указано' /*'&nbsp'*/ });
                                    directionCmbx.setValue('');
                                    directionCmbx.setDisabled(true);

                                    groupCmbx.getStore().removeAll();
                                    groupCmbx.getStore().insert(0, { Id: '', Name: 'Не указано' /*'&nbsp'*/ });
                                    groupCmbx.setValue('');
                                    groupCmbx.setDisabled(true);
                                }
                            );
                        },
                        blur: function (combo, event, eOpts) {
                            blurEvent(combo);
                       ");
            WriteLiteral(@" },
                        beforedeselect: function (combo, rec) {
                            beforedeselect(combo, rec);
                        },
                        expand: function (field, eOpts) {
                            expandEvent(divisionCmbx);
                        }
                    }
                });

                var directionCmbx = Ext.create('Ext.form.ComboBox', {
                    xtype: 'combobox',
                    fieldLabel: 'Направление обучения',
                    store: directionStore,
                    valueField: 'Id',
                    displayField: 'Name',
                    queryMode: 'local',
                    labelWidth: filterLabelWidth,
                    width: 600,
                    multiSelect: true,
                    value: prevSettings['Direction'],
                    tpl: new Ext.XTemplate('<tpl for=""."">', '<div class=""x-boundlist-item"">', '<input type=""checkbox"" />', '   ' + '{Name}', '</div>', '</tpl>'),
    ");
            WriteLiteral(@"                listeners: {
                        select: function (combo, records, eOpts) {
                            selectEvent(combo, records,
                                function (ids) { // выбран элемент
                                    reloadGroups();
                                    groupCmbx.setDisabled(false);
                                },
                                function () { // выбрано ""Не указано""
                                    var year = yearCmbx.getValue();
                                    var institute = divisionCmbx.getValue();
                                    reloadGroupStore(year, institute, '');
                                    prevSettings[""Group""] = '';
                                    groupCmbx.setDisabled(false);
                                }
                            );
                        },
                        blur: function (combo, event, eOpts) {
                            blurEvent(combo);
             ");
            WriteLiteral(@"           },
                        beforedeselect: function (combo, rec) {
                            beforedeselect(combo, rec);
                        },
                        expand: function (field, eOpts) {
                            expandEvent(directionCmbx);
                        }
                    }
                });

                reloadOksoStore(divisionCmbx.getValue());
                reloadGroupStore(yearCmbx.getValue(), divisionCmbx.getValue(), directionCmbx.getValue());

                var groupCmbx = Ext.create('Ext.form.ComboBox', {
                    xtype: 'combobox',
                    fieldLabel: 'Группа',
                    store: groupStore,
                    labelWidth: filterLabelWidth,
                    valueField: 'Id',
                    displayField: 'Name',
                    queryMode: 'local',
                    multiSelect: true,
                    anyMatch: true,
                    width: 600,
                    value: p");
            WriteLiteral(@"revSettings[""Group""],
                    tpl: new Ext.XTemplate('<tpl for=""."">', '<div class=""x-boundlist-item"">', '<input type=""checkbox"" />', '   ' + '{Name}', '</div>', '</tpl>'),
                    listeners: {
                        select: function (combo, records, eOpts) {
                            selectEvent(combo, records,
                                function (ids) {
                                },
                                function () {
                                }
                            );
                        },
                        blur: function (combo, event, eOpts) {
                            blurEvent(combo);
                        },
                        beforedeselect: function (combo, rec) {
                            beforedeselect(combo, rec);
                        },
                        expand: function (field, eOpts) {
                            expandEvent(groupCmbx);
                        }
                    }
");
            WriteLiteral(@"                });

                //var familirizationTypeCmbx = Ext.create('Ext.form.ComboBox' , {
                //    //xtype: 'combobox',
                //    fieldLabel: 'Форма обучения',
                //    store: familirizationTypeStore,
                //    valueField: 'Name',
                //    displayField: 'Name',
                //    queryMode: 'local',
                //    labelWidth: 190,
                //    width: 600,
                //    value: prevSettings['FamilirizationType']
                //});

                //var qualificationCmbx = Ext.create('Ext.form.ComboBox', {
                //    //xtype: 'combobox',
                //    fieldLabel: 'Уровень обучения',
                //    store: qualificationStore,
                //    valueField: 'Name',
                //    displayField: 'Name',
                //    queryMode: 'local',
                //    labelWidth: 190,
                //    width: 600,
                //    value: prevSetti");
            WriteLiteral(@"ngs['Qualification']
                //});

                var filtersWnd = Ext.create('Ext.window.Window', {
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
                        yearCmbx,
                        semesterCmbx,
                        divisionCmbx,
                        directionCmbx,
                        groupCmbx,
                        //familirizationTypeCmbx,
                        //qualificationCmbx
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Название практики',
                            value: prevSettings[""PracticeName""],
                            labelWidth: filterLabelWidth");
            WriteLiteral(@",
                            itemId: 'PracticeName',
                            margin: '0 0 10 0',
                            width: 600
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'ФИО студента',
                            value: prevSettings[""StudentName""],
                            labelWidth: filterLabelWidth,
                            itemId: 'StudentName',
                            margin: '0 0 10 0',
                            width: 600
                        }
                    ],
                    buttons: [{
                        text: ""ОК"",
                        handler: function () {
                            setFilters();
                            filtersWnd.hide();
                        }
                    }, {
                        text: ""Отмена"",
                        handler: function () { filtersWnd.hide(); }
                    }]
      ");
            WriteLiteral(@"          });

                setFilters();

                var gridPanel = Ext.create('Ext.grid.Panel',
                {
                    region: 'center',
                    store: store,
                    loadMask: true,
                    columnLines: true,


                    tbar: [{
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

                                yearCmbx.setValue(");
#nullable restore
#line 506 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                              Write(DateTime.Now.Month < 7? DateTime.Now.Year -1 : DateTime.Now.Year);

#line default
#line hidden
#nullable disable
            WriteLiteral(");\r\n                                semesterCmbx.setValue(");
#nullable restore
#line 507 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                                  Write(DateTime.Now.Month < 7?2:1);

#line default
#line hidden
#nullable disable
            WriteLiteral(@");
                                divisionCmbx.setValue(divisionStore.getAt(1));
                                directionStore.removeAll();
                                directionCmbx.setValue('');
                                groupStore.removeAll();
                                groupCmbx.setValue('');
                                //familirizationTypeCmbx.setValue('');
                                //qualificationCmbx.setValue('');
                                filtersWnd.getComponent(""PracticeName"").setValue('');
                                filtersWnd.getComponent(""StudentName"").setValue('');
                                setFilters();

                                reloadOksoStore(divisionCmbx.getValue());
                                reloadGroupStore(yearCmbx.getValue(), divisionCmbx.getValue(), '');
                                directionCmbx.setDisabled(false);
                                groupCmbx.setDisabled(false);
                            }
      ");
            WriteLiteral("                  }, \'-\',\r\n                        {\r\n                            xtype: \'button\',\r\n                            text: \"Экспорт в Excel\",\r\n                            handler: function () {\r\n                                var fileUrl = \'");
#nullable restore
#line 529 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeReport\Students.cshtml"
                                          Write(Url.Action("DownloadStudentsReportExcel"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"'
                                        .concat('?filter=' + encodeURIComponent(localStorage.getItem(localStorageName) || ""[]""));
                                    window.location.href = fileUrl;
                                    return false;
                            }
                        }
                    ],
                    columns: [
                        { xtype: 'rownumberer', width: 50 },
                        { header: 'Фамилия', dataIndex: 'Surname', width: 150, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Имя', dataIndex: 'Name', width: 150, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Отчество', dataIndex: 'PatronymicName', width: 150, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Группа', dataIndex: 'GroupName', ");
            WriteLiteral(@"width: 130, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Целевой прием', dataIndex: 'IsTarget', width: 100, align: 'center', cellWrap: true, sortable: true, renderer: function (val) { return val ? 'Да' : ""Нет"" } },
                        { header: 'Иностранный студент', dataIndex: 'IsInternational', width: 150, align: 'center', cellWrap: true, sortable: true, renderer: function (val) { return val ? 'Да' : ""Нет"" } },
                        { header: 'Вид возмещения затрат', dataIndex: 'Compensation', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },

                        { header: 'Институт', dataIndex: 'Institute', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Департамент', dataIndex: 'Department', width: 200, align: 'left', cellWrap: true, sortable: true, renderer");
            WriteLiteral(@": Urfu.renders.htmlEncodeWithToolTip },

                        { header: 'ОКСО+направление', dataIndex: 'Okso', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Учебный год', dataIndex: 'Year', width: 140, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Семестр', dataIndex: 'Semester', width: 100, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Название практики', dataIndex: 'PracticeName', width: 350, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Тип практики', dataIndex: 'PracticeType', width: 250, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Сроки практики', dataIndex: 'PrcaticeD");
            WriteLiteral(@"ate', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'С выездом', dataIndex: 'IsExternal', width: 100, align: 'center', cellWrap: true, sortable: true, renderer: function (val) { return val ? 'Да' : ""Нет"" }},
                        { header: 'Руководитель от УрФУ', dataIndex: 'Teacher', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Соруководитель от УрФУ', dataIndex: 'Teacher2', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Общая тема', dataIndex: 'Theme', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Уточненная тема', dataIndex: 'FinishTheme', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htm");
            WriteLiteral(@"lEncodeWithToolTip },
                        { header: 'Статус по заявке от УрФУ', dataIndex: 'UrfuStatus', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Предприятие', dataIndex: 'Company', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Страна', dataIndex: 'Country', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Город', dataIndex: 'City', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Руководитель от предприятия', dataIndex: 'PersonInCharge', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                        { header: 'Статус по заявке на предприятие', dataIndex: '");
            WriteLiteral(@"CompanyStatus', width: 200, align: 'left', cellWrap: true, sortable: true, renderer: Urfu.renders.htmlEncodeWithToolTip },
                    ],
                    });

                gridPanel.view.markDirty = false;

                var items = [
                    gridPanel
                ];

                Urfu.createViewport('border', items);

            });
        </script>
    </div>
");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
