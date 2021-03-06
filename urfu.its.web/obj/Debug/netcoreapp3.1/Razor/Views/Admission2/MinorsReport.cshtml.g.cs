#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Admission2\MinorsReport.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "855b0019d989843a0e2f96daaa41a08a1b76cb79"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Admission2_MinorsReport), @"mvc.1.0.view", @"/Views/Admission2/MinorsReport.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"855b0019d989843a0e2f96daaa41a08a1b76cb79", @"/Views/Admission2/MinorsReport.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Admission2_MinorsReport : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<Urfu.Its.Web.Controllers.MinorReportVM>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Admission2\MinorsReport.cshtml"
  
    ViewBag.Title = "Отчет по майнорам";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n    <script type=\"text/javascript\">\r\n        \r\n        Ext.onReady(function () {\r\n            function showSpecialities(id) {\r\n                Ext.Ajax.request({\r\n                    url: \'");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Admission2\MinorsReport.cshtml"
                     Write(Url.Action("GetSpecialities","Admission2"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?moduleId=' + id,
                    success: function (response) {
                        var data = Ext.decode(response.responseText);
                        Ext.MessageBox.alert('Направления', data.specialities, function () {
                            //action to complete when user clicks ok.
                        });
                    }
                });
            }
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

            // Amir: Это как то крыво но другого варианта не было...
            Ext.define('Override.grid.feature.Grouping', {
                override: 'Ext.grid.feature.Grouping',

                vetoEvent: function (record, row, rowIndex, e) {
                    // Do not veto mouse");
                WriteLiteral(@"over/mouseout or mousedown if clicks on a Link
                    if ((e.type === 'mousedown' || e.type === 'mouseover') && e.getTarget('a') && e.getTarget('a').innerText.indexOf(""Направления"") > -1) {
                        showSpecialities(record.data.moduleId);
                        return true;
                    }
                }
            });

            var SemestersStore = Ext.create('Ext.data.Store',
            {
                model: 'ComboBoxModel',
                proxy:
                {
                    type: 'ajax',
                    url: '/Minors/Semesters',
                    reader: { type: 'json', root: 'data' }
                }
            });
            var store = Ext.create(""Ext.data.Store"",
            {
                fields: [
                    ""Id"", ""GroupName"", ""Surname"", ""Name"", ""PatronymicName"", ""Rating"", ""IsTarget"", ""IsInternational"",
                    ""Compensation"", ""MinStudentsCount"", ""MaxStudentsCount"", ""specialities""
         ");
                WriteLiteral(@"       ],
                groupField: 'numberAndTitle',
                autoLoad: false,
                //pageSize: 300,
                remoteSort: true,
                remoteFilter: true,
                
                proxy: {
                    type: 'ajax',
                    url: window.location.pathname + window.location.search,
                    reader: {
                        type: 'json',
                        rootProperty: 'data',
                        totalProperty: 'total'
                    }
                }
            });
            var prevSettings = {};
            try {
                var prevSettingString = JSON.parse(localStorage.getItem(""MinorsReportFilters"") || ""[]"");

                for (var i = 0; i < prevSettingString.length; i++) {
                    prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
                }
            } catch (err) {

            }

            var filtersWnd = null;
         ");
                WriteLiteral(@"   function loadStore() {
                store.proxy.setUrl(window.location.pathname + window.location.search +
                    '?filter=' +
                    encodeURIComponent(localStorage.getItem(""MinorsReportFilters"")));
                store.load();
            }
            var setFilters = function() {
                var settings = [
                    { property: 'number', value: filtersWnd.getComponent(""number"").getValue() },
                    { property: 'GroupName', value: filtersWnd.getComponent(""GroupNameField"").getValue() },
                    { property: 'Surname', value: filtersWnd.getComponent(""SurnameField"").getValue() },
                    { property: 'Name', value: filtersWnd.getComponent(""NameField"").getValue() },
                    { property: 'semesterId', value: filtersWnd.getComponent(""semester"").getValue() },
                    { property: 'PatronymicName', value: filtersWnd.getComponent(""PatronymicNameField"").getValue() },
                    { property:");
                WriteLiteral(@" 'IsTarget', value: filtersWnd.getComponent(""IsTargetField"").getValue().IsTargetField },
                    { property: 'Year', value: filtersWnd.getComponent(""Year"").getValue() },
                    {
                        property: 'IsInternational',
                        value: filtersWnd.getComponent(""IsInternationalField"").getValue().IsInternationalField
                    }
                ];
                localStorage.setItem(""MinorsReportFilters"", JSON.stringify(settings));
                loadStore();
            };
            var grouping = Ext.create('Ext.grid.feature.Grouping',
            {
                ftype: 'grouping',
                collapsible: true,
                startCollapsed: true,
                hideGroupedHeader: true,
                groupHeaderTpl: '{name} / Лимит нижний {[values.rows[0].data.MinStudentsCount]} / Лимит верхний {[values.rows[0].data.MaxStudentsCount]} / Зачислено: {rows.length} / <a>Направления</a>'
            });

            filt");
                WriteLiteral(@"ersWnd = Ext.create('Ext.window.Window',
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
                    { fieldLabel: ""Номер модуля"", itemId: ""number"", value: prevSettings[""number""] },
                    { fieldLabel: ""Название группы"", itemId: ""GroupNameField"", value: prevSettings[""GroupName""] },
                    { fieldLabel: ""Фамилия"", itemId: ""SurnameField"", value: prevSettings[""Surname""] },
                    { fieldLabel: ""Имя"", itemId: ""NameField"", value: prevSettings[""Name""] },
                    { fieldLabel: ""Номер"", itemId: ""PersonalNumber"", value: prevSettings[""PersonalNumber""] },
                    { fieldLabel: ""Год"", itemId: ""Year"", value: prevSettings[""Year""] },
                    { field");
                WriteLiteral(@"Label: ""Отчество"", itemId: ""PatronymicNameField"", value: prevSettings[""PatronymicName""] },
                    {
                        fieldLabel: ""Семестр"",
                        itemId: ""semester"",
                        value: prevSettings[""semesterId""],
                        xtype: ""combobox"",
                        store: SemestersStore,
                        valueField: 'Id',
                        displayField: 'Name',
                        queryMode: 'remote'
                    },
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
                         ");
                WriteLiteral(@"       boxLabel: 'Да',
                                name: 'IsTargetField',
                                inputValue: 'true'
                            }, {
                                boxLabel: 'Нет',
                                name: 'IsTargetField',
                                inputValue: 'false'
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
");
                WriteLiteral(@"                                inputValue: 'true'
                            }, {
                                boxLabel: 'Нет',
                                name: 'IsInternationalField',
                                inputValue: 'false'
                            }
                        ]
                    }
                ],
                buttons: [
                    {
                        text: ""OK"",
                        handler: function() {
                            //store.filter([
                            //    { property: 'GroupName', value: filtersWnd.getComponent(""GroupNameField"").getValue() },
                            //    { property: 'Surname', value: filtersWnd.getComponent(""SurnameField"").getValue() },
                            //    { property: 'Name', value: filtersWnd.getComponent(""NameField"").getValue() },
                            //    { property: 'StudentStatus', value: filtersWnd.getComponent(""StudentStatus"").getValue() },
         ");
                WriteLiteral(@"                   //    { property: 'PersonalNumber', value: filtersWnd.getComponent(""PersonalNumber"").getValue() },
                            //    { property: 'PatronymicName', value: filtersWnd.getComponent(""PatronymicNameField"").getValue() },
                            //    { property: 'IsTarget', value: filtersWnd.getComponent(""IsTargetField"").getValue() },
                            //    { property: 'IsInternational', value: filtersWnd.getComponent(""IsInternationalField"").getValue() }
                            //]);
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
         ");
                WriteLiteral("       xtype: \'button\',\r\n                text: \'Отчёт по майнорам в Excel\',\r\n                handler: function() {\r\n                    var fileUrl = \'");
#nullable restore
#line 233 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Admission2\MinorsReport.cshtml"
                              Write(Url.Action("DownloadMinorsReport"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' +
                        ""?filter="" +
                        encodeURIComponent(localStorage.getItem(""MinorsReportFilters"") || ""[]"");
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
                    {
                        xtype: 'button',
                        text: ""Отменить фильтры"",
                        handler: function() {
                            localStorage.setItem(""MinorsReportFilters"", []);
                            store.clearFilter();
                  ");
                WriteLiteral(@"      }
                    },
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
                    },
                    '-',
                    downloadReport
                ],
                columns: [
                    { xtype: 'rownumberer', width: 50 },
                    {
                        header: 'Год',
                        dataIndex: 'Year',
                        width: 80,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Название группы',
       ");
                WriteLiteral(@"                 dataIndex: 'GroupName',
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
                        width: 200,
                        renderer: Ext.util.Format.htmlEncode
                ");
                WriteLiteral(@"    },
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
                        align: 'center',
                        dataIndex: 'Compensation',
                        width: 210,
                   ");
                WriteLiteral(@"     renderer: Urfu.renders.htmlEncode
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
                features: [grouping]
            });
            d = null;
            var items = [
                gridPanel
            ];

            Urfu.createViewport('border', items);
        });
    </script>
");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<Urfu.Its.Web.Controllers.MinorReportVM>> Html { get; private set; }
    }
}
#pragma warning restore 1591
