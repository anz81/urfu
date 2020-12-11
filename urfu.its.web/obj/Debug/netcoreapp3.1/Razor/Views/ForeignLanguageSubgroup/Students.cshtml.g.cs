#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "830ae2bff612b800bb1d7260f7e992ecdd711c52"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ForeignLanguageSubgroup_Students), @"mvc.1.0.view", @"/Views/ForeignLanguageSubgroup/Students.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"830ae2bff612b800bb1d7260f7e992ecdd711c52", @"/Views/ForeignLanguageSubgroup/Students.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_ForeignLanguageSubgroup_Students : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Web.DataContext.ForeignLanguageSubgroup>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
  
    ViewBag.Title = "Редактирование студентов подгруппы " + Model.Name;
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n    <script type=\"text/javascript\">\r\n        var subgroupId = ");
#nullable restore
#line 10 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
                    Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@";
        var lastFilter;
        Ext.onReady(function() {
            Ext.tip.QuickTipManager.init();
            var hideStudentsStoreName = 'HideNotactiveStudentsFL';

            var store = Ext.create(""Ext.data.Store"",
            {
                idProperty: 'Id',
                fields: [
                    ""Id"", ""GroupName"", ""Surname"", ""Name"", ""PatronymicName"", { name: ""Included"", type: 'bool' },
                    ""AnotherGroup"",
                ],
                autoLoad: true,
                remoteSort:
                    true,
                remoteFilter:
                    true,
                proxy:
                {
                    type: 'ajax',
                    url: '/ForeignLanguageSubgroup/StudentsAjax',
                    extraParams: {
                        id: ");
#nullable restore
#line 33 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
                       Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@",
                        hideStudents: sessionStorage.getItem(hideStudentsStoreName) || false
                    },
                    reader:
                    {
                        type: 'json',
                        rootProperty:
                            'data',
                        totalProperty:
                            'total'
                    }
                },
                listeners: {
                    update: function(self, record, operation, modifiedFieldNames, details, eOpts) {
                        if (operation !== Ext.data.Model.EDIT) {
                            //self.sourcerejectChanges();
                            return;
                        }
                        if (modifiedFieldNames.length !== 1 || modifiedFieldNames[0] !== 'Included') {
                            return;
                        }

                    }
                }
            });

            function studentsMembership(include, similarsubgroup)");
                WriteLiteral(@" {
                var title = include ? ""Зачисление"" : ""Удаление зачислений"";
                Ext.MessageBox.wait(title);
                setTimeout(store.each(function(record, index) {
                        if (!record.get('AnotherGroup')) {
                            record.set(""Included"", include);
                        }
                    }),
                    0);
                Ext.Ajax.request({
                    url: '/ForeignLanguageSubgroup/StudentsMembership',
                    method: 'POST',
                    params: {
                        include: include,
                        subgroupId: subgroupId,
                        filter: JSON.stringify(lastFilter),
                        similarsubgroup: similarsubgroup || false
                    },
                    success: function(response) {
                        var data = Ext.decode(response.responseText);
                        if (response.status !== 200 || data.reload)
                      ");
                WriteLiteral(@"      window.location.reload();
                        Ext.MessageBox.hide();
                        if (data.msg && data.msg.length > 0) {
                            Ext.MessageBox.show({
                                title: 'Информационное сообщение',
                                msg: data.msg,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.INFO // иконка мб {ERROR,INFO,QUESTION,WARNING}
                                //width:300,                       // есть еще minWidth
                                //closable:false,                  // признак наличия икнки закрытия окна
                            });
                        }
                        if (data.success)
                            store.commitChanges();
                    },
                    complete: function(response) {
                        Ext.MessageBox.hide();
                    }
                });
            }

            f");
                WriteLiteral(@"unction checkMemberships() {
                Ext.Ajax.request({
                    url: '/ForeignLanguageSubgroup/CheckMemberships',
                    params: {
                        subgroupId: subgroupId
                    },
                    success: function (response) {
                        var data = Ext.decode(response.responseText);
                        if (data.msg && data.msg.length > 0) {
                            Ext.MessageBox.show({
                                title: 'Информационное сообщение',
                                msg: 'Удалить студентов из подгрупп по  ' + data.msg + '?',
                                buttons: Ext.MessageBox.YESNO,
                                fn: function (button) {
                                    if (button == 'yes') {                                        
                                        studentsMembership(included = false, withanothersubgroup = true)
                                    }
                   ");
                WriteLiteral(@"                 else if(button == 'no'){
                                        studentsMembership(false)      
                                    }
                                }
                            }) 
                        }
                        else studentsMembership(false);
                    },
                    failure: function (response) {
                        Ext.MessageBox.alert('', 'Ошибка запроса');
                    }
                })                   
            }

            var levelStore = Ext.create('Ext.data.Store',
                {
                    fields: [""Level""],
                    proxy: {
                        type: 'memory',
                        reader: {
                            type: 'json'
                        }
                    }
                });
            levelStore.loadData(");
#nullable restore
#line 142 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
                           Write(Html.Raw(Json.Encode(ViewBag.Levels)));

#line default
#line hidden
#nullable disable
                WriteLiteral(@");
        
            
            var filtersWnd = Ext.create('Ext.window.Window',
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
                    { fieldLabel: ""Группа"", itemId: ""GroupName"" },
                    { fieldLabel: ""Фамилия"", itemId: ""Surname"" },
                    { fieldLabel: ""Имя"", itemId: ""Name"" },
                    { fieldLabel: ""Отчество"", itemId: ""PatronymicName"" },
                    { fieldLabel: """", itemId: ""ForeignLanguageLevel"" },
                    {
                      xtype: 'combobox',
                      itemId: 'ForeignLanguageLevel',
                      fieldLabel: ""Уровень"",
                      store: levelStore,
                      valueField: 'Level',
");
                WriteLiteral(@"                      displayField: 'Level',
                      queryMode: 'local',
                      editable: false,
                      disableKeyFilter: false
                    }
                ],
                buttons: [
                    {
                        text: ""OK"",
                        handler: function() {
                            lastFilter = [
                                { property: 'Surname', value: filtersWnd.getComponent(""Surname"").getValue() },
                                { property: 'GroupName', value: filtersWnd.getComponent(""GroupName"").getValue() },
                                { property: 'Name', value: filtersWnd.getComponent(""Name"").getValue() },
                                { property: 'PatronymicName',value: filtersWnd.getComponent(""PatronymicName"").getValue() },
                                { property: 'ForeignLanguageLevel', value: filtersWnd.getComponent(""ForeignLanguageLevel"").getValue() }

                            ");
                WriteLiteral(@"];
                            store.filter(lastFilter);
                            filtersWnd.hide();
                        }
                    },
                    {
                        text: ""Отмена"",
                        handler: function() { filtersWnd.hide(); }
                    }
                ]
            });

            var gridPanel = Ext.create('Ext.grid.Panel',
            {
                region: 'center',
                store: store,
                loadMask: true,
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
       ");
                WriteLiteral(@"                         text: ""Отменить фильтры"",
                                handler: function () {
                                    store.clearFilter();
                                    lastFilter = [];
                                }
                            },
                            {
                                xtype: 'button',
                                text: ""Скрыть\\показать включенных студентов"",
                                handler: function () {
                                    window.showIncluded = !window.showIncluded;
                                    if (window.showIncluded) {
                                        store.filter('Included', true);
                                        lastFilter.Included = true;
                                    } else {
                                        store.clearFilter();
                                        lastFilter = [];
                                    }
                            ");
                WriteLiteral(@"    }
                            },
                            {
                                xtype: 'button',
                                text: ""Список подгрупп"",
                                handler: function () { window.location = ""/ForeignLanguageSubgroup/Index?focus=");
#nullable restore
#line 237 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
                                                                                                          Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral("&competitionGroupId=");
#nullable restore
#line 237 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
                                                                                                                                       Write(Model.Meta.CompetitionGroupId);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"""; }
                            },
                            {
                                xtype: 'button',
                                text: ""Зачислить оставшихся"",
                                handler: function () {
                                    studentsMembership(true);
                                }
                            },
                            {
                                xtype: 'button',
                                text: ""Удалить зачисления"",
                                handler: function () {
                                    //studentsMembership(false);
                                    checkMemberships();
                                }
                            },
                            {
                                xtype: 'button',
                                text: ""Экспорт в Excel"",
                                handler: function () {
                                    window.location = ""/ForeignLanguageSu");
                WriteLiteral("bgroup/DownloadSubgroupReport?subgroupId=");
#nullable restore
#line 258 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
                                                                                                             Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""";
                                }
                            }
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
                                        store.reload();
                                    }
             ");
                WriteLiteral(@"                   }
                            }
                        ]
                    }
                ],
                columns: [
                    { xtype: 'rownumberer', width: 50 },
                    {
                        header: 'Группа',
                        dataIndex: 'GroupName',
                        width: 150,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Фамилия',
                        dataIndex: 'Surname',
                        width: 200,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Имя',
                        dataIndex: 'Name',
                        width: 200,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Отчество',
                        dataIndex:");
                WriteLiteral(@" 'PatronymicName',
                        width: 200,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Статус',
                        dataIndex: 'Status',
                        width: 150,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Баллы',
                        dataIndex: 'ForeignLanguageRating',
                        width: 120,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Уровень',
                        dataIndex: 'ForeignLanguageLevel',
                        width: 120,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },

                    {
                        header: 'В подгруппе',
                        dataIndex: 'Includ");
                WriteLiteral(@"ed',
                        width: 150,
                        xtype: 'checkcolumn',

                        listeners: {
                            beforecheckchange: function(column, recordIndex) {
                                var record = store.getAt(recordIndex);

                                var included = record.get('Included');
                                var studentId = record.get('Id');
                                var subgroupId = ");
#nullable restore
#line 340 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
                                            Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@";

                                Ext.Ajax.request({
                                    url: '/ForeignLanguageSubgroup/StudentMembership',
                                    method: 'POST',
                                    params: {
                                        include: !included,
                                        studentId: studentId,
                                        subgroupId: subgroupId
                                    },
                                    success: function(response) {
                                        var data = Ext.decode(response.responseText);
                                        if (response.status === 200)
                                            record.commit();
                                        else
                                            record.reject();
                                        if (data.msg && data.msg.length > 0) {
                                            Ext.MessageBox.show({
          ");
                WriteLiteral(@"                                      title: 'Информационное сообщение',
                                                msg: data.msg,
                                                buttons: Ext.MessageBox.OK,
                                                icon: Ext.MessageBox.INFO // иконка мб {ERROR,INFO,QUESTION,WARNING}
                                                //width:300,                       // есть еще minWidth
                                                //closable:false,                  // признак наличия икнки закрытия окна
                                            });
                                        }
                                        if (data.rmer && data.subgroupId) {
                                            Ext.MessageBox.show({
                                                title: 'Информационное сообщение',
                                                msg: 'Удалить данного студента из подгруппы по ' + data.rmer + '?',
                          ");
                WriteLiteral(@"                      buttons: Ext.MessageBox.YESNO,
                                                fn: function (button) {
                                                    if (button == 'yes') {
                                                        Ext.Ajax.request({
                                                            url: '/ForeignLanguageSubgroup/StudentMembership',
                                                            params: {
                                                                include: !included,
                                                                studentId: studentId,
                                                                subgroupId: data.subgroupId,
                                                            },
                                                            success: function (response) {                                                               
                                                                var data = Ex");
                WriteLiteral(@"t.decode(response.responseText);                                                                    
                                                                if (data.msg.length ==0) 
                                                                    Ext.MessageBox.alert('Информация', 'Студент удален из подгруппы');
                                                            },
                                                            failure: function (response) {
                                                                Ext.MessageBox.alert('Ошибка', 'Не удалось удалить студента из подгруппы');
                                                            }

                                                        })
                                                    }
                                                },
                                                icon: Ext.MessageBox.QUESTION
                                            })
                                       ");
                WriteLiteral(@" }

                                    }
                                });
                            }
                        }
                    },
                    {
                        header: 'Зачислен в подгруппу',
                        align: 'center',
                        dataIndex: 'AnotherGroup',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 600
                    }
                ],
                selModel: {
                    selType: 'cellmodel'
                },
                plugins: [
                    Ext.create('Ext.grid.plugin.CellEditing',
                    {
                        clicksToEdit: 1,
                        listeners: {
                            beforeedit:function(editor, ctx, options) {
                                var record = ctx.record;
                                if (ctx.field === 'Score' && !record.get('Included'))
                                    retur");
                WriteLiteral(@"n false;
                                return true;
                            },
                            edit: function(editor, ctx, options) {
                                var record = ctx.record;

                                if (ctx.field !== 'Score')
                                    return true;

                                if (record.get('AnotherGroup') !== null) {
                                    record.reject();
                                    return false;
                                }
                                var studentId = record.get(""Id"");
                                var subgroupId = ");
#nullable restore
#line 434 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\ForeignLanguageSubgroup\Students.cshtml"
                                            Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@";
                            }
                        }
                    })
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Web.DataContext.ForeignLanguageSubgroup> Html { get; private set; }
    }
}
#pragma warning restore 1591