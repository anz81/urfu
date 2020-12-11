function uiInit(documentId, documentType, data, schema, emptyData) {

    var panelId = 'eduResultsId';
    var notificationId = 'notificationId';
    var emptyCompetences = false;
    var allEduResultsExist = true;
    
    function updateNotification() {
        var vm = Ext.getCmp(panelId).getViewModel().get('EduResults');

        var allCompetences = vm.UniversalCompetences.concat(vm.GeneralCompetences);
        emptyCompetences = false;
        allEduResultsExist = true;
        eduResultsStore.data.items.forEach(function (competence) {

            var competenceData = allCompetences.find(c => c.Competence.Id == competence.data.CompetenceId);

            var eduResultsInCompetenceData = competenceData.Modules.flatMap(m => m.Disciplines).flatMap(d => d.EduResults);

            competence.data.EduResults.forEach(function (eduResult) {
                if (!eduResultsInCompetenceData.some(e => e.Id == eduResult.Id))
                    allEduResultsExist = false;
            });
            if (competenceData.Modules.length == 0)
                emptyCompetences = true;
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
                    items.push({}, {}, {}, {});
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

        var items = [{
            html: 'Компетенция',
            width: 300
        }, {
            html: 'Модуль',
            width: 200
        }, {
            html: 'Дисциплина',
            width: 200
            },
            {
            html: '',
            width: 20
            },
            {
            html: 'Результаты обучения',
            width: 200
        }, {
            html: '',
            width: 20
            }];
        
        var plannedResults = vm.get('EduResults');

        plannedResults.UniversalCompetences.forEach(function (competence) {
            items = fillCompetenceData(competence, items, panel);
        });
        
        plannedResults.GeneralCompetences.forEach(function (competence) {
            items = fillCompetenceData(competence, items, panel);
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
    var ids = data.EduResults.UniversalCompetences.map(c => c.Competence.Id)
        .concat(data.EduResults.GeneralCompetences.map(c => c.Competence.Id));
    
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

    return {
        viewModel: {
            stores: {
            }
        },
        items: [
            {
                xtype: 'label',
                html: `Институт/подразделение: <b>${data.Institute.Name}</b>`
            },
            {
                xtype: 'label',
                html: `Код направления и уровня подготовки: <b>${data.Direction.Code}</b>`
            },
            {
                xtype: 'label',
                html: `Направление подготовки: <b>${data.Direction.Title}</b>`
            },
            {
                xtype: 'label',
                html: `Код ОП/Образовательная программа: <b>${data.Profile.Code} ${data.Profile.Name}</b>`
            },
            {
                xtype: 'label',
                margin: '10 0 0 0',
                html: `<b>Пояснительная записка</b>`
            },
            {
                xtype: 'label',
                html: `&emsp;&emsp;Изучение дисциплин модуля предусматривает формирование компетенций посредством последовательного освоения результатов обучения на определенном уровне сложности содержания.<br>
                       &emsp;&emsp;Паспорт компетенций представляет собой таблицу, в которой содержание каждой компетенции, реализуемой ОП, раскрывается через результаты обучения(индикаторы) и увязывается с дисциплинами модулей, которые их формируют. `
            },
            {
                xtype: 'label',
                html: `&emsp;&emsp;<b>Результаты обучения (индикаторы) по дисциплине (далее – РО)</b> 
                            – это конкретные знания, умения, опыт и другие результаты (содержательные компоненты компетенций), 
                            которых планируется достичь на этапе изучения дисциплины модуля и которые должны будут продемонстрированы обучающимися и оценены преподавателем по индикаторам/измеряемым критериям.`
            },
            {
                xtype: 'label',
                margin: '10 0 0 0',
                html: `<b>Правила формулировки РО:</b><br>
                    &emsp;&emsp;Под <b>знанием как составляющем РО</b> в данном документе понимается совокупность сведений в определенной предметно-научной или предметно-профессиональной области, которые позволяют решить поставленную в умении интеллектуальную задачу и формируют понимание, каким способом можно и нужно решать эту задачу.<br>
                    &emsp;&emsp;Рекомендуется формулировать знания предельно конкретными (знать /понимать теоретические положения…, законы…, методы…, подходы…, классификацию… и т.п.), в необходимом и достаточном объеме для освоения компетенции (умений). Не рекомендуется формулировать знания в дисциплинарном формате – теоретические основы…; неконкретно – знать инструкции, документацию…, металлы…, оборудование… и т.п.<br>
	                &emsp;&emsp;<b>Умения как составляющие РО</b> формулируются глаголами в активной форме или отглагольным существительным, должны содержать индикатор/измеряемый критерий (например, самостоятельно формулировать предложения…; рассчитывать необходимое количество материалов…/ расчет необходимого количества материалов… и т.д.). Рекомендуется использовать таксономию Блума.<br>
	                &emsp;&emsp;<b>Опыт как составляющая РО</b> в данном документе понимается как степень овладения каким-либо знанием или умением, степень самостоятельности совершить какое-то действие, заложенное в компетенции. Опыт осваивается на практических или лабораторных занятиях, на практике и может формироваться на уровне навыка или первичного опыта.<br>
                    &emsp;&emsp;Формулировка РО должна содержать индикатор. Индикатор – это признак/сигнал/маркер, который показывает, на каком уровне обучающийся должен освоить результаты обучения и их предъявление должно подтвердить факт освоения предметного содержания данной дисциплины.<br>
                    &emsp;&emsp;Индикаторы, заложенные в РО, должны учитываться при выборе и составлении ФОС, заданий контрольно-оценочных мероприятий (оценочных средств) текущей и промежуточной аттестации.`
            },
            {
                name: 'EduResults',
                id: panelId,
                viewModel: {
                    stores: {
                        disciplines: {
                            data: []
                        },

                        competences: {
                            data: data.EduResults.UniversalCompetences.map(c => c.Competence)
                                .concat(data.EduResults.GeneralCompetences.map(c => c.Competence))
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
                                        this.up().items.items.find(i => i.name == 'eduResults').setValue('');
                                        this.up().items.items.find(i => i.name == 'disciplines').setValue('');
                                        this.up().items.items.find(i => i.name == 'modules').setValue('');

                                        try {
                                            this.lookupViewModel().getStore('eduResults').setData(
                                                eduResultsStore.data.items
                                                    .find(d => d.data.CompetenceId == newValue).data.EduResults);
                                        }
                                        catch{}
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

                                    var competenceCmbx = this.up().items.items.find(i => i.name == 'competence');
                                    var moduleCmbx = this.up().items.items.find(i => i.name == 'modules');
                                    var disciplineCmbx = this.up().items.items.find(i => i.name == 'disciplines');
                                    var eduResultCmbx = this.up().items.items.find(i => i.name == 'eduResults');

                                    if (competenceCmbx.getValue() == '' || moduleCmbx.getValue() == '' || disciplineCmbx.getValue() == '' || eduResultCmbx.getValue().length == 0) {
                                        Ext.MessageBox.alert('Ошибка', "Заполните все поля");
                                        return;
                                    }

                                    var competence = competenceCmbx.getStore().getData().items
                                        .find(c => c.data.Id == competenceCmbx.getValue()).data;

                                    var dataEduResults = null;
                                    if (competence.Type == 'УК') {
                                        dataEduResults = rootVm.get('EduResults').UniversalCompetences
                                            .find(c => c.Competence.Id == competenceCmbx.getValue());
                                    }
                                    if (competence.Type == 'ОПК') {
                                        dataEduResults = rootVm.get('EduResults').GeneralCompetences
                                            .find(c => c.Competence.Id == competenceCmbx.getValue());
                                    }

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
                items: [{
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