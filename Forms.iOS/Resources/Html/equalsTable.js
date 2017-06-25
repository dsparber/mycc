"use strict";

var sorterSet = false;

function setHeader(columns) {
    var table = document.getElementById("coinTable");
    table.deleteTHead();
    var header = table.createTHead();
    var row = header.insertRow();
    row.insertCell(-1);
    for (var i = 0; i < columns.length; i++) {
        var cell = row.insertCell(-1);
        cell.innerHTML = "<span>" + columns[i]["Text"] + "</span>";
        cell.setAttribute("class", columns[i]["Ascending"] === true ? "down" : columns[i]["Ascending"] === false ? "up" : "");
        cell.onclick = headerClicked(columns[i]["Id"]);
    }
}

function updateTable(data) {

    var coinTable = document.getElementById("coinTable");
    clearTable(coinTable);

    coinTable = coinTable.getElementsByTagName("tbody")[0];

    for (var i = 0; i < data.length; i++) {
        var row = coinTable.insertRow(i);
        var firstCell = row.insertCell(0);
        var amountCell = row.insertCell(1);
        var codeCell = row.insertCell(2);

        var rateHtml = "<div><span>" + data[i]["Rate"].substring(0, data[i]["Rate"].length - 5) + "</span><span>" + data[i]["Rate"].substring(data[i]["Rate"].length - 5) + "</span></div>";
        var amountHtml = "<div><span>" + data[i]["Amount"].substring(0, data[i]["Amount"].length - 5) + "</span><span>" + data[i]["Amount"].substring(data[i]["Amount"].length - 5) + "</span></div>";

        firstCell.innerHTML = "<div>" + (!data[i]["Expanded"] ? "" : "<div>x</div>") + "<div>=</div></div>";
        amountCell.innerHTML = "<div>" + (!data[i]["Expanded"]  ? "" : rateHtml) + amountHtml + "</div>";
        codeCell.innerHTML = "<div><div>" + data[i]["Code"] + "</div></div>";
    }
    sizeAllocated();
}

function headerClicked(id) {
    return function () {
        window.open("http://none?" + "HeaderClickedCallback=" + id);
    };
}

function sizeAllocated() {
    window.open("http://none?" + "CallbackSizeAllocated=" + document.getElementById("coinTable").offsetHeight);
}

function clearTable(table) {
    var rows = table.rows;
    var i = rows.length;
    while (--i) {
        rows[i].parentNode.removeChild(rows[i]);
    }
}
