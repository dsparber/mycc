$(document).ready(function () {
    var h4 = $("h4");
    for (var i = 0; i < h4.length; i++) {
        $(h4[i]).next().toggle();
        $(h4[i]).click(function () {
            var $header = $(this);
            //getting the next element
            var $content = $header.next();
            // icon
            var icon = $content.css('display') === "none" ? "collapse.png" : "expand.png";
            $header.css("background-image", "url(\"" + icon + "\")");
            //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
            $content.slideToggle(300, function () {
                //execute this after slideToggle is done
            });
        });
    }
});