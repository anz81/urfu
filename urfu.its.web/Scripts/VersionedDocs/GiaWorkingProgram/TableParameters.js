function uiInit(documentId, documentType, data) {
        return {
        viewModel: {
            stores: {
                Directions: {
                    autoLoad: true,
                    data: data.Directions
                },
                Fdps: {
                    autoLoad: true,
                    data: data.Fdps
                }
            }
        },

        items: [{
            name: 'Fdps',
            dataReader: function (block) {
                var vm = block.lookupViewModel();
                var items = vm.get('Fdps').getData().items.map(function (r) { return r.getData() });
                return {
                    Fdps: items
                }
            },
            contentUpdater: function(content, vm) {
                vm.get('Fdps').setData(content);                
            },
            items: [/*{
                xtype: 'label',
                text: '...'
            }, */{
                xtype: 'fdpcontrol',
                directionsStore: '{Directions}',
                fdpStore: '{Fdps}'
            }]
        }]
	}
}