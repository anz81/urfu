function uiInit(documentId, documentType, data, schema, emptyData, user, additionalData) {
    var boldStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };
    var captionStyle = { fontWeight: 'bold' };

    function prepareTechCardDisciplineCertificationsItems(practiceForms, directionCode) {
        return practiceForms.map(function (link, i) {
            return {
                xtype: 'panel',
                title: directionCode + ', ' + link.Title,
                header: true,
                border: true,
                margin: '0 0 10 0',
                bodyPadding: 6,
                defaults: {
                    labelWidth: 200
                },
                items: [
                    {
                        xtype: 'container',
                        html: '<b>Процедуры текущей и промежуточной аттестации по дисциплине</b> [<i>в случае реализации дисциплины в течение нескольких семестров  текущая и промежуточная  аттестация проектируются для каждого семестра</i>]'
                    },
                    {
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
                            itemId: 'year' + link.DisciplineUid,
                            width: 200,
                            value: link.TechCardDisciplineCertification.Year,
                        }, {
                            xtype: 'combobox',
                            fieldLabel: 'Выберите семестр',
                            itemId: 'semester' + link.DisciplineUid,
                            editable: false,
                            value: link.TechCardDisciplineCertification.Semester,
                            bind: {
                                store: '{semesters}'
                            },
                            displayField: 'Name',
                            valueField: 'Name',
                            width: 200
                        }, {
                            fieldLabel: 'Дисциплина',
                            xtype: 'displayfield',
                            value: link.DisciplineName,//link.Title,
                            autoWidth: true,
                            maxWidth: 300
                        }, {
                            xtype: 'comboedit',
                            fieldLabel: 'Выберите группу',
                            itemId: 'groupId' + link.DisciplineUid,
                            value: link.TechCardDisciplineCertification.GroupId,
                            bind: {
                                store: '{groups}'
                            },
                            displayField: 'Name',
                            valueField: 'Id',
                            width: 200
                        }, {
                            xtype: 'button',
                            margin: '38 8 0 0',
                            text: 'Загрузить тех-карту',
                                handler: function (sender, pressed, eOpts) {
                                    var items = this.up().items.items;
                                    var year = items[0].getValue();
                                    var semester = items[1].getValue();
                                    var disciplineName = items[2].getValue();
                                    var groupId = items[3].getValue();
                                    var t = this;
                                    if (!year || !semester || !groupId) {
                                        Ext.MessageBox.alert('Ошибка', 'Все поля параметров запроса карты должны быть заполнены');
                                        return;
                                    }

                                    var request = function () {
                                        var maskEl = t.up('#rootPanel').getEl();
                                        maskEl.mask('Загрузка данных тех-карты...');

                                        Ext.Ajax.request({
                                            url: '/PracticesWorkingProgram/GetTechCard',
                                            params: {
                                                documentId: documentId,
                                                year: year,
                                                semester: semester,
                                                disciplineName: disciplineName,
                                                groupId: groupId
                                            },
                                            success: function (response) {
                                                var result = JSON.parse(response.responseText);
                                                t.up().up().up().down('#techCard' + link.DisciplineUid).store.setData(result.TechCardDisciplineCertification.EduLoad.Controls);
                                                t.up().up().up().down('#semesterSignificanceCoefficients' + link.DisciplineUid).store.setData(result.TechCardSemesterSignificanceCoefficients);
                                                t.up().up().up().down('#currentCoefficient' + link.DisciplineUid).setText(result.TechCardDisciplineCertification.EduLoad.CurrentCoefficient);
                                                t.up().up().up().down('#intermediateCertification' + link.DisciplineUid).setText(result.TechCardDisciplineCertification.EduLoad.IntermediateCertification);
                                                t.up().up().up().down('#intermediateCoefficient' + link.DisciplineUid).setText(result.TechCardDisciplineCertification.EduLoad.IntermediateCoefficient);

                                                maskEl.unmask();
                                            },
                                            failure: function (d) {
                                                maskEl.unmask();
                                                Ext.MessageBox.alert('Ошибка', 'Тех. карта не найдена');
                                            }
                                        });
                                    }
                                    
                                    if (this.up().up().items.items[2].store.data.items.length != 0) {
                                        Ext.MessageBox.show({
                                            title: 'Предупреждение',
                                            msg: 'Введенные данные и данные из технологической карты будут сброшены. Продолжить?',
                                            buttons: Ext.MessageBox.YESNO,
                                            fn: function (btn) {
                                                if (btn === 'no') {
                                                    return;
                                                }
                                                if (btn === 'yes') {

                                                    request();

                                                }
                                            }
                                        });
                                    }
                                    else
                                        request();
                            }
                        }]
                    },
                    {
                        xtype: 'grid',
                        enableLocking: false,
                        enableColumnMove: false,
                        enableColumnHide: false,
                        sortableColumns: false,
                        itemId: 'techCard' + link.DisciplineUid,
                        title: 'Нагрузка: коэффициент значимости совокупных результатов – 1',
                        tools: [{
                            type: 'gear',
                            hidden: !data.canEdit,
                            tooltip: 'Редактирование блока',
                            callback: function () {
                                var vm = findRootVm(this);
                                var disciplineCertification = link.TechCardDisciplineCertification;
                                var itemData = disciplineCertification.EduLoad;
                                var blockType = 'practice';
                                var t = this;
                                var window = createEditLoadWindow(vm, blockType, itemData, function (newData) {
                                    t.up('#techCard' + link.DisciplineUid).store.setData(newData.Controls);
                                    t.up().up().up().down('#currentCoefficient' + link.DisciplineUid).setText(newData.CurrentCoefficient);
                                    t.up().up().up().down('#intermediateCertification' + link.DisciplineUid).setText(newData.IntermediateCertification);
                                    t.up().up().up().down('#intermediateCoefficient' + link.DisciplineUid).setText(newData.IntermediateCoefficient);
                                });
                                window.show();
                            }
                        }],
                        viewConfig: {
                            markDirty: false
                        },
                        store: {
                            data: link.TechCardDisciplineCertification.EduLoad.Controls
                        },
                        columns: [
                            { dataIndex: 'Name', text: 'Текущая аттестация на практике', width: 500 },
                            { dataIndex: 'Semester', text: 'Cеместр', width: 140, editor: { xtype: 'numberfield', allowDecimals: false, allowNegative: false, minValue: 1 } },
                            { dataIndex: 'Week', text: 'Учебная неделя', width: 140, editor: { xtype: 'textfield' } },
                            { dataIndex: 'MaxPoints', text: 'Максимальная оценка в баллах', width: 170 },
                        ],
                        plugins: [Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 })],
                        bbar: [
                            {
                            xtype: 'container',
                            layout: { type: 'vbox', align: 'stretch' },
                            defaults: { xtype: 'label' },
                            items: [
                                {
                                    xtype: 'panel',
                                    border: false,
                                    layout: { type: 'hbox' },
                                    items: [{
                                        xtype: 'label',
                                        text: 'Весовой коэффициент значимости результатов текущей аттестации по нагрузке – ',
                                    },
                                    {
                                        xtype: 'label',
                                        itemId: 'currentCoefficient' + link.DisciplineUid,
                                        text: link.TechCardDisciplineCertification.EduLoad.CurrentCoefficient
                                    }
                                    ]
                                },
                                {
                                    xtype: 'panel',
                                    border: false,
                                    layout: { type: 'hbox' },
                                    items: [{
                                        xtype: 'label',
                                        text: 'Промежуточная аттестация по нагрузке – ',
                                    },
                                    {
                                        xtype: 'label',
                                        itemId: 'intermediateCertification' + link.DisciplineUid,
                                        text: link.TechCardDisciplineCertification.EduLoad.IntermediateCertification
                                    }
                                    ]
                                },
                                {
                                    xtype: 'panel',
                                    border: false,
                                    layout: { type: 'hbox' },
                                    items: [{
                                        xtype: 'label',
                                        text: 'Весовой коэффициент значимости результатов промежуточной аттестации по нагрузке – ',
                                    },
                                    {
                                        xtype: 'label',
                                        itemId: 'intermediateCoefficient' + link.DisciplineUid,
                                        text: link.TechCardDisciplineCertification.EduLoad.IntermediateCoefficient
                                    }
                                    ]
                                },
                            ]
                        }]
                    },
                    {
                        xtype: 'container',
                        html: '<b>Коэффициент значимости семестровых результатов освоения дисциплины</b>'
                    },
                    {
                        xtype: 'list',
                        enableLocking: false,
                        enableColumnMove: false,
                        enableColumnHide: false,
                        sortableColumns: false,
                        plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
                            clicksToEdit: 1
                        })],
                        title: link.Title,
                        header: false,
                        itemId: 'semesterSignificanceCoefficients' + link.DisciplineUid,
                        store: {
                            data: link.TechCardSemesterSignificanceCoefficients
                        },
                        tbar: [{
                            xtype: 'button',
                            text: 'Добавить строку',
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
                ]
            }
        });
    }

    function prepareTechCardDisciplineItems(practiceForms) {
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
                        items: prepareTechCardDisciplineCertificationsItems(Ext.clone(link.Items), link.DirectionCode)
                    }
                ]
            }
        });
    }
    
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
                        url: '/PracticesWorkingProgram/GetGroups/' + window.location.search,
                        reader: { type: 'json' },
                        extraParams: {
                            documentType: documentType,
                            documentId: documentId
                        }
                    }
                }
            },
            formulas: {
                allowLoadTechCard: function(get) {
                    var d = get('TechCardDisciplineCertification');
                    return d.Year && d.Semester && d.GroupId;
                }
            }
        },
        items: [
            {
                name: 'PracticeEvalutionStudentPracticeStructure',
                contentReader: function (content, vm) {
                    var clone = Ext.clone(content);
                    var t = this;
                    clone.forEach(function (item, index, array) {
                        clone[index].Items.forEach(function (item2, index2, array2) {
                            var disciplineUid = clone[index].Items[index2].DisciplineUid;
                            clone[index].Items[index2].TechCardDisciplineCertification.EduLoad.Controls = t.down('#techCard' + disciplineUid).getStore().getData().items.map(function (i) { return i.getData() });
                            clone[index].Items[index2].TechCardDisciplineCertification.Year = t.down('#year' + disciplineUid).getValue();
                            clone[index].Items[index2].TechCardDisciplineCertification.Semester = t.down('#semester' + disciplineUid).getValue();
                            clone[index].Items[index2].TechCardDisciplineCertification.GroupId = t.down('#groupId' + disciplineUid).getValue();
                            clone[index].Items[index2].TechCardSemesterSignificanceCoefficients = selectStoreItemsData(t.down('#semesterSignificanceCoefficients' + disciplineUid).getStore());
                            clone[index].Items[index2].TechCardDisciplineCertification.EduLoad.CurrentCoefficient = t.down('#currentCoefficient' + disciplineUid).text;
                            clone[index].Items[index2].TechCardDisciplineCertification.EduLoad.IntermediateCertification = t.down('#intermediateCertification' + disciplineUid).text;
                            clone[index].Items[index2].TechCardDisciplineCertification.EduLoad.IntermediateCoefficient = t.down('#intermediateCoefficient' + disciplineUid).text;
                        });
                    });
                    return clone;
                },
                items: [
                    {
                        xtype: 'container',
                        html: '<b>Весовой коэффициент значимости дисциплины</b> – …[<i>утверждается ученым советом института</i>], в том числе, <b>коэффициент значимости курсовых работ/проектов, если они предусмотрены</b> –...'
                    },
                    {
                        items: prepareTechCardDisciplineItems(Ext.clone(data.PracticeEvalutionStudentPracticeStructure))
                    }
                ]
            }
        ]
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
                stores: {
                    CurrentControls: {
                        data: itemData.Controls
                    }                    
                },
                data: itemData
            },
            closeAction: 'destroy',
            title: "Редактирование блока 'Нагрузка'",
            width: 800,
            autoHeight: true,
            modal: true,
            layout: { type: 'vbox', align: 'stretch' },
            bodyPadding: 8,
            items: [{
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
                }]
            }],
            buttons: [{
                text: 'ОК',
                handler: function () {
                    var vm = this.lookupViewModel();
                    var data = Ext.clone(vm.getData());

                    data.Controls = selectStoreItemsData(data.CurrentControls);

                    if ((+(data.CurrentCoefficient.toString().replace(",",".")) || 0) +
                        (+(data.IntermediateCoefficient.toString().replace(",", ".")) || 0) !==
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