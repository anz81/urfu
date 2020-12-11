#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\SynopticReport.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4e6b6ab8b6d2d8468518f85695b4dd2a8bd2a93b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_SectionFKAdmission_SynopticReport), @"mvc.1.0.view", @"/Views/SectionFKAdmission/SynopticReport.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4e6b6ab8b6d2d8468518f85695b4dd2a8bd2a93b", @"/Views/SectionFKAdmission/SynopticReport.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_SectionFKAdmission_SynopticReport : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<Urfu.Its.Web.Controllers.MinorReportVM>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\SynopticReport.cshtml"
  
    ViewBag.Title = "Отчет по секциям ФК Сводный";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">

        Ext.onReady(function () {
            Ext.tip.QuickTipManager.init();
            
            Ext.define('ComboBoxModel',
            {
                extend: 'Ext.data.Model',
                fields:
                [
                    { type: 'string', name: 'Id' },
                    { type: 'string', name: 'Name' }
                ]
            });

            function parseJson(json) {
                var data = JSON.parse(json.replace(/&quot;/g, '""'));
                return data;
            }
            
            var dataSemesters = parseJson('");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\SynopticReport.cshtml"
                                      Write(ViewBag.Semesters);

#line default
#line hidden
#nullable disable
                WriteLiteral("\');\r\n            var SemestersStore = Ext.create(\"Ext.data.Store\",\r\n                {\r\n                    data: dataSemesters\r\n                });\r\n\r\n            var dataYears = parseJson(\'");
#nullable restore
#line 34 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\SynopticReport.cshtml"
                                  Write(ViewBag.Years);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"');
            var yearsStore = Ext.create(""Ext.data.Store"",
                {
                    data: dataYears,

                });
           
            var store = Ext.create(""Ext.data.Store"",
            {
                fields: [
                    ""CompetitionGroupName"", ""moduleName"", ""GroupName"", ""Surname"", ""Name"", ""PatronymicName"", ""Rating"", ""IsTarget"", ""IsInternational"",
                    ""Compensation"", ""MinStudentsCount"", ""MaxStudentsCount"", ""specialities"", ""StudentStatus""
                ],
                groupField: 'CompetitionGroupName',
                //autoLoad: true,
                //pageSize: 300,
                remoteSort: true,
                remoteFilter: true,
                autoLoad: false,
                proxy: {
                    type: 'ajax',
                    url: window.location.pathname + window.location.search,
                    reader: {
                        type: 'json',
                        rootProperty: 'data',
         ");
                WriteLiteral("               totalProperty: \'total\'\r\n                    },\r\n                    timeout: 1200000\r\n\r\n                }\r\n            });\r\n            var defaultYear = ");
#nullable restore
#line 65 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\SynopticReport.cshtml"
                          Write(DateTime.Now.Month < 7? DateTime.Now.Year -1 : DateTime.Now.Year);

#line default
#line hidden
#nullable disable
                WriteLiteral(";\r\n\r\n            var prevSettings = {Year:defaultYear,semesterId:");
#nullable restore
#line 67 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\SynopticReport.cshtml"
                                                        Write(DateTime.Now.Month < 7?2:1);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"};

            try {
                var prevSettingString = JSON.parse(localStorage.getItem(""SectionFKSynopticReportFilters"") || ""[]"");

                for (var i = 0; i < prevSettingString.length; i++) {
                    prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
                }
            } catch (err) {

            }
            var semesterCmbx = Ext.create('Ext.form.ComboBox',
           {
               xtype: 'combobox',
               //header: ""Семестр"",
               fieldLabel: 'Семестр',
               store: SemestersStore,
               value: prevSettings[""semesterId""] || ");
#nullable restore
#line 84 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\SynopticReport.cshtml"
                                                Write(DateTime.Now.Month < 7?2:1);

#line default
#line hidden
#nullable disable
                WriteLiteral(@",
               valueField: 'Id',
               displayField: 'Name',
               queryMode: 'remote',
               editable: false,
               disableKeyFilter: false,
               width: 230
           });
            var yearCmbx = Ext.create('Ext.form.ComboBox',
          {
              xtype: 'combobox',
              //header: ""Год"",
              fieldLabel: 'Год',
              store: yearsStore,
              value: prevSettings[""Year""] || defaultYear,
              valueField: 'Id',
              displayField: 'Name',
              queryMode: 'remote',
              editable: false,
              disableKeyFilter: false,
              width: 190
          });
            var filtersWnd = null;
            function loadStore() {
                store.proxy.setUrl(window.location.pathname +
                    '?filter=' +
                    encodeURIComponent(localStorage.getItem(""SectionFKSynopticReportFilters"")));
                store.load();
            ");
                WriteLiteral(@"}
            var setFilters = function() {
                var settings = [
                    { property: 'CompetitionGroupName', value: filtersWnd.getComponent(""CompetitionGroupNameField"").getValue() },
                    { property: 'GroupName', value: filtersWnd.getComponent(""GroupNameField"").getValue() },
                    { property: 'moduleName', value: filtersWnd.getComponent(""moduleNameField"").getValue() },
                    { property: 'Surname', value: filtersWnd.getComponent(""SurnameField"").getValue() },
                    { property: 'Name', value: filtersWnd.getComponent(""NameField"").getValue() },
                    { property: 'PatronymicName', value: filtersWnd.getComponent(""PatronymicNameField"").getValue() },
                    { property: 'IsTarget', value: filtersWnd.getComponent(""IsTargetField"").getValue().IsTargetField },
                    { property: 'semesterId', value: semesterCmbx.getValue() || 0, verb: 'Equals' },
                    { property: 'Year', value: ");
                WriteLiteral(@"yearCmbx.getRawValue() || defaultYear, verb: 'Equals' },
                    {
                        property: 'IsInternational',
                        value: filtersWnd.getComponent(""IsInternationalField"").getValue().IsInternationalField
                    }
                ];
                //store.setFilters(settings);
                localStorage.setItem(""SectionFKSynopticReportFilters"", JSON.stringify(settings));
                loadStore();
            };

            var grouping = Ext.create('Ext.grid.feature.Grouping',
            {
                ftype: 'grouping',
                collapsible: true,
                startCollapsed: true,
                hideGroupedHeader: true,
                groupHeaderTpl: '{name} / Лимит нижний {[values.rows[0].data.MinStudentsCount]} / Лимит верхний {[values.rows[0].data.MaxStudentsCount]} / Зачислено: {rows.length} / Год: {[values.rows[0].data.Year]} / Семестр: {[values.rows[0].data.Semester]} '
            });

            filtersWnd");
                WriteLiteral(@" = Ext.create('Ext.window.Window',
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
                    { fieldLabel: ""Секция"", itemId: ""moduleNameField"", value: prevSettings[""moduleName""] },
                    { fieldLabel: ""Конкурсная группа"", itemId: ""CompetitionGroupNameField"", value: prevSettings[""CompetitionGroupName""] },
                    { fieldLabel: ""Название группы"", itemId: ""GroupNameField"", value: prevSettings[""GroupName""] },
                    { fieldLabel: ""Фамилия"", itemId: ""SurnameField"", value: prevSettings[""Surname""] },
                    { fieldLabel: ""Имя"", itemId: ""NameField"", value: prevSettings[""Name""] },
                    { fieldLabel: ""Отчество"", itemId: ""PatronymicNameField"", value: pr");
                WriteLiteral(@"evSettings[""PatronymicName""] },
                    { fieldLabel: ""Номер"", itemId: ""PersonalNumber"", value: prevSettings[""PersonalNumber""] },
                    yearCmbx,
                    semesterCmbx,
                    {
                        xtype: 'radiogroup',
                        fieldLabel: 'Целевой',
                        itemId: ""IsTargetField"",
                        value: prevSettings[""IsTarget""],
                        items: [
                            {
                                boxLabel: 'Все',
                                name: 'IsTargetField',
                                inputValue: ''

                            }, {
                                boxLabel: 'Да',
                                name: 'IsTargetField',
                                inputValue: 'true'
                            }, {
                                boxLabel: 'Нет',
                                name: 'IsTargetField',
                                input");
                WriteLiteral(@"Value: 'false'
                            }
                        ]
                    },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: 'Иностранный',
                        itemId: ""IsInternationalField"",
                        value: prevSettings[""IsInternational""],
                        items: [
                            {
                                boxLabel: 'Все',
                                name: 'IsInternationalField',
                                inputValue: ''

                            }, {
                                boxLabel: 'Да',
                                name: 'IsInternationalField',
                                inputValue: 'true'
                            }, {
                                boxLabel: 'Нет',
                                name: 'IsInternationalField',
                                inputValue: 'false'
                            }
                        ]
                WriteLiteral(@"
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

            setFilters();
            //loadStore();
            //semesterCmbx.on('select', setFilters);
            //yearCmbx.on('select', setFilters);
            var gridPanel = null;

            var downloadReport = Ext.create('Ext.Button',
            {
                xtype: 'button',
                text: 'Отчёт по секциям в Excel',
                handler: function() {
                    var fileUrl = '");
#nullable restore
#line 235 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\SectionFKAdmission\SynopticReport.cshtml"
                              Write(Url.Action("DownloadSynopticReport"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' +
                        ""?filter="" +
                        encodeURIComponent(localStorage.getItem(""SectionFKSynopticReportFilters"") || ""[]"");
                    window.location.href = fileUrl;
                }
            });


            gridPanel = Ext.create('Ext.grid.Panel',
            {
                multiSelect: true,
                region: 'center',
                store: store,
                loadMask: true,
                columnLines: true,
                tbar: [
                    {
                        xtype: 'button',
                        text: 'Фильтры...',
                        handler: function() { filtersWnd.show(); }
                    },
                    //semesterCmbx,
                   // yearCmbx,
                  /* {
                        xtype: 'button',
                        text: ""Применить"",
                        handler: function () {
                            setFilters();
                        }
              ");
                WriteLiteral(@"      }*/,
                    {
                        xtype: 'button',
                        text: ""Отменить фильтры"",
                        handler: function () {
                            localStorage.setItem(""SectionFKSynopticReportFilters"", JSON.stringify([
                                { property: 'semesterId', value: semesterCmbx.getValue(), verb: 'Equals' },
                                { property: 'Year', value: yearCmbx.getRawValue(), verb: 'Equals' },
                            ]));

                            //localStorage.setItem(""SectionFKSynopticReportFilters"", JSON.stringify(settings));
                            store.proxy.setUrl(window.location.pathname + '?filter=' + encodeURIComponent(localStorage.getItem(""SectionFKSynopticReportFilters"")));
                            store.load();

                        }
                    },'-',
                    {
                        xtype: 'button',
                        text: ""Развернуть\\Свернуть групп");
                WriteLiteral(@"ировку"",
                        handler: function() {
                            window.groupsCollapsed = !window.groupsCollapsed;
                            if (window.groupsCollapsed)
                                grouping.expandAll();
                            else
                                grouping.collapseAll();
                        }
                    },
                    '-',
                    downloadReport
                ],
                columns: [
                    { xtype: 'rownumberer', width: 50 },
                    {
                        header: 'Секция',
                        dataIndex: 'moduleName',
                        width: 160,
                        renderer: Ext.util.Format.htmlEncode
                    },
                       {
                           header: 'Приоритет',
                           dataIndex: 'priority',
                           width: 130,
                           renderer: Ext.util.Format.htmlEnco");
                WriteLiteral(@"de
                       },
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
                        renderer: function(value, metaData, record) {
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
  ");
                WriteLiteral(@"                      width: 200,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Статус студента',
                        dataIndex: 'StudentStatus',
                        align: 'center',
                        width: 125
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
                        align: 'c");
                WriteLiteral(@"enter',
                        renderer: function(val) { return val ? 'Да' : ""Нет"" }
                    },
                    {
                        header: 'Вид возмещения затрат',
                        align: 'center',
                        dataIndex: 'Compensation',
                        width: 210,
                        renderer: Urfu.renders.htmlEncode
                    },
                    {
                        header: 'Лимит нижний',
                        align: 'center',
                        dataIndex: 'MinStudentsCount',
                        width: 200,
                        renderer: Urfu.renders.htmlEncode
                    },
                    {
                        header: 'Лимит верхний',
                        align: 'center',
                        dataIndex: 'MaxStudentsCount',
                        width: 200,
                        renderer: Urfu.renders.htmlEncode
                    }
                ],
                f");
                WriteLiteral("eatures: [grouping]\r\n            });\r\n            d = null;\r\n            var items = [\r\n                gridPanel\r\n            ];\r\n\r\n            Urfu.createViewport(\'border\', items);\r\n        });\r\n    </script>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<Urfu.Its.Web.Controllers.MinorReportVM>> Html { get; private set; }
    }
}
#pragma warning restore 1591