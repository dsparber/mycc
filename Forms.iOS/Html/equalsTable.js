"use strict";

var testData = [{
    "Amount": "750.45",
    "Code": "EUR",
    "Rate": "0,12345678"
}, {
  "Amount": "850.45",
  "Code": "USD",
  "Rate": "0,12345678"
}, {
  "Amount": "325750.45",
  "Code": "DOGE",
  "Rate": "0,12345678"
}];

var testColumns = [{
    "Text": "Betrag",
    "Type": "Amount"
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
    row.insertCell(-1);
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
        var xCell = row.insertCell(0);
        var amountCell = row.insertCell(1);
        var codeCell = row.insertCell(2);
        xCell.innerHTML = "<div><div>x</div><div>=</div></div>";
        amountCell.innerHTML = "<div><div>"+data[i]["Rate"]+"</div><div>"+data[i]["Amount"]+"</div></div>";
        codeCell.innerHTML = "<div>"+data[i]["Code"]+"</div>";

        row.onclick = rowClicked(data[i]["Code"]);
    }

    $("#coinTable thead").children().removeClass();
    $("#coinTable td[type=" + sort["Type"] + "]").addClass((sort["Direction"] == "Ascending") ? "down" : "up");

    sizeAllocated();
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
