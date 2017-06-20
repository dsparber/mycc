"use strict";

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
        cell.setAttribute("class", columns[i]["Ascending"] === true ? "down" : columns[i]["Ascending"] === false ? "up" : "");
        cell.onclick = headerClicked(columns[i]["Id"]);
    }
    row.insertCell(-1);
}

function updateTable(data) {

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
    sizeAllocated();
}

function rowClicked(code) {
    return function () {
        window.open("http://none?" + "Callback=" + code);
    };
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
