"use strict";

var sorterSet = false;

function setHeader(columns) {
    var table = document.getElementById("coinTable");
    table.deleteTHead();
    var header = table.createTHead();
    var row = header.insertRow(0);
    for (var i = 0; i < columns.length; i++) {
        var cell = row.insertCell(i);
        cell.innerHTML = "<span>" + columns[i]["Text"] + "</span>";
        cell.setAttribute("class", columns[i]["Ascending"] === true ? "down" : columns[i]["Ascending"] === false ? "up" : "");
        cell.onclick = headerClicked(columns[i]["Id"]);
    }
    row.insertCell(columns.length);
}

function updateTable(data) {

    var coinTable = document.getElementById("coinTable");
    clearTable(coinTable);

    coinTable = coinTable.getElementsByTagName("tbody")[0];

    for (var i = 0; i < data.length; i++) {
        var row = coinTable.insertRow(i);
        var nameCell = row.insertCell(0);
        var amountCell = row.insertCell(1);
        row.insertCell(2); // For Arrow
        amountCell.innerHTML = "<span>" + data[i]["Amount"].substring(0, data[i]["Amount"].length - 5) + "</span><span>" + data[i]["Amount"].substring(data[i]["Amount"].length - 5) + "</span>";
        nameCell.innerHTML = "<div><span>" + data[i]["Name"] + "</span></div>";

        if (data[i]["Disabled"]) {
            row.className = "disabled";
        }

        row.onclick = rowClicked(data[i]["Id"]);
    }

    sizeAllocated();
}

function rowClicked(id) {
    return function () {
        window.open("http://none?" + "Callback=" + id);
    };
}

function sizeAllocated() {
    window.open("http://none?" + "CallbackSizeAllocated=" + document.getElementById("coinTable").offsetHeight);
}

function headerClicked(id) {
    return function () {
        window.open("http://none?" + "HeaderClickedCallback=" + id);
    };
}

function clearTable(table) {
    var rows = table.rows;
    var i = rows.length;
    while (--i) {
        rows[i].parentNode.removeChild(rows[i]);
    }
}
