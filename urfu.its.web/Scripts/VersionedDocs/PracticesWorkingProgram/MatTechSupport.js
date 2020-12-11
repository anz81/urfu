function uiInit(documentId, documentType, data, schema, emptyData) {
    Ext.tip.QuickTipManager.init();
    var captionStyle = { fontWeight: 'bold' };
    var textAlignRight = { 'text-align': 'right' };
    var italicStyle = { fontStyle: 'italic' };
    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });
    
    return {
        viewModel: {
            data: data
        },
        listeners: {
            afterrender: function () {
                var vm = this.lookupViewModel();
            }
        },
        
        items: [
            {
                title: 'Все виды практик',
                name: 'PracticeTypes',
                items: [
                    {
                        xtype: 'textarea',
                        height: 150,
                        name: 'PracticeTypes',
                        bind: '{PracticeTypes}'
                    },
                ]
            },
       
            {
                name: 'PracticeMatTechSupportStructure',
                header: false,
                border: true,
                margin: '0 0 10 0',
                bodyPadding: 6,
                contentReader: function (content, vm) {
                    return this.query('grid').map(function (g) {
                        return g.getStore().getData().items.map(function (i) { return i.getData(); });
                    })[0];
                },
                layout: { type: 'vbox', align: 'stretch' },
                defaults: {
                    labelWidth: 200
                },
                items: [
                    {
                        xtype: 'grid',
                        viewConfig: {
                            markDirty: false
                        },
                        cls: 'grid-header-minimal',
                        store: {
                            data: data.PracticeMatTechSupportStructure
                        },
                        plugins: [cellEditing],
                        columns: [
                            {
                                dataIndex: 'DisciplineUid',
                                hidden: true
                            },
                            {
                                text: '№',
                                xtype: 'rownumberer',
                                width: 50
                            },
                            {
                                text: 'Вид практики',
                                dataIndex: 'Title',
                                width: 300,
                                cellWrap: true,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            },
                            {
                                text: 'Перечень необходимого материально-технического обеспечения',
                                dataIndex: 'MatTechSupport',
                                width: 800,
                                cellWrap: true,
                                editor: {
                                    xtype: 'textarea',
                                    allowBlank: true,
                                    height: 100
                                },
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            }
                        ]
                    }]
            }

        ]
    }
}