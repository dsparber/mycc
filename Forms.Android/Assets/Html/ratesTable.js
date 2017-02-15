"use strict";

var testData = [{
    "Code": "BTC",
    "Reference": 213.96
}, {
    "Code": "ETH",
    "Reference": "347,852.23"
}, {
    "Code": "PVC",
    "Reference": 478.54
}];

var testColumns = [{
    "Text": "Währung",
    "Type": "Currency"
}, {
    "Text": "Entspricht",
    "Type": "Value"
}];

var testSort = {
    "Type": "Currency",
    "Direction": "Ascending"
};


var sorterSet = false;

function setHeader(columns) {
    var table = document.getElementById("coinTable");
    table.deleteTHead();
    var header = table.createTHead();
    var row = header.insertRow(0);
    row.insertCell(0);
    for (var i = 0; i < columns.length; i++) {
        var cell = row.insertCell(-1);
        cell.innerHTML = "<span>" + columns[i]["Text"] + "</span>";
        cell.setAttribute("type", columns[i]["Type"]);
        cell.onclick = headerClicked(columns[i]["Type"]);
    }
    row.insertCell(-1);
}

function updateTable(data, sort) {

    var coinTable = document.getElementById("coinTable");
    clearTable(coinTable);

    coinTable = coinTable.getElementsByTagName("tbody")[0];

    for (var i = 0; i < data.length; i++) {
        var row = coinTable.insertRow(i);
        var numberCell = row.insertCell(0);
        var codeCell = row.insertCell(1);
        var referenceCell = row.insertCell(2);
        row.insertCell(3); // For Arrow
        numberCell.innerHTML = "1";
        codeCell.innerHTML = data[i]["Code"];
        referenceCell.innerHTML = data[i]["Reference"];

        row.onclick = rowClicked(data[i]["CallbackString"]);
    }

    $("#coinTable thead").children().removeClass();
    $("#coinTable td[type=" + sort["Type"] + "]").addClass(sort["Direction"] === "Ascending" ? "down" : "up");

    sizeAllocated();
}

function rowClicked(code) {
    return function () {
        Native("Callback", code);
    };
}

function headerClicked(type) {
    return function () {
        Native("HeaderClickedCallback", type);
    };
}

function sizeAllocated() {
    Native("CallbackSizeAllocated", document.getElementById("coinTable").offsetHeight);
}

function clearTable(table) {
    var rows = table.rows;
    var i = rows.length;
    while (--i) {
        rows[i].parentNode.removeChild(rows[i]);
    }
}
