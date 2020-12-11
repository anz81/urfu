function uiInit(documentId, documentType, data, schema, emptyData, canEdit, user) {
    var isModule = documentType === 1 || documentType === 7;
    var isDiscipline = documentType === 2 || documentType === 8;
    var isGia = documentType === 3 || documentType === 9;
    var isPractices = documentType === 4 || documentType === 10;

    var genitiveName = isModule
        ? 'модуля'
        : isDiscipline
        ? 'дисциплины'
        : isGia
        ? 'государственной итоговой аттестации'
        : isPractices
        ? 'практик'
        : null;

    var instituteBlock = function (number) {
        return {
            name: 'Institute',
            contentReader: function (content) {
                return !content || !content.Id ? emptyData[this.name] : content;
            },

            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: number + ') Институт'
            }, {
                xtype: 'comboedit',
                valueField: 'Id',
                displayField: 'Name',
                bind: {
                    value: '{Institute.Id}',
                    selection: '{Institute}',
                    store: '{institutes}'
                }
            }]
        };
    };
    var directionsBlock = function (number) {
        return {
            name: 'Directions',
            listeners: {
                saved: function (b) {
                    var vm = b.lookupViewModel();
                    vm.get('profiles').reload();
                }
            },

            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: number + ') Направления [указывается наименование в соответствии с ОХОП]'
            }, {
                xtype: 'list',
                header: false,
                loadMask: true,
                bind: {
                    store: '{Directions}'
                },
                tbar: [{
                    xtype: 'button',
                    text: 'Выбрать',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = Ext.create('Ext.window.Window', {
                            viewModel: vm,
                            closeAction: 'close',
                            title: 'Направления',
                            modal: true,
                            height: 700,
                            width: 628,
                            layout: 'fit',
                            items: [{
                                xtype: 'grid',
                                loadMask: true,
                                selModel: 'rowmodel',
                                multiSelect: true,
                                allowDeselect: true,
                                reference: 'directionsGrid',
                                store: {
                                    autoLoad: true,
                                    proxy: {
                                        type: 'ajax',
                                        url: '/WorkingPrograms/GetDirections/' + window.location.search,
                                        reader: { type: 'json' },
                                        extraParams: {
                                            documentType: documentType,
                                            documentId: documentId
                                        }
                                    }
                                },
                                columns: [{
                                    header: 'Id',
                                    dataIndex: 'Id',
                                    width: 100,
                                    hidden: true,
                                    renderer: Ext.util.Format.htmlEncode
                                }, {
                                    header: 'Код',
                                    dataIndex: 'Code',
                                    width: 100,
                                    renderer: Ext.util.Format.htmlEncode
                                }, {
                                    header: 'Название',
                                    dataIndex: 'Title',
                                    width: 500,
                                    renderer: Ext.util.Format.htmlEncode
                                }],
                                bbar: ['->', {
                                    width: 100,
                                    text: 'Выбрать',
                                    bind: {
                                        disabled: '{!directionsGrid.selection}'
                                    },
                                    handler: function (btn) {
                                        var selection = btn.up('grid').getSelection();
                                        vm.get('Directions').add(selection);
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
                        modal.show();
                    }
                }],

                columns: [{
                    header: 'Код',
                    dataIndex: 'Code',
                    width: 100,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Название',
                    dataIndex: 'Title',
                    width: 500,
                    renderer: Ext.util.Format.htmlEncode
                }]
            }]
        };
    };
    
    var profilesBlock = function (number) {
        return {
            name: 'Profiles',
            layout: { type: 'vbox', align: 'stretch' },

            /*contentUpdater: function (content, vm) {
                this.down('list').getStore().setData(content);
            },*/

            items: [{
                xtype: 'label',
                text: number + ') Образовательная программа [указывается наименование в соответствии с титульным листом ОХОП]'
            }, {
                xtype: 'list',
                tbar: [{
                    xtype: 'container',
                    layout: { type: 'hbox' },
                    items: [{
                        xtype: 'comboedit',
                        width: 500,
                        valueField: 'Id',
                        displayField: 'DisplayName',
                        bind: {
                            store: '{profiles}'
                        }
                    }, {
                        xtype: 'button',
                        text: 'Добавить',
                        handler: function () {
                            var selection = this.up('list').down('combo').getSelection();
                            var vm = findRootVm(this);
                            vm.get('Profiles').add(selection);
                        }
                    }]
                }],
                bind: {
                    store: '{Profiles}'
                },
                columns: [{
                    text: 'Код ОП',
                    dataIndex: 'Code',
                    width: 150
                }, {
                    text: 'Название ОП',
                    dataIndex: 'Name',
                    width: 400
                }, {
                    xtype: 'widgetcolumn',
                    text: 'Траектории ОП',
                    tdCls: 'widget-column',
                    width: 150,
                    hidden: !canEdit,
                    widget: {
                        xtype: 'button',
                        text: 'Редактировать',
                        handler: function (w) {
                            var rec = w.$widgetRecord;
                            var vm = findRootVm(this);// this.lookupViewModel();
                            var win = Ext.create('Ext.window.Window', {
                                viewModel: {
                                    showMessage: vm.showMessage,
                                    confirmMessage: vm.confirmMessage,
                                    trajectoryName: null
                                },
                                width: 500,
                                title: 'Траектории ОП',
                                closeAction: 'destroy',
                                resizable: false,
                                autoHeight: true,
                                bodyPadding: 6,
                                modal: true,
                                items: [{
                                    xtype: 'list',
                                    tbar: [{
                                        xtype: 'container',
                                        layout: { type: 'hbox' },
                                        items: [{
                                            xtype: 'textfield',
                                            bind: '{trajectoryName}',
                                            width: 400
                                        }, {
                                            xtype: 'button',
                                            text: '+',
                                            disabled: true,
                                            bind: {
                                                disabled: '{!trajectoryName}'
                                            },
                                            handler: function () {
                                                var list = this.up('list');
                                                var vm = findRootVm(this);
                                                var trajectoryName = vm.get('trajectoryName');
                                                vm.set('trajectoryName', null);
                                                list.getStore().add({ Name: trajectoryName });
                                            }
                                        }]
                                    }],
                                    store: {
                                        data: rec.get('Trajectories').map(function (t) {
                                            return { Name: t }
                                        })
                                    },
                                    hideHeaders: true,
                                    columns: [{
                                        dataIndex: 'Name',
                                        width: 400
                                    }]
                                }],
                                buttons: [{
                                    text: 'ОК',
                                    width: 100,
                                    handler: function () {
                                        var trajectories = this.up('window').down('list').getStore().getData().items.map(function (i) {
                                            return i.get('Name');
                                        });
                                        rec.set('Trajectories', trajectories);
                                        this.up('window').close();
                                    }
                                }, {
                                    text: 'Отмена',
                                    width: 100,
                                    handler: function () {
                                        this.up('window').close();
                                    }
                                }]
                            });

                            win.show();
                            console.log('click');
                        }
                    }
                }],
                plugins: [{
                    ptype: 'overriderowexpander',
                    pluginId: 'overriderowexpanderId',
                    bind: {
                        viewModel: '{record}'
                    },
                    rowBodyTpl: new Ext.XTemplate("<div><b>Траектории образовательной программы: </b></div><tpl for=\'Trajectories\'><tpl><div>{.}</div></tpl></tpl>")
                }]
            }]
        };
    };
    var authorsBlock = function (number) {
        return {
            name: 'Authors',
            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: number + ') Рабочая программа ' + genitiveName + ' составлена авторами'
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
                    header: 'Кафедра',
                    dataIndex: 'Cathedra',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
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
                            vm.set('EduProgramHead', {
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
                            vm.set('EduProgramHead',
                                {
                                    ShortName: people.ShortName,
                                    AuthorId: people.AuthorId,
                                    TeacherId: null
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
    var councilBlock = function(number) {
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
                        //rawValue: '{Council.ProtocolDate}',
                        value: '{Council.ProtocolDate}',
                    },
                    width: 270,
                    submitFormat: 'd.m.Y',
                    //rawFormat: 'd.m.Y',
                    format: 'd.m.Y'                    
                }, {
                    xtype: 'label',
                    margin: '0 0 0 8',
                    text: 'г.'
                }]
            }]
        };
    }
    var directionBlock = function(number) {
        return {
            name: 'Direction',
                header: false,

                    layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: number + ') Согласовано:'
            }, {
                xtype: 'label',
                text: 'Дирекция образовательных программ [И.О. Фамилия]'
            }, {
                xtype: 'panel',
                header: false,
                tbar: [{
                    xtype: 'button',
                    text: 'Выбрать',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleSelectorWindow(function (selection) {
                            vm.set('Direction', {
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
                            vm.set('Direction', {
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
                    bind: '{Direction.ShortName}'
                }, {
                    xtype: 'button',
                    text: 'X',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        if (vm.confirmMessage('Вы действительно желаете очистить выбранное значение?')) {
                            vm.set('Direction', {
                                ShortName: null,
                                AuthorId: null,
                                TeacherId: null
                            });
                        }
                    }
                }]
            }]
        };
    }
    var requisiteOrdersBlock = function(number) {
        return {
            name: 'RequisitesOrders',
                dataReader: function (block) {
                    var vm = findRootVm(block);
                    var items = vm.get('RequisitesOrders').getData().items.map(function (r) { return r.getData() });
                    return { RequisitesOrders: items };
                },

            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: number + ') Реквизиты приказа Минобрнауки РФ об утверждении ФГОС ВО'
            }, {
                xtype: 'grid',
                header: false,
                loadMask: true,
                bind: {
                    store: '{RequisitesOrders}'
                },

                tbar: [{
                    xtype: 'button',
                    text: 'Выбрать',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = Ext.create('Ext.window.Window', {
                            viewModel: vm,
                            closeAction: 'close',
                            title: 'Реквизиты приказов',
                            modal: true,
                            height: 400,
                            width: 528,
                            layout: 'fit',
                            items: [{
                                xtype: 'grid',
                                loadMask: true,
                                reference: 'requisitesGrid',
                                store: {
                                    autoLoad: true,
                                    proxy: {
                                        type: 'ajax',
                                        url: '/WorkingPrograms/GetRequisitesOrders/' + window.location.search,
                                        reader: { type: 'json' },
                                        extraParams: {
                                            documentType: documentType,
                                            documentId: documentId
                                        }
                                    }
                                },
                                columns: [{
                                    header: 'Id',
                                    dataIndex: 'Id',
                                    width: 100,
                                    hidden: true,
                                    renderer: Ext.util.Format.htmlEncode
                                }, {
                                    header: 'Код направления',
                                    dataIndex: 'DirectionCode',
                                    width: 100,
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
                                }],
                                bbar: ['->', {
                                    width: 100,
                                    text: 'Выбрать',
                                    bind: {
                                        disabled: '{!requisitesGrid.selection}'
                                    },
                                    handler: function (btn) {
                                        var selection = btn.up('grid').getSelection();
                                        findRootVm(btn).get('RequisitesOrders').add(selection);
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
                        modal.show();
                    }
                }, {
                    xtype: 'button',
                    text: 'Добавить',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createRequisiteOrderWindow(function (r) {
                            //vm.get('requisitesOrders').add(r);
                            vm.get('RequisitesOrders').add(r);
                        }, vm.get('savedDirections'));
                        modal.show();
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
                }, {
                    xtype: 'actioncolumn',
                    resizable: false,
                    sortable: false,
                    width: 60,
                    items: [{
                        icon: '/Content/Images/remove.png',
                        tooltip: 'Удалить',
                        handler: function (grid, rowIndex, colIndex, item, e, record) {
                            var vm = findRootVm(grid);
                            if (vm.confirmMessage('Вы действительно желаете удалить запись?')) {
                                grid.getStore().remove(record);
                            }
                        }
                    }]
                }]
            }]
        };
    }
    var headBlock = function(number) {
        return {
            name: 'Head',
                layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: number + ') Руководитель модуля [И.О. Фамилия]'
            }, {
                xtype: 'panel',
                header: false,
                tbar: [{
                    xtype: 'button',
                    text: 'Выбрать',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        var modal = createPeopleSelectorWindow(function (selection) {
                            vm.set('Head', {
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
                            vm.set('Head', {
                                ShortName: people.ShortName,
                                AuthorId: people.AuthorId,
                                TeacherId: null
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
                            bindTo: '{Head.ShortName}',
                            deep: true
                        }
                    }
                }, {
                    xtype: 'button',
                    text: 'X',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        if (vm.confirmMessage('Вы действительно желаете очистить выбранное значение?')) {
                            vm.set('Head', {
                                ShortName: null,
                                AuthorId: null,
                                TeacherId: null
                            });
                        }
                    }
                }]
            }]
        };
    }
    
    var blocks;
    if (documentType === 1) {
        blocks = [
            instituteBlock(1),
            directionsBlock(2),
            profilesBlock(3),
            authorsBlock(4),
            headBlock(5),
            eduProgramHeadBlock(6),
            councilBlock(7),
            directionBlock(8),
            requisiteOrdersBlock(9)
        ];
    }
    else if (documentType === 2) {
        blocks = [
            instituteBlock(1),
            directionsBlock(2),
            profilesBlock(3),
            authorsBlock(4),
            headBlock(5),
            councilBlock(6),
            directionBlock(7),
            requisiteOrdersBlock(8)
        ];
    }
    else {
        blocks = [
            instituteBlock(1),
            directionsBlock(2),
            profilesBlock(3),
            authorsBlock(4),
            eduProgramHeadBlock(5),
            councilBlock(6),
            directionBlock(7),
            requisiteOrdersBlock(8)
        ];
    }
    
    return {
        viewModel: {
            stores: {
                institutes: {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: '/WorkingPrograms/GetInstitutes/' + window.location.search,
                        reader: { type: 'json' },
                        extraParams: {
                            documentType: documentType,
                            documentId: documentId
                        }
                    }
                },
                Directions: {
                    data: data.Directions,
                    proxy: {
                        type: 'memory'
                    }
                },
                savedDirections: {
                    autoLoad: false,
                    proxy: {
                        type: 'ajax',
                        url: '/WorkingPrograms/GetDocumentDirections/' + window.location.search,
                        reader: { type: 'json' },
                        extraParams: {
                            documentType: documentType,
                            documentId: documentId
                        }
                    }
                },
                Profiles: {
                    data: data.Profiles
                },
                profiles: {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: '/WorkingPrograms/GetProfiles/' + window.location.search,
                        reader: { type: 'json' },
                        extraParams: {
                            documentType: documentType,
                            documentId: documentId
                        }
                    }
                },
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
    }

    function createRequisiteOrderWindow(selectHandler, directionsStore) {
        directionsStore.load();
        var modal = Ext.create('Ext.window.Window', {
            viewModel: {
                data: {
                    Number: null,
                    Date: null,
                    DirectionId: null                    
                }
            },
            title: 'Добавить реквизиты приказа',
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
                    fieldLabel: 'Номер приказа',
                    xtype: 'textfield',
                    bind: '{Number}',
                    allowBlank: false
                }, {
                    fieldLabel: 'Дата приказа',
                    xtype: 'datefield',
                    bind: '{Date}',
                    allowBlank: false
                }, {
                    fieldLabel: 'Направление',
                    xtype: 'combobox',
                    store: directionsStore,
                    bind: {
                        value: '{DirectionId}'
                    },
                    displayField: 'DisplayName',
                    valueField: 'Id',
                    queryMode: 'local',
                    allowBlank: false
                }],
                buttons: [{
                    xtype: 'button',
                    text: 'Добавить',
                    formBind: true,
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        Ext.Ajax.request({
                            url: '/WorkingPrograms/CreateRequisiteOrder/' + window.location.search,
                            jsonData: {
                                Order: vm.get('Number'),
                                Date: Ext.Date.format(vm.get('Date'), 'd.m.Y'),
                                DirectionId: vm.get('DirectionId')
                            },
                            success: function (response) {
                                var requisiteOrder = JSON.parse(response.responseText);
                                selectHandler(requisiteOrder);
                                btn.up('window').close();
                            },
                            failure: function (xhr) {
                                alert(xhr.responseText);
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
                },{
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
                close: function() {
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
                    renderer: Ext.util.Format.htmlEncode
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