var datesContainer = {
    xtype: 'fieldcontainer',
    defaultType: 'datefield',
    layout: {
        type: 'hbox',
        align: 'stretch'
    },
    items: [
        {
            xtype: 'label',
            text: '',
            width: 0
        },
        {
            xtype: 'datefield',
            fieldLabel: 'с',
            name:'BeginDate',
            labelWidth: 25,
            width: 215,
            format: 'd.m.Y',
            startDay: 1,
            margin: '0 0 0 155',
            value: '@string.Format("{0:dd.MM.yyyy}", Model.ExternalStartDate)'
        },
        {
            xtype: 'datefield',
            margin: '0 25 0 25',
            fieldLabel: 'по',
            name: 'EndDate',
            labelWidth: 25,
            width: 215,
            format: 'd.m.Y',
            startDay: 1,
            value: '@string.Format("{0:dd.MM.yyyy}", Model.ExternalFinishDate)'
        },
        {
            xtype: 'button',
            width: 100,
            text: 'Удалить'
        }
    ]
};

var addDatesContainer = function (containerId) {
    var length = Ext.getCmp(containerId).items.items.length;
    datesContainer.id = containerId + length;
    Ext.getCmp(containerId).add(datesContainer);
    Ext.getCmp(containerId).items.items[length].items.items[3].setHandler(function () {
        Ext.getCmp(containerId).remove(Ext.getCmp(this.config.$initParent.id));
    });
};

Ext.define('Ext.container.BigDatesContainer', {
    extend: 'Ext.container.Container',
    xtype: 'bigDatesContainer',
    layout: {
        type: 'vbox',
        align: 'stretch',
        margin: '0 0 0 180'
    },
    items: [
        {
            xtype: 'fieldcontainer',
            defaultType: 'datefield',
            layout: {
                type: 'hbox',
                align: 'stretch'
            },
            items: [
                {
                    xtype: 'label',
                    text: 'Сроки',
                    width: 155,
                    style: { fontWeight: 'normal' }
                },
                {
                    xtype: 'datefield',
                    fieldLabel: 'с',
                    name: 'BeginDate',
                    labelWidth: 25,
                    width: 215,
                    format: 'd.m.Y',
                    startDay: 1,
                    value: '@string.Format("{0:dd.MM.yyyy}", Model.ExternalStartDate)'
                },
                {
                    xtype: 'datefield',
                    margin: '0 25 0 25',
                    fieldLabel: 'по',
                    name: 'EndDate',
                    labelWidth: 25,
                    width: 215,
                    format: 'd.m.Y',
                    startDay: 1,
                    value: '@string.Format("{0:dd.MM.yyyy}", Model.ExternalFinishDate)'
                },
                {
                    xtype: 'button',
                    text: 'Добавить',
                    width: 100
                }
            ]
        },
    ]
});

var datesContainerDsId = 'datesContainerDs';
var datesContainerDs = Ext.create('Ext.container.BigDatesContainer', {
    requires: [
        'bigDatesContainer'
    ],
    id: datesContainerDsId,
    bind: {
        disabled: '{IsAdmitted}'
    },
    listeners: {
        beforerender: 'beforerenderDates'
    }
});

Ext.define('Practice.view.ContractDsPanel', {
    extend: 'Ext.form.Panel',
    xtype: 'contractDsPanel',
    title: 'Информация о предприятии',
    collapsible: true,
    padding: '5px',
    bodyPadding: '10px',
    //border: true,
    //frame: true,
    defaults: {
        width: '100%',
        cls: 'field-margin',
        labelWidth: 180
    },
    defaultListenerScope: true,

    viewModel: {
        //formulas: {
        //    statusName: function (get) {
        //        var status = get('status');
        //        if (status === 1) return 'Согласовано';
        //        if (status === 2) return 'Отказано';
        //        return null;
        //    }
        //}
    
    },

    config: {
        contract: null,
        admission: null,
    },
    items: [{
        xtype: 'hidden',
        name: 'practiceId',
        bind: '{contract.practiceId}'
    }, {
        xtype: 'hidden',
        name: 'limit',
        bind: '{contract.limit}'
    }, {
        xtype: 'combobox',
        itemId: 'ContractsDsData',
        anyMatch: true,
        queryMode: 'local',
        allowBlank: false,
        valueField: 'contractId',
        displayField: 'name',
        fieldLabel: 'Название',
        name: 'contractId',
        readOnlyCls: 'x-item-disabled',
        bind: {
            value: '{contract.contractId}',
            store: '{dsContracts}',
            readOnly: '{IsAdmitted}'
        },
        listeners: { select: 'selectCompany' }
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Адрес',
        name: 'address',
        bind: {
            value: '{contract.address}',
            disabled: '{IsAdmitted}'
        }
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Телефон',
        name: 'phone',
        bind: {
            value: '{contract.phone}',
            disabled: '{IsAdmitted}'
        }
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Сайт',
        name:'site',
        bind: {
            value: '{contract.site}',
            disabled: '{IsAdmitted}'
        }
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Ответственный от предприятия',
        name:'personInChargeInfo',
        bind: {
            value: '{contract.personInChargeInfo}',
            disabled: '{IsAdmitted}'
        }
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Номер договора',
        name:'contractNumber',
        bind: {
            value: '{contract.contractNumber}',
            disabled: '{IsAdmitted}'
        }
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Вид деятельности отдела прохождения практики',
            name: 'divisionDescription',
            bind: {
                value: '{contract.divisionDescription}',
                hidden: '{!contract.divisionDescription}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'displayfield',
            fieldLabel: 'Дополнительные условия прохождения практики',
            name: 'additionalTerms',
            bind: {
                value: '{contract.additionalTerms}',
                hidden: '{!contract.additionalTerms}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'component',
            margin: '0 0 12 0',
            bind: {
                hidden: '{!contract.fileId}',
                html: 'Дополнительная информация об условиях прохождения практики:  <a href="/PracticeGroup/DownloadContractPeriodDocument?id={contract.fileId}">{contract.fileName}</a>',
            }
        },
        datesContainerDs,
    {
        xtype: 'radiogroup',
        fieldLabel: 'Статус',
        itemId: 'status',
        name: 'status',
        allowBlank: false,
        msgTarget:'under',
        blankText:'Укажите статус заявки',
        bind: {
            value: {
                status: '{contract.status}'
            }
        },
        listeners: { change: 'selectStatus' },
        layout: 'hbox',
        items: [
            {
                boxLabel: 'Согласовано',
                name: 'status',
                inputValue: 1,
                margin: '0 20 0 0'
            }, {
                boxLabel: 'Отклонено',
                name: 'status',
                inputValue: 2,
                margin: '0 20 0 0'
            }, {
                boxLabel: 'На формировании',
                name: 'status',
                inputValue: 0
            }
        ]
    }, {
        xtype: 'textfield',
        name: 'reasonOfDeny',
        itemId: 'reasonOfDenyDs',
        fieldLabel: 'Причина отклонения',
        bind: {
            value: '{contract.reasonOfDeny}',
            disabled: '{contract.IsAdmitted}'
        }
    }
    ],

    buttons: [{
        text: 'Сохранить',
        listeners: { click: 'saveDogovorDs' }
    }
    ],

    selectCompany: function () {
        this.fireEvent('select', this);
    },

    selectStatus: function (t, newValue, oldValue, eOpts) {
        this.updateStatus(newValue.status);
    },

    saveDogovorDs: function () {
        if (!this.canEdit) {
            Ext.Msg.alert('Ошибка', 'У вас нет прав вносить изменения');
            return;
        }

        this.fireEvent('save', this);
    },

    resetStatus: function (status) {
        if (status === null)
            this.getComponent('status').reset();

    },
    updateStatus: function (status) {
        var vm = this.getViewModel();
        vm.set('contract.status', status);

        if (status === 2) {
            var reasonOfDenyfield = this.getComponent('reasonOfDenyDs');
            reasonOfDenyfield.setDisabled(false);
            reasonOfDenyfield.focus();
        }
        else {
            // блокируем поле Причина отказа
            this.getComponent('reasonOfDenyDs').setDisabled(true);
        }
    },

    updateAdmission: function (admission) {
        console.log('contractId ' + this.contract.contractID + ' updateAdmission ' + admission.contractId);

        if (admission.contractId > 0 && admission.contractId !== this.contract.contractID) {
            this.hide();
        } else {
            this.show();
        }

    },

    beforerenderDates: function () {
        this.fireEvent('renderDates', this);
    }
});


var countryStore = Ext.create("Ext.data.Store",
    {
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/PracticeGroup/Countries',
            reader: {
                type: 'json',
                rootProperty: 'data'
            }
        }
    });

var regionStore = Ext.create("Ext.data.Store",
    {
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/PracticeGroup/Regions',
            reader: {
                type: 'json',
                rootProperty: 'data'
            }
        }
    });

var cityStore = Ext.create("Ext.data.Store",
    {
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/PracticeGroup/Cities',
            reader: {
                type: 'json',
                rootProperty: 'data'
            }
        }
    });

function reloadRegionStore(countryId) {
    regionStore.proxy.setUrl('/PracticeGroup/Regions?countryId=' + countryId);
    regionStore.reload();
}

function reloadCityStore(regionId) {
    cityStore.proxy.setUrl('/PracticeGroup/Cities?regionId=' + regionId);
    cityStore.reload();
}

function selectCountry(id) {
    regionCmbx.setValue('');
    cityCmbx.setValue('');
    cityCmbx.setDisabled(true);
    reloadRegionStore(id);
    regionCmbx.setDisabled(false);
}

function selectRegion(id) {
    cityCmbx.setValue('');
    reloadCityStore(id);
    cityCmbx.setDisabled(false);
}

var countryCmbx = Ext.create('Ext.form.ComboBox',
    {
        xtype: 'combobox',
        fieldLabel: 'Страна',
        labelWidth: 180,
        width: 600,
        store: countryStore,
        name: 'countryId',
        readOnlyCls: 'x-text-readonly',
        validator: required,
        forceSelection:true,
        bind: {
            value: '{countryId}',
            disabled: '{IsAdmitted}',
            readOnly: '{companyId}'
        },
        valueField: 'CountryId',
        displayField: 'Country',
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            'select': function (combo, records, eOpts) {
                selectCountry(records.data.CountryId);
            },
            'blur': function (combo, event, eOpts) {
                if (combo.getValue() == null) {
                    regionCmbx.setValue('');
                    regionCmbx.setDisabled(true);
                    cityCmbx.setValue('');
                    cityCmbx.setDisabled(true);
                }
            }
        }
    });

var regionCmbx = Ext.create('Ext.form.ComboBox',
    {
        xtype: 'combobox',
        fieldLabel: 'Регион',
        labelWidth: 180,
        store: regionStore,
        width: 600,
        name: 'regionId',
        readOnlyCls: 'x-text-readonly',
        bind: {
            value: '{regionId}',
            disabled: '{IsAdmitted}',
            readOnly: '{companyId}'
        },
        valueField: 'RegionId',
        displayField: 'Region',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        validator: required,
        listeners: {
            'select': function (combo, records, eOpts) {
                selectRegion(records.data.RegionId);
            },
            'blur': function (combo, event, eOpts) {
                if (combo.getValue() == null) {
                    cityCmbx.setValue('');
                    cityCmbx.setDisabled(true);
                }
            }
        }
    });

var cityCmbx = Ext.create('Ext.form.ComboBox',
    {
        xtype: 'combobox',
        fieldLabel: 'Город',
        labelWidth: 180,
        store: cityStore,
        width: 600,
        name: 'cityId',
        readOnlyCls: 'x-text-readonly',
        forceSelection: true,
        validator: required,
        bind: {
            value: '{cityId}',
            disabled: '{IsAdmitted}',
            readOnly: '{companyId}'
        },
        valueField: 'CityId',
        displayField: 'City',
        queryMode: 'local',
        anyMatch: true
    });



Ext.define('Company', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'CompanyId', type: 'int' },
        { name: 'Name', type: 'string' },
    ]
});

var companyStore = Ext.create("Ext.data.Store",
    {
        model: 'Company',
        proxy: {
            type: 'ajax',
            url: '../PracticeGroup/GetCompany',
            reader: {
                type: 'json'
            }
        },
        autoLoad: {
            scope: this,
            callback: function () {                
                var comboBox = Ext.getCmp("companyNameField");
        
                //if (!window.practiceConfig || !window.practiceConfig.currentCompanyId)
                //    return;
                //comboBox.setReadOnly(true);
                comboBox.setValue(Ext.create('Company',
                    {
                        CompanyId: window.practiceConfig.currentCompanyId,
                        Name: window.practiceConfig.currentCompanyName
                    }));
            }
        }
    });

var companyNameCmbx = Ext.create('Ext.form.ComboBox',
    {
        xtype: 'combobox',
        id: 'companyNameField',
        name: 'companyId',
        fieldLabel: 'Наименование предприятия',
        store: companyStore,
        queryMode: 'remote',
        valueField: 'CompanyId',
        displayField: 'Name',
        queryParam: 'q',
        triggerAction: 'all',
        hideTrigger: true,
        minChars: 3,
        enableKeyEvents: true,
        readOnlyCls: 'x-text-readonly',
        allowBlank: false,
        validateOnChange: false,
        bind: {
            value: '{CompanyId}',
            disabled: '{IsAdmitted}',
            readOnly: '{companyId}'
        },
        listeners: {
            'select': function (combo, record, eOpts) {

                var contractKsPanel = combo.up();

                if (contractKsPanel !== null) {
                    getCompanyInfo(contractKsPanel.viewModel);
                }
            },
            'blur': function (t, e) {
                t.up().down('#companyName').setValue(t.rawValue);
            }
        }

    });

function getCompanyInfo(viewModel) {
    Ext.Ajax.request({
        //method: 'GET',
        dataType: 'json',
        url: '/PracticeGroup/GetCompanyInfo',
        params: {
            companyId: viewModel.data.CompanyId,
            practiceId: viewModel.data.practiceId
        },
        success: function (response) {
            var objAjax = Ext.decode(response.responseText);
            viewModel.setData(objAjax.ContractKs);
            countryCmbx.setValue(objAjax.ContractKs.countryId);
            reloadRegionStore(objAjax.ContractKs.countryId);
            reloadCityStore(objAjax.ContractKs.regionId);
            regionCmbx.setValue(objAjax.ContractKs.regionId);
            cityCmbx.setValue(objAjax.ContractKs.cityId);
        },
        failure: function (response, opt) {
            Ext.MessageBox.show({
                title: 'Ошибка',
                msg: 'Произошла ошибка',
                buttons: Ext.MessageBox.OK
            });
        }
    });
};

function required(value) {
    return Ext.isEmpty(value) ? "Поле не может быть пустым" : true;
};
var datesContainerKsId = 'datesContainerKs';
var datesContainerKs = Ext.create('Ext.container.BigDatesContainer', {
    requires: [
        'bigDatesContainer'
    ],
    id: datesContainerKsId,
    name: datesContainerKsId,
    bind: {
        disabled: '{IsAdmitted}'
    },
    listeners: {
        beforerender: 'beforerenderDates'
    }
});

Ext.define(null, {
    override: 'Ext.form.field.VTypes',
    email: function (value) {
        return /^(")?(?:[^\."])(?:(?:[\.])?(?:[\w\-!#$%&'*+\/=?\^_`{|}~А-Яа-я]))*\1@(\w[\-\w]*\.){1,5}([A-Za-z]){2,6}$/.test(value);
    }
});

Ext.define('Practice.view.ContractKsPanel', {
    extend: 'Ext.form.Panel',
    xtype: 'contractKsPanel',
    title:
        'Добавление информации о предприятии по к/с (если студент проходит практику на кафедре или в отделе УрФУ, то данный блок ЗАПОЛНЯТЬ НЕ НАДО)',
    collapsible: true,
    padding: '5px',
    bodyPadding: '10px',
    defaults: {
        width: '100%',
        cls: 'field-margin',
        labelWidth: 180
    },
    defaultListenerScope: true,
    config: {
        admission: null
    },

    items: [
        {
            xtype: 'hidden',
            name: 'practiceId',
            bind: '{practiceId}'
        },
        {
            xtype: 'hidden',
            name: 'contractId',
            bind: '{contractId}'
        },
        {
            xtype: 'hidden',
            name: 'company',
            id: 'companyName',
            bind: '{company}'
        },
        companyNameCmbx,
        {
            xtype: 'textfield',
            name: 'shortname',
            fieldLabel: 'Сокращенное название предприятия',
            readOnlyCls: 'x-text-readonly',
            bind: {
                value: '{shortname}',
                disabled: '{IsAdmitted}',
                readOnly: '{companyId}'
            }
        },
        {
            xtype: 'textfield',
            name: 'inn',
            fieldLabel: 'ИНН',
            regex: /^[0 -9]+$/,
            readOnlyCls: 'x-text-readonly',
            bind: {
                value: '{inn}',
                disabled: '{IsAdmitted}',
                readOnly: '{companyId}'
            }
        },
        {
            xtype: 'textfield',
            name: 'director',
            fieldLabel: 'Руководитель',
            regex:/^\D+$/,
            regexText:'Значение в этом поле не может содержать цифры',
            bind: {
                value: '{director}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'directorInitials',
            fieldLabel: 'Фамилия и инициалы (пр: Иванов И.И.)',
            regex: /^\D+$/,
            regexText: 'Значение в этом поле не может содержать цифры',
            bind: {
                value: '{directorInitials}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'directorGenetive',
            fieldLabel: 'ФИО руководителя в родительном падеже',
            regex: /^\D+$/,
            regexText: 'Значение в этом поле не может содержать цифры',
            bind: {
                value: '{directorGenetive}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'postOfDirector',
            fieldLabel: 'Должность руководителя предприятия',
            bind: {
                value: '{postOfDirector}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'postOfDirectorGenetive',
            fieldLabel: 'Должность руководителя в родительном падеже',
            bind: {
                value: '{postOfDirectorGenetive}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'personInCharge',
            fieldLabel: 'Ответственный от предприятия',
            regex: /^\D+$/,
            regexText: 'Значение в этом поле не может содержать цифры',
            bind: {
                value: '{personInCharge}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'personInChargeInitials',
            fieldLabel: 'Фамилия и инициалы ответственного (пр: Иванов И.И.)',
            regex: /^\D+$/,
            regexText: 'Значение в этом поле не может содержать цифры',
            bind: {
                value: '{personInChargeInitials}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'postPersonInCharge',
            fieldLabel: 'Должность ответственного лица',
            bind: {
                value: '{postPersonInCharge}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'phone',
            fieldLabel: 'Телефон ответственного лица',
            bind: {
                value: '{phone}',
                disabled: '{IsAdmitted}'
            }
        },
        {
            xtype: 'textfield',
            name: 'personInChargeEmail',
            fieldLabel: 'E-mail ответственного лица',
            vtype: 'email',
            bind: {
                value: '{personInChargeEmail}',
                disabled: '{IsAdmitted}',
            }
        },
        countryCmbx,
        regionCmbx,
        cityCmbx,
        {
            xtype: 'textfield',
            name: 'address',
            fieldLabel: 'Адрес предприятия',
            readOnlyCls: 'x-text-readonly',
            bind: {
                value: '{address}',
                disabled: '{IsAdmitted}',
                readOnly: '{companyId}'
            }
        },
        {
            xtype: 'textfield',
            name: 'companyPhone',
            fieldLabel: 'Телефон предприятия',
            readOnlyCls: 'x-text-readonly',
            bind: {
                value: '{companyPhone}',
                disabled: '{IsAdmitted}',
                readOnly: '{companyId}'
            }
        },
        {
            xtype: 'textfield',
            name: 'email',
            fieldLabel: 'E-mail предприятия',
            vtype: 'email',
            readOnlyCls: 'x-text-readonly',
            bind: {
                value: '{email}',
                disabled: '{IsAdmitted}',
                readOnly: '{companyId}'
            }
        },
        {
            xtype: 'textfield',
            name: 'site',
            fieldLabel: 'Сайт',
            readOnlyCls: 'x-text-readonly',
            bind: {
                value: '{site}',
                disabled: '{IsAdmitted}',
                readOnly: '{companyId}'
            }
        },
        {
            xtype: 'textfield',
            name: 'comment',
            fieldLabel: 'Комментарий',
            bind: {
                value: '{comment}',
                disabled: '{IsAdmitted}'
            }
        },
        datesContainerKs,
        {
            xtype: 'radiogroup',
            fieldLabel: 'Статус',
            name: 'StatusDogovorKs',
            itemId: 'StatusDogovorKs',
            bind: {
                value: {
                    StatusDogovorKs: '{status}'
                }
            },
            listeners: { change: 'selectStatus' },
            layout: 'hbox',
            items: [
                {
                    boxLabel: 'Согласовано',
                    name: 'StatusDogovorKs',
                    inputValue: '1',
                    margin: '0 20 0 0'
                }, {
                    boxLabel: 'Отклонено',
                    name: 'StatusDogovorKs',
                    inputValue: '2',
                    margin: '0 20 0 0'
                }, {
                    boxLabel: 'На формировании',
                    name: 'StatusDogovorKs',
                    inputValue: '0'
                }, {
                    boxLabel: '',
                    hidden: true,
                    name: 'StatusDogovorKs',
                    inputValue: '-1',
                    margin: '0 20 0 0'
                }
            ]
        }, {
            xtype: 'textfield',
            name: 'reasonOfDeny',
            itemId: 'reasonOfDenyKs',
            fieldLabel: 'Причина отклонения',
            bind: {
                value: '{reasonOfDeny}',
                disabled: '{IsAdmitted}'
            }
        }
    ],

    buttons: [
        {
            text: 'Добавить новое предприятие',
            id: 'addnewCompany',
            bind: {
                disabled: '{IsAdmitted}'
            },
            listeners: { click: 'addNewCompany' },
        },
        {
            text: 'Сохранить',
            listeners: { click: 'saveClick' }
        }
       
    ],

    saveClick: function () {
        if (!this.canEdit) {
            Ext.Msg.alert('Ошибка', 'У вас нет прав вносить изменения');
            return;
        }

        this.fireEvent('save', this);
    },

    addNewCompany: function () {
        if (!this.canEdit) {
            Ext.Msg.alert('Ошибка', 'У вас нет прав вносить изменения');
            return;
        }

        var ksPanel = this;
        Ext.Msg.confirm('',
            'Вы хотите удалить информацию о текущем предприятии по к/с? ',
            function(button) {
                if ('yes' == button) {
                    var fields = ksPanel.query('[isFormField][name !="practiceId"]');
                    fields.forEach(function(field) {
                            field.reset();
                        }
                    );
                    var readonlyitems = ksPanel.query('[readOnly =true]');
                    Ext.each(readonlyitems, function(item) { item.setReadOnly(false); });
                } 
            }
        );

    },
    selectStatus: function (t, newValue, oldValue, eOpts) {
        var vm = this.getViewModel();
        vm.set('status', newValue.StatusDogovorKs);

        var status = newValue.StatusDogovorKs;
        if (status == 2) {
            this.getComponent('reasonOfDenyKs').setDisabled(false);
        }
        else {
            // блокируем поле Причина отказа
            this.getComponent('reasonOfDenyKs').setDisabled(true);
        }
    },

    updateAdmission: function (admission) {
        console.log('contractId ' + this.contract.contractID + ' updateAdmission ' + admission.contractId);

        if (admission.contractId > 0 && admission.contractId !== this.contract.contractID) {
            this.hide();
        } else {
            this.show();
        }
    },

    beforerenderDates: function () {
        this.fireEvent('renderDates', this);
    }

});
