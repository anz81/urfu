#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b13407017e7340693cbc8f3858770056649e2c2b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_MUPAdmission_CompetitionGroupStudents), @"mvc.1.0.view", @"/Views/MUPAdmission/CompetitionGroupStudents.cshtml")]
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
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
using Urfu.Its.Common;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
using Urfu.Its.Web.DataContext;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b13407017e7340693cbc8f3858770056649e2c2b", @"/Views/MUPAdmission/CompetitionGroupStudents.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"571aa92c4ca967469f2915635495580dde4274ca", @"/Views/_ViewImports.cshtml")]
    public class Views_MUPAdmission_CompetitionGroupStudents : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MUPProperty>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 5 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
  
    Layout = "~/Views/Shared/_ExtLayout.cshtml";
    var filterName = "MUPCompetitionGroupStudentsFilters";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n<script type=\"text/javascript\">\r\n    \r\n        var statuses =\r\n            ");
#nullable restore
#line 15 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
       Write(Html.Raw(Json.Serialize(EnumHelper<AdmissionStatus>.GetValues(AdmissionStatus.Admitted).Select(m => new {Value = m, Text = EnumHelper<AdmissionStatus>.GetDisplayValue(m)}).ToList())));

#line default
#line hidden
#nullable disable
                WriteLiteral(@";
        var statesStore = Ext.create('Ext.data.Store',
        {
            fields: ['Value', 'Text'],
            data: statuses
        });

        Ext.onReady(function() {
            Ext.tip.QuickTipManager.init();

            var canEdit = '");
#nullable restore
#line 25 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                      Write(ViewBag.CanEdit);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' == 'True';

            var hideStudentsStoreName = 'hideStudentsMUPAdmission';
            var store = Ext.create(""Ext.data.BufferedStore"",
            {
                fields: [
                    ""Id"", ""GroupName"", ""Surname"", ""Name"", ""PatronymicName"", ""Rating"", ""IsTarget"", ""IsInternational"", ""Compensation"",
                    ""Priority"", ""Status"", ""PersonalNumber"", ""StudentStatus"", ""VariantId"",
                    ""Published"", ""Modified"", ""OtherAdmission"", ""studentSelection""
                ],
                autoLoad: false,
                pageSize: 300,
                remoteSort: true,
                remoteFilter: true,
                proxy: {
                    type: 'ajax',
                    url: '");
#nullable restore
#line 41 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                     Write(Url.Action("CompetitionGroupStudentsAjax"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                    extraParams: {\r\n                        id: \'");
#nullable restore
#line 43 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                        Write(ViewBag.MUPPeriodId);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                        hideStudents: sessionStorage.getItem(hideStudentsStoreName) || false
                    },
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
#line 56 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
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
            function loadStore() {
                store.proxy.setUrl('");
#nullable restore
#line 67 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                               Write(Url.Action("CompetitionGroupStudentsAjax"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\' +\r\n                    \'?filter=\' +\r\n                    encodeURIComponent(localStorage.getItem(\"");
#nullable restore
#line 69 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                                                        Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""")));
                store.load();
            }
            var setFilters = function() {
                var settings = [
                    { property: 'GroupName', value: filtersWnd.getComponent(""GroupNameField"").getValue() },
                    { property: 'Surname', value: filtersWnd.getComponent(""SurnameField"").getValue() },
                    { property: 'Name', value: filtersWnd.getComponent(""NameField"").getValue() },
                    { property: 'StudentStatus', value: filtersWnd.getComponent(""StudentStatusField"").getValue() },
                    { property: 'Status', value: filtersWnd.getComponent(""StatusField"").getValue() },
                    { property: 'PersonalNumber', value: filtersWnd.getComponent(""PersonalNumber"").getValue() },
                    { property: 'PatronymicName', value: filtersWnd.getComponent(""PatronymicNameField"").getValue() }
                ];
                localStorage.setItem(""");
#nullable restore
#line 82 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                                 Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""", JSON.stringify(settings));
                loadStore();
            };

        filtersWnd = Ext.create('Ext.window.Window',
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
                { fieldLabel: ""Название группы"", itemId: ""GroupNameField"", value: prevSettings[""GroupName""] },
                { fieldLabel: ""Фамилия"", itemId: ""SurnameField"", value: prevSettings[""Surname""] },
                { fieldLabel: ""Имя"", itemId: ""NameField"", value: prevSettings[""Name""] },
                { fieldLabel: ""Статус"", itemId: ""StudentStatusField"", value: prevSettings[""StudentStatus""] },
                { fieldLabel: ""Номер"", itemId: ""PersonalNumber"", value: prevSettings[""PersonalNumber""] },
                { fieldLabel: ""Отчество"", itemId: ""PatronymicNameField"", val");
                WriteLiteral(@"ue: prevSettings[""PatronymicName""] },
                {
                    fieldLabel: ""Состояние"",
                    itemId: ""StatusField"",
                    value: prevSettings[""Status""],
                    xtype: ""combobox"",
                    store: statesStore,
                    valueField: 'Value',
                    displayField: 'Text',
                    queryMode: 'local'

                }
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

        var selRecord = [];
        var gridPanel = null;

        function setAdmissionStatus(rec, status) {
            var request = fu");
                WriteLiteral("nction() {\r\n                Ext.Ajax.request({\r\n                    url: \'");
#nullable restore
#line 139 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                     Write(Url.Action("SetCompetitionGroupAdmissionStatus", new {propertyId = Model.Id,}));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                    params: {\r\n                        studentIds: rec.map(function(l) { return l.get(\"Id\") }),\r\n                        minorPeriodId: ");
#nullable restore
#line 142 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                                  Write(ViewBag.MUPPeriodId);

#line default
#line hidden
#nullable disable
                WriteLiteral(@",
                        status: status
                    },
                    success: function(response) {
                        var data = Ext.decode(response.responseText);
                        if (data.msg)
                            Ext.MessageBox.show({
                                title: 'Информационное сообщение',
                                msg: data.msg,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.INFO
                            });
                        if (data.reload)
                            location.reload();


                        for (var i in rec) {
                            rec[i].set(""Status"", status);
                            rec[i].set(""Published"", false);
                            // rec.Save();
                        }
                        statusChanged(rec);
                        //store.commitChanges();
                        gridPanel.getView().refres");
                WriteLiteral(@"h();
                    }
                });
            }
            if (rec[0].data.Status === 1 && (status === 0 || status === 2)) {

                request();
            } else {
                request();
            }
        }


        var indeterminateButton = Ext.create('Ext.Button',
        {
            xtype: 'button',
            disabled: true,
            text: 'Нет решения',
            handler: function() { setAdmissionStatus(selRecord, 0); }
        });

        var admittedButton = Ext.create('Ext.Button',
        {
            xtype: 'button',
            disabled: true,
            text: 'Зачислен',
            handler: function() { setAdmissionStatus(selRecord, 1); }
        });

        var deniedButton = Ext.create('Ext.Button',
        {
            xtype: 'button',
            disabled: true,
            text: 'Не зачислен',
            handler: function() { setAdmissionStatus(selRecord, 2); }
        });

        var sendButton = Ext.create");
                WriteLiteral(@"('Ext.Button',
        {
            xtype: 'button',
            disabled: true,
            text: 'Отправить в ЛК',
            handler: function() {
                setTimeout(function() {
                        selRecord.forEach(function(el, index, array) {
                            el.set(""Published"", true);
                        });
                    },
                    0);
                Ext.Ajax.request({
                    url: '");
#nullable restore
#line 215 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                     Write(Url.Action("PublishCompetitionGroupAdmission"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                    params: { studentId: selRecord.map(function(l) { return l.get(\"Id\"); }), propertyId: ");
#nullable restore
#line 216 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                                                                                                    Write(Model.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral(@" },
                    success: function(response) {
                        Ext.toast({ html: ""Сообщение отправлено"", align: 't' });
                        gridPanel.getView().refresh();
                    }
                });

            }
        });


        function statusChanged(rec) {
            if (rec == null)
                return;
            for (var i in rec) {
                var otherAdmission = rec[i].get(""OtherAdmission"");
                var status = rec[i].get(""Status"");
                if (!otherAdmission) {
                    indeterminateButton.setDisabled(status == 0 || !canEdit);
                    admittedButton.setDisabled(status == 1 || !canEdit);
                    deniedButton.setDisabled(status == 2 || !canEdit);
                    sendButton.setDisabled(!canEdit);
                } else {
                    indeterminateButton.disable();
                    admittedButton.disable();
                    deniedButton.disable();
              ");
                WriteLiteral(@"      sendButton.disable();
                }
            }
        }

        gridPanel = Ext.create('Ext.grid.Panel',
        {
            multiSelect: true,
            region: 'center',
            store: store,
            loadMask: true,
            columnLines: true,
            listeners: {
                selectionchange: function(el, records) {
                    selRecord = records.slice();
                    statusChanged(selRecord);
                }
            },
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
                            text: ""Отменить фильтры"",
                   ");
                WriteLiteral(@"         handler: function () {
                                filtersWnd.items.items.forEach(function (element, index, array) {
                                    element.setValue('');
                                });
                                localStorage.setItem(""");
#nullable restore
#line 277 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Web\Views\MUPAdmission\CompetitionGroupStudents.cshtml"
                                                 Write(filterName);

#line default
#line hidden
#nullable disable
                WriteLiteral(@""", []);
                                loadStore();
                            }
                        },
                        '-',
                        indeterminateButton,
                        admittedButton,
                        deniedButton,
                        //'-',
                        //sendButton
                    ]
                },
                {
                    xtype: 'toolbar',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'checkbox',
                            boxLabel: 'Скрыть неактивных студентов',
                            itemId: 'HideNotactiveStudentsMUPAdmission',
                            value: sessionStorage.getItem(hideStudentsStoreName) || false,
                            listeners: {
                                change: function (t, newValue, oldValue, eOpts) {
                                    store.getProxy().setExtraParam('hideStudents', ");
                WriteLiteral(@"newValue);
                                    sessionStorage.setItem(hideStudentsStoreName, newValue);
                                    console.log('alert', newValue);
                                    setFilters();
                                }
                            }
                        },
                    ]
                }
            ],
            viewConfig: {
                getRowClass: function(record) {
                    return !record.get('Published')
                        ? 'highlight'
                        : '';
                },
                markDirty: false
            },
            columns: [
                { xtype: 'rownumberer', width: 50 },
                {
                    header: 'Название группы',
                    dataIndex: 'GroupName',
                    width: 160,
                    renderer: Ext.util.Format.htmlEncode
                },
                {
                    header: 'Фамилия',
                 ");
                WriteLiteral(@"   dataIndex: 'Surname',
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
                },
                {
                    header: 'Статус',
                    dataIndex: 'StudentStatus',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
                },
                {
                    header: 'Личный номер студента',
                    da");
                WriteLiteral(@"taIndex: 'PersonalNumber',
                    width: 120,
                    renderer: Ext.util.Format.htmlEncode
                },
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
                 ");
                WriteLiteral(@"   width: 210,
                    renderer: Urfu.renders.htmlEncode
                },
                {
                    header: 'Состояние',
                    width: 150,
                    dataIndex: 'Status',
                    xtype: 'templatecolumn',
                    tpl: new Ext.XTemplate(
                        '<tpl for=""."" if=""!!AnotherAdmission"">',
                        ""Зачислен в {AnotherAdmission:htmlEncode}"",
                        '<tpl else>',
                        '{[this.getAdmissionStatus(values.Status)]}',
                        ""</tpl>"",
                        {
                            getAdmissionStatus: function(status) {
                                return { 0: ""Нет решения"", 1: ""Зачислен"", 2: ""Не зачислен"" }[status];
                            }
                        }
                    )
                },
                {
                    header: 'Другой МУП',
                    align: 'center',
                    dataInd");
                WriteLiteral(@"ex: 'OtherAdmission',
                    width: 210,
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
            );
            WriteLiteral("\r\n\r\n\r\n\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MUPProperty> Html { get; private set; }
    }
}
#pragma warning restore 1591
