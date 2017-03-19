var _code;

function setCode(code) {

    _code = code;

    var size = document.body.clientHeight > document.body.clientWidth ? document.body.clientWidth : document.body.clientHeight;
    size -= 100;

    $( "div" ).empty();

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ConstructorCallNotUsed
    new QRCode("qrcode", {
        text: code,
        width: size,
        height: size,
        colorDark : "#000000",
        colorLight : "#ffffff",
        correctLevel : QRCode.CorrectLevel.H
    });
}

$( window ).resize(function() {
  setCode(_code);
});
