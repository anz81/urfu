#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "77b883e18d5c23f0b1a0ff82db27e2f26770a31b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Index), @"mvc.1.0.view", @"/Views/Account/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"77b883e18d5c23f0b1a0ff82db27e2f26770a31b", @"/Views/Account/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\Index.cshtml"
  
    ViewBag.Title = "Пользователи";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">
        Ext.onReady(function () {
            Ext.tip.QuickTipManager.init();

            var store = Ext.create(""Ext.data.BufferedStore"",
                {
                    idProperty: 'UserName',
                    fields: [""Id"", ""UserName"", ""FirstName"", ""LastName"", ""Patronymic"", ""AdName"", ""Email"", ""Divisions"", ""AllDirections"", ""Directions"", ""AllMinors"", ""Minors"",""RoleSets""],
                    autoLoad: true,
                    pageSize: 6000,
                    remoteSort: true,
                    remoteFilter: true,
                    proxy: {
                        type: 'ajax',
                        url: '/Account/Index',
                        reader: {
                            type: 'json',
                            rootProperty: 'data',
                            totalProperty: 'total'
                        }
                    }
                });

            var filtersWnd = Ext.create('Ext.window.Window', {
   ");
                WriteLiteral(@"             title: ""Фильтры"",
                closeAction: 'hide',
                resizable: false,
                autoHeight: true,
                bodyPadding: 6,
                defaults: {
                    xtype: 'textfield',
                    width: 500
                },
                items: [
                    { fieldLabel: ""Имя пользователя"", itemId: ""UserNameField"" },
                    { fieldLabel: ""Имя"", itemId: ""FirstNameField"" },
                    { fieldLabel: ""Фамилия"", itemId: ""LastNameField"" },
                    { fieldLabel: ""Отчество"", itemId: ""PatronymicField"" },
                    { fieldLabel: ""Логин в ActiveDirectory"", itemId: ""AdNameField"" },
                    { fieldLabel: ""Email"", itemId: ""EmailField"" }
                ],
                buttons: [
                    {
                        text: ""OK"",
                        handler: function () {
                            store.filter([
                                { property: 'Use");
                WriteLiteral(@"rName', value: filtersWnd.getComponent(""UserNameField"").getValue() },
                                { property: 'FirstName', value: filtersWnd.getComponent(""FirstNameField"").getValue() },
                                { property: 'LastName', value: filtersWnd.getComponent(""LastNameField"").getValue() },
                                { property: 'Patronymic', value: filtersWnd.getComponent(""PatronymicField"").getValue() },
                                { property: 'AdName', value: filtersWnd.getComponent(""AdNameField"").getValue() },
                                { property: 'Email', value: filtersWnd.getComponent(""EmailField"").getValue() }
                            ]);
                            filtersWnd.hide();
                        }
                    },
                    {
                        text: ""Отмена"",
                        handler: function () { filtersWnd.hide(); }
                    }
                ]
            });

            var tpl = '<a href=""/Acco");
                WriteLiteral(@"unt/Edit/{Id}"">Свойство</a>\
                | <a href=""/Account/UserRoles/{Id}"">Роли</a> | <a href=""/Account/UserDirections/{Id}"">Направления</a> | <a href=""/Account/UserMinors/{Id}"">Майноры</a>\
                | <a href=""/Account/UserDivisions/{Id}"">Институты</a> | <a href=""/Account/Delete/{Id}"">Удалить</a> | <a href=""/Account/Impersonate/{Id}"">Войти</a>';

            var gridPanel = Ext.create('Ext.grid.Panel',
                {
                    region: 'center',
                    store: store,
                    loadMask: true,
                    tbar: [
                        {
                            xtype: 'button',
                            text: 'Фильтры...',
                            handler: function () { filtersWnd.show(); }
                        },
                        {
                            xtype: 'button',
                            text: ""Отменить фильтры"",
                            handler: function () { store.clearFilter(); }
              ");
                WriteLiteral(@"          },
                        {   
                            xtype: 'button',
                            text: ""Экспорт в Excel"",
                            handler: function () { window.location = ""/Account/DownloadReport""; }
                        }
                    ],
                    columns: [
                        { xtype: 'rownumberer', width: 50 },
                        {
                            header: 'Имя пользователя',
                            dataIndex: 'UserName',
                            width: 170,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Фамилия',
                            dataIndex: 'LastName',
                            width: 140,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        },
                        {
                            header: 'Имя',
                        ");
                WriteLiteral(@"    dataIndex: 'FirstName',
                            width: 140,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        },
                        {
                            header: 'Отчество',
                            dataIndex: 'Patronymic',
                            width: 140,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Логин в ActiveDirectory',
                            align: 'center',
                            dataIndex: 'AdName',
                            width: 210
                        },
                        {
                            header: 'Email',
                            dataIndex: 'Email',
                            renderer: Ext.util.Format.htmlEncode,
                            width: 180
                        },
                        {
                            header: 'Роли',
   ");
                WriteLiteral(@"                         dataIndex: 'RoleSets',
                            renderer: Urfu.renders.htmlEncodeWithToolTip,
                            width: 180
                        },
                       {
                           dataIndex: 'Directions',
                           header: 'Направления',
                           width: 180,
                           renderer: function (value, p, record) {
                               value = value.join("", "");
                               if (record.data.AllDirections)
                                   value = 'Все';
                               p.tdAttr = 'data-qtip=""' + Ext.String.htmlEncode(value) + '""';
                               return value;
                           }
                       },
                       {
                           dataIndex: 'Minors',
                           header: 'Майноры',
                           width: 180,
                           renderer: function (value, p, recor");
                WriteLiteral(@"d) {
                               if (record.data.AllMinors)
                                   return 'Все';
                               return value;
                           }
                       },
                       {
                           dataIndex: 'Divisions',
                           header: 'Подразделения',
                           width: 180,
                           renderer: function (value, p, record) {
                               if (record.data.AllDirections)
                                   return 'Все';
                               return value;
                           }
                       },
                       {
                           xtype: 'templatecolumn',
                           tpl: tpl,
                           sortable: false,
                           width: 570
                       }
                    ]
                });

            var items = [
                gridPanel
            ];

      ");
                WriteLiteral("      Urfu.createViewport(\'border\', items);\r\n            gridPanel.getStore().on(\'load\', function (store, records, options) {\r\n                var focus = \'");
#nullable restore
#line 189 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\Account\Index.cshtml"
                        Write(ViewBag.Focus);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"';
                if (focus && focus.length > 0) {
                    
                    var focusRow = store.findExact('Id', focus);
                    if (focusRow > 0) {
                        var rowData = store.getAt(focusRow);
                        gridPanel.getView().focusRow(rowData);
                        gridPanel.getSelectionModel().select(rowData);
                    }
                }
                return false;
            });
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
