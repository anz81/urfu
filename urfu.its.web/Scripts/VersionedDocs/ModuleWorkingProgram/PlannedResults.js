function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };
    var textAlignRight = { 'text-align': 'right' };    

    return {
        viewModel: {
            stores: {
                profiles: {
                    data: data.Profiles
                },
                allUsedCompetences: {
                    data:[]
                }
            }
        },

        items: [{
            xtype: 'container',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '4.1.	Планируемые результаты освоения модуля и составляющие их компетенции',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[формулируются разработчиками модуля на основе Табл.4 и Табл.2  тех ОП, для которых реализуется модуль]',                
                style: italicStyle
            }]
        }, {
            name: 'PlannedResults',
            listeners: {
                saved: function() {
                    var vm = this.lookupViewModel();
                    updateUsedCompetencesStore(vm);
                }
            },
            viewModel: {
                data: {
                    profileId: null,
                    isProfileLocked: false,
                    selectedUniversalCompetences: [],
                    selectedEduResults: []
                },
                formulas: {
                    profileLockButtonText: function (get) {
                        if (!get('isProfileLocked'))
                            return 'Выбрать';
                        else
                            return 'Сбросить';
                    }
                },
                stores: {
                    universalCompetences: {
                        autoLoad: '{profileId}',
                        proxy: {
                            type: 'ajax',
                            url: '/ModuleWorkingProgram/GetUniversalCompetences/' + window.location.search,
                            reader: { type: 'json' },
                            extraParams: {
                                documentType: documentType,
                                documentId: documentId,
                                profileId: '{profileId}'
                            }
                        }
                    },
                    eduResults: {
                        autoLoad: '{profileId}',
                        proxy: {
                            type: 'ajax',
                            url: '/ModuleWorkingProgram/GetEduResults/' + window.location.search,
                            reader: { type: 'json' },
                            extraParams: {
                                documentType: documentType,
                                documentId: documentId,
                                profileId: '{profileId}'
                            }
                        }
                    },
                    eduResultsCompetences: {
                        data: [],
                        proxy: {
                            type: 'memory'
                        }
                    },
                    competences: {
                        autoLoad: '{profileId}',
                        proxy: {
                            type: 'ajax',
                            url: '/ModuleWorkingProgram/GetCompetences/' + window.location.search,
                            reader: { type: 'json' },
                            extraParams: {
                                documentId: documentId,
                                profileId: '{profileId}'                                
                            },
                        }
                    }
                }
            },
            tbar: [{
                xtype: 'panel',
                border: true,
                bodyPadding: 6,
                width: 720,
                layout: { type: 'vbox', align: 'stretch' },
                tbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    defaults: {
                        labelWidth: 230,
                        margin: '0 0 10 0',
                        width: 700,
                        grow: true
                    },
                    items: [{
                        xtype: 'container',
                        layout: { type: 'hbox' },
                        items: [{
                            xtype: 'comboedit',
                            labelWidth: 230,
                            fieldLabel: 'Образовательная программа',
                            displayField: 'Code',
                            valueField: 'Id',
                            width: 600,
                            bind: {
                                value: '{profileId}',
                                store: '{profiles}',
                                disabled: '{isProfileLocked}'
                            }
                        }, {
                            xtype: 'button',
                            margin: '0 0 0 6',
                            enableToggle: true,
                            bind: {
                                pressed: '{isProfileLocked}',
                                disabled: '{!profileId}',
                                text: '{profileLockButtonText}'
                            },
                            listeners: {
                                toggle: function (sender, pressed, eOpts) {
                                    var vm = findRootVm(sender);
                                    var clearForm = false;
                                    var oldPressed = this.lookupViewModel().get(sender.bind.pressed.stub.name);
                                    if (!oldPressed || (clearForm = vm.confirmMessage('Данные формы будут сброшены. Продолжить?'))) {
                                        this.lookupViewModel().set(sender.bind.pressed.stub.name, pressed);
                                        if (clearForm) {
                                            var localVm = sender.lookupViewModel();
                                            localVm.set('selectedEduResults', []);
                                            localVm.set('selectedUniversalCompetences', []);
                                            localVm.get('eduResultsCompetences').removeAll();
                                        }
                                    }
                                }
                            }
                        }]
                    }, {
                        xtype: 'tagfield',
                        fieldLabel: 'Универсальные компетенции',
                        bind: {
                            store: '{universalCompetences}',
                            disabled: '{!isProfileLocked}',
                            value: '{selectedUniversalCompetences}'
                        },
                        valueField: 'Id',
                        displayField: 'Code',
                        queryMode: 'local',
                        filterPickList: false
                    }, {
                        xtype: 'tagfield',
                        fieldLabel: 'Результаты обучения',
                        bind: {
                            store: '{eduResults}',
                            disabled: '{!isProfileLocked}',
                            value: '{selectedEduResults}'
                        },
                        valueField: 'Id',
                        displayField: 'Name',
                        queryMode: 'local',
                        filterPickList: false,
                        tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                            '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                            '<div><b>{Name}</b> - {Description}</div>',
                            '</div>',
                            '</tpl>'
                        ),
                        listeners: {
                            /*select: function (field, records) {
                                console.time('select');
                                var vm = field.lookupViewModel();
                                var listStore = vm.get('eduResultsCompetences');
                                for (var i = 0; i < records.length; i++) {
                                    var record = records[i];
                                    record.set('Competences', []);
                                    listStore.add(record);
                                }
                                console.timeEnd('select');
                            },*/
                            change: function (field, values, oldValues) {
                                console.time('change');
                                var addedValues = values.filter(function (v) { return oldValues.indexOf(v) < 0; });
                                var removedValues = oldValues.filter(function(v) { return values.indexOf(v) < 0; });

                                var vm = field.lookupViewModel();
                                var listStore = vm.get('eduResultsCompetences');
                                var currentStore = this.getStore();

                                listStore.remove(removedValues.map(function (id) {
                                    return listStore.findRecord('Id', id);
                                }));
                                listStore.add(addedValues.map(function (id) {
                                    return currentStore.findRecord('Id', id);
                                }));

                                console.timeEnd('change');
                            }
                        }
                    }]
                }],
                items: [{
                    xtype: 'grid',
                    stripeRows: false,
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
                    viewConfig: {
                        markDirty: false,
                    },
                    enableLocking: false,  // turn off column lock context items
                    enableColumnMove: false,  // turn off column reorder drag drop
                    enableColumnHide: false,  // turn off column reorder drag drop
                    sortableColumns: false,
                    columns: [{
                        text: 'РО',
                        dataIndex: 'Name',
                        width: 200
                    }, {
                        width: 500,
                        cellWrap: true,
                        text: 'Компетенции',
                        dataIndex: 'Competences',
                        variableRowHeight: true,
                        tdCls: 'multiline-column',
                        flex: 1,
                        editor: {
                            xtype: 'tagfield',
                            grow: true,
                            bind: {
                                store: '{competences}'
                            },
                            valueField: 'Id',
                            displayField: 'Code',
                            queryMode: 'local',
                            tpl: Ext.create('Ext.XTemplate',
                                '<tpl for=".">',
                                '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                                '<div><b>{Code}</b> - {Content}</div>',
                                '</div>',
                                '</tpl>'
                            ),
                            filterPickList: false                            
                        },
                        renderer: function (ids) {
                            var vm = this.lookupViewModel();
                            return ids.map(function (id) {
                                return vm.get('competences').findRecord('Id', id).get('Code');
                            }).join(' ');
                        },
                    }],
                    bind: {
                        store: '{eduResultsCompetences}'
                    }
                }],
                bbar: [{
                    xtype: 'button',
                    text: 'Добавить',
                    bind: {
                        disabled: '{!isProfileLocked}'
                    },
                    handler: function () {
                        var vm = this.lookupViewModel();
                        var rootVm = findRootVm(this);
                        var resultCompetencesStore = vm.get('eduResultsCompetences');
                        var universalCompetences = vm.get('universalCompetences');
                        var competences = vm.get('competences');
                        var selectedUniversalCompetences = vm.get('selectedUniversalCompetences');
                        var profilesStore = vm.get('profiles');
                        var plannedResults = rootVm.get('PlannedResults');
                        var profileId = vm.get('profileId');
                        
                        var plannedResult = {
                            ProfileId: profileId,
                            ProfileCode: profilesStore.findRecord('Id', profileId).get('Code'),
                            UniversalCompetences: selectedUniversalCompetences.map(function(c) {
                                return universalCompetences.findRecord('Id', c).getData();
                            }),
                            Results: selectStoreItemsData(resultCompetencesStore).map(function(rc) {
                                var clone = Ext.clone(rc);
                                clone.Competences = rc.Competences.map(function(c) {
                                    return competences.findRecord('Id', c).getData();
                                });
                                return clone;
                            })
                        };
                        plannedResults.push(plannedResult);
                        
                        vm.set('selectedEduResults', []);
                        vm.set('selectedUniversalCompetences', []);
                        vm.get('eduResultsCompetences').removeAll();
                        vm.set('profileId', null);
                        vm.set('isProfileLocked', false);
                        
                        buildTableItems(Ext.ComponentQuery.query('#plannedResultsTable')[0], rootVm);
                    }
                }]
            }],
            items: {
                xtype: 'container',
                itemId: 'plannedResultsTable',
                cls: 'table-all-borders table-cell-padding-normal',
                layout: { type: 'table', columns: 6 },
                listeners: {
                    afterrender: function() {
                        var vm = this.lookupViewModel();
                        buildTableItems(this, vm);
                    }
                }
            }
        }, {
            xtype: 'container',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '4.2.Распределение формирования компетенций по дисциплинам модуля',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[отметить звездочкой или другим символом  компетенции, формируемые каждой дисциплиной модуля]',
                style: italicStyle
            }]
        }, {
            name: 'DisciplineCompetences',
            contentUpdater: function (content, vm) {
                this.query('grid').forEach(function (g, i) {
                    var fdpItem = content.filter(function(fdpItem) {
                        return fdpItem.FdpId === g.fdpId;
                    })[0];
                    if (fdpItem !== null) {
                        g.getStore().setData(fdpItem.Items);
                        g.getView().refresh();
                    }                    
                });
                //updateUsedCompetencesStore(vm);
            },
            contentReader: function(content, vm) {
                var result = this.query('grid').map(function(g) {
                    var rcs = selectStoreItemsData(g.getStore());
                    return {
                        FdpId: g.fdpId,
                        Items: rcs
                    };
                });
                return result;
            },
            items: prepareFdpLinkItems(Ext.clone(data.DisciplineCompetences)),
            listeners: {
                beforerender: function () {
                    var vm = this.lookupViewModel();
                    updateUsedCompetencesStore(vm);
                }
            }
        }]
    }

    function prepareFdpLinkItems(fdpForms) {
        return fdpForms.map(function (link, i) {
            var fdp = data.Fdps.filter(function (fdp) { return fdp.ItemId === link.FdpId })[0];
            var famType = fdp.FamType;
            var directionCode = fdp.DirectionCode;

            return {
                fdpId: link.FdpId,
                xtype: 'panel',
                header: false,
                border: true,
                margin: '0 0 10 0',
                bodyPadding: 6,
                reference: 'fdpLinkItemPanel',
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
                }, {
                    fdpId: link.FdpId,
                    xtype: 'grid',
                    enableLocking: false,  // turn off column lock context items
                    enableColumnMove: false,  // turn off column reorder drag drop
                    enableColumnHide: false,  // turn off column reorder drag drop
                    sortableColumns: false,
                    store: {
                        data: link.Items
                    },
                    viewConfig: {
                        markDirty: false
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
                    columns: [{
                        text: '№',
                        xtype: 'rownumberer',
                        width: 70
                    }, {
                        dataIndex: 'DisciplineDisplayName',
                        text: 'Дисциплина',
                        width: 400
                    }, {
                        width: 500,
                        cellWrap: true,
                        text: 'Компетенции',
                        dataIndex: 'CompetenceIds',
                        variableRowHeight: true,
                        tdCls: 'multiline-column',
                        flex: 1,
                        editor: {
                            xtype: 'tagfield',
                            grow: true,
                            bind: {
                                store: '{allUsedCompetences}'
                            },
                            valueField: 'Id',
                            displayField: 'Code',
                            queryMode: 'local',
                            tpl: Ext.create('Ext.XTemplate',
                                '<tpl for=".">',
                                '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                                '<div><b>{Code}</b> - {Content}</div>',
                                '</div>',
                                '</tpl>'
                            ),
                            filterPickList: false
                        },
                        renderer: function (ids) {
                            var vm = this.lookupViewModel();
                            var allUsedCompetences = vm.get('allUsedCompetences');
                            return ids.map(function (id) {
                                return allUsedCompetences.findRecord('Id', id).get('Code');
                            }).join(' ');
                        },
                    }]
                }]
            }
        });
    }

    function buildTableItems(panel, vm) {
        console.time('buildTableItems');
        var items = [{
            html: 'Коды ОП, для которых реализуется модуль',
            width: 200
        }, {
            html: 'Универсальные компетенции (УОК, УОПК,УПК), формируемые при освоении модуля для нескольких ОП',
            width: 300
        }, {
            html: '',
            width: 50
        }, {
            html: 'Планируемые в ОХОП результаты обучения -РО, которые формируются при освоении модуля ',
            width: 300
        }, {
            html: 'Компетенции в соответствии с ФГОС ВО, а также дополнительные из ОХОП, формируемые при освоении модуля',
            width: 300
        }, {
            html: '',
            width: 50
        }];

        var plannedResults = vm.get('PlannedResults');
        plannedResults.forEach(function (r) {
            var profileIdItem = {
                html: r.ProfileCode,
                dataItem: r,
                rowspan: r.Results.length
            };
            var universalCompetencesItem = {
                html: r.UniversalCompetences.join('<br/>'),
                dataItem: r,
                rowspan: r.Results.length
            };
            var profileRemoveItem = {
                xtype: 'image',
                src: '/Content/Images/remove.png',
                alt: 'Удалить',
                margin: 0,
                width: 16,
                dataItem: r,
                rowspan: r.Results.length,
                listeners: {
                    el: {
                        click: function () {
                            var vm = findRootVm(this.component);
                            if (!vm.confirmMessage('Удалить строку?'))
                                return;
                            Ext.Array.remove(plannedResults, this.component.dataItem);
                            buildTableItems(panel, vm);
                        }
                    }
                }
            };
            items.push(profileIdItem, universalCompetencesItem, profileRemoveItem);

            if (!r.Results.length) {
                items.push({}, {}, {});
            } else {
                r.Results.forEach(function (rc) {
                    var roItem = {
                        html: rc.Name + ' ' + rc.Description,
                        dataItem: rc
                    };
                    var competencesItem = {
                        html: rc.Competences.map(function (c) { return c.Content + ' (' + c.Code + ')' }).join('; '),
                        dataItem: rc
                    };
                    var rcRemoveItem = {
                        xtype: 'image',
                        src: '/Content/Images/remove.png',
                        alt: 'Удалить',
                        margin: 0,
                        width: 16,
                        dataItem: rc,
                        listeners: {
                            el: {
                                click: function () {
                                    var vm = findRootVm(this.component);
                                    if (!vm.confirmMessage('Удалить строку?'))
                                        return;
                                    Ext.Array.remove(r.Results, this.component.dataItem);
                                    buildTableItems(panel, vm);
                                }
                            }
                        }
                    };
                    items.push(roItem, competencesItem, rcRemoveItem);
                });
            }
        });
        console.timeEnd('buildTableItems');

        console.time('buildTableItemsUpdateUI');
        //panel.update('');
        // add the new component
        //panel.add(items);
        // redraw the containing panel
        //panel.doLayout();

        //panel.suspendEvents();
        panel.removeAll();
        panel.add(items);
        //panel.container.component.updateLayout();
        //panel.updateLayout();
        //panel.resumeEvents();
        
        console.timeEnd('buildTableItemsUpdateUI');

        Ext.ComponentQuery.query('block[name=PlannedResults]')[0].detectChanges();
    }

    function updateUsedCompetencesStore(vm) {
        var allUsedCompetences = [];
        var prs = vm.get('PlannedResults');
        prs.forEach(function (pr) {
            pr.Results.forEach(function (rc) {
                rc.Competences.forEach(function (c) {
                    if (allUsedCompetences.every(function (r) {
                        return r.Id !== c.Id;
                    }))
                        allUsedCompetences.push(c);
                });
            });
        });

        var allCompetencesStore = vm.get('allUsedCompetences');
        allCompetencesStore.setData(allUsedCompetences);        
    }
}