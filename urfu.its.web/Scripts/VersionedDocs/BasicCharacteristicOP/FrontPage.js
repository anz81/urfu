function uiInit(documentId, documentType, data, schema, emptyData, canEdit, user) {

    var instituteBlock = function (number) {
        return {
            name: 'Institute',
            items: [{
                xtype: 'label',
                text: `${number}) ${data.Institute.Name}`
            }],
            buttons: []
        };
    };

    var directionBlock = function (number) {
        return {
            name: 'Direction',
            items: [{
                xtype: 'label',
                text: `${number}) Направление ${data.Direction.Code} ${data.Direction.Title}`
            }],
            buttons: []
        };
    };

    var profileBlock = function (number) {
        return {
            name: 'Profile',
            items: [{
                xtype: 'label',
                text: `${number}) Образовательная программа ${data.Profile.Code} ${data.Profile.Name}`
            }],
            buttons: []
        };
    };
    
    var authorsBlock = function (number) {
        return {
            name: 'Authors',
            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: `${number}) Общая характеристика основной образовательной программы (далее - ОХОП) составлена авторами:`
            }, {
                xtype: 'grid',
                header: false,
                loadMask: true,
                bind: {
                    store: '{Authors}'
                },

                tbar: [{
                    xtype: 'button',
                    text: 'Выбрать',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleSelectorWindow(function (selection) {
                            selection.forEach(function (item) {
                                var itemFio = item.get('Fio');
                                var insertIndex = getOrderedIndex(vm.get('Authors'), function (author) {
                                    return author.Fio <= itemFio;
                                });
                                vm.get('Authors').insert(insertIndex, item);
                            });
                        }, vm, 'authors');
                        modal.show();
                    }
                }, {
                    xtype: 'button',
                    text: 'Добавить',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleWindow(function (people) {
                            vm.get('authors').add(people);
                            var itemFio = people.Fio;
                            var insertIndex = getOrderedIndex(vm.get('Authors'), function (author) {
                                return author.Fio <= itemFio;
                            });
                            vm.get('Authors').insert(insertIndex, people);
                        }, vm);
                        modal.show();
                    }
                }],

                columns: [{
                    header: 'ФИО',
                    dataIndex: 'Fio',
                    width: 250,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Ученая степень/ученое звание',
                    dataIndex: 'Degree',
                    width: 150,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Должность',
                    dataIndex: 'Post',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Подразделение (полное наименование)',
                    dataIndex: 'Cathedra',
                    width: 200,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                }, {
                    xtype: 'actioncolumn',
                    resizable: false,
                    sortable: false,
                    width: 60,
                    items: [{
                        icon: '/Content/Images/remove.png',
                        tooltip: 'Удалить',
                        handler: function (grid, rowIndex, colIndex, item, e, record) {
                            if (confirm('Вы действительно желаете удалить запись?')) {
                                grid.getStore().remove(record);
                            }
                        }
                    }]
                }]
            }]
        };
    };

    var eduProgramHeadBlock = function (number) {
        return {
            name: 'EduProgramHead',
            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: number + ')  Руководитель образовательной программы (ОП) [И.О. Фамилия]'
            }, {
                xtype: 'panel',
                header: false,
                tbar: [{
                    xtype: 'button',
                    text: 'Выбрать',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleSelectorWindow(function (selection) {
                            vm.set('EduProgramHead', selection[0]);
                        }, vm, 'authors');
                        modal.show();
                    }
                }, {
                    xtype: 'button',
                    text: 'Добавить',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleWindow(function (people) {
                            vm.get('authors').add(people);
                            vm.set('EduProgramHead', people);
                        }, vm);
                        modal.show();
                    }
                }],

                layout: { type: 'hbox', align: 'stretch' },
                items: [{
                    xtype: 'textfield',
                    editable: false,
                    width: 400,
                    bind: '{EduProgramHead.ShortName}'
                }, {
                    xtype: 'button',
                    text: 'X',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        if (vm.confirmMessage('Вы действительно желаете очистить выбранное значение?')) {
                            vm.set('EduProgramHead', {
                                ShortName: null,
                                AuthorId: null,
                                TeacherId: null
                            });
                        }
                    }
                }]
            }]
        };
    };

    var councilBlock = function (number) {
        return {
            name: 'Council',
            header: false,
            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: number + ') Рекомендовано учебно-методическим советом института [полное наименование института, в котором реализуется образовательная программа]'
            }, {
                xtype: 'textfield',
                editable: false,
                bind: '{Institute.Name}',
                width: 400
            }, {
                xtype: 'label',
                text: 'Председатель учебно-методического совета [И.О. Фамилия]'
            }, {
                xtype: 'panel',
                header: false,
                tbar: [{
                    xtype: 'button',
                    text: 'Выбрать',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleSelectorWindow(function (selection) {
                            vm.set('Council', {
                                Chairman: {
                                    ShortName: selection[0].get('ShortName'),
                                    TeacherId: selection[0].get('TeacherId'),
                                    AuthorId: null
                                },
                                ProtocolNumber: vm.get('Council').ProtocolNumber,
                                ProtocolDate: vm.get('Council').ProtocolDate
                            });
                        }, vm, 'authors');
                        modal.show();
                    }
                }, {
                    xtype: 'button',
                    text: 'Добавить',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleWindow(function (people) {
                            vm.get('authors').add(people);
                            vm.set('Council', {
                                Chairman: {
                                    ShortName: people.ShortName,
                                    AuthorId: people.AuthorId,
                                    TeacherId: null
                                },
                                ProtocolNumber: vm.get('Council').ProtocolNumber,
                                ProtocolDate: vm.get('Council').ProtocolDate
                            });
                        }, vm);
                        modal.show();
                    }
                }],

                layout: { type: 'hbox', align: 'stretch' },
                items: [{
                    xtype: 'textfield',
                    editable: false,
                    width: 400,
                    bind: {
                        value: {
                            bindTo: '{Council.Chairman.ShortName}',
                            deep: true
                        }
                    }
                }, {
                    xtype: 'button',
                    text: 'X',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        if (vm.confirmMessage('Вы действительно желаете очистить выбранное значение?')) {
                            vm.set('Council', {
                                Chairman: Ext.apply({}, emptyData['Council'].Chairman),
                                ProtocolNumber: vm.get('Council').ProtocolNumber,
                                ProtocolDate: vm.get('Council').ProtocolDate
                            });
                        }
                    }
                }]
            }, {
                xtype: 'container',
                layout: {
                    type: 'hbox',
                    align: 'center'
                },
                margin: "8 0 0 0",
                items: [{
                    fieldLabel: 'Протокол №',
                    xtype: 'textfield',
                    bind: '{Council.ProtocolNumber}',
                    width: 210
                }, {
                    labelAlign: 'right',
                    xtype: 'datefield',
                    fieldLabel: 'от',
                    bind: {
                        value: '{Council.ProtocolDate}'
                    },
                    width: 270,
                    submitFormat: 'd.m.Y',
                    format: 'd.m.Y'
                }, {
                    xtype: 'label',
                    margin: '0 0 0 8',
                    text: 'г.'
                }]
            }]
        };
    };


    var firstChange = true;

    var ratifyingDataBlock = function (number) {
        return {
            name: 'RatifyingInfo',
            header: false,
            layout: { type: 'vbox', align: 'stretch' },
            items: [
                {
                    xtype: 'label',
                    text: `${number}) Данные о согласующих`
                },
                {
                    xtype: 'container',
                    layout: { type: 'hbox' },
                    items: [
                        {
                            xtype: 'comboedit',
                            labelWidth: 130,
                            fieldLabel: `Должность согласующего`,
                            displayField: 'RatifyingPersonPost',
                            valueField: 'RatifyingPersonPost',
                            width: 500,
                            store: ratifyingPersonPostStore,
                            bind: {
                                value: '{RatifyingInfo.RatifyingPersonPost}'
                            },
                            listeners: {
                                change: function (t, newValue) {
                                    if (firstChange) {
                                        firstChange = false;
                                        return;
                                    }
                                    var personNameCmbx = t.up().up().getComponent("RatifyingPersonNameContainer").getComponent("RatifyingPersonNameCmbx");
                                    personNameCmbx.setValue(null);
                                    var names = getRatifyingPersonNames(newValue);
                                    personNameCmbx.getStore().setData(names);
                                }
                            }
                        }
                    ]
                },
                {
                    xtype: 'container',
                    layout: { type: 'hbox' },
                    itemId: "RatifyingPersonNameContainer",
                    items: [
                        {
                            xtype: 'comboedit',
                            itemId: "RatifyingPersonNameCmbx",
                            labelWidth: 130,
                            fieldLabel: `Согласующий`,
                            displayField: 'RatifyingPersonName',
                            valueField: 'RatifyingPersonName',
                            width: 500,
                            store: ratifyingPersonNameStore,
                            bind: {
                                value: '{RatifyingInfo.RatifyingPersonName}'
                            }
                        }
                    ]
                },
                {
                    xtype: 'container',
                    layout: { type: 'hbox' },
                    items: [
                        {
                            xtype: 'comboedit',
                            labelWidth: 130,
                            fieldLabel: `Согласующее подразделение`,
                            displayField: 'RatifyingSubdivision',
                            valueField: 'RatifyingSubdivision',
                            width: 500,
                            store: ratifyingSubdivisionStore,
                            bind: {
                                value: '{RatifyingInfo.RatifyingSubdivision}'
                            }
                        }
                    ]
                }
            ]
        }
    };

    var directionHeadBlock = function (number) {
        return {
            name: 'DirectionHead',
            header: false,

            layout: { type: 'vbox', align: 'stretch' },

            items: [
                {
                    xtype: 'label',
                    text: `${number}) Согласовано`
                },
                {
                    xtype: 'label',
                    bind: {
                        text: '{RatifyingInfo.RatifyingSubdivision}'
                    }
                },
                {
                    xtype: 'panel',
                header: false,
                tbar: [{
                    xtype: 'button',
                    text: 'Выбрать',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleSelectorWindow(function (selection) {
                            vm.set('DirectionHead', {
                                ShortName: selection[0].get('ShortName'),
                                TeacherId: selection[0].get('TeacherId'),
                                AuthorId: null
                            });
                        }, vm, 'authors');
                        modal.show();
                    }
                }, {
                    xtype: 'button',
                    text: 'Добавить',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleWindow(function (people) {
                            vm.get('authors').add(people);
                            vm.set('DirectionHead', {
                                ShortName: people.ShortName,
                                AuthorId: people.AuthorId,
                                TeacherId: null
                            });
                        }, vm);
                        modal.show();
                    }
                }],

                layout: {
                    type: 'hbox',
                    align: 'stretch'
                },
                items: [{
                    xtype: 'textfield',
                    editable: false,
                    width: 400,
                    bind: '{DirectionHead.ShortName}'
                }, {
                    xtype: 'button',
                    text: 'X',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        if (vm.confirmMessage('Вы действительно желаете очистить выбранное значение?')) {
                            vm.set('DirectionHead', {
                                ShortName: null,
                                AuthorId: null,
                                TeacherId: null
                            });
                        }
                    }
                }]
            }]
        };
    };

    var requisiteOrdersBlock = function (number) {
        return {
            name: 'RequisitesOrders',
            dataReader: function (block) {
                var vm = findRootVm(block);
                var items = vm.get('RequisitesOrders').getData().items.map(function (r) { return r.getData() });
                return { RequisitesOrders: items };
            },
            contentReader: function (content, vm) {
                content.RequisitesOrders = selectStoreItemsData(vm.get('RequisitesOrders'));
                return content;
            },

            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: data.Direction.Standard === 'СУОС'
                    ? `${number}) Реквизиты приказа УрФУ об утверждении ${data.Direction.Standard}`
                    : `${number}) Реквизиты приказа Минобрнауки РФ ${data.Direction.Standard}`
            }, {
                xtype: 'grid',
                header: false,
                loadMask: true,
                bind: {
                    store: '{RequisitesOrders}'
                },
                tbar: [{
                    xtype: 'button',
                    text: 'Обновить',
                    cls: 'btn-text-color',
                    style: {
                        borderColor: '#157fcc'
                    },
                    handler: function (btn) {
                        var vm = findRootVm(this);
                        var orders = vm.get('RequisitesOrders');
                        var thisBlock = this.up().up();
                        thisBlock.mask('Обновление');
                        Ext.Ajax.request({
                            method: 'GET',
                            url: '/BasicCharacteristicOP/RequisitesOrders',
                            params: {
                                id: data.EduProgramInfo.Id
                            },
                            success: function (response) {
                                thisBlock.unmask();
                                try {
                                    var data = Ext.decode(response.responseText);
                                    orders.setData(data.orders);
                                }
                                catch{
                                    Ext.MessageBox.show({
                                        title: 'Ошибка',
                                        msg: 'Повторите попытку позже',
                                        buttons: Ext.MessageBox.OK
                                    });
                                }
                            },
                            failure: function (response) {
                                thisBlock.unmask();
                                Ext.MessageBox.show({
                                    title: 'Ошибка',
                                    msg: 'Повторите попытку позже',
                                    buttons: Ext.MessageBox.OK
                                });
                            }
                        });
                    }
                }],
                columns: [{
                    header: 'Id',
                    dataIndex: 'Id',
                    width: 100,
                    hidden: true,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Код направления',
                    dataIndex: 'DirectionCode',
                    width: 150,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Номер приказа',
                    dataIndex: 'Number',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Дата приказа',
                    dataIndex: 'Date',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
                }]
            }]
        };
    };

    var ratifyingPersonPostStore = Ext.create('Ext.data.Store', {});
    var ratifyingPersonNameStore = Ext.create('Ext.data.Store', {});
    var ratifyingSubdivisionStore = Ext.create('Ext.data.Store', {});

    function getRatifyingPersonNames(post) {
        return Array.from(new Set(ratifyingDataStore.getData().items.filter(d => d.data.RatifyingPersonPost == post || post == null)
            .map(d => d.data.RatifyingPersonName)))
            .map(function (d) { var obj = {}; obj.RatifyingPersonName = d; return obj; });
    };

    var ratifyingDataStore = Ext.create('Ext.data.Store',
        {
            proxy:
            {
                type: 'ajax',
                url: '/BasicCharacteristicOP/RatifyData',
                reader: { type: 'json' }
            },
            autoLoad: true,
            listeners: {
                load: function (t, records) {
                    ratifyingPersonPostStore.setData(
                        Array.from(new Set(records.map(d => d.data.RatifyingPersonPost)))
                            .map(function (d) { var obj = {}; obj.RatifyingPersonPost = d; return obj; })
                    );

                    ratifyingPersonNameStore.setData(getRatifyingPersonNames());

                    ratifyingSubdivisionStore.setData(
                        Array.from(new Set(records.map(d => d.data.RatifyingSubdivision)))
                            .map(function (d) { var obj = {}; obj.RatifyingSubdivision = d; return obj; })
                    );
                }
            }
        });
    ratifyingDataStore.load();

    var blocks = [
        {
            name: 'EduProgramInfo',
            hidden: true
        },
            instituteBlock(1),
            directionBlock(2),
            profileBlock(3),
            authorsBlock(4),
            eduProgramHeadBlock(5),
            councilBlock(6),
            ratifyingDataBlock(7),
            directionHeadBlock(8),
            requisiteOrdersBlock(9)
        ];
    
    return {
        viewModel: {
            stores: {
                Authors: {
                    data: data.Authors
                },
                authors: {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: '/WorkingPrograms/GetAuthors/' + window.location.search,
                        reader: { type: 'json' },
                        extraParams: {
                            documentType: documentType,
                            documentId: documentId
                        }
                    }
                },
                RequisitesOrders: {
                    data: data.RequisitesOrders
                }
            }
        },
        items: blocks
    };

    function createPeopleWindow(selectHandler, rootVm) {
        var modal = Ext.create('Ext.window.Window', {
            viewModel: {
                data: {
                    FirstName: null,
                    MiddleName: null,
                    LastName: null,
                    AcademicDegree: null,
                    AcademicTitle: null,
                    Post: null,
                    Workplace: null
                },
                showMessage: rootVm.showMessage
            },
            title: 'Авторы',
            closeAction: 'close',
            modal: true,
            autoHeight: true,
            width: 400,
            layout: 'fit',
            items: {
                xtype: 'form',
                bodyPadding: 10,
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                items: [{
                    fieldLabel: 'Фамилия',
                    xtype: 'textfield',
                    bind: '{LastName}'
                }, {
                    fieldLabel: 'Имя',
                    xtype: 'textfield',
                    bind: '{FirstName}'
                }, {
                    fieldLabel: 'Отчество',
                    xtype: 'textfield',
                    bind: '{MiddleName}'
                }, {
                    fieldLabel: 'Ученая степень',
                    xtype: 'textfield',
                    bind: '{AcademicDegree}'
                }, {
                    fieldLabel: 'Ученое звание',
                    xtype: 'textfield',
                    bind: '{AcademicTitle}'
                }, {
                    fieldLabel: 'Должность',
                    xtype: 'textfield',
                    bind: '{Post}'
                }, {
                    fieldLabel: 'Кафедра',
                    xtype: 'textfield',
                    bind: '{Workplace}'
                }],
                buttons: [{
                    xtype: 'button',
                    text: 'Добавить',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        Ext.Ajax.request({
                            url: '/WorkingPrograms/CreateAuthor/' + window.location.search,
                            jsonData: vm.getData(),
                            success: function (response) {
                                var author = JSON.parse(response.responseText);
                                selectHandler(author);
                                btn.up('window').close();
                            },
                            failure: function (xhr) {
                                vm.showMessage(xhr.responseText);
                            }
                        });
                    }
                }, {
                    xtype: 'button',
                    text: 'Отмена',
                    handler: function (btn) {
                        btn.up('window').close();
                    }
                }]
            }
        });
        return modal;
    }

    function createPeopleSelectorWindow(selectHandler, vm, selectorStoreName) {
        var modal = Ext.create('Ext.window.Window', {
            viewModel: vm,
            title: 'Добавление автора',
            closeAction: 'close',
            modal: true,
            height: 700,
            width: 828,
            layout: 'fit',
            listeners: {
                close: function () {
                    this.lookupViewModel().get(selectorStoreName).clearFilter();
                }
            },
            items: [{
                xtype: 'grid',
                loadMask: true,
                reference: 'authorsGrid',
                bind: {
                    store: '{' + selectorStoreName + '}'
                },
                tbar: [{
                    xtype: 'textfield',
                    fieldLabel: 'Поиск',
                    name: 'query',
                    listeners: {
                        change: function (textfield) {
                            var store = this.lookupViewModel().get(selectorStoreName);
                            store.clearFilter();
                            var searchText = textfield.getValue();
                            if (searchText !== "") {
                                store.filterBy(function (item) {
                                    var tester = new RegExp(Ext.String.escapeRegex(searchText), 'i');

                                    var fioMatch = tester.test(item.data.Fio);
                                    var degreeMatch = tester.test(item.data.Degree);
                                    var postMatch = tester.test(item.data.Post);
                                    var cathedraMatch = tester.test(item.data.Cathedra);

                                    return fioMatch || degreeMatch || postMatch || cathedraMatch;
                                });
                            }
                        }
                    },
                    width: 500
                }],
                columns: [{
                    header: 'ФИО',
                    dataIndex: 'Fio',
                    width: 250,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Ученая степень/ученое звание',
                    dataIndex: 'Degree',
                    width: 150,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Должность',
                    dataIndex: 'Post',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Кафедра',
                    dataIndex: 'Cathedra',
                    width: 200,
                    renderer: Urfu.renders.htmlEncodeWithToolTip
                }],
                bbar: ['->', {
                    width: 100,
                    text: 'Выбрать',
                    bind: {
                        disabled: '{!authorsGrid.selection}'
                    },
                    handler: function (btn) {
                        var selection = btn.up('grid').getSelection();
                        selectHandler(selection);
                        btn.up('window').close();
                    }
                }, {
                        width: 100,
                        text: 'Отмена',
                        handler: function (btn) {
                            btn.up('window').close();
                        }
                    }]
            }]
        });

        return modal;
    }
}