"use strict";

var testData = [{
    "Code": "BTC",
    "Name": "Bitcoin",
    "Amount": 23525.2131,
    "Reference": 213.96
}, {
    "Code": "ETH",
    "Name": "Ethereum",
    "Amount": 986.2425,
    "Reference": 347852.23
}, {
    "Code": "PVC",
    "Name": "PuraVidaCoin",
    "Amount": 45845.8978,
    "Reference": 478.654
}];

var testColumns = [{
    "Text": "Währung",
    "Type": "Currency",
}, {
    "Text": "Anzahl",
    "Type": "Amount",
}, {
    "Text": "in EUR",
    "Type": "ReferenceCurrency",
}, ];

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
    for (var i = 0; i < columns.length; i++) {
        var cell = row.insertCell(i);
        cell.innerHTML = "<span>" + columns[i]["Text"] + "</span>";
        cell.setAttribute("type", columns[i]["Type"]);
        cell.setAttribute("class", columns[i]["XYZ"]);
        cell.onclick = headerClicked(columns[i]["Type"])
    }
    row.insertCell(columns.length);
}

function updateTable(data, sort, local) {

    var coinTable = document.getElementById("coinTable");
    clearTable(coinTable);

    coinTable = coinTable.getElementsByTagName("tbody")[0];

    for (var i = 0; i < data.length; i++) {
        var row = coinTable.insertRow(i);
        var codeCell = row.insertCell(0);
        var amountCell = row.insertCell(1);
        var referenceCell = row.insertCell(2);
        row.insertCell(3); // For Arrow
        codeCell.innerHTML = data[i]["Code"];
        amountCell.innerHTML = data[i]["Amount"].toLocaleString(local, {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
        referenceCell.innerHTML = data[i]["Reference"].toLocaleString(local, {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });

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
