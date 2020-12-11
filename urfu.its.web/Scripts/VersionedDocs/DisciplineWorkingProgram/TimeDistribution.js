function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var subcaptionStyle = { fontWeight: 'bold' };
    var textAlignRight = { 'text-align': 'right' };

    Ext.tip.QuickTipManager.init();

    var timeDistributionsBlockDescriptor = schema.Blocks.filter(function (d) { return d.Name === "TimeDistributions"; })[0].Items;
    var timeDistributionsSectionDescriptor = timeDistributionsBlockDescriptor.Properties.filter(function (d) { return d.Name === "Sections" })[0].Items;
    var fdpTimeDistributionEmptyObject = schema.Blocks
        .filter(function (d) { return d.Name === "TimeDistributions" })[0]
        .Items.Properties.reduce(function (p, c) {
            p[c.Name] = null;
            return p;
        }, {});

    
    function prepareFdpLinkItemsPanel(fdpLinkItems) {
        var panels = fdpLinkItems.map(function (link) {
            var fdp = data.Fdps.filter(function (fdp) { return fdp.ItemId === link.FdpId })[0];
            var famType = fdp.FamType;
            var directionCode = fdp.DirectionCode;

            var firstColumnWidth = 270;
            var secondColumnWidth = 310;

            var tableThreeColumnsWidths = [50, 220, 80];
            var tableOtherColumnsWidth = 60;

            var tableColumns = [{
                text: 'Раздел дисциплины',
                //height: 500,
                columns: [{
                    dataIndex: 'SectionCode',
                    text: 'Код раздела, темы',
                    vertical: true,
                    width: tableThreeColumnsWidths[0]
                }, {
                    dataIndex: 'SectionName',
                    text: 'Наименование раздела, темы',
                    width: tableThreeColumnsWidths[1]
                }, {
                    dataIndex: 'TotalTime',
                    text: 'Всего по разделу, теме (час.)',
                    vertical: true,
                    width: tableThreeColumnsWidths[2]
                }]
            }, {
                text: 'Аудиторные занятия (час.)',
                columns: [{
                    dataIndex: 'TotalAuditoryTime',
                    text: 'Всего аудиторной работы (час.)',
                    vertical: true,
                    width: tableOtherColumnsWidth
                }, {
                    dataIndex: 'LectionsTime',
                    text: 'Лекции',
                    vertical: true,
                    width: tableOtherColumnsWidth
                }, {
                    dataIndex: 'PracticesTime',
                    text: 'Практические занятия',
                    vertical: true,
                    width: tableOtherColumnsWidth
                }, {
                    dataIndex: 'LabsTime',
                    text: 'Лабораторные работы',
                    vertical: true,
                    width: tableOtherColumnsWidth
                }]
            }, {
                text: 'Самостоятельная работа: виды, количество и объемы мероприятий',
                columns: [{
                    dataIndex: 'TotalHomeworkTime',
                    text: 'Всего самостоятельной работы студентов (час.)',
                    vertical: true,
                    width: tableOtherColumnsWidth
                }, {
                    text: '&nbsp;<br/>Подготовка к аудиторным занятиям (час.)<br/>&nbsp;',
                    height: 120,
                    columns: [{
                        dataIndex: 'TotalPreparationTime',
                        text: 'Всего (час.)',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'PreparationLectionsTime',
                        text: 'Лекции',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'PreparationPracticesTime',
                        text: 'Практ., семинар. занятие',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'PreparationLabsTime',
                        text: 'Лабораторное занятие',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'PreparationSeminarsTime',
                        text: 'Н/и семинар, семинар-конфер., коллоквиум (магистратура)',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }]
                }, {
                    text: '&nbsp;<br/>Выполнение самостоятельных внеаудиторных работ (колич.)<br/>&nbsp;',
                    columns: [{
                        dataIndex: 'TotalOutOfDoreTime',
                        text: 'Всего (час.)',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'HomeworkCount',
                        text: 'Домашняя работа*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'GraphicsWorkCount',
                        text: 'Графическая работа*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'ReferatsCount',
                        text: 'Реферат, эссе, творч. работа*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'ProjectWorkCount',
                        text: 'Проектная работа*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'CalcWorkCount',
                        text: 'Расчетная работа, разработка программного продукта*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'CalcGraphicsWorkCount',
                        text: 'Расчетно-графическая работа*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'ForeignLanguageWorkCount',
                        text: 'Домашняя работа на иностр. языке*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'TranslationWorkCount',
                        text: 'Перевод инояз. литературы*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'CourseWorkCount',
                        text: 'Курсовая работа*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'CourseProjectCount',
                        text: 'Курсовой проект*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }]
                }, {
                    text: 'Подготовка к контрольным мероприятиям текущей аттестации (колич.)',
                    columns: [{
                        dataIndex: 'TotalControlWorkTime',
                        text: 'Всего (час.)',
                        vertical: true,
                        width: tableOtherColumnsWidth,
                        height: 120
                    }, {
                        dataIndex: 'ControlWorkCount',
                        text: 'Контрольная работа*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }, {
                        dataIndex: 'ColloquiumCount',
                        text: 'Коллоквиум*',
                        vertical: true,
                        width: tableOtherColumnsWidth
                    }]
                }]
            }];
            selectColumnsRecursively(tableColumns, 0).forEach(function(c) {
                c.tooltip = c.text;
            });

            var createEmptySectionItem = function () {
                return timeDistributionsSectionDescriptor.Properties.reduce(function (p, c) {
                    p[c.Name] = null;
                    return p;
                }, {});
            }

            return {
                xtype: 'panel',
                header: false,
                border: true,
                bodyPadding: 6,
                reference: 'fdpLinkItemPanel',
                //itemId: 'fdpLinkItemPanel',
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
                    },
                    {
                    xtype: 'panel',
                    layout: { type: 'vbox', align: 'stretch' },
                    header: false,
                    viewModel: {
                        data: createEmptySectionItem()
                    },
                    defaults: {
                        labelWidth: 250
                    },
                    tbar: [{
                        xtype: 'form',
                        layout: { type: 'vbox', align: 'stretch' },
                        tbar: [{
                            fieldLabel: 'Код раздела, темы',
                            labelWidth: 200,
                            xtype: 'comboedit',
                            itemId: 'sections',
                            bind: {
                                store: '{Sections}',
                                value: '{SectionId}'
                            },
                            valueField: 'ItemId',
                            displayField: 'Code'
                        }],
                        bbar: [{
                            xtype: 'button',
                            width: 120,
                            text: 'Добавить',
                            bind: {
                                disabled: '{!SectionId}'
                            },
                            handler: function () {
                                var panelVm = this.lookupViewModel();

                                var sectionId = panelVm.get('SectionId');
                                if (!sectionId) {
                                    var vm = findRootVm(this);
                                    vm.showMessage("Необходимо указать код раздела, темы");
                                    return;
                                }

                                var newItemData = panelVm.getData();
                                var itemPanel = this.up('panel[reference=fdpLinkItemPanel]');
                                var store = itemPanel.down('list').getStore();                                

                                Ext.Ajax.request({
                                    url: '/DisciplineWorkingProgram/PickUpTimeDistributionSection',
                                    jsonData: {
                                        documentId: documentId,
                                        section: newItemData
                                    },
                                    success: function (response) {
                                        var recData = JSON.parse(response.responseText);

                                        var sectionCode = data.Sections.filter(function (s) { return s.ItemId === newItemData.SectionId; })[0].Code;
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

                                        store.insert(index, recData);

                                        itemPanel.down('form').getForm().reset();
                                    },
                                    failure: function (d) {
                                        console.error(d.responseText);
                                        alert(d.responseText);
                                    }
                                });                                
                            }
                        }],
                        items: [{
                            xtype: 'panel',
                            cls: 'table-all-borders table-cell-padding-normal',
                            layout: { type: 'table', columns: 5 },
                            defaults: {
                                margin: 0,
                                allowNegative: false,
                                minValue: 0
                            },
                            items: [{
                                html: 'Аудиторные занятия (час.)',
                                rowspan: 3,
                                width: firstColumnWidth
                            }, {
                                html: 'Лекции',
                                colspan: 2
                            }, {
                                xtype: 'numberfield',
                                bind: '{LectionsTime}',
                                colspan: 2
                            }, {
                                html: 'Практические занятия',
                                colspan: 2
                            }, {
                                xtype: 'numberfield',
                                bind: '{PracticesTime}',
                                colspan: 2
                            }, {
                                html: 'Лабораторные работы',
                                colspan: 2
                            }, {
                                xtype: 'numberfield',
                                bind: '{LabsTime}',
                                colspan: 2
                            }, {
                                html: 'Самостоятельная работа: виды, количество и объемы мероприятий',
                                rowspan: 16,
                                width: firstColumnWidth
                            }, {
                                html: 'Подготовка к аудиторным занятиям (час.)',
                                rowspan: 4,
                                width: secondColumnWidth
                            }, {
                                html: 'Лекция'
                            }, {
                                xtype: 'numberfield',
                                bind: '{PreparationLectionsTime}',
                                colspan: 2
                            }, {
                                html: 'Практ., семинар. занятие'                                
                            }, {
                                xtype: 'numberfield',
                                bind: '{PreparationPracticesTime}',
                                colspan: 2
                            }, {
                                html: 'Лабораторное занятие'
                            }, {
                                xtype: 'numberfield',
                                bind: '{PreparationLabsTime}',
                                colspan: 2
                            }, {
                                html: 'Н/и семинар, семинар-конфер., коллоквиум (магистратура)'
                            }, {
                                xtype: 'numberfield',
                                bind: '{PreparationSeminarsTime}',
                                colspan: 2
                            }, {
                                html: 'Выполнение самостоятельных внеаудиторных работ (кол-во, час.)',
                                rowspan: 10,
                                width: secondColumnWidth
                            }, {
                                html: 'Домашняя работа*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{HomeworkCount}',
                                allowDecimals: false 
                            }, {
                                xtype: 'numberfield',
                                bind: '{HomeworkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Графическая работа*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{GraphicsWorkCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{GraphicsWorkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Реферат, эссе, творч. работа*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{ReferatsCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{ReferatsTime}',
                                allowDecimals: false
                            }, {
                                html: 'Проектная работа*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{ProjectWorkCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{ProjectWorkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Расчетная работа, разработка программного продукта*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{CalcGraphicsWorkCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{CalcGraphicsWorkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Расчетно-графическая работа*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{CalcWorkCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{CalcWorkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Домашняя работа на иностр. языке*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{ForeignLanguageWorkCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{ForeignLanguageWorkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Перевод инояз. литературы*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{TranslationWorkCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{TranslationWorkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Курсовая работа*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{CourseWorkCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{CourseWorkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Курсовой проект*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{CourseProjectCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{CourseProjectTime}',
                                allowDecimals: false
                            }, {
                                html: 'Подготовка к контрольным мероприятиям текущей аттестации (кол-во, час)',
                                rowspan: 2,
                                width: secondColumnWidth
                            }, {
                                html: 'Контрольная работа*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{ControlWorkCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{ControlWorkTime}',
                                allowDecimals: false
                            }, {
                                html: 'Коллоквиум*'
                            }, {
                                xtype: 'numberfield',
                                bind: '{ColloquiumCount}',
                                allowDecimals: false
                            }, {
                                xtype: 'numberfield',
                                bind: '{ColloquiumTime}',
                                allowDecimals: false
                            }]
                        }]
                    }]
                }, {
                    xtype: 'list',
                    cls: 'grid-header-minimal grid-body-minimal',
                    //plugins: [Ext.create('VersionedDocs.ux.VerticalHeader', {})],
                    fdpId: fdp.ItemId,
                    store: {
                        data: Ext.clone(link.Sections)
                    },
                    header: false,
                    columns: tableColumns
                }, {
                    xtype: 'container',
                    itemId: 'manualTotals',
                    layout: { type: 'table', columns: 3 },
                    margin: '10 0 0 0',
                    cls: 'table-all-borders table-cell-padding-normal',
                    viewModel: {
                        data: {
                            TestTime: link.TestTime,
                            ExamTime: link.ExamTime,
                            IntegratedExamTime: link.IntegratedExamTime,
                            ModuleProjectTime: link.ModuleProjectTime
                        }
                    },
                    defaults: {
                        margin: 0,
                        allowNegative: false,
                        minValue: 0
                    },
                    items: [{
                        html: 'Подготовка к промежуточной аттестации по дисциплине (час.)',
                        rowspan: 2
                    }, {
                        html: 'Зачет'
                    }, {
                        xtype: 'numberfield',
                        bind: '{TestTime}'
                    }, {
                        html: 'Экзамен'
                    }, {
                        xtype: 'numberfield',
                        bind: '{ExamTime}'
                    }, {
                        html: 'Подготовка в рамках дисциплины к промежуточной аттестации по модулю',
                        rowspan: 2
                    }, {
                        html: 'Интегрированный экзамен по модулю'
                    }, {
                        xtype: 'numberfield',
                        bind: '{IntegratedExamTime}',
                        allowDecimals: false
                    }, {
                        html: 'Проект по модулю'
                    }, {
                        xtype: 'numberfield',
                        bind: '{ModuleProjectTime}',
                        allowDecimals: false
                    }]
                }]
            }
        });
        return panels;
    }

    return {
        viewModel: {
            stores: {
                Sections: {
                    data: data.Sections
                }
            }
        },

        items: [{
            contentReader: function (content, vm) {
                var itemPanels = this.query('panel[reference=fdpLinkItemPanel]');
                var fdpsTimeDistributions = itemPanels.map(function (linkItemPanel) {
                    var l = linkItemPanel.down('list');                    
                    var items = l.getStore().getData().items.map(function (item) { return item.getData(); });
                    var manualTotals = linkItemPanel.down('#manualTotals').getViewModel().getData();
                    manualTotals = Object.keys(manualTotals).reduce(function(p, c) {
                        p[c] = manualTotals[c];
                        return p;
                    }, {});

                    var result =  Ext.apply({}, Ext.apply({
                            FdpId: l.fdpId,
                            Sections: items
                        }, manualTotals)
                        , fdpTimeDistributionEmptyObject);
                    return result;
                });
                return fdpsTimeDistributions;
            },

            hasChanges: function() {
                var vm = findRootVm(this);
                var savedData = Ext.clone(data[this.name]);
                savedData.forEach(function(savedItem) {
                    savedItem.DisciplineUnits = null;
                    savedItem.ModuleUnits = null;
                    savedItem.TotalAuditoryTime = null;
                    savedItem.TotalHomeworkTime = null;
                    savedItem.TotalTime = null;
                });
                var preparedData = prepareDataToSave(vm, [this], schema);
                var currentData = preparedData[this.name];
                return hasChanges(savedData, currentData, this);
            },

            name: 'TimeDistributions',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '3.1. Распределение аудиторной нагрузки и мероприятий самостоятельной работы по разделам дисциплины',
                style: captionStyle
            }, {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: prepareFdpLinkItemsPanel(data.TimeDistributions)
            }]
        }]
    }
}