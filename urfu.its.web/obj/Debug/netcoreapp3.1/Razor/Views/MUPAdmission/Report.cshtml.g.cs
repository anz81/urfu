#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\Report.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "110acae040ee43be38967cb9f5a6a79cc0b215ab"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_MUPAdmission_Report), @"mvc.1.0.view", @"/Views/MUPAdmission/Report.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"110acae040ee43be38967cb9f5a6a79cc0b215ab", @"/Views/MUPAdmission/Report.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_MUPAdmission_Report : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.Controllers.MinorReportVM>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\Report.cshtml"
  
    ViewBag.Title = "Отчет по подгруппам Модеуса по зачисленным студентам";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">
        Ext.onReady(function() {
            Ext.Ajax.setTimeout(1200000);
            Ext.tip.QuickTipManager.init();


            function parseJson(json) {
                var data = JSON.parse(json.replace(/&quot;/g, '""'));
                return data;
            }

            var dataYears = parseJson('");
#nullable restore
#line 20 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\Report.cshtml"
                                  Write(ViewBag.Years);

#line default
#line hidden
#nullable disable
                WriteLiteral("\');\r\n\r\n            var yearsStore = Ext.create(\"Ext.data.Store\",\r\n                {\r\n                    data: dataYears\r\n\r\n                });\r\n\r\n            var dataSemesters = parseJson(\'");
#nullable restore
#line 28 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\Report.cshtml"
                                      Write(ViewBag.Semesters);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"');
            var SemestersStore = Ext.create(""Ext.data.Store"",
                {
                    data: dataSemesters
                });

            var store = Ext.create(""Ext.data.Store"",
                {
                    fields: [
                        ""CompetitionGroupName"", ""MUP"", ""GroupName"", ""Surname"", ""Name"", ""PatronymicName"",
                        ""StudentStatus"",""SubgroupName"",""Description"",""Teacher""
                    ],
                    groupField: 'CompetitionGroupName',
                    autoLoad: false,
                    //pageSize: 300,
                    remoteSort: true,
                    remoteFilter: true,

                    proxy: {
                        type: 'ajax',
                        url: window.location.pathname ,
                        reader: {
                            type: 'json',
                            rootProperty: 'data',
                            totalProperty: 'total'
                        },
         ");
                WriteLiteral("               timeout: 1200000\r\n                    }\r\n                });\r\n\r\n            var defaultYear = ");
#nullable restore
#line 58 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\Report.cshtml"
                          Write(DateTime.Now.Month < 7? DateTime.Now.Year -1 : DateTime.Now.Year);

#line default
#line hidden
#nullable disable
                WriteLiteral(";\r\n            var prevSettings = { Year: defaultYear, semesterId:");
#nullable restore
#line 59 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\Report.cshtml"
                                                           Write(DateTime.Now.Month < 7?2:1);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"};

            try {
                var prevSettingString = JSON.parse(localStorage.getItem(""MUPReportFilters"") || ""[]"");

                for (var i = 0; i < prevSettingString.length; i++) {
                    prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
                }
            }
            catch (err) {
                console.log(err);
            }

            var semesterCmbx = Ext.create('Ext.form.ComboBox',
                {
                    xtype: 'combobox',
                    fieldLabel: 'Семестр',
                    store: SemestersStore,
                    value: prevSettings[""semesterId""] || """);
#nullable restore
#line 77 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\Report.cshtml"
                                                      Write(DateTime.Now.Month < 7?2:1);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""",
                    valueField: 'Id',
                    displayField: 'Name',
                    queryMode: 'local',
                    editable: false,
                    disableKeyFilter: false,
                    width: 300
                });

            var yearCmbx = Ext.create('Ext.form.ComboBox',
                {
                    xtype: 'combobox',
                    fieldLabel: 'Год',
                    store: yearsStore,
                    value: prevSettings[""Year""] || defaultYear,
                    valueField: 'Id',
                    displayField: 'Name',
                    queryMode: 'remote',
                    editable: false,
                    disableKeyFilter: false,
                    width: 200
                });

            var filtersWnd = null;

            function loadStore() {
                store.proxy.setUrl(window.location.pathname +
                    '?filter=' +
                    encodeURIComponent(localStorage.getItem(");
                WriteLiteral(@"""MUPReportFilters"")));
                store.load();
            }

            var setFilters = function() {
                var settings = [
                    {property: 'MUP', value: filtersWnd.getComponent(""MUPNameField"").getValue()},
                    { property: 'CompetitionGroupName', value: filtersWnd.getComponent(""CompetitionGroupNameField"").getValue() },
                    { property: 'GroupName', value: filtersWnd.getComponent(""GroupNameField"").getValue() },
                    { property: 'Surname', value: filtersWnd.getComponent(""SurnameField"").getValue() },
                    { property: 'Name', value: filtersWnd.getComponent(""NameField"").getValue() },
                    { property: 'PatronymicName', value: filtersWnd.getComponent(""PatronymicNameField"").getValue() },
                    {property:'SubgroupName',value:filtersWnd.getComponent(""SubgroupNameField"").getValue() },
                    { property: 'semesterId', value: semesterCmbx.getValue() || 0, verb: 'Equals' },
");
                WriteLiteral(@"                    { property: 'Year', value: yearCmbx.getRawValue() || defaultYear, verb: 'Equals' },
                    { property: 'Description', value: filtersWnd.getComponent(""DescriptionField"").getValue() }
                ];

                localStorage.setItem(""MUPReportFilters"", JSON.stringify(settings));
                loadStore();
            };

            var grouping = Ext.create('Ext.grid.feature.Grouping',
                {
                    ftype: 'grouping',
                    collapsible: true,
                    startCollapsed: true,
                    hideGroupedHeader: true,
                    groupHeaderTpl: '{name} / Год: {[values.rows[0].data.Year]} / Семестр: {[values.rows[0].data.Semester]} ',
                    enableGroupingMenu:false
                });

            filtersWnd = Ext.create('Ext.window.Window',
                {
                    title: ""Фильтры"",
                    closeAction: 'hide',
                    resizable: false,
  ");
                WriteLiteral(@"                  autoHeight: true,
                    bodyPadding: 6,
                    defaults: {
                        xtype: 'textfield',
                        width: 500
                    },
                    items: [
                        { fieldLabel: ""МУП"", itemId: ""MUPNameField"", value: prevSettings[""MUPName""] },
                        { fieldLabel: ""Конкурсная группа"", itemId: ""CompetitionGroupNameField"", value: prevSettings[""CompetitionGroupName""] },
                        { fieldLabel: ""Название группы"", itemId: ""GroupNameField"", value: prevSettings[""GroupName""] },
                        { fieldLabel: ""Фамилия"", itemId: ""SurnameField"", value: prevSettings[""Surname""] },
                        { fieldLabel: ""Имя"", itemId: ""NameField"", value: prevSettings[""Name""] },
                        { fieldLabel: ""Отчество"", itemId: ""PatronymicNameField"", value: prevSettings[""PatronymicName""] },
                        { fieldLabel: ""Подгруппа"", itemId: ""SubgroupNameField"", value");
                WriteLiteral(@": prevSettings[""SubgroupName""] },
                        { fieldLabel: ""Комментарий"", itemId: ""DescriptionField"", value: prevSettings[""Description""] },
                        yearCmbx,
                        semesterCmbx
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

            var gridPanel = null;

            var downloadReport = Ext.create('Ext.Button',
                {
                    xtype: 'button',
                    text: 'Отчет по МУПам',
                    handler: function() {
                        var ");
                WriteLiteral("fileUrl = \'");
#nullable restore
#line 184 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\Report.cshtml"
                                  Write(Url.Action("DownloadReport"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' +
                            ""?filter="" +
                            encodeURIComponent(localStorage.getItem(""MUPReportFilters"") || ""[]"");
                        window.location.href = fileUrl;
                    }
                });

            gridPanel = Ext.create('Ext.grid.Panel',
                {
                    store: store,
                    region: 'center',
                    columnLines: true,
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
                                localStorage.setItem(""MUPReportFilters"", JSON.stringify([
                                    { property: 'semesterId', value");
                WriteLiteral(@": semesterCmbx.getValue(), verb: 'Equals' },
                                    { property: 'Year', value: yearCmbx.getRawValue(), verb: 'Equals' },
                                ]));
                                store.proxy.setUrl(window.location.pathname + '?filter='+encodeURIComponent(localStorage.getItem(""MUPReportFilters"")));
                                store.load();
                            }
                        }, '-',
                        {
                            xtype: 'button',
                            text: ""Развернуть\\Свернуть группировку"",
                            handler: function() {
                                window.groupsCollapsed = !window.groupsCollapsed;
                                if (window.groupsCollapsed)
                                    grouping.expandAll();
                                else
                                    grouping.collapseAll();
                            }
                            //text: 'Свора");
                WriteLiteral(@"чивание',
                            //menu: [
                            //    {
                            //        text: 'Collapse all',
                            //        handler: 'collapseAll'
                            //    },
                            //    {
                            //        text: 'Expand all',
                            //        handler: 'expandAll'
                            //    }

                            //    ]
                        },
                        '-',
                        downloadReport
                    ],
                    columns: [
                        { xtype: 'rownumberer', width: 50 },
                        {
                            header: 'МУП',
                            dataIndex: 'MUP',
                            width: 400,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Название гр");
                WriteLiteral(@"уппы',
                            dataIndex: 'GroupName',
                            width: 110,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Фамилия',
                            dataIndex: 'Surname',
                            width: 155,
                            renderer: function(value, metaData, record) {
                                return '<a href=""/PersonalInfo/Student?studentId=' + record.data.Id + '"">' + value + '</a>';
                            }
                        },
                        {
                            header: 'Имя',
                            dataIndex: 'Name',
                            width: 170,
                            renderer: Urfu.renders.htmlEncode
                        },
                        {
                            header: 'Отчество',
                            dataIndex: 'PatronymicName',
                ");
                WriteLiteral(@"            width: 150,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Статус студента',
                            dataIndex: 'StudentStatus',
                            align: 'center',
                            width: 125
                        },
                        {
                            header: 'Подгруппа',
                            dataIndex: 'SubgroupName',
                            align: 'center',
                            width: 450
                        },
                        {
                            header: 'Комментарий',
                            dataIndex: 'Description',
                            align: 'center',
                            width: 125
                        },
                        {
                            header: 'Преподаватель',
                            dataIndex: 'Teacher',
                        ");
                WriteLiteral(@"    align: 'center',
                            width: 125
                        }
                    ],
                    features: [grouping]
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
            WriteLiteral("\r\n\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.Controllers.MinorReportVM> Html { get; private set; }
    }
}
#pragma warning restore 1591