'use strict';

var color = "gray";
var backgroundColor = "white";

function applyColor() {
    
    var buttons = document.getElementsByClassName("button");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].style = "";
    }
    
    var colored = document.getElementsByClassName("colored");
    for (var i = 0; i < colored.length; i++) {
        colored[i].style.borderColor = color;
        colored[i].style.backgroundColor = backgroundColor;
        colored[i].style.color = color;
    }
    
    var active = document.getElementsByClassName("active");
    for (var i = 0; i < active.length; i++) {
        active[i].style.backgroundColor = color;
        active[i].style.color = backgroundColor; 
        active[i].style.borderColor = color;
    }
}

function setColor(c, bc) {
    color = c;
    backgroundColor = bc;
    applyColor();
}

function setTabs(titles) {

    var control = document.getElementById("control");
    while (control.firstChild) {
        control.removeChild(control.firstChild);
    }
    
    for (var i = 0; i < titles.length; i++) {
        var button = document.createElement("div");
        button.className = "button";
        button.id = i;
        button.appendChild(document.createTextNode(titles[i])); 
        button.onclick = function () { buttonClicked(this); };
        control.appendChild(button);
    } 
    applyColor();
}

function buttonClicked(button){
    
    var buttons = document.getElementsByClassName("button");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].className = "button";
    }
        
    button.className += " active";
    notifyNative(button.id);
    applyColor();
}

function notifyNative(index) {
     Native("selectedCallback", index);
}