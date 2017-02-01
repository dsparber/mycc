$(document).ready(function () {
    var h4s = $("h4");
    for (var i = 0; i < h4s.length; i++) {
        $(h4s[i]).next().toggle();
        $(h4s[i]).click(function () {
            $header = $(this);
            //getting the next element
            $content = $header.next();
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