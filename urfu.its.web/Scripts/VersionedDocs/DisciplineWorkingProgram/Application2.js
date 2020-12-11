function uiInit(documentId, documentType, data, schema, emptyData) {
    var boldStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };

    return {
        viewModel: {
            stores: {
                
            }
        },

        items: [{
            xtype: 'container',
            html: '<b>Для проведения промежуточной аттестации используется…</b> [выбрать одну из систем тестирования: ФЭПО, Интернет-тренажеры, СМУДС УрФУ; либо две, либо все три. Здесь задается возможность использования соответствующих материалов.]'
        }, {
            name: 'SmudsTests',
            contentReader: function(content, vm) {
                var items = this.down('list').getStore().getData().items.map(function (i) { return i.getData(); });
                var result = vm.get(this.name);
                result.Tests = items;
                return result;
            },
            items: {
                xtype: 'list',
                cls: 'table-all-borders',
                store: {
                    data: Ext.clone(data.SmudsTests.Tests)
                },
                tbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    items: [{
                        xtype: 'container',
                        html: '[Структура тестовых материалов при использовании СМУДС УрФУ]',
                        style: italicStyle
                    }, {
                        xtype: 'form',
                        layout: { type: 'table', columns: 3 },
                        cls: 'table-cell-padding-normal',
                        defaults: {
                            margin: 0
                        },
                        items: [{
                            xtype: 'label',
                            text: 'Код раздела',
                            style: boldStyle,
                            width: 200
                        }, {
                            html: '[указать код раздела в соответствии со структурой банка СМУДС]'
                        }, {
                            name: 'SectionCode',
                            xtype: 'textareafield',
                            height: 100,
                            width: 300
                        }, {
                            xtype: 'label',
                            text: 'Раздел дисциплины',
                            style: boldStyle
                        }, {
                            html: '[указать наименование раздела в соответствии со структурой банка СМУДС]'
                        }, {
                            name: 'SectionName',
                            xtype: 'textareafield',
                            height: 100,
                            width: 300
                        }, {
                            xtype: 'label',
                            text: 'Код темы',
                            style: boldStyle
                        }, {
                            html: '[указать код темы в соответствии со структурой банка СМУДС]'
                        }, {
                            name: 'ThemeCode',
                            xtype: 'textareafield',
                            height: 100,
                            width: 300
                        }, {
                            xtype: 'label',
                            text: 'Тема',
                            style: boldStyle
                        }, {
                            html: '[указать наименование  темы в соответствии со структурой банка СМУДС]'
                        }, {
                            name: 'Theme',
                            xtype: 'textareafield',
                            height: 100,
                            width: 300
                        }, {
                            xtype: 'label',
                            text: 'Индекс вариации темы',
                            style: boldStyle
                        }, {
                            html: '[указать индекс вариации и темы в соответствии со структурой банка СМУДС]'
                        }, {
                            name: 'VariationIndex',
                            xtype: 'textareafield',
                            height: 100,
                            width: 300
                        }, {
                            xtype: 'label',
                            text: 'Наименование вариации',
                            style: boldStyle
                        }, {
                            html: '[указать наименование вариации в соответствии со структурой банка СМУДС]'
                        }, {
                            name: 'VariationName',
                            xtype: 'textareafield',
                            height: 100,
                            width: 300
                        }, {
                            xtype: 'label',
                            text: 'Число заданий в тесте',
                            style: boldStyle
                        }, {
                            html: '[указать число заданий в тесте]'
                        }, {
                            xtype: 'numberfield',
                            name: 'TaskCount',
                            width: 300,
                            allowDecimals: false,
                            allowNegative: false                            
                        }, {
                            colspan: 3,
                            items: {
                                xtype: 'button',
                                text: 'Добавить',
                                handler: function() {
                                    var form = this.up('form').getForm();
                                    var formData = form.getFieldValues();
                                    this.up('grid').getStore().add(formData);
                                    form.reset();
                                }
                            }
                        }]
                    }]                    
                }],
                columns: [{
                    text: 'Код раздела',
                    dataIndex: 'SectionCode',
                    tdCls: 'multiline-column',
                    width: 150
                }, {
                    text: 'Раздел дисциплины',
                    dataIndex: 'SectionName',
                    tdCls: 'multiline-column',
                    width: 190
                }, {
                    text: 'Код темы',
                    dataIndex: 'ThemeCode',
                    tdCls: 'multiline-column',
                    width: 150
                }, {
                    text: 'Тема',
                    dataIndex: 'Theme',
                    tdCls: 'multiline-column',
                    width: 150
                }, {
                    text: 'Индекс вариации темы',
                    dataIndex: 'VariationIndex',
                    tdCls: 'multiline-column',
                    width: 150
                }, {
                    text: 'Наименование вариации темы',
                    dataIndex: 'VariationName',
                    tdCls: 'multiline-column',
                    width: 150
                }, {
                    text: 'Число заданий в тесте',
                    dataIndex: 'TaskCount',
                    width: 150
                }],
                bbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    defaults: {
                        margin: '0 0 10 0'
                    },
                    items: [{
                        layout: { type: 'hbox', align: 'center' },
                        items: [{
                            xtype: 'textfield',
                            fieldLabel: 'Номер спецификации',
                            labelWidth: 170,
                            width: 340,
                            bind: '{SmudsTests.SpecificationNumber}'
                        }, {
                            margin: '0 0 0 10',
                            html: '(указать номер спецификации, сохраненной в портале СМУДС).'
                        }]
                    }, {
                        layout: { type: 'hbox', align: 'center' },
                        items: [{
                            xtype: 'numberfield',
                            fieldLabel: 'Время тестирования',
                            labelWidth: 170,
                            width: 340,
                            bind: '{SmudsTests.TestTime}',
                            allowDecimals: false,
                            allowNegative: false
                        }, {
                            margin: '0 0 0 10',
                            html: 'мин.'
                        }]
                    }, {
                        layout: { type: 'hbox', align: 'center' },
                        items: [{
                            xtype: 'numberfield',
                            fieldLabel: 'Число заданий в тесте',
                            labelWidth: 170,
                            width: 340,
                            bind: '{SmudsTests.TaskCount}',
                            allowDecimals: false,
                            allowNegative: false
                        }, {
                            margin: '0 0 0 10',
                            html: 'шт.'
                        }]
                    }, {
                        html: 'Выбор заданий – случайным образом из соответствующего раздела, без повторения.'
                    }]
                }]
            }            
        }, { // ФЕПО ============================================================================================
            name: 'FepoTests',
            contentReader: function (content, vm) {
                var items = this.query('list').map(function(l) {
                    return l.getStore().getData().items.map(function(i) { return i.getData(); });
                });
                var result = vm.get(this.name);
                result.Block1 = items[0];
                result.Block2 = items[1];
                result.Block3 = items[2];
                return result;
            },
            items: [{
                xtype: 'panel',
                items: [{
                    xtype: 'container',
                    html: '[Структура тестовых материалов при использовании ФЭПО]',
                    style: italicStyle
                }, {
                    xtype: 'list',
                    cls: 'table-all-borders',
                    store: {
                        data: Ext.clone(data.FepoTests.Block1)
                    },
                    tbar: [{
                        xtype: 'container',
                        layout: { type: 'vbox', align: 'stretch' },
                        items: [{
                            xtype: 'container',
                            html: 'Блок 1. Темы',
                            style: boldStyle
                        }, {
                            xtype: 'form',
                            layout: { type: 'table', columns: 3 },
                            cls: 'table-cell-padding-normal',
                            defaults: {
                                margin: 0
                            },
                            items: [{
                                xtype: 'label',
                                text: 'Код структурной единицы',
                                style: boldStyle,
                                width: 200
                            }, {
                                html: '[Указать код темы в соответствии с кодификатором ФЭПО из файла «Дисциплины ФЭПО.xls»]'
                            }, {
                                name: 'Code',
                                xtype: 'textareafield',
                                height: 100,
                                width: 300
                            }, {
                                xtype: 'label',
                                text: 'Наименование структурной единицы',
                                style: boldStyle
                            }, {
                                html: '[Указать наименование темы в соответствии с кодификатором ФЭПО из файла «Дисциплины ФЭПО.xls»]'
                            }, {
                                name: 'Name',
                                xtype: 'textareafield',
                                height: 100,
                                width: 300
                            }, {
                                xtype: 'label',
                                text: 'Число заданий в тесте',
                                style: boldStyle
                            }, {
                                html: '[указать число заданий в тесте]'
                            }, {
                                xtype: 'numberfield',
                                name: 'TaskCount',
                                width: 300,
                                allowDecimals: false,
                                allowNegative: false
                            }, {
                                xtype: 'label',
                                text: 'Число баллов',
                                style: boldStyle
                            }, {
                                html: ''
                            }, {
                                xtype: 'numberfield',
                                name: 'PointCount',
                                width: 300,
                                allowDecimals: false,
                                allowNegative: false
                            }, {
                                colspan: 3,
                                items: {
                                    xtype: 'button',
                                    text: 'Добавить',
                                    handler: function () {
                                        var form = this.up('form').getForm();
                                        var formData = form.getFieldValues();
                                        this.up('grid').getStore().add(formData);
                                        form.reset();
                                    }
                                }
                            }]
                        }]
                    }],
                    columns: [{
                        text: 'Код структурной единицы',
                        dataIndex: 'Code',
                        tdCls: 'multiline-column',
                        width: 250
                    }, {
                        text: 'Наименование структурной единицы',
                        dataIndex: 'Name',
                        tdCls: 'multiline-column',
                        width: 300
                    }, {
                        text: 'Число заданий в тесте',
                        dataIndex: 'TaskCount',
                        width: 200
                    }, {
                        text: 'Число баллов',
                        dataIndex: 'PointCount',
                        width: 200
                    }]
                }, {
                    xtype: 'list',
                    cls: 'table-all-borders',
                    store: {
                        data: Ext.clone(data.FepoTests.Block2)
                    },
                    tbar: [{
                        xtype: 'container',
                        layout: { type: 'vbox', align: 'stretch' },
                        items: [{
                            xtype: 'container',
                            html: 'Блок 2. Модули',
                            style: boldStyle
                        }, {
                            xtype: 'form',
                            layout: { type: 'table', columns: 3 },
                            cls: 'table-cell-padding-normal',
                            defaults: {
                                margin: 0
                            },
                            items: [{
                                xtype: 'label',
                                text: 'Код структурной единицы',
                                style: boldStyle,
                                width: 200
                            }, {
                                html: '[Указать код темы в соответствии с кодификатором ФЭПО из файла «Дисциплины ФЭПО.xls»]'
                            }, {
                                name: 'Code',
                                xtype: 'textareafield',
                                height: 100,
                                width: 300
                            }, {
                                xtype: 'label',
                                text: 'Наименование структурной единицы',
                                style: boldStyle
                            }, {
                                html: '[Указать наименование темы в соответствии с кодификатором ФЭПО из файла «Дисциплины ФЭПО.xls»]'
                            }, {
                                name: 'Name',
                                xtype: 'textareafield',
                                height: 100,
                                width: 300
                            }, {
                                xtype: 'label',
                                text: 'Число заданий в тесте',
                                style: boldStyle
                            }, {
                                html: '[указать число заданий в тесте]'
                            }, {
                                xtype: 'numberfield',
                                name: 'TaskCount',
                                width: 300,
                                allowDecimals: false,
                                allowNegative: false
                            }, {
                                xtype: 'label',
                                text: 'Число баллов',
                                style: boldStyle
                            }, {
                                html: ''
                            }, {
                                xtype: 'numberfield',
                                name: 'PointCount',
                                width: 300,
                                allowDecimals: false,
                                allowNegative: false
                            }, {
                                colspan: 3,
                                items: {
                                    xtype: 'button',
                                    text: 'Добавить',
                                    handler: function () {
                                        var form = this.up('form').getForm();
                                        var formData = form.getFieldValues();
                                        this.up('grid').getStore().add(formData);
                                        form.reset();
                                    }
                                }
                            }]
                        }]
                    }],
                    columns: [{
                        text: 'Код структурной единицы',
                        dataIndex: 'Code',
                        tdCls: 'multiline-column',
                        width: 250
                    }, {
                        text: 'Наименование структурной единицы',
                        dataIndex: 'Name',
                        tdCls: 'multiline-column',
                        width: 300
                    }, {
                        text: 'Число заданий в тесте',
                        dataIndex: 'TaskCount',
                        width: 200
                    }, {
                        text: 'Число баллов',
                        dataIndex: 'PointCount',
                        width: 200
                    }]
                }, {
                    xtype: 'list',
                    cls: 'table-all-borders',
                    store: {
                        data: Ext.clone(data.FepoTests.Block3)
                    },
                    tbar: [{
                        xtype: 'container',
                        layout: { type: 'vbox', align: 'stretch' },
                        items: [{
                            xtype: 'container',
                            html: 'Блок 3. Кейс-задания',
                            style: boldStyle
                        }, {
                            xtype: 'form',
                            layout: { type: 'table', columns: 3 },
                            cls: 'table-cell-padding-normal',
                            defaults: {
                                margin: 0
                            },
                            items: [{
                                xtype: 'label',
                                text: 'Число заданий в тесте',
                                style: boldStyle
                            }, {
                                html: '[указать число заданий в тесте]'
                            }, {
                                xtype: 'numberfield',
                                name: 'TaskCount',
                                width: 300,
                                allowDecimals: false,
                                allowNegative: false
                            }, {
                                xtype: 'label',
                                text: 'Число баллов',
                                style: boldStyle
                            }, {
                                html: ''
                            }, {
                                xtype: 'numberfield',
                                name: 'PointCount',
                                width: 300,
                                allowDecimals: false,
                                allowNegative: false
                            }, {
                                colspan: 3,
                                items: {
                                    xtype: 'button',
                                    text: 'Добавить',
                                    handler: function () {
                                        var form = this.up('form').getForm();
                                        var formData = form.getFieldValues();
                                        this.up('grid').getStore().setData([formData]);
                                        form.reset();
                                    }
                                }
                            }]
                        }]
                    }],
                    columns: [{
                        text: 'Число заданий в тесте',
                        dataIndex: 'TaskCount',
                        width: 250
                    }, {
                        text: 'Число баллов',
                        dataIndex: 'PointCount',
                        width: 250
                    }]                    
                }],
                bbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    defaults: {
                        margin: '0 0 10 0'
                    },
                    items: [{
                        layout: { type: 'hbox', align: 'center' },
                        items: [{
                            xtype: 'numberfield',
                            fieldLabel: 'Время тестирования',
                            labelWidth: 170,
                            width: 340,
                            bind: '{FepoTests.TestTime}',
                            allowDecimals: false,
                            allowNegative: false
                        }, {
                            margin: '0 0 0 10',
                            html: 'мин.'
                        }]
                    }, {
                        layout: { type: 'hbox', align: 'center' },
                        items: [{
                            xtype: 'numberfield',
                            fieldLabel: 'Число заданий в тесте',
                            labelWidth: 170,
                            width: 340,
                            bind: '{FepoTests.TaskCount}',
                            allowDecimals: false,
                            allowNegative: false
                        }, {
                            margin: '0 0 0 10',
                            html: 'шт.'
                        }]
                    }]
                }]
            }]
        }, { // Интернет-тренажеры ================================================================================
            name: 'InternetTrainerTests',
            contentReader: function (content, vm) {
                var items = this.query('list').map(function (l) {
                    return l.getStore().getData().items.map(function (i) { return i.getData(); });
                });
                var result = vm.get(this.name);
                result.Tests = items[0];                
                result.KeysTasks = items[1];                
                return result;
            },
            items: [{
                xtype: 'panel',
                items: [{
                    xtype: 'container',
                    html: '[Структура тестовых материалов при использовании ФЭПО]',
                    style: italicStyle
                }, {
                    xtype: 'list',
                    cls: 'table-all-borders',
                    store: {
                        data: Ext.clone(data.InternetTrainerTests.Tests)
                    },
                    tbar: [{
                        xtype: 'container',
                        layout: { type: 'vbox', align: 'stretch' },
                        items: [{
                            xtype: 'container',
                            html: 'Темы',
                            style: boldStyle
                        }, {
                            xtype: 'form',
                            layout: { type: 'table', columns: 3 },
                            cls: 'table-cell-padding-normal',
                            defaults: {
                                margin: 0
                            },
                            items: [{
                                xtype: 'label',
                                text: 'Код раздела',
                                style: boldStyle,
                                width: 200
                            }, {
                                html: '[указать код раздела в соответствии с кодификатором ФЭПО из файла «Дисциплины ФЭПО.xls»]'
                            }, {
                                name: 'SectionCode',
                                xtype: 'textareafield',
                                height: 100,
                                width: 300
                            }, {
                                xtype: 'label',
                                text: 'Раздел дисциплины',
                                style: boldStyle
                            }, {
                                html: '[указать наименование  раздела в соответствии с кодификатором ФЭПО из файла «Дисциплины ФЭПО.xls»]'
                            }, {
                                name: 'SectionName',
                                xtype: 'textareafield',
                                height: 100,
                                width: 300
                            }, {
                                xtype: 'label',
                                text: 'Код темы',
                                style: boldStyle
                            }, {
                                html: '[указать код темы в соответствии с кодификатором ФЭПО из файла «Дисциплины ФЭПО.xls»]'
                            }, {
                                name: 'ThemeCode',
                                xtype: 'textareafield',
                                height: 100,
                                width: 300
                            }, {
                                xtype: 'label',
                                text: 'Тема',
                                style: boldStyle
                            }, {
                                html: '[указать наименование  темы в соответствии с кодификатором ФЭПО из файла «Дисциплины ФЭПО.xls»]'
                            }, {
                                name: 'Theme',
                                xtype: 'textareafield',
                                height: 100,
                                width: 300
                            }, {
                                xtype: 'label',
                                text: 'Число заданий в тесте',
                                style: boldStyle
                            }, {
                                html: '[указать число заданий в тесте]'
                            }, {
                                xtype: 'numberfield',
                                name: 'TaskCount',
                                width: 300,
                                allowDecimals: false,
                                allowNegative: false
                            }, {
                                colspan: 3,
                                items: {
                                    xtype: 'button',
                                    text: 'Добавить',
                                    handler: function () {
                                        var form = this.up('form').getForm();
                                        var formData = form.getFieldValues();
                                        this.up('grid').getStore().add(formData);
                                        form.reset();
                                    }
                                }
                            }]
                        }]
                    }],
                    columns: [{
                        text: 'Код раздела',
                        dataIndex: 'SectionCode',
                        tdCls: 'multiline-column',
                        width: 200
                    }, {
                        text: 'Раздел дисциплины',
                        dataIndex: 'SectionName',
                        tdCls: 'multiline-column',
                        width: 300
                    }, {
                        text: 'Код темы',
                        dataIndex: 'ThemeCode',
                        tdCls: 'multiline-column',
                        width: 150
                    }, {
                        text: 'Тема',
                        dataIndex: 'Theme',
                        tdCls: 'multiline-column',
                        width: 150
                    }, {
                        text: 'Число заданий в тесте',
                        dataIndex: 'TaskCount',
                        width: 150
                    }]
                }, {
                    xtype: 'list',
                    cls: 'table-all-borders',
                    store: {
                        data: Ext.clone(data.InternetTrainerTests.KeysTasks)
                    },
                    tbar: [{
                        xtype: 'container',
                        layout: { type: 'vbox', align: 'stretch' },
                        items: [{
                            xtype: 'container',
                            html: 'Кейс-задания',
                            style: boldStyle
                        }, {
                            xtype: 'form',
                            layout: { type: 'table', columns: 3 },
                            cls: 'table-cell-padding-normal',
                            defaults: {
                                margin: 0
                            },
                            items: [{
                                xtype: 'label',
                                text: 'Число заданий в тесте',
                                style: boldStyle
                            }, {
                                html: '[указать число заданий в тесте]'
                            }, {
                                xtype: 'numberfield',
                                name: 'TaskCount',
                                width: 300,
                                allowDecimals: false,
                                allowNegative: false
                            }, {
                                colspan: 3,
                                items: {
                                    xtype: 'button',
                                    text: 'Добавить',
                                    handler: function () {
                                        var form = this.up('form').getForm();
                                        var formData = form.getFieldValues();
                                        this.up('grid').getStore().setData([formData]);
                                        form.reset();
                                    }
                                }
                            }]
                        }]
                    }],
                    columns: [{
                        text: 'Число заданий в тесте',
                        dataIndex: 'TaskCount',
                        width: 300
                    }]
                }],
                bbar: [{
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },
                    defaults: {
                        margin: '0 0 10 0'
                    },
                    items: [{
                        layout: { type: 'hbox', align: 'center' },
                        items: [{
                            xtype: 'numberfield',
                            fieldLabel: 'Время тестирования',
                            labelWidth: 170,
                            width: 340,
                            bind: '{InternetTrainerTests.TestTime}',
                            allowDecimals: false,
                            allowNegative: false
                        }, {
                            margin: '0 0 0 10',
                            html: 'мин.'
                        }]
                    }, {
                        layout: { type: 'hbox', align: 'center' },
                        items: [{
                            xtype: 'numberfield',
                            fieldLabel: 'Число заданий в тесте',
                            labelWidth: 170,
                            width: 340,
                            bind: '{InternetTrainerTests.TaskCount}',
                            allowDecimals: false,
                            allowNegative: false
                        }, {
                            margin: '0 0 0 10',
                            html: 'шт.'
                        }]
                    }]
                }]
            }]
        }, {
            xtype: 'container',
            html: '<b>Если дисциплины нет на</b> <i>ФЭПО, Интернет-тренажерах, СМУДС УрФУ, то пишем следующий текст:</i>'
        }, {
            xtype: 'container',
            html: '<i>Дисциплина и ее аналоги, по которым возможно тестирование, отсутствуют на сайте ФЭПО http://fepo.i-exam.ru.</i>'
        }, {
            xtype: 'container',
            html: '<i>Дисциплина и ее аналоги, по которым возможно тестирование, отсутствуют на сайте Интернет-тренажеры http://training.i-exam.ru.</i>'
        }, {
            xtype: 'container',
            html: '<i>Дисциплина и ее аналоги, по которым возможно тестирование, отсутствуют на портале СМУДС УрФУ.</i>'
        }, {
            xtype: 'container',
            html: '<i>В связи с отсутствием Дисциплины и ее аналогов, по которым возможно тестирование, на сайтах ФЭПО, Интернет-тренажеры и портале СМУДС УрФУ, тестирование в рамках НТК не проводится.</i>'
        }, {
            name: 'TestsComments',
            items: {
//                bind: {
//                    viewModel: '{TestsComments}'
//                },
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [{
                    xtype: 'checkboxfield',
                    labelWidth: 250,
                    fieldLabel: 'Использовать стандартный текст',
                    bind: '{TestsComments.UseStandardText}'
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    bind: {
                        disabled: '{TestsComments.UseStandardText}',
                        value: '{TestsComments.CustomText}'
                    }
                }]
            }
        }]
    }
}