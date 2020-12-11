function uiInit(documentId, documentType, data, schema, emptyData) {

    var id = 'FilesGrid';

    var fileWnd = function (btn) {
        return Ext.create('Ext.window.Window',
            {
                title: "Загрузить файл",
                closeAction: 'hide',
                resizable: false,
                height: 150,
                bodyPadding: 6,
                fileUpload: true,
                items: [
                    Ext.create('Ext.form.Panel', {
                        renderTo: Ext.getBody(),
                        items: [{
                            xtype: 'filefield',
                            name: 'document',
                            fieldLabel: 'Выберите файл',
                            labelWidth: 120,
                            width: 500,
                            msgTarget: 'side',
                            allowBlank: false,
                            multiple: false
                        }],
                        buttons: [{
                            text: 'Загрузить файл',
                            handler: function () {
                                var wnd = this.up().up().up();
                                var form = this.up('form').getForm();
                                if (form.isValid()) {
                                    form.submit({
                                        url: '/BasicCharacteristicOP/UploadScan/?info=' + data.EduProgramInfo.Id + '&comment=Приложение 3',
                                        waitMsg: 'Загрузка...',
                                        success: function (fp, o) {
                                            Ext.Msg.alert('Загрузка прошла успешно', 'Файл загружен');
                                            var responseData = Ext.decode(o.response.responseText);

                                            Ext.getCmp(id).getStore().add({
                                                FileName: responseData.fileName,
                                                FileId: responseData.id
                                            });

                                            wnd.close();
                                        }
                                    });
                                }
                            }
                        }]
                    })
                ]
            });
    };
    
    return {
        viewModel: {
            stores: {
                files: {
                    data: data.Files
                }
            }
        },
        items: [
            {
                name: 'Files',
                contentReader: function (content, vm) {
                    content = selectStoreItemsData(Ext.getCmp(id).getStore());
                    return content;
                },
                items: [
                    {
                        xtype: 'grid',
                        id: id,
                        bind: {
                            store: '{files}'
                        },
                        tbar: [
                            {
                                xtype: 'button',
                                text: 'Добавить файл',
                                handler: function (btn) {
                                    var wnd = fileWnd(btn);
                                    wnd.show();
                                }
                            }
                        ],
                        columns: [
                            {
                                xtype: 'hidden',
                                dataIndex: 'FileName'
                            },
                            {
                                xtype: 'hidden',
                                dataIndex: 'FileId'
                            },
                            {
                                header: '',
                                xtype: 'templatecolumn',
                                sortable: false,
                                tpl: '<a href="/BasicCharacteristicOP/DownloadScan/?id={FileId}">{FileName}</a>',
                                width: 300
                            },
                            {
                                xtype: 'actioncolumn',
                                resizable: false,
                                sortable: false,
                                width: 30,
                                items: [{
                                    icon: '/Content/Images/remove.png',
                                    tooltip: 'Удалить файл',
                                    handler: function (grid, rowIndex, colIndex, item, e, record) {

                                        var remove = function () {
                                            grid.store.remove(record);
                                        };

                                        Ext.MessageBox.show({
                                            title: 'Информационное сообщение',
                                            msg: "Вы хотите удалить документ?",
                                            buttons: Ext.MessageBox.YESNO,
                                            fn: function (btn) {
                                                if (btn === 'yes') {
                                                    remove();
                                                }
                                            }
                                        });

                                    }
                                }]
                            }
                        ]
                    }
                ]
            }
        ]
    };
}