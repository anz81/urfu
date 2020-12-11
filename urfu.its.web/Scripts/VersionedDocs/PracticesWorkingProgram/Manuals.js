function uiInit(documentId, documentType, data, schema, emptyData, canEdit, user, additionalData) {
    var captionStyle = { fontWeight: 'bold' };
    var subcaptionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };

    var literatureUrl = additionalData.LiteratureServiceUrl;
    var literatureError = additionalData.LiteratureServiceError;

    function prepareMainLiteratureItems(practiceForms) {
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
                        xtype: 'textfield',
                        fieldLabel: 'Поставьте курсор в поле и нажмите Ctrl+V, либо нажмите правой кнопкой мышки по полю и выберите пункт "Вставить"',
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
                                            this.component.up('block').down('#mainLiterature' + link.DisciplineUid).getStore().add(literature);
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
                        itemId: 'mainLiterature' + link.DisciplineUid,
                        enableColumnMove: true,
                        enableColumnHide: true,
                        enableColumnResize: true,
                        store: { data: link.Literature.MainLiterature },
                        columns: [{
                            dataIndex: 'recorddata',
                            text: 'Название',
                            width: 500
                        }, {
                            dataIndex: 'bookscount',
                            text: 'Количество',
                            width: 120,
                            renderer: function (value) {
                                return value + ' экз.';
                            }
                        }]
                    }]
            }
        });
    }

    function prepareAdditionalLiteratureItems(practiceForms) {
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
                        xtype: 'textfield',
                        fieldLabel: 'Поставьте курсор в поле и нажмите Ctrl+V, либо нажмите правой кнопкой мышки по полю и выберите пункт "Вставить"',
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
                                            this.component.up('block').down('#additionalLiterature' + link.DisciplineUid).getStore().add(literature);
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
                        itemId: 'additionalLiterature' + link.DisciplineUid,
                        enableColumnMove: true,
                        enableColumnHide: true,
                        enableColumnResize: true,
                        store: { data: link.Literature.AdditionalLiterature },
                        columns: [
                            {
                                dataIndex: 'recorddata',
                                text: 'Название',
                                width: 700,
                                renderer: Urfu.renders.htmlEncodeWithToolTip
                            }, {
                                dataIndex: 'bookscount',
                                text: 'Количество',
                                width: 120,
                                renderer: function (value) {
                                    return value + ' экз.';
                                }
                            }]
                    }
                ]
            }
        });
    }

    function prepareOtherLiteratureItems(practiceForms) {
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
                        itemId: 'otherLiterature' + link.DisciplineUid,
                        height: 150,
                        width: 700,
                        anchor: true,
                        value: link.Literature.OtherLiterature
                    }
                ]
            }
        });
    }

    function prepareMethodicalSupportItems(practiceForms) {
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
                        itemId: 'methodicalSupport' + link.DisciplineUid,
                        height: 150,
                        width: 700,
                        anchor: true,
                        value: link.MethodicalSupport
                    }
                ]
            }
        });
    }

    function prepareSoftwareItems(practiceForms) {
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
                        xtype: 'checkboxfield',
                        itemId: 'softwareNotUsed' + link.DisciplineUid,
                        fieldLabel: 'Не предусмотрено',
                        labelWidth: 140,
                        value: link.SoftwareNotUsed
                    }, 
                    {
                        xtype: 'list',
                        store: { data: link.SoftwareSystemOrOffice },
                        itemId: 'systemOrOffice' + link.DisciplineUid,
                        tbar: [{
                            xtype: 'container',
                            viewModel: { data: { foundedItem: null } },
                            layout: { type: 'hbox' },
                            items: [{
                                xtype: 'comboedit',
                                width: 800,
                                store: {
                                    autoLoad: true,
                                    proxy: {
                                        type: 'ajax',
                                        url: '/DisciplineWorkingProgram/GetSoftware',
                                        reader: { type: 'json' }
                                    }
                                },
                                //bind: {
                                //    value: '{foundedItem}',
                                //    store: '{software}'
                                //},
                                value: '{foundedItem}',
                                displayField: 'DisplayName',
                                valueField: 'Id'
                            }, {
                                xtype: 'button',
                                text: 'Добавить',
                                //bind: { disabled: '{!foundedItem}' },
                                handler: function () {
                                    var combo = this.up('container').down('comboedit');
                                    var selection = combo.getSelection();
                                    if (selection != null) {
                                        this.up('list').getStore().add(selection);
                                        combo.reset();
                                    }
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
                    }
                ]
            }
        });
    }

    function prepareDatabasesItems(practiceForms) {
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
                        itemId: 'databases' + link.DisciplineUid,
                        height: 150,
                        width: 700,
                        anchor: true,
                        value: link.Databases
                    }
                ]
            }
        });
    }

    function prepareElectronicEducationalResourcesItems(practiceForms) {
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
                        itemId: 'electronicEducationalResources' + link.DisciplineUid,
                        height: 150,
                        width: 700,
                        anchor: true,
                        value: link.ElectronicEducationalResources
                    }
                ]
            }
        });
    }

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
                }
            }
        },

        items: [{
            name: 'PracticeManualsStructure',
            contentReader: function (content, vm) {
                var clone = Ext.clone(content);
                var t = this;
                clone.forEach(function (item, index, array) {
                    clone[index].Literature.MainLiterature = selectStoreItemsData(t.down('#mainLiterature' + item.DisciplineUid).getStore());
                    clone[index].Literature.AdditionalLiterature = selectStoreItemsData(t.down('#additionalLiterature' + item.DisciplineUid).getStore());
                    clone[index].Literature.OtherLiterature = t.down('#otherLiterature' + item.DisciplineUid).getValue();
                    clone[index].MethodicalSupport = t.down('#methodicalSupport' + item.DisciplineUid).getValue();
                    clone[index].Databases = t.down('#databases' + item.DisciplineUid).getValue();
                    clone[index].ElectronicEducationalResources = t.down('#electronicEducationalResources' + item.DisciplineUid).getValue();
                    clone[index].SoftwareNotUsed = t.down('#softwareNotUsed' + item.DisciplineUid).getValue();
                    clone[index].SoftwareSystemOrOffice = selectStoreItemsData(t.down('#systemOrOffice' + item.DisciplineUid).getStore());
                });
                return clone;
            },
            items: [{
                xtype: 'label',
                margin: '15 0 0 0',
                text: '5.1. Рекомендуемая литература',
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
                text: '5.1.1. Основная литература',
                style: captionStyle
            }, {
                html: '[список с обязательным указанием наименований из ЭБС]',
                style: italicStyle
            }, {
                html: '[заполняется с учетом наличия печатных изданий в ЗНБ УрФУ]',
                style: italicStyle
            },
            {
                items: prepareMainLiteratureItems(Ext.clone(data.PracticeManualsStructure))
            }, 
            {
                xtype: 'label',
                margin: '15 0 0 0',
                text: '5.1.2. Дополнительная литература',
                style: captionStyle
            }, {
                html: '[список с указанием наименований из ЭБС]',
                style: italicStyle
            },
            {
                items: prepareAdditionalLiteratureItems(Ext.clone(data.PracticeManualsStructure))
            }, 
            {
                xtype: 'label',
                margin: '15 0 0 0',
                text: '5.1.3. Другая литература',
                style: captionStyle
            }, 
            {
                html: '[список с указанием наименований из ЭБС]',
                style: italicStyle
            },
            {
                items: prepareOtherLiteratureItems(Ext.clone(data.PracticeManualsStructure))
            },
            {
                xtype: 'label',
                text: '5.2. Методические разработки',
                style: captionStyle
            }, {
                html: '[список с указанием наименований из ЭБС]',
                style: italicStyle
            }, {
                html: '[в случае отсутствия  указывается: «не используются»]'
            },
            {
                items: prepareMethodicalSupportItems(Ext.clone(data.PracticeManualsStructure))
            },
            {
                xtype: 'label',
                text: '5.3. Программное обеспечение',
                style: captionStyle
            }, {
                html: '[список]',
                style: italicStyle
            }, {
                html: '[в случае отсутствия указывается: «не используются»]'
            },
            {
                items: prepareSoftwareItems(Ext.clone(data.PracticeManualsStructure))
            },
            {
                xtype: 'label',
                text: '5.4. Базы данных, информационно-справочные и поисковые системы',
                style: captionStyle
            }, {
                html: '[список с указанием наименования  баз данных, информационно-справочных и поисковых систем]',
                style: italicStyle
            },
            {
                items: prepareDatabasesItems(Ext.clone(data.PracticeManualsStructure))
            },
            {
                xtype: 'label',
                text: '5.5. Электронные образовательные ресурсы',
                style: captionStyle
            }, {
                html: '[список наименований ЭОР,  имеющих статус «ЭОР УрФУ», ресурсов Интернет с указанием режима доступа]',
                style: italicStyle
            },
            {
                items: prepareElectronicEducationalResourcesItems(Ext.clone(data.PracticeManualsStructure))
            }
            ]
        }]
    }
}