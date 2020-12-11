function uiInit(documentId, documentType, data, schema, emptyData) {
	return {
        viewModel: {
            stores: {
                
            }
        },

        items: [{
            title: 'Аннотация',
            name: 'Annotation',
            items: {
                xtype: 'textarea',
                height: 150,
                name: 'Annotation',
                bind: '{Annotation}'
            }
        }]
	}
}