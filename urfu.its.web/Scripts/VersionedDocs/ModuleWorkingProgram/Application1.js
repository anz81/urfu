function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var subcaptionStyle = { fontWeight: 'bold' };

    return {
        viewModel: {
            stores: {

            }
        },

        items: [{
            name: 'ControlEventsEstimationCriterias',
            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: '5.3.1. ОБЩИЕ КРИТЕРИИ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО МОДУЛЮ',
                style: captionStyle
            }, {
                xtype: 'label',
                text: 'Система критериев оценивания результатов обучения в рамках модуля опирается на три уровня освоения: пороговый, повышенный, высокий.'
            }, {
                xtype: 'label',
                text: 'Признаки уровня освоения компонентов компетенций',
                style: captionStyle
            }, {
                xtype: 'panel',
                title: 'Знания',
                layout: { type: 'vbox', align: 'stretch' },
                defaults: {
                    xtype: 'textareafield',
                    maxLength: 10000,
                    enforceMaxLength: true
                },
                items: [{
                    xtype: 'label',
                    text: 'пороговый',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.ThresholdKnowledgeLevel}'
                }, {
                    xtype: 'label',
                    text: 'повышенный',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.ElevatedKnowledgeLevel}'
                }, {
                    xtype: 'label',
                    text: 'высокий',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.HighKnowledgeLevel}'
                }]
            }, {
                xtype: 'panel',
                title: 'Умения',
                layout: { type: 'vbox', align: 'stretch' },
                defaults: {
                    xtype: 'textareafield',
                    maxLength: 10000,
                    enforceMaxLength: true
                },
                items: [{
                    xtype: 'label',
                    text: 'пороговый',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.ThresholdSkillsLevel}'
                }, {
                    xtype: 'label',
                    text: 'повышенный',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.ElevatedSkillsLevel}'
                }, {
                    xtype: 'label',
                    text: 'высокий',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.HighSkillsLevel}'
                }]
            }, {
                xtype: 'panel',
                title: 'Личностные качества',
                layout: { type: 'vbox', align: 'stretch' },
                defaults: {
                    xtype: 'textareafield',
                    maxLength: 10000,
                    enforceMaxLength: true
                },
                items: [{
                    xtype: 'label',
                    text: 'пороговый',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.ThresholdPersonalQualitiesLevel}'
                }, {
                    xtype: 'label',
                    text: 'повышенный',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.ElevatedPersonalQualitiesLevel}'
                }, {
                    xtype: 'label',
                    text: 'высокий',
                    style: subcaptionStyle
                }, {
                    bind: '{ControlEventsEstimationCriterias.HighPersonalQualitiesLevel}'
                }]
            }]
        }, {
            xtype: 'label',
            text: '5.3.2. ОЦЕНОЧНЫЕ СРЕДСТВА ДЛЯ ПРОВЕДЕНИЯ ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО МОДУЛЮ',
            style: captionStyle
        }, {
            name: 'IntegratedExamQuestions',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            hasChanges: function() {
                var savedData = data[this.name];
                var currentData = this.contentReader();
                return hasChanges(savedData, currentData, this);
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    html: '<b>5.3.2.1. Перечень примерных вопросов для интегрированного экзамена по модулю</b> [<i>список</i>].',                    
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.IntegratedExamQuestions
                }]
            }
        }, {
            xtype: 'label',
            text: '5.3.2.2. Перечень примерных тем итоговых проектов по модулю [список].',
            style: captionStyle
        }, {
            name: 'ModuleProjectThemes',
            items: {
                xtype: 'textarea',
                height: 200,
                bind: '{ModuleProjectThemes}'
            }
        }]
    }
}