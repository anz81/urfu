function uiInit(documentId, documentType, data, schema, emptyData, canEdit, user, additionalData) {
    var captionStyle = { fontWeight: 'bold' };
    var subcaptionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };

    var literatureUrl = additionalData.LiteratureServiceUrl;
    var literatureError = additionalData.LiteratureServiceError;

    function prepareEvalutionToolsPracticeItems(practiceForms) {
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
                        itemId: 'evalutionTools' + link.DisciplineUid,
                        height: 150,
                        width: 700,
                        anchor: true,
                        value: link.EvalutionTools
                    }
                ]
            }
        });
    }

    function prepareEvalutionToolsItems(practiceForms) {
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
                        text: link.DirectionCode,
                        style: captionStyle
                    },
                    {
                        items: prepareEvalutionToolsPracticeItems(Ext.clone(link.Items))
                    }
                ]
            }
        });
    }
    
    return {
        viewModel: {
        },
        items: [{
            name: 'PracticeEvalutionToolsStructure',
            contentReader: function (content, vm) {
                var clone = Ext.clone(content);
                var t = this;
                clone.forEach(function (item, index, array) {
                    clone[index].Items.forEach(function (item2, index2, array2) {
                        clone[index].Items[index2].EvalutionTools = t.down('#evalutionTools' + clone[index].Items[index2].DisciplineUid).getValue();
                    });
                });
                return clone;
            },
            items: [{
                xtype: 'label',
                margin: '15 0 0 0',
                text: 'Виды практик и примерная тематика контрольных мероприятий текущей и промежуточной аттестации',
                style: captionStyle
            }, 
            {
                items: prepareEvalutionToolsItems(Ext.clone(data.PracticeEvalutionToolsStructure))
            }
            ]
        }]
    }
}