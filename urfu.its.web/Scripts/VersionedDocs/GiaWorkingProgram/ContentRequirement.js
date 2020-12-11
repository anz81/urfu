function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontWeight: 'italic' };
    var subcaptionStyle = { fontWeight: 'bold' };

    return {
        viewModel: {
            stores: {
            }
        },

        items: [

            {
                xtype: 'label',
                text: '2.1.	Тематика государственного экзамена ',
                style: captionStyle
            }, {
                name: 'ExamSubject',
                contentReader: function () {
                    var data = this.down('imagetextlistcontrol').getItemsData();
                    return data;
                },
                hasChanges: function () {
                    var savedData = data[this.name];
                    var currentData = this.contentReader();
                    return hasChanges(savedData, currentData, this);
                },
                items: {
                    xtype: 'container',
                    layout: { type: 'vbox', align: 'stretch' },

                    items: [
                        {
                            html:
                            '<i>[при наличии государственного экзамена указать список примерных экзаменационных  вопросов и заданий, соответствующих ОХОП, и выявляющих сформированность комплекса  результатов обучения]</i>',
                        }, {
                            xtype: 'imagetextlistcontrol',
                            initialItemsData: data.IntegratedExamQuestions
                        }
                    ]
                }
            }, {
                xtype: 'label',
                text: '2.2. Тематика выпускных квалификационных работ ',
                style: captionStyle
            }, {
                //xtype: 'container',
                //layout: { type: 'vbox', align: 'stretch' },
                name: 'QualificationWorkSubject',
                items: [
                    {
                        html:
                        '<i>[привести примерные разделы, темы для выпускных квалификационных работ, соответствующие  направлению подготовки   и требованиям работодателей]</i>',
                    },
                    {
                        xtype: 'textareafield',
                        maxLength: 10000,
                        enforceMaxLength: true,
                        height: 200,

                        bind: '{QualificationWorkSubject}'
                    }]
            }
        ]
    }
}
