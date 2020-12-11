function uiInit(documentId, documentType, data, schema, emptyData, canEdit, user, additionalData) {
    var captionStyle = { fontWeight: 'bold' };
    var subcaptionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };

    /*function debounce(func, delay) {
        var inDebounce;
        return function () {
            var context = this;
            var args = arguments;
            clearTimeout(inDebounce);
            inDebounce = setTimeout(function () { func.apply(context, args) }, delay);
        }
    }

    var setSearchDebounce = debounce(function (vm, value, store) {
        store.getProxy().extraParams.search = value;
        store.load();
    }, 1000);*/

    var literatureUrl = additionalData.LiteratureServiceUrl;
    var literatureError = additionalData.LiteratureServiceError;

    function pasteFromClipboard(evt) {
        var pastedText = "";
        if (Ext.isIE) {
            pastedText = window.clipboardData.getData('Text');
        }
        else if (Ext.isGecko) {
            pastedText = evt.browserEvent.clipboardData.getData('text/plain');;
        }
        else if (Ext.isOpera) {
            pastedText = evt.browserEvent.clipboardData.getData('text/plain');
        }
        else if (Ext.isWebKit) {
            //WebKit based browsers. i.e: Chrome, Safari
            pastedText = evt.browserEvent.clipboardData.getData('text/plain');
        }
        evt.stopEvent();
        if (pastedText.indexOf('#') == 0) {
            pastedText = pastedText.substr(1, pastedText.length);
            //this.setValue(pastedText);
        }

        return pastedText;
    }

    function parseLiterature(event) {
        var literatureData = pasteFromClipboard(event);
        var literature = JSON.parse(literatureData);
        if (literature instanceof Array) {
            if (literature.length === 1)
                literature = literature[0];
            else
                return null;
        }

        if (!literature ||
            !literature.hasOwnProperty('recordid') ||
            !literature.hasOwnProperty('recorddata') ||
            !literature.hasOwnProperty('bookscount') ||
            !literature.hasOwnProperty('barcode'))
            return null;

        return literature;
    }

    return {
        viewModel: {
            stores: {
                software: {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: '/DisciplineWorkingProgram/GetSoftware',
                        reader: { type: 'json' }
                    }                
                },
                SystemOrOffice: {
                    data: data.Software.SystemOrOffice
                },
                Application: {
                    data: data.Software.Application
                }
            }
        },

        items: [{
            name: 'Literature',
            contentReader: function (content, vm) {
                var clone = Ext.clone(content);
                clone.MainLiterature = selectStoreItemsData(this.down('#mainLiterature').getStore());
                clone.AdditionalLiterature = selectStoreItemsData(this.down('#additionalLiterature').getStore());
                return clone;
            },
            items: [{
                xtype: 'label',
                margin: '15 0 0 0',
                text: '9.1. Рекомендуемая литература',
                style: captionStyle
            }, {
                xtype: 'panel',
                height: 600,
                layout: 'fit',
                items: [{
                    xtype: 'label',
                    hidden: !literatureError,
                    text: 'При обращении к сервису литературы возникла ошибка: ' + literatureError,
                    style: {
                        color: 'salmon'
                    }
                }, {
                    xtype: "component",                    
                    hidden: literatureError,
                    style: 'width: 100%; height: 600; frameborder=0',
                    autoEl: {
                        tag: "iframe",
                        src: literatureUrl
                    },
                    listeners: {
                        load: {
                            element: 'el',
                            fn: function () {
                                this.parent().unmask();                                
                            }
                        },
                        render: function () {
                            this.up('panel').body.mask('Загрузка литературы...');
                        }
                    }
                }]
            }, {
                xtype: 'label',
                text: '9.1.1. Основная литература',
                style: captionStyle
            }, {
                html: '[список с обязательным указанием наименований из ЭБС]',
                style: italicStyle
            }, {
                html: '[заполняется с учетом наличия печатных изданий в ЗНБ УрФУ]',
                style: italicStyle
            }, {
                xtype: 'textfield',
                fieldLabel: '<b>Поставьте курсор в поле и нажмите Ctrl+V, либо нажмите правой кнопкой мышки по полю и выберите пункт "Вставить"</b>',
                emptyText: 'Вставлять сюда',
                labelWidth: 480,
                height: 50,
                listeners: {
                    change: function(field) {
                        field.reset();
                    },
                    paste: {
                        element: 'inputEl',
                        fn: function (event, inputEl) {
                            try {
                                var literature = parseLiterature(event);
                                if (literature != null)
                                    this.component.up('block').down('#mainLiterature').getStore().add(literature);
                                else
                                    throw new Error('Не удалось распарсить данные о литературе.');
                            } catch (ex) {
                                console.error(ex.message);
                                alert('ошибка формата данных');
                            }
                            event.stopPropagation();
                            event.preventDefault();
                            return false;
                        }
                    }                    
                }
            }, {
                xtype: 'list',
                itemId: 'mainLiterature',
                enableColumnMove: true,
                enableColumnHide: true,
                enableColumnResize: true,
                store: { data: data.Literature.MainLiterature },
                columns: [{
                    dataIndex: 'recorddata',
                    text: 'Название',
                    width: 500
                }, {
                    dataIndex: 'bookscount',
                    text: 'Количество',
                    width: 120,
                    renderer: function(value) {
                        return value + ' экз.';
                    }
                }]
            }, {
                xtype: 'label',
                margin: '15 0 0 0',
                text: '9.1.2. Дополнительная литература',
                style: captionStyle
            }, {
                html: '[список с указанием наименований из ЭБС]',
                style: italicStyle
            }, {
                xtype: 'textfield',
                fieldLabel: '<b>Поставьте курсор в поле и нажмите Ctrl+V, либо нажмите правой кнопкой мышки по полю и выберите пункт "Вставить"</b>',
                emptyText: 'Вставлять сюда',
                labelWidth: 480,
                height: 50,
                listeners: {
                    change: function (field) {
                        field.reset();
                    },
                    paste: {
                        element: 'inputEl',
                        fn: function (event, inputEl) {
                            try {
                                var literature = parseLiterature(event);
                                if (literature != null)
                                    this.component.up('block').down('#additionalLiterature').getStore().add(literature);
                                else
                                    throw new Error('Не удалось распарсить данные о литературе.');
                            } catch (ex) {
                                console.error(ex.message);
                                alert('ошибка формата данных');
                            }
                            event.stopPropagation();
                            event.preventDefault();
                            return false;
                        }
                    }
                }
            }, {
                xtype: 'list',
                itemId: 'additionalLiterature',
                enableColumnMove: true,
                enableColumnHide: true,
                enableColumnResize: true,
                store: { data: data.Literature.AdditionalLiterature },
                columns: [{
                    dataIndex: 'recorddata',
                    text: 'Название',
                    width: 700
                }, {
                    dataIndex: 'bookscount',
                    text: 'Количество',
                    width: 120,
                    renderer: function (value) {
                        return value + ' экз.';
                    }
                }]
            }, {
                xtype: 'label',
                margin: '15 0 0 0',
                text: '9.1.3. Другая литература',
                style: captionStyle
            }, {
                html: '[список с указанием наименований из ЭБС]',
                style: italicStyle
            }, {
                xtype: 'textareafield',
                height: 150,
                bind: '{Literature.OtherLiterature}'
            }]
        }, {
            name: 'MethodicalSupport',
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '9.2. Методические разработки',
                    style: captionStyle
                }, {
                    html: '[список с указанием наименований из ЭБС]',
                    style: italicStyle
                }, {
                    html: '[в случае отсутствия  указывается: «не используются»]'
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    bind: '{MethodicalSupport}'
                }]
            }        
        }, {
            name: 'Software',
            contentReader: function(content, vm) {
                content.SystemOrOffice = selectStoreItemsData(vm.get('SystemOrOffice'));
                content.Application = selectStoreItemsData(vm.get('Application'));
                return content;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '9.3. Программное обеспечение',
                    style: captionStyle
                }, {
                    html: '[список]',
                    style: italicStyle
                }, {
                    html: '[в случае отсутствия указывается: «не используются»]'
                }, {
                    xtype: 'checkboxfield',
                    fieldLabel: 'Не предусмотрено',
                    labelWidth: 140,
                    bind: { value: '{Software.NotUsed}' }
                }, {
                    xtype: 'label',
                    text: '9.3.1. Общесистемное и офисное ПО (Microsoft Windows, Microsoft Office, Антивирус Касперского)',
                    style: subcaptionStyle
                }, {
                    xtype: 'list',
                    bind: { store: '{SystemOrOffice}' },                    
                    tbar: [{
                        xtype: 'container',
                        viewModel: { data: { foundedItem: null } },
                        layout: { type: 'hbox' },
                        items: [{
                            xtype: 'comboedit',
                            width: 800,
                            bind: {
                                value: '{foundedItem}',
                                store: '{software}'
                            },
                            displayField: 'DisplayName',
                            valueField: 'Id'                           
                        }, {
                            xtype: 'button',
                            text: 'Добавить',
                            bind: { disabled: '{!foundedItem}' },
                            handler: function() {
                                var combo = this.up('container').down('comboedit');
                                var selection = combo.getSelection();
                                this.up('list').getStore().add(selection);
                                combo.reset();
                            }
                        }]
                    }],
                    columns: [{
                        dataIndex: 'Name',
                        text: 'Название',
                        width: 300
                    }, {
                        dataIndex: 'Class',
                        text: 'Класс',
                        width: 300
                    }]
                }, {
                    xtype: 'label',
                    text: '9.3.2 Прикладное ПО, поставленное в комплекте с приборами (программно-аппаратные комплексы)',
                    style: subcaptionStyle
                }, {
                    xtype: 'textareafield',
                    height: 150,                    
                    bind: '{Software.Hardware}'
                }, {
                    xtype: 'label',
                    text: '9.3.3 Свободно распространяемое и бесплатное ПО',
                    style: subcaptionStyle
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    bind: '{Software.Free}'
                }, {
                    xtype: 'label',
                    text: '9.3.4 Ознакомительные или демонстрационные (trial) версии ПО',
                    style: subcaptionStyle
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    bind: '{Software.Trial}'
                }, {
                    xtype: 'label',
                    text: '9.3.5 Прикладное ПО',
                    style: subcaptionStyle
                }, {
                    xtype: 'list',
                    bind: { store: '{Application}' },
                    tbar: [{
                        xtype: 'container',
                        viewModel: { data: { foundedItem: null } },
                        layout: { type: 'hbox' },
                        items: [{
                            xtype: 'comboedit',
                            width: 800,
                            bind: {
                                value: '{foundedItem}',
                                store: '{software}'
                            },
                            displayField: 'DisplayName',
                            valueField: 'Id'
                        }, {
                            xtype: 'button',
                            text: 'Добавить',
                            bind: { disabled: '{!foundedItem}' },
                            handler: function () {
                                var combo = this.up('container').down('comboedit');
                                var selection = combo.getSelection();
                                this.up('list').getStore().add(selection);
                                combo.reset();
                            }
                        }]
                    }],
                    columns: [{
                        dataIndex: 'Name',
                        text: 'Название',
                        width: 300
                    }, {
                        dataIndex: 'Class',
                        text: 'Класс',
                        width: 300
                    }]
                }, {
                    xtype: 'label',
                    text: '9.3.6 Прикладное ПО (дополнительное), отсутствующее в реестре ЛПО УрФУ',
                    style: subcaptionStyle
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    bind: '{Software.AdditionalApplication}'
                }]
            }
        }, {
            name: 'Databases',
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '9.4. Базы данных, информационно-справочные и поисковые системы',
                    style: captionStyle
                }, {
                    html: '[список с указанием наименования  баз данных, информационно-справочных и поисковых систем]',
                    style: italicStyle
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    bind: '{Databases}'
                }]
            }
        }, {
            name: 'ElectronicEducationalResources',
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '9.5. Электронные образовательные ресурсы',
                    style: captionStyle
                }, {
                    html: '[список наименований ЭОР,  имеющих статус «ЭОР УрФУ», ресурсов Интернет с указанием режима доступа]',
                    style: italicStyle
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    bind: '{ElectronicEducationalResources}'
                }]
            }
        }]
    }
}