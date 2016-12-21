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
var borderColors = [
        "#5DA5DA", "#FAA43A", "#60BD68", "#ed7d55",
        "#F17CB0", "#B2912F", "#B276B2", "#DECF3F",
        "#F15854", "#D444D4", "#0b7891", "#811b1b",
        "#0BD686", "#17CB0F", "#3a2db4", "#a23e0a",
        "#3CF3FD", "#15854F", "#838383", "#8e2258",
        "#8b0d0d", "#411a41", "#57801f", "#03b261",
        "#09171a", "#4D4D4D"
];

var colors = [
        "rgba(93, 165, 218, 0.5)", "rgba(250, 164, 58, 0.5)", "rgba(96, 189, 104, 0.5)", "rgba(237, 125, 85, 0.5)",
        "rgba(241, 124, 176, 0.5)", "rgb(178, 145, 47, 0.5)", "rgb(178, 118, 178, 0.5)", "rgb(222, 207, 63, 0.5)",
        "rgb(241, 88, 84, 0.5)", "rgb(212, 68, 212, 0.5)", "rgb(11, 120, 145, 0.5)", "rgb(129, 27, 27, 0.5)",
        "rgb(11, 214, 134, 0.5)", "rgb(23, 203, 15, 0.5)", "rgb(58, 45, 180, 0.5)", "rgb(162, 62, 10, 0.5)",
        "rgb(60, 243, 253, 0.5)", "rgb(21, 133, 79, 0.5)", "rgb(131, 131, 131, 0.5)", "rgb(142, 34, 88, 0.5)",
        "rgb(139, 13, 13, 0.5)", "rgb(65, 26, 65, 0.5)", "rgb(87, 128, 31, 0.5)", "rgb(3, 178, 97, 0.5)",
        "rgb(9, 23, 26, 0.5)", "rgb(77, 77, 77, 0.5)"
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
                    borderColor: borderColors,
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