#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "305c1f476eeaab011abb9681cc98b72154a110cb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_SectionFKAdmission_CompetitionGroupStudents), @"mvc.1.0.view", @"/Views/SectionFKAdmission/CompetitionGroupStudents.cshtml")]
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
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
using Urfu.Its.Common;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
using Urfu.Its.Web.DataContext;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"305c1f476eeaab011abb9681cc98b72154a110cb", @"/Views/SectionFKAdmission/CompetitionGroupStudents.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_SectionFKAdmission_CompetitionGroupStudents : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<SectionFKProperty>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 4 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
  
    Layout = "~/Views/Shared/_ExtLayout.cshtml";
    var filterName = "SectionFKCompetitionGroupStudentsFilters";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"<div id=""title2"" style=""display:none;"">
    <table class=""table table-bordered"">
        <thead>
            <tr>
                <th>
                    Лимит
                </th>
                <th>
                    Кол-во зачисленных
                </th>
                <th>
                    Кол-во зачисленных без спортсменов
                </th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                    ");
#nullable restore
#line 26 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
               Write(Model.Limit);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 29 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
               Write(ViewBag.AdmittedCount);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
               Write(ViewBag.AdmittedCountWOSportsmens);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n</div>\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n    <script type=\"text/javascript\">\r\n\r\n        function downloadReport() {\r\n            var fileUrl = \'");
#nullable restore
#line 43 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                      Write(Url.Action("DownloadMinorStudentsReport", new {id = ViewBag.MinorPeriodId}));

#line default
#line hidden
#nullable disable
                WriteLiteral("\'\r\n                .concat(\'?filter=\' + encodeURIComponent(localStorage.getItem(\"");
#nullable restore
#line 44 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                                                         Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral("\") || \"[]\"));\r\n            window.location.href = fileUrl;\r\n            return false;\r\n        }\r\n\r\n        var statuses =\r\n            ");
#nullable restore
#line 50 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
       Write(Html.Raw(Json.Encode(EnumHelper<AdmissionStatus>.GetValues(AdmissionStatus.Admitted).Select(m => new {Value = m, Text = EnumHelper<AdmissionStatus>.GetDisplayValue(m)}).ToList())));

#line default
#line hidden
#nullable disable
                WriteLiteral(@";
        var statesStore = Ext.create('Ext.data.Store',
        {
            fields: ['Value', 'Text'],
            data: statuses
        });

        Ext.onReady(function() {

            Ext.tip.QuickTipManager.init();
            var propInfo = $('#title2').html();

            var plainPanel = Ext.create('Ext.Panel',
            {
                //padding: '5px',
                border: false,
                html: propInfo
                });

            var hideStudentsStoreName = 'hideStudentsSectionFKAdmission';
            var isPriority = null;

            var store = Ext.create(""Ext.data.BufferedStore"",
            {
                fields: [
                    ""Id"", ""GroupName"", ""Surname"", ""Name"", ""PatronymicName"", ""Rating"", ""IsTarget"", ""IsInternational"",
                    ""Compensation"", ""Priority"", ""ChangePriority"", ""Status"", ""PersonalNumber"", ""StudentStatus"", ""AnotherAdmission"",
                    ""VariantId"", ""Published"", ""Modified"", ""OtherAdmission"", ""sect");
                WriteLiteral(@"ionFKDebtTerms""
                ],
                autoLoad: false,
                pageSize: 300,
                remoteSort: true,
                remoteFilter: true,
                proxy: {
                    type: 'ajax',
                    url: '");
#nullable restore
#line 85 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                     Write(Url.Action("CompetitionGroupStudentsAjax"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                    extraParams: {\r\n                        id: \'");
#nullable restore
#line 87 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                        Write(ViewBag.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                        hideStudents: sessionStorage.getItem(hideStudentsStoreName) || false
                    },
                    reader: {
                        type: 'json',
                        rootProperty: 'data',
                        totalProperty: 'total'
                    }
                }
            });

            var prevSettings = {};
            try {
                var prevSettingString = JSON.parse(localStorage.getItem(""");
#nullable restore
#line 100 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                                                    Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""") || ""[]"");

                for (var i = 0; i < prevSettingString.length; i++) {
                    prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
                }
            } catch (err) {

            }

            var filtersWnd = null;
            function loadStore() {
                store.proxy.setUrl('");
#nullable restore
#line 111 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                               Write(Url.Action("CompetitionGroupStudentsAjax"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\' +\r\n                    \'?filter=\' +\r\n                    encodeURIComponent(localStorage.getItem(\"");
#nullable restore
#line 113 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                                        Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""")));
                store.load();
            }
            var setFilters = function() {
                var settings = [
                    { property: 'GroupName', value: filtersWnd.getComponent(""GroupNameField"").getValue() },
                    { property: 'Surname', value: filtersWnd.getComponent(""SurnameField"").getValue() },
                    { property: 'Name', value: filtersWnd.getComponent(""NameField"").getValue() },
                    { property: 'StudentStatus', value: filtersWnd.getComponent(""StudentStatusField"").getValue() },
                    { property: 'Status', value: filtersWnd.getComponent(""StatusField"").getValue() },
                    { property: 'PersonalNumber', value: filtersWnd.getComponent(""PersonalNumber"").getValue() },
                    { property: 'PatronymicName', value: filtersWnd.getComponent(""PatronymicNameField"").getValue() },
                    { property: 'Teacher', value: filtersWnd.getComponent(""Teacher"").getValue() },                  
          ");
                WriteLiteral(@"          {
                        property: 'Sportsman',
                        value: filtersWnd.getComponent(""SportsmanField"").getValue().SportsmanField
                    }
                ];
                if (isPriority)
                    settings.push({ property: 'IsPriority', value: true });
                localStorage.setItem(""");
#nullable restore
#line 133 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                 Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""", JSON.stringify(settings));
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
                    xtype: 'textfield',
                    width: 500
                },
                items: [
                    { fieldLabel: ""Название группы"", itemId: ""GroupNameField"", value: prevSettings[""GroupName""] },
                    { fieldLabel: ""Фамилия"", itemId: ""SurnameField"", value: prevSettings[""Surname""] },
                    { fieldLabel: ""Имя"", itemId: ""NameField"", value: prevSettings[""Name""] },
                    { fieldLabel: ""Отчество"", itemId: ""PatronymicNameField"", value: prevSettings[""PatronymicName""] },
                    { fieldLabel: ""Статус"", itemId: ""StudentStatusField"", value: prevSettings[""StudentStatus""] },
");
                WriteLiteral(@"                    { fieldLabel: ""Номер"", itemId: ""PersonalNumber"", value: prevSettings[""PersonalNumber""] },
                    {
                        fieldLabel: ""Состояние"",
                        itemId: ""StatusField"",
                        value: prevSettings[""Status""],
                        xtype: ""combobox"",
                        store: statesStore,
                        valueField: 'Value',
                        displayField: 'Text',
                        queryMode: 'local'

                    },
                    { fieldLabel: ""Преподаватель"", labelWidth: 110, itemId: ""Teacher"", value: prevSettings[""Teacher""] },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: 'Спортсмен',
                        itemId: ""SportsmanField"",
                        value: prevSettings[""Sportsman""],
                        items: [
                            {
                                boxLabel: 'Все',
                  ");
                WriteLiteral(@"              name: 'SportsmanField',
                                inputValue: ''

                            }, {
                                boxLabel: 'Да',
                                name: 'SportsmanField',
                                inputValue: 'true'
                            }, {
                                boxLabel: 'Нет',
                                name: 'SportsmanField',
                                inputValue: 'false'
                            }
                        ]
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
             ");
                WriteLiteral(@"   ]
            });

            setFilters();

            var selRecord = [];
            var gridPanel = null;

            function setAdmissionStatus(rec, status) {
                var request = function() {
                    Ext.Ajax.request({
                        url: '");
#nullable restore
#line 214 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                         Write(Url.Action("SetCompetitionGroupAdmissionStatus", new {propertyId = Model.Id,}));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                        params: {\r\n                            studentIds: rec.map(function(l) { return l.get(\"Id\"); }),\r\n                            minorPeriodId: ");
#nullable restore
#line 217 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                      Write(ViewBag.MinorPeriodId);

#line default
#line hidden
#nullable disable
                WriteLiteral(@",
                            status: status
                        },
                        success: function(response) {
                            var data = Ext.decode(response.responseText);
                            if (data.msg)
                                Ext.MessageBox.show({
                                    title: 'Информационное сообщение',
                                    msg: data.msg,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                            if (data.reload)
                                location.reload();


                            for (var i in rec) {
                                rec[i].set(""Status"", status);
                                rec[i].set(""Published"", false);
                                // rec.Save();
                            }
                            statusChanged(rec);
                        ");
                WriteLiteral(@"    //store.commitChanges();
                            gridPanel.getView().refresh();
                        }
                    });
                }
                if (rec[0].data.Status === 1 && (status === 0 || status === 2)) {
                    request();
                } else {
                    request();
                }
            }


            var indeterminateButton = Ext.create('Ext.Button',
            {
                xtype: 'button',
                disabled: true,
                text: 'Нет решения',
                handler: function() { setAdmissionStatus(selRecord, 0); }
            });

            var admittedButton = Ext.create('Ext.Button',
            {
                xtype: 'button',
                disabled: true,
                text: 'Зачислен',
                handler: function() { setAdmissionStatus(selRecord, 1); }
            });

            var deniedButton = Ext.create('Ext.Button',
            {
                xtype: 'button',");
                WriteLiteral(@"
                disabled: true,
                text: 'Не зачислен',
                handler: function() { setAdmissionStatus(selRecord, 2); }
            });

            var sendButton = Ext.create('Ext.Button',
            {
                xtype: 'button',
                disabled: true,
                text: 'Отправить в ЛК',
                handler: function() {
                    setTimeout(function() {
                        selRecord.forEach(function(el, index, array) {
                            el.set(""Published"", true);
                        });
                    },
                        0);
                    Ext.Ajax.request({
                        url: '");
#nullable restore
#line 289 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                         Write(Url.Action("PublishCompetitionGroupAdmission"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                        params: {\r\n                            studentId: selRecord.map(function(l) { return l.get(\"Id\"); }),\r\n                            propertyId: ");
#nullable restore
#line 292 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                   Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
                            },
                        success: function(response) {
                            Ext.toast({ html: ""Сообщение отправлено"", align: 't' });
                            gridPanel.getView().refresh();
                        }
                    });

                }
            });
            var downloadExcel = Ext.create('Ext.Button',
            {
                xtype: 'button',
                text: 'В Excel',
                handler: function() {
                    var fileUrl = '");
#nullable restore
#line 307 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                              Write(Url.Action("DownloadCompetitionGroupStudents",new {Id = Model.Id}));

#line default
#line hidden
#nullable disable
                WriteLiteral("\' +\r\n                        \"?filter=\" +\r\n                        encodeURIComponent(localStorage.getItem(\"");
#nullable restore
#line 309 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                                            Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""") || ""[]"");
                    window.location.href = fileUrl;
                }
            });


            function statusChanged(rec) {
                if (rec == null)
                    return;
                for (var i in rec) {
                    var anotherAdmission = rec[i].get(""AnotherAdmission"");
                    var status = rec[i].get(""Status"");
                    if (!anotherAdmission) {
                        indeterminateButton.setDisabled(status == 0 || !");
#nullable restore
#line 322 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                                                   Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral(");\r\n                        admittedButton.setDisabled(status == 1 || !");
#nullable restore
#line 323 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                                              Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral(");\r\n                        deniedButton.setDisabled(status == 2 || !");
#nullable restore
#line 324 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                                            Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral(");\r\n                        sendButton.setDisabled(!");
#nullable restore
#line 325 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                           Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral(@");
                    } else {
                        indeterminateButton.disable();
                        admittedButton.disable();
                        deniedButton.disable();
                        sendButton.disable();
                    }
                }
            }

            var tpl = '<a href=""/SectionFKAdmission/CompetitionGroupStudents/{Id}"">Долги</a>';

            gridPanel = Ext.create('Ext.grid.Panel',
            {
                multiSelect: true,
                region: 'center',
                store: store,
                padding: '5px',
                frame:true,
                loadMask: true,
                height:Ext.getBody().getViewSize().height - Ext.get('navbar').getHeight() - $('#propInfo').height() - 150-5,

                columnLines: true,
                listeners: {
                    selectionchange: function(el, records) {
                        selRecord = records.slice();
                        statusChanged(selRecord);
   ");
                WriteLiteral(@"                 }
                },
                dockedItems: [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                            {
                                xtype: 'button',
                                text: 'Фильтры...',
                                handler: function () { filtersWnd.show(); }
                            },
                            {
                                xtype: 'button',
                                text: ""Отменить фильтры"",
                                handler: function () {
                                    filtersWnd.items.items.forEach(function (element, index, array) {
                                        element.setValue('');
                                    });
                                    localStorage.setItem(""");
#nullable restore
#line 371 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\CompetitionGroupStudents.cshtml"
                                                     Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""", []);
                                    loadStore();
                                }
                            },
                            {
                                xtype: 'button',
                                text: ""Скрыть\\показать студентов c приоритетом"",
                                handler: function () {
                                    window.showPriority = !window.showPriority;
                                    if (window.showPriority) {
                                        isPriority = true;
                                    }
                                    else {
                                        isPriority = null;
                                    }
                                    setFilters();
                                }
                            },
                            '-',
                            indeterminateButton,
                            admittedButton,
                            deniedButton,
 ");
                WriteLiteral(@"                           '-',
                            sendButton,
                            downloadExcel
                        ]
                    },
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                            {
                                xtype: 'checkbox',
                                boxLabel: 'Скрыть неактивных студентов',
                                value: sessionStorage.getItem(hideStudentsStoreName) || false,
                                listeners: {
                                    change: function (t, newValue, oldValue, eOpts) {
                                        store.getProxy().setExtraParam('hideStudents', newValue);
                                        sessionStorage.setItem(hideStudentsStoreName, newValue);
                                        console.log('alert', newValue);
                                        setFilters();
       ");
                WriteLiteral(@"                             }
                                }
                            },
                        ]
                    }
                ],
                viewConfig: {
                    getRowClass: function(record) {
                        return !record.get('Published')
                            ? 'highlight'
                            : '';
                    },
                    markDirty: false
                },
                columns: [
                    { xtype: 'rownumberer', width: 50 },
                    {
                        header: 'Название группы',
                        dataIndex: 'GroupName',
                        width: 160,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Фамилия',
                        dataIndex: 'Surname',
                        width: 200,
                        renderer: function(value, metaData, record) {");
                WriteLiteral(@"
                            return '<a href=""/PersonalInfo/Student?studentId=' + record.data.Id + '"">' + value + '</a>';
                        }
                    },
                    {
                        header: 'Имя',
                        dataIndex: 'Name',
                        width: 200,
                        renderer: Urfu.renders.htmlEncode
                    },
                    {
                        header: 'Отчество',
                        dataIndex: 'PatronymicName',
                        width: 200,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Статус',
                        dataIndex: 'StudentStatus',
                        width: 200,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Личный номер студента',
                        dataIndex: 'PersonalNumber',
 ");
                WriteLiteral(@"                       width: 120,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Рейтинг',
                        dataIndex: 'Rating',
                        width: 90
                    },
                    {
                        header: 'Целевой',
                        dataIndex: 'IsTarget',
                        width: 100,
                        align: 'center',
                        renderer: function(val) { return val ? 'Да' : ""Нет"" }
                    },
                    {
                        header: 'Иностранный студент',
                        dataIndex: 'IsInternational',
                        width: 200,
                        align: 'center',
                        renderer: function(val) { return val ? 'Да' : ""Нет"" }
                    },
                    {
                        header: 'Вид возмещения затрат',
                        align: 'cent");
                WriteLiteral(@"er',
                        dataIndex: 'Compensation',
                        width: 210,
                        renderer: Urfu.renders.htmlEncode
                    },
                    {
                        header: 'Приоритет в ЛК',
                        dataIndex: 'Priority',
                        align: 'center',
                        width: 150
                    },
                    {
                        header: 'Изменение выбора',
                        dataIndex: 'ChangePriority',
                        align: 'center',
                        width: 150
                    },
                    {
                        header: 'Дата приоритета',
                        dataIndex: 'Modified',
                        align: 'center',
                        width: 160,
                        renderer: Ext.util.Format.dateRenderer('Y-n-d H:i:s')
                    },
                    {
                        header: 'Спортсмен',
                ");
                WriteLiteral(@"        dataIndex: 'Sportsman',
                        align: 'center',
                        width: 160,
                        renderer: function(val) { return val ? 'Да' : ""Нет"" }

                    },
                    {
                        header: 'Состояние',
                        width: 160,
                        dataIndex: 'Status',
                        xtype: 'templatecolumn',
                        tpl: new Ext.XTemplate(
                            '<tpl for=""."" if=""!!AnotherAdmission"">',
                            ""Зачислен в {AnotherAdmission:htmlEncode}"",
                            '<tpl else>',
                            '{[this.getAdmissionStatus(values.Status)]}',
                            ""</tpl>"",
                            {
                                getAdmissionStatus: function(status) {
                                    return { 0: ""Нет решения"", 1: ""Зачислен"", 2: ""Не зачислен"" }[status];
                                }
           ");
                WriteLiteral(@"                 }
                        )
                    },
                    {
                        //xtype: 'actioncolumn',
                        header: '<span data-qtip=""Должники"">Должники</span>',

                        dataIndex: 'sectionFKDebtTerms',
                        //tooltip: 'Должники',
                        width: 100,
                        renderer: function(val, meta, rec) {
                            if (val)
                                return Ext.String
                                    .format('<a onclick=\'showDebts({0})\' class=""glyphicon glyphicon-eye-open"" style=""cursor:pointer""></a>',
                                        val);
                            else return """";

                        }


                    },
                    {
                        header: 'Преподаватель',
                        align: 'center',
                        dataIndex: 'Teacher',
                        width: 200,
              ");
                WriteLiteral(@"          renderer: Urfu.renders.htmlEncode
                    },
                    {
                        header: 'Другая секция',
                        align: 'center',
                        dataIndex: 'OtherAdmission',
                        width: 210,
                        renderer: Urfu.renders.htmlEncode
                    }
                ]
            });

            //var items = [
            //    gridPanel
            //];

            //Urfu.createViewport('border', items);

            var settings = {
                tbar: [],
                //autoscroll: true
                overflowY: 'scroll'
            };
            var items = [plainPanel, gridPanel];
            Urfu.createViewport('anchor', items, settings);
        });

        function showDebts(debts) {
            //alert(debts);
            var msg = """";
            for (var i = 0; i < debts.length; i++) {
                msg += Ext.String.format(""{0} семестр; {1} год<br>"", debts[i]");
                WriteLiteral(".term, debts[i].year);\r\n            }\r\n            Ext.MessageBox.show({\r\n                title: \'Долги по семестрам\',\r\n                msg: msg,\r\n            });\r\n        }\r\n    </script>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<SectionFKProperty> Html { get; private set; }
    }
}
#pragma warning restore 1591
