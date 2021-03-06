#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c3ce7953d98d66165cefb1f8b655a78cdaeebaf0"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_PracticeOrders_Numbers), @"mvc.1.0.view", @"/Views/PracticeOrders/Numbers.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c3ce7953d98d66165cefb1f8b655a78cdaeebaf0", @"/Views/PracticeOrders/Numbers.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_PracticeOrders_Numbers : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml"
  
    ViewBag.Title = "Номера приказов";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            WriteLiteral(@"    <div class=""form-horizontal"">
        <script type=""text/javascript"">

            Ext.onReady(function () {
                Ext.tip.QuickTipManager.init();

                var store = Ext.create(""Ext.data.Store"",
                    {
                        idProperty: 'Id',
                        fields: [
                            ""Id"", ""Year"" ,""Number"", ""DecreeDate""
                        ],
                        autoLoad: true,
                        proxy: {
                            type: 'ajax',
                            url: '");
#nullable restore
#line 23 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml"
                             Write(Url.Action("GetNumbers"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                            reader: {
                                type: 'json'
                            }
                        }
                    });

                function required(value) {
                    return (value != undefined && value.length > 0) ? true : ""Поле не может быть пустым"";
                }

                function checkEditRole(actions) {
                    if ('");
#nullable restore
#line 35 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml"
                    Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' == 'True') {
                        actions();
                    }
                    else {
                        Ext.MessageBox.alert('Ошибка', ""У вас нет прав вносить изменения в справочник"");
                    }
                }

                var editWnd = Ext.create('Ext.window.Window',
                    {
                        title: ""Номер приказа"",
                        closeAction: 'hide',
                        resizable: false,
                        autoHeight: true,
                        bodyPadding: 6,
                        rowHeight: 100,
                        items: [{
                            xtype: 'hidden',
                            itemId: 'Id',
                        },{
                            xtype: 'numberfield',
                            fieldLabel: 'Год*',
                            labelWidth: 140,
                            itemId: 'Year',
                            width: 300,
                            minValue:");
            WriteLiteral(@" 2000,
                            maxValue: 3000,
                            allowDecimals: false,
                            validator: required
                        },{
                            xtype: 'textfield',
                            fieldLabel: 'Номер приказа*',
                            labelWidth: 140,
                            itemId: 'Number',
                            width: 500,
                            maxLength:20,
                            validator: required
                        }, {
                            xtype: 'datefield',
                            fieldLabel: 'Дата приказа*',
                            labelWidth: 140,
                            itemId: 'DecreeDate',
                            format: 'd.m.Y',
                            width: 500,
                            startDay: 1
                        }, {
                            xtype: 'textfield',
                            fieldLabel: 'Номер приказа во изменение");
            WriteLiteral(@"*',
                            labelWidth: 140,
                            itemId: 'ChangeDecreeNumber',
                            width: 500,
                            maxLength: 20,
                            validator: required
                        }, {
                            xtype: 'datefield',
                            fieldLabel: 'Дата приказа во изменение*',
                            labelWidth: 140,
                            itemId: 'ChangeDecreeDate',
                            format: 'd.m.Y',
                            width: 500,
                            startDay: 1
                        }],

                        buttons: [{
                            //id: 'btnSaveCountry',
                            text: ""Сохранить"",
                            handler: function () {
                                var isValid = true;
                                editWnd.items.items.forEach(function (element, index, array) {
                             ");
            WriteLiteral("       if (!element.isValid()) isValid = false;\r\n                                });\r\n                                if (!isValid) return false;\r\n\r\n                                Ext.Ajax.request({\r\n                                        url: \'");
#nullable restore
#line 109 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml"
                                         Write(Url.Action("EditNumber"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                        params: {
                                            id: editWnd.getComponent(""Id"").getValue() ? editWnd.getComponent(""Id"").getValue() : 0,
                                            year: editWnd.getComponent(""Year"").getValue(),
                                            number: editWnd.getComponent(""Number"").getValue(),
                                            decreeDate: editWnd.getComponent(""DecreeDate"").getValue(),
                                            changeDecreeNumber: editWnd.getComponent(""ChangeDecreeNumber"").getValue(),
                                            changeDecreeDate: editWnd.getComponent(""ChangeDecreeDate"").getValue(),
                                        },
                                        success: function (response) {
                                            if (response.responseText != """") {
                                                var data = Ext.decode(response.responseText);
                  ");
            WriteLiteral(@"                              if (data.success) {
                                                    editWnd.hide();
                                                    store.reload();
                                                }
                                                else
                                                {
                                                    Ext.MessageBox.alert('Ошибка', data.message);
                                                }
                                            }
                                        },
                                        failure: function (response) {
                                            Ext.MessageBox.alert('Ошибка', 'Неизвестная ошибка');
                                        }
                                    });
                            }
                        },{
                            text: ""Отмена"",
                            handler: function () { editWnd.hide(); }
        ");
            WriteLiteral(@"                }]
                    });

                var gridPanel = Ext.create('Ext.grid.Panel',
                    {
                        region: 'center',
                        store: store,
                        loadMask: true,
                        columnLines: true,
                        tbar: [{
                            xtype: 'button',
                            text: ""Добавить"",
                            handler: function () {
                                checkEditRole(function () {
                                    editWnd.items.items.forEach(function (element, index, array) {
                                        element.setValue("""");
                                        element.clearInvalid();
                                    });
                                    editWnd.show();
                                });
                            }
                        }],
                        columns: [{
                            x");
            WriteLiteral(@"type: 'rownumberer', width: 50
                        }, {
                            header: 'Год',
                            align: 'right',
                            dataIndex: 'Year',
                            width: 100,
                            cellWrap: true,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            header: 'Номер приказа',
                            align: 'right',
                            dataIndex: 'Number',
                            width: 300,
                            cellWrap: true,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            //xtype: 'datecolumn',
                            header: 'Дата приказа',
                            align: 'right',
                            dataIndex: 'DecreeDate',
                            width: 300,
                                cellWrap: true,
  ");
            WriteLiteral(@"                              startDay: 1,

                            //format: 'd.m.Y'
                        }, {
                            header: 'Номер приказа во изменение',
                            align: 'right',
                            dataIndex: 'ChangeDecreeNumber',
                            width: 300,
                            cellWrap: true,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
                        }, {
                            //xtype: 'datecolumn',
                            header: 'Дата приказа во изменение',
                            align: 'right',
                            dataIndex: 'ChangeDecreeDate',
                            width: 300,
                            cellWrap: true,
                            startDay: 1,

                            //format: 'd.m.Y'
                        }, {
                            xtype: 'actioncolumn',
                            region: 'center',
        ");
            WriteLiteral("                    sortable: false,\r\n                            width: 70,\r\n                            items: [{\r\n                                icon: \'");
#nullable restore
#line 210 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml"
                                  Write(Url.Content("~/Content/Images/edit.png"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                iconCls: 'icon-padding',
                                text: 'Редактировать',
                                tooltip: 'Редактировать',
                                handler: function (grid, rowIndex, colIndex) {
                                    checkEditRole(function () {
                                        var rec = grid.getStore().getAt(rowIndex);
                                        editWnd.items.items.forEach(function (element, index, array) {
                                        //if (rec.data[element.itemId] != undefined)  ???
                                              element.setValue(rec.data[element.itemId]);
                                        });
                                        editWnd.show();
                                    });
                                }
                            }, {
                                icon: '");
#nullable restore
#line 225 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml"
                                  Write(Url.Content("~/Content/Images/remove.png"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                iconCls: 'icon-padding',
                                text: 'Удалить',
                                tooltip: 'Удалить',
                                handler: function (grid, rowIndex, colIndex) {
                                    checkEditRole(function () {
                                        var request = function () {
                                                var rec = grid.getStore().getAt(rowIndex);
                                                Ext.Ajax.request({
                                                    url: '");
#nullable restore
#line 234 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml"
                                                     Write(Url.Action("RemoveNumber"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                                                    params: {
                                                        id: rec.get(""Id"")
                                                    },
                                                    success: function (response) {
                                                        if (response.responseText != """") {
                                                            var data = Ext.decode(response.responseText);
                                                            if (data.success) {
                                                                store.reload();
                                                            }
                                                            else {
                                                                Ext.MessageBox.alert('Ошибка', data.message);
                                                            }
                                                        }
                 ");
            WriteLiteral(@"                                   },
                                                    failure: function (response) {
                                                        Ext.MessageBox.alert('Ошибка', 'Неизвестная ошибка');
                                                    }
                                                });
                                            };

                                        Ext.MessageBox.show({
                                            title: 'Информационное сообщение',
                                            msg: ""Удалить номер приказа из списка? Больше вы не сможете формировать приказы ниииикоооогда-а-а-а!"",
                                            buttons: Ext.MessageBox.YESNO,
                                            fn: function (btn) {
                                                if (btn === 'yes') {
                                                    request();
                                                }
              ");
            WriteLiteral(@"                              }
                                        });
                                    });
                                }
                            }]
                        }
                        ]

                    });

                var items = [
                    gridPanel
                ];

                Urfu.createViewport('border', items);

                gridPanel.getStore().on('load', function(store, records, options) {
                    var focus = '");
#nullable restore
#line 280 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\PracticeOrders\Numbers.cshtml"
                            Write(ViewBag.Focus);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"';
                    if (focus && focus.length > 0) {

                        var focusRow = store.findExact('Id', parseInt(focus));
                        if (focusRow >= 0) {
                            var rowData = store.getAt(focusRow);
                            gridPanel.getView().focusRow(rowData);
                            gridPanel.getSelectionModel().select(rowData);
                        }
                    }
                    return false;
                });

            });
        </script>
    </div>
");
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
