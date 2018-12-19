
var hide = getCookie('menu.hide');
if (hide == 'true') {
    //var c = document.getElementById("container").style.paddingLeft = '0px';
    var c = document.querySelectorAll("div.container");
    for (var i = 0; i < c.length; i++) {
        c[i].style.paddingLeft = '0px';
    }
    document.getElementById('slide-out').style.transform = 'translateX(-105%)';
    document.getElementById('div-menu').style.width = '100px';
} else {
    //var c = document.getElementById("container").style.paddingLeft = '300px';
    var c = document.querySelectorAll("div.container");
    for (var i = 0; i < c.length; i++) {
        c[i].style.paddingLeft = '300px';
    }
    document.getElementById('slide-out').style.transform = 'translateX(0%)';
    document.getElementById('div-menu').style.width = '320px';
}