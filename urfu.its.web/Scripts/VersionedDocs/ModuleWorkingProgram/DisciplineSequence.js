function uiInit(documentId, documentType, data, schema, emptyData, canEdit) {
    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1        
    });

    var defaultPriority = 'Порядок не важен';
    var priorities = [{
        Value: 0,
        DisplayName: defaultPriority
    }];
    for (var i = 0; i < data.DisciplineSequence.Items.length; i++) {
        priorities.push({
            Value: i + 1,
            DisplayName: i + 1
        });
    }

    return {
        viewModel: {
            stores: {
                DisciplineSequenceItems: {
                    data: data.DisciplineSequence.Items
                },
                priorities: {
                    data: priorities
                }
            }
        },

        items: [{
            name: 'DisciplineSequence',
            contentReader: function(content, vm) {
                var items = vm.get('DisciplineSequenceItems').getData().items.map(function (i) { return i.getData(); });
                content.Items = items;
                return content;
            },
            hideHeaders: true,
            items: {
                xtype: 'grid',
                plugins: [cellEditing],
                bind: '{DisciplineSequenceItems}',
                viewConfig: {
                    markDirty: false
                },
                columns: [{
                    dataIndex: 'DisciplineName',
                    width: 400
                }, {
                    dataIndex: 'Number',
                    editor: {
                        xtype: 'combobox',
                        displayField: 'DisplayName',
                        valueField: 'Value',
                        /*editable: false,
                        readOnly: false,*/
                        forceSelection: true,
                        queryMode: 'local',
                        allowBlank: false,                        
                        bind: {
                            store: '{priorities}'
                        }
                    },
                    renderer: function(val) {
                        if (!val)
                            return defaultPriority;
                        return val;
                    },
                    width: 200
                }],
                bbar: [{
                    xtype: 'checkboxfield',
                    fieldLabel: 'Требования не предъявляются',
                    labelWidth: 300,
                    bind: '{DisciplineSequence.NoRequirements}'
                }]
            }
        }]
	}
}