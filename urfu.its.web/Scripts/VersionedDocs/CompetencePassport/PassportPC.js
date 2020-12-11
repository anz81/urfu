function uiInit(documentId, documentType, data, schema, emptyData) {

    var panelId = 'eduResultsId';
    var notificationId = 'notificationId';
    var emptyCompetences = false;
    var allEduResultsExist = true;

    function updateNotification() {
        var vm = Ext.getCmp(panelId).getViewModel().get('EduResults');

        var allCompetences = vm.VariantProfCompetences.flatMap(v => v.ProfCompetences);
        emptyCompetences = allCompetences.some(f => f.Modules.length == 0 || f.Modules.flatMap(m => m.Disciplines).some(d => d.EduResults.length == 0));
        allEduResultsExist = true;
        eduResultsStore.data.items.forEach(function (competence) {

            var competenceData = allCompetences.find(c => c.Competence.Id == competence.data.CompetenceId);

            var eduResultsInCompetenceData = competenceData.Modules.flatMap(m => m.Disciplines).flatMap(d => d.EduResults);

            competence.data.EduResults.forEach(function (eduResult) {
                if (!eduResultsInCompetenceData.some(e => e.Id == eduResult.Id))
                    allEduResultsExist = false;
            });
        });

        var message = '';
        if (emptyCompetences)
            message += 'Не для всех компетенций указаны дисциплины. ';
        if (!allEduResultsExist)
            message += 'Не все результаты обучения привязаны к дисциплинам.';

        Ext.getCmp(notificationId).setHtml(`<font color="red"><b>${message}</b></font>`)
        Ext.getCmp(notificationId).setHidden(!emptyCompetences && allEduResultsExist);
    }


    function fillCompetenceData(competenceData, items, panel) {

        var eduResultsCount = competenceData.Modules.length == 0 ? 0
            : competenceData.Modules.map(m => m.Disciplines).reduce((s, a) => s.concat(a)).map(d => d.EduResults).reduce((s, a) => s.concat(a)).length;

        var сompetenceItem = {
            html: `${competenceData.Competence.Code} - ${competenceData.Competence.Content}`,
            dataItem: competenceData.Competence,
            rowspan: eduResultsCount
        };

        items.push(сompetenceItem);

        competenceData.Modules = competenceData.Modules.filter(m => m.Disciplines.length != 0 && m.Disciplines.some(d => d.EduResults.length > 0));
        if (!competenceData.Modules.length) {
            items.push({}, {}, {}, {}, {});
        } else {
            competenceData.Modules.forEach(function (module, index, modules) {

                var moduleEduResultsCount = module.Disciplines.length == 0 ? 0
                    : module.Disciplines.map(d => d.EduResults).reduce((s, a) => s.concat(a)).length;
                
                var moduleItem = {
                    html: module.FullName,
                    dataItem: module,
                    rowspan: moduleEduResultsCount
                };
                items.push(moduleItem);

                module.Disciplines = module.Disciplines.filter(m => m.EduResults.length != 0);
                if (!module.Disciplines.length) {
                    items.push({}, {}, {});
                }
                else {
                    module.Disciplines.forEach(function (discipline) {
                        var disciplineItem = {
                            html: discipline.Name,
                            dataItem: discipline,
                            rowspan: discipline.EduResults.length
                        };
                        var disciplineRemoveItem = {
                            xtype: 'image',
                            src: '/Content/Images/remove.png',
                            alt: 'Удалить',
                            margin: 0,
                            width: 16,
                            dataItem: discipline,
                            rowspan: discipline.EduResults.length,
                            listeners: {
                                el: {
                                    click: function () {
                                        var vm = findRootVm(this.component);
                                        if (!vm.confirmMessage('Удалить дисциплину и ее связи с РО?'))
                                            return;
                                        Ext.Array.remove(module.Disciplines, this.component.dataItem);
                                        buildTableItems(panel, vm);
                                    }
                                }
                            }
                        };
                        items.push(disciplineItem, disciplineRemoveItem);

                        if (!discipline.EduResults.length) {
                            items.push({}, {});
                        }
                        else {
                            discipline.EduResults = discipline.EduResults.sort(function (a, b) {
                                if (a.SerialNumber > b.SerialNumber) {
                                    return 1;
                                }
                                if (a.SerialNumber < b.SerialNumber) {
                                    return -1;
                                }
                                return 0;
                            }).sort(function (a, b) {
                                if (a.KindId > b.KindId) {
                                    return 1;
                                }
                                if (a.KindId < b.KindId) {
                                    return -1;
                                }
                                return 0;
                            }).sort(function (a, b) {
                                if (a.TypeId > b.TypeId) {
                                    return 1;
                                }
                                if (a.TypeId < b.TypeId) {
                                    return -1;
                                }
                                return 0;
                            });
                            discipline.EduResults.forEach(function (eduResult) {
                                var eduResultItem = {
                                    html: `${eduResult.Code} - ${eduResult.Description}`,
                                    dataItem: eduResult
                                };
                                var eduResultRemoveItem = {
                                    xtype: 'image',
                                    src: '/Content/Images/remove.png',
                                    alt: 'Удалить',
                                    margin: 0,
                                    width: 16,
                                    dataItem: eduResult,
                                    listeners: {
                                        el: {
                                            click: function () {
                                                var vm = findRootVm(this.component);
                                                if (!vm.confirmMessage('Удалить РО?'))
                                                    return;
                                                Ext.Array.remove(discipline.EduResults, this.component.dataItem);
                                                buildTableItems(panel, vm);
                                            }
                                        }
                                    }
                                };

                                items.push(eduResultItem, eduResultRemoveItem);
                            });
                        }
                    });
                }
            });
        }

        return items;
    };

    function buildTableItems(panel, vm) {
        
        var items = [];
        var plannedResults = vm.get('EduResults');
        plannedResults.VariantProfCompetences.forEach(function (variant) {
            items.push({
                html: `<b>${variant.Name}</b>`,
                dataItem: variant,
                colspan: 6
            });
            items.push({
                html: 'Компетенция',
                width: 300
            });
            
            items.push({
                html: 'Модуль',
                width: 200
            });
            items.push({
                html: 'Дисциплина',
                width: 200
            });
            items.push({
                html: '',
                width: 20
            });

            items.push({
                html: 'Результаты обучения',
                width: 200
            });
            items.push({
                html: '',
                width: 20
            });
            variant.ProfCompetences.forEach(function (competence) {
                items = fillCompetenceData(competence, items, panel);
            })
            
        });

        panel.removeAll();
        panel.add(items);
        
        Ext.ComponentQuery.query('block[name=EduResults]')[0].detectChanges();
        updateNotification();
    };

    var moduleStore = Ext.create("Ext.data.Store",
        {
            autoLoad: true,
            proxy: {
                type: 'ajax',
                url: '/CompetencePassport/DirectionModules',
                reader: { type: 'json' },
                extraParams: {
                    id: data.Direction.Id
                }
            }
        }
    );

    var variantStore = Ext.create("Ext.data.Store",
        {
            data: data.EduResults.VariantProfCompetences
        }
    );

    var isProfile = data.EduResults.VariantProfCompetences.some(c => c.IdSource == 0);

    var ids = data.EduResults.VariantProfCompetences.flatMap(c => c.ProfCompetences).map(c => c.Competence.Id);

    var eduResultsStore = Ext.create("Ext.data.Store",
        {
            autoLoad: true,
            proxy:
            {
                type: 'ajax',
                url: '/CompetencePassport/AllEduResults',
                reader: { type: 'json' },
                extraParams: {
                    ids: JSON.stringify(ids),
                }
            },
            listeners: {
                load: function () {
                    updateNotification();
                }
            }
        }
    );
    data.EduResults.VariantProfCompetences = data.EduResults.VariantProfCompetences.map(function (i) { delete i.id; return i; });
    return {
        viewModel: {
            stores: {
            }
        },
        items: [
            {
                name: 'EduResults',
                id: panelId,
                contentReader: function (content, vm) {
                    content.VariantProfCompetences = content.VariantProfCompetences.map(function (i) { delete i.id; return i; });
                    return content;
                },
                viewModel: {
                    stores: {
                        disciplines: {
                            data: []
                        },
                        
                        competences: {
                            data: []
                        },

                        eduResults: {
                            data: []
                        }
                    }
                },
                tbar: [{
                    xtype: 'panel',
                    border: true,
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
                        items: [
                            {
                                xtype: 'comboedit',
                                labelWidth: 230,
                                fieldLabel: isProfile ? 'Образовательая программа' : 'Траектория',
                                displayField: 'Name',
                                valueField: 'Id',
                                name: 'variant',
                                width: 600,
                                store: variantStore,
                                listeners: {
                                    change: function (t, newValue, oldValue) {
                                        
                                        this.up().items.items.find(i => i.name == 'eduResults').setValue('');
                                        this.up().items.items.find(i => i.name == 'disciplines').setValue('');
                                        this.up().items.items.find(i => i.name == 'modules').setValue('');
                                        this.up().items.items.find(i => i.name == 'competence').setValue('');

                                        try {
                                            var variantData = variantStore.data.items.find(s => s.data.Id == newValue).data;

                                            this.lookupViewModel().getStore('competences').setData(variantData.ProfCompetences.map(c => c.Competence));
                                        }
                                        catch{ }
                                    },
                                    afterrender: function (t) {
                                        if (isProfile)
                                            t.setValue(variantStore.data.items[0].data.Id);
                                    }
                                }
                            },
                            {
                                xtype: 'comboedit',
                                labelWidth: 230,
                                fieldLabel: 'Компетенция',
                                displayField: 'Code',
                                valueField: 'Id',
                                name: 'competence',
                                width: 600,
                                bind: {
                                    store: '{competences}'
                                },
                                tpl: Ext.create('Ext.XTemplate',
                                    '<tpl for=".">',
                                    '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                                    '<div><b>{Code}</b> - {Content}</div>',
                                    '</div>',
                                    '</tpl>'
                                ),
                                listeners: {
                                    change: function (t, newValue, oldValue) {
                                        if (newValue != '') {
                                            this.up().items.items.find(i => i.name == 'eduResults').setValue('');
                                            this.up().items.items.find(i => i.name == 'disciplines').setValue('');
                                            this.up().items.items.find(i => i.name == 'modules').setValue('');
                                            
                                            try {
                                                this.lookupViewModel().getStore('eduResults').setData(
                                                    eduResultsStore.data.items
                                                        .find(d => d.data.CompetenceId == newValue).data.EduResults);
                                            }
                                            catch{ }
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'comboedit',
                                labelWidth: 230,
                                fieldLabel: 'Модуль',
                                displayField: 'FullName',
                                valueField: 'Id',
                                name: 'modules',
                                width: 600,
                                store: moduleStore,
                                listeners: {
                                    change: function (t, newValue, oldValue) {
                                        if (newValue != '') {
                                            this.up().items.items.find(i => i.name == 'disciplines').setValue('');
                                            this.up().items.items.find(i => i.name == 'eduResults').setValue('');

                                            try {
                                                var moduleData = moduleStore.data.items.find(s => s.data.Id == newValue).data;

                                                this.lookupViewModel().getStore('disciplines').setData(moduleData.Disciplines);
                                            }
                                            catch{ }
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'comboedit',
                                fieldLabel: 'Дисциплины',
                                bind: {
                                    store: '{disciplines}'
                                },
                                name: 'disciplines',
                                valueField: 'Id',
                                displayField: 'Name',
                                queryMode: 'local',
                                listeners: {
                                    change: function (t, newValue, oldValue) {
                                        if (newValue != '') {
                                            this.up().items.items.find(i => i.name == 'eduResults').setValue('');
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'tagfield',
                                fieldLabel: 'Результаты обучения',
                                bind: {
                                    store: '{eduResults}'
                                },
                                name: 'eduResults',
                                valueField: 'Id',
                                displayField: 'Code',
                                queryMode: 'local',
                                filterPickList: false,
                                tpl: Ext.create('Ext.XTemplate',
                                    '<tpl for=".">',
                                    '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                                    '<div><b>{Code}</b> - {Description}</div>',
                                    '</div>',
                                    '</tpl>'
                                )
                            },
                            {
                                xtype: 'button',
                                text: 'Добавить',
                                handler: function () {
                                    var rootVm = findRootVm(this);

                                    var variantCmbx = this.up().items.items.find(i => i.name == 'variant');
                                    var competenceCmbx = this.up().items.items.find(i => i.name == 'competence');
                                    var moduleCmbx = this.up().items.items.find(i => i.name == 'modules');
                                    var disciplineCmbx = this.up().items.items.find(i => i.name == 'disciplines');
                                    var eduResultCmbx = this.up().items.items.find(i => i.name == 'eduResults');

                                    if (variantCmbx.getValue() == '' || competenceCmbx.getValue() == '' || moduleCmbx.getValue() == '' || disciplineCmbx.getValue() == '' || eduResultCmbx.getValue().length == 0) {
                                        Ext.MessageBox.alert('Ошибка', "Заполните все поля");
                                        return;
                                    }

                                    var variant = variantCmbx.getStore().getData().items
                                        .find(c => c.data.Id == variantCmbx.getValue()).data;

                                    var competence = competenceCmbx.getStore().getData().items
                                        .find(c => c.data.Id == competenceCmbx.getValue()).data;

                                    var dataEduResults = rootVm.get('EduResults').VariantProfCompetences.find(v => v.Id == variantCmbx.getValue())
                                        .ProfCompetences.find(c => c.Competence.Id == competenceCmbx.getValue());
                                   
                                    if (!dataEduResults.Modules.some(m => m.Id == moduleCmbx.getValue()))
                                        dataEduResults.Modules.push(moduleCmbx.getStore().getData().items.find(m => m.data.Id == moduleCmbx.getValue()).data);

                                    var dataModule = dataEduResults.Modules.find(m => m.Id == moduleCmbx.getValue());
                                    dataModule.Disciplines = dataModule.Disciplines.filter(d => d.EduResults.length > 0 || d.Id == disciplineCmbx.getValue()); // сохраняем те дисциплины, на которые были ранее добавлены РО или ту, на которую добавлено сейчас.
                                    if (!dataModule.Disciplines.some(d => d.Id == disciplineCmbx.getValue()))
                                        dataModule.Disciplines.push(disciplineCmbx.getStore().getData().items.find(d => d.data.Id == disciplineCmbx.getValue()).data);

                                    var dataEduResults = dataModule.Disciplines.find(d => d.Id == disciplineCmbx.getValue()).EduResults;
                                            
                                    eduResultCmbx.getValue().forEach(function (item) {
                                        if (!dataEduResults.some(r => r.Id == item))
                                            dataEduResults.push(eduResultCmbx.getStore().getData().items.find(i => i.data.Id == item).data);
                                    });
                                    
                                    buildTableItems(Ext.ComponentQuery.query('#eduResultsTable')[0], rootVm);
                                }
                            }
                        ]
                    }]
                }],

                items:[
                {
                    xtype: 'container',
                    itemId: 'eduResultsTable',
                    cls: 'table-all-borders table-cell-padding-normal',
                    layout: { type: 'table', columns: 6 },
                    listeners: {
                        afterrender: function () {
                            buildTableItems(Ext.ComponentQuery.query('#eduResultsTable')[0], findRootVm(this));
                        }
                    }
                },
                {
                    xtype: 'label',
                    id: notificationId,
                    hidden: true,
                    style: {
                        'text-align': 'right'
                    },
                    html: ''
                }]
            }
        ]
    };
}