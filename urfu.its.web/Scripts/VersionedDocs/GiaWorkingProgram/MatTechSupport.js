function uiInit(documentId, documentType, data, schema, emptyData) {
    var italicStyle = { fontStyle: 'italic' };

    return {
        viewModel: {
            stores: {

            }
        },

        items: [
            {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: [ {
                    xtype: 'label',
                    text: '[Указывается необходимое для проведения государственной итоговой аттестации материально-техническое обеспечение, например: полигоны, лаборатории, специально оборудованные кабинеты, измерительные и вычислительные комплексы, транспортные средства и т.д. Указывается компьютерное оборудование, необходимое для проведения ГИА]',
                    style: italicStyle
                }]
            },
            {
            name: 'MatTechSupport',
            items: {
                xtype: 'textarea',
                height: 150,
                name: 'MatTechSupport',
                bind: '{MatTechSupport}'
            }
        }]
    }
}