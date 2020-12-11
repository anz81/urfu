function uiInit(documentId, documentType, data) {
    var captionStyle = { fontWeight: 'bold' };

    function preparePracticeSections(practiceForms) {
        return practiceForms.map(function (link, i) {
            return {
                xtype: 'panel',
                header: false,
                border: true,
                margin: '0 0 10 0',
                bodyPadding: 6,
                defaults: {
                    labelWidth: 200
                },
                items: [
                    {
                        xtype: 'label',
                        textAlign: 'left',
                        text: link.Title,
                        style: captionStyle
                    },
                    {
                        xtype: 'list',
                        itemId: 'section' + link.DisciplineUid,
                        plugins: Ext.create('Ext.grid.plugin.CellEditing', {
                            clicksToEdit: 1,
                            listeners: {
                                beforeEdit: function (editor, context, eOpts) {
                                    var height = context.row.clientHeight;
                                    var editor = context.column.getEditor();
                                    editor.setHeight(height);
                                }
                            }
                        }),
                        tbar: [{
                            xtype: 'button',
                            text: 'Добавить раздел',
                            handler: function (btn) {
                                btn.up('grid').getStore().add({
                                    Name: null,
                                    Content: null
                                });
                            }
                        }],
                        store: {
                            data: link.SectionInfo
                        },
                        columns: [{
                            text: 'Этапы (разделы) практики',
                            dataIndex: 'Name',
                            editor: {
                                xtype: 'textarea',
                                allowBlank: false
                            },
                            cellWrap: true,
                            flex: 1,
                            renderer: Urfu.renders.htmlEncodeWithToolTip
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
                    }
                ]
            }
        });
    }

    function prepareSectionStructureItems(practiceForms) {
        return practiceForms.map(function (link, i) {
            return {
                xtype: 'panel',
                header: false,
                border: true,
                margin: '0 0 10 0',
                bodyPadding: 6,
                defaults: {
                    labelWidth: 200
                },
                items: [
                    {
                        xtype: 'label',
                        textAlign: 'left',
                        text: 'Направление: ' + link.DirectionCode, //link.Title,
                        style: captionStyle
                    },
                    {
                        items: preparePracticeSections(Ext.clone(link.Sections))
                    }
                ]
            }
        });
    }

    return {
        viewModel: {
        },

        items: [{
            layout: { type: 'vbox', align: 'stretch' },
            name: 'PracticeSectionsStructure',

            contentReader: function (content, vm) {
                var clone = Ext.clone(content);
                var t = this;
                clone.forEach(function (item, index, array) {
                    clone[index].Sections.forEach(function (item2, index2, array2) {
                        clone[index].Sections[index2].SectionInfo = selectStoreItemsData(
                            t.down('#section' + clone[index].Sections[index2].DisciplineUid).getStore());
                    });
                });
                return clone;
            },

            items: [
                {
                    items: prepareSectionStructureItems(Ext.clone(data.PracticeSectionsStructure))
                }


                ]
        }]
	}
}