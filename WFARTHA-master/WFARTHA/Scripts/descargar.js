function descargar(id) {
    var cont = id.split('-')[1];
    //var link = document.getElementById("desc-" + cont).value;
    //alert(link);
    ////alert(tsol + soci + pais);
    ////$.ajax({
    ////    url: "Descargar",
    ////    type: "POST",
    ////    async: false,
    ////    timeout: 30000,
    ////    dataType: "json",
    ////    data: { archivo: link },
    ////    success: function (data) {

    ////    },
    ////    error: function (error) {

    ////    }

    ////});
    //window.location = link;
    document.getElementById("lbl_cargar-" + cont).click();
}

function borraDescarga(id) {
    var cont = id.split('-')[1];
    document.getElementById("desc-" + cont).classList.add("hidden");
    document.getElementById("sube-" + cont).classList.remove("hidden");
    var a = document.getElementById("file_" + cont).classList.contains("ne");
    if (a)
        var a = document.getElementById("file_" + cont).classList.add("nec");
    var val = document.getElementById("txt_sop_borr").value;
    val = val + cont + ",";
    document.getElementById("txt_sop_borr").value = val;
}