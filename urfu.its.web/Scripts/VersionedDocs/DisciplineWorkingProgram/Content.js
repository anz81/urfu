function uiInit(documentId, documentType, data) {
    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1,
        listeners: {
            beforeEdit: function (editor, context, eOpts) {
                var height = context.row.clientHeight;
                var editor = context.column.getEditor();
                editor.setHeight(height);
            }
        }
    });

    return {
        viewModel: {
            stores: {
                Sections: {
                    data: data.Sections
                }
            }
        },

        items: [{
            layout: { type: 'vbox', align: 'stretch' },
            name: 'Sections',

            contentUpdater: function(content, vm) {
                vm.get(this.name).setData(content);
            },

            items: [{
                xtype: 'list',
                plugins: [cellEditing],
                tbar: [{
                    xtype: 'button',
                    text: 'Добавить раздел',
                    handler: function(btn) {
                        btn.up('grid').getStore().add({
                            Name: null,
                            Content: null
                        });
                    }
                }],
                bind: {
                    store: '{Sections}'                        
                },
                columns: [{
                    xtype: 'rownumberer',
                    header: 'Код раздела',
                    width: 100,
                    renderer: function(colIndex, meta, rec, rowIndex) {
                        return "Р" + (rowIndex + 1);
                    }
                }, {
                    header: 'Раздел, тема дисциплины*',
                    dataIndex: 'Name',
                    editor: {
                        xtype: 'textfield',
                        allowBlank: false
                    },
                    flex: 1
                }, {
                    cellWrap: true,
                    header: 'Содержание',
                    dataIndex: 'Content',
                    variableRowHeight: true,
                    tdCls: 'multiline-column',
                    editor: {
                        xtype: 'textareafield',
                        maxLength: 10000,
                        enforceMaxLength: true,
                        grow: true,
                        completeOnEnter: false
                    },
                    flex: 2,
                    sortable: false,
                    sortableColumn: false
                }]
            }]
        }]
	}
}