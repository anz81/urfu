function uiInit(documentId, documentType, data, schema, emptyData) {

    var id = 'ApprovalActsInfo';

    var chair = data.Chair.Name;
    
    var profCompetencesCmbxName = 'profCompetencesCmbxName';

    var getApprovalActData = function (item) {
        var values = {};

        values.NumberOfApprovalAct = item.items.findBy(i => i.name === "NumberOfApprovalAct").value;
        values.Experts = item.items.findBy(i => i.name === "Experts").store.getData().items.map(function (i) { return i.getData(); }).map(function (i) { delete i.id; return i; });
        values.ChairDirector = item.items.findBy(i => i.name === "ChairDirectorContainer").items.items.find(i => i.name === "ChairDirector").value;
        values.DateChair = item.items.findBy(i => i.name === "DateChairContainer").items.items.find(i => i.name === "DateChair").value;
        values.Company = item.items.findBy(i => i.name === "Company").value;
        values.CompanyDirector = item.items.findBy(i => i.name === "CompanyDirector").value;
        values.DateCompany = item.items.findBy(i => i.name === "DateCompanyContainer").items.items.find(i => i.name === "DateCompany").value;
        values.DateOfApprovalAct = item.items.findBy(i => i.name === "DateOfApprovalActContainer").items.items.find(i => i.name === "DateOfApprovalAct").value;

        var variantInfos = selectStoreItemsData(item.items.findBy(i => i.name === "VariantInfos").getStore()).map(function (i) { delete i.id; return i; });

        values.VariantInfos = [];
        variantInfos.forEach(function (value) {
            
            if (!values.VariantInfos.some(i => i.Id === value.Id)) {
                values.VariantInfos.push({
                    Id: value.Id,
                    Name: value.Name,
                    IdSource: value.IdSource,
                    ProfActivityRows: []
                });
            }
            
            var info = values.VariantInfos.find(i => i.Id === value.Id);
            
            delete value.Id;
            delete value.Name;
            delete value.IdSource;
            delete value.VariantIndex;
            delete value.RowIndex;

            info.ProfActivityRows.push(value);
        });


        values.FileName = item.items.findBy(i => i.name === "FileName").value;
        values.FileId = item.items.findBy(i => i.name === "FileId").value;
        if (values.FileId == "")
            values.FileId = null;
        if (values.FileId !== null)
            values.FileId = Number(values.FileId);

        return values;
    };

    var loadFileFunction = function (id)
    {
        return function () {
            Ext.DomHelper.append(Ext.getBody(), {
                tag: 'iframe',
                frameBorder: 0,
                width: 0,
                height: 0,
                css: 'display:none;visibility:hidden;height:0px;',
                src: '/BasicCharacteristicOP/DownloadScan/?id=' + id
            });
        };
    };

    var profCompetencesWnd = function (variant, taskTypes, record, storeData, grid, insertIndex) {
        var competences = record.data.Competences.map(c => c.Id);
        return Ext.create('Ext.window.Window',
            {
                title: "Профессиональные компетенции",
                closeAction: 'hide',
                overflowY: 'auto',
                resizable: true,
                x: 100,
                y: 100,
                maxHeight: 500,
                width: 525,
                autoHeight: true,
                modal: true,
                bodyPadding: 6,
                layout: { type: 'vbox', align: 'stretch' },
                items: [
                    {
                        xtype: 'label',
                        text: 'Траектория',
                        style: 'font-weight: bold;'
                    },
                    {
                        xtype: 'label',
                        text: variant
                    },
                    {
                        xtype: 'label',
                        text: 'Типы задачи профессиональной деятельности',
                        style: 'font-weight: bold;'
                    },
                    {
                        xtype: 'label',
                        text: taskTypes
                    },
                    {
                        xtype: 'label',
                        text: 'Профессиональные компетенции',
                        style: 'font-weight: bold;'
                    },
                    Ext.create('Ext.form.field.Tag', {
                        fieldLabel: '',
                        store: {
                            data: storeData
                        },
                        value: competences,
                        name: profCompetencesCmbxName,
                        labelWidth: 0,
                        editable: true,
                        queryMode: 'local',
                        displayField: 'Code',
                        valueField: 'Id',
                        width: 500,
                        tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                            '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                            '<div><b>{Code}</b> {Content}</div>' +
                            '</div>',
                            '</tpl>'
                        )
                    })
                ],
                buttons: [
                    {
                        text: "ОК",
                        handler: function (btn) {
                            
                            var cmbx = this.up().up().items.items.find(c => c.name === profCompetencesCmbxName);
                            var variantInfoStore = grid.getStore();

                            var competenceIds = cmbx.getValue();
                            var competences = cmbx.getStore().data.items.filter(c => competenceIds.some(com => com === c.data.Id)).map(c => c.data);
                            
                            record.data.Competences = competences;
                            variantInfoStore.insert(insertIndex, record);
                            btn.up('window').close();
                        }
                    }
                ]
            });
    };

    var fileWnd = function (fileId, btn) {
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
                                        url: '/BasicCharacteristicOP/UploadScan/?info=' + data.EduProgramInfo.Id + '&comment=акт согласования',
                                        waitMsg: 'Загрузка...',
                                        success: function (fp, o) {
                                            Ext.Msg.alert('Загрузка прошла успешно', 'Файл загружен');
                                            var responseData = Ext.decode(o.response.responseText);

                                            var loadFileComponent = btn.up().items.items.find(i => i.name === "LoadFileLink");

                                            loadFileComponent.setHtml('<a href="#">' + responseData.fileName + '</a>');
                                            loadFileComponent.el.clearListeners();
                                            loadFileComponent.el.addListener('click', loadFileFunction(responseData.id));
                                            loadFileComponent.setHidden(false);

                                            btn.up().up().items.items.find(i => i.name === "FileName").setValue(responseData.fileName);
                                            btn.up().up().items.items.find(i => i.name === "FileId").setValue(responseData.id);

                                            btn.up().items.items.find(i => i.name === "RemoveFileButton").setHidden(false);

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

    var getVariantInfosData = function (dataBlock) {
        var variantInfosDataInitial = dataBlock !== null
            ? dataBlock.VariantInfos !== null ? dataBlock.VariantInfos : data.Variants.VariantInfos
            : data.Variants.VariantInfos;

        var variantInfosData = [];
        variantInfosDataInitial.forEach(function (value, index, array) {

            value.ProfActivityRows.forEach(function (row, rowIndex) {
                var rowData = Object.assign({}, row);

                rowData.Id = value.Id;
                rowData.Name = value.Name;
                rowData.IdSource = value.IdSource;
                rowData.VariantIndex = index;
                rowData.RowIndex = rowIndex;

                variantInfosData.push(rowData);
            });
        });
        return variantInfosData;
    };

    var filterStore = function (store, variant, profStandard) {
        store.clearFilter();
        store.filterBy(function (item) {

            return (item.data.IdSource !== 0 /*IdSource.Profile*/ && item.data.Id === variant || variant === null) &&
                (profStandard === null || profStandard === '' && item.data.NoProfStandard || profStandard === item.data.StandardCode && !item.data.NoProfStandard);
        });
    };

    var approvalActBlock = function (dataBlock, index) {

        var variantInfosData = getVariantInfosData(dataBlock);

        var fileName = dataBlock !== null ? (dataBlock.FileName !== null ? dataBlock.FileName : "") : "";
        var fileId = dataBlock !== null ? dataBlock.FileId : null;
        
        var dataVariants = [];
        var dataProfStandards = [];

        var dataForFilters = dataBlock !== null ? dataBlock : data.Variants;
        dataForFilters.VariantInfos.forEach(function (item, index, array) {
            if (item.IdSource !== 0) // IdSource.Profile
            {
                if (!dataVariants.some(v => v.Id === item.Id && v.Name === item.Name)) {
                    dataVariants.push({
                        Id: item.Id,
                        Name: item.Name
                    });
                }

                item.ProfActivityRows.forEach(function (row) {
                    if (row.NoProfStandard) {
                        if (dataProfStandards.some(s => s.Title === 'Не указан')) {
                            dataProfStandards.push({
                                Code: '',
                                Title: 'Не указан'
                            });
                        }
                    }
                    else {
                        if (!dataProfStandards.some(s => s.Code === row.StandardCode && s.Title === row.StandardTitle)) {
                            dataProfStandards.push({
                                Code: row.StandardCode,
                                Title: row.StandardTitle
                            });
                        }
                    }
                });
            }
        });

        return {
            margin: '20 0 0 0',
            name: index + 'approvalact',
            title: 'Акт согласования', //index + '. Акт согласования',
            closable: true,
            closeToolText: 'Удалить Акт согласования',
            layout: { type: 'vbox', align: 'stretch' },
            defaults: { labelWidth: 200 },
            items: [
                {
                    xtype: 'hidden',
                    name: 'FileName',
                    value: dataBlock !== null ? dataBlock.FileName : ""
                },
                {
                    xtype: 'hidden',
                    name: 'FileId',
                    value: dataBlock !== null ? dataBlock.FileId : null
                },
                {
                    xtype: 'textfield',
                    margin: '10 0 0 0',
                    name: 'NumberOfApprovalAct',
                    value: dataBlock !== null ? dataBlock.NumberOfApprovalAct : "",
                    fieldLabel: 'Акт согласования №'
                },
                {
                    xtype: 'label',
                    margin: '10 10 0 0',
                    style: { 'font-weight': 'bold' },
                    text: 'Согласовано'
                },
                {
                    xtype: 'textfield',
                    fieldLabel: 'Кафедра/департамент',
                    labelWidth: 200,
                    value: chair,
                    editable: false
                },
                {
                    xtype: 'container',
                    name: 'ChairDirectorContainer',
                    layout: { type: 'hbox' },
                    items: [
                        {
                            xtype: 'textfield',
                            labelWidth: 200,
                            width: 500,
                            fieldLabel: 'Зав.кафедрой/Руководитель департамента',
                            name: 'ChairDirector',
                            value: dataBlock !== null ? dataBlock.ChairDirector : ""
                        },
                        {
                            xtype: 'button',
                            text: 'Выбрать',
                            margin: '5 0 0 10',
                            handler: function (btn) {
                                var vm = findRootVm(btn);
                                var name = this.up().up().name;
                                var modal = createPeopleSelectorWindow(function (selection) {
                                    var panel = Ext.getCmp(id).items.items.find(i => i.name === name);
                                    panel.items.items.find(i => i.name === "ChairDirectorContainer").items.items.find(i => i.name === "ChairDirector").setValue(selection[0].get('ShortName'));
                                }, vm, 'employees');
                                modal.show();
                            }
                        },
                        {
                            xtype: 'button',
                            text: 'Добавить',
                            margin: '5 0 0 10',
                            handler: function (btn) {
                                var vm = findRootVm(btn);
                                var name = this.up().up().name;
                                var modal = createPeopleWindow(function (people) {
                                    var panel = Ext.getCmp(id).items.items.find(i => i.name === name);
                                    panel.items.items.find(i => i.name === "ChairDirectorContainer").items.items.find(i => i.name === "ChairDirector").setValue(people.ShortName);
                                }, vm);
                                modal.show();
                            }
                        }
                    ]
                },
                {
                    xtype: 'container',
                    name: 'DateChairContainer',
                    layout: { type: 'hbox' },
                    items: [{
                        xtype: 'datefield',
                        fieldLabel: 'Дата',
                        labelWidth: 200,
                        name: 'DateChair',
                        value: dataBlock !== null ? dataBlock.DateChair : "",
                        submitFormat: 'd.m.Y',
                        format: 'd.m.Y',
                        startDay: 1
                    }]
                },
                {
                    xtype: 'label',
                    margin: '20 10 0 0',
                    style: { 'font-weight': 'bold' },
                    text: 'Согласовано'
                },
                {
                    xtype: 'textfield',
                    fieldLabel: 'Предприятие (организация)',
                    name: 'Company',
                    value: dataBlock !== null ? dataBlock.Company : ""
                },
                {
                    xtype: 'textfield',
                    fieldLabel: 'Руководитель',
                    name: 'CompanyDirector',
                    value: dataBlock !== null ? dataBlock.CompanyDirector : ""
                },
                {
                    xtype: 'container',
                    layout: { type: 'hbox' },
                    name: 'DateCompanyContainer',
                    items: [{
                        xtype: 'datefield',
                        fieldLabel: 'Дата',
                        labelWidth: 200,
                        name: 'DateCompany',
                        value: dataBlock !== null ? dataBlock.DateCompany : "",
                        submitFormat: 'd.m.Y',
                        format: 'd.m.Y',
                        startDay: 1
                    }]
                },
                {
                    xtype: 'label',
                    margin: '20 0 0 0',
                    style: { 'font-weight': 'bold' },
                    text: 'В составе'
                },
                {
                    xtype: 'list',
                    plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
                        clicksToEdit: 1
                    })],
                    header: false,
                    loadMask: true,
                    name: 'Experts',
                    store: {
                        data: dataBlock !== null ? Ext.clone(dataBlock.Experts) : []
                    },
                    tbar: [
                        {
                        xtype: 'button',
                        text: 'Добавить',
                        handler: function (btn) {
                            btn.up('grid').getStore().add({
                                FullName: null,
                                Post: null
                            });
                        }
                    }],
                    columns: [
                        {
                            xtype: 'rownumberer',
                            width: 50
                        },
                        {
                            header: 'ФИО',
                            dataIndex: 'FullName',
                            width: 200,
                            editor: {
                                xtype: 'textfield'
                            },
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Должность',
                            dataIndex: 'Post',
                            width: 200,
                            editor: {
                                xtype: 'textfield'
                            },
                            renderer: Ext.util.Format.htmlEncode
                        }
                    ]
                },
                {
                    xtype: 'label',
                    margin: '20 0 0 0',
                    style: { 'font-weight': 'bold' },
                    text: 'Приложение к Акту согласования'
                },
                {
                    xtype: 'container',
                    name: 'DateOfApprovalActContainer',
                    margin: '0 0 10 0',
                    layout: { type: 'hbox' },
                    items: [{
                        xtype: 'datefield',
                        fieldLabel: 'Дата',
                        labelWidth: 200,
                        name: 'DateOfApprovalAct',
                        value: dataBlock !== null ? dataBlock.DateOfApprovalAct : "",
                        submitFormat: 'd.m.Y',
                        format: 'd.m.Y',
                        startDay: 1
                    }]
                },
                {
                    xtype: 'list',
                    name: 'VariantInfos',
                    store: {
                        data: variantInfosData
                    },
                    margin: '20 0 0 0',
                    tbar: [
                        {
                            fieldLabel: "Траектория",
                            itemId: "variant",
                            xtype: "combobox",
                            store: Ext.create("Ext.data.Store",
                                {
                                    data: dataVariants
                                }),
                            valueField: 'Id',
                            displayField: 'Name',
                            queryMode: 'local',
                            anyMatch: true,
                            width: 300,
                            labelWidth: 90,
                            listeners: {
                                change: function (t, newValue, oldValue, eOpts) {
                                    filterStore(t.up().up().getStore(), newValue, t.up().getComponent('standard').getValue());
                                }
                            }
                        },
                        {
                            fieldLabel: "Проф. стандарт",
                            itemId: "standard",
                            xtype: "combobox",
                            store: Ext.create("Ext.data.Store",
                                {
                                    data: dataProfStandards
                                }),
                            valueField: 'Code',
                            displayField: 'Code',
                            queryMode: 'local',
                            anyMatch: true,
                            width: 300,
                            labelWidth: 70,
                            tpl: Ext.create('Ext.XTemplate',
                                '<tpl for=".">',
                                '<div class="x-boundlist-item" style="border-bottom:1px solid #f0f0f0;">',
                                '<div><b>{Code}</b> {Title}</div>' +
                                '</div>',
                                '</tpl>'
                            ),
                            listeners: {
                                change: function (t, newValue, oldValue, eOpts) {
                                    filterStore(t.up().up().getStore(), t.up().getComponent('variant').getValue(), newValue);
                                }
                            }
                        },
                        '->',
                        {
                            xtype: 'button',
                            text: 'Обновить таблицу (из данных Таблиц 1,4)',
                            handler: function (btn) {
                                var setData = function () {
                                    btn.up().up().getStore().setData(getVariantInfosData(null));
                                };

                                Ext.MessageBox.show({
                                    title: 'Информационное сообщение',
                                    msg: "Отменить все изменения в таблице?",
                                    buttons: Ext.MessageBox.YESNO,
                                    fn: function (btn) {
                                        if (btn === 'yes') {
                                            setData();
                                        }
                                    }
                                });
                            }
                        }
                    ],
                    columns: [
                        {
                            header: 'Наименование и код траектории ОП',
                            dataIndex: 'Name',
                            width: 200,
                            cellWrap: true,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Область и вид профессиональной деятельности',
                            width: 200,
                            cellWrap: true,
                            renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {

                                value = '';
                                if (record.data.NoProfStandard)
                                    value += `${Ext.String.htmlEncode(record.data.AreaTitle)}`;
                                else
                                    value += `${Ext.String.htmlEncode(record.data.AreaCode)} - ${Ext.String.htmlEncode(record.data.AreaTitle)}. 
                                                    ${Ext.String.htmlEncode(record.data.KindCode)} - ${Ext.String.htmlEncode(record.data.KindTitle)}`;

                                return value;
                            }
                        },
                        {
                            header: 'Объекты профессиональной деятельности',
                            dataIndex: 'ProfObjects',
                            width: 200,
                            editor: {
                                xtype: 'textfield'
                            },
                            cellWrap: true,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Тип (типы) задач профессиональной деятельности',
                            dataIndex: 'ProfTaskTypes',
                            width: 200,
                            cellWrap: true,
                            renderer: Ext.util.Format.htmlEncode
                        },
                        {
                            header: 'Профессиональные компетенции, формируемые в рамках образовательной траектории',
                            dataIndex: 'Competences',
                            width: 300,
                            cellWrap: true,
                            renderer: function (value, metaData) {

                                var str = "";
                                value.forEach(function (val, index, arr) {
                                    str += val.Code + ' - ' + val.Content + '; ';
                                });
                                
                                value = Ext.String.htmlEncode(str);
                                metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                                return value;
                            }
                        },
                        {
                            xtype: 'actioncolumn',
                            resizable: false,
                            sortable: false,
                            width: 30,
                            items: [{
                                icon: '/Content/Images/edit.png',
                                tooltip: 'Редактировать компетенции',
                                handler: function (grid, rowIndex, colIndex, item, e, record) {

                                    var storeData = data.Variants.VariantInfos[record.data.VariantIndex].
                                        ProfActivityRows[record.data.RowIndex].Competences;

                                    var wnd = profCompetencesWnd(record.data.Name, record.data.ProfTaskType, record, storeData, grid, rowIndex);
                                    wnd.show();
                                }
                            }]
                        }
                    ]
                },

                {
                    xtype: 'container',
                    layout: { type: 'hbox' },
                    items: [

                        {
                            xtype: 'button',
                            text: 'Скачать шаблон',
                            margin: '10 0 0 10',
                            width: 200,
                            handler: function (btn) {
                                var actData = getApprovalActData(btn.up().up());

                                actData.Chair = data.Chair.Name;
                                actData.Institute = data.Institute.Name;
                                actData.EducationLevelGenitive = data.EduProgramInfo.EducationLevelGenitive;
                                actData.ProfileCode = data.Profile.Code;
                                actData.ProfileName = data.Profile.Name;

                                actData.DateChair = actData.DateChair !== null ? actData.DateChair.toLocaleDateString() : null;
                                actData.DateCompany = actData.DateCompany !== null ? actData.DateCompany.toLocaleDateString() : null;
                                actData.DateOfApprovalAct = actData.DateOfApprovalAct !== null ? actData.DateOfApprovalAct.toLocaleDateString() : null;

                                var request = new XMLHttpRequest();
                                var url = '/BasicCharacteristicOP/GetApprovalAct/';
                                var params = "act=" + Ext.util.JSON.encode(actData);

                                request.open("POST", url, true);
                                request.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                                request.responseType = 'blob';

                                request.onload = function () {
                                    if (this.status === 200) {
                                        var filename = "Акт согласования " + actData.NumberOfApprovalAct + ".docx"; // название на русском не поддерживается, если возвращать название с сервера, поэтому пока так
                                        var type = request.getResponseHeader('Content-Type');
                                        
                                        var blob = new Blob([this.response], { type: type });
                                        if (typeof window.navigator.msSaveBlob !== 'undefined') {
                                            // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
                                            window.navigator.msSaveBlob(blob, filename);
                                        } else {
                                            var URL = window.URL || window.webkitURL;
                                            var downloadUrl = URL.createObjectURL(blob);

                                            if (filename) {
                                                // use HTML5 a[download] attribute to specify filename
                                                var a = document.createElement("a");
                                                // safari doesn't support this yet
                                                if (typeof a.download === 'undefined') {
                                                    window.location = downloadUrl;
                                                } else {
                                                    a.href = downloadUrl;
                                                    a.download = filename;
                                                    document.body.appendChild(a);
                                                    a.click();
                                                }
                                            } else {
                                                window.location = downloadUrl;
                                            }

                                            setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100);
                                        }
                                    }
                                };

                                request.send(params);
                            }
                        },
                        {
                            xtype: 'button',
                            text: 'Загрузить скан',
                            margin: '10 0 0 10',
                            width: 200,
                            handler: function (btn) {
                                var wnd = fileWnd(fileId, btn);
                                wnd.show();
                            }
                        },
                        Ext.create('Ext.Component', {
                            name: "LoadFileLink",
                            margin: '15 0 0 10',
                            html: '<a href="#">' + fileName + '</a>',
                            listeners: {
                                'click': loadFileFunction(fileId),
                                // name of the component property which refers to the element to add the listener to
                                element: 'el',
                                // css selector to filter the target element
                                delegate: 'a'
                            }
                        }),
                        {
                            xtype: 'button',
                            text: 'Удалить',
                            margin: '10 0 0 10',
                            name: 'RemoveFileButton',
                            hidden: fileId === null,
                            handler: function (btn) {
                                var panelItems = btn.up().up().items.items;
                                var panelFileItems = btn.up().items.items;
                                
                                var remove = function () {
                                    panelItems.find(i => i.name === "FileId").setValue(null);
                                    panelItems.find(i => i.name === "FileName").setValue("");

                                    panelFileItems.find(i => i.name === "LoadFileLink").setHidden(true);
                                    panelFileItems.find(i => i.name === "RemoveFileButton").setHidden(true);
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
                        }
                    ]
                }

            ],
            
            listeners: {
                beforeclose: function (panel, eOpts) {
                    if (panel.closeMe) {
                        panel.closeMe = false;
                        return true;
                    }
                    
                    Ext.MessageBox.show({
                        title: 'Информационное сообщение',
                        msg: "Удалить Акт согласования?",
                        buttons: Ext.MessageBox.YESNO,
                        fn: function (btn) {
                            if (btn === 'yes') {
                                panel.closeMe = true;
                                panel.close();
                            }
                        }
                    });

                    return false;
                }
            }
        };
    };
    
    var approvalActs = function () {
        
        var items = [];

        items = data.ApprovalActs.map(function (data, index) {
            return approvalActBlock(data, index + 1);
        });

        return {
            id: id,
            name: 'ApprovalActs',
            items: items,
            contentReader: function (content, vm) {
             
                content = [];
                Ext.getCmp(id).items.items.forEach(function (item, index, array) {
                    content.push(getApprovalActData(item));
                });

                return content;
            }
        };
    };

    return {
        viewModel: {
            stores: {
                employees: {
                    autoLoad: true,
                    proxy: {
                        type: 'ajax',
                        url: '/WorkingPrograms/GetAuthors/' + window.location.search,
                        reader: { type: 'json' },
                        extraParams: {
                            documentType: documentType,
                            documentId: documentId
                        }
                    }
                }
            }
        },
        items: [
            {
                name: 'Chair',
                hidden: true
            },
            approvalActs(),
            {
                xtype: 'button',
                margin: '0 0 20 0',
                width: 300,
                text: 'Добавить Акт согласования',
                handler: function (btn) {
                    Ext.getCmp(id).setHidden(false);
                    Ext.getCmp(id).add(approvalActBlock(null, 1));
                }
            }
        ]
    };

    function createPeopleWindow(selectHandler, rootVm) {
        var modal = Ext.create('Ext.window.Window', {
            viewModel: {
                data: {
                    FirstName: null,
                    MiddleName: null,
                    LastName: null,
                    AcademicDegree: null,
                    AcademicTitle: null,
                    Post: null,
                    Workplace: null
                },
                showMessage: rootVm.showMessage
            },
            title: 'Авторы',
            closeAction: 'close',
            modal: true,
            autoHeight: true,
            width: 400,
            layout: 'fit',
            items: {
                xtype: 'form',
                bodyPadding: 10,
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                items: [{
                    fieldLabel: 'Фамилия',
                    xtype: 'textfield',
                    bind: '{LastName}'
                }, {
                    fieldLabel: 'Имя',
                    xtype: 'textfield',
                    bind: '{FirstName}'
                }, {
                    fieldLabel: 'Отчество',
                    xtype: 'textfield',
                    bind: '{MiddleName}'
                }, {
                    fieldLabel: 'Ученая степень',
                    xtype: 'textfield',
                    bind: '{AcademicDegree}'
                }, {
                    fieldLabel: 'Ученое звание',
                    xtype: 'textfield',
                    bind: '{AcademicTitle}'
                }, {
                    fieldLabel: 'Должность',
                    xtype: 'textfield',
                    bind: '{Post}'
                }, {
                    fieldLabel: 'Кафедра',
                    xtype: 'textfield',
                    bind: '{Workplace}'
                }],
                buttons: [{
                    xtype: 'button',
                    text: 'Добавить',
                    handler: function (btn) {
                        var vm = findRootVm(btn);
                        Ext.Ajax.request({
                            url: '/WorkingPrograms/CreateAuthor/' + window.location.search,
                            jsonData: vm.getData(),
                            success: function (response) {
                                var author = JSON.parse(response.responseText);
                                selectHandler(author);
                                btn.up('window').close();
                            },
                            failure: function (xhr) {
                                vm.showMessage(xhr.responseText);
                            }
                        });
                    }
                }, {
                    xtype: 'button',
                    text: 'Отмена',
                    handler: function (btn) {
                        btn.up('window').close();
                    }
                }]
            }
        });
        return modal;
    }

    function createPeopleSelectorWindow(selectHandler, vm, selectorStoreName) {
        var modal = Ext.create('Ext.window.Window', {
            viewModel: vm,
            title: 'Добавление зав.кафедрой/руководителя дипартамента',
            closeAction: 'close',
            modal: true,
            height: 700,
            width: 828,
            layout: 'fit',
            listeners: {
                close: function () {
                    this.lookupViewModel().get(selectorStoreName).clearFilter();
                }
            },
            items: [{
                xtype: 'grid',
                loadMask: true,
                reference: 'authorsGrid',
                bind: {
                    store: '{' + selectorStoreName + '}'
                },
                tbar: [{
                    xtype: 'textfield',
                    fieldLabel: 'Поиск',
                    name: 'query',
                    listeners: {
                        change: function (textfield) {
                            var store = this.lookupViewModel().get(selectorStoreName);
                            store.clearFilter();
                            var searchText = textfield.getValue();
                            if (searchText !== "") {
                                store.filterBy(function (item) {
                                    var tester = new RegExp(Ext.String.escapeRegex(searchText), 'i');

                                    var fioMatch = tester.test(item.data.Fio);
                                    var degreeMatch = tester.test(item.data.Degree);
                                    var postMatch = tester.test(item.data.Post);
                                    var cathedraMatch = tester.test(item.data.Cathedra);

                                    return fioMatch || degreeMatch || postMatch || cathedraMatch;
                                });
                            }
                        }
                    },
                    width: 500
                }],
                columns: [{
                    header: 'ФИО',
                    dataIndex: 'Fio',
                    width: 250,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Ученая степень/ученое звание',
                    dataIndex: 'Degree',
                    width: 150,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Должность',
                    dataIndex: 'Post',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
                }, {
                    header: 'Кафедра',
                    dataIndex: 'Cathedra',
                    width: 200,
                    renderer: Ext.util.Format.htmlEncode
                }],
                bbar: ['->', {
                    width: 100,
                    text: 'Выбрать',
                    bind: {
                        disabled: '{!authorsGrid.selection}'
                    },
                    handler: function (btn) {
                        var selection = btn.up('grid').getSelection();
                        selectHandler(selection);
                        btn.up('window').close();
                    }
                }, {
                        width: 100,
                        text: 'Отмена',
                        handler: function (btn) {
                            btn.up('window').close();
                        }
                    }]
            }]
        });

        return modal;
    }
}