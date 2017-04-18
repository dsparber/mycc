"use strict";

var testData = [{
    "Amount": "750.12345678",
    "Code": "EUR",
    "Rate": "0,12345678"
}, {
    "Amount": "850.12345678",
    "Code": "USD",
    "Rate": "0,12345678"
}, {
    "Amount": "325750.12345678",
    "Code": "DOGE",
    "Rate": "0,12345678"
}];

var testColumns = [{
    "Text": "Betrag",
    "Type": "Amount"
}, {
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
    var row = header.insertRow();
    row.insertCell(-1);
    for (var i = 0; i < columns.length; i++) {
        var cell = row.insertCell(-1);
        cell.innerHTML = "<span>" + columns[i]["Text"] + "</span>";
        cell.setAttribute("type", columns[i]["Type"]);
        cell.onclick = headerClicked(columns[i]["Type"]);
    }
}

function updateTable(data, sort, hideRate) {

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

        firstCell.innerHTML = "<div>" + (hideRate ? "" : "<div>x</div>") + "<div>=</div></div>";
        amountCell.innerHTML = "<div>" + (hideRate ? "" : rateHtml) + amountHtml + "</div>";
        codeCell.innerHTML = "<div><div>" + data[i]["Code"] + "</div></div>";
    }

    $("#coinTable thead").children().removeClass();
    $("#coinTable td[type=" + sort["Type"] + "]").addClass(("Ascending" === sort["Direction"]) ? "down" : "up");

    sizeAllocated();
}

function headerClicked(type) {
    return function () {
        // ReSharper disable once UndeclaredGlobalVariableUsing
        window.open("http://none?" + "HeaderClickedCallback=" + type);
    };
}

function sizeAllocated() {
    // ReSharper disable once UndeclaredGlobalVariableUsing
    window.open("http://none?" + "CallbackSizeAllocated=" + document.getElementById("coinTable").offsetHeight);
}

function clearTable(table) {
    var rows = table.rows;
    var i = rows.length;
    while (--i) {
        rows[i].parentNode.removeChild(rows[i]);
    }
}
