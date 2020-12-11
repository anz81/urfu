Ext.define('PracticeDocument', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'DocumentType', type: 'int' },
        { name: 'DocumentTypeName', type: 'string' },
        { name: 'TemplateName', type: 'string' },
        { name: 'DocumentId', type: 'int' },
        { name: 'DocumentName', type: 'string' },
        { name: 'Status', type: 'int' },
        { name: 'StatusName', type: 'string' },
        { name: 'Comment', type: 'string' }
    ],

    idProperty: 'DocumentType'
});

Ext.define('DocumnetStore', {
    extend: 'Ext.data.Store',
    model: 'PracticeDocument'
});

Ext.define('Practice.view.DeclineDocument', {
    extend: 'Ext.window.Window',
    title: 'Причина отказа',
    closeAction: 'hide',
    resizable: false,
    autoHeight: true,
    width: 500,

    defaultListenerScope: true,
    viewModel:
    {
        comment: null
    },

    config: {
        saveDocument: function () { },
        record: null,
        store: null,
    },

    items: [{
        xtype: 'textareafield',
        width: 498,
        maxRows: 4,
        bind: '{comment}'
    }],

    buttons: [
        {
            text: "ОК",
            listeners: { click: 'onOK' }
        },
        {
            text: "Отмена",
            listeners: { click: 'onCancel' }
        }
    ],

    onOK: function () {
        var comment = this.viewModel.get('comment');
        if (comment != undefined && comment.length > 0)
        {
            this.saveDocument('deny', this.record, this.viewModel.get('comment'), this.store);
        }
        else
        {
            Ext.Msg.alert('Ошибка', 'Укажите причину отказа');
        }
    },

    onCancel: function () {
        this.close();
    },

});

Ext.define('Practice.view.Upload', {
    extend: 'Ext.window.Window',
    title: 'Загрузка документа на сервер',
    //xtype: 'practice.view.upload',
    //closeAction: 'hide',
    resizable: true,
    autoHeight: true,
    width: 500,
    bodyPadding: 10,
    fileUpload: true,

    config: {
        url: null,
        parcticeId: null,
        documentType: null,
        store: null
    },

    defaultListenerScope: true,
    referenceHolder: true,
    items: [{
        xtype: 'form',
        reference: 'form',
        bodyPadding: 10,
        fileUpload: true,
        frame: true,
        items: [{
            xtype: 'filefield',
            fieldLabel: 'Документ',
            labelWidth: 100,
            id: 'form-file',
            name: 'document-path',
            emptyText: 'Файл не выбран',
            allowBlank: false,
            anchor: '100%',
            multiple: false,
            buttonText: 'Выберите документ...',
            listeners: {
                change: function (t, value, eOpts) {
                    var newValue = value.replace(/^c:\\fakepath\\/i, ''); // remove fakepath
                    t.setRawValue(newValue);
                }
            }
        }],
        buttons: [{
            text: 'Отправить',
            listeners: { click: 'onUpload' }
        }, {
            text: "Отмена",
            listeners: { click: 'onCancel' }
        }]
    }],

    onUpload: function () {
        var me = this;
        var documentType = this.documentType;

        var form = me.lookupReference('form');
        var f1 = form.getForm();

        if (!me.canEdit) {
            Ext.Msg.alert('Ошибка', 'У вас нет прав вносить изменения');
            return;
        }
        
        if (f1.getFieldValues()['document-path'] != "") {
            if (f1.isValid()) {
                f1.submit({

                    url: this.url + '?practiceId=' + this.parcticeId + '&type=' + documentType,
                    waitMsg: 'Загрузка документа...',
                    success: function (f, a) {
                        me.store.getById(documentType).set({ DocumentId: a.result.fileId, DocumentName: a.result.fileName, Date: a.result.date });

                        Ext.Msg.alert('Success', 'Ваш документ "' + a.result.fileName + '" успешно загружен.');
                        me.close();
                    },
                    failure: function (f, a) {
                        Ext.Msg.alert('Ошибка', 'Ваш документ не загружен. ' + a.result.message);
                        me.close();
                    }
                });
            }
        }
    },

    onCancel: function () {
        this.close();
    }
});


Ext.define('Practice.view.Button2Column', {
    extend: 'Ext.container.Container',
    xtype: 'button2column',
    defaultListenerScope: true,
    /*viewModel: {
        data: {
            buttonDisabled:false
        }  
    },*/
    layout: {
        type: 'hbox'
    },
    items: [{
        xtype: 'button',
        text: 'Согласовать',
        listeners: {
            click: 'submit'
        }
    },
    {
        xtype: 'button',
        text: 'Отклонить',
        listeners: {
            click: 'deny'
        }
    },
    {
        xtype: 'button',
        text: 'Удалить',
        /*bind: {
            disabled: '{buttonDisabled}',      
        },*/
        listeners: {
            click:'remove'
        }        
      },
    ],
    /*listeners: {
        beforerender: function (widget, eOpts) {
            var isDisabled = widget.getWidgetRecord().data.Status === 2;
            widget.getViewModel().setData({ 'buttonDisabled': isDisabled })
        }
    },*/

    submit: function () {
        console.log('button submit');
        this.fireEvent('command', 'subbmit', this.getWidgetRecord().data);
    },

    deny: function () {
        console.log('button deny');
        this.fireEvent('command', 'deny', this.getWidgetRecord().data);
    },
   
    remove: function () {
        console.log('button remove');
        this.fireEvent('command', 'remove', this.getWidgetRecord().data);
    }


});

Ext.define('Ux.grid.column.Widget', {
    extend :'Ext.grid.column.Widget',
    alias :'widget.ux-widgetcolumn',
    onUpdate :null,
    onWidgetAttach: function (column, widget, record) {
        var me = this,
            fn = me.onUpdate;
        if (fn) {
            fn.call(widget, record, column);
            me.mon(me.up('gridpanel').getView(), {
                scope: widget,
                args: [record, column],
                itemupdate: fn
            })
        }
    }
}
)
Ext.define('Practice.view.DocumentGrid', {
    extend: 'Ext.grid.Panel',
    xtype: 'practiceDocumentGrid',
    region: 'center',
    defaultListenerScope: true,

    viewConfig: {
        markDirty: false
    },
    columnLines: true,

    border: 1,

    config: {
        documents: null,
    },

    store: {
        xtype: 'DocumnetStore'
    },

    columns: [{
        xtype: 'rownumberer', width: 30
    }, {
        dataIndex: 'DocumentTypeName',
        width: 300,
        renderer: Urfu.renders.htmlEncodeWithToolTip
    }, {
        xtype: 'templatecolumn',
        sortable: false,
        header: 'Шаблон',
        width: 170,
        renderer: function (a, cell, record) {
            if (record.data.DocumentType == 10)
                return 'Свободная форма'; 
            else
                return cell.column.defaultRenderer(null, null, record);
        }
    }, {
        xtype: 'ux-widgetcolumn',
        sortable: false,
        hideable: false,
        menuDisabled: true,
        width: 150,
        tdCls: 'no-padding',
        widget: {
            xtype: 'button',
            text: 'Обзор...',
            listeners: {
                click: 'upload'
            }
            },
            onUpdate: function (record, column) {
                this.setDisabled(record.get('Status') == 1);
            }
       
    }, {
        xtype: 'templatecolumn',
        sortable: false,
        width: 200,
    }, {
        header: 'Дата импорта файла',
        dataIndex: 'Date',
        width: 150
    }, {
        header: 'Статус',
        dataIndex: 'StatusName',
        width: 150
    }, {
        header: 'Комментарий',
        dataIndex: 'Comment',
        width: 200
    }, {
        xtype: 'widgetcolumn',
        sortable: false,
        hideable: false,

        menuDisabled: true,
        tdCls: 'no-padding',
        widget: {
            xtype: 'button2column',
            listeners: {
                command: 'command'
            }
        },
        width: 335
    }],

    initComponent: function () {
        var me = this;
        me.columns[2].tpl = me.$initParent.tplDownloadTemplate;
        me.columns[4].tpl = me.$initParent.tplDownloadDocument;
        me.callParent(arguments);
    },

    upload: function (a) {
        var t = a.getWidgetRecord().data.DocumentType;
        this.fireEvent('upload', t, this.store);
    },

    command: function (a, d) {
        this.fireEvent('command', a, d, this.store);
    },

    updateDocuments: function (documents) {
        documents.forEach(function (item) {
            var document = Ext.create('PracticeDocument', item);
            this.store.add(document);
        }, this);
    },

})

Ext.define('Practice.view.DocumentPanel', {
    extend: 'Ext.panel.Panel',
    xtype: 'practiceDocumentPanel',

    title: 'Сканы документов',
    collapsible: true,

    padding: '5px',
    bodyPadding: '5px',

    defaultListenerScope: true,

    viewModel: {
        data: {
            documents: null,
        }
    },

    config: {
        documents: null
    },

    items: [{
        xtype: 'label',
            html: 'Объем файла не более 10 Мб.<br> Перед загрузкой документов сократите объем изображений.<br>' +
                   'Не загружайте документы с изображениями высокого качества.<br>' + 
                   'По статистике средний объем загружаемого документа не превышает 3 Мб.'
        },{
            xtype: 'practiceDocumentGrid',
        reference: 'gridBefore',
        title: 'До начала практики',
        padding: '5px',
        bind: {
            documents: '{documents.before}'
        },
        listeners: {
            command: 'command',
            upload: 'upload'
        }
    },{
            xtype: 'practiceDocumentGrid',
        reference: 'gridDistant',
        title: 'Для дистанционной формы прохождения практики',
        padding: '5px',
        bind: {
            documents: '{documents.distant}'
        },
        listeners: {
            command: 'command',
            upload: 'upload'
        }
    }, {
        xtype: 'practiceDocumentGrid',
        title: 'Отчет по практике',
        padding: '5px',
        bind: {
            documents: '{documents.after}'
        },
        listeners: {
            command: 'command',
            upload: 'upload'
        }
    }],

    upload: function (type, store) {
        this.fireEvent('upload', type, store);
    },

    command: function (action, data, store) {
        this.fireEvent('command', action, data, store);
    },

    updateDocuments: function (documents) {
        var vm = this.getViewModel();
        vm.set('documents', documents);
    },

    Refresh: function () {
        this.items.items.forEach(function (i) { i.getView().refresh() });
    }

});


