function uiInit(documentId, documentType, data, schema, emptyData, user, additionalData) {
    var boldStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };
    
    return {
        viewModel: {
            stores: {
                semesters: {
                    data: [['осенний'], ['весенний'], ['прочий']],
                    fields: ['Name']
                },
                groups: {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: '/DisciplineWorkingProgram/GetGroups/' + window.location.search,
                        reader: { type: 'json' },
                        extraParams: {
                            documentType: documentType,
                            documentId: documentId
                        }
                    }
                },
                techCardLections: {
                    data: data.TechCardDisciplineCertification.Lections.Controls
                },
                techCardPractices: {
                    data: data.TechCardDisciplineCertification.Practices.Controls
                },
                techCardLabs: {
                    data: data.TechCardDisciplineCertification.Labs.Controls
                },
                techCardCourseWorks: {
                    data: data.TechCardCourseWorksCertification.Controls
                },
                techCardSemesterCoefficients: {
                    data: data.TechCardSemesterSignificanceCoefficients
                }
            },
            formulas: {
                allowLoadTechCard: function(get) {
                    var d = get('TechCardDisciplineCertification');
                    return d.Year && d.Semester && d.GroupId;
                }
            }
        },

        items: [{
            xtype: 'container',
            html: '<b>6.1. Весовой коэффициент значимости дисциплины</b> – …[<i>утверждается ученым советом института</i>], в том числе, <b>коэффициент значимости курсовых работ/проектов, если они предусмотрены</b> –...'
        }, {
            xtype: 'container',
            html: '<b>6.2. Процедуры текущей и промежуточной аттестации по дисциплине</b> [<i>в случае реализации дисциплины в течение нескольких семестров  текущая и промежуточная  аттестация проектируются для каждого семестра</i>]'
        }, {
            name: 'TechCardDisciplineCertification',
            contentReader: function(content, vm) {
                content.Lections.Controls = selectStoreItemsData(vm.get('techCardLections'));
                content.Practices.Controls = selectStoreItemsData(vm.get('techCardPractices'));
                content.Labs.Controls = selectStoreItemsData(vm.get('techCardLabs'));
                return content;
            },
            contentUpdater: function (content, vm) {
                var disciplineCertification = vm.get('TechCardDisciplineCertification');
                vm.set('TechCardDisciplineCertification', content);
                //disciplineCertification.Lections = content.Lections;
                //disciplineCertification.Practices = content.Practices;
                //disciplineCertification.Labs = content.Labs;
                vm.get('techCardLections').setData(content.Lections.Controls);
                vm.get('techCardPractices').setData(content.Practices.Controls);
                vm.get('techCardLabs').setData(content.Labs.Controls);
            },
            tbar: [{
                xtype: 'panel',
                border: true,
                layout: { type: 'hbox' },
                defaults: {
                    labelAlign: 'top',
                    margin: 6
                },
                items: [{
                    xtype: 'textfield',
                    fieldLabel: 'Укажите год',
                    width: 200,
                    value: data.TechCardDisciplineCertification.Year,
                    bind: '{TechCardDisciplineCertification.Year}'
                }, {
                    xtype: 'combobox',
                    fieldLabel: 'Выберите семестр',
                    editable: false,       
                    value: data.TechCardDisciplineCertification.Semester,
                    bind: {
                        value: '{TechCardDisciplineCertification.Semester}',
                        store: '{semesters}'
                    },
                    displayField: 'Name',
                    valueField: 'Name',
                    width: 200
                }, {
                    fieldLabel: 'Дисциплина',
                    xtype: 'displayfield',
                    bind: '{Name}',
                    width: 400
                }, {
                    xtype: 'comboedit',
                    fieldLabel: 'Выберите группу',
                    value: data.TechCardDisciplineCertification.GroupId,
                    bind: {
                        store: '{groups}',
                        value: '{TechCardDisciplineCertification.GroupId}'
                    },
                    displayField: 'Name',
                    valueField: 'Id',
                    width: 200
                }, {
                    xtype: 'button',
                    margin: '38 8 0 0',
                    text: 'Загрузить тех-карту',
                    bind: {
                        //disabled: '{!allowLoadTechCard}'
                    },
                    handler: function (sender, pressed, eOpts) {
                        var vm = this.lookupViewModel();

                        var d = vm.get('TechCardDisciplineCertification');
                        if (!d.Year || !d.Semester || !d.GroupId) {
                            vm.showMessage('Все поля параметров запроса карты должны быть заполнены');
                            return;
                        }

                        var needAsk = vm.get('TechCardSemesterSignificanceCoefficients').length;

                        if (needAsk && !vm.confirmMessage('Введенные данные и данные из технологической карты будут сброшены. Продолжить?'))
                            return;

                        var maskEl = this.up('#rootPanel').getEl();
                        maskEl.mask('Загрузка данных тех-карты...');
                        var paramsObject = vm.get('TechCardDisciplineCertification');
                        Ext.Ajax.request({
                            url: '/DisciplineWorkingProgram/GetTechCard',
                            params: {
                                documentId: documentId,
                                year: paramsObject.Year,
                                semester: paramsObject.Semester,
                                disciplineName: data.Name,
                                groupId: paramsObject.GroupId
                            },
                            success: function (response) {
                                var result = JSON.parse(response.responseText);
                                vm.set('TechCardDisciplineCertification', result.TechCardDisciplineCertification);
                                vm.set('TechCardCourseWorksCertification', result.TechCardCourseWorksCertification);
                                vm.set('TechCardSemesterSignificanceCoefficients', result.TechCardSemesterSignificanceCoefficients);
                                vm.get('techCardLections').setData(result.TechCardDisciplineCertification.Lections.Controls);
                                vm.get('techCardPractices').setData(result.TechCardDisciplineCertification.Practices.Controls);
                                vm.get('techCardLabs').setData(result.TechCardDisciplineCertification.Labs.Controls);
                                vm.get('techCardCourseWorks').setData(result.TechCardCourseWorksCertification.Controls);
                                vm.get('techCardSemesterCoefficients').setData(result.TechCardSemesterSignificanceCoefficients);
                                maskEl.unmask();
                            },
                            failure: function (d) {
                                maskEl.unmask();
                                console.error(d.responseText);
                                vm.showMessage(d.responseText);
                            }
                        });
                    }
                }]
            }],
            items: [{
                xtype: 'grid',
                enableLocking: false,
                enableColumnMove: false,
                enableColumnHide: false,
                sortableColumns: false,
                tools: [{
                    type: 'gear',
                    hidden: !data.canEdit,
                    tooltip: 'Редактирование блока',
                    callback: function () {
                        var vm = findRootVm(this);
                        var disciplineCertification = vm.get('TechCardDisciplineCertification');
                        var itemData = disciplineCertification.Lections;
                        var window = createEditLoadWindow(vm, 'lections', itemData, function (newData) {
                            vm.get('techCardLections').setData(newData.Controls);
                            vm.set('TechCardDisciplineCertification', {
                                Lections: newData,
                                Practices: disciplineCertification.Practices,
                                Labs: disciplineCertification.Labs
                            });
                        });
                        window.show();
                    }
                }],
                viewConfig: {
                    markDirty: false
                },
                bind: {
                    title: '1. Лекции: коэффициент значимости совокупных результатов лекционных занятий – {TechCardDisciplineCertification.Lections.TotalCoefficient}',
                    store: '{techCardLections}'
                },
                columns: [
                    { dataIndex: 'Name', text: 'Текущая аттестация на лекциях', width: 500 },
                    { dataIndex: 'Semester', text: 'Cеместр', width: 140, editor: { xtype: 'numberfield', allowDecimals: false, allowNegative: false, minValue: 1 } },
                    { dataIndex: 'Week', text: 'Учебная неделя', width: 140, editor: { xtype: 'textfield' } },
                    { dataIndex: 'MaxPoints', text: 'Максимальная оценка в баллах', width: 170 },
                ],
                plugins: [Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 })],
                bbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    defaults: { xtype: 'label' },
                    items: [
                        { bind: { text: 'Весовой коэффициент значимости результатов текущей аттестации по лекциям – {TechCardDisciplineCertification.Lections.CurrentCoefficient}' } },
                        { bind: { text: 'Промежуточная аттестация по лекциям – {TechCardDisciplineCertification.Lections.IntermediateCertification}' } },
                        { bind: { text: 'Весовой коэффициент значимости результатов промежуточной аттестации по лекциям – {TechCardDisciplineCertification.Lections.IntermediateCoefficient}' } },
                    ]
                }]
            }, {
                xtype: 'grid',
                enableLocking: false,
                enableColumnMove: false,
                enableColumnHide: false,
                sortableColumns: false,
                viewConfig: {
                    markDirty: false
                },
                bind: {
                    title: '2. Практические/семинарские занятия: коэффициент значимости совокупных результатов практических/семинарских занятий – {TechCardDisciplineCertification.Practices.TotalCoefficient}',
                    store: '{techCardPractices}'
                },
                tools: [{
                    type: 'gear',
                    hidden: !data.canEdit,
                    tooltip: 'Редактирование блока',
                    callback: function () {
                        var vm = findRootVm(this);
                        var disciplineCertification = vm.get('TechCardDisciplineCertification');
                        var itemData = disciplineCertification.Practices;
                        var window = createEditLoadWindow(vm, 'practices', itemData, function (newData) {
                            vm.get('techCardPractices').setData(newData.Controls);
                            vm.set('TechCardDisciplineCertification', {
                                Lections: disciplineCertification.Lections,
                                Practices: newData,
                                Labs: disciplineCertification.Labs
                            });
                        });
                        window.show();
                    }
                }],
                columns: [
                    { dataIndex: 'Name', text: 'Текущая аттестация на практических/семинарских занятиях', width: 500 },
                    { dataIndex: 'Semester', text: 'Cеместр', width: 140, editor: { xtype: 'numberfield', allowDecimals: false, allowNegative: false, minValue: 1 } },
                    { dataIndex: 'Week', text: 'Учебная неделя', width: 140, editor: { xtype: 'textfield'  } },
                    { dataIndex: 'MaxPoints', text: 'Максимальная оценка в баллах', width: 170 },
                ],
                plugins: [Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 })],
                bbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    defaults: { xtype: 'label' },
                    items: [
                        { bind: { text: 'Весовой коэффициент значимости результатов текущей аттестации по практическим/семинарским занятиям – {TechCardDisciplineCertification.Practices.CurrentCoefficient}' } },
                        { bind: { text: 'Промежуточная аттестация по практическим/семинарским занятиям – {TechCardDisciplineCertification.Practices.IntermediateCertification}' } },
                        { bind: { text: 'Весовой коэффициент значимости результатов промежуточной аттестации по практическим/семинарским занятиям – {TechCardDisciplineCertification.Practices.IntermediateCoefficient}' } },
                    ]
                }]
            }, {
                xtype: 'grid',
                enableLocking: false,
                enableColumnMove: false,
                enableColumnHide: false,
                sortableColumns: false,
                viewConfig: {
                    markDirty: false
                },
                bind: {
                    title: '3. Лабораторные занятия: коэффициент значимости совокупных результатов лабораторных занятий – {TechCardDisciplineCertification.Labs.TotalCoefficient}',
                    store: '{techCardLabs}'
                },
                tools: [{
                    type: 'gear',
                    hidden: !data.canEdit,
                    tooltip: 'Редактирование блока',
                    callback: function () {
                        var vm = findRootVm(this);
                        var disciplineCertification = vm.get('TechCardDisciplineCertification');
                        var itemData = disciplineCertification.Labs;
                        var window = createEditLoadWindow(vm, 'labs', itemData, function (newData) {
                            vm.get('techCardLabs').setData(newData.Controls);
                            vm.set('TechCardDisciplineCertification', {
                                Lections: disciplineCertification.Lections,
                                Practices: disciplineCertification.Practices,
                                Labs: newData
                            });
                        });
                        window.show();
                    }
                }],
                columns: [
                    { dataIndex: 'Name', text: 'Текущая аттестация на лабораторных занятиях', width: 500 },
                    { dataIndex: 'Semester', text: 'Cеместр', width: 140, editor: { xtype: 'numberfield', allowDecimals: false, allowNegative: false, minValue: 1 } },
                    { dataIndex: 'Week', text: 'Учебная неделя', width: 140, editor: { xtype: 'textfield' } },
                    { dataIndex: 'MaxPoints', text: 'Максимальная оценка в баллах', width: 170 },
                ],
                plugins: [Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 })],
                bbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    defaults: { xtype: 'label' },
                    items: [
                        { bind: { text: 'Весовой коэффициент значимости результатов текущей аттестации по лабораторным занятиям – {TechCardDisciplineCertification.Labs.CurrentCoefficient}' } },
                        { bind: { text: 'Промежуточная аттестация по лабораторным занятиям – {TechCardDisciplineCertification.Labs.IntermediateCertification}' } },
                        { bind: { text: 'Весовой коэффициент значимости результатов промежуточной аттестации по лабораторным занятиям – {TechCardDisciplineCertification.Labs.IntermediateCoefficient}' } },
                    ]
                }]
            }]
        }, {
            name: 'TechCardCourseWorksCertification',
            contentReader: function (content, vm) {
                content.Controls = selectStoreItemsData(vm.get('techCardCourseWorks'));
                return content;
            },
            contentUpdater: function (content, vm) {
                vm.get('techCardCourseWorks').setData(content.Controls);                
            },
            margin: '20 0 0 0',
            items: {
                xtype: 'grid',
                enableLocking: false,
                enableColumnMove: false,
                enableColumnHide: false,
                sortableColumns: false,
                viewConfig: {
                    markDirty: false
                },
                title: '6.3. Процедуры текущей и промежуточной аттестации курсовой работы/проекта',
                bind: {
                    store: '{techCardCourseWorks}'
                },
                tools: [{
                    type: 'gear',
                    hidden: !data.canEdit,
                    tooltip: 'Редактирование блока',
                    callback: function () {
                        var vm = findRootVm(this);
                        var courseWorkCertification = vm.get('TechCardCourseWorksCertification');
                        var window = createEditLoadWindow(vm, 'courseWork', courseWorkCertification, function (newData) {
                            vm.get('techCardCourseWorks').setData(newData.Controls);
                            vm.set('TechCardCourseWorksCertification', newData);
                        });
                        window.show();
                    }
                }],
                columns: [
                    { dataIndex: 'Name', text: 'Текущая аттестация выполнения курсовой работы/проекта', width: 500  },
                    { dataIndex: 'Semester', text: 'Cеместр', width: 140, editor: { xtype: 'numberfield', allowDecimals: false, allowNegative: false, minValue: 1 } },
                    { dataIndex: 'Week', text: 'Учебная неделя', width: 140, editor: { xtype: 'textfield' } },
                    { dataIndex: 'MaxPoints', text: 'Максимальная оценка в баллах', width: 170 }
                ],
                plugins: [Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 })],
                bbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    defaults: { xtype: 'label' },
                    items: [
                        { bind: { text: 'Весовой коэффициент текущей аттестации выполнения курсовой работы/проекта – {TechCardCourseWorksCertification.CurrentCoefficient}' } },
                        { bind: { text: 'Весовой коэффициент промежуточной аттестации выполнения курсовой работы/проекта - защиты – {TechCardCourseWorksCertification.IntermediateCoefficient}' } },                        
                    ]
                }]
            }
        }, {
            name: 'TechCardSemesterSignificanceCoefficients',
            contentReader: function(content, vm) {
                return selectStoreItemsData(vm.get('techCardSemesterCoefficients'));
            },
            margin: '20 0 0 0',
            items: {
                xtype: 'list',
                enableLocking: false,
                enableColumnMove: false,
                enableColumnHide: false,
                sortableColumns: false,
                /*allowDeleteItem: function(rec, meta) {
                    return meta.recordIndex !== 0;
                },*/
                plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
                    clicksToEdit: 1,
                    /*listeners: {
                        beforeedit: function (e, editor) {
                            if (editor.rowIdx == 0)
                                return false;                            
                        }
                    }*/
                })],
                title: '6.4. Коэффициент значимости семестровых результатов освоения дисциплины',
                bind: {
                    store: '{techCardSemesterCoefficients}'
                },
                tbar: [{
                    xtype: 'button',
                    text: 'Добавить строку',
                    /*bind: {
                        disabled: '{!TechCardSemesterSignificanceCoefficients.length}'
                    },*/
                    handler: function () {
                        var grid = this.up('grid');
                        var store = grid.getStore();
                        var rec = store.add({
                            SemesterNumber: null,
                            Coefficient: null
                        });
                        grid.editingPlugin.startEdit(rec[0], 0);
                    }
                }],
                columns: [
                    { dataIndex: 'SemesterNumber', text: 'Порядковый номер семестра по учебному плану, в котором осваивается дисциплина', width: 300, editor: { xtype: 'numberfield', allowNegative: false, allowDecimals: false } },
                    { dataIndex: 'Coefficient', text: 'Коэффициент значимости результатов освоения дисциплины в семестре', width: 300, editor: { xtype: 'numberfield', allowNegative: false } }
                ]
            }
        }]
    };

    function createEditLoadWindow(vm, blockType, itemData, successCallback) {
        itemData = Ext.clone(itemData);

        var emptyControl = {
            Name: null,
            Semester: null,
            Week: null,
            MaxPoints: null
        };

        var window = new Ext.window.Window({
            viewModel: {
                confirmMessage: vm.confirmMessage,
                showMessage: vm.showMessage,
                formulas: {
                    loadName: function() {
                        switch (blockType) {
                        case 'lections':
                            return 'Лекции';                           
                        case 'practices':
                            return 'Практические/семинарские занятия';                            
                        case 'labs':
                            return 'Лабораторные занятия';                            
                        case 'courseWork':
                            return 'Курсовая работа/проект';                            
                        }
                        return null;
                    }
                },
                stores: {
                    CurrentControls: {
                        data: itemData.Controls
                    }                    
                },
                data: itemData
            },
            closeAction: 'destroy',
            bind: {
                title: "Редактирование блока '{loadName}'"
            },
            width: 800,
            autoHeight: true,
            modal: true,
            layout: { type: 'vbox', align: 'stretch' },
            bodyPadding: 8,
            items: [{
                xtype: 'numberfield',
                fieldLabel: 'Коэффициент значимости совокупных результатов',
                hidden: blockType === 'courseWork',
                labelWidth: 400,
                step: 0.1,
                maxValue: 1,
                minValue: 0,
                allowDecimals: true,
                allowNegative: false,
                bind: '{TotalCoefficient}'
            }, {
                xtype: 'list',
                plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
                    clicksToEdit: 1
                })],
                title: 'Текущая аттестация',
                margin: '0 0 8 0',
                height: 400,
                tbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'start' },
                    items: [{
                        xtype: 'numberfield',
                        itemId: 'currentCoefficientEditor',
                        fieldLabel: 'Коэффициент значимости',
                        step: 0.1,
                        labelWidth: 200,
                        maxValue: 1,
                        minValue: 0,
                        allowDecimals: true,
                        allowNegative: false,
                        bind: {
                            value: '{CurrentCoefficient}'
                        }
                    }, {
                        xtype: 'container',
                        layout: { type: 'hbox' },
                        items: [{
                            xtype: 'button',
                            hidden: blockType === 'courseWork',
                            text: 'Выбрать КМ',
                            handler: function () {
                                var store = this.up('grid').getStore();
                                var selectorWindow = createControlSelectorWindow(blockType, 'current', function (selection) {
                                    store.add(selection);
                                });
                                selectorWindow.show();
                            }
                        }, {
                            xtype: 'button',
                            text: 'Добавить КМ',
                            margin: '0 0 0 8',
                            handler: function () {
                                var grid = this.up('grid');
                                var store = grid.getStore();
                                var newControl = Ext.clone(emptyControl);
                                var rec = store.add(newControl);
                                grid.editingPlugin.startEdit(rec[0], 0);
                            }
                        }]
                    }]
                }],
                bind: {
                    store: '{CurrentControls}'
                },
                columns: [{
                    text: 'Название КМ',
                    dataIndex: 'Name',
                    editor: { xtype: 'textfield' },
                    flex: 3
                }, {
                    text: 'Семестр',
                    dataIndex: 'Semester',
                    editor: { xtype: 'numberfield', allowDecimals: false, allowNegative: false, minValue: 1 },
                    flex: 1
                }, {
                    text: 'Учебная неделя',
                    dataIndex: 'Week',
                    editor: { xtype: 'textfield' },
                    flex: 1
                }, {
                    text: 'Максимальная оценка в баллах',
                    dataIndex: 'MaxPoints',
                    editor: { xtype: 'numberfield', allowDecimals: false, allowNegative: false, maxValue: 100 },
                    flex: 1.5
                }]
            }, {
                xtype: 'panel',
                title: 'Промежуточная аттестация',
                layout: { type: 'vbox', align: 'stretch' },
                tbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'start' },
                    items: [{
                        xtype: 'numberfield',
                        fieldLabel: 'Коэффициент значимости',
                        itemId: 'intermediateCoefficientEditor',
                        step: 0.1,
                        labelWidth: 200,
                        maxValue: 1,
                        minValue: 0,
                        allowDecimals: true,
                        allowNegative: false,
                        bind: {                            
                            value: '{IntermediateCoefficient}'
                        }
                    }, {
                        xtype: 'button',
                        hidden: blockType === 'courseWork',
                        text: 'Выбрать КМ',
                        handler: function (btn) {
                            var vm = btn.lookupViewModel();
                            var selectorWindow = createControlSelectorWindow(blockType, 'intermediate', function (selection) {
                                vm.set('IntermediateCertification', selection[0].get('Name'));
                            });
                            selectorWindow.show();                            
                        }
                    }]
                }],
                items: [{
                    xtype: 'textfield',
                    editable: true,
                    hidden: blockType === 'courseWork',
                    bind: '{IntermediateCertification}'
                }/*, {
                    xtype: 'button',
                    text: 'X',
                    handler: function (btn) {
                        var vm = btn.lookupViewModel();
                        if (vm.confirmMessage('Вы действительно желаете очистить выбранное значение?')) {
                            vm.set('IntermediateCertification', null);
                        }
                    }
                }*/]
            }],
            buttons: [{
                text: 'ОК',
                handler: function () {
                    var vm = this.lookupViewModel();
                    var data = Ext.clone(vm.getData());

                    data.Controls = selectStoreItemsData(data.CurrentControls);

                    if ((+data.CurrentCoefficient || 0) +
                        (+data.IntermediateCoefficient || 0) !==
                        1) {
                        vm.showMessage('Сумма коэффициентов текущей и промежуточной аттестации должна быть равна 1.');
                        return;
                    }

                    if (data.Controls.length) {
                        var pointsTotal = data.Controls.reduce(function(p, c) {
                            return p + (c.MaxPoints || 0);
                        }, 0);
                        if (pointsTotal !== 100) {
                            vm.showMessage('Сумма баллов в нагрузке должна быть равна 100.');
                            return;
                        }
                    }

                    delete data.CurrentControls;
                    delete data.loadData;

                    var window = this.up('window');
                    var maskEl = window.getEl();
                    maskEl.mask('Применение изменений...');

                    Ext.Ajax.request({
                        url: '/DisciplineWorkingProgram/PrepareTechCardCertificationItem',
                        jsonData: data,
                        success: function(response) {
                            var result = JSON.parse(response.responseText);
                            successCallback(result);
                            maskEl.unmask();
                            window.close();
                        },                        
                        failure: function (d) {
                            maskEl.unmask();
                            console.error(d.responseText);
                            vm.showMessage(d.responseText);
                        }
                    });                    
                }
            }, {
                text: 'Отмена',
                handler: function() {
                    this.up('window').close();
                }
            }]
        });
        return window;
    }

    function createControlSelectorWindow(loadKind, loadType, successCallback) {
        var window = new Ext.window.Window({
            viewModel: {

            },
            width: 400,
            height: 500,
            modal: true,
            layout: 'fit',
            title: 'Выбор КМ',
            closeAction: 'destroy',
            items: [{
                xtype: 'grid',
                hideHeaders: true,
                selModel: 'rowmodel',
                multiSelect: true,
                allowDeselect: true,
                reference: 'grid',
                store: {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: '/DisciplineWorkingProgram/GetControls',
                        extraParams: {
                            loadKind: loadKind,
                            loadType: loadType
                        }
                    }
                },
                columns: [{
                    dataIndex: 'Name',
                    flex: 1
                }]
            }],
            buttons: [{
                text: 'ОК',
                bind: {
                    disabled: '{!grid.selection}'
                },
                handler: function() {
                    successCallback(this.up('window').down('grid').getSelection());
                    this.up('window').close();
                }
            }, {
                text: 'Отмена',
                handler: function() {
                    this.up('window').close();
                }
            }]
        });
        return window;
    }
}