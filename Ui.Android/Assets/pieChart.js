var chartColors =
    [
        "#ba9043",
        "#ffc700",
        "#ed4040",
        "#ec8644",
        "#ab75f9",
        "#2dc4e6",
        "#62c366",
        "#c173d5",
        "#f2826d",
        "#6b9ae9",
        "#f26d75",
        "#6df2c2",
        "#6df2c2"
    ];


var chartOptions = {
    "header": {
        "title": {
            "color": "#444",
            "fontSize": 18,
            "font": "arial"
        },
        "subtitle": {
            "color": "#999",
            "fontSize": 14,
            "font": "arial"
        },
        "location": "pie-center",
        "titleSubtitlePadding": 2
    },
    "size": {
        "pieInnerRadius": "60%",
        "pieOuterRadius": "85%"
    },
    "data": {
        "sortOrder": "none",
        "smallSegmentGrouping": {
            "enabled": true,
            "value": 8,
            "valueType": "percentage"
        },
        "content": null
    },
    "labels": {
        "formatter": labelFormatter,
        "outer": {
            "format": "label-percentage2",
            "pieDistance": 15
        },
        "inner": {
            "format": "label-percentage2"
        },
        "mainLabel": {
            "color": "segment",
            "font": "arial",
            "fontSize": 14
        },
        "percentage": {
            "color": "#000",
            "font": "arial",
            "fontSize": 11,
            "decimalPlaces": 2
        },
        "value": {
            "color": "#fff",
            "font": "arial",
            "fontSize": 14
        },
        "lines": {
            "enabled": false,
            "style": "curved",
            "color": "segment"
        }
    },
    "effects": {
        "load": {
            "effect": "default", // none / default
            "speed": 500
        },
        "pullOutSegmentOnClick": {
            "effect": "none", // none / linear / bounce / elastic / back
            "speed": 0,
            "size": 0
        },
        "highlightSegmentOnMouseover": true,
        "highlightLuminosity": -0.2
    },
    "misc": {
        "colors": {
            "segmentStroke": "#fff",
            "segments": chartColors
        }
    },
    "callbacks": {
        "onClickSegment": clickedListener
    }
};

var _textAccounts;
var _textCurrencies;
var _textFurter;
var _currencyCode;
var _roundNumbers;
var _culture;

function showChart(data, textAccounts, textCurrencies, textFurther, textNoData, currencyCode, roundNumbers, culture) {

    _textAccounts = textAccounts;
    _textCurrencies = textCurrencies;
    _textFurter = textFurther;
    _currencyCode = currencyCode;
    _roundNumbers = roundNumbers;
    _culture = culture;

    $("div.overlay").remove();
    createMainGraph(data);

    $("#pieChart").resize(function () {
        createMainGraph(data);
    });
}

function createMainGraph(data) {

    var chartArea = document.getElementById("pieChart");
    while (chartArea.firstChild) {
        chartArea.removeChild(chartArea.firstChild);
    }

    chartOptions["data"]["content"] = data;
    chartOptions["data"]["smallSegmentGrouping"].enabled = groupData(data);

    var numAccounts = 0;
    var sum = 0;
    for (var i in data) {
        numAccounts += data[i]["accounts"].length;
        sum += data[i]["value"];
    }
    chartOptions["header"]["title"].text = formatNumber(sum);
    chartOptions["header"]["subtitle"].text = data.length === 1 ? _textCurrencies[0] : data.length + " " + _textCurrencies[1];

    var div = document.getElementById("pieChart");
    chartOptions["size"]["canvasWidth"] = div.offsetWidth;
    chartOptions["size"]["canvasHeight"] = div.offsetHeight;

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ConstructorCallNotUsed
    new d3pie("pieChart", chartOptions);
}

function showOverlay(id, callback) {
    var overlay = document.createElement("div");
    overlay.className = "overlay";
    overlay.id = id;
    var close = document.createElement("div");
    close.className = "close";
    close.onclick = function () {
        $("#" + id).toggle("scale", {
            direction: "both"
        },
            function () {
                document.getElementById("content").removeChild(document.getElementById(id));
            });
    };
    overlay.appendChild(close);
    document.getElementById("content").appendChild(overlay);
    $("#" + id).toggle(0);
    $("#" + id).toggle("scale", {
        direction: "both"
    }, callback);
}

function labelFormatter(context) {
    var sum = 0;
    for (var i in chartOptions["data"]["content"]) {
        sum += chartOptions["data"]["content"][i]["value"];
    }
    var percent = chartOptions["data"]["content"][context.index]["value"] / sum;
    var length = chartOptions["data"]["content"].length;

    if (percent < 0.08) {
        if (context.section === "outer") {
            return context.part === "mainLabel" ? context.index === length - 1 ? context.label : _textFurter : formatPercent(context.label) + " %";
        }
        if (context.section === "inner") {
            return "";
        }
    }
    if (context.section === "outer") return "";
    if (context.part === "mainLabel") return chartOptions["data"]["content"][context.index]["label"];
    if (context.part === "percentage") return formatPercent(context.label) + " %";
    return context.label;
}

function clickedListener(data) {
    data = data["data"];

    var id;
    // Opend group - Either accounts or currencies
    if (data["isGrouped"]) {
        id = "overlay_" + $(".overlay").length + 1;
        var createGroupedGraph = function () {
            var numAccounts = 0;
            // Opend group of currencies
            if (data["groupedData"][0].hasOwnProperty("accounts")) {
                for (var i in data["groupedData"]) {
                    numAccounts += data["groupedData"][i]["accounts"].length;
                }
                chartOptions["header"]["title"].text = formatNumber(data["value"]); //
                chartOptions["header"]["subtitle"].text = data["groupedData"].length === 1 ? _textCurrencies[0] : data["groupedData"].length + " " + _textCurrencies[1];
                // Opend further accounts
            } else {
                numAccounts = data["groupedData"].length;
                chartOptions["header"]["subtitle"].text = numAccounts === 1 ? _textAccounts[0] : numAccounts + " " + _textAccounts[1];
                chartOptions["header"]["title"].text = formatNumber(data["value"]);
            }
            chartOptions["data"]["content"] = data["groupedData"];
            var div = document.getElementById(id);
            chartOptions["size"]["canvasWidth"] = div.offsetWidth;
            chartOptions["size"]["canvasHeight"] = div.offsetHeight;
            chartOptions["data"]["smallSegmentGrouping"].enabled = groupData(data["groupedData"]);

            // ReSharper disable once InconsistentNaming
            // ReSharper disable once ConstructorCallNotUsed
            new d3pie(id, chartOptions);
        };

        showOverlay(id, function () {
            createGroupedGraph();

            $("#" + id).resize(function () {
                clearChartArea(id);
                createGroupedGraph();
            });
        });
    }
    // Directly selected one currency form overview
    else if (data.hasOwnProperty("accounts")) {
        id = "overlay_" + $(".overlay").length + 1;

        if (data["accounts"].length === 1) {
            // ReSharper disable once PossiblyUnassignedProperty
            // ReSharper disable once UseOfImplicitGlobalInFunctionScope
            Native.OpenView(data["accounts"][0]["id"]);
        } else {
            var createAccountsGraph = function () {
                chartOptions["header"]["title"].text = formatNumber(data["value"]);
                chartOptions["header"]["subtitle"].text = data["accounts"].length === 1
                    ? _textAccounts[0]
                    : data["accounts"].length + " " + _textAccounts[1];
                chartOptions["data"]["content"] = data["accounts"];
                var div = document.getElementById(id);
                chartOptions["size"]["canvasWidth"] = div.offsetWidth;
                chartOptions["size"]["canvasHeight"] = div.offsetHeight;
                chartOptions["data"]["smallSegmentGrouping"].enabled = groupData(data["accounts"]);

                // ReSharper disable once InconsistentNaming
                // ReSharper disable once ConstructorCallNotUsed
                new d3pie(id, chartOptions);
            };

            showOverlay(id,
                function () {
                    createAccountsGraph();

                    $("#" + id).resize(function () {
                        clearChartArea(id);
                        createAccountsGraph();
                    });
                });
        }
    } else {
        // ReSharper disable once PossiblyUnassignedProperty
        // ReSharper disable once UseOfImplicitGlobalInFunctionScope
        Native.OpenView(data["id"]);
    }
}

function groupData(data) {
    if (data.length < 3) return false;
    var sum = 0, i;
    for (i in data) {
        sum += data[i]["value"];
    }
    var countBelow5Percent = 0;
    for (i in data) {
        if (0.08 * sum > data[i]["value"]) {
            countBelow5Percent += 1;
        }
    }
    return countBelow5Percent > 1;
}

function formatNumber(inputNumber) {
    var number = _roundNumbers ? Math.round(inputNumber * 100) / 100 : Math.floor(inputNumber * 100) / 100;
    return number.toLocaleString(_culture) + " " + _currencyCode;
}

function formatPercent(inputNumber) {
    var number = Math.floor(inputNumber * 100) / 100;
    return number.toLocaleString(_culture);
}

function clearChartArea(id) {
    var chartArea = document.getElementById(id);
    chartArea.removeChild(chartArea.lastChild);

}
