function uiInit(documentId, documentType, data, schema, emptyData) {

    // Приложение 1
    var applicationBlock1 = function () {
        var profile = `${data.Profile.Code} - ${data.Profile.Name}`;
        return {
            name: 'ProfStandardsList',
            layout: { type: 'vbox', align: 'stretch' },
            title: 'Перечень профессиональных стандартов, используемых при разработке образовательной программы ' + profile,
            items: [
                {
                    xtype: 'grid',
                    header: false,
                    loadMask: true,
                    bind: {
                        store: '{ProfStandardsList}'
                    },
                    tbar: [{
                        xtype: 'button',
                        text: 'Обновить',
                        cls: 'btn-text-color',
                        style: {
                            borderColor: '#157fcc'
                        },
                        handler: function (btn) {
                            var thisBlock = this.up().up();
                            thisBlock.mask('Обновление');
                            Ext.Ajax.request({
                                method: 'POST',
                                url: '/BasicCharacteristicOP/ProfStandardsList',
                                params: {
                                    id: data.EduProgramInfo.Id,
                                    codes: JSON.stringify(data.Variants.VariantInfos.flatMap(v => v.ProfActivityRows).map(r => r.StandardCode).filter(s => s != null))
                                },
                                success: function (response) {
                                    thisBlock.unmask();
                                    try {
                                        var data = Ext.decode(response.responseText);
                                        thisBlock.getStore().setData(data.profStandardsList);
                                    }
                                    catch{
                                        Ext.MessageBox.show({
                                            title: 'Ошибка',
                                            msg: 'Повторите попытку позже',
                                            buttons: Ext.MessageBox.OK
                                        });
                                    }
                                },
                                failure: function (response) {
                                    thisBlock.unmask();
                                    Ext.MessageBox.show({
                                        title: 'Ошибка',
                                        msg: 'Повторите попытку позже',
                                        buttons: Ext.MessageBox.OK
                                    });
                                }
                            });
                        }
                    }],
                    columns: [
                        {
                            xtype: 'rownumberer',
                            width: 50
                        },
                        {
                            header: 'Код ПС',
                            dataIndex: 'ProfStandardCode',
                            width: 150,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Наименование ПС',
                            dataIndex: 'ProfStandardTitle',
                            width: 200,
                            cellWrap: true,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Реквизиты приказа Министерства труда и социальной защиты Российской Федерации об утверждении; реквизиты изменений в профессиональный стандарт (при наличии)',
                            dataIndex: 'ProfOrderInfo',
                            width: 300,
                            cellWrap: true,
                            renderer: function (value, metaData) {
                                value = `${value.NumberOfMintrud} от ${value.DateOfMintrud}`;

                                if (metaData.record.data.ProfOrderChanges.length > 0)
                                    value += "; ";

                                metaData.record.data.ProfOrderChanges.forEach(function (item, index, array) {
                                    value += `${item.NumberOfMintrud} от ${item.DateOfMintrud}`;
                                    if (index < array.length - 1)
                                        value += ", ";
                                });
                                
                                metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                                return value;
                            }
                        },
                        {
                            header: 'Дата и регистрационный номер Министерства юстиции Российской Федерации; дата и регистрационный номер Минюста РФ при внесении изменений в профессиональный стандарт (при наличии)',
                            dataIndex: 'ProfOrderInfo',
                            width: 300,
                            cellWrap: true,
                            renderer: function (value, metaData) {
                                value = `${value.RegNumberOfMinust} от ${value.RegNumberDateOfMinust}`;

                                if (metaData.record.data.ProfOrderChanges.length > 0)
                                    value += "; ";

                                metaData.record.data.ProfOrderChanges.forEach(function (item, index, array) {
                                    value += `${item.RegNumberOfMinust} от ${item.RegNumberDateOfMinust}`;
                                    if (index < array.length - 1)
                                        value += ", ";
                                });

                                metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                                return value;
                            }
                        }
                    ]
                }
            ]
        };
    };
    
    return {
        viewModel: {
            stores: {
                ProfStandardsList: {
                    data: data.ProfStandardsList
                }
            }
        },

        items: [
            applicationBlock1()
        ]
    };
}