function uiInit(documentId, documentType, data, schema, emptyData) {
    Ext.tip.QuickTipManager.init();
    var captionStyle = { fontWeight: 'bold' };
    var textAlignRight = { 'text-align': 'right' };
    var italicStyle = { fontStyle: 'italic' };
    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });

    function prepareFdpPracticeStructures(fdpForms) {
        return fdpForms.map(function (link, i) {
        
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
                    columns: [{
                        text: '№',
                        xtype: 'rownumberer',
                        width: 50
                    }, {
                        text: 'Вид практики ',
                        dataIndex: 'Title',
                        width: 600,
                        renderer: Urfu.renders.htmlEncodeWithToolTip

                    },{
                        text: 'Номер учебного семестра',
                        dataIndex: 'Semesters',
                        width : 100
                    }, {
                        text: 'Объем практики',
                        columns: [{
                                text: 'в неделях',
                                dataIndex: 'AdditionalWeeks'
                            }, {
                                text: 'в з.е.',
                                dataIndex: 'TotalUnits'
                            }]
                        }]
                }]
            }
        });
    }

    function preparePracticeWays(fdpForms) {
        //return fdpForms.map(function (link, i) {
            return {
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
                    xtype: 'grid',
                    //itemId: 'ways' + link.DirectionId,
                    viewConfig: {
                        markDirty: false
                    },
                    cls: 'grid-header-minimal',
                    store: {
                        data: fdpForms //link
                    },
                    plugins: [cellEditing],
                    columns: [{
                        text: '№',
                        xtype: 'rownumberer',
                        width: 50
                    },
                        {
                            text: 'Вид практики ',
                            dataIndex: 'DisciplineTitle',
                            width: 600,
                            renderer: Urfu.renders.htmlEncodeWithToolTip

                        },
                        {
                            dataIndex: 'PracticeMethod',
                            text: 'Форма проведения',
                            editor: {
                                xtype: 'combobox',
                                displayField: 'Name',
                                valueField: 'Name',
                                forceSelection: true,
                                queryMode: 'local',
                                allowBlank: false,
                                store: {
                                    autoLoad: true,
                                    proxy: {
                                        type: 'ajax',
                                        autoload: true,
                                        url: '/PracticesWorkingProgram/GetPracticeMethods/',
                                        reader: { type: 'json' },
                                        fields: ['Id', 'Name']
                                    }
                                }
                            },

                            width: 200
                        },
                        {
                            dataIndex: 'PracticeWay',
                            text: 'Способ проведения',
                            editor: {
                                xtype: 'combobox',
                                displayField: 'Name',
                                valueField: 'Name',
                                forceSelection: true,
                                queryMode: 'local',
                                allowBlank: false,
                                store: {
                                                autoLoad: true,
                                                proxy: {
                                                    type: 'ajax',
                                                    autoload: true,
                                                    url: '/PracticesWorkingProgram/GetPracticeWays/',
                                                    reader: { type: 'json' },
                                                    fields:['Id','Name']
                                                }
                                }
                            },

                            width: 200
                        }]
                    }]
            }
       // });
    }

    function preparePracticeResultSubItems(practiceForms) {
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
                        xtype: 'textareafield',
                        itemId: 'do' + link.DisciplineUid,
                        height: 100,
                        width: 700,
                        anchor: true,
                        value: link.MustDo,
                        fieldLabel: 'Уметь'
                    },
                    {
                        xtype: 'textareafield',
                        itemId: 'show' + link.DisciplineUid,
                        height: 100,
                        width: 700,
                        anchor: true,
                        value: link.MustShow,
                        fieldLabel: 'Демонстрировать навыки и опыт деятельности'
                    }
                ]
            }
        });
    }

    function preparePracticeResultItems(practiceForms) {
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
                        text: 'Направление: ' + link.DirectionCode, 
                        style: captionStyle
                    },
                    {
                        items: preparePracticeResultSubItems(Ext.clone(link.Results))
                    }
                ]
            }
        });
    }
    var getPractices = function () {
        var _practices = [];
        data.PracticeStructures.forEach(function (item, index, arr) {
            item.Items.forEach(function (item2, index2, arr2) {
                _practices.push(item2);
            });
        })
        return _practices;
    }

	return {
        viewModel: {
            data: data,
            stores: {
                profiles: {
                    data: data.Profiles
                },
                allUsedCompetences: {
                    data: []
                }
            }
        },
        listeners: {
            afterrender: function () {
                var vm = this.lookupViewModel();
            }
        },

        items: [
            {
                title: 'Аннотация',
                name: 'Annotation',
                items: {
                    xtype: 'textarea',
                    height: 150,
                    name: 'Annotation',
                    bind: '{Annotation}'
                }
            },
            {
                xtype: 'label',
                text: '1.2.Структура практик, их сроки и продолжительность',
                style: captionStyle
            },
            {
                xtype: 'container',
                name: 'PracticeStructures',
                items: prepareFdpPracticeStructures(Ext.clone(data.PracticeStructures))
            },
            {
                contentReader: function (content, vm) {
                    var res = this.query('grid')[0].getStore().getData().items.map(function (i) { return i.getData(); });
                    return res;
                },
                name: 'PracticeWays',
                items: [
                    {
                        xtype: 'label',
                        text: '1.3. Базы практик, форма проведения практик ',
                        style: captionStyle
                    }, {
                        xtype: 'label',
                        text: '[предполагаемые места проведения практик, объекты, организации и т.д. в соответствии с заключенными договорами]',
                        style: italicStyle
                    },
                    {
                        xtype: 'container',
                        contentReader: function (content, vm) {
                            return this.query('grid').map(function (g) {
                                return {
                                    FdpId: g.fdpId,
                                    Items: g.getStore().getData().items.map(function (i) { return i.getData(); })
                                };
                            });
                        },
                        layout: { type: 'vbox', align: 'stretch' },
                        items: preparePracticeWays(Ext.clone(data.PracticeWays))
                    },
                    
                ],
                //hasChanges: function (savedValue, currentValue) {
                //    return currentValue.some(function (v, i) {
                //        var s = savedValue[i];
                //        if (v.Items.length !== s.Items.length)
                //            return true;
                //        return v.Items.some(function (vi, i) {
                //            var si = s.Items[i];
                //            return vi.PracticeWay != si.PracticeWay || vi.PracticeMethod != si.PracticeMethod  ;
                //        });
                //    });
                //}
            },
            {
                xtype: 'label',
                text: '1.4. Процедура организации практик',
                style: captionStyle
            },
            {
                xtype: 'label',
                text: 'Порядок планирования, организации и проведения практик, структура и форма документов по организации практик и их аттестации сформулированы в утвержденном в УрФУ приказом ректора от 27.09.2012 г. №698/03, в "Положении о порядке организации и проведения практик" (СМК-ПВД-7.5.3-01-11-2012).',
            },
            {
                xtype: 'label',
                text: '1.5. Планируемые  результаты прохождения практик',
                style: captionStyle
            }, {
                xtype: 'label',
                text:
                    '[таблицы формируются отдельно по каждой форме обучения]',
                style: italicStyle
            },
            {
                name: 'PlannedResultPracticeInfos',
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
                        practices: {
                            data: getPractices()
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
                tbar: [
                    {
                        xtype: 'panel',
                        border: true,
                        bodyPadding: 6,
                        width: 720,
                        layout: { type: 'vbox', align: 'stretch' },
                        tbar: [
                            {
                                xtype: 'container',
                                layout: { type: 'vbox', align: 'stretch' },
                                defaults: {
                                    labelWidth: 230,
                                    margin: '0 0 10 0',
                                    width: 700,
                                    grow: true
                                },
                                items: [
                                    {
                                        xtype: 'container',
                                        layout: { type: 'hbox' },
                                        items: [
                                            {
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
                                                        var oldPressed = this.lookupViewModel()
                                                            .get(sender.bind.pressed.stub.name);
                                                        if (!oldPressed ||
                                                            (clearForm =
                                                                vm.confirmMessage(
                                                                    'Данные формы будут сброшены. Продолжить?'))) {
                                                            this.lookupViewModel().set(sender.bind.pressed.stub.name,
                                                                pressed);
                                                            if (clearForm) {
                                                                var localVm = sender.lookupViewModel();
                                                                localVm.set('selectedEduResults', []);
                                                                localVm.set('selectedUniversalCompetences', []);
                                                                localVm.get('eduResultsCompetences').removeAll();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        ]
                                    },
                                    {
                                        xtype: 'comboedit',
                                        labelWidth: 230,
                                        fieldLabel: 'Практика',
                                        displayField: 'Title',
                                        valueField: 'DisciplineId',
                                        width: 600,
                                        bind: {
                                            value: '{disciplineId}',
                                            store: '{practices}', 
                                        }
                                    },
                                    {
                                        xtype: 'tagfield',
                                        fieldLabel: 'Результаты обучения',
                                        bind: {
                                            store: '{eduResults}',
                                            disabled: '{!isProfileLocked}',
                                            value: '{selectedEduResults}'
                                        },
                                        valueField: 'Id',
                                        displayField: 'Name',
                                        tpl: Ext.create('Ext.XTemplate',
                                            '<tpl for=".">',
                                            '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                                            '<div><b>{Name}</b> - {Description}</div>',
                                            '</div>',
                                            '</tpl>'
                                        ),
                                        queryMode: 'local',
                                        filterPickList: false,
                                        listeners: {

                                            change: function (field, values, oldValues) {
                                                console.time('change');
                                                var addedValues = values.filter(function (v) {
                                                    return oldValues.indexOf(v) < 0;
                                                });
                                                var removedValues = oldValues.filter(function (v) {
                                                    return values.indexOf(v) < 0;
                                                });

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
                                    }
                                ]
                            }
                        ],
                        items: [
                            {
                                xtype: 'grid',
                                stripeRows: false,
                                plugins: [
                                    Ext.create('Ext.grid.plugin.CellEditing',
                                        {
                                            clicksToEdit: 1,
                                            listeners: {
                                                beforeEdit: function (editor, context, eOpts) {
                                                    var height = context.row.clientHeight;
                                                    var editor = context.column.getEditor();
                                                    editor.setHeight(height);
                                                }
                                            }
                                        })
                                ],
                                viewConfig: {
                                    markDirty: false,
                                },
                                enableLocking: false, // turn off column lock context items
                                enableColumnMove: false, // turn off column reorder drag drop
                                enableColumnHide: false, // turn off column reorder drag drop
                                sortableColumns: false,
                                columns: [
                                    {
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
                                    }
                                ],
                                bind: {
                                    store: '{eduResultsCompetences}'
                                }
                            }
                        ],
                        bbar: [
                            {
                                xtype: 'button',
                                text: 'Добавить',
                                bind: {
                                    disabled: '{!isProfileLocked}'
                                },
                                handler: function () {
                                    var vm = this.lookupViewModel();
                                    var rootVm = findRootVm(this);
                                    var practiceId = vm.get('disciplineId'); //vm.get('practiceId');
                                    var practiceStore = vm.get('practices');

                                    var resultCompetencesStore = vm.get('eduResultsCompetences');
                                    var competences = vm.get('competences');
                                    var profilesStore = vm.get('profiles');
                                    var plannedResults = rootVm.get('PlannedResultPracticeInfos');
                                    var profileId = vm.get('profileId');

                                    var plannedResult = {
                                        ProfileId: profileId,
                                        ProfileCode: profilesStore.findRecord('Id', profileId).get('Code'),
                                        DisciplineId: practiceId,
                                        DisciplineName: practiceStore.findRecord('DisciplineId', practiceId).get('Title'),
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
                                    vm.get('eduResultsCompetences').removeAll();
                                    vm.set('disciplineId', null);
                                    vm.set('profileId', null);
                                    vm.set('isProfileLocked', false);

                                    buildTableItems(Ext.ComponentQuery.query('#plannedResultsTable')[0], rootVm);
                                }
                            }
                        ]
                    }
                ],
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
                xtype: 'label',
                margin: '15 0 0 0',
                text: 'Результаты обучения',
                style: captionStyle
            },
            {
                name: 'PracticeResults',
                contentReader: function (content, vm) {
                    var clone = Ext.clone(content);
                    var t = this;
                    clone.forEach(function (item, index, array) {
                        clone[index].Results.forEach(function (item2, index2, array2) {
                            clone[index].Results[index2].MustDo = t.down('#do' + clone[index].Results[index2].DisciplineUid).getValue();
                            clone[index].Results[index2].MustShow = t.down('#show' + clone[index].Results[index2].DisciplineUid).getValue();
                        });
                    });
                    return clone;
                },
                items: preparePracticeResultItems(Ext.clone(data.PracticeResults))
            }
        ]
    }

    function buildTableItems(panel, vm) {
        console.time('buildTableItems');
        var items = [{
            html: 'Коды ОП, для которых реализуется модуль',
            width: 200
        }, {
            html: 'Практика',
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

        var plannedResults = vm.get('PlannedResultPracticeInfos');
        if (plannedResults) {
            plannedResults.forEach(function (prac) {
                //prac.PracticeEduResultItemInfos.forEach(function (r) {
                    var profileIdItem = {
                        html: prac.ProfileCode,
                        dataItem: prac,
                        rowspan: prac.Results.length
                    };
                    var practice = {
                        html: prac.DisciplineName,
                        dataitem: prac,
                        rowspan: prac.Results.length 
                    }

                    var practiceRemoveItem = {
                        xtype: 'image',
                        src: '/Content/Images/remove.png',
                        alt: 'Удалить',
                        margin: 0,
                        width: 16,
                        dataItem: prac,
                        rowspan: prac.Results.length,
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
                    items.push(profileIdItem, practice, practiceRemoveItem);

                if (!prac.Results.length) {
                        items.push({}, {}, {});
                    } else {
                    prac.Results.forEach(function (rc) {
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
                                            Ext.Array.remove(prac.Results, this.component.dataItem);
                                            buildTableItems(panel, vm);
                                        }
                                    }
                                }
                            };
                            items.push(roItem, competencesItem, rcRemoveItem);
                        });
                    }
             //   });

            });
        }
        console.timeEnd('buildTableItems');

        console.time('buildTableItemsUpdateUI');

        panel.removeAll();
        panel.add(items);


        console.timeEnd('buildTableItemsUpdateUI');
        Ext.ComponentQuery.query('block[name=PlannedResultPracticeInfos]')[0].detectChanges();
    }
    
}