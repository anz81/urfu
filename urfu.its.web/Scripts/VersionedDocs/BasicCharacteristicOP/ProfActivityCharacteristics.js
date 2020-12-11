function uiInit(documentId, documentType, data, schema, emptyData) {

    var profActivityAreaStore = Ext.create('Ext.data.Store',
        {
            proxy:
            {
                type: 'ajax',
                url: '/BasicCharacteristicOP/ProfActivityArea',
                reader: { type: 'json', rootProperty: 'data' }
            },
            autoLoad: true
        });
    profActivityAreaStore.load();

    var profActivityKindStore = Ext.create('Ext.data.Store', { });

    var updateKindStore = function (area) {
        try {
            var areaData = profActivityAreaStore.findRecord("AreaCode", area).data;
            profActivityKindStore.setData(areaData.Kinds);
        }
        catch{
            profActivityKindStore.setData([]);
        }
    };

    var standardTextAreaName = 'standardTextAreaName';

    var editRowWindow = function (grid, rowData, rowIndex, edit) {
        var competences = edit ? rowData.Competences : [];
        if (edit) {
            updateKindStore(rowData.AreaCode);
        }
        var title = edit ? 'Редактирование' : 'Добавление';
        return Ext.create('Ext.window.Window', {
            closeAction: 'close',
            title: `${title} области, объекта или типа задач профессиональной деятельности`,
            modal: true,
            width: 830,
            height: 500,
            scrollable: true,
            defaults: {
                width: 780
            },
            items: [
                {
                    xtype: 'checkbox',
                    margin: '0 10 0 10',
                    boxLabel: 'Нет профессионального стандарта',
                    itemId: 'NoProfStandard',
                    readOnly: edit,
                    value: edit ? rowData.NoProfStandard : false,
                    width: 300,
                    listeners: {
                        change: function (t, newValue, oldValue, eOpts) {
                            // блоки, если проф стандарта нет
                            t.up().getComponent("AreaTitleText").setHidden(!newValue);
                            
                            // блоки относятся к проф стандарту
                            t.up().getComponent("ProfActivityArea").setHidden(newValue);
                            t.up().getComponent("ProfActivityKind").setHidden(newValue);
                            t.up().getComponent("ProfStandardLabel").setHidden(newValue);
                            t.up().getComponent(standardTextAreaName).setHidden(newValue);
                            t.up().getComponent('FunctionsLabel').setHidden(newValue);
                            t.up().getComponent('Functions').setHidden(newValue);
                        }
                    }
                },
                {
                    xtype: 'textfield',
                    margin: '0 10 10 10',
                    fieldLabel: 'Наименование области профессиональной деятельности',
                    itemId: "AreaTitleText",
                    labelWidth: 180,
                    hidden: edit ? !rowData.NoProfStandard : true,
                    value: edit ? rowData.AreaTitle : ''
                },
                {
                    fieldLabel: "Область профессиональной деятельности",
                    itemId: "ProfActivityArea",
                    xtype: "combobox",
                    store: profActivityAreaStore,
                    valueField: 'AreaCode',
                    displayField: 'AreaFullTitle',
                    queryMode: 'local',
                    labelWidth: 150,
                    margin: '0 10 0 10',
                    anyMatch: true,
                    readOnly: edit,
                    hidden: edit ? rowData.NoProfStandard : false,
                    value: edit ? rowData.AreaCode : null,
                    listeners: {
                        change: function (t, newValue) {
                            t.up().getComponent("ProfActivityKind").setValue(null);
                            updateKindStore(newValue);
                        }
                    }
                },
                {
                    fieldLabel: "Вид профессиональной деятельности",
                    itemId: "ProfActivityKind",
                    xtype: "combobox",
                    store: profActivityKindStore,
                    valueField: 'KindCode',
                    displayField: 'KindFullTitle',
                    queryMode: 'local',
                    labelWidth: 150,
                    margin: '0 10 0 10',
                    anyMatch: true,
                    readOnly: edit,
                    hidden: edit ? rowData.NoProfStandard : false,
                    value: edit ? rowData.KindCode : null,
                    listeners: {
                        change: function (t, newValue, oldValue) {
                            try {
                                var standardLabel = t.up().items.items.find(i => i.name === standardTextAreaName);

                                standardLabel.setText('');
                            
                                var kindData = profActivityKindStore.findRecord("KindCode", newValue).data;

                                var oldStandard = kindData.IsOldStandard ? "(Утратил силу) " : "";

                                standardLabel.setText(`${oldStandard}${kindData.StandardCode} - ${kindData.StandardTitle}`);
                            }
                            catch{ }
                        }
                    }
                },
                {
                    xtype: 'label',
                    margin: '20 10 0 10',
                    itemId: 'ProfStandardLabel',
                    text: 'Профессиональный стандарт:',
                    hidden: edit ? rowData.NoProfStandard : false,
                },
                {
                    xtype: 'label',
                    margin: '0 10 0 10',
                    itemId: standardTextAreaName,
                    name: standardTextAreaName,
                    text: edit ? `${rowData.StandardCode} - ${rowData.StandardTitle}` : null,
                    hidden: edit ? rowData.NoProfStandard : false,
                },
                {
                    xtype: 'label',
                    margin: '20 10 0 10',
                    itemId: 'FunctionsLabel',
                    hidden: edit ? rowData.NoProfStandard : false,
                    text: 'Обобщенные трудовые функции/трудовые функции из соответствующих профессиональных стандартов, к выполнению которых должен быть подготовлен выпускник в рамках траектории образовательной программы:'
                },
                {
                    xtype: 'textarea',
                    itemId: "Functions",
                    margin: '0 10 10 10',
                    height: 50,
                    hidden: edit ? rowData.NoProfStandard : false,
                    value: edit ? rowData.Functions : ''
                },
                {
                    xtype: 'label',
                    margin: '10 10 0 10',
                    text: 'Объекты профессиональной деятельности, конкретизирующие сферу деятельности выпускников в рамках траектории образовательной программы:'
                },
                {
                    xtype: 'textarea',
                    margin: '0 10 0 10',
                    height: 150,
                    itemId: "ProfObjects",
                    value: edit ? rowData.ProfObjects : ''
                },

                {
                    xtype: 'label',
                    margin: '10 10 0 10',
                    text: 'Тип (типы) задач профессиональной деятельности и/или профессиональные задачи, соответствующие обобщенным трудовым функциям/трудовым функциям и объектам профессиональной деятельности в рамках траектории образовательной программы:'
                },
                {
                    xtype: 'textarea',
                    margin: '0 10 10 10',
                    itemId: "ProfTaskTypes",
                    value: edit ? rowData.ProfTaskTypes : '',
                    height: 150
                }
            ],
            buttons: [
                {
                    text: 'ОК',
                    handler: function (btn) {
                        var wnd = btn.up().up();

                        var areaValue = wnd.getComponent("ProfActivityArea").getValue();
                        var kindValue = wnd.getComponent("ProfActivityKind").getValue();
                        var noProfStandard = wnd.getComponent("NoProfStandard").getValue();

                        var areaCodeText = "";
                        var areaTitleText = wnd.getComponent("AreaTitleText").getValue();
                        
                        areaTitleText = areaTitleText === null ? "" : areaTitleText;

                        var isValid = noProfStandard && areaTitleText.trim() !== ''
                            || !noProfStandard && areaValue !== null && kindValue !== null;
                        
                        if (!isValid) {
                            Ext.MessageBox.alert('Ошибка', noProfStandard ? "Укажите код и наименование области профессиональной деятельности" : "Укажите область и вид профессиональной деятельности");
                            return;
                        }

                        var areaData = !noProfStandard ? profActivityAreaStore.findRecord("AreaCode", wnd.getComponent("ProfActivityArea").getValue()).data : null;
                        var kindData = !noProfStandard ? profActivityKindStore.findRecord("KindCode", wnd.getComponent("ProfActivityKind").getValue()).data : null;

                        if (!noProfStandard) {
                            if (kindData.IsOldStandard) {
                                Ext.MessageBox.alert('Ошибка', 'Профессиональный стандарт утратил силу');
                                return;
                            }
                        }

                        var newData = {
                            ProfObjects: wnd.getComponent("ProfObjects").getValue(),
                            ProfTaskTypes: wnd.getComponent("ProfTaskTypes").getValue(),

                            AreaCode: noProfStandard ? areaCodeText : areaData.AreaCode,
                            AreaTitle: noProfStandard ? areaTitleText : areaData.AreaTitle,

                            NoProfStandard: noProfStandard,

                            KindCode: noProfStandard ? null : kindData.KindCode,
                            KindTitle: noProfStandard ? null : kindData.KindTitle,

                            StandardCode: noProfStandard ? null : kindData.StandardCode,
                            StandardTitle: noProfStandard ? null : kindData.StandardTitle,

                            Functions: noProfStandard ? null : wnd.getComponent("Functions").getValue(),

                            Competences: competences
                        };
                        if (edit) {
                            grid.store.getAt(rowIndex).data = newData;
                            grid.refresh();
                        }
                        else {
                            grid.store.add(newData);
                        }
                        
                        wnd.close();
                    }
                }
            ]
        });
    };

    var variantOrProfileIdName = 'variantOrProfileId';
    var idSourceName = 'idSource';

    var blockVariantInfo = function (variantData, closable = true) {
        var name = variantData.Name;
        if (!variantData.hasOwnProperty("ProfActivityRows"))
            variantData.ProfActivityRows = [];

        var gridStoreData = Ext.clone(variantData.ProfActivityRows);
        
        return {
            margin: '10 10 10 10',
            title: name,
            closable: closable,
            closeToolText: 'Удалить',
            style: {
                borderStyle: 'solid'
            },
            layout: { type: 'vbox', align: 'stretch' },
            defaults: { margin: '10 10 10 10' },
            items: [
                {
                    name: variantOrProfileIdName,
                    value: variantData.Id,
                    readOnly: true,
                    hidden: true
                },
                {
                    name: idSourceName,
                    value: variantData.IdSource,
                    readOnly: true,
                    hidden: true
                },
                {
                    name: variantOrProfileName,
                    value: variantData.Name,
                    readOnly: true,
                    hidden: true
                },
                {
                    xtype: 'list',
                    header: false,
                    loadMask: true,
                    store: Ext.create('Ext.data.Store', {
                        data: gridStoreData
                    }),
                    name: gridName,
                    tbar: [
                        {
                            xtype: 'button',
                            text: 'Добавить',
                            handler: function (btn) {
                                var modal = editRowWindow(btn.up().up());
                                modal.show();
                            }
                        }
                    ],
                    columns: [
                        {
                            xtype: 'rownumberer',
                            width: 50
                        },
                        {
                            header: 'Область и вид профессиональной деятельности',
                            width: 200,
                            cellWrap: true,
                            renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {

                                value = '';
                                if (record.data.NoProfStandard)
                                    value += `${Ext.String.htmlEncode(record.data.AreaTitle)}`;
                                else
                                    value += `${Ext.String.htmlEncode(record.data.AreaCode)} - ${Ext.String.htmlEncode(record.data.AreaTitle)}. 
                                                    ${Ext.String.htmlEncode(record.data.KindCode)} - ${Ext.String.htmlEncode(record.data.KindTitle)}`;

                                return value;
                            }
                        },
                        {
                            header: 'Область профессиональной деятельности',
                            dataIndex: 'AreaCode',
                            width: 200,
                            hidden: true,
                            cellWrap: true
                        },
                        {
                            header: 'Область профессиональной деятельности',
                            dataIndex: 'AreaTitle',
                            width: 1500,
                            cellWrap: true,
                            hidden: true,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Вид профессиональной деятельности',
                            dataIndex: 'KindCode',
                            width: 200,
                            cellWrap: true,
                            hidden: true,
                            renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                value = Ext.String.htmlEncode(value) + " - " + Ext.String.htmlEncode(record.data.KindTitle);
                                return value;
                            }
                        },
                        {
                            header: 'Вид профессиональной деятельности',
                            dataIndex: 'KindTitle',
                            width: 200,
                            cellWrap: true,
                            hidden: true,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Код и наименование профессиональнолго стандарта',
                            dataIndex: 'StandardCode',
                            width: 200,
                            cellWrap: true,
                            renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                value = record.data.NoProfStandard ? 'Отсутствует' : `${Ext.String.htmlEncode(value)} - ${Ext.String.htmlEncode(record.data.StandardTitle)}`;
                                return value;
                            }
                        },
                        {
                            header: 'Код и наименование профессиональнолго стандарта',
                            dataIndex: 'StandardTitle',
                            width: 200,
                            cellWrap: true,
                            hidden: true,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Обобщенные трудовые функции/ трудовые функции',
                            dataIndex: 'Functions',
                            width: 200,
                            cellWrap: true,
                            renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                value = record.data.NoProfStandard ? 'Отсутствует' : Ext.String.htmlEncode(value);
                                return value;
                            }
                        },
                        {
                            header: 'Объекты профессиональной деятельности',
                            dataIndex: 'ProfObjects',
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
                            xtype: 'actioncolumn',
                            region: 'center',
                            sortable: false,
                            width: 30,
                            items: [
                                {
                                    icon: '/Content/Images/edit.png")',
                                    //iconCls: 'icon-padding',
                                    tooltip: 'Редактировать',
                                    handler: function (grid, rowIndex, colIndex) {
                                        var rec = grid.getStore().getAt(rowIndex);
                                        var modal = editRowWindow(grid, rec.data, rowIndex, true);
                                        modal.show();
                                    }
                                }
                            ]
                        }
                    ]
                }
            ]
        };
    };

    var id = 'VariantsBlockId';
    var isProfileCheckBoxName = 'isProfileCheckBox';
    var addVariantButtonName = 'addVariantButtonName';

    var objectsTextName = 'objectsTextName';
    var taskTypesTextName = 'taskTypesTextName';
    var gridName = 'gridName';

    var variantOrProfileName = 'Name';
    
    var getVariantInfosData = function (isProfile) {
        var variantInfosData = [];
        Ext.getCmp(id).items.items.forEach(function (item, index, array) {
            if (item.name !== isProfileCheckBoxName && item.name !== addVariantButtonName) {

                var blockData = {};
                
                var id = item.items.findBy(i => i.name === variantOrProfileIdName).value;
                var idSource = item.items.findBy(i => i.name === idSourceName).value;

                blockData.Id = id;
                blockData.IdSource = idSource;
                blockData.Name = item.items.findBy(i => i.name === variantOrProfileName).value;
                
                var rows = [];
                var gridData = selectStoreItemsData(item.items.findBy(i => i.name === gridName).getStore()).map(function (i) { return i; });
                gridData.forEach(function (row) {
                    rows.push(row);
                });

                blockData.ProfActivityRows = rows;

                variantInfosData.push(blockData);

            }
        });
        return variantInfosData;
    };

    // Таблица 1
    var tableBlock1 = function () {
        var cancelValue = data.Variants.IsProfile; // используется для отмены значения чекбокса. TODO найти более изящное решение. можно использовать setRawValue, но тогда значение IsProfile не меняется, а это нужно

        var block = {
            name: 'Variants',
            id: id,
            contentReader: function (content, vm) {
                content.VariantInfos = getVariantInfosData(content.IsProfile);
                return content;
            },
            layout: { type: 'vbox', align: 'stretch' },
            title: 'Таблица 1. Траектории образовательной программы, области, объекты и типы задач профессиональной деятельности',
            items: [
                {
                    xtype: 'checkbox',
                    boxLabel: 'По образовательной программе',
                    bind: {
                        value: '{Variants.IsProfile}'
                    },
                    name: isProfileCheckBoxName,
                    width: 300,
                    listeners: {
                        change: function (t, newValue, oldValue, eOpts) {
                            if (cancelValue) {
                                cancelValue = false;
                                return;
                            }

                            var changeBlockFunc = function () {
                                // удаляем блоки. оставляем только чекбокс и кнопку
                                var idsToRemove = [];
                                Ext.getCmp(id).items.items.forEach(function (item, index, array) {
                                    if (item.name !== isProfileCheckBoxName && item.name !== addVariantButtonName) {
                                        //Ext.getCmp(id).remove(index);
                                        idsToRemove.push(index);
                                    }
                                });
                                idsToRemove.sort((a, b) => b - a).forEach(function (item, index, array) {
                                    Ext.getCmp(id).remove(item);
                                });

                                // если по обр. программе, то кнопку "Добавить траекторию" скрываем, если нет (по траекториям) - то показываем
                                Ext.getCmp(id).items.items.find(i => i.name === addVariantButtonName).setHidden(newValue === true);

                                if (newValue === true) // по образовательной программе
                                {
                                    // добавляем блок для данных по Образовательной программе
                                    Ext.getCmp(id).insert(Ext.getCmp(id).items.items.length, blockVariantInfo({
                                        Id: data.Profile.Id,
                                        IdSource: 0, // Urfu.Its.Web.Models.//OHOPModels//.Variants.cs enum IdSource.Profile
                                        Name: data.Profile.Name
                                    }, false));
                                }
                            };

                            if (getVariantInfosData(true).length > 0) {
                                Ext.MessageBox.show({
                                    title: 'Предупреждение',
                                    msg: "Заполненные данные будут удалены. Продолжить?",
                                    buttons: Ext.MessageBox.YESNO,
                                    fn: function (btn) {
                                        if (btn === 'no') { // отменяем изменение значения чекбокса, данные не трогаем
                                            cancelValue = true;
                                            t.setValue(!newValue);
                                        }
                                        else {
                                            // стираем данные
                                            changeBlockFunc();
                                        }
                                    }
                                });
                            }
                            else {
                                changeBlockFunc();
                            }
                        }
                    }
                },
                {
                    xtype: 'button',
                    text: 'Добавить траекторию',
                    bind: {
                        hidden: '{Variants.IsProfile}'
                    },
                    width: 300,
                    name: addVariantButtonName,
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = Ext.create('Ext.window.Window', {
                            viewModel: vm,
                            closeAction: 'close',
                            title: 'Траектории',
                            modal: true,
                            height: 500,
                            width: 510,
                            layout: 'fit',
                            items: [{
                                xtype: 'grid',
                                loadMask: true,
                                selModel: 'rowmodel',
                                multiSelect: false,
                                allowDeselect: true,
                                reference: 'variantsGrid',
                                store: {
                                    autoLoad: true,
                                    proxy: {
                                        type: 'ajax',
                                        url: '/BasicCharacteristicOP/GetVariants/' + window.location.search,
                                        reader: { type: 'json' },
                                        extraParams: {
                                            documentId: documentId
                                        }
                                    },
                                    listeners: {
                                        load: function (t, records, successful, operation, eOpts) {
                                            var variantIds = getVariantInfosData(false).map(i => i.Id);
                                            if (variantIds.length > 0)
                                                records = records.filter(r => !variantIds.some(i => i === r.data.Id));
                                                        
                                            t.setRecords(records);
                                        }
                                    }
                                },
                                columns: [{
                                    header: 'Id',
                                    dataIndex: 'Id',
                                    width: 100,
                                    hidden: true,
                                    renderer: Ext.util.Format.htmlEncode
                                }, {
                                    header: 'Название',
                                    dataIndex: 'Name',
                                    cellWrap: true,
                                    width: 500,
                                    renderer: Ext.util.Format.htmlEncode
                                }],
                                bbar: ['->',
                                    {
                                        width: 100,
                                        text: 'Выбрать',
                                        bind: {
                                            disabled: '{!variantsGrid.selection}'
                                        },
                                        handler: function (btn) {
                                            var selection = btn.up('grid').getSelection();
                                            selection.forEach(function (item, i, arr) {
                                                var block = Ext.getCmp(id);
                                                block.insert(block.items.items.length, blockVariantInfo(item.data));
                                            });
                                            btn.up('window').close();
                                        }
                                    },
                                    {
                                        width: 100,
                                        text: 'Отмена',
                                        handler: function (btn) {
                                            btn.up('window').close();
                                        }
                                    }]
                            }]
                        });
                        modal.show();
                    }
                }
            ]
        };

        data.Variants.VariantInfos.forEach(function (value) {
            block.items.push(blockVariantInfo(value, data.Variants.IsProfile === false));
        });
        
        return block;
    };

    return {
        viewModel: {
            //stores: {
            //    VariantInfos: {
            //        data: data.Variants.VariantInfos
            //    }
            //}
        },

        items: [
            {
                title: '2.1. Профессиональные стандарты',
                name: 'ProfStandards',
                items: {
                    xtype: 'label',
                    html: `Образовательная программа разработана на основе профессиональных стандартов (Приложение 1). 
                            Согласована с региональными работодателями – социальными партнерами 
                            <i>[в приложении размещаются акты согласования, рекомендации или иные документы, 
                                    подтверждающие согласование требований работодателей – социальных партнеров]</i> (Приложение 2).`
                },
                buttons: []
            },
            {
                title: '2.2. Траектории образовательной программы, области, объекты и типы задач профессиональной деятельности',
                name: 'VariantText',
                items: {
                    xtype: 'label',
                    html: `Профиль образовательной программы, траектории ОП (ТОП) определяются с учетом специфики видов профессиональной деятельности (ВПД) 
                            и профессиональных стандартов (ПС) соответствующего квалификационного уровня в определенной области (и/или сфере) деятельности, 
                            особенностей объектов профессиональной деятельности и типов решаемых выпускниками задач профессиональной деятельности (Табл. 1).<br><br>
                            <i>[Траектории в образовательной программе допускается вводить при наличии в них специфики осваиваемых областей(сфер), 
                                объектов и типов задач профессиональной деятельности в соответствии с профессиональными стандартами. 
                                В случае отсутствия выраженной специфики в данном абзаце указывается только наименование образовательной программы (ее профиль). 
                                Траектории ОП (профиль) и типы профессиональных задач должны соответствовать указанным 
                                в профессиональных стандартах обобщенным трудовым функциям (ОТФ) и/или трудовым функциям (ТФ) определенного квалификационного уровня: 
                                как правило, 6 квалификационный уровень – уровень бакалавриата, 7 квалификационный уровень – уровень магистратуры, 
                                6-7 уровни – уровень специалитета.].</i>`
                },
                buttons: []
            },
            tableBlock1()
        ]
    };
}