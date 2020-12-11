
var Urfu = Urfu || {};

Urfu.renders = Urfu.renders || {};

Urfu.createViewport = function (layout, items, mainPanelSettings) {
    var navbar = Ext.get('navbar');
    var body = Ext.getBody();

    function getY() {
        return navbar.getHeight();
    }

    function getWidth() {
        return body.getViewSize().width;
    }

    function getHeight() {
        return body.getViewSize().height - navbar.getHeight();
    }

    var settings =
    {
        title: appSettings.title,
        //padding: 15,
        layout: layout,
        y: getY(),
        width: getWidth(),
        height: getHeight(),
        renderTo: Ext.getBody(),
        items: items,
    }
    if (mainPanelSettings) {
        for (var propertyName in mainPanelSettings) {
            if (mainPanelSettings.hasOwnProperty(propertyName)) {
                settings[propertyName] = mainPanelSettings[propertyName];
            }
        }
    }
    settings['items'] = items;

    var panel = Ext.create('Ext.panel.Panel', settings);
    Urfu.panel = panel;


    Ext.on('resize', function () {
        panel.setY(navbar.getHeight());
        panel.setSize(getWidth(), getHeight());
    });
};

Urfu.VariantState = { 0: "Формируется", 1: "На согласовании", 2: "Утверждён" };
Urfu.ProgramState = { "Формируется": "Формируется", "На согласовании": "На согласовании", "Утверждена": "Утверждена" };


Urfu.renders.htmlEncodeWithToolTip = function (value, metaData) {
    // Sample value: msimms & Co. "like" putting <code> tags around your code

    value = Ext.String.htmlEncode(value);

    // "double-encode" before adding it as a data-qtip attribute
    metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';

    return value;
};

Urfu.parseJson = function (json) {
    var data = JSON.parse(json.replace(/&quot;/g, '"'));
    return data;
}


