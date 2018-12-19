$(document).ready(function () {
    setTimeout(explode, 2500);
});
function explode() {
    //alert("Boom!");
    var url = window.location.href;
    url = url.replace('mensajes', 'home');
    //$(this).load(url);
    window.location.href = url;
}