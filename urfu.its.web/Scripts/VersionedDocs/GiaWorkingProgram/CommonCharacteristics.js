function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };
    var textAlignRight = { 'text-align': 'right' };

    var giaStructure = Ext.create('Ext.data.Store',
        {
            fields: ['name'],
            data: [
                { "name": "бакалаврской работы" },
                { "name": "магистерской диссертации" },
                { "name": "дипломной работы" },
                { "name": "дипломного проекта" },
            ]
        });
        
    

    return {
        viewModel: {
            stores: {
                profiles: {
                    data: data.Profiles
                },
                allUsedCompetences: {
                    data: []
                }
            }
        },

        items: [{
            xtype: 'container',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '1.1.	Цель государственной итоговой аттестации',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[указать перечень результатов обучения и соответствующих им компетенций, уровень сформированности которых предусматривается проверить  в рамках  ГИА, в соответствии с  табл.2 ОХОП]',
                style: italicStyle
            }]
        }, {
            name: 'PlannedResults',
            listeners: {
                saved: function () {
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
                                var removedValues = oldValues.filter(function (v) { return values.indexOf(v) < 0; });

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
                            UniversalCompetences: selectedUniversalCompetences.map(function (c) {
                                return universalCompetences.findRecord('Id', c).getData();
                            }),
                            Results: selectStoreItemsData(resultCompetencesStore).map(function (rc) {
                                var clone = Ext.clone(rc);
                                clone.Competences = rc.Competences.map(function (c) {
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
                    afterrender: function () {
                        var vm = this.lookupViewModel();
                        buildTableItems(this, vm);
                    }
                }
            }
            },
            {
            xtype: 'container',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '1.2. Структура  государственной итоговой аттестации',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[указать утвержденные Ученым советом института, где реализуется образовательная программа, виды ГИА]',
                style: italicStyle
                }]

           },
            {
                name: 'GiaStructure',
                items: [
                    {
                        xtype: 'container',
                        layout: {
                            type: 'hbox',
                            align: 'center'
                        },
                        margin: "8 0 0 0",
                        items: [{
                            fieldLabel: 'Протокол №',
                            xtype: 'textfield',
                            bind: '{GiaStructure.GiaStructureProtocol.ProtocolNumber}',
                            width: 210
                        }, {
                            labelAlign: 'right',
                            xtype: 'datefield',
                            fieldLabel: 'от',
                            bind: {
                                value: '{GiaStructure.GiaStructureProtocol.ProtocolDate}',
                            },
                            width: 270,
                            submitFormat: 'd.m.Y',
                            //rawFormat: 'd.m.Y',
                            format: 'd.m.Y'
                        }, {
                            xtype: 'label',
                            margin: '0 0 0 8',
                            text: 'г.'
                        },
                        {
                            width: 350,
                            labelAlign: 'right',
                            fieldLabel: 'Структура ГИА',
                            xtype: 'combobox',
                            store: giaStructure,
                            bind: {
                                value: '{GiaStructure.QualificationWork}'
                            },
                            displayField: 'name',
                            valueField: 'name',
                            queryMode: 'local',
                            allowBlank: false
                        }
                    

                        ]
                    }
                ]
              
            },
            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'label',
                    text: '1.2.1.Форма проведения государственного экзамена ',
                    style: captionStyle
                }, {
                    xtype: 'label',
                    text: '[ при наличии государственного экзамена указать  форму: устный,  письменный или смешанный]',
                    style: italicStyle
                }]

            },
            {
                name: 'TotalLabor',
                items: [
                    {
                        xtype: 'container',
                        layout: {
                            type: 'hbox',
                            align: 'center'
                        },
                        //margin: "8 0 0 0",
                        items: [
                            {
                                width: 500,
                                //labelAlign: 'right',
                                xtype: 'textarea',
                                bind: {
                                    value: '{TotalLabor}'
                                },
                            }]
                    }]
            },
            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'label',
                    text: '1.3. Трудоемкость государственной итоговой аттестации',
                    style: captionStyle
                }, {
                    xtype: 'label',
                    text: 'Общая трудоемкость государственной итоговой аттестации составляет [указать в з.е. в соответствии с утвержденным учебным планом]',
                    style: italicStyle
                }]

            },
            {
                name: 'Module',
                items: [
                    {
                        xtype: 'container',
                        layout: {
                            type: 'hbox',
                            align: 'center'
                        },
                        //margin: "8 0 0 0",
                        items: [
                            {
                                width: 100,
                                //labelAlign: 'right',
                                xtype: 'numberfield',
                                hideTrigger: true,
                                editable: false,
                                bind: {
                                    value: '{Module.Capacity}'
                                },
                            }]
                    }]
            },
            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'label',
                    text: '1.4.	Время проведения государственной итоговой аттестации ',
                    style: captionStyle
                }, {
                    xtype: 'label',
                    text: '[указать сроки государственной итоговой аттестации, установленные календарным учебным графиком соответствующего учебного плана]',
                    style: italicStyle
                }]

            },
            {
                name: 'TimeOfGia',
                items: [
                    {
                        xtype: 'container',
                        layout: {
                            type: 'hbox',
                            align: 'center'
                        },
                        //margin: "8 0 0 0",
                        items: [
                            {
                                width: 500,
                                //labelAlign: 'right',
                                xtype: 'textarea',
                                bind: {
                                    value: '{TimeOfGia}'
                                },
                            }]
                    }]
            },
            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'label',
                    text: '1.5.Требования к процедуре государственной итоговой аттестации.',
                    style: captionStyle
                }, {
                    xtype: 'label',
                    text: 'Требования к порядку планирования, организации и проведения ГИА, к структуре и форме документов по организации ГИА сформулированы в утвержденной в УрФУ документированной процедуре «Порядок проведения государственной итоговой аттестации по образовательным программам высшего образования - программам бакалавриата, программам специалитета и программам магистратуры» (СМК-ПВД-6.1-01-65-2015), введенной в действие приказом ректора от 01.12.2015 №899/03.',
                    style: { 'max-width': '500px','border-style':'inset' },
                }]

            },
            {
                name: 'ProcedureRequirement',
                items: [
                    {
                        xtype: 'container',
                        layout: {
                            type: 'hbox',
                            align: 'center'
                        },
                        //margin: "8 0 0 0",
                        items: [
                            {
                                width: 500,
                                //labelAlign: 'right',
                                xtype: 'textarea',
                                bind: {
                                    value: '{ProcedureRequirement}'
                                },
                            }]
                    }]
            },
            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'label',
                    text: '1.6.	Требования к оцениванию результатов освоения ОП в рамках государственной итоговой аттестации',
                    style: captionStyle
                }]

            },
            {
                name: 'EvalutionReuqirementProtocol',
                items: [
                    {
                        xtype: 'container',
                        layout: {
                            type: 'hbox',
                            align: 'center'
                        },
                        margin: "8 0 0 0",
                        items: [{
                            fieldLabel: 'Протокол №',
                            xtype: 'textfield',
                            bind: '{EvalutionReuqirementProtocol.ProtocolNumber}',
                            width: 210
                        }, {
                            labelAlign: 'right',
                            xtype: 'datefield',
                            fieldLabel: 'от',
                            bind: {
                                value: '{EvalutionReuqirementProtocol.ProtocolDate}',
                            },
                            width: 270,
                            submitFormat: 'd.m.Y',
                            //rawFormat: 'd.m.Y',
                            format: 'd.m.Y'
                        }, {
                            xtype: 'label',
                            margin: '0 0 0 8',
                            text: 'г.'
                        },


                        ]
                    }
                ]

            }
        

        ]
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

        panel.removeAll();
        panel.add(items);
    

        console.timeEnd('buildTableItemsUpdateUI');
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