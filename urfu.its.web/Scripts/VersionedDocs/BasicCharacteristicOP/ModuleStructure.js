function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };

    var capacitySumGridId = 'capacitySumGridId';

    var numbersTest = /^\d{1,3}$/;
    Ext.apply(Ext.form.field.VTypes, {
        numbers: function (val, field) {
            return numbersTest.test(val);
        },
        numbersText: 'Допустимы только числа'
    });

    // ключи к словарю Urfu.Its.VersionedDocs.Documents.Shared.ModuleStructure.cs ModuleAdditionals
    // в блоке Модули в сумму должны входить з.е. только этих записей. Снимать с них галку нельзя!
    var modulePartIds = ['requiredPart', 'formedPart']; 

    function required(value) {
        return (value !== undefined && value.length > 0) ? true : "Поле не может быть пустым";
    }

    var moduleAttributes = {
        title: 'Блок 1. Дисциплины (модули)',
        store: '{modules}',
        columnTitle: 'Дисциплины (модули)',
        hideCapacity: false,
        defaultCapacityText: '',
        editableBlock: true
    };

    var practiceAttributes = {
        title: 'Блок 2. Практика',
        store: '{practices}',
        columnTitle: 'Практика',
        hideCapacity: false,
        defaultCapacityText: '',
        editableBlock: true
    };

    var giaAttributes = {
        title: 'Блок 3. Государственная итоговая аттестация',
        store: '{gia}',
        columnTitle: 'Государственная итоговая аттестация',
        hideCapacity: false,
        defaultCapacityText: '',
        editableBlock: true
    };

    var facultativesAttributes = {
        title: 'Блок 4. Факультативы',
        store: '{facultatives}',
        columnTitle: 'Факультативы',
        hideCapacity: true,
        defaultCapacityText: 'не менее 3 з.е.',
        editableBlock: false
    };

    var notificationId = 'notificationId';

    function changeFinalSum(panel) {

        var gia = sumBlock(panel, 'gia');
        var practice = sumBlock(panel, 'practices');
        var modules = sumBlock(panel, 'modules');

        var sum = gia + practice + modules;

        sumStore.data.items[0].data.sum = sum;
        
        Ext.getCmp(notificationId).setHidden(sum === data.ModuleStructure.RequiredSum);
    
        Ext.getCmp(capacitySumGridId).getView().refresh();
    }

    function sumBlock(panel, storeName) {
        var sum = 0;
        panel.lookupViewModel().getStore(storeName).data.items.forEach(function (item, index, arr) {
            if (item.data.Selected && item.data.Capacity !== "" && (storeName === 'modules' && modulePartIds.includes(item.data.Id) || storeName !== 'modules'))
                sum += Number(item.data.Capacity);
        });
        return sum;
    }

    var moduleTableBlock = function (att) {
        return {
            items: [
                {
                    xtype: 'label',
                    text: att.title,
                    style: captionStyle
                },
                {
                    xtype: 'grid',
                    margin: '0 0 20 0',
                    bind: {
                        store: att.store
                    },
                    plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
                        clicksToEdit: 1,
                        listeners: {
                            edit: function (editor, context, eOpts) {
                                changeFinalSum(this.grid.up().up().up());
                            }
                        }
                    })],
                    viewConfig: {
                        markDirty: false
                    },
                    columns: [
                        { xtype: 'rownumberer', width: 50 },
                        {
                            header: '',
                            dataIndex: 'Selected',
                            width: 70,
                            xtype: 'checkcolumn',
                            listeners: {
                                beforecheckchange: function (t, rowIndex, checked) {
                                    var record = t.up().up().getStore().getAt(rowIndex).data;
                                    if (modulePartIds.includes(record.Id) || !att.editableBlock) // ключи к словарю Urfu.Its.VersionedDocs.Documents.Shared.ModuleStructure.cs ModuleAdditionals
                                        return false;
                                    return true;
                                },
                                checkchange: function () {
                                    changeFinalSum(this.up().up().up().up().up());
                                }
                            }
                        },
                        {
                            header: 'Включен в ядро',
                            dataIndex: 'IncludeInCore',
                            width: 100,
                            cellWrap: true,
                            renderer: function (val) { return val ? 'Да' : 'Нет'; }
                        },
                        {
                            header: att.columnTitle,
                            dataIndex: 'Name',
                            width: 500,
                            cellWrap: true,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Объем программы (з.е.)',
                            dataIndex: 'Capacity',
                            hidden: att.hideCapacity,
                            width: 150,
                            editor: {
                                xtype: 'textfield',
                                vtype: 'numbers',
                                validator: required
                            },
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Объем программы (з.е.)',
                            hidden: !att.hideCapacity,
                            width: 150,
                            renderer: function (val) { return att.defaultCapacityText; }
                        }
                    ]
                }
            ]
        };
    };

    var sumStore = Ext.create("Ext.data.Store", {
        data: [{
            name: 'Объем образовательной программы (з.е.)',
            sum: data.ModuleStructure.Sum
        }]
    });

    return {
        viewModel: {
            stores: {
                modules: {
                    data: data.ModuleStructure.Modules
                },
                practices: {
                    data: data.ModuleStructure.Practices
                },
                gia: {
                    data: data.ModuleStructure.Gia
                },
                facultatives: {
                    data: data.ModuleStructure.Facultative
                }
            }
        },

        items: [
            {
                name: 'ModuleStructure',
                contentReader: function (content, vm) {
                    content.Practices = selectStoreItemsData(vm.get('practices'));
                    content.Modules = selectStoreItemsData(vm.get('modules'));
                    content.Gia = selectStoreItemsData(vm.get('gia'));
                    content.Facultative = selectStoreItemsData(vm.get('facultatives'));
                    content.SelectedPractices = content.Practices.filter(m => m.Selected && m.Capacity > 0); 
                    content.SelectedModules = content.Modules.filter(m => m.Selected);
                    content.SelectedGia = content.Gia.filter(m => m.Selected && m.Capacity > 0);
                    content.SelectedFacultative = content.Facultative.filter(m => m.Selected && m.Capacity > 0);
                    return content;
                },
                items: [
                    moduleTableBlock(moduleAttributes),
                    moduleTableBlock(practiceAttributes),
                    moduleTableBlock(giaAttributes),
                    moduleTableBlock(facultativesAttributes),
                    
                    {
                        xtype: 'grid',
                        store: sumStore,
                        id: capacitySumGridId,
                        columns: [
                            { xtype: 'rownumberer', width: 50 },
                            { width: 70 },
                            { width: 100 },
                            {
                                header: '',
                                dataIndex: 'name',
                                width: 500,
                                cellWrap: true,
                                renderer: Ext.util.Format.htmlEncode
                            },
                            {
                                header: '',
                                dataIndex: 'sum',
                                width: 150,
                                renderer: Ext.util.Format.htmlEncode
                            }
                        ]
                    },
                    {
                        xtype: 'label',
                        id: notificationId,
                        hidden: data.ModuleStructure.Sum === data.ModuleStructure.RequiredSum,
                        style: {
                            'text-align': 'right'
                        },
                        html: `<font color="red">Объем образовательной программы должен быть <b>${data.ModuleStructure.RequiredSum} з.е.</b></font>`
                    }
                ]
            }
        ]
    };
}