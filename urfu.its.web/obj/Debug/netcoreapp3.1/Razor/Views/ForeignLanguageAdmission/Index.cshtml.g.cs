#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b253c88454c10d5ef49525700df5d538dff22d7e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ForeignLanguageAdmission_Index), @"mvc.1.0.view", @"/Views/ForeignLanguageAdmission/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b253c88454c10d5ef49525700df5d538dff22d7e", @"/Views/ForeignLanguageAdmission/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ForeignLanguageAdmission_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.ForeignLanguageCompetitionGroup>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
  
    ViewBag.Title = "Зачисление на Конкурсную группу " + Model.ToString();
    Layout = "~/Views/Shared/_ExtLayout.cshtml";
    var filterName = "ForeignLanguageCompetitionGroupsForAdmissionsFilters";

#line default
#line hidden
#nullable disable
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n    <script type=\"text/javascript\">\r\n        \r\n        function downloadReport() {\r\n            var fileUrl = \'");
#nullable restore
#line 12 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
                      Write(Url.Action("DownloadCompetitionGroupReport"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\'\r\n                .concat(\'?filter=\' + encodeURIComponent(localStorage.getItem(\'");
#nullable restore
#line 13 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
                                                                         Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"') || ""[]""));
            window.location.href = fileUrl;
            return false;
        }

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
                pageSize: 300,
                remoteSort: true,
                remoteFilter: true,
                proxy: {
                    type: 'ajax',
                    url: '");
#nullable restore
#line 39 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
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
#line 51 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
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
                    { property: 'shortTitle', value: filtersWnd.getComponent(""title"").getValue() },                    
                    { property: 'number', value: filtersWnd.getComponent(""number"").getValue() },                    
                ];
                store.setFilters(settings);
                localStorage.setItem(""");
#nullable restore
#line 67 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
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
                    { fieldLabel: ""Название модуля"", itemId: ""title"", value: prevSettings[""shortTitle""] },
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
                        ha");
                WriteLiteral(@"ndler: function () { filtersWnd.hide(); }
                    }
                ]
            });

            setFilters();


            var tpl = '<a href=""/ForeignLanguageAdmission/CompetitionGroupStudents/{Id}"">Студенты</a>';

            var sendButton;
");
#nullable restore
#line 105 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
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
                                      ");
                WriteLiteral(" waitConfig: { interval: 200 }\r\n                                   });\r\n                                \r\n                                   Ext.Ajax.request({\r\n                                       url: \'");
#nullable restore
#line 130 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
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
                                       error: function(response) {
                                           Ext.MessageBox.hide();
                                       }
                                    
                                   });

                                   publishAdmWnd.hide();
                               }
                           },
                  ");
                WriteLiteral(@"         {
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
#line 166 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
                       
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
#line 183 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
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
                        handler: function() {
                            window.location = ""/ForeignLanguageAdmission/PrepareAuto?competitionGroupId=");
#nullable restore
#line 191 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageAdmission\Index.cshtml"
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
                        header: 'Номер',
                        align: 'left',
                        dataIndex: 'number',
                        sortable: false,
                        width: 150,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Короткое название модуля',
                        dataIndex: 'shortTitle',
                        width: 500,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Зачетные единицы',
                        align: 'right',
                        dataIndex: 'testUnits',
               ");
                WriteLiteral(@"         width: 200,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
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
                        header: 'Зачислено',
                        align: 'right',
                        dataIndex: 'addmission',
                        width: 150,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
               ");
                WriteLiteral(@"         xtype: 'templatecolumn',
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.ForeignLanguageCompetitionGroup> Html { get; private set; }
    }
}
#pragma warning restore 1591
