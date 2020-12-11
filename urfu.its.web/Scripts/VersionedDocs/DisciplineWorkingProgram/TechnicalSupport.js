function uiInit(documentId, documentType, data) {
    var captionStyle = { fontWeight: 'bold' };

    return {
        viewModel: {
            data: data
        },

        items: [{
            name: 'TechnicalSupport',  
            items: [{
                xtype: 'label',
                text: 'Сведения об оснащенности дисциплины специализированным и лабораторным оборудованием',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[текст с перечнем типов аудиторий, специализированного и лабораторного оборудования и т.д.]'
            }, {
                xtype: 'textareafield',
                height: 250,
                bind: '{TechnicalSupport}'
            }]
        }]
    }
}