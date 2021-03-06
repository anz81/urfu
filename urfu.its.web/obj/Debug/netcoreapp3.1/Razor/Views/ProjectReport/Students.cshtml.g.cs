#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectReport\Students.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ad744f6d586d044c77656fecc22642f9ad5b1fd5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ProjectReport_Students), @"mvc.1.0.view", @"/Views/ProjectReport/Students.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ad744f6d586d044c77656fecc22642f9ad5b1fd5", @"/Views/ProjectReport/Students.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ProjectReport_Students : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectReport\Students.cshtml"
  
    ViewBag.Title = "Отчет по студентам";

    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<script type=\"text/javascript\">\r\n    \r\n    Ext.onReady(function () {\r\n\r\n        Ext.tip.QuickTipManager.init();\r\n\r\n        var filterName = \"ProjectReportStudentsFilters\"\r\n\r\n        var dataYears = Urfu.parseJson(\'");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectReport\Students.cshtml"
                                   Write(ViewBag.Years);

#line default
#line hidden
#nullable disable
            WriteLiteral("\');\r\n        var yearsStore = Ext.create(\"Ext.data.Store\",\r\n            {\r\n                data: dataYears\r\n            });\r\n        var dataSemesters = Urfu.parseJson(\'");
#nullable restore
#line 20 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectReport\Students.cshtml"
                                       Write(ViewBag.Semesters);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"');
        var semestersStore = Ext.create(""Ext.data.Store"",
            {
                data: dataSemesters
            });

        var store = Ext.create(""Ext.data.Store"",
        {
            autoLoad: false,
            remoteSort: false,
            remoteFilter: true,
            proxy: {
                type: 'ajax',
                url: '");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectReport\Students.cshtml"
                 Write(Url.Action("Students"));

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

            if (prevSettings[""Years""] == null || prevSettings[""Years""] == ""[]"") {
                prevSettings[""Years""] = [];
                prevSettings[""Years""].push(parseInt('");
#nullable restore
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectReport\Students.cshtml"
                                                Write(ViewBag.CurrentYear);

#line default
#line hidden
#nullable disable
            WriteLiteral("\', 10));\r\n            }\r\n\r\n            if (prevSettings[\"Semesters\"] == null || prevSettings[\"Semesters\"] == \"[]\") {\r\n                prevSettings[\"Semesters\"] = [];\r\n                prevSettings[\"Semesters\"].push(parseInt(\'");
#nullable restore
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectReport\Students.cshtml"
                                                    Write(ViewBag.CurrentSemester);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"', 10));
            }

        } catch (err) {

        }
        
        var setFilters = function (year, semester, direction, title, student) {
            var settings = [
                { property: 'Years', value: year },
                { property: 'Semesters', value: semester },
                { property: 'Direction', value: direction == undefined ? filtersWnd.getComponent(""Direction"").getValue() : direction },
                { property: 'Title', value: title == undefined ? filtersWnd.getComponent(""Project"").getValue() : title },
                { property: 'Student', value: student == undefined ? filtersWnd.getComponent(""Student"").getValue() : student }
            ];
            store.setFilters(settings);
            localStorage.setItem(filterName, JSON.stringify(settings));
        };

        filtersWnd = Ext.create('Ext.window.Window', {
            title: ""Фильтры"",
            closeAction: 'hide',
            resizable: false,
            autoHeight: true,
         ");
            WriteLiteral(@"   bodyPadding: 6,
            defaults: {
                xtype: 'textfield',
                width: 500
            },
            items: [
                { fieldLabel: ""Направление"", itemId: ""Direction"", value: prevSettings[""Direction""] },
                { fieldLabel: ""Проект"", itemId: ""Project"", value: prevSettings[""Title""] },
                { fieldLabel: ""ФИО студента"", itemId: ""Student"", value: prevSettings[""Student""] }
            ],
            buttons: [
                {
                    text: ""OK"",
                    handler: function () {
                        setFilters(Ext.getCmp(yearCmbxId).getValue(), Ext.getCmp(semesterCmbxId).getValue());
                        filtersWnd.hide();
                    }
                },
                {
                    text: ""Отмена"",
                    handler: function () { filtersWnd.hide(); }
                }
            ]
        });
        setFilters(prevSettings[""Years""], prevSettings[""Semesters""]);

       ");
            WriteLiteral(" function downloadReport() {\r\n            var fileUrl = \'");
#nullable restore
#line 107 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectReport\Students.cshtml"
                      Write(Url.Action("DownloadStudentsReport", "ProjectReport"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"'
                .concat('?filter=' + encodeURIComponent(localStorage.getItem(filterName) || ""[]""));
            window.location.href = fileUrl;
            return false;
        }

        function changeYearSemester(year, semester) {
            if (year.length > 0 && semester.length > 0) {
                setFilters(year, semester);
            }
        }

        var semesterCmbxId = 'ProjectStudentsReportSemesterId';
        var yearCmbxId = 'ProjectStudentsReportYearId';
     
        var gridPanel = Ext.create('Ext.grid.Panel',
        {
            region: 'center',
            store: store,
            loadMask: true,
            columnLines: true,
            dockedItems: [
                {
                    xtype: 'toolbar',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'tagfield',
                            fieldLabel: 'Учебный год',
                            id: yearCmbxId,
         ");
            WriteLiteral(@"                   value: prevSettings[""Years""],
                            store: yearsStore,
                            valueField: 'Year',
                            displayField: 'Year',
                            queryMode: 'local',
                            forceSelection: false,
                            allowBlank: false,
                            listeners: {
                                change: function (t, newValue, oldValue) {
                                    var semester = Ext.getCmp(semesterCmbxId).getValue();
                                    changeYearSemester(newValue, semester);
                                }
                            }
                        },
                        {
                            xtype: 'tagfield',
                            fieldLabel: 'Семестр',
                            id: semesterCmbxId,
                            margin: '0 5 0 5',
                            labelWidth: 65,
                            ");
            WriteLiteral(@"value: prevSettings[""Semesters""],
                            store: semestersStore,
                            valueField: 'Id',
                            displayField: 'Name',
                            queryMode: 'local',
                            forceSelection: false,
                            allowBlank: false,
                            listeners: {
                                change: function (t, newValue, oldValue) {
                                    var year = Ext.getCmp(yearCmbxId).getValue();
                                    changeYearSemester(year, newValue);
                                }
                            }
                        }
                    ]
                },

                {
                    xtype: 'toolbar',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'button',
                            text: 'Фильтры',
                            handler: ");
            WriteLiteral(@"function () { filtersWnd.show(); }
                        },
                        {
                            xtype: 'button',
                            text: ""Отменить фильтры"",
                            handler: function () {
                                setFilters(Ext.getCmp(yearCmbxId).getValue(), Ext.getCmp(semesterCmbxId).getValue(), """", """", """");
                            }
                        }, '-',
                        {
                            xtype: 'button',
                            text: ""Отчет в Excel"",
                            handler: downloadReport
                        }

                    ]
                },
            ],
            columns: [
                { xtype: 'rownumberer', width: 50 },
                {
                    header: 'ФИО',
                    dataIndex: 'FullName',
                    width: 300,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
    ");
            WriteLiteral(@"                header: 'Группа',
                    dataIndex: 'Group',
                    width: 130,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Статус',
                    dataIndex: 'Status',
                    width: 100,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Вид возмещения затрат',
                    dataIndex: 'Compensation',
                    width: 130,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Проектная группа',
                    dataIndex: 'CompetitionGroupShortName',
                    width: 200,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Проект',
                    dataIndex: 'Project',
                    w");
            WriteLiteral(@"idth: 300,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Уровень',
                    align: 'centre',
                    dataIndex: 'Level',
                    width: 110,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Приоритет при выборе',
                    dataIndex: 'Priority',
                    width: 130,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Проектный статус',
                    dataIndex: 'AdmissionStatusName',
                    width: 130,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                },
                {
                    header: 'Роль',
                    dataIndex: 'Role',
                    width: 150,
                    renderer: Urfu.renders.htmlEncodeWithToolTi");
            WriteLiteral(@"p
                },
                {
                    header: 'Подгруппы',
                    dataIndex: 'Subgroups',
                    width: 300,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
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
