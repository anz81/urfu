function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var textAlignRight = { 'text-align': 'right' };

    function prepareFdpLinkItems(fdpForms) {
        return fdpForms.map(function(link) {
            var fdp = data.Fdps.filter(function (fdp) { return fdp.ItemId === link.FdpId })[0];
            var famType = fdp.FamType;
            var directionCode = fdp.DirectionCode;

            return {
                xtype: 'panel',
                header: false,
                border: true,
                margin: '0 0 10 0',
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
                }, {
                    xtype: 'displayfield',
                    value: link.Form
                }]
            }
        });
    }

    return {
        viewModel: {
            stores: {

            }
        },

        items: [{
            xtype: 'container',
            layout: { type: 'vbox', align: 'stretch' },            
            items: [{
                xtype: 'label',
                text: '5.1. Весовой коэффициент значимости промежуточной аттестации по модулю',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[указать коэффициент, утвержденный  ученым(и) советом(ами) института(ов), в котором(ых) реализуется модуль,  протокол заседания ученого совета № ______   от __________ г. ]',                                
            }],
            margin: '0 0 30 0',
        }, {
            xtype: 'container',
            name: 'ModuleIntermediateCertificationForm',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '5.2. Форма промежуточной аттестации по модулю:',
                style: captionStyle
            }, {
                xtype: 'label',
                text: '[указать форму промежуточной аттестации для оценки интегрированного результата освоения дисциплин модуля: интегрированный экзамен по модулю, выполнение и защита проекта по модулю]'            
            }, {
                xtype: 'container',
                layout: { type: 'vbox', align: 'stretch' },
                items: prepareFdpLinkItems(data.ModuleIntermediateCertificationForms)
            }],
            margin: '0 0 30 0',
        }, {
            xtype: 'container',
            layout: { type: 'vbox', align: 'stretch' },
            items: [{
                xtype: 'label',
                text: '5.3. Фонд оценочных средств для проведения промежуточной аттестации по модулю (Приложение 1)',
                style: captionStyle
            }]
        }]
    }
}