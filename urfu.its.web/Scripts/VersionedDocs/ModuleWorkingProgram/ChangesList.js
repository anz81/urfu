function uiInit(documentId, documentType, data, schema, emptyData) {
    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1        
    });

    return {
        viewModel: {
            stores: {
                ChangesList: {
                    data: data.ChangesList
                }
            }
        },

        items: [{
            name: 'ChangesList',
            items: {
                xtype: 'list',
                plugins: [cellEditing],
                tbar: [{
                    xtype: 'button',
                    text: 'Добавить строку',
                    handler: function (btn) {
                        btn.up('grid').getStore().add({
                            Number: null,
                            ProtocolNumber: null,
                            Date: null,
                            ListCount: null
                        });
                    }
                }],
                bind: {
                    store: '{ChangesList}'
                },
                listeners: {
                    beforeedit: function (e, context) {
                        if (context.field === 'Date') {
                            if(!(context.value instanceof Date))
                                context.value = Ext.Date.parseDate(context.value, "d.m.Y");
                        }
                    }
                },
                columns: [{
                    header: 'Номер листа изменений',
                    dataIndex: 'Number',
                    editor: {
                        xtype: 'textfield'
                    },
                    flex: 1
                }, {
                    header: 'Номер протокола заседания проектной группы модуля',
                    dataIndex: 'ProtocolNumber',
                    editor: {
                        xtype: 'textfield'
                    },
                    flex: 1
                }, {
                    header: 'Дата заседания проектной группы модуля',
                    dataIndex: 'Date',
                    renderer: function (value) {
                        if(value instanceof Date)
                            return Ext.Date.format(value, 'd.m.Y');
                        return value;
                    },
                    editor: {
                        xtype: 'datefield',
                        dateFormat: 'd.m.Y',
                        bind: {
                            value: '{Date}'
                        }
                    },                    
                    flex: 1
                }, {
                    header: 'Всего листов в документе',
                    dataIndex: 'ListCount',
                    editor: {
                        xtype: 'textfield'
                    },
                    flex: 1
                }, {
                    header: 'Подпись руководителя проектной группы  модуля',
                    flex: 1
                }]
            }
        }]
    }
}