#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "150eb8dea50e6c3e542c7c91ccbde3157463e53e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_EduPrograms_Index), @"mvc.1.0.view", @"/Views/EduPrograms/Index.cshtml")]
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
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Index.cshtml"
using Urfu.Its.Web.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"150eb8dea50e6c3e542c7c91ccbde3157463e53e", @"/Views/EduPrograms/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_EduPrograms_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Index.cshtml"
  
    ViewBag.Title = "Версии образовательных программ";
    Layout = "~/Views/Shared/_ExtLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 175, "\"", 237, 1);
#nullable restore
#line 9 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Index.cshtml"
WriteAttributeValue("", 181, Url.Content("~/Scripts/VersionedDocs/versionedDocs.js"), 181, 56, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@"></script>

    <script type=""text/javascript"">
        Ext.onReady(function() {
            Ext.tip.QuickTipManager.init();

            var store = Ext.create(""Ext.data.BufferedStore"",
            {
                idProperty: 'Id',
                fields: [""Id"", ""DirectionOkso"", ""DirectionTitle"", ""Name"", ""HeadFullName"", ""qualification"", ""DivisionTitle"", ""ChairTitle"", ""Profile"", ""familirizationType"", ""familirizationCondition"", ""Year""],
                autoLoad: true,
                pageSize: 300,
                remoteSort: true,
                remoteFilter: true,
                proxy: {
                    type: 'ajax',
                    url: '/EduPrograms/Index',
                    reader: {
                        type: 'json',
                        rootProperty: 'data',
                        totalProperty: 'total'
                    }
                }
            });

            var prevSettings = {};
            try {
                var prevSettingString = JSON.");
                WriteLiteral(@"parse(localStorage.getItem(""EduProgramFilters"") || ""[]"");

                for (var i = 0; i < prevSettingString.length; i++) {
                    prevSettings[prevSettingString[i][""property""]] = prevSettingString[i][""value""];
                }
            } catch (err) {

            }

            var VariantStateStore = [["""", ""(Не выбрано)""]];
            if (Urfu.ProgramState)
                VariantStateStore = VariantStateStore.concat(Object.keys(Urfu.ProgramState).map(function(key) { return [key, Urfu.ProgramState[key]]; }));


            var fTypeCmbx = Ext.create('Ext.form.ComboBox', {
                xtype: 'combobox',
                header: ""Форма освоения"",
                store: Ext.create('Ext.data.Store', {
                    fields: ['name'],
                    data: [
                        { ""name"": ""Очная"" },
                        { ""name"": ""Очно-заочная"" },
                        { ""name"": ""Заочная"" }
                    ]
                }),
             ");
                WriteLiteral(@"   displayField: 'name',
                value: prevSettings[""familirizationType""] || ""Очная"",
                valueField: 'name',
                mode: 'local',
                editable: false,
                disableKeyFilter: false
            });
            var fCndCmbx = Ext.create('Ext.form.ComboBox', {
                xtype: 'combobox',
                header: ""Условие освоения"",
                store: Ext.create('Ext.data.Store', {
                    fields: ['name'],
                    data: [
                        { ""name"": ""Полный срок"" },
                        { ""name"": ""Сокращенная программа"" },
                        { ""name"": ""Ускоренная программа"" }
                    ]
                }),
                displayField: 'name',
                value: prevSettings[""familirizationCondition""] || ""Полный срок"",
                valueField: 'name',
                mode: 'local',
                editable: false,
                disableKeyFilter: false
            });
");
                WriteLiteral(@"
            var filtersWnd = null;
            var setFilters = function() {
                var settings = [
                    { property: 'DirectionOkso', value: filtersWnd.getComponent(""DirectionOksoField"").getValue() },
                    { property: 'DirectionTitle', value: filtersWnd.getComponent(""DirectionTitleField"").getValue() },
                    { property: 'Name', value: filtersWnd.getComponent(""NameField"").getValue() },
                    { property: 'HeadFullName', value: filtersWnd.getComponent(""HeadFullNameField"").getValue() },
                    { property: 'qualification', value: filtersWnd.getComponent(""qualificationField"").getValue() },
                    { property: 'DivisionTitle', value: filtersWnd.getComponent(""DivisionTitleField"").getValue() },
                    { property: 'ChairTitle', value: filtersWnd.getComponent(""ChairTitleField"").getValue() },
                    { property: 'Profile', value: filtersWnd.getComponent(""ProfileField"").getValue() },
         ");
                WriteLiteral(@"           { property: 'State', value: filtersWnd.getComponent(""StateField"").getValue() },
                    { property: 'familirizationType', value: fTypeCmbx.getRawValue(), verb: 'Equals' },
                    { property: 'familirizationCondition', value: fCndCmbx.getRawValue(), verb: 'Equals' },
                    { property: 'Year', value: filtersWnd.getComponent(""YearField"").getValue() }
                ];
                store.setFilters(settings);
                localStorage.setItem(""EduProgramFilters"", JSON.stringify(settings));
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
                    { fieldLabel: ""ОКСО"", itemId: ""DirectionOksoFie");
                WriteLiteral(@"ld"", value: prevSettings[""DirectionOkso""] },
                    { fieldLabel: ""Название направления"", itemId: ""DirectionTitleField"", value: prevSettings[""DirectionTitle""] },
                    { fieldLabel: ""Название версии ОП"", itemId: ""NameField"", value: prevSettings[""Name""] },
                    { fieldLabel: ""Состояние"", itemId: ""StateField"", xtype: ""combobox"", store: VariantStateStore, value: prevSettings[""State""] },
                    { fieldLabel: ""ФИО руководителя"", itemId: ""HeadFullNameField"", value: prevSettings[""HeadFullName""] },
                    { fieldLabel: ""Уровень обучения"", itemId: ""qualificationField"", value: prevSettings[""qualification""] },
                    { fieldLabel: ""Подразделение"", itemId: ""DivisionTitleField"", value: prevSettings[""DivisionTitle""] },
                    { fieldLabel: ""Кафедра"", itemId: ""ChairTitleField"", value: prevSettings[""ChairTitle""] },
                    { fieldLabel: ""Название профиля"", itemId: ""ProfileField"", value: prevSettings[""Profile""] },");
                WriteLiteral(@"
                    { fieldLabel: ""Год"", itemId: ""YearField"", value: prevSettings[""Year""] }
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

            fTypeCmbx.on('select', setFilters);
            fCndCmbx.on('select', setFilters);

            var tpl = '<a href=""/EduPrograms/EditVariant?id={Id}"">Редактировать версию ОП</a>\
                | <a href=""/EduPrograms/Edit?id={Id}"">Редактировать</a> \
                | <a href=""/EduPrograms/Copy?id={Id}"">Копировать</a>\
                | <a href=""/EduPrograms/CopyAdmissions?programId={Id}"">");
                WriteLiteral(@"Копировать зачисления</a>\
                | <a href=""/Subgroup/Index?programId={Id}"">Подгруппы</a>\
                | <a href=""/EduPrograms/Delete?id={Id}"">Удалить</a>';

            var gridPanel = Ext.create('Ext.grid.Panel',
            {
                region: 'center',
                store: store,
                loadMask: true,
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
                            store.clearFilter();
                            localStorage.setItem(""EduProgramFilters"", ""[]"");
                        }
                    }, fTypeCmbx, fCndCmbx
                ],
                columns: [
                    { xtype: 'rownumber");
                WriteLiteral(@"er', width: 50 },
                    {
                        header: 'ОКСО',
                        dataIndex: 'DirectionOkso',
                        width: 90,
                        renderer: Ext.util.Format.htmlEncode
                    },
                    {
                        header: 'Название направления',
                        dataIndex: 'DirectionTitle',
                        width: 310,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'Название версии ОП',
                        dataIndex: 'Name',
                        width: 190,
                        renderer: Urfu.renders.htmlEncodeWithToolTip
                    },
                    {
                        header: 'ФИО руководителя',
                        dataIndex: 'HeadFullName',
                        width: 230,
                        renderer: Ext.util.Format.htmlEncode
                    }");
                WriteLiteral(@",
                    {
                        header: 'Уровень обучения',
                        align: 'center',
                        dataIndex: 'qualification',
                        width: 210
                    },
                    {
                        header: 'Подразделение',
                        dataIndex: 'DivisionTitle',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 150
                    },
                    {
                        header: 'Кафедра',
                        dataIndex: 'ChairTitle',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 180
                    },
                    {
                        header: 'Название профиля',
                        dataIndex: 'Profile',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 190
                    },
                    {
                        header: ");
                WriteLiteral(@"'Год',
                        dataIndex: 'Year',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 70
                    },
                    {
                        header: 'Состояние',
                        dataIndex: 'State',
                        renderer: Ext.util.Format.htmlEncode,
                        width: 120
                    },
                    {
                        xtype: 'templatecolumn',
                        tpl: tpl,
                        sortable: false,
                        width: 800
                    }
                ]
            });

            var items = [
                gridPanel
            ];

            Urfu.createViewport('border', items);
            gridPanel.getStore().on('load', function (store, records, options) {
                var focus = '");
#nullable restore
#line 252 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\EduPrograms\Index.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591