var testData = [{
    "label": "BTC",
    "value": 1.2,
    "amount": 12
}, {
    "label": "ETH",
    "value": 0.8,
    "amount": 13
}, {
    "label": "EUR",
    "value": 0.2,
    "amount": 14
}, {
    "label": "USD",
    "value": 0.5,
    "amount": 15
}];

function display(title, subtitle, data, currency) {

    var div = document.getElementById("pieChart");

    var labelFormatter = function(context) {
        if (context.section == "outer" && context.part == "value") {
            return "\u2248 " + context.value + " " + currency;
        }
        if (context.section == "outer" && context.part == "mainLabel") {
            return data[context.index]["amount"] + " " + context.label;
        }
        return data[context.index]["label"];
    };

    var clickedListener = function(index) {
        alert(JSON.stringify(index));
    };

    var pie = new d3pie("pieChart", {
        "header": {
            "title": {
                "text": title,
                "color": "#333333",
                "fontSize": 24,
                "font": "courier"
            },
            "subtitle": {
                "text": subtitle,
                "color": "#666666",
                "fontSize": 18,
                "font": "courier"
            },
            "location": "pie-center"
        },
        "size": {
            "canvasHeight": div.offsetWidth,
            "canvasWidth": div.offsetWidth,
            "pieInnerRadius": "67%",
            "pieOuterRadius": "70%"
        },
        "data": {
            "sortOrder": "none",
            "smallSegmentGrouping": {
                "enabled": false
            },
            "content": data
        },
        "labels": {
            "formatter": labelFormatter,
            "outer": {
                "format": "label-value2",
                "pieDistance": 20
            },
            "inner": {
                "format": "percentage",
            },
            "mainLabel": {
                "color": "#333333",
                "font": "arial",
                "fontSize": 10
            },
            "percentage": {
                "color": "#fff",
                "font": "arial",
                "fontSize": 18,
                "decimalPlaces": 0
            },
            "value": {
                "color": "#999",
                "font": "arial",
                "fontSize": 10
            },
            "lines": {
                "enabled": true,
                "style": "curved",
                "color": "segment"
            }
        },
        "effects": {
            "load": {
                "effect": "none", // none / default
            },
            "pullOutSegmentOnClick": {
                "effect": "none", // none / linear / bounce / elastic / back
                "speed": 0,
                "size": 0
            },
            "highlightSegmentOnMouseover": true,
            "highlightLuminosity": -0.2
        },
        "callbacks": {
            "onClickSegment": clickedListener
        }
    });
}
