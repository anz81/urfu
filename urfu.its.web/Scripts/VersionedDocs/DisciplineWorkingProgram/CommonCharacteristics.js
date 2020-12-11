function uiInit(documentId, documentType, data, docSchema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };
    var textAlignRight = { 'text-align': 'right' };  

    var disciplineScopesItems = [];
    data.DisciplineScopes.forEach(function (ds) {        
        var fdp = data.Fdps.filter(function(fdp) { return fdp.ItemId === ds.FdpId; })[0];

        var getSemesterNumber = function(i) {
            return (i + ds.StartSemester);
        }

        var store = Ext.create('Ext.data.ArrayStore',
            {
                autoLoad: true,
                data: [ds.SelfWork.Semesters],
                fields: ds.SelfWork.Semesters.map(function (s, i) { return 's' + getSemesterNumber(i); })
            });

        var columns = ds.SelfWork.Semesters.map(function(s, i) {
            var semesterNumber = getSemesterNumber(i);
            return {
                text: semesterNumber,
                dataIndex: 's' + semesterNumber,
                flex: 1,
                editor: {
                    xtype: 'numberfield'
                },
                sortable: false,
                sortableColumn: false
            }
        });

        disciplineScopesItems.push({
            margin: '15 0 0 0',
            xtype: 'label',
            textAlign: 'right',
            text: fdp.FamType + ' форма',
            style: Ext.apply({}, textAlignRight, captionStyle)
        }, {
            xtype: 'label',
            textAlign: 'right',
            text: 'Направление: ' + fdp.DirectionCode,
            style: Ext.apply({}, textAlignRight, captionStyle)
        },
        {
            xtype: 'label',
            textAlign: 'right',
            text: 'УП №' + fdp.PlanNumber + " (" + fdp.PlanVersionTitle + ")",
            style: Ext.apply({}, textAlignRight, captionStyle)
        },
        {
            xtype: 'label',
            text: 'Самостоятельная работа студентов, включая все виды текущей аттестации',
            style: Ext.apply({}, captionStyle)
        }, {
            xtype: 'grid',
            //reference: 'DisciplineScopeGrid',
            viewConfig: {
                markDirty: false,
            },
            plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1,
                listeners: {
                    beforeEdit: function (editor, context, eOpts) {
                        var height = context.row.clientHeight;
                        var editor = context.column.getEditor();
                        editor.setHeight(height);
                    }
                }
            })],
            store: store,
            columns: columns
        });
    });
    
    return {
        viewModel: {
            stores: {
                Directions: {
                    autoLoad: true,
                    data: data.Directions
                },
                Fdps: {
                    autoLoad: true,
                    data: data.Fdps                  
                },
                languages: {
                    data: [['русский'], ['английский']],
                    type: 'array',
                    fields: ['name']
                },
                CompetenceResults: {
                    data: data.PlannedLearningOutcomes.Items.map(function(item) {
                        var correctItem = Ext.apply({}, item);
                        correctItem.Competences = item.Competences.map(function(c) { return c.Id; });
                        return correctItem;
                    })
                },
                competences: {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: '/DisciplineWorkingProgram/GetCompetences/' + window.location.search,
                        reader: { type: 'json' },
                        extraParams: {
                            documentId: documentId
                        }
                    },
                    listeners: {
                        load: function() {
                            Ext.ComponentQuery.query('grid')[0].getView().refresh();
                        }
                    }
                }
            }
        },

        items: [{
            layout: { type: 'vbox', align: 'stretch' },
            name: 'Annotation',
            items: [{
                xtype: 'label',
                text: '1.1.	Аннотация содержания дисциплины',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[описание места дисциплины в структуре модуля, связи с другими дисциплинами модуля, краткая характеристика содержательных и методических особенностей дисциплины].'
            }, {
                xtype: 'textarea',
                height: 150,
                name: 'Annotation',
                bind: '{Annotation}'
            }]
        }, {
            name: 'Language',
            contentReader: function(content) {
                return !content ? emptyData[this.name] : content;
            },

            layout: { type: 'vbox', align: 'stretch' },
            
            items: [{
                xtype: 'label',
                text: '1.2. Язык реализации программы',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[указываются языки, на которых реализуется программа дисциплины]'
            }, {
                xtype: 'comboedit',
                name: 'Language',
                valueField: 'name',
                displayField: 'name',
                bind: {
                    value: '{Language}',
                    store: '{languages}'
                }
            }]
        }, {
            name: 'PlannedLearningOutcomes',
            contentReader: function (content, vm) {
                var competenceStore = vm.get('competences');
                if (competenceStore) {
                    var items = selectStoreItemsData(vm.get('CompetenceResults'));
                    var correctItems = items.map(function(item) {
                        var correctItem = Ext.apply({}, item);
                        correctItem.Competences = item.Competences.map(function(cId) {
                            var rec = competenceStore.findRecord('Id', cId);
                            return rec.getData();
                        });
                        return correctItem;
                    });
                    content.Items = correctItems;
                }
                return content;
            },

            hasChanges: function () {
                var vm = findRootVm(this);
                var savedData = data[this.name];
                var preparedData = prepareDataToSave(vm, [this], docSchema);
                var currentData = preparedData[this.name];
                return hasChanges(savedData, currentData, this);
            },

            layout: { type: 'vbox', align: 'stretch' },
            
            items: [{
                xtype: 'label',
                text: '1.3. Планируемые  результаты обучения по дисциплине',
                style: captionStyle
            }, {
                xtype: 'label',
                text: 'Результатом обучения в рамках дисциплины является формирование у студента следующих компетенций:'
            }, {
                xtype: 'label',
                text: ' [перечень формируемых компетенций в соответствии с разделом 4.1 и 4.2. Программы модуля с указанием их кодов]'
            }, {
                xtype: 'panel',
                layout: { type: 'vbox', align: 'stretch' },
                header: false,
                items: [{
                    xtype: 'list',
                    plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
                        clicksToEdit: 1,
                        listeners: {
                            beforeEdit: function (editor, context, eOpts) {
                                var height = context.row.clientHeight;
                                var editor = context.column.getEditor();
                                editor.setHeight(height);
                            }
                        }
                    })],
                    tbar: [{
                        xtype: 'button',
                        text: 'Добавить блок текста',
                        handler: function(btn) {
                            btn.up('grid').getStore().add({
                                Competences: [],
                                Description: null
                            });
                        }
                    }],
                    bind: {
                        store: '{CompetenceResults}'                        
                    },
                    infinite: false,
                    columns: [{
                        cellWrap: true,
                        header: 'Описание',
                        dataIndex: 'Description',
                        variableRowHeight: true,
                        tdCls: 'multiline-column',
                        editor: {
                            xtype: 'textareafield',
                            maxLength: 10000,
                            enforceMaxLength: true,
                            grow: true,
                            completeOnEnter: false                            
                        },
                        flex: 1,
                        sortable: false,
                        sortableColumn: false
                    }, {
                        //xtype: 'widgetcolumn',
                        cellWrap: true,
                        header: 'Компетенции',
                        dataIndex: 'Competences',
                        variableRowHeight: true,
                        tdCls: 'multiline-column',
                        flex: 1,
                        editor: {
                            xtype: 'tagfield',
                            allowBlank: false,
                            grow: true,
                            bind: {
                                store: '{competences}'
                            },
                            valueField: 'Id',
                            displayField: 'Code',
                            queryMode: 'local',
                            filterPickList: false
                        },
                        renderer: function(ids) {
                            var vm = this.lookupViewModel();
                            return ids.map(function(id) {
                                return vm.get('competences').findRecord('Id', id).get('Code');
                            }).join(' ');
                        },
                        width: 370,
                        sortable: false,
                        sortableColumn: false
                    }]
                }, {                        
                    xtype: 'label',
                    margin: '10 0 10 0',
                    text: ' В результате освоения дисциплины студент должен:'
                }, {
                    xtype: 'label',
                    text: 'Знать: [текст]'
                }, {
                    xtype: 'textareafield',
                    bind: '{PlannedLearningOutcomes.MustKnow}',
                    height: 150,
                    width: 600
                }, {
                    xtype: 'label',
                    text: 'Уметь: [текст]'
                }, {
                    xtype: 'textareafield',
                    bind: '{PlannedLearningOutcomes.MustOwn}',
                    height: 150,
                    width: 600
                }, {
                    xtype: 'label',
                    text: 'Владеть (демонстрировать навыки и опыт деятельности): [текст]'
                }, {
                    xtype: 'textareafield',
                    bind: '{PlannedLearningOutcomes.MustBeAbleTo}',
                    height: 150,
                    width: 600
                }]
            }]
        }, {
            contentReader: function (scopes, vm) {
                var grids = this.query('grid');
                var items = grids.map(function (g) { return g.getStore().getData().items[0].getData(); });
                scopes.forEach(function(s, i) {
                    var item = items[i];
                    s.SelfWork.Semesters = Object.keys(item).filter(function(k) {
                        return k[0] === 's';
                    }).map(function(k) {
                        return item[k];
                    });
                });
                return scopes;
            },
            name: 'DisciplineScopes',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '1.4 Объем дисциплины',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[таблицы формируются отдельно по каждой форме обучения]'
            }, {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: disciplineScopesItems
            }]
        }]
	}
}