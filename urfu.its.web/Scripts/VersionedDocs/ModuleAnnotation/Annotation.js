function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };

    var variantsCmbxName = 'variantsCmbx';

    var variantsWnd = function (record, insertIndex, att) {
        var variants = record.data.Variants.map(c => c.Id);

        return Ext.create('Ext.window.Window',
            {
                title: "Траектории для модуля",
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
                        variants: {
                            data: data.ModuleAnnotations.PossibleVariants
                        }
                    }
                },
                items: [
                    {
                        xtype: 'label',
                        text: record.data.FullName
                    },
                    {
                        xtype: 'label',
                        text: 'Траектории',
                        style: 'font-weight: bold;'
                    },
                    Ext.create('Ext.form.field.Tag', {
                        fieldLabel: '',
                        bind: {
                            store: '{variants}'
                        },
                        value: variants,
                        name: variantsCmbxName,
                        labelWidth: 0,
                        editable: true,
                        queryMode: 'local',
                        displayField: 'Name',
                        valueField: 'Id',
                        width: 500
                    })
                ],
                buttons: [
                    {
                        text: "ОК",
                        handler: function (btn) {

                            var grid = Ext.getCmp(att.id);
                            var cmbx = this.up().up().items.items.find(c => c.name === variantsCmbxName);
                            var modulesStore = grid.getStore();

                            var variantIds = cmbx.getValue();
                            var variants = cmbx.getStore().data.items.filter(c => variantIds.some(v => v === c.data.Id)).map(c => c.data);
                           
                            record.data.Variants = variants;
                            modulesStore.insert(insertIndex, record);
                            
                            btn.up('window').close();
                        }
                    }
                ]
            });
    };

    var modulesTableBlock = function (att) {

        return {
            name: att.blockName,
            margin: '30 0 30 0',
            items: [
                {
                    xtype: 'label',
                    text: att.title,
                    style: captionStyle
                },
                {
                    xtype: 'container',
                    width: 720,
                    id: att.containerId,
                    margin: '0 0 10 0',
                    layout: { type: 'hbox' },
                    items: [
                        {
                            xtype: 'tagfield',
                            fieldLabel: 'Модули',
                            width: 600,
                            id: att.modulesCmbxId,
                            store: att.modulesCmbxStore,
                            valueField: 'Id',
                            displayField: 'Name',
                            queryMode: 'local'
                        },
                        {
                            xtype: 'button',
                            text: 'Добавить',
                            margin: '0 0 0 10',
                            handler: function (btn) {
                                var modules = Ext.getCmp(att.modulesCmbxId).getValue();
                                var gridStore = Ext.getCmp(att.id).getStore();

                                att.allModulesStore.getData().items.filter(m => modules.some(module => module === m.data.Id)).forEach(function (item) {
                                    if (!gridStore.data.items.some(elem => elem.data.Id === item.data.Id)) {
                                        gridStore.add(item);
                                    }
                                });
                                gridStore.sort('Name', 'ASC');

                                Ext.getCmp(att.modulesCmbxId).setValue('');
                                updateModuleCmbxStore(att.modulesCmbxStore, att.allModulesStore.getData().items, att.id, att.containerId);
                            }
                        }
                    ]
                },
                {
                    xtype: 'grid',
                    id: att.id,
                    bind: {
                        store: att.gridStore
                    },
                    columns: [{
                        header: 'Модуль',
                        dataIndex: 'FullName',
                        width: 300,
                        cellWrap: true,
                        renderer: Ext.util.Format.htmlEncode
                    }, {
                        header: 'Аннотация',
                        dataIndex: 'Annotation',
                        width: 500,
                        cellWrap: true,
                        renderer: Ext.util.Format.htmlEncode
                    }, {
                        header: 'Траектории',
                        dataIndex: 'Variants',
                        width: 300,
                        cellWrap: true,
                        renderer: function (value, metaData) {
                            value = value.map(v => ' ' + v.Name);
                            metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                            return value;
                        }
                    }, {
                        xtype: 'actioncolumn',
                        resizable: false,
                        sortable: false,
                        width: 60,
                        items: [{
                            icon: '/Content/Images/edit.png',
                            iconCls: 'icon-padding',
                            tooltip: 'Редактировать траектории',
                            handler: function (grid, rowIndex, colIndex, item, e, record) {
                                if (data.ModuleAnnotations.PossibleVariants.length == 0) {
                                    Ext.MessageBox.show({
                                        title: 'Уведомление',
                                        msg: 'Траектории не добавлены в версию ОХОП',
                                        buttons: Ext.MessageBox.OK
                                    });
                                    return;
                                }
                                
                                var wnd = variantsWnd(record, rowIndex, att);
                                wnd.show();
                            }
                        }, {
                            icon: '/Content/Images/remove.png',
                            iconCls: 'icon-padding',
                            tooltip: 'Удалить',
                            handler: function (grid, rowIndex, colIndex, item, e, record) {
                                if (confirm('Вы действительно желаете удалить запись?')) {
                                    grid.getStore().remove(record);
                                    updateModuleCmbxStore(att.modulesCmbxStore, att.allModulesStore.getData().items, att.id, att.containerId);
                                }
                            }
                        }]
                    }
                    ]
                }
            ]
        };
    };

    function updateModuleCmbxStore(store, allRecords, gridId, containerId) {
        var tableData = selectStoreItemsData(Ext.getCmp(gridId).getStore());
        var storeData = [];
        allRecords.forEach(function (record) {
            if (!tableData.some(d => d.Id == record.data.Id)) {
                storeData.push(record.data);
            }
        });
        store.setData(storeData);
        Ext.getCmp(containerId).setHidden(store.getData().items.length == 0);
    };
    
    function createRecordsStore(action, cmbxStore, gridId, containerId) {
        return Ext.create("Ext.data.Store",
            {
                autoLoad: true,
                proxy: {
                    type: 'ajax',
                    url: `/ModuleAnnotations/${action}`,
                    reader: { type: 'json' },
                    extraParams: {
                        profile: data.Profile.Id,
                        planNumber: data.Plan.Number,
                        planVersionNumber: data.Plan.Version
                    }
                },
                listeners: {
                    load: function (t, records) {
                        updateModuleCmbxStore(cmbxStore, records, gridId, containerId);
                    }
                }
            }
        )
    };

    var requiredModulesGridId = 'RequiredModules';
    var modulesGridId = 'Modules';
    var practicesGridId = 'Practices';
    var giaGridId = 'Gia';

    var requiredModulesContainerId = 'RequiredModulesContainer';
    var modulesContainerId = 'ModulesContainer';
    var practicesContainerId = 'PracticesContainer';
    var giaContainerId = 'GiaContainer';
    
    var requiredModulesCmbxStore = Ext.create("Ext.data.Store", { data: [] });
    var requiredModulesStore = createRecordsStore("RequiredModules", requiredModulesCmbxStore, requiredModulesGridId, requiredModulesContainerId);

    var modulesCmbxStore = Ext.create("Ext.data.Store", { data: [] });
    var modulesStore = createRecordsStore("Modules", modulesCmbxStore, modulesGridId, modulesContainerId);
    
    var practiceCmbxStore = Ext.create("Ext.data.Store", { data: [] });
    var practiceStore = createRecordsStore("PracticeModules", practiceCmbxStore, practicesGridId, practicesContainerId);

    var giaCmbxStore = Ext.create("Ext.data.Store", { data: [] });
    var giaStore = createRecordsStore("GiaModules", giaCmbxStore, giaGridId, giaContainerId);
    
    var requiredModulesAttributes = {
        title: 'Обязательная часть',
        blockName: 'RequiredModules',
        id: requiredModulesGridId,
        containerId: requiredModulesContainerId,
        modulesCmbxId: 'requiredModulesCmbx',
        modulesCmbxStore: requiredModulesCmbxStore,
        allModulesStore: requiredModulesStore,
        gridStore: '{requiredModules}'
    };
    
    var modulesAttributes = {
        title: 'Часть, формируемая участниками образовательных отношений',
        blockName: 'Modules',
        id: modulesGridId,
        containerId: modulesContainerId,
        modulesCmbxId: 'modulesCmbx',
        modulesCmbxStore: modulesCmbxStore,
        allModulesStore: modulesStore,
        gridStore: '{modules}'
    };

    var practiceAttributes = {
        title: 'Практика',
        blockName: 'Practices',
        id: practicesGridId,
        containerId: practicesContainerId,
        modulesCmbxId: 'practicesCmbx',
        modulesCmbxStore: practiceCmbxStore,
        allModulesStore: practiceStore,
        gridStore: '{practiceModules}'
    };

    var giaAttributes = {
        title: 'Государственная итоговая аттестация',
        blockName: 'Gia',
        id: giaGridId,
        containerId: giaContainerId,
        modulesCmbxId: 'giaCmbx',
        modulesCmbxStore: giaCmbxStore,
        allModulesStore: giaStore,
        gridStore: '{giaModules}'
    };

    return {
        viewModel: {
            stores: {
                requiredModules: {
                    data: data.ModuleAnnotations.RequiredModules
                },
                modules: {
                    data: data.ModuleAnnotations.Modules
                },
                practiceModules: {
                    data: data.ModuleAnnotations.Practices
                },
                giaModules: {
                    data: data.ModuleAnnotations.Gia
                }
            }
        },
        items: [
            {
                xtype: 'label',
                html: `<b>Учебный план: </b>${data.Plan.Number} (${data.Plan.Version})`
            },
            {
                xtype: 'label',
                html: `<b>Институт/подразделение: </b>${data.Institute.Name}`
            },
            {
                xtype: 'label',
                html: `<b>Направление: </b>${data.Direction.Code} - ${data.Direction.Title}`
            },
            {
                xtype: 'label',
                html: `<b>Образовательная программа: </b>${data.Profile.Code} ${data.Profile.Name}`
            },
            {
                xtype: 'label',
                html: `<b>Описание образовательной программы:</b>`
            },
            {
                xtype: 'textarea',
                height: 300,
                bind: '{Description}',
                border: false,
                editable: false
            },
            {
                name: 'ModuleAnnotations',
                contentReader: function (content, vm) {
                    
                    content.RequiredModules = selectStoreItemsData(Ext.getCmp(requiredModulesAttributes.id).getStore());
                    content.Modules = selectStoreItemsData(Ext.getCmp(modulesAttributes.id).getStore());
                    content.Practices = selectStoreItemsData(Ext.getCmp(practiceAttributes.id).getStore());
                    content.Gia = selectStoreItemsData(Ext.getCmp(giaAttributes.id).getStore());

                    content.PossibleVariants = data.ModuleAnnotations.PossibleVariants;
                    return content;
                },
                items: [
                    modulesTableBlock(requiredModulesAttributes),
                    modulesTableBlock(modulesAttributes),
                    modulesTableBlock(practiceAttributes),
                    modulesTableBlock(giaAttributes)
                ]
            }
        ]
    };
}