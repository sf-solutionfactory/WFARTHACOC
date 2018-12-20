$(document).ready(function () {

});

$(window).on('load', function () {
    if ($("#TSOL_ID").val() === "SCO") {
        var e = $('#norden_compra').val();
        $.ajax({
            type: "POST",
            url: '../getEKKOInfo',
            dataType: "json",
            data: { "ebeln": e },
            success: function (data) {
                var ekko = data.ekmo;
                var cuentas = data.res;
                var mtr = data.mtr;
                var brtwr = data.brtwr;
                llenarTablaOc(ekko, cuentas, mtr, brtwr);
            }
        });
    }
});

function llenarTablaOc(a, b, mtr, brtwr) {
    var tabl = $('#tableOC').DataTable();
    //Limpio primero la tabla
    $("#tableOC tbody tr[role='row']").each(function () {
        var _t = $(this);
        tabl.row(_t).remove().draw(false);
    });
    var c = b.split('?');
    //Añado los datos
    tabl.row.add([
        toShow(brtwr),
        a.RETPC,  //%ret             
        a.DPPCT//,//%deposito
    ]).draw(false).node();
    alinearTOC();
}

function alinearTOC() {
    //--------
    //Para los titulos
    var t0 = $("#tableOC>thead>tr").find('th.BRTWR ');
    t0.css("text-align", "left");
    var t1 = $("#tableOC>thead>tr").find('th.FondoGarantia ');
    t1.css("text-align", "left");
    var t2 = $("#tableOC>thead>tr").find('th.AmortAnt');
    t2.css("text-align", "left");
    var t3 = $("#tableOC>thead>tr").find('th.MontoAntT');
    t3.css("text-align", "left");
    var t4 = $("#tableOC>thead>tr").find('th.AntAmort');
    t4.css("text-align", "left");
    var t5 = $("#tableOC>thead>tr").find('th.PorAnt');
    t5.css("text-align", "left");
    var t6 = $("#tableOC>thead>tr").find('th.AntSol');
    t6.css("text-align", "left");
    var t7 = $("#tableOC>thead>tr").find('th.AntTr');
    t7.css("text-align", "left");
    //--------
    $("#tableOC > tbody  > tr[role='row']").each(function () {
        //0
        var R0 = $(this).find("td.BRTWR");
        R0.css("text-align", "left");
        //1
        var R1 = $(this).find("td.FondoGarantia");
        R1.css("text-align", "left");
        //2
        var R2 = $(this).find("td.AmortAnt");
        R2.css("text-align", "left");
        //3
        var R3 = $(this).find("td.MontoAntT");
        R3.css("text-align", "left");
        //4
        var R4 = $(this).find("td.AntAmort");
        R4.css("text-align", "left");
        //5
        var R5 = $(this).find("td.PorAnt");
        R5.css("text-align", "left");
        //6
        var R6 = $(this).find("td.AntSol");
        R6.css("text-align", "left");
        //7
        var R7 = $(this).find("td.AntTr");
        R7.css("text-align", "left");
    });
}

function llenarCOC(tsol) {

    //Recupero el valor de la sociedad
    var buk = $("#SOCIEDAD_ID").val();
    if (tsol === "SCO") {
        if ($("#PAYER_ID").val() != "") {
            llenaOrdenes($("#D_PAYER_ID").val(), buk);
        }
    }
}

function llenaOrdenes(lifnr, bukrs) {
    var tr = $(this).closest('tr'); //Obtener el row
    var t = $('#table_infoP').DataTable();

    var pedidosNum = [];
    $("#norden_compra").empty();

    $.ajax({
        type: "POST",
        url: '../getPedidos',
        dataType: "json",
        data: { "lifnr": lifnr.trim(), "bukrs": bukrs },
        success: function (data) {
            $("#norden_compra").empty();
            for (var i = 0; i < data.length; i++) {
                var ebeln = data[i];
                $("#norden_compra").append($("<option>").attr('value', ebeln).text(ebeln));
            }

            var elem = document.querySelectorAll("select");
            var instance = M.Select.init(elem, []);
        }
    });
}