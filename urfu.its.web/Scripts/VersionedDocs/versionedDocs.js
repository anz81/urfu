Ext.define('VersionedDocs.Block', {
    alias: 'widget.block',
    extend: 'Ext.form.Panel',
    cls: 'block',

    autoWidth: true,
    border: false,
    referenceHolder: true,
    bodyPadding: '0 0 10 0',
    session: true,

    viewModel: {
        data: { isChanged: false }
    },

    config: {
        // contentReader: function(content, vm)
        // contentUpdater: function(content, vm)
    },

    defaults: {
        labelWidth: 200,
        anchor: '100%'
    },

    buttons: [{
        xtype: 'label',
        text: 'Блок изменен',
        bind: {
            hidden: '{!isChanged}'
        }
    },
    {
        text: 'Сохранить',
        itemId: 'saveBlock',
        bind: {
            disabled: '{isBusy}'
        }
    }],

    detectChanges: function () {
        var vm = findRootVm(this);
        var blockVm = this.getViewModel();
        var block = this;
        var docSchema = schema;

        var preparedData = prepareDataToSave(vm, [block], docSchema);
        var currentData = preparedData[block.name];
        
        var dataHasChanged = block.hasChanges ? block.hasChanges(data[block.name], currentData, block) : hasChanges(data[block.name], currentData, block);
        blockVm.set('isChanged', dataHasChanged);
        return dataHasChanged;
    },

    initComponent: function() {
        if (!this.config.name) {
            console.log('Необходимо указать название (свойство name) блока:');
            console.log(this);
            alert('Необходимо указать название (свойство name) блока. Смотрите в консоль.');
        }

        function debounce(func, delay) {
            var inDebounce;
            return function () {
                var context = this;
                var args = arguments;
                clearTimeout(inDebounce);
                inDebounce = setTimeout(function () { func.apply(context, args) }, delay);
            }
        }

        var block = this;

        var detectChangeDebounce = debounce(function () {
            block.detectChanges();
        }, 300);
        
        this.callParent();

        var fields = this.query('field');
        fields.forEach(function(field) {
            field.on('change', function () {
                detectChangeDebounce();
            });
        });

        var vm = this.lookupViewModel();

        this.query('grid').forEach(function(grid) {
            var store;
            if (grid.config.bind && grid.config.bind.store) {
                store = vm.get(grid.config.bind.store.substr(1, grid.config.bind.store.length - 2));
            } else {
                store = grid.getStore();
            }

            if (grid.plugins) {
                grid.plugins.forEach(function(p) {
                    if (p instanceof Ext.grid.plugin.CellEditing) {
                        p.on('edit', function () {
                            block.detectChanges();
                        });
                    }
                });
            }

            store.on('add', detectChangeDebounce); 
            store.on('remove', detectChangeDebounce); 
            store.on('update', detectChangeDebounce); 
            store.on('clear', detectChangeDebounce); 
            store.on('load', detectChangeDebounce); 
            store.on('refresh', detectChangeDebounce); 
        });
    }
});

Ext.define('VersionedDocs.ux.VerticalHeader', {
    alias: 'plugin.verticalheader',
    grid: null,
    textMetric: null,
    init: function (grid) {
        var me = this;
        me.grid = grid;
        me.grid.addCls('v-vertical-header-grid');        
        me.grid.on({
            afterlayout: {
                scope: me,
                fn: me.handleAfterLayout
            }
        });
    },

    constructor: function (cfg) {
        Ext.apply(this, cfg);
    },

    handleAfterLayout: function (cmp) {
        function selectItemsRecursively(items, level) {
            var innerItems = [];
            for (var i = 0; i < items.length; i++) {
                var item = items[i];                
                item.level = level;
                innerItems.push(item);
                if (item.items.items) {
                    for (var j = 0; j < item.items.items.length; j++)
                        item.items.items[j].parentColumn = item;
                    Array.prototype.push.apply(innerItems, selectItemsRecursively(item.items.items, level + 1));
                }
            }
            return innerItems;
        }

        var allHeaderItems = selectItemsRecursively(cmp.headerCt.items.items, 0);

        var currentHeight;
        var currentWidth;
        var maxWidth = 0;
        var widthTextMetric = new Ext.util.TextMetrics();
        var levelHeights = [];
        allHeaderItems.forEach(function (item) {
            var textMetric = new Ext.util.TextMetrics(item.el, item.el.getWidth());
            item.addCls('column-header-level-' + item.level);
            var maxHeight = levelHeights[item.level] || 0;
            var size = textMetric.getSize(item.text);
            item.measuredSize = size;
            if ((currentHeight = size.height) > maxHeight) {
                levelHeights[item.level] = currentHeight;
            }

            if (item.vertical) {
                item.addCls('column-header-vertical');
                if ((currentWidth = widthTextMetric.getWidth(item.text)) > maxWidth) {
                    maxWidth = currentWidth + 10;
                }
            }
        });

        levelHeights.forEach(function (lineHeight, i) {
            if (i == levelHeights.length - 1)
                return;
            cmp.headerCt.el.select('.column-header-level-' + i + ' .x-column-header-text').each(function(el) {
                el.setHeight(lineHeight + 14*(i+1));
            });
        });

        var levelsHeight = levelHeights.filter(function (h, i) { return i < levelHeights.length - 1; })
            .reduce(function (p, c) { return p + c; });

        allHeaderItems.forEach(function (item) {
            if (item.vertical) {                
                var parentColumns = [];
                var b = item;
                while (b.parentColumn) {
                    parentColumns.push(b.parentColumn);
                    b = b.parentColumn;
                }
                var currentTopLevelHeight = parentColumns.reduce(function(p, c) {
                    return p + levelHeights[c.level];
                }, 0)// + 14*(item.level+1);
                var diff = levelsHeight - currentTopLevelHeight;
                item.setHeight(maxWidth + diff);
                item.el.select('.x-column-header-text')
                    .setSize(maxWidth + diff, maxWidth + diff);                
            }
        });
        /*cmp.headerCt.el.select('.column-header-vertical .x-column-header-text').each(function (el) {
            el.setSize(maxWidth, maxWidth);
        });*/
    }
});

Ext.define('VersionedDocs.Combo', {
    alias: 'widget.comboedit',
    extend: 'Ext.form.field.ComboBox',

    listeners: {
        // Поиск в комбобоксах по вхождению подстроки без учета регистра в displayField, а не по началу значения displayField
        beforequery: function(qe) {
            qe.query = new RegExp(escapeRegExp(qe.query), 'i');
            qe.forceAll = true;
        }
    },
    editable: true,
    queryMode: 'local',
    forceSelection: true
});

Ext.define('VersionedDocs.FamilirizationTypeDisciplinePlanControl', {
    alias: 'widget.fdpcontrol',
    extend: 'Ext.container.Container',

    layout: { type: 'vbox', align: 'stretch' },

    config: {
        directionsStore: null,
        fdpStore: null // табличка с формой обучения, направлением и УП
    },

    initComponent: function() {
        var config = this.config;
        var canEdit = (this.up('#rootPanel') || Ext.ComponentQuery.query('#rootPanel')[0]).canEdit;
        this.items = [{
            xtype: 'container',
            layout: { type: 'hbox' },

            defaults: {
                margin: 5,
                readOnly: !canEdit
            },

            items: [{
                xtype: 'comboedit',
                reference: 'famTypeCombo',
                itemId: 'famTypeCombo',
                store: {
                    data: [['Экстернат'], ['Очно-заочная'], ['Заочная'], ['Очная']],
                    fields: ['name'],
                    type: 'array'
                },
                valueField: 'name',
                displayField: 'name',
                emptyText: 'Добавить форму обучения',
                width: 350
            }, {
                xtype: 'comboedit',
                bind: {
                    store: config.directionsStore
                },
                reference: 'directionsCombo',                
                itemId: 'directionsCombo',
                valueField: 'Id',
                displayField: 'Code',                
                emptyText: 'Добавить направление',
                width: 350
            }, {
                xtype: 'button',
                text: '+',
                handler: function() {
                    var container = this.up('container');
                    var famType = container.down('#famTypeCombo').getValue();
                    var directionsCombo = container.down('#directionsCombo');
                    var direction = directionsCombo.getValue();
                    if (!famType || !direction) {
                        alert('Необходимо указать форму обучения и направление');
                        return;
                    }

                    var fdpStore = container.up('fdpcontrol').down('#fdpGrid').getStore();
                    fdpStore.add({
                        FamType: famType,
                        DirectionId: direction,
                        DirectionCode: directionsCombo.getDisplayValue(),
                        DisciplineId: null,
                        ItemId: null
                    });
                }
            }]
        }, {
            xtype: 'list',
            itemId: 'fdpGrid',
            bind: {
                store: config.fdpStore
            },
            columns: [{
                header: 'Форма обучения',
                dataIndex: 'FamType',
                sortable: false,
                sortableColumn: false,
                width: 250
            }, {
                header: 'Направление',
                dataIndex: 'DirectionCode',
                sortable: false,
                sortableColumn: false,
                width: 200
            }, {
                xtype: 'widgetcolumn',
                tdCls: 'widget-column',
                header: 'УП',
                width: 200,
                dataIndex: 'DisciplineId',
                sortable: false,
                sortableColumn: false,
                widget: {
                    readOnly: !canEdit,
                    xtype: 'comboedit',
                    displayField: 'DisplayName',
                    valueField: 'DisciplineId',
                    allowBlank: false,
                    listeners: {
                        select: function(datefield, value, eOpts) {
                            var rowIndex = datefield.up('gridview').indexOf(datefield.el.up('table'));
                            var record = datefield.up('gridview').getStore().getAt(rowIndex);
                            record.set('DisciplineId', value.get('DisciplineId'));
                        }
                    }
                },
                onWidgetAttach: function(column, widget, record) {
                    var directionId = record.get('DirectionId');
                    var famType = record.get('FamType');
                    var store = Ext.create('Ext.data.Store', {
                        autoLoad: true,
                        proxy: {
                            type: 'ajax',
                            url: '/WorkingPrograms/GetPlans' + window.location.search,
                            reader: { type: 'json' },
                            extraParams: {
                                documentId: documentId,
                                documentType: documentType,
                                famType: famType,
                                directionId: directionId
                            }
                        }
                    });
                    widget.setStore(store);
                }
            }]
        }];

        this.callParent();
    }
});

Ext.define('VersionedDocs.ListControl', {
    alias: 'widget.list',
    extend: 'Ext.grid.Panel',

    enableLocking: false,  // turn off column lock context items
    enableColumnMove: false,  // turn off column reorder drag drop
    enableColumnHide: false,  // turn off column reorder drag drop
    enableColumnResize: false,  // turn off column resize for whole grid
    sortableColumns: false,

    //deferEmptyText: false,
    config: {
        allowDeleteItem: function() {
            return true;
        }
    },

    viewConfig: {
        emptyText: 'Список пуст',
        markDirty: false
    },

    initComponent: function () {
        var canEdit = (this.up('#rootPanel') || Ext.ComponentQuery.query('#rootPanel')[0]).canEdit;

        this.columns.push({
            xtype: 'actioncolumn',
            hidden: !canEdit,
            resizable: false,
            sortable: false,
            width: 60,
            items: [{
                icon: '/Content/Images/remove.png',
                tooltip: 'Удалить',
                handler: function (grid, rowIndex, colIndex, item, e, record) {
                    var vm = findRootVm(grid);
                    //var vm = grid.lookupViewModel();
                    if (vm.confirmMessage('Вы действительно желаете удалить запись?')) {
                        grid.getStore().remove(record);
                    }
                },               
                getClass: function(v, meta, rec) {          
                    if(!this.up('grid').allowDeleteItem(rec, meta)) {
                        return "x-hidden";
                    }
                }
            }]
        });

        this.callParent();
    }
});

Ext.define('VersionedDocs.ImageTextListControl', {
    alias: 'widget.imagetextlistcontrol',
    extend: 'Ext.container.Container',

    layout: { type: 'vbox', align: 'stretch' },
    style: { width: '100%' },

    config: {
        initialItemsData: null
    },

    getItemsData: function() {
        var data = this.down('#imageTextItems').items.items.map(function (item) {
            var result = {
                Text: null,
                ImageDataUrl: null
            };

            var image = item.down('image');
            if (image)
                result.ImageDataUrl = image.src;            

            var text = item.down('label');
            if(text)
                result.Text = text.text;

            return result;
        });
        return data;
    },

    setItemsData: function (items) {
        var me = this;
        items.forEach(function(item) { me.addItem(item) });
    },

    addItem: function (itemData) {
        var imageTextItems = this.down('#imageTextItems');
        var block = this.up('block');
        if (itemData.Text) {
            imageTextItems.add({
                xtype: 'panel',
                header: false,
                border: true,
                bodyPadding: 8,
                items: {
                    xtype: 'label',
                    text: itemData.Text,
                    style: {
                        'white-space': 'pre-wrap'
                    }
                },
                rbar: [{
                    xtype: 'button',
                    bind: {
                        disabled: '{!canEdit}'
                    },
                    text: 'X',
                    padding: 0,
                    height: 28,
                    width: 28,
                    handler: function() {
                        var item = this.up('panel');
                        var panel = item.up('#imageTextItems');
                        panel.remove(item);
                        block.detectChanges();
                    }
                }]
            });
        } else {
            imageTextItems.add({
                xtype: 'panel',
                bodyPadding: 8,
                header: false,
                border: true,
                layout: 'anchor',
                autoHeight: true,
                items: {
                    xtype: 'image',
                    autoHeight: true,
                    alt: 'img',                    
                    src: itemData.ImageDataUrl,
                    maxWidth: 1000,
                    listeners: {
                        afterrender: function () {
                            var imagePanel = this.up('panel');
                            var el = this.getEl();
                            var imgDom = el.dom;
                            imgDom.onload = function() {
                                var h = el.getHeight();
                                imagePanel.setHeight(h + 18);                                
                            }                                                       
                        }
                    }
                },
                rbar: [{
                    xtype: 'button',
                    text: 'X',
                    bind: {
                        disabled: '{!canEdit}'
                    },
                    padding: 0,
                    height: 28,
                    width: 28,
                    handler: function () {
                        var item = this.up('panel');
                        var panel = item.up('#imageTextItems');
                        panel.remove(item);
                        block.detectChanges();
                    }
                }]
            });            
        }
        block.detectChanges();
    },

    listeners: {
        afterrender: function () {
            if (this.initialItemsData) {
                this.setItemsData(this.initialItemsData);
            }
        }
    },
    
    initComponent: function () {              
        this.items = [{
            xtype:'panel',
            margin: '0 0 10 0',
            items: [{
                xtype: 'textareafield',
                itemId: 'newText',
                style: { width: '100%' },
                height: 150
            }],
            rbar: [{
                xtype: 'button',
                text: '+',
                width: 50,
                handler: function () {
                    var control = this.up('imagetextlistcontrol');
                    var newTextEditor = control.down('#newText');
                    var newText = newTextEditor.getValue();
                    var vm = this.lookupViewModel();
                    if (!newText) {
                        vm.showMessage('Не заполнен текст');
                        return;
                    }
                    newTextEditor.setValue(null);
                    control.addItem({ Text: newText });
                }
            }]
        }, {
            xtype: 'panel',
            items: [{
                xtype: 'filefield',
                itemId: 'newImage',
                style: { width: '100%' },
                emptyText: 'Файл не выбран',
                buttonText: 'Выбрать изображение',
                listeners: {
                    change: function (t, value, eOpts) {
                        var newValue = value.replace(/^c:\\fakepath\\/i, ''); // remove fakepath
                        t.setRawValue(newValue);
                        t.file = t.getEl().down('input[type=file]').dom.files[0];
                    }
                }
            }],
            rbar: [{
                xtype: 'button',
                text: '+',
                width: 50,
                handler: function() {
                    var control = this.up('imagetextlistcontrol');
                    var newImageEditor = control.down('#newImage');
                    var newImage = newImageEditor.getValue();
                    var vm = this.lookupViewModel();
                    if (!newImage) {
                        vm.showMessage('Не выбрано изображение');
                        return;
                    }
                    
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        control.addItem({ ImageDataUrl: e.target.result });
                        newImageEditor.reset();                        
                    }
                    reader.readAsDataURL(newImageEditor.file);
                }
            }]
        }, {
            xtype: 'panel',
            bodyPadding: 6,
            header: false,
            border: true,
            defaults: {
                margin: '0 0 10 0'
            },
            itemId: 'imageTextItems',    
            layout: { type: 'vbox', align:'left' },

        }];
        
        this.callParent();
    }
});

Ext.define('Its.controller.VersionedDocument', {
    extend: 'Ext.app.ViewController',
    alias: 'controller.versioneddocument',

    control: {
        'block button#saveBlock': {
            click: function(btn) {                    
                var vm = findRootVm(btn);
                var blocksToSave = [btn.up('block')];
                var view = this.getView();
                saveBlocks(view.saveUrl, vm, blocksToSave, 'Сохранение блока выполнено');
            }
        },
        '#saveSection': {
            click: function (btn) {                                        
                var vm = findRootVm(btn);
                var blocksToSave = Ext.ComponentQuery.query('block');
                var view = this.getView();
                saveBlocks(view.saveUrl, vm, blocksToSave, 'Сохранение раздела выполнено');
            }
        },
        'grid': {
            beforeedit: function(e) {
                var canEdit = this.getView().canEdit;
                if (!canEdit)
                    return false;
            }
        }
    }
});

Ext.define('VersionedDocs.Selector', {
    alias: 'widget.selector',
    extend: 'Ext.window.Window',

    labelWidth: 100,
    layout: 'fit',
    title: 'Создание версии',

    width: 507,

    config: {
        documentKind: null,
        linkedEntityId: null,
        canView: null,
        canEdit: null,
        hiddenStandard: null,
        hiddenYear: true,
        year: null
    },

    setDocumentKind: function(v) {
        this.lookupViewModel().set('documentKind', v);
    },

    setLinkedEntityId: function(v) {
        this.lookupViewModel().set('linkedEntityId', v);
    },

    setCanView: function(v) {
        this.lookupViewModel().set('canView', v);
    },

    setCanEdit: function (v) {
        this.lookupViewModel().set('canEdit', v);
    },

    setHiddenStandard: function (v) {
        this.lookupViewModel().set('hiddenStandard', v);
    },

    setHiddenYear: function (v) {
        this.lookupViewModel().set('hiddenYear', v);
    },

    setYear: function (v) {
        this.lookupViewModel().set('year', v);
    },

    viewModel: {
        data: {
            standard: 'ФГОС ВО',
            documentId: null,
            documentKind: null,
            linkedEntityId: null,
            canView: null,
            canEdit: null,
            hiddenStandard: null,
            hiddenYear: true,
            year: null
        },
        formulas: {
            canCreate: function(get) {
                return get('canEdit') && !get('documentId');
            },
            canCreateBasedOn: function(get) {
                return get('canEdit') && get('documentId');
            },
            canOpen: function(get) {
                return get('canView') && get('documentId');
            },
            hideYear: function (get) {
                return get('canEdit') && (!get('documentId') || get('hiddenYear'));
            }
        },
        stores: {
            standards: {
                autoLoad: true,
                proxy: {
                    type: 'ajax',
                    url: '/WorkingPrograms/GetStandards' + window.location.search,
                    reader: { type: 'json' }
                }
            },
            versions: {
                autoLoad: '{standard}{documentKind}{linkedEntityId}{year}',
                proxy: {
                    type: 'ajax',
                    url: '/WorkingPrograms/GetVersions' + window.location.search,
                    reader: { type: 'json' },
                    extraParams: {
                        standard: '{standard}',
                        documentKind: '{documentKind}',
                        linkedEntityId: '{linkedEntityId}',
                        year: '{year}'
                    }
                }
            }
        }
    },

    items: {
        xtype: 'form',
        bodyPadding: 6,
        layout: { type: 'vbox', align: 'stretch' },
        items: [{
            fieldLabel: "Стандарт",
            xtype: "combobox",
            name: 'standard',
            bind: {
                store: '{standards}',
                value: '{standard}',
                hidden: '{hiddenStandard}'
            },
            valueField: 'Name',
            allowBlank: false,
            displayField: 'Name',
            emptyText: 'Выберите стандарт',
            queryMode: 'local',
            cls: 'field-margin',
            editable: false
        },
        {
            fieldLabel: "Версии",
            xtype: "combobox",
            name: 'documentId',
            bind: {
                store: '{versions}',
                value: '{documentId}'
            },
            valueField: 'DocumentId',
            displayField: 'Name',
            queryMode: 'local',
            cls: 'field-margin',
            allowBlank: true,
            forceSelection: true,
            editable: true,
            labelWidth: 110
        },
        {
            xtype: 'numberfield',
            fieldLabel: 'Учебный год (новая версия)',
            name: 'year',
            hideTrigger: true,
            cls: 'field-margin',
            minValue: 2014,
            maxValue: 2099,
            bind: {
                value: '{year}',
                hidden: '{hideYear}'
            },
            width: 200,
            labelWidth: 110,
            validator: function required(value) {
                return (value != undefined && value.length > 0) ? true : "Поле не может быть пустым";
            }
        }


        ],

        buttons: [{
            text: "Создать новую",
            bind: {
                disabled: '{!canCreate}'
            },
            handler: function() {
                var vm = this.lookupViewModel();
                var maskEl = this.up('window').getEl();
                maskEl.mask('Выполнение операции...');
                Ext.Ajax.request({
                    url: '/WorkingPrograms/CreateNew' + window.location.search,
                    success: function (response) {
                        var d = Ext.JSON.decode(response.responseText);
                        window.location.assign(d.redirect);
                        maskEl.unmask();
                    },
                    failure: function(d) {
                        console.error(d.responseText);
                        maskEl.unmask();
                        alert(d.responseText);
                    },
                    jsonData: {
                        linkedEntityId: vm.get('linkedEntityId'),
                        standard: vm.get('standard'),
                        documentKind: vm.get('documentKind'),
                        year: vm.get('year')
                    }
                });
            }
        }, {
            text: "Создать на основе",
            bind: {
                disabled: '{!canCreateBasedOn}'
            },
            handler: function () {

                var year = this.up('form').getForm().getValues().year;
                if (!this.up('form').up().getViewModel().getData().hiddenYear && (year == undefined || year.length < 4))
                    return;

                var maskEl = this.up('window').getEl();
                maskEl.mask('Выполнение операции...');
                Ext.Ajax.request({
                    url: '/WorkingPrograms/CreateBasedOn' + window.location.search,
                    success: function(response) {
                        var d = Ext.JSON.decode(response.responseText);
                        window.location.assign(d.redirect);
                        maskEl.unmask();
                    },
                    failure: function(d) {
                        alert(d.statusText);
                        console.error(d.statusText);
                        maskEl.unmask();
                    },
                    jsonData: this.up('form').getForm().getValues()
                });
            }
        }, {
            text: "Открыть",
            bind: {
                disabled: '{!canOpen}'
            },
            handler: function() {
                var vm = this.lookupViewModel();
                window.location.assign('/Document/' + vm.get('documentId'));
            }
        }, {
            text: "Отмена",
            handler: function() { this.up('window').close(); }
        }]
    }
});

Ext.define("ciber.ux.grid.OverridePluginRowExpander", {
    alias: "plugin.overriderowexpander",
    extend: "Ext.grid.plugin.RowExpander",

    isCollapsed: function (rowIdx) {
        var rowNode = this.view.getNode(rowIdx);
        var row = Ext.fly(rowNode, '_rowExpander');

        return row.hasCls(this.rowCollapsedCls);
    },

    collapse: function (rowIdx) {
        if (this.isCollapsed(rowIdx) === false)
            this.toggleRow(rowIdx, this.grid.getStore().getAt(rowIdx));
    },

    collapseAll: function () {
        for (var i = 0; i < this.grid.getStore().getTotalCount(); i++)
            this.collapse(i);
    },

    expand: function (rowIdx) {
        if (this.isCollapsed(rowIdx) === true)
            this.toggleRow(rowIdx, this.grid.getStore().getAt(rowIdx));
    },

    expandAll: function () {
        for (var i = 0; i < this.grid.getStore().getTotalCount(); i++)
            this.expand(i);
    }
});

Ext.define('Ext.ux.GroupComboBox', {
    extend: 'Ext.form.field.ComboBox',
    alias: 'widget.groupcombobox',
    /*
     * @cfg groupField String value of field to groupBy, set this to any field in your model
     */
    groupField: 'group',
    listConfig: {
        cls: 'grouped-list'
    },
    initComponent: function () {
        var me = this;
        me.tpl = new Ext.XTemplate([
            '{%this.currentGroup = null%}',
            '<tpl for=".">',
            '   <tpl if="this.shouldShowHeader(' + me.groupField + ')">',
            '       <div class="group-header">{[this.showHeader(values.' + me.groupField + ')]}</div>',
            '   </tpl>',
            '   <div class="x-boundlist-item">' + me.displayField + '</div>',
            '</tpl>', {
                shouldShowHeader: function (group) {
                    return this.currentGroup != group;
                },
                showHeader: function (group) {
                    this.currentGroup = group;
                    return group;
                }
            }
        ]);
        me.callParent(arguments);
    }
});


function arrayDistinct(array) {
    var flags = [], output = [], l = array.length, i;
    for (i = 0; i < l; i++) {
        if (flags[array[i]]) continue;
        flags[array[i]] = true;
        output.push(array[i]);
    }
    return output;
}

function selectStoreItemsData(store) {
    return store.getData().items.map(function (i) { return i.getData() });
}

function selectColumnsRecursively(items, level) {
    var innerItems = [];
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        item.level = level;
        innerItems.push(item);
        if (item.columns) {
            for (var j = 0; j < item.columns.length; j++)
                item.columns.parentColumn = item;
            Array.prototype.push.apply(innerItems, selectColumnsRecursively(item.columns, level + 1));
        }
    }
    return innerItems;
}

function findRootVm(cmp) {
    var vm = cmp.lookupViewModel();
    while (vm.config.parent != null) {
        vm = vm.config.parent;
    }
    return vm;
}

function prepareInspectionMessagesText(inspections, messageType, caption) {
    if (!inspections || !inspections.length)
        return null;

    var filteredInspections = inspections.filter(function (inspection) {
        return inspection[messageType].length;
    });

    var number = 0;

    var text = '';
    for (var i = 0; i < filteredInspections.length; i++) {
        var inspection = filteredInspections[i];
        var messages = inspection[messageType];
        if (caption && !text)
            text = caption;

        for (var j = 0; j < messages.length; j++) {
            var message = messages[j];
            var numberedMessage = ++number + ". " + message;
            if (j !== messages.length - 1)
                numberedMessage += "\n";
            text += numberedMessage;
        }

        if (i !== filteredInspections.length - 1) {
            text += "\n--\n";
        }
    }

    return text;
}

function prepareDataToSave(vm, blocks, docSchema) {
    var data = {};

    blocks.forEach(function(block) {
        var rawData = vm.get(block.name);
        var contentData = rawData;
        var blockSchema = docSchema.Blocks.filter(function (b) { return b.Name === block.name })[0];
        if (rawData != null) {
            if (rawData instanceof Ext.data.Record || rawData instanceof Ext.data.Model) {
                contentData = fixContent(rawData.getData(), blockSchema);
            } else if (rawData instanceof Ext.data.Store) {
                contentData = fixContent(selectStoreItemsData(rawData), blockSchema);                
            }
        }

        var content = contentData;
        if (block.contentReader)
            content = block.contentReader(contentData, vm);

        // Все даты преобразуем в строки (если нужна будет дата со временем, нужно информацию в схему добавлять и вытаскивать, пока такой необходимости не видно)
        prepareDates(content, '');

        data[block.name] = fixContent(content, blockSchema);
    });

    return data;
}

function saveBlocks(saveUrl, vm, blocks, successMessage, successCallback) {
    var allBlocks = Ext.ComponentQuery.query('block').reduce(function (p, c) { p[c.name] = c; return p; }, {});

    var blocksData = prepareDataToSave(vm, blocks, schema);

    vm.set('isBusy', true);
    Ext.Ajax.request({
        url: saveUrl,
        jsonData: {
            blocksData: JSON.stringify(blocksData)
        },
        success: function (response) {
            vm.set('isBusy', false);
            var saveResult = JSON.parse(response.responseText);

            if (saveResult.inspections.length)
                console.log(saveResult.inspections);

            if (saveResult.success) {
                var updatedData = saveResult.changedDocumentData;
                for (var blockName in updatedData) {
                    data[blockName] = Ext.clone(updatedData[blockName]);
                    var blockToUpdate = allBlocks[blockName];
                    if (blockToUpdate && blockToUpdate.contentUpdater) {
                        var content = updatedData[blockName];
                        blockToUpdate.contentUpdater(content, vm);
                    }
                }
                console.log(updatedData);

                blocks.forEach(function(b) {
                    b.detectChanges();
                });

                blocks.forEach(function(b) {
                    b.fireEvent('saved', b);
                    //b.getViewModel().set('isChanged', false);
                });

                console.log("Сохранение блоков [" + blocks.map(function (b) { return b.name }).join(", ") + "] успешно выполнено.");

                Ext.Msg.alert(successMessage);

                if (successCallback)
                    successCallback();
                /*Ext.toast{
                    html: successMessage,
                    align: 'tr'
                });*/
            }

            var warningsText = prepareInspectionMessagesText(saveResult.inspections, "Warnings", "Предупреждения:\n");
            var errorsText = prepareInspectionMessagesText(saveResult.inspections, "Errors", "Ошибки:\n");
            var message = [errorsText, warningsText].filter(function (m) { return m != null; }).join("\n");
            if (errorsText)
                console.error(errorsText);
            if (warningsText)
                console.warn(warningsText);
            if (message)
                vm.showMessage(message);
        },
        failure: function (d) {
            vm.set('isBusy', false);
            console.error(d.responseText);
            vm.showMessage(d.responseText);
        }
    });
}

function prepareDates(obj, stack) {
    for (var property in obj) {
        if (obj.hasOwnProperty(property)) {
            var propValue = obj[property];
            if (propValue instanceof Date) {
                propValue = Ext.Date.format(propValue, 'd.m.Y');
                obj[property] = propValue;
            }
            else if (typeof propValue == "object") {
                prepareDates(propValue, (stack ? '.' : '') + property);
            }
        }
    }
}

function escapeRegExp(str) {
    return str.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
}

function getOrderedIndex(items, condition) {
    var array = items;
    if (items instanceof Ext.data.Store)
        array = selectStoreItemsData(items);

    var insertIndex = 0;
    for (var i = array.length - 1; i >= 0; i--) {
        var item = array[i];
        if (condition(item)) {
            insertIndex = i + 1;
            break;
        }
    }

    return insertIndex;
}

function hasChanges(savedValue, currentValue, block) {
    if (currentValue === "")
        return savedValue != null && savedValue !== "";
    return JSON.sortify(currentValue) !== JSON.sortify(savedValue);
}

function fixContent(currentData, itemSchema) {
    //var itemSchema = schema.Blocks.filter(function (b) { return b.Name === blockName })[0];
    if (itemSchema.Kind === 65) {
        if (currentData === "")
            return null;        
    }
    if (itemSchema.Kind === 16) {
        var result = itemSchema.Properties.reduce(function (p, c) {
            p[c.Name] = fixContent(currentData[c.Name], c);
            return p;
        }, {});
        return result;
    }
    if (itemSchema.Kind === 32) {
        var items = currentData.map(function (cd) {
            return fixContent(cd, itemSchema.Items);            
        });
        return items;
    }
    return currentData;
}