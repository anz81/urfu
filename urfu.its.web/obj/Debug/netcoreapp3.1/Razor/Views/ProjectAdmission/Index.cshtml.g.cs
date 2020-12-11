#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "db2fa3511464afc0ef0014e39beb67e85b45d26a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ProjectAdmission_Index), @"mvc.1.0.view", @"/Views/ProjectAdmission/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"db2fa3511464afc0ef0014e39beb67e85b45d26a", @"/Views/ProjectAdmission/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ProjectAdmission_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.ProjectCompetitionGroup>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
  
    ViewBag.Title = "Зачисление на Проектную группу " + Model.ToString();
    Layout = "~/Views/Shared/_ExtLayout.cshtml";
    var filterName = "ProjectCompetitionGroupsForAdmissionsFilters";

#line default
#line hidden
#nullable disable
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">

        Ext.onReady(function () {
            Ext.tip.QuickTipManager.init();

            Ext.define('ComboBoxModel', {
                extend: 'Ext.data.Model',
                fields:
                [
                    { type: 'string', name: 'Id' },
                    { type: 'string', name: 'Name' }
                ]
            });

            var store = Ext.create(""Ext.data.BufferedStore"",
            {
                fields: [""Id"", ""number"", ""title"",""type"", ""testUnits"", ""limit"", ""selection"", ""addmission""],
                autoLoad: true,
                pageSize: 25,
                remoteSort: true,
                remoteFilter: true,
                proxy: {
                    type: 'ajax',
                    url: '");
#nullable restore
#line 32 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
                     Write(Url.Action("Index",new {competitionGroupId = Model.Id}));

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
                var prevSettingString = JSON.parse(localStorage.getItem(""");
#nullable restore
#line 44 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
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
            var setFilters = function () {
                var settings = [
                    { property: 'title', value: filtersWnd.getComponent(""title"").getValue() },
                    { property: 'number', value: filtersWnd.getComponent(""number"").getValue() },
                ];
                store.setFilters(settings);
                localStorage.setItem(""");
#nullable restore
#line 60 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
                                 Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""", JSON.stringify(settings));
            };

            filtersWnd = Ext.create('Ext.window.Window', {
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
                    { fieldLabel: ""Название проекта"", itemId: ""title"", value: prevSettings[""title""] },
                    { fieldLabel: ""Номер"", itemId: ""number"", value: prevSettings[""number""] },
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
                        handle");
                WriteLiteral("r: function () { filtersWnd.hide(); }\r\n                    }\r\n                ]\r\n            });\r\n\r\n            setFilters();\r\n            var sendButton;\r\n\r\n            var tpl = \'<a href=\"/ProjectAdmission/CompetitionGroupStudents/{Id}\">Студенты</a>\';\r\n");
#nullable restore
#line 96 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
             if (ViewBag.IsInMassPublishRole) {
                

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
                   var publishAdmWnd = Ext.create('Ext.window.Window', {
                       title: ""Отправить зачисления в ЛК"",
                       closeAction: 'hide',
                       resizable: false,
                       autoHeight: true,
                       bodyPadding: 6,
                       items: [
                           { xtype:""box"", html: ""Отправить все результаты зачислений в ЛК студента?"" },

                       ],
                       buttons: [
                           {
                               text: ""OK"",
                               handler: function () {
                                   Ext.MessageBox.show({
                                       msg: 'Отправка сообщений, пожалуйста подождите...',
                                       progressText: 'Отправка...',
                                       width: 300,
                                       wait: true,
                                       waitConfig: { interval: 20");
                WriteLiteral("0 }\r\n                                   });\r\n\r\n                                   Ext.Ajax.request({\r\n                                       url: \'");
#nullable restore
#line 121 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
                                        Write(Url.Action("PublishCompetitionGroupAdmissions",new {competitionGroupId = Model.Id}));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' ,
                                       params: { },
                                       success: function (response) {
                                           if (response.status === 200) {
                                               Ext.toast({ html: ""Сообщения отправлены"", align: 't' });
                                           } else {
                                               Ext.toast({ html: ""Сообщения не отправлены"", align: 't' });
                                           }
                                           Ext.MessageBox.hide();
                                       },
                                       failure: function(response) {
                                           Ext.MessageBox.hide();
                                           Ext.toast({ html: ""Ошибка. Повторите попытку позже"", align: 't' });
                                       }

                                   });



                                   publishAdmWnd.hide();
 ");
                WriteLiteral(@"                              }
                           },
                           {
                               text: ""Отмена"",
                               handler: function () { publishAdmWnd.hide(); }
                           }
                       ]
                   });


                sendButton = Ext.create('Ext.Button',
                {
                    xtype: 'button',
                    //disabled: true,
                    text: 'Отправить зачисления в ЛК',
                    handler: function() {
                        publishAdmWnd.show();
                    }
                });
                ");
#nullable restore
#line 160 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
                       
            }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"

            var gridPanel = Ext.create('Ext.grid.Panel',
            {
                region: 'center',
                store: store,
                tbar: [
                    {
                        xtype: 'button',
                        text: 'Фильтры...',
                        handler: function () { filtersWnd.show(); }
                    },
                    {
                        xtype: 'button',
                        text: ""Отменить фильтры"",
                        handler: function() {
                            localStorage.setItem(""");
#nullable restore
#line 178 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
                                             Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""", []);
                            store.clearFilter();
                        }
                    },
                    {
                        xtype: 'button',
                        text: ""Перейти к автоматическому зачислению"",
                        disabled: '");
#nullable restore
#line 185 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
                              Write(ViewBag.CanAutoAdmit);

#line default
#line hidden
#nullable disable
                WriteLiteral("\' == \'False\',\r\n                        handler: function () {\r\n                            window.location = \"/ProjectAdmission/PrepareAuto?competitionGroupId=");
#nullable restore
#line 187 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ProjectAdmission\Index.cshtml"
                                                                                           Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""";
                        }
                    },
                    '-',
                    sendButton


                ],
                loadMask: true,
                columns: [
                    { xtype: 'rownumberer', width: 50 },
                    {
                        header: 'Уровень',
                        dataIndex: 'Level',
                        width: 110,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Название проекта',
                        dataIndex: 'title',
                        width: 300,
                        cellWrap: true,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Зачетные единицы',
                        align: 'right',
                        dataIndex: 'testUnits',
                        width: 120,
                        rende");
                WriteLiteral(@"rer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Лимит',
                        align: 'right',
                        dataIndex: 'limit',
                        width: 150,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Кол-во заявлений',
                        align: 'right',
                        dataIndex: 'selection',
                        width: 150,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Зачислено всего',
                        align: 'right',
                        dataIndex: 'addmission',
                        width: 130,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Свободно мест',
          ");
                WriteLiteral(@"              align: 'right',
                        dataIndex: 'vacancy',
                        width: 130,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },

                    {
                        xtype: 'templatecolumn',
                        sortable: false,
                        tpl: tpl,
                        width: 420
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
            WriteLiteral("\r\n\r\n\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.ProjectCompetitionGroup> Html { get; private set; }
    }
}
#pragma warning restore 1591
