var chartOptions = {
    "header": {
        "title": {
            "color": "#fff",
            "fontSize": 18,
            "font": "arial"
        },
        "subtitle": {
            "color": "#999",
            "fontSize": 12,
            "font": "arial"
        },
        "location": "pie-center",
        "titleSubtitlePadding": 2
    },
    "size": {
        "pieInnerRadius": "67%",
        "pieOuterRadius": "50%"
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
            "format": "label-value2",
            "pieDistance": 20
        },
        "inner": {
            "format": "percentage",
        },
        "mainLabel": {
            "color": "#fff",
            "font": "arial",
            "fontSize": 12
        },
        "percentage": {
            "color": "#fff",
            "font": "arial",
            "fontSize": 12,
            "decimalPlaces": 0
        },
        "value": {
            "color": "#999",
            "font": "arial",
            "fontSize": 12
        },
        "lines": {
            "enabled": true,
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
            "segmentStroke": "rgb(36, 39, 46)"
        }
    },
    "callbacks": {
        "onClickSegment": clickedListener
    }
};

var _textAccounts;
var _textCurrencies;
var _textFurter;

function showChart(data, textAccounts, textCurrencies, textFurther, textNoData) {

    _textAccounts = textAccounts;
    _textCurrencies = textCurrencies;
    _textFurter = textFurther;

    if (data.length == 0) {
        document.getElementById("pieChart").innerHtml = textNoData;
    } else {
        createMainGraph(data);

        $("#pieChart").resize(function () {
            createMainGraph(data);
        });
    }
}

function createMainGraph(data) {

    var chartArea = document.getElementById("pieChart");
    while (chartArea.firstChild) {
        chartArea.removeChild(chartArea.firstChild);
    }

    chartOptions["data"]["content"] = data;
    chartOptions["data"]["smallSegmentGrouping"].enabled = groupData(data);

    chartOptions["header"]["title"].text = data.length == 1 ? _textCurrencies[0] : data.length + " " + _textCurrencies[1];
    var numAccounts = 0;
    for (var i in data) {
        numAccounts += data[i]["accounts"].length;
    }
    chartOptions["header"]["subtitle"].text = numAccounts == 1 ? _textAccounts[0] : numAccounts + " " + _textAccounts[1];

    var div = document.getElementById("pieChart");
    chartOptions["size"]["canvasWidth"] = div.offsetWidth;
    chartOptions["size"]["canvasHeight"] = div.offsetHeight;

    var pie = new d3pie("pieChart", chartOptions);
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
    if (context.value != chartOptions["data"]["content"][context.index]["value"]) {
        if (context.section == "outer" && context.part == "mainLabel") {
            return _textFurter;
        }
        if (context.section == "inner") {
            return "";
        }
    }
    if (context.section == "outer" && context.part == "value") {
        return "\u2248 " + chartOptions["data"]["content"][context.index]["reference"];
    }
    if (context.section == "outer" && context.part == "mainLabel") {
        return chartOptions["data"]["content"][context.index]["money"];
    }
    return chartOptions["data"]["content"][context.index]["label"];
};

function clickedListener(data) {
    data = data["data"];

    if (data["isGrouped"]) {
        var id = "overlay_" + $('.overlay').length + 1;
        var createGroupedGraph = function () {
            if (data["groupedData"][0].hasOwnProperty("accounts")) {
                var numAccounts = 0;
                for (var i in data["groupedData"]) {
                    numAccounts += data["groupedData"][i]["accounts"].length;
                }
                chartOptions["header"]["title"].text = data["groupedData"].length == 1 ? _textCurrencies[0] : data["groupedData"].length + " " + _textCurrencies[1];
                chartOptions["header"]["subtitle"].text = numAccounts == 1 ? _textAccounts[0] : numAccounts + " " + _textAccounts[1];
            } else {
                var numAccounts = data["groupedData"].length;
                chartOptions["header"]["subtitle"].text = numAccounts == 1 ? _textAccounts[0] : numAccounts + " " + _textAccounts[1];
            }
            chartOptions["data"]["content"] = data["groupedData"];
            var div = document.getElementById(id);
            chartOptions["size"]["canvasWidth"] = div.offsetWidth;
            chartOptions["size"]["canvasHeight"] = div.offsetHeight - 51;
            chartOptions["data"]["smallSegmentGrouping"].enabled = groupData(data["groupedData"]);

            var pie = new d3pie(id, chartOptions);
        };

        showOverlay(id, function () {
            createGroupedGraph();

            $("#" + id).resize(function () {
                clearChartArea(id);
                createGroupedGraph();
            });
        });
    } else if (data.hasOwnProperty("accounts")) {
        var id = "overlay_" + $('.overlay').length + 1;

        if (data["accounts"].length == 1) {
            Native("selectedCallback", data["accounts"][0]["id"]);
        } else {
            var createAccountsGraph = function () {
                chartOptions["header"]["title"].text = data["name"];
                chartOptions["header"]["subtitle"].text = data["accounts"].length == 1 ? _textAccounts[0] : data["accounts"].length + " " + _textAccounts[1];
                chartOptions["data"]["content"] = data["accounts"];
                var div = document.getElementById(id);
                chartOptions["size"]["canvasWidth"] = div.offsetWidth;
                chartOptions["size"]["canvasHeight"] = div.offsetHeight - 51;
                chartOptions["data"]["smallSegmentGrouping"].enabled = groupData(data["accounts"]);

                var pie = new d3pie(id, chartOptions);
            };

            showOverlay(id, function () {
                createAccountsGraph();

                $("#" + id).resize(function () {
                    clearChartArea(id);
                    createAccountsGraph();
                });
            });
        }
    } else
        Native("selectedCallback", data["id"]);
};

function groupData(data) {
    var sum = 0;
    for (var i in data) {
        sum += data[i]["value"];
    }
    var countBelow5Percent = 0;
    for (var i in data) {
        if (0.05 * sum > data[i]["value"]) {
            countBelow5Percent += 1;
        }
    }
    return countBelow5Percent > 1;
}

function clearChartArea(id) {
    var chartArea = document.getElementById(id);
    chartArea.removeChild(chartArea.lastChild);

}
