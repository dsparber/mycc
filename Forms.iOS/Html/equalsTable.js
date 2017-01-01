"use strict";

var testData = [{
    "Amount": "750.45",
    "Code": "EUR"
}, {
  "Amount": "850.45",
  "Code": "USD"
}, {
  "Amount": "325750.45",
  "Code": "DOGE"
}];

var testColumns = [{
    "Text": "Referenz",
    "Type": "Reference"
}, {
    "Text": "Entspricht",
    "Type": "Equals"
},{
    "Text": "Währung",
    "Type": "Code"
}];

var testSort = {
    "Type": "Code",
    "Direction": "Ascending"
};


var sorterSet = false;

function setHeader(columns) {
    var table = document.getElementById("coinTable");
    table.deleteTHead();
    var header = table.createTHead();
    var row = header.insertRow(i);
    for (var i = 0; i < columns.length; i++) {
        var cell = row.insertCell(-1);
        cell.innerHTML = "<span>" + columns[i]["Text"] + "</span>";
        cell.setAttribute("type", columns[i]["Type"]);
        cell.onclick = headerClicked(columns[i]["Type"])
    }
}

function updateTable(data, sort) {

    var coinTable = document.getElementById("coinTable");
    clearTable(coinTable);

    coinTable = coinTable.getElementsByTagName("tbody")[0];

    for (var i = 0; i < data.length; i++) {
        var row = coinTable.insertRow(i);
        var codeCell = row.insertCell(0);
        var amountCell = row.insertCell(1);
        amountCell.innerHTML = data[i]["Amount"];
        codeCell.innerHTML = data[i]["Code"];

        row.onclick = rowClicked(data[i]["Code"]);
    }

    $("#coinTable thead").children().removeClass();
    $("#coinTable td[type=" + sort["Type"] + "]").addClass((sort["Direction"] == "Ascending") ? "down" : "up");
}

function rowClicked(code) {
    return function () {
        Native("Callback", code);
    }
}

function headerClicked(type) {
    return function () {
        Native("HeaderClickedCallback", type);
    }
}

function clearTable(table) {
    var rows = table.rows;
    var i = rows.length;
    while (--i) {
        rows[i].parentNode.removeChild(rows[i]);
    }
}
