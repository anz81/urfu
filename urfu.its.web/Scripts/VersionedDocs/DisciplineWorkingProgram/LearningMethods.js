function uiInit(documentId, documentType, data, schema) {
    var captionStyle = { fontWeight: 'bold' };

    var descriptor = schema.Blocks.filter(function (d) { return d.Name === 'LearningMethods' })[0];

    var items = data.Sections.map(function (s, i) {
        return {
            xtype: 'panel',
            title: 'Р' + (i+1) + '. ' +  s.Name,
            border: true,
            bodyPadding: 6,
            layout: { type: 'vbox', align: 'stretch' },
            defaults: {
                labelWidth: 200,
                xtype: 'checkboxfield'
            },
            items: [{
                xtype: 'label',
                text: 'Активные методы обучения',
                style: captionStyle
            }, {
                fieldLabel: 'Проектная работа',
                bind: '{LearningMethod' + i + 'ProjectWork}'
            }, {
                fieldLabel: 'Кейс-анализ',
                bind: '{LearningMethod' + i + 'KeysAnalysis}'
            }, {
                fieldLabel: 'Деловые игры',
                bind: '{LearningMethod' + i + 'BusinessGames}'
            }, {
                fieldLabel: 'Проблемное обучение',
                bind: '{LearningMethod' + i + 'ProblemTraining}'
            }, {
                fieldLabel: 'Командная работа',
                bind: '{LearningMethod' + i + 'CommandWork}'
            }, {
                fieldLabel: 'Другие (указать, какие)',
                xtype: 'textareafield',
                height: 150,
                bind: '{LearningMethod' + i + 'OtherActiveMethods}'
            }, {
                xtype: 'label',
                text: 'Дистанционные образовательные технологии и электронное обучение',
                style: captionStyle
            }, {
                fieldLabel: 'Сетевые учебные курсы',
                bind: '{LearningMethod' + i + 'NetworkCourses}'
            }, {
                fieldLabel: 'Виртуальные практикумы и тренажеры',
                bind: '{LearningMethod' + i + 'VirtualPractices}'
            }, {
                fieldLabel: 'Вебинары и видеоконференции',
                bind: '{LearningMethod' + i + 'Webinars}'
            }, {
                fieldLabel: 'Асинхронные web-конференции и семинары',
                bind: '{LearningMethod' + i + 'AsyncWebConferences}'
            }, {
                fieldLabel: 'Совместная работа и разработка контента',
                bind: '{LearningMethod' + i + 'Collaboration}'
            }, {
                fieldLabel: 'Другие (указать, какие)',
                xtype: 'textareafield',
                height: 150,
                bind: '{LearningMethod' + i + 'OtherDistanceMethods}'
            }]
        };
    });

    var formulas = {};
    data.LearningMethods.forEach(function(m, i) {
        descriptor.Items.Properties.forEach(function(prop) {
            if (prop.Name !== 'SectionId') {
                formulas['LearningMethod' + i + prop.Name] = {
                    get: function(get) {
                        return this.get(descriptor.Name)[i][prop.Name];
                    },

                    set: function (value) {
                        this.get(descriptor.Name)[i][prop.Name] = value;                        
                    }
                }
            }
        });
    });

    return {
        viewModel: {
            formulas: formulas
        },

        items: [{
            name: 'LearningMethods',
            itemId: 'themes',
            layout: { type: 'vbox', align: 'stretch' },
            items: items
        }]
    }
}