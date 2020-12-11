function uiInit(documentId, documentType, data, schema, emptyData) {
    var captionStyle = { fontWeight: 'bold' };
    var italicStyle = { fontStyle: 'italic' };

    function getLanguageString(language){
        return `Программа ${data.EduProgramInfo.EducationLevelGenitive} реализуется ${language}`;
    };

    return {
        viewModel: {
            stores: {

            }
        },

        items: [
            {
                title: `1.1. Общая характеристика основной образовательной программы`,
                name: 'CommonCharacteristic',
                contentReader: function (content, vm) {
                    // по умолчанию значение null - в таком случае подставляется текст (в CommonCharacteristicLoader.cs)
                    // если значение ставим пустую строкй - значит, пользователь оставил поле пустым и текст по умолчанию вставлять не нужно
                    if (content === '')
                        content = " ";
                    return content;
                },
                items: [
                    {
                        xtype: 'label',
                        text: `Общая характеристика основной образовательной программы ${data.EduProgramInfo.EducationLevelGenitive} 
                                    "${data.Profile.Code} ${data.Profile.Name}" разработана на основе образовательного
                                    стандарта Уральского федерального университета (${data.Direction.Standard} УрФУ) в области образования "${data.Direction.AreaEducationTitle}".`
                    },
                    {
                        xtype: 'label',
                        text: `[данный абзац заполняется в случае разработки ОП по сетевому договору, указывается полное наименование сетевой организации, 
                                    с которой заключен договор на совместную реализацию ОП, в ином случае абзац удаляется]`,
                        style: italicStyle
                    },
                    {
                        xtype: 'textarea',
                        height: 150,
                        name: 'CommonCharacteristic',
                        bind: '{CommonCharacteristic}'
                    },
                    {
                        xtype: 'label',
                        text: `Основная образовательная программа реализуется в институте/департаменте "${data.Institute.Name}" Уральского федерального университета.`
                    }
                ]
            },
            {
                title: '1.2. Назначение и особенность образовательной программы ',
                name: 'PurposeAndFeature',
                items: {
                    xtype: 'textarea',
                    height: 300,
                    name: 'PurposeAndFeature',
                    bind: '{PurposeAndFeature}'
                }
            },
            {
                title: '1.3. Форма обучения и срок освоения образовательной программы',
                name: 'FormAndDuration',
                items: [
                    {
                        xtype: 'container',
                        margin: '5 0 5 0',
                        items: {
                            xtype: 'button',
                            text: 'Обновить',
                            cls: 'btn-text-color x-btn-default-toolbar-small',
                            style: {
                                borderColor: '#157fcc'
                            },
                            handler: function (btn) {
                                var vm = findRootVm(this);
                                var thisBlock = this.up().up();
                                thisBlock.mask('Обновление');
                                Ext.Ajax.request({
                                    method: 'GET',
                                    url: '/BasicCharacteristicOP/FormAndDuration',
                                    params: {
                                        id: data.EduProgramInfo.Id
                                    },
                                    success: function (response) {
                                        thisBlock.unmask();
                                        try {
                                            var data = Ext.decode(response.responseText);
                                            vm.data.FormAndDuration = data.formAndDuration;
                                            thisBlock.items.items[1].setValue(data.formAndDuration.Text);
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
                        }
                    },
                    {
                        xtype: 'textarea',
                        editable: false,
                        height: 150,
                        bind: '{FormAndDuration.Text}',
                        autoHeight: true
                    }
                ]
            },
            {
                title: '1.4. Технология образовательной программы',
                name: 'Elearning',
                items: {
                    xtype: 'label',
                    text: `Образовательная программа реализуется с применением электронного обучения (дистанционных образовательных технологий). 
                        При применении электронного обучения (дистанционных образовательных технологий) предусматривается возможность 
                        приема-передачи информации в формах, доступных для инвалидов и лиц с ограниченными возможностями здоровья.`
                },
                buttons: []
            },
            {
                title: '1.5. Объем программы',
                name: 'ProgramSize',
                items: {
                    xtype: 'label',
                    editable: false,
                    text: `Объем программы ${data.EduProgramInfo.EducationLevelGenitive} для всех форм обучения 
                        составляет ${data.ModuleStructure.Sum} зачетных единиц (далее з.е.) вне зависимости от применяемых образовательных технологий, 
                        реализации программы с использованием сетевой формы, реализации программы по индивидуальному учебному плану. 
                        Объем образовательной программы, реализуемый за один учебный год, вне зависимости от формы обучения, 
                        применяемых образовательных технологий, реализации программы с использованием сетевой формы, 
                        реализации программы по индивидуальному учебному плану составляет не более 70 з.е., при ускоренном обучении – не более 80 з.е.`
                },
                buttons: []
            },
            {
                title: '1.6. Язык реализации образовательной программы',
                name: 'Language',
                items: [
                    {
                        xtype: 'container',
                        margin: '5 0 5 0',
                        items: {
                            xtype: 'button',
                            text: 'Обновить',
                            cls: 'btn-text-color x-btn-default-toolbar-small',
                            style: {
                                borderColor: '#157fcc'
                            },
                            handler: function (btn) {
                                var vm = findRootVm(this);
                                var thisBlock = this.up().up();
                                thisBlock.mask('Обновление');
                                Ext.Ajax.request({
                                    method: 'GET',
                                    url: '/BasicCharacteristicOP/Language',
                                    params: {
                                        id: data.EduProgramInfo.Id
                                    },
                                    success: function (response) {
                                        thisBlock.unmask();
                                        try {
                                            var data = Ext.decode(response.responseText);
                                            vm.data.Language = data.language;
                                            thisBlock.items.items[1].setValue(getLanguageString(data.language));
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
                        }
                    },
                    {
                        xtype: 'textarea',
                        value: getLanguageString(data.Language)
                    }
                ]
            }
        ]
    };
}