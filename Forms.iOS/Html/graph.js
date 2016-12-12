"use strict";

/* Disable scrolling with keys */
window.addEventListener("keydown", function (e) {
    // space, startupPage up, startupPage down and arrow keys:
    if ([32, 33, 34, 37, 38, 39, 40].indexOf(e.keyCode) > -1) {
        e.preventDefault();
    }
}, false);

/* Chart settings */
Chart.defaults.global.responsive = true;
Chart.defaults.global.legend.position = "none";
Chart.defaults.global.legend.labels.fontFamily = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";

/* Graph colors */
var colors = [
        "#5DA5DA", "#FAA43A", "#60BD68", "#ed7d55",
        "#F17CB0", "#B2912F", "#B276B2", "#DECF3F",
        "#F15854", "#D444D4", "#0b7891", "#811b1b",
        "#0BD686", "#17CB0F", "#3a2db4", "#a23e0a",
        "#3CF3FD", "#15854F", "#838383", "#8e2258",
        "#8b0d0d", "#411a41", "#57801f", "#03b261",
        "#09171a", "#4D4D4D"
];


/* Global variables for labels, data and the chart */
var labels;
var data;
var chart;

/* Display the graph */
function displayGraph(l, d, color) {

    document.body.style.backgroundColor = color;

    labels = l;
    data = d;

    resize();
    generateGraph();
}

function generateGraph() {

    if (data != undefined) {

        /* Remove children*/
        var canvas = document.getElementById("chart");
        var node = document.getElementById("graphWrapper");
        while (node.hasChildNodes()) {
            node.removeChild(node.firstChild);
        }
        node.appendChild(canvas);

        var ctx = document.getElementById("chart");

        if (chart !== undefined) {
            chart.destroy();
        }
        chart = new Chart(ctx, {
            type: "pie",
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: colors,
                    borderColor: colors,
                    borderWidth: 1
                }]
            },
            options: {
                tooltips: false,
                cutoutPercentage: 10,
                animation: {
                    duration: 0
                }
            }
        });

        document.getElementById("chart").onclick = function (e) {
            var activePoint = chart.getElementAtEvent(e);
            Native("selectedCallback", activePoint[0]._index);
        };

        document.getElementById("legend").innerHTML = chart.generateLegend();
    }
}

function resize() {
    var wrapper = document.getElementById("graphWrapper");
    var legend = document.getElementById("legend");

    var h = document.body.offsetHeight - legend.offsetHeight - 40;

    if (h < document.body.offsetWidth * 0.8) {
        wrapper.setAttribute("style", "width: " + h + ";");
    } else {
        wrapper.removeAttribute("style");
    }
}

/* When resized */
window.onresize = function (event) {
    resize();
    generateGraph();
};