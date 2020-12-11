function uiInit(documentId, documentType, data, schema) {
    var captionStyle = { fontWeight: 'bold' };
    var subcaptionStyle = { fontWeight: 'bold' };
    var textAlignRight = { 'text-align': 'right' };

    function prepareFdpLessonsPanel(fdpLessons) {
        return fdpLessons.map(function (lessons) {
            var fdp = data.Fdps.filter(function (fdp) { return fdp.ItemId === lessons.FdpId })[0];
            var famType = fdp.FamType;
            var directionCode = fdp.DirectionCode;

            return {
                xtype: 'panel',
                header: false,
                border: true,
                bodyPadding: 6,
                itemId: 'fdpLessonsItemPanel',
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
                    },{
                    xtype: 'panel',
                    layout: { type:'vbox', align: 'stretch'},
                    header: false,
                    defaults: {
                        labelWidth: 250
                    },
                    items: [{
                        fieldLabel: 'Код раздела, темы',
                        xtype: 'comboedit',
                        valueField: 'ItemId',
                        displayField: 'Code',
                        itemId: 'section',
                        reference: 'section',
                        store: {
                            data: Ext.clone(data.Sections)
                        }
                    }, {
                        xtype: 'textfield',
                        itemId: 'name',
                        reference: 'name',
                        fieldLabel: 'Наименование работы'
                    }, {
                        xtype: 'numberfield',
                        itemId: 'duration',
                        reference: 'duration',
                        fieldLabel: 'Время проведения занятия (час.)'
                    }],
                    bbar: [{
                        xtype: 'button',
                        text: '+',
                        width: 50,
                        margin: '0 0 10 0',
                        handler: function () {
                            var vm = this.lookupViewModel();
                            var panel = this.up('#fdpLessonsItemPanel');
                            var grid = panel.down('grid');
                            var sectionEditor = panel.down('#section');
                            var nameEditor = panel.down('#name');
                            var durationEditor = panel.down('#duration');
                            var section = sectionEditor.getValue();
                            var name = nameEditor.getValue();
                            var duration = durationEditor.getValue();

                            if (!section || !name || duration == null) {
                                vm.showMessage('Необходимо заполнить все поля перед добавлением');
                                return;
                            }

                            var sectionCode = data.Sections.filter(function (s) { return s.ItemId === section; })[0].Code;
                            var store = grid.getStore();
                            var items = store.getData().items.map(function (item) { return item.get('SectionCode'); });
                            items.reverse();
                            var index = 0;
                            var sectionNumber = parseInt(sectionCode.substr(1, sectionCode.length));
                            for (var i = 0; i < items.length; i++) {
                                var item = items[i];
                                var itemNumber = parseInt(item.substr(1, item.length));
                                if (itemNumber <= sectionNumber) {
                                    index = items.length - i;
                                    break;
                                }
                            }

                            sectionEditor.reset();
                            nameEditor.reset();
                            durationEditor.reset();

                            store.insert(index, {
                                SectionId: section,
                                SectionCode: sectionCode,
                                Name: name,
                                Duration: duration
                            });
                        }
                    }]
                }, {
                    xtype: 'list',
                    fdpId: fdp.ItemId,
                    store: {
                        data: Ext.clone(lessons.Lessons)
                    },
                    columns: [{
                        dataIndex: 'SectionCode',
                        header: 'Код раздела, темы',
                        width: 180
                    }, {
                        text: 'Номер работы',
                        xtype: 'rownumberer',
                        width: 170
                    }, {
                        dataIndex: 'Name',
                        header: 'Наименование работы',
                        width: 400
                    }, {
                        dataIndex: 'Duration',
                        header: 'Время на выполнение работы (час.)',
                        width: 350
                    }]
                }]
            }
        });
    }

    var fdpsLessonsContentReader = function(content, vm) {
        var lists = this.query('list');
        var fdpsLessons = lists.map(function(l) {
            var items = selectStoreItemsData(l.getStore());
            return {
                FdpId: l.fdpId,
                Lessons: items
            }
        });
        return fdpsLessons;
    };

    function prepareFdpSelfWorkThemesPanel(fdpSelfWorkThemes) {
        return fdpSelfWorkThemes.map(function(selfWorkThemes) {
            var fdp = data.Fdps.filter(function (fdp) { return fdp.ItemId === selfWorkThemes.FdpId })[0];
            var famType = fdp.FamType;
            var directionCode = fdp.DirectionCode;

            return {
                fdpId: fdp.ItemId,
                xtype: 'panel',
                header: false,
                border: true,
                bodyPadding: 6,
                itemId: 'fdpSelfWorkThemesItemPanel',
                layout: { type: 'vbox', align: 'stretch' },
                defaults: {
                    labelWidth: 200
                },
                items: [
                    {
                        xtype: 'label',
                        text: famType + ' форма',
                        style: Ext.apply({}, textAlignRight, captionStyle)
                    }, {
                        xtype: 'label',
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
                        xtype: 'panel',
                        layout: { type: 'vbox', align: 'stretch' },
                        header: false,
                        items: [{
                            xtype: 'label',
                            text: '4.3.1. Примерный перечень тем домашних работ',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'HomeworkThemes',
                            height: 150,
                            value: selfWorkThemes.HomeworkThemes
                        }, {
                            xtype: 'label',
                            text: '4.3.2. Примерный перечень тем графических работ',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'GraphicsWorkThemes',
                            height: 150,
                            value: selfWorkThemes.GraphicsWorkThemes
                        }, {
                            xtype: 'label',
                            text: '4.3.3. Примерный перечень тем рефератов (эссе, творческих работ)',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'ReferatThemes',
                            height: 150,
                            value: selfWorkThemes.ReferatThemes
                        }, {
                            xtype: 'label',
                            text: '4.3.4. Примерная тематика индивидуальных или групповых проектов',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'ProjectThemes',
                            height: 150,
                            value: selfWorkThemes.ProjectThemes
                        }, {
                            xtype: 'label',
                            text: '4.3.5. Примерный перечень тем расчетных работ (программных продуктов)',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'CalcWorkThemes',
                            height: 150,
                            value: selfWorkThemes.CalcWorkThemes
                        }, {
                            xtype: 'label',
                            text: '4.3.6. Примерный перечень тем расчетно-графических работ',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'CalcGraphicsWorkThemes',
                            height: 150,
                            value: selfWorkThemes.CalcGraphicsWorkThemes
                        }, {
                            xtype: 'label',
                            text: '4.3.7. Примерный перечень тем  курсовых проектов (курсовых работ)',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'CourseThemes',
                            height: 150,
                            value: selfWorkThemes.CourseThemes
                        }, {
                            xtype: 'label',
                            text: '4.3.8. Примерная тематика контрольных работ',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'ControlWorkThemes',
                            height: 150,
                            value: selfWorkThemes.ControlWorkThemes
                        }, {
                            xtype: 'label',
                            text: '4.3.9.  Примерная тематика коллоквиумов',
                            style: subcaptionStyle
                        }, {
                            xtype: 'label',
                            text: '[список]'
                        }, {
                            xtype: 'label',
                            text: '[заполняется, если предусмотрено, в ином  случае указывается: «не предусмотрено»]'
                        }, {
                            xtype: 'textareafield',
                            itemId: 'ColloquiumThemes',
                            height: 150,
                            value: selfWorkThemes.ColloquiumThemes
                        }]
                    }
                ]
            }
        });
    }

    return {
        viewModel: {
            
        },

        items: [{
            contentReader: fdpsLessonsContentReader,
            name: 'Labs',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '4.1. Лабораторные работы',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[заполняется, если предусмотрено учебным планом, в ином случае указывается: «не предусмотрено»]',
            }, {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: prepareFdpLessonsPanel(data.Labs)
            }]
        }, {
            contentReader: fdpsLessonsContentReader,
            name: 'Practices',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '4.2. Практические занятия',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[заполняется, если предусмотрено учебным планом, в ином случае указывается: «не предусмотрено»]',
            }, {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: prepareFdpLessonsPanel(data.Practices)
            }]
        }, {
            contentReader: function(content, vm) {
                var fdpItems = this.query('#fdpSelfWorkThemesItemPanel');
                var dataItems = fdpItems.map(function(itemPanel) {
                    return {
                        FdpId: itemPanel.fdpId,
                        HomeworkThemes: itemPanel.down('#HomeworkThemes').getValue(),
                        GraphicsWorkThemes: itemPanel.down('#GraphicsWorkThemes').getValue(),
                        ReferatThemes: itemPanel.down('#ReferatThemes').getValue(),
                        ProjectThemes: itemPanel.down('#ProjectThemes').getValue(),
                        CalcWorkThemes: itemPanel.down('#CalcWorkThemes').getValue(),
                        CalcGraphicsWorkThemes: itemPanel.down('#CalcGraphicsWorkThemes').getValue(),
                        CourseThemes: itemPanel.down('#CourseThemes').getValue(),
                        ControlWorkThemes: itemPanel.down('#ControlWorkThemes').getValue(),
                        ColloquiumThemes: itemPanel.down('#ColloquiumThemes').getValue()
                    }
                });
                return dataItems;
            },
            name: 'SelfWorkThemes',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '4.3.Примерная тематика самостоятельной работы',
                style: captionStyle
            }, {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: prepareFdpSelfWorkThemesPanel(data.SelfWorkThemes)
            }]
        }]
    }
}