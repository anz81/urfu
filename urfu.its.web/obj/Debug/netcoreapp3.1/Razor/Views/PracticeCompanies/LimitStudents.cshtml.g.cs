#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeCompanies\LimitStudents.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "af4cd64e60554fde602e93973401889e7f5ff0f7"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_PracticeCompanies_LimitStudents), @"mvc.1.0.view", @"/Views/PracticeCompanies/LimitStudents.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af4cd64e60554fde602e93973401889e7f5ff0f7", @"/Views/PracticeCompanies/LimitStudents.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_PracticeCompanies_LimitStudents : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeCompanies\LimitStudents.cshtml"
  
    ViewBag.Title = ViewBag.Title;
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            WriteLiteral(@"    <div class=""form-horizontal"">
        <script type=""text/javascript"">

            var lastFilter = [];
            Ext.onReady(function () {
                Ext.tip.QuickTipManager.init();

                function parseJson(json) {
                    var data = JSON.parse(json.replace(/&quot;/g, '""'));
                    return data;
                }

                var dataStatus = parseJson('");
#nullable restore
#line 20 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeCompanies\LimitStudents.cshtml"
                                       Write(ViewBag.Status);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"');
                var statusStore = Ext.create(""Ext.data.Store"",
                    {
                        data: dataStatus,

                    });

                var store = Ext.create(""Ext.data.BufferedStore"",
                    {
                        idProperty: 'Id',
                        autoLoad: true,
                        pageSize: 25,
                        remoteSort: true,
                        remoteFilter: true,
                        proxy: {
                            type: 'ajax',
                            url: '/PracticeCompanies/ContractStudents?limitId=");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeCompanies\LimitStudents.cshtml"
                                                                         Write(ViewBag.LimitId);

#line default
#line hidden
#nullable disable
            WriteLiteral("&contractKsId=");
#nullable restore
#line 36 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeCompanies\LimitStudents.cshtml"
                                                                                                       Write(ViewBag.ContractKsId);

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

                var statusFilterCmbx = Ext.create('Ext.form.ComboBox',
                    {
                        xtype: 'combobox',
                        fieldLabel: 'Статус',
                        labelWidth: 50,
                        width: 200,
                        store: statusStore,
                        value: '',
                        valueField: 'StatusValue',
                        displayField: 'StatusName',
                        queryMode: 'local'
                    });

                function setFilters() {
                    settings = [
                        { property: 'Student', value: Ext.ComponentQuery.query('#StudentFilter')[0].getValue() },
                        { p");
            WriteLiteral(@"roperty: 'Group', value: Ext.ComponentQuery.query('#GroupFilter')[0].getValue() },
                        { property: 'StatusName', value: statusFilterCmbx.getValue() }
                    ];
                    store.setFilters(settings);
                }

                var tpl =
                '<a href=\'");
#nullable restore
#line 68 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeCompanies\LimitStudents.cshtml"
                      Write(Url.Action("Practice", "PracticeGroup"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"?id={id}&studentID={StudentId}&groupID={groupId}&year={year}&semesterID={semestrId}&disciplineUID={disciplineUID}\'>Руководители, темы</a>';
                function showEmpDetails(val, meta, rec, rIndex, cIndex, store) {
                return ""<a href='#'>"" + val + ""</a>"";
                //or apply any css class of your choice using
                //meta.css = ‘awesome-font-style’
                //return val;
            }



                var gridPanel = Ext.create('Ext.grid.Panel',
                    {
                        region: 'center',
                        store: store,
                        loadMask: true,
                        columnLines: true,
                        //enableLocking: true,
                        tbar: [
                            {
                                xtype: 'button',
                                text: ""Договоры"",
                                handler: function () {
                                    window.location = ""/Pr");
            WriteLiteral("acticeCompanies/Contracts?id=");
#nullable restore
#line 90 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeCompanies\LimitStudents.cshtml"
                                                                                  Write(ViewBag.CompanyId);

#line default
#line hidden
#nullable disable
            WriteLiteral("&focus=");
#nullable restore
#line 90 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeCompanies\LimitStudents.cshtml"
                                                                                                           Write(ViewBag.ContractId);

#line default
#line hidden
#nullable disable
            WriteLiteral(@""";
                                }
                            }, '-',
                            {
                                xtype: 'textfield',
                                fieldLabel: 'ФИО студента',
                                labelWidth: 105,
                                itemId: 'StudentFilter',
                                width: 400
                            },
                            {
                                xtype: 'textfield',
                                fieldLabel: 'Группа',
                                labelWidth: 50,
                                itemId: 'GroupFilter',
                                width: 200
                            },
                            statusFilterCmbx,
                            {
                                xtype: 'button',
                                text: ""Применить"",
                                handler: function () {
                                    setFilters();
            ");
            WriteLiteral(@"                    }
                            },
                            {
                                xtype: 'button',
                                text: ""Отменить фильтры"",
                                handler: function () {
                                    Ext.ComponentQuery.query('#StudentFilter')[0].setValue('');
                                    Ext.ComponentQuery.query('#GroupFilter')[0].setValue('');
                                    statusFilterCmbx.setValue('');
                                    store.clearFilter();
                                    settings = [];
                                }
                            },
                        ],
                        columns: [
                            { xtype: 'rownumberer', width: 50 },
                            {
                                header: 'Студент',
                                align: 'left',
                                dataIndex: 'Student',
                    ");
            WriteLiteral(@"            width: 300,
                                cellWrap: true,
                              //  locked: true,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                header: 'Группа',
                                align: 'left',
                                dataIndex: 'Group',
                                width: 150,
                                cellWrap: true,
                               // locked: true,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                header: 'Статус',
                                align: 'left',
                                dataIndex: 'StatusName',
                                cellWrap: true,
                                width: 150,
                                renderer: Urfu.renders.htmlEncodeWithTool");
            WriteLiteral(@"Tip
                            },
                            {
                                header: 'Направление',
                                align: 'left',
                                dataIndex: 'Direction',
                                width: 200,
                                cellWrap: true,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                header: 'Образовательная программа',
                                align: 'left',
                                dataIndex: 'Profile',
                                cellWrap: true,
                                width: 300,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                header: 'Название практики',
                                align: 'left',
                                dataIndex:");
            WriteLiteral(@" 'PracticeName',
                                cellWrap: true,
                                width: 300,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                header: 'Тип практики',
                                align: 'left',
                                dataIndex: 'PracticeType',
                                cellWrap: true,
                                width: 200,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                header: 'Сроки проведения',
                                align: 'left',
                                dataIndex: 'Dates',
                                cellWrap: true,
                                width: 200,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },

   ");
            WriteLiteral(@"                         {
                                xtype: 'templatecolumn',
                                tpl: tpl,
                                sortable: false,
                                width: 175
                            }


                        ]
                    });


            var items = [
                gridPanel
            ];

            Urfu.createViewport('border', items);

            });
        </script>
    </div>
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
