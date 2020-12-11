function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };

    var ucAttributes = {
        blockName: 'UniversalCompetences',
        type: 'УК',
        id: 'UniversalCompetences',
        competenceGroupsId: 'competenceGroupForUC',
        competencesListId: 'universalCompetencesList',
        competenceGroupsStore: '{competenceGroupsUC}',
        gridStore: '{universalCompetences}',
        cmbxName: 'Универсальные',
        grpColumnName: 'универсальных',
        cmpColumnName: 'универсальной'
    };

    var gcAttributes = {
        blockName: 'GeneralCompetences',
        type: 'ОПК',
        id: 'GeneralCompetences',
        competenceGroupsId: 'competenceGroupForGC',
        competencesListId: 'generalCompetencesList',
        competenceGroupsStore: '{competenceGroupsGC}',
        gridStore: '{generalCompetences}',
        cmbxName: 'Общепрофессиональные',
        grpColumnName: 'общепрофессиональных',
        cmpColumnName: 'общепрофессиональной'
    };

    var profCompetencesGridId = 'profCompetencesGridId';
    var profCompetencesCmbxName = 'profCompetencesCmbxName';

    var labelWidth = 180;

    function updateCompetencesStore(t, att) {
        var competenceGroupCmbx = Ext.getCmp(att.competenceGroupsId);
        var competenceGroup = competenceGroupCmbx.getValue();
        var competencesGroupStore = competenceGroupCmbx.getStore();

        Ext.getCmp(att.competencesListId).setValue();

        var competencesArray = competencesGroupStore.data.items.filter(g => g.data.Id === competenceGroup).map(g => g.data.Competences.filter(c => c.Type === att.type));
        var competences = [];
        if (competencesArray.length > 0) {
            competences = competencesArray.reduce(function (a, b) {
                return a.concat(b);
            });
        }

        t.up().lookupViewModel().getStore('competences').setData(competences);
    }

    var competencesTableBlock = function (att) {

        return {
            name: att.blockName,
            contentReader: function (content, vm) {
                var grid = Ext.getCmp(att.id);
                content = grid.getStore().data.items.map(i => i.data);
                return content;
            },
            viewModel: {
                stores: {
                    competences: {}
                }
            },
            items: [
                //{
                //    xtype: 'container',
                //    width: 720,
                //    layout: { type: 'vbox' },
                //    items: [
                //        {
                //            xtype: 'comboedit',
                //            id: att.competenceGroupsId,
                //            labelWidth: labelWidth,
                //            fieldLabel: 'Группа компетенций',
                //            displayField: 'Name',
                //            valueField: 'Id',
                //            width: 600,
                //            bind: {
                //                store: att.competenceGroupsStore
                //            },
                //            listeners: {
                //                change: function () {
                //                    updateCompetencesStore(this, att);
                //                }
                //            }
                //        },
                //        {
                //            xtype: 'container',
                //            width: 720,
                //            margin: '0 0 10 0',
                //            layout: { type: 'hbox' },
                //            items: [
                //                {
                //                    xtype: 'tagfield',
                //                    fieldLabel: att.cmbxName + ' компетенции',
                //                    id: att.competencesListId,
                //                    labelWidth: labelWidth,
                //                    width: 600,
                //                    bind: {
                //                        store: '{competences}'
                //                    },
                //                    valueField: 'Id',
                //                    displayField: 'Code',
                //                    queryMode: 'local',
                //                    tpl: Ext.create('Ext.XTemplate',
                //                        '<tpl for=".">',
                //                        '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                //                        '<div><b>{Code}</b> - {Content}</div>',
                //                        '</div>',
                //                        '</tpl>'
                //                    )
                //                },
                //                {
                //                    xtype: 'button',
                //                    text: 'Добавить',
                //                    margin: '0 0 0 10',
                //                    handler: function (btn) {
                //                        var competences = Ext.getCmp(att.competencesListId).getValue();
                //                        var store = btn.up().lookupViewModel().getStore('competences');
                //                        var gridStore = Ext.getCmp(att.id).getStore();

                //                        store.data.items.filter(c => competences.some(com => com === c.data.Id)).forEach(function (item, index, arr) {
                //                            if (!gridStore.data.items.some(elem => elem.data.Id === item.data.Id)) {
                //                                gridStore.add(item);
                //                            }
                //                        });
                //                    }
                //                }]
                //        }
                //    ]
                //},
                {
                    xtype: 'grid',
                    id: att.id,
                    bind: {
                        store: att.gridStore
                    },
                    tbar: [{
                        xtype: 'button',
                        text: 'Обновить',
                        cls: 'btn-text-color',
                        style: {
                            borderColor: '#157fcc'
                        },
                        handler: function (btn) {
                            var thisBlock = this.up().up();
                            thisBlock.mask('Обновление');
                            Ext.Ajax.request({
                                method: 'GET',
                                url: `/BasicCharacteristicOP/${att.id}`,
                                params: {
                                    id: data.EduProgramInfo.Id
                                },
                                success: function (response) {
                                    thisBlock.unmask();
                                    try {
                                        var data = Ext.decode(response.responseText);
                                        thisBlock.getStore().setData(data.competences);
                                    }
                                    catch{
                                        Ext.MessageBox.show({
                                            title: 'Ошибка',
                                            msg: 'Повторите попытку позже',
                                            buttons: Ext.MessageBox.OK
                                        });
                                    }
                                },
                                failure: function (response) {
                                    thisBlock.unmask();
                                    Ext.MessageBox.show({
                                        title: 'Ошибка',
                                        msg: 'Повторите попытку позже',
                                        buttons: Ext.MessageBox.OK
                                    });
                                }
                            });
                        }
                    }],
                    columns: [{
                        header: 'Наименование категории (группы) ' + att.grpColumnName + ' компетенций',
                        dataIndex: 'CompetenceGroupName',
                        width: 300,
                        cellWrap: true,
                        renderer: Ext.util.Format.htmlEncode
                    }, {
                        header: 'Код и наименование ' + att.cmpColumnName + ' компетенции выпускника образовательной программы',
                        dataIndex: 'Content',
                        width: 500,
                        cellWrap: true,
                        renderer: function (value, metaData) {
                            value = Ext.String.htmlEncode(metaData.record.data.Code + ' - ' + metaData.record.data.Content);
                            metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                            return value;
                        }
                    },
                    //{
                    //    xtype: 'actioncolumn',
                    //    resizable: false,
                    //    sortable: false,
                    //    width: 60,
                    //    items: [{
                    //        icon: '/Content/Images/remove.png',
                    //        tooltip: 'Удалить',
                    //        handler: function (grid, rowIndex, colIndex, item, e, record) {
                    //            if (confirm('Вы действительно желаете удалить запись?')) {
                    //                grid.getStore().remove(record);
                    //            }
                    //        }
                    //    }]
                    //}
                    ]
                }
            ]
        };
    };
 
    var profCompetencesWnd = function (record, insertIndex) {
        var competences = record.data.Competences.map(c => c.Id);
        return Ext.create('Ext.window.Window',
            {
                title: "Профессиональные компетенции",
                closeAction: 'hide',
                overflowY: 'auto',
                resizable: true,
                x: 100,
                y: 100,
                maxHeight: 500,
                width: 525,
                autoHeight: true,
                modal: true,
                bodyPadding: 6,
                layout: { type: 'vbox', align: 'stretch' },
                viewModel: {
                    stores: {
                        profCompetences: {
                            autoLoad: true,
                            proxy: {
                                type: 'ajax',
                                url: '/BasicCharacteristicOP/GetProfessionalCompetences/' + window.location.search,
                                reader: { type: 'json' },
                                extraParams: {
                                    id: data.EduProgramInfo.Id
                                }
                            }
                        }
                    }
                },
                items: [
                    {
                        xtype: 'label',
                        text: 'Траектория',
                        style: 'font-weight: bold;'
                    },
                    {
                        xtype: 'label',
                        text: record.data.Name
                    },
                    {
                        xtype: 'label',
                        text: 'Типы задачи профессиональной деятельности',
                        style: 'font-weight: bold;'
                    },
                    {
                        xtype: 'label',
                        text: record.data.ProfTaskTypes
                    },
                    {
                        xtype: 'label',
                        text: 'Профессиональный стандарт',
                        style: 'font-weight: bold;'
                    },
                    {
                        xtype: 'label',
                        text: record.data.NoProfStandard ? 'Отсутствует' : `${record.data.StandardCode} - ${record.data.StandardTitle}`
                    },
                    {
                        xtype: 'label',
                        text: 'Функции',
                        style: 'font-weight: bold;'
                    },
                    {
                        xtype: 'label',
                        text: record.data.NoProfStandard ? 'Отсутствует' : record.data.Functions
                    },
                    {
                        xtype: 'label',
                        text: 'Профессиональные компетенции',
                        style: 'font-weight: bold;'
                    },
                    Ext.create('Ext.form.field.Tag', {
                        fieldLabel: '',
                        bind: {
                            store: '{profCompetences}'
                        },
                        value: competences,
                        name: profCompetencesCmbxName, 
                        labelWidth: 0,
                        editable: true,
                        queryMode: 'local',
                        displayField: 'Code',
                        valueField: 'Id',
                        width: 500,
                        tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                            '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                            '<div><b>{Code}</b> {Content}</div>' +
                            '</div>',
                            '</tpl>'
                        )
                    })
                ],
                buttons: [
                    {
                        text: "ОК",
                        handler: function (btn) {
                            
                            var grid = Ext.getCmp(profCompetencesGridId);
                            var cmbx = this.up().up().items.items.find(c => c.name === profCompetencesCmbxName);
                            var vm = findRootVm(grid);
                            var variantInfoStore = grid.getStore();

                            var competenceIds = cmbx.getValue();
                            var competences = cmbx.getStore().data.items.filter(c => competenceIds.some(com => com === c.data.Id)).map(c => c.data);

                            record.data.Competences = competences;
                            variantInfoStore.insert(insertIndex, record);

                            btn.up('window').close();
                        }
                    }
                ]
            });
    };

    var competencesData = [];
    data.Variants.VariantInfos.forEach(function (item, index, arr) {
        
        item.ProfActivityRows.forEach(function (row) {
            var rowData = Ext.clone(row);
            
            rowData.Id = item.Id;
            rowData.Name = item.Name;
            rowData.IdSource = item.IdSource;

            competencesData.push(rowData);
        });
    });

    return {
        viewModel: {
            stores: {
                //competenceGroupsUC: {
                //    autoLoad: true,
                //    proxy: {
                //        type: 'ajax',
                //        url: '/BasicCharacteristicOP/GetCompetencesGroups/' + window.location.search,
                //        reader: { type: 'json' },
                //        extraParams: {
                //            profileId: data.EduProgramInfo.ProfileId,
                //            type: "УК"
                //        }
                //    }
                //},
                //competenceGroupsGC: {
                //    autoLoad: true,
                //    proxy: {
                //        type: 'ajax',
                //        url: '/BasicCharacteristicOP/GetCompetencesGroups/' + window.location.search,
                //        reader: { type: 'json' },
                //        extraParams: {
                //            profileId: data.EduProgramInfo.ProfileId,
                //            type: "ОПК"
                //        }
                //    }
                //},
                universalCompetences: {
                    data: data.UniversalCompetences
                },
                generalCompetences: {
                    data: data.GeneralCompetences
                }
            }
        },

        items: [
            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'label',
                    text: 'Таблица 2. Универсальные компетенции',
                    style: captionStyle
                }, {
                    xtype: 'label',
                        text: `УК указываются в соответствии с ${data.Direction.Standard} УрФУ (п.3.3)`,
                    style: italicStyle
                }]
            },
            competencesTableBlock(ucAttributes),

            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'label',
                    text: 'Таблица 3. Общепрофессиональные компетенции',
                    style: captionStyle
                }, {
                    xtype: 'label',
                    text: `ОПК указываются в соответствии с ${data.Direction.Standard} УрФУ (п.3.4)`,
                    style: italicStyle
                }]
            },
            competencesTableBlock(gcAttributes),
            
            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'label',
                    text: 'Таблица 4. Профессиональные компетенции',
                    style: captionStyle
                }, {
                    xtype: 'label',
                    text: `Профессиональные компетенции выпускников ОП разработаны 
                        на основе соответствующих профессиональных стандартов (при наличии), а также на основе анализа требований 
                        к профессиональным компетенциям выпускников образовательной программы, предъявляемым на региональном рынке труда, 
                        обобщения зарубежного опыта, проведения консультаций с ведущими работодателями, объединениями работодателей отрасли, 
                        иных источников.`,
                    style: italicStyle
                    }, {
                        xtype: 'label',
                        text: `Профессиональные компетенции формулируются глаголами действия в завершенной форме, 
                                которая указывает на те действия, которые студенты должны освоить в процессе обучения и продемонстрировать. 
                                Профессиональные компетенции ориентированы на решение профессиональных задач различных типов в рамках, 
                                указанных в табл. 1 траекторий ОП или профиля ОП (5-10 ПК для одной траектории).`,
                        style: italicStyle
                    }]
            },
            {
                name: 'Variants',
                contentReader: function (content, vm) {

                    var grid = Ext.getCmp(profCompetencesGridId);

                    var contentVariantInfos = [];

                    grid.getStore().getData().items.forEach(function (item) {
                        
                        delete item.data.id;

                        if (!contentVariantInfos.some(i => i.Id === item.data.Id && i.IdSource === item.data.IdSource)) {
                            contentVariantInfos.push({
                                Id: item.data.Id,
                                IdSource: item.data.IdSource,
                                Name: item.data.Name,
                                ProfActivityRows: []
                            });
                        }
                        
                        contentVariantInfos
                            .find(i => i.Id === item.data.Id && i.IdSource === item.data.IdSource)
                            .ProfActivityRows.push(item.data);
                    });

                    content.VariantInfos = contentVariantInfos;
                    return content;
                },
                items: [
                    {
                        xtype: 'grid',
                        id: profCompetencesGridId,
                        store: Ext.create('Ext.data.Store', {
                            data: competencesData
                        }),
                        columns: [
                            {
                                header: 'Наименование и код траектории ОП',
                                dataIndex: 'Name',
                                width: 300,
                                cellWrap: true,
                                renderer: Ext.util.Format.htmlEncode
                            },
                            {
                                header: 'Тип (типы) задач профессиональной деятельности',
                                dataIndex: 'ProfTaskTypes',
                                width: 300,
                                cellWrap: true,
                                renderer: Ext.util.Format.htmlEncode
                            },
                            {
                                header: 'Код профессионального стандарта и трудовых функций',
                                width: 300,
                                cellWrap: true,
                                renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                    var functions = record.data.Functions !== null ? record.data.Functions : "";
                                    value = record.data.NoProfStandard
                                        ? 'Отсутствуют'
                                        : `ПС ${Ext.String.htmlEncode(record.data.StandardCode)}, ОТФ/ТФ ${functions}`;
                                    return value;
                                }
                            },
                            {
                                header: 'Профессиональные компетенции, формируемые в рамках образовательной траектории',
                                dataIndex: 'Competences',
                                width: 500,
                                cellWrap: true,
                                renderer: function (value, metaData) {

                                    var str = "";
                                    value.forEach(function (val, index, arr) {
                                        str += val.Code + ' - ' + val.Content + '; ';
                                    });
                                    
                                    value = Ext.String.htmlEncode(str);
                                    metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                                    return value;
                                }
                            },
                            {
                                xtype: 'actioncolumn',
                                resizable: false,
                                sortable: false,
                                width: 60,
                                items: [{
                                    icon: '/Content/Images/edit.png',
                                    tooltip: 'Редактировать компетенции',
                                    handler: function (grid, rowIndex, colIndex, item, e, record) {
                                        var wnd = profCompetencesWnd(record, rowIndex);
                                        wnd.show();
                                    }
                                }]
                            }
                        ]
                    }
                ]
            }
        ]
    };
}