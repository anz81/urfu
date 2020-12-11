function uiInit(documentId, documentType, data) {
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
                text: '8.1. КРИТЕРИИ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ КОНТРОЛЬНО-ОЦЕНОЧНЫХ МЕРОПРИЯТИЙ ТЕКУЩЕЙ И ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО ДИСЦИПЛИНЕ В РАМКАХ БРС',
                style: captionStyle
            }, {
                xtype: 'label',
                text: 'В рамках БРС применяются утвержденные на кафедре критерии оценивания достижений студентов по каждому контрольно-оценочному мероприятию. Система критериев оценивания, как и при проведении промежуточной аттестации по модулю, опирается на три уровня освоения компонентов компетенций: пороговый, повышенный, высокий.'
            }, {
                xtype: 'label',
                text: 'Признаки уровня освоения компонентов компетенций',
                style: captionStyle
            }, {
                xtype: 'panel',
                title: 'Знания',
                layout: { type: 'vbox', align: 'stretch' },
                defaults: {
                    xtype: 'displayfield',
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
                    xtype: 'displayfield',
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
                    xtype: 'displayfield',
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
            xtype: 'container',
            layout: { type: 'vbox', align: 'stretch' },

            items: [{
                xtype: 'label',
                text: '8.2. КРИТЕРИИ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ  ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПРИ ИСПОЛЬЗОВАНИИ НЕЗАВИСИМОГО ТЕСТОВОГО КОНТРОЛЯ',
                style: captionStyle
            }, {
                xtype: 'label',
                text: 'При проведении независимого тестового контроля как формы промежуточной аттестации применяется  методика оценивания результатов, предлагаемая разработчиками тестов. Процентные показатели результатов независимого тестового контроля  переводятся в баллы промежуточной аттестации по 100-балльной шкале в БРС:'
            }, {
                xtype: 'label',
                text: '- в случае балльной оценки по тесту (блокам, частям теста) переводится процент набранных баллов от общего числа возможных баллов по тесту;'
            }, {
                xtype: 'label',
                text: '- при отсутствии балльной оценки по тесту  переводится процент верно выполненных заданий теста, от общего числа заданий.'
            }]
        }, {
            xtype: 'container',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '8.3. ОЦЕНОЧНЫЕ СРЕДСТВА ДЛЯ ПРОВЕДЕНИЯ ТЕКУЩЕЙ И ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ',
                style: captionStyle
            }, {
                xtype: 'label',
                text: 'Выбрать из списка, либо дополнить наименования оценочных средств'
            }]
        }, {
            name: 'MiniControlWorkThemes',
            contentReader: function() {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.1. Примерные  задания для проведения мини-контрольных в рамках учебных занятий',
                    style: subcaptionStyle
                }, {
                    xtype: 'label',
                    text: '[тексты заданий]'
                }, {
                    xtype: 'label',
                    text: '[в случае отсутствия  указывается: «не предусмотрено»]'
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.MiniControlWorkThemes
                }]
            }        
        }, {
            name: 'ControlWorkThemes',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.2. Примерные  контрольные задачи в рамках учебных занятий',
                    style: subcaptionStyle
                }, {
                    xtype: 'label',
                    text: '[тексты задач]'
                }, {
                    xtype: 'label',
                    text: '[в случае отсутствия  указывается: «не предусмотрено»]'
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.ControlWorkThemes
                }]
            }
        }, {
            name: 'ControlKeys',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.3. Примерные  контрольные кейсы',
                    style: subcaptionStyle
                }, {
                    xtype: 'label',
                    text: '[тексты кейсов]'
                }, {
                    xtype: 'label',
                    text: '[в случае отсутствия  указывается: «не предусмотрено»]'
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.ControlKeys
                }]
            }
        }, {
            name: 'TestQuestions',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.4. Перечень примерных  вопросов для зачета',
                    style: subcaptionStyle
                }, {
                    xtype: 'label',
                    text: '[список]'
                }, {
                    xtype: 'label',
                    text: '[в случае отсутствия  указывается: «не предусмотрено»]'
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.TestQuestions
                }]
            }
        }, {
            name: 'ExamQuestions',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.5. Перечень примерных  вопросов для экзамена',
                    style: subcaptionStyle
                }, {
                    xtype: 'label',
                    text: '[список]'
                }, {
                    xtype: 'label',
                    text: '[в случае отсутствия  указывается: «не предусмотрено»]'
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.ExamQuestions
                }]
            }
        }, {
            name: 'UrfuResources',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.6. Ресурсы АПИМ УрФУ, СКУД УрФУ для проведения тестового контроля в рамках текущей и промежуточной аттестации',
                    style: subcaptionStyle
                }, {
                    xtype: 'label',
                    text: '[список и ссылки на официально утвержденные электронные ресурсы]'
                }, {
                    xtype: 'label',
                    text: '[в случае отсутствия  указывается: «не используются»]'
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.UrfuResources
                }]
            }
        }, {
            name: 'FepoResources',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.7. Ресурсы ФЭПО для проведения независимого тестового контроля',
                    style: subcaptionStyle
                }, {
                    xtype: 'label',
                    text: '[список на основе ресурса www.фэпо.рф]'
                }, {
                    xtype: 'label',
                    text: '[в случае отсутствия  указывается: «не используются»]'
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.FepoResources
                }]
            }
        }, {
            name: 'InternetTrainers',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.8. Интернет-тренажеры',
                    style: subcaptionStyle
                }, {
                    xtype: 'label',
                    text: '[список на основе ресурса www.i-exam.ru]'
                }, {
                    xtype: 'label',
                    text: '[в случае отсутствия  указывается: «не используются»]'
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.InternetTrainers
                }]
            }
        }, {
            name: 'OtherEvaluationTools',
            contentReader: function () {
                var data = this.down('imagetextlistcontrol').getItemsData();
                return data;
            },
            items: {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },

                items: [{
                    xtype: 'label',
                    text: '8.3.9....[указать иные наименования оценочных средств, не представленных в списке].',
                    style: subcaptionStyle
                }, {
                    xtype: 'imagetextlistcontrol',
                    initialItemsData: data.OtherEvaluationTools
                }]
            }
        }]
    }
}