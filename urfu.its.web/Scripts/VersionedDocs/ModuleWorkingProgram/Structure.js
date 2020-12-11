function uiInit(documentId, documentType, data, schema, emptyData, canEdit) {
    var captionStyle = { fontWeight: 'bold' };
    var textAlignRight = { 'text-align': 'right' };

    function prepareFdpLinkItems(fdpForms) {
        return fdpForms.map(function (link, i) {
            var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1
            });

            var fdp = data.Fdps.filter(function (fdp) { return fdp.ItemId === link.FdpId })[0];
            var famType = fdp.FamType;
            var directionCode = fdp.DirectionCode;

            return {
                xtype: 'panel',
                header: false,
                border: true,
                margin: '0 0 10 0',
                bodyPadding: 6,
                reference: 'fdpLinkItemPanel',
                //itemId: 'fdpLinkItemPanel',
                layout: { type: 'vbox', align: 'stretch' },
                defaults: {
                    labelWidth: 200
                },
                items: [{
                    xtype: 'label',
                    textAlign: 'right',
                    text: famType + ' форма',
                    style: Ext.apply({}, textAlignRight, captionStyle)
                }, {
                    xtype: 'label',
                    textAlign: 'right',
                    text: 'Направление: ' + directionCode,
                    style: Ext.apply({}, textAlignRight, captionStyle)
                    },
                    {
                        xtype: 'label',
                        textAlign: 'right',
                        text: 'УП №' + fdp.PlanNumber + " (" + fdp.PlanVersionTitle + ")",
                        style: Ext.apply({}, textAlignRight, captionStyle)
                    },
                    {
                    fdpId: link.FdpId,
                    xtype: 'grid',
                    viewConfig: {
                        markDirty: false
                    },
                    cls: 'grid-header-minimal',
                    store: {
                        data: link.Items
                    },                    
                    plugins: [cellEditing],
                    columns: [{
                        text: '№',
                        xtype: 'rownumberer',
                        width: 50
                    }, {
                        xtype: 'widgetcolumn',
                        text: 'Часть ОП: базовая (Б), вариативная – по выбору вуза (ВВ), вариативная - по выбору студента (ВС)',
                        dataIndex: 'EducationalProgramPart',
                        tdCls: 'widget-column',
                        widget: {
                            xtype: 'combobox',
                            queryMode: 'local',
                            readOnly: !canEdit,
                            displayField: 'Name',
                            valueField: 'Name',
                            allowBlank: false,
                            store: {
                                data: [['Б'], ['ВВ'], ['ВС']],
                                type: 'array',
                                fields: ['Name']
                            },
                            listeners: {
                                change: function(field, val) {
                                    field.getWidgetRecord().set('EducationalProgramPart', val);
                                }
                            }
                        }
                    }, {
                        text: 'Семестр обучения',
                        dataIndex: 'Semesters'
                    }, {
                        text: 'Наименование дисциплины',
                        dataIndex: 'DisciplineName',
                        width: 300
                    }, {
                        text: 'Объем времени, отведенный на освоение дисциплин модуля',
                        columns: [{
                            text: 'Аудиторные занятия, час.',
                            columns: [{
                                text: 'Лекции',
                                dataIndex: 'Lections'
                            }, {
                                text: 'Практики',
                                dataIndex: 'Practices'
                            }, {
                                text: 'Лабораторные',
                                dataIndex: 'Labs'
                            }, {
                                text: 'Всего',
                                dataIndex: 'AuditoryTotal'
                            }]
                        }, {
                            text: 'Самостоятельная работа, час.',
                            dataIndex: 'SelfWork'
                        }, {
                            text: 'Всего',
                            columns: [{
                                text: 'Час.',
                                dataIndex: 'TotalTime'
                            }, {
                                text: 'Зач. ед.',
                                dataIndex: 'TotalUnits'
                            }]
                        }]
                    }]
                }]
            }
        });
    }

    return {
        viewModel: {
            data: data            
        },

        listeners: {
            afterrender: function () {
                var vm = this.lookupViewModel();
            }
        },

        items: [{
            xtype: 'label',
            text: '[таблицы формируются отдельно по каждой форме обучения]'
        }, {
            contentReader: function(content, vm) {
                return this.query('grid').map(function(g) {
                    return {
                        FdpId: g.fdpId,
                        Items: g.getStore().getData().items.map(function(i) { return i.getData(); })
                    };
                });
            },
            name: 'ModuleStructures',            
            items: prepareFdpLinkItems(Ext.clone(data.ModuleStructures)),
            hasChanges: function (savedValue, currentValue) {
                return currentValue.some(function (v, i) {
                    var s = savedValue[i];
                    if (v.Items.length !== s.Items.length)
                        return true;
                    return v.Items.some(function(vi, i) {
                        var si = s.Items[i];
                        return vi.EducationalProgramPart != si.EducationalProgramPart;
                    });
                });                
            }
        }]
    }
}